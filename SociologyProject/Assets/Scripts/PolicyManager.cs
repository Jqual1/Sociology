using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;
using System.Linq;

public class PolicyManager : MonoBehaviour
{
    public static PolicyManager Instance { get; private set; }

    public Animator policyAnimator;
    public Canvas policyCanvas;
    public Canvas dialogueCanvas;
    private AudioSource clickSound;

    public GameObject policyTextBox;
    private Coroutine dialogueCo;

    private int openPolicy;
    public GameObject[] policyButtons;
    public GameObject enactButton;
    public Sprite[] notBoughtImages;
    public Sprite[] BoughtImages;

    private bool policyPurchasedThisRound = false;
    private bool badPurchase = false;
    private int highlightedPolicyNumber = -1;

    // Start is called before the first frame update
    void Start()
    {
        clickSound = gameObject.GetComponent<AudioSource>();
    }
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(Instance);

        }
        else
        {
            Destroy(gameObject);

        }
    }

    private string[,] policy = new string[,]
    {
        {"Extended Bus Routes",                     "Bus routes have a longer reach to pick up kids living further from schools"},
        {"Voucher System",                          "Allows public money to follow students to private schools"},
        {"FAFSA",                                   "Provides financial support for students pursuing higher education based on need."},
        {"Career and Technical Education Program",  "Establish an Office of Civil Rights in the CTE that works to close the discrimination gap in STEM fields."},
        {"Establish Magnet Schools",                "Allow for publicly funded schools that draw students from a variety of school districts under a specialized curriculum."},
        {"Federal Cultural Competency Training",    "A government funded program that will support schools with cultural competency training should they request it."},
        {"Title IX Training",                       "Require all schools to follow the guidelines of Title IX relating to discrimination based on sex."},
        {"After School Program",                    "Fund schools to host programs for students who may not be able to return home immediately after school."},
        {"School Resource Officer (SRO)",           "Give your school a School Resource Officer (SRO)"},
        {"Dress Code",                              "Enact a dress code policy"},
        {"Zero Tolerance Disciplin",                "Create a zero tolerance disoplinary policy"},
        {"Critical Conversation Space",             "Create Critical Conversation Spaces for students"},
        {"IQ testing",                              "Use IQ testing to select kids for gift education services"}
    };

    private List<String> capitals = new List<String>() { "Teachers", "Faculty", "Parents", "Students", "Community" };

    private int[,] policyCostN = new int[,] {
    // { teachers, faculty, parents, students, community, Maya}
      { 5, -5, 10, -100, 5, 10 },   // Extended Bus Routes
      { 10, -5, 5, 10, -5, 15 },   // Voucher System
      { 15, 5, 15, 15, 0, 15 },   // FAFSA
      { -10, -10, -15, 10, -5, 10 },   // Career and Technical Education Program
      { 10, -10, 5, 10, -5, 10 },   // Establish Magnet Schools
      { -25, -25, -10, 10, -5, 15 },   // Federal Cultural Competency Training
      { -10, -15, 10, 5, -10, 15 },   // Title IX Training
      { 10, -10, 20, -5, 10, 10 },   // After School Program
      { -15, -10, -10, -15, 10, -5 },   // School Resource Officer (SRO)
      { 20, 20, -10, -20, 0, -20 },   // Dress Code
      { 5, 15, -5, -25, 10, -20 },   // Zero Tolerance Disciplin
      { -10, 0, 10, 10, -10, 10 },   // Critical Conversation Space
      { 15, -5, -10, 10, 10, 15 }   // IQ testing
    };

    private Dictionary<int, int> imageDict = new Dictionary<int, int>()
    {
        [0] = 0,
        [1] = 1,
        [2] = 2,
        [3] = 2,
        [4] = 0,
        [5] = 1,
        [6] = 1,
        [7] = 2,
        [8] = 0

    };

    private int[] policyBenefit = new int[] { 1, 1, 2, 2, 2, 3, 2, 2, -1}; //Need to reavaluate these and double check them with progress requirements

    public bool[] policyPurchased = new bool[] { false, false, false, false, false, false, false, false, false, false, false, false, false};

    public void policyPReset()
	{
        policyPurchased = new bool[] { false, false, false, false, false, false, false, false, false, false, false, false, false };
        foreach (GameObject policyButton in policyButtons)
		{
            //ColorUtility.TryParseHtmlString("#737373", out Color color);
            policyButton.GetComponent<Image>().color = Color.white;
            resetPickedThisRound();
		}
        enactButton.SetActive(false);
    }

    public string getPolicyTitle(int policyNumber) { return policy[policyNumber, 0]; }
    public string getPolicyDescription(int policyNumber) { return policy[policyNumber, 1]; }
    public bool getPolicyPurchased(int policyNumber) { return policyPurchased[policyNumber]; }

    public void purchasePolicy()
    {
        bool canBuy = true;
        int[] currentCapital = GameController.Instance.capArr;
        int[] policyCapitalCost = GetRow(policyCostN, openPolicy);
        string tooLow = "";
        for(int i = 0; i<5; i++)
        {
            if (policyCapitalCost[i] < 0)
            {
                if (policyCapitalCost[i]*-1 > currentCapital[i])
                {
                    tooLow = capitals[i];
                    canBuy = false;
                    break;
                }
            }
        }
        if (canBuy && !policyPurchasedThisRound && !policyPurchased[openPolicy])
        {
            GameController.Instance.ChangeCapital(GetRow(policyCostN, openPolicy));
            GameController.Instance.ChangeProgress(new int[] { policyBenefit[openPolicy] });
            policyPurchased[openPolicy] = true;
            
            ColorUtility.TryParseHtmlString("#737373", out Color color);
            policyButtons[openPolicy].GetComponent<Image>().color = color;
            policyPurchasedThisRound = true;
            policyCanvas.GetComponent<PolicyController>().ClosePolicy();
        }
        else if (policyPurchased[openPolicy])
        {
            ExplainNoPurchase("You cannot purchase the same policy more than once!", false);
        }
        else if (!canBuy){
            ExplainNoPurchase(tooLow, true);
        }
    }
    public void ExplainNoPurchase(string reason, Boolean badRelations)
    {
        string explanation = "";
        if (badRelations)
        {
             explanation = "You cannot enact this policy since support of " + reason + " is too low.";
        }
        else
        {
             explanation = reason;
        }
        badPurchase = true;
        StopAllCoroutines();
        dialogueCo = StartCoroutine(typeText(explanation));
    }
    public void activePolicy(int policyNumber)
    {
        openPolicy = policyNumber;
    }

    public void PolicyClicked(int policyNumber)
    {   if (highlightedPolicyNumber != -1)
        {
            policyButtons[highlightedPolicyNumber].GetComponent<Image>().sprite = notBoughtImages[imageDict[highlightedPolicyNumber]];
        }
        StartDialogue(getPolicyDescription(policyNumber));
        GameController.Instance.UpdateChangeCap(GetRow(policyCostN, policyNumber));
        policyButtons[policyNumber].GetComponent<Image>().sprite = BoughtImages[imageDict[policyNumber]];
        highlightedPolicyNumber = policyNumber;
        enactButton.SetActive(true);
    }

    public void StartDialogue(string text)
    {
        StopAllCoroutines();
        dialogueCo = StartCoroutine(typeText(text));
    }

    IEnumerator typeText(string text)
    {
        policyTextBox.GetComponent<TextMeshProUGUI>().text = "";
        if (badPurchase)
        {
            Color c = new Color(233f/255f, 98f/255f, 98f/255f, 1);
            policyTextBox.GetComponent<TextMeshProUGUI>().color = c;
            badPurchase = false;
        }
        else
        {
            policyTextBox.GetComponent<TextMeshProUGUI>().color = Color.white;

        }
        foreach (char c in text.ToCharArray())
        {

            policyTextBox.GetComponent<TextMeshProUGUI>().text += c;
            while (GameController.Instance.gamePaused)
            {
                yield return null;
            }
            yield return new WaitForSeconds(.015f);
        }
    }
    public bool IsActive(string title)
    {
        for ( int i = 0; i < policyPurchased.Length; i++ )
		{
            if (policy[i,0].Equals(title))
			{
                return policyPurchased[i];
			}
		}
        return false;
    }

    public void PlayClickSound()
    {
        clickSound.Play();
    }


    //https://www.codegrepper.com/code-examples/csharp/select+a+whole+row+out+of+a+2d+array+C%23
    public int[] GetRow(int[,] matrix, int rowNumber)
    {
        return Enumerable.Range(0, matrix.GetLength(1))
                .Select(x => matrix[rowNumber, x])
                .ToArray();
    }


    public void resetPickedThisRound()
    {
        policyPurchasedThisRound = false;
    }
}
