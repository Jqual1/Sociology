using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;

public class PolicyManager : MonoBehaviour
{
    public static PolicyManager Instance { get; private set; }

    public Animator policyAnimator;
    public Canvas policyCanvas;
    public Canvas dialogueCanvas;
    private AudioSource clickSound;

    //public GameObject moneyTextBox;

    public GameObject policyTextBox;
    private Coroutine dialogueCo;

    private int openPolicy;
    public GameObject[] policyButtons;

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

 // Old System that uses Money for the cost
    private int[] policyCost = new int[] {
        2000,
        1500,
        1000,
        2000,
        1500,
        2000,
        2000,
        1500,
        1000,
        //school policies
        2000,
        1500,
        1200,
        0,
        0,
        0,
        0,
        0,
        0};

    // Unsure why the school policies comments are in the middle of the code?
    // *Cameron said he didn't remember why, so we will keep them there for now.
    // Should we attempt to combine the different parts of each policy together in the code to make it more readable?
    // This works for now, but what if we want to make this better y'know?

    private string[,] policy = new string[,]
    {
        // I think it would be best to use a 2D array like this to better group everything together and make it more readable.
        // 2D Arrays are superior
        {"Free Lunch Program",                      "The government pays for free lunches for public schools"},
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
        {"IQ testing",                              "Use IQ testing to select kids for gift education services"},
        {"6",                                       "6"},
        {"7",                                       "7"},
        {"8",                                       "8"},
        {"9",                                       "9"}
    };
    // This will replace the money system
    // This is how to do 2D arrays in C#
    private int[,] policyCostN = new int[,] {
    //{ teachers, faculty, parents, students, community }
      { 0, 0, 0, 0, 0 },   // Free Lunch Program
      { 0, 0, 0, 0, 0 },   // Extended Bus Routes
      { 0, 0, 0, 0, 0 },   // Voucher System
      { 0, 0, 0, 0, 0 },   // FAFSA
      { 0, 0, 0, 0, 0 },   // Career and Technical Education Program
      { 0, 0, 0, 0, 0 },   // Establish Magnet Schools
      { 0, 0, 0, 0, 0 },   // Federal Cultural Competency Training
      { 0, 0, 0, 0, 0 },   // Title IX Training
      { 0, 0, 0, 0, 0 },   // After School Program
      { 0, 0, 0, 0, 0 },   // School Resource Officer (SRO)
      { 0, 0, 0, 0, 0 },   // Dress Code
      { 0, 0, 0, 0, 0 },   // Zero Tolerance Disciplin
      { 0, 0, 0, 0, 0 },   // Critical Conversation Space
      { 0, 0, 0, 0, 0 },   // IQ testing
      { 0, 0, 0, 0, 0 },   // 6
      { 0, 0, 0, 0, 0 },   // 7
      { 0, 0, 0, 0, 0 },   // 8
      { 0, 0, 0, 0, 0 }    // 9
    };

    private int[] policyBenefit = new int[] { 5, 4, 2, 2, 2, 2, 3, 2, 1};

    public bool[] policyPurchased = new bool[] { false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false };

    public void policyPReset()
	{
        policyPurchased = new bool[] { false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false };
    }

    public string getPolicyTitle(int policyNumber) { return policy[policyNumber, 0]; }
    public string getPolicyDescription(int policyNumber) { return policy[policyNumber, 1]; }
    public int getPolicyCost(int policyNumber) { return policyCost[policyNumber]; }
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
        if (GameController.Instance.money >= policyCost[openPolicy] && !policyPurchased[openPolicy])
        {
            GameController.Instance.ChangeMoney(policyCost[openPolicy] * -1);
            GameController.Instance.ChangeProgress(policyBenefit[openPolicy]);
            policyPurchased[openPolicy] = true;
            policyButtons[openPolicy].GetComponent<Image>().color = Color.white;
            Debug.Log("Purchased Policy " + openPolicy);
        }
    }

    public void activePolicy(int policyNumber)
    {
        openPolicy = policyNumber;
    }

    public void PolicyClicked(int policyNumber)
    {
        StartDialogue(getPolicyDescription(policyNumber));
        Debug.Log("hey I know the Policy Number: " + policyNumber);
    }

    public void StartDialogue(string text)
    {
        StopAllCoroutines();
        //GameController.Instance.StopAllCoroutines();
        dialogueCo = StartCoroutine(typeText(text));
    }

    IEnumerator typeText(string text)
    {
        Debug.Log(text);
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
        Debug.Log(title);
        return policyPurchased[Array.IndexOf(policy, title)];
    }

    public void PlayClickSound()
    {
        clickSound.Play();
    }
}
