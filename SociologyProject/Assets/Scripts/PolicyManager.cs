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

    private bool policyPurchasedThisRound = false;

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

    private int[,] policyCostN = new int[,] {
    // { teachers, faculty, parents, students, community, Maya}
      { 0, 0, 0, 0, 0, 0 },   // Extended Bus Routes
      { 0, 0, 0, 0, 0, 0 },   // Voucher System
      { 0, 0, 0, 0, 0, 0 },   // FAFSA
      { 0, 0, 0, 0, 0, 0 },   // Career and Technical Education Program
      { 0, 0, 0, 0, 0, 0 },   // Establish Magnet Schools
      { 10, 0, -20, -41, 0, 0 },   // Federal Cultural Competency Training
      { 0, 0, 0, 0, 0, 0 },   // Title IX Training
      { 0, 0, 0, 0, 0, 0 },   // After School Program
      { 0, 0, 0, 0, 0, 0 },   // School Resource Officer (SRO)
      { -11, 0, 0, -25, 0, 0 },   // Dress Code
      { 0, 0, 0, -25, 0, 0 },   // Zero Tolerance Disciplin
      { 0, 0, 0, 0, 0, 0 },   // Critical Conversation Space
      { 0, 0, 0, 0, 0, 0 }   // IQ testing
    };

    private int[] policyBenefit = new int[] { 5, 4, 2, 2, 2, 2, 3, 2, 1};

    public bool[] policyPurchased = new bool[] { false, false, false, false, false, false, false, false, false, false, false, false, false };

    public void policyPReset()
	{
        policyPurchased = new bool[] { false, false, false, false, false, false, false, false, false, false, false, false, false };
    }

    public string getPolicyTitle(int policyNumber) { return policy[policyNumber, 0]; }
    public string getPolicyDescription(int policyNumber) { return policy[policyNumber, 1]; }
    public bool getPolicyPurchased(int policyNumber) { return policyPurchased[policyNumber]; }


    public void OpenPolicy()
    {
        dialogueCanvas.enabled = false;
        policyCanvas.enabled = true;
        policyAnimator.SetBool("isOpen", true);
    }

    public void ClosePolicy()
    {
        dialogueCanvas.enabled = true;
        policyCanvas.enabled = false;
        policyAnimator.SetBool("isOpen", false);
    }

    public void purchasePolicy()
    {
        bool canBuy = true;
        int[] currentCapital = GameController.Instance.capArr;
        int[] policyCapitalCost = GetRow(policyCostN, openPolicy);
        for(int i = 0; i<5; i++)
        {
            if (policyCapitalCost[i] < 0)
            {
                if (policyCapitalCost[i]*-1 > currentCapital[i])
                {
                    canBuy = false;
                    break;
                }
            }
        }
        if (canBuy && !policyPurchasedThisRound)
        {
            GameController.Instance.ChangeCapital(GetRow(policyCostN, openPolicy));
            GameController.Instance.ChangeProgress(new int[] { policyBenefit[openPolicy] });
            policyPurchased[openPolicy] = true;
            policyButtons[openPolicy].GetComponent<Image>().color = Color.white;
            policyPurchasedThisRound = true;
        }
    }

    public void activePolicy(int policyNumber)
    {
        openPolicy = policyNumber;
    }

    public void PolicyClicked(int policyNumber)
    {
        StartDialogue(getPolicyDescription(policyNumber));
    }

    public void StartDialogue(string text)
    {
        StopAllCoroutines();
        dialogueCo = StartCoroutine(typeText(text));
    }

    IEnumerator typeText(string text)
    {
        policyTextBox.GetComponent<TextMeshProUGUI>().text = "";
        foreach (char c in text.ToCharArray())
        {

            policyTextBox.GetComponent<TextMeshProUGUI>().text += c;
            while (GameController.Instance.gamePaused)
            {
                yield return null;
            }
            yield return new WaitForSeconds(.03f);
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
        return policyPurchased[0];
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
