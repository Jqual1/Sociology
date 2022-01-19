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
    public GameObject settingsScreen;
    public GameObject sourcesScreen;
    
    public GameObject dialogueScreen;
    public GameObject policyScreen;

    public DialogueViewer dialogueViewer;
    public GameObject pauseMenu;
    public bool gamePaused = false;
    public bool macLineEndings = false;
    string delimeter = "\r\n";
    public List<Policy> activeSchoolPolicies = new List<Policy>();

    public PolicyManager policyManager;
    public SourcesScript sourcesScript;

    public List<Policy> schoolPolicies = new List<Policy>();
    [SerializeField] TextAsset policies;

    // Sounds
    public AudioMixer mixer;


    private int sourcesCount;

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
        ResetCapital();
        UpdateCapital();
        progress = 0;
        PolicyObject policyObject = new PolicyObject();
        
        //if (macLineEndings) { delimeter = "\n"; }
        schoolPolicies = policyObject.ParsePolicies(policies, delimeter);

        mainMenu.SetActive(true);
        pauseMenu.SetActive(false);
        endScreen.SetActive(false);
        settingsScreen.SetActive(false);
        sourcesScreen.SetActive(false);
        policyScreen.SetActive(false);
        sourcesCount = 0;
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
        dialogueScreen.SetActive(true);
        mainMenu.SetActive(false);
        capitalScreen.SetActive(true);
        policyScreen.SetActive(true);
        ResetCapital();
        // TODO: Add assertion
        //curChapterIndex = 0;

        //chapters[curChapterIndex].SetActive(true);
        dialogueViewer.InitializeDialogue();
        dialogueViewer.ClearCharacters();
        dialogueViewer.intro.SetActive(true);

        List<Policy> activeSchoolPolicies = new List<Policy>();
        progress = 0;

        // isPlaying = true;
        // TODO: Reset purchased policies
        dialogueViewer.HideAllChoices();
        dialogueViewer.dialogueBoxController.dialogueSide.SetActive(true);
        dialogueViewer.countdown.reset();
    }

    public void Pause()
    {
        Time.timeScale = 0;
        gamePaused = true;
        pauseMenu.SetActive(true);
    }

    public void Resume()
    {
        Time.timeScale = 1;
        gamePaused = false;
        pauseMenu.SetActive(false);
    }

    public void MainMenu()
    {
        foreach (GameObject background in dialogueViewer.backgrounds)
        {
            background.SetActive(false);
        }
        dialogueViewer.ClearCharacters();
        Resume();
        gamePaused = false;
        pauseMenu.SetActive(false);
        endScreen.SetActive(false);
        settingsScreen.SetActive(false);
        sourcesScreen.SetActive(false);
        sourcesCount = 0;
        capitalScreen.SetActive(false);
        dialogueScreen.SetActive(false);
        mainMenu.SetActive(true);
        policyScreen.SetActive(false);
    }

    public void EndGame()
    {
        endScreen.SetActive(true);
        mainMenu.SetActive(false);
        //moneyGraphic.SetActive(false);
    }
    
    public void Settings()
	{
        settingsScreen.SetActive(true);
        mainMenu.SetActive(false);
	}

    public void Sources()
    {
        sourcesScreen.SetActive(true);
        sourcesNext();
        mainMenu.SetActive(false);
    }
	public void ResetCapital()
	{
        progress = 0;
        capArr = new int[] { teachers, faculty, parents, students, community, Maya };
        for (int i = 0; i < 6; i++)
        {
            capArr[i] = 50;
        }
        capChangeBool = true;
        ToggleCapitalChange();
        UpdateCapital();
        policyManager.policyPReset();
	}

	// New system for changing capital
	public void ChangeCapital(int[] amount)
    {
        // REMINDER { teachers, faculty, parents, students, community, Maya }
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
    private int CheckChangeCap(int capChange, int i)
	{
        // Return ints
        int product = capArr[i] + capChange;
        if (capArr[i] <= 20)
        {
            if(product <= -20)
			{
                return 0;
			}
            if(product <= 0)
			{
                return 1;
			}
            if(product <= 20)
			{
                return 2;
			}
            if(product <= 40)
			{
                return 3;
			}
			else
			{
                return 4;
			}
        }
        else if (capArr[i] <= 40)
        {
            if (product <= 0)
            {
                return 0;
            }
            if (product <= 20)
            {
                return 1;
            }
            if (product <= 40)
            {
                return 2;
            }
            if (product <= 60)
            {
                return 3;
            }
            else
            {
                return 4;
            }
        }
        else if (capArr[i] <= 60)
        {
            if (product <= 20)
            {
                return 0;
            }
            if (product <= 40)
            {
                return 1;
            }
            if (product <= 60)
            {
                return 2;
            }
            if (product <= 80)
            {
                return 3;
            }
            else
            {
                return 4;
            }
        }
        else if (capArr[i] <= 80)
        {
            if (product <= 40)
            {
                return 0;
            }
            if (product <= 60)
            {
                return 1;
            }
            if (product <= 80)
            {
                return 2;
            }
            if (product <= 100)
            {
                return 3;
            }
            else
            {
                return 4;
            }
        }
        else
        {
            if (product <= 60)
            {
                return 0;
            }
            if (product <= 80)
            {
                return 1;
            }
            if (product <= 100)
            {
                return 2;
            }
            if (product <= 120)
            {
                return 3;
            }
            else
            {
                return 4;
            }
        }
    }
    public void UpdateChangeCap(int[] capChange)
	{
        Image[] capEmoArr = new Image[] { teaCha, facCha, parCha, stuCha, comCha };
        Sprite[] emoChangeArr = new Sprite[] { doubleD, singleD, noChange, singleU, doubleU };
        for ( int i = 0; i < 5; i++ )
		{
            int index = CheckChangeCap(capChange[i], i);
            capEmoArr[i].sprite = emoChangeArr[index];
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
        Image[] capEmoChaArr = new Image[] { teaCha, facCha, parCha, stuCha, comCha };
        for (int i = 0; i < 5; i++)
		{
            if(capChangeBool)
			{
                capEmoChaArr[i].transform.gameObject.SetActive(false);
			}
            else
            {
                capEmoChaArr[i].transform.gameObject.SetActive(true);
            }
        }
        capChangeBool = !capChangeBool;
    }

	public void ChangeProgress(int[] amount)
    {
        Debug.Log(amount[0]);
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
                Debug.Log("VALUE " + action.Value);
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

    // Sets the volume using the slider
    public void setVolume(float sliderValue)
    {
        mixer.SetFloat("masterVol", (Mathf.Log10(sliderValue) * 20));
    }
    public void setMusic(float sliderValue)
    {
        mixer.SetFloat("musicVol", (Mathf.Log10(sliderValue) * 20));
    }
    public void setOther(float sliderValue)
    {
        mixer.SetFloat("otherVol", (Mathf.Log10(sliderValue) * 20));
    }


    public void sourcesNext()
    {
        
        if (sourcesCount != sourcesScript.getSourceLength())
        {
            sourcesScreen.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = sourcesScript.getSource(sourcesCount);
            sourcesCount += 1;
        }
    }
}
