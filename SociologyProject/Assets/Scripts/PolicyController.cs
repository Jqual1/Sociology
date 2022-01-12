using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PolicyController : MonoBehaviour
{
    public Animator policyAnimator;
    public GameObject staticElements;

    public DialogueViewer dialogueViewer;

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
