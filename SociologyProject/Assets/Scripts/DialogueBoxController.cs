using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class DialogueBoxController : MonoBehaviour
{
    //public Animator dialogueAnimator;
    private Animator dialogueAnimator;

    public GameObject dialogueSide;
    public GameObject choiceSide;
    public GameObject dialogueBox;
    public GameObject textBox;
    public GameObject policyButton;
    public GameObject nextButton;
    private AudioSource nextSound;
    public DialogueViewer dialogueViewer;

    private Coroutine dialogueCo;
    private bool typingComplete = true;
    private string fullMessage;

    /*void Awake()
    {
        HideAllButtons();
    }*/


    void Start()
    {
        dialogueAnimator = gameObject.GetComponent<Animator>();
        nextSound = gameObject.GetComponent<AudioSource>();
    }

    private void Update()
    {

    }

    public void OpenBox()
    {
        dialogueAnimator.SetBool("isOpen", true);

    }

    public void CloseBox()
    {
        textBox.GetComponent<Button>().interactable = false;
        dialogueAnimator.SetBool("isOpen", false);
    }

    public void StartDialogue(string text)
    {
        fullMessage = text;
        OpenBox();
        if (dialogueCo != null)
        {
            StopCoroutine(dialogueCo);
        }
        dialogueCo = StartCoroutine(typeText(text));
    }

    public void HideDialogue()
    {
        StopCoroutine(dialogueCo);
        typingComplete = true;

        CloseBox();
    }

    public void Next()
    {
        if (typingComplete)
        {
            textBox.GetComponent<Button>().interactable = true;
            dialogueViewer.Next();
            nextButton.SetActive(false);
        }
        else
        {
            textBox.GetComponent<Button>().interactable = false;
            StopCoroutine(dialogueCo);
            textBox.GetComponent<TextMeshProUGUI>().text = fullMessage;
            typingComplete = true;
            nextButton.SetActive(true);
        }
        PlayNextSound();
    }

    IEnumerator typeText(string text)
    {
        typingComplete = false;
        textBox.GetComponent<Button>().interactable = true;
        nextButton.SetActive(false);
        textBox.GetComponent<TextMeshProUGUI>().text = "";
        foreach (char c in text.ToCharArray())
        {
            textBox.GetComponent<TextMeshProUGUI>().text += c;
            // https://answers.unity.com/questions/904429/pause-and-resume-coroutine-1.html
            while (GameController.Instance.gamePaused)
            {
                yield return null;
            }
            yield return new WaitForSeconds(.03f);
        }
        typingComplete = true;
        textBox.GetComponent<Button>().interactable = false;
        nextButton.SetActive(true);
    }

    public void PlayNextSound()
    {
        nextSound.Play();
    }
}
