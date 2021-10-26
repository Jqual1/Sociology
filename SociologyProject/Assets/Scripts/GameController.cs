using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using static PolicyObject;
using UnityEngine.Audio;

public class GameController : MonoBehaviour
{
    // This is acting as GameManager
    public static GameController Instance { get; private set; }
    public GameObject moneyTextBox;
    public int money;
    //capital
    private int parents;
    private int teachers;
    private int faculty;
    private int students;
    private int community;

    public int progress;
    public GameObject mainMenu;
    public GameObject endScreen;

    public DialogueViewer dialogueViewer;
    public GameObject pauseMenu;
    public bool gamePaused = false;
    public bool macLineEndings = false;
    string delimeter = "\r\n";
    public List<Policy> activeSchoolPolicies = new List<Policy>();

    public PolicyManager policyManager;

    public List<Policy> schoolPolicies = new List<Policy>();
    [SerializeField] TextAsset policies;

    //Sounds
    public AudioMixer mixer;
    public GameObject volumeButton;
    public GameObject volumeSlider;

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
    // Start is called before the first frame update
    void Start()
    {
        progress = 0;
        PolicyObject policyObject = new PolicyObject();
        
        //if (macLineEndings) { delimeter = "\n"; }
        //schoolPolicies = policyObject.ParsePolicies(policies, delimeter);
        foreach (Policy policy in schoolPolicies)
        {
            Debug.Log("Name: " + policy.name);
            Debug.Log("Cost: " + policy.cost);
            Debug.Log("--");

        }

        mainMenu.SetActive(true);
        pauseMenu.SetActive(false);
        endScreen.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        // Toggle pause menu
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (!gamePaused)
            {
                Pause();
            }
            else
            {
                Resume();
            }
        }
    }

    /*public GameObject GetCurChapter()
    {
        return chapters[curChapterIndex];
    }*/

    public void StartGame()
    {
        mainMenu.SetActive(false);
        // TODO: Add assertion
        //curChapterIndex = 0;

        //chapters[curChapterIndex].SetActive(true);
        dialogueViewer.InitializeDialogue();

        List<Policy> activeSchoolPolicies = new List<Policy>();
        money = 0;
        ChangeMoney(1000);
        progress = 0;

        // isPlaying = true;
        // TODO: Reset purchased policies
    }

    public void Pause()
    {
        gamePaused = true;
        pauseMenu.SetActive(true);
    }

    public void Resume()
    {
        gamePaused = false;
        pauseMenu.SetActive(false);
    }

    public void MainMenu()
    {
        gamePaused = false;
        pauseMenu.SetActive(false);
        endScreen.SetActive(false);
        mainMenu.SetActive(true);
        //policyScreen.SetActive(false);
        progress = 0;
        money = 1000;
        parents = 50;
        teachers = 50;
        faculty = 50;
        students = 50;
        community = 50;
        ChangeMoney(0);
    }

    public void EndGame()
    {
        endScreen.SetActive(true);
        mainMenu.SetActive(false);
        
    }

    // Old System, Will have to add in a new ChangeCapital
    public void ChangeMoney(int amount)
    {
        money += amount;
        moneyTextBox.GetComponent<TextMeshProUGUI>().text = money.ToString();
    }
    //new system for changing capital
    public void ChangeCapital(int[] amount)
    {
        // REMINDER { teachers, faculty, parents, students, community }

        teachers = teachers + amount[0];
        if (teachers > 100) { teachers = 100; }
        if (teachers < 0) { teachers = 0; }
        faculty = faculty + amount[1];
        if (faculty > 100) { faculty = 100; }
        if (faculty < 0) { faculty = 0; }
        parents = parents + amount[2];
        if (parents > 100) { parents = 100; }
        if (parents < 0) { parents = 0; }
        students = students + amount[3];
        if (students > 100) { students = 100; }
        if (students < 0) { students = 0; }
        community = community + amount[4];
        if (community > 100) { community = 100; }
        if (community < 0) { community = 0; }
    }

    public void ChangeProgress(int amount)
    {
        progress += amount;
    }

    public Policy FindPolicy(string name)
    {
        foreach (Policy policy in schoolPolicies)
        {
            if (policy.name == name)
            { 
                return policy;
            }
        }
        return null;
    }

    public bool IsActiveSchoolPolicy(string policyName)
    {
        foreach (Policy policy in activeSchoolPolicies)
        {
            if (policy.name == policyName)
            {
                return true;
            }
        }
        return false;
    }

    public bool IsAvailable(string policyName)
    {
        Policy policy = FindPolicy(policyName);
        return IsEnoughMoney(policy) && ConditionsMet(policy);
    }

    private bool IsEnoughMoney(Policy policy)
    {
        return policy.cost <= money;
    }

    private bool ConditionsMet(Policy policy)
    {
        foreach (string condition in policy.requires)
        {
            if (!PolicyManager.Instance.IsActive(condition)) 
            { 
                return false; 
            }
        }
        return true;
    }

    public void ActivatePolicy(Policy policy)
    {
        foreach ((string Name, int Value) action in policy.actions)
        {
            if (action.Name == "money")
            {
                ChangeMoney(action.Value);
            }
            else
            {
                ChangeProgress(action.Value);
            }
        }
        activeSchoolPolicies.Add(policy);
    }


    public IEnumerator LoadYourAsyncScene(string scene)
    {        
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(scene);

        while (!asyncLoad.isDone)
        {
            yield return null;
        }
    }

    // Volume Controls
    // Activates the volume slider by clicking the icon
    public void volumeOnClick()
    {
        if (volumeSlider.activeSelf == true)
        {
            volumeSlider.SetActive(false);
        }
        else
        {
            volumeSlider.SetActive(true);
        }
    }
    // Sets the volume using the slider
    public void setVolume(float sliderValue)
    {
        mixer.SetFloat("masterVol", (Mathf.Log10(sliderValue) * 20));
    }
}
