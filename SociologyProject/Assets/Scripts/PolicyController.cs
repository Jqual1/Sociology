using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PolicyController : MonoBehaviour
{
    public Animator policyAnimator;
    public GameObject staticElements;

    public DialogueViewer dialogueViewer;
    public Sprite[] notBoughtImages;
    public GameObject[] policyButtons;

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

    // Start is called before the first frame update
    void Start()
    {
        staticElements.SetActive(false);
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OpenPolicy()
    {
        
        policyAnimator.SetBool("isOpen", true);
        staticElements.SetActive(true);
        GameController.Instance.ToggleCapitalChange();
        dialogueViewer.HideDialogue();
        for (int i = 0; i < 9; i++)
        {
            policyButtons[i].GetComponent<Image>().sprite = notBoughtImages[imageDict[i]];
        }
    }

    public void ClosePolicy()
    {
        policyAnimator.SetBool("isOpen", false);
        staticElements.SetActive(false);
        StartCoroutine(OnAnimationComplete());
        GameController.Instance.ToggleCapitalChange();
    }

    // https://gamedev.stackexchange.com/questions/117423/unity-detect-animations-end
    IEnumerator OnAnimationComplete()
    {
        // while (policyAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1.0f && policyAnimator.IsInTransition(0))
        //    yield return null;

        yield return new WaitForSeconds(0.4f);
        dialogueViewer.Next();
    }

    
}
