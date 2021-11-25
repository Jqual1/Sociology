using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using static PolicyObject;
using UnityEngine.Audio;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    // This is acting as GameManager
    public static GameController Instance { get; private set; }
    // Capital
    private int teachers, faculty, parents, students, community, Maya;
    public Image teaEmo, facEmo, parEmo, stuEmo, comEmo;
    public Sprite sadEmo, mehEmo, midEmo, okayEmo, happyEmo;
    public Image teaCha, facCha, parCha, stuCha, comCha;
    public Sprite doubleD, singleD, noChange, singleU, doubleU;
    public int[] capArr;
    public GameObject capitalScreen;
    private bool capChangeBool = false;

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

    // Sounds
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
        capArr = new int[]{ teachers, faculty, parents, students, community, Maya };
        for (int i = 0; i < 6; i++)
		{
            capArr[i] = 50;
		}
        UpdateCapital();
        progress = 0;
        PolicyObject policyObject = new PolicyObject();
        
        //if (macLineEndings) { delimeter = "\n"; }
        schoolPolicies = policyObject.ParsePolicies(policies, delimeter);

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
    public void StartGame()
    {
        //moneyGraphic.SetActive(true);
        mainMenu.SetActive(false);
        // TODO: Add assertion
        //curChapterIndex = 0;

        //chapters[curChapterIndex].SetActive(true);
        dialogueViewer.InitializeDialogue();

        List<Policy> activeSchoolPolicies = new List<Policy>();
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
        parents = 50;
        teachers = 50;
        faculty = 50;
        students = 50;
        community = 50;
    }

    public void EndGame()
    {
        endScreen.SetActive(true);
        mainMenu.SetActive(false);
        //moneyGraphic.SetActive(false);
    }

    // New system for changing capital
    public void ChangeCapital(int[] amount)
    {
        // REMINDER { teachers, faculty, parents, students, community, Maya }
        Debug.Log(amount[0]);
		for( int i = 0; i < 6; i++ )
		{
            capArr[i] += amount[i];
            if (capArr[i] > 100) { capArr[i] = 100; }
            if (capArr[i] < 0) { capArr[i] = 0; }
        }
        UpdateCapital();
    }
    public void UpdateCapital()
	{
        Image[] capEmoArr = new Image[] { teaEmo, facEmo, parEmo, stuEmo, comEmo };
        Sprite[] emoArr = new Sprite[] { sadEmo, mehEmo, midEmo, okayEmo, happyEmo };
		for( int i = 0; i < 5; i++ )
		{
			if (capArr[i] <= 20)
			{
                capEmoArr[i].sprite = emoArr[0];
			}
            else if (capArr[i] <= 40)
            {
                capEmoArr[i].sprite = emoArr[1];
            }
            else if (capArr[i] <= 60)
            {
                capEmoArr[i].sprite = emoArr[2];
            }
            else if (capArr[i] <= 80)
            {
                capEmoArr[i].sprite = emoArr[3];
            }
            else
            {
                capEmoArr[i].sprite = emoArr[4];
            }
        }

	}
    public void UpdateChangeCap(int[] capChange)
	{
        Image[] capEmoArr = new Image[] { teaCha, facCha, parCha, stuCha, comCha };
        Sprite[] emoChangeArr = new Sprite[] { doubleD, singleD, noChange, singleU, doubleU };
        for ( int i = 0; i < 5; i++ )
		{
            if( capChange[i] > 15)
			{
                capEmoArr[i].sprite = emoChangeArr[4];
			}
            else if (capChange[i] > 5)
            {
                capEmoArr[i].sprite = emoChangeArr[3];
            }
            else if (capChange[i] > -6)
            {
                capEmoArr[i].sprite = emoChangeArr[2];
            }
            else if (capChange[i] > -16)
            {
                capEmoArr[i].sprite = emoChangeArr[1];
            }
            else
            {
                capEmoArr[i].sprite = emoChangeArr[0];
            }
        }
	}
    public void ToggleCapital()
	{
        if(capitalScreen.activeInHierarchy)
		{
            capitalScreen.SetActive(false);
		}
        else
		{
            capitalScreen.SetActive(true);
		}
	}

    public void ToggleCapitalChange()
	{
        Image[] capEmoArr = new Image[] { teaCha, facCha, parCha, stuCha, comCha };
        for (int i = 0; i < 5; i++)
		{
            if(capChangeBool)
			{
                capEmoArr[i].transform.gameObject.SetActive(false);
			}
            else
            {
                capEmoArr[i].transform.gameObject.SetActive(true);
            }
        }
        capChangeBool = !capChangeBool;
    }

	public void ChangeProgress(int[] amount)
    {
        progress += amount[0];
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
    // Phase out all money-related things. 
    private bool IsEnoughMoney(Policy policy)
    {
        return policy.cost <= 1000;
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
        foreach ((string Name, int[] Value) action in policy.actions)
        {
            if (action.Name == "money")
            {
                ChangeCapital(action.Value);
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
