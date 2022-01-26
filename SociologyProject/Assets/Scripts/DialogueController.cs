// Courtesy of Matthew Ventures
// http://www.mrventures.net/all-tutorials/converting-a-twine-story-to-unity

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static DialogueObject;

public class DialogueController : MonoBehaviour
{

    //[SerializeField] TextAsset[] chapters;
    //[SerializeField] TextAsset twineText;
    //public GameObject[] backgrounds;
    Dialogue curDialogue;
    Node curNode;
    string delimeter = "\r\n";
    public bool macLineEndings = false;


    public delegate void NodeEnteredHandler(Node node);
    public event NodeEnteredHandler onEnteredNode;
    private int chapter = 1;

    public Node GetCurrentNode()
    {
        return curNode;
    }

    public void InitializeDialogue(TextAsset twineText)
    {
        
        if (macLineEndings) { delimeter = "\n"; }
        curDialogue = new Dialogue(twineText, delimeter);
        curNode = curDialogue.GetStartNode();
        onEnteredNode(curNode);
    }    

    public List<Response> GetCurrentResponses()
    {
        return curNode.responses;
    }

    public void ChooseResponse(int responseIndex)
    {
        Node nextNode;
        string nextNodeID = curNode.responses[responseIndex].destinationNode;
        if (nextNodeID.ToLower().Contains("cutscene"))
        {
            chapter += 1;
            int progress = GameController.Instance.progress;
            if (progress < (5+ (chapter-1)*3))
            {
                 nextNode = curDialogue.GetNode("CutScenePoor");
            }
            else if (progress < (7 + (chapter - 1) * 3))
            {
                 nextNode = curDialogue.GetNode("CutSceneNeutral");
            }
            else
            {
                 nextNode = curDialogue.GetNode("CutSceneGood");
            }
        }
        else
        {
             nextNode = curDialogue.GetNode(nextNodeID);
            
        }
        curNode = nextNode;
        onEnteredNode(nextNode);
    }

    public void UpdateDialogue()
    {
        Response newResponse = new Response();
        newResponse.displayText = "Next";
        newResponse.destinationNode = "UniformB";
        curDialogue.GetNode("Policy").responses[0] = newResponse;
    }

    public Dialogue GetCurrentDialogue()
    {
        return curDialogue;
    }
}