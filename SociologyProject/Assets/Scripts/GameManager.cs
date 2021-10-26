using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public GameObject dialogueManager;
    public GameObject events;

    public bool choiceA = false;
    public bool choiceB = false;
    public bool choiceC = false;
    public bool choiceD = false;
    public bool choiceE = false;
    public bool choiceF = false;
    public bool choiceG = false;
    public bool choiceH = false;
    public bool choiceI = false;
    public bool choiceJ = false;


    public GameObject chapteronebackground;
    public GameObject chaptertwobackground;
    public GameObject chapterthreebackground;
    public GameObject chapterfourbackground;
    public GameObject endscreen;
    public GameObject schoolbackground;
    public GameObject cutscene;

    public bool chap1;
    public bool chap1done;
    public bool chap2done;
    public bool chap3done;
    public bool chap4done;

    public Canvas dialoguecanvas;
    public Canvas canvas;
   
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(Instance);
            DontDestroyOnLoad(dialogueManager);
            DontDestroyOnLoad(events);
        }
        else
        {
            Destroy(gameObject);
            Destroy(dialogueManager);
            Destroy(events);
        }
    }
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {

    }

    public IEnumerator LoadYourAsyncScene(string scene)
    {
        // The Application loads the Scene in the background as the current Scene runs.
        // This is particularly good for creating loading screens.
        // You could also load the Scene by using sceneBuildIndex. In this case Scene2 has
        // a sceneBuildIndex of 1 as shown in Build Settings.

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(scene);

        // Wait until the asynchronous scene fully loads
        while (!asyncLoad.isDone)
        {
            yield return null;
        }
    }
}
