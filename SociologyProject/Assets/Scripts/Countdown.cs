using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Countdown : MonoBehaviour
{
    public int duration = 30;
    public int timeRemaining;
    public bool isCountingDown = false;
    public GameObject CountdownBox;
    public DialogueViewer dialogueViewer;
    public bool r = false;
    public void Begin()
    {
        if (!isCountingDown)
        {
            CountdownBox.SetActive(true);
            Debug.Log("Time Start");
            isCountingDown = true;
            timeRemaining = duration;
            Invoke("_tick", 1f);
        }
    }

    private void _tick()
    {
        timeRemaining--;
        if (timeRemaining > 0)
        {
            CountdownBox.GetComponentInChildren<TextMeshProUGUI>().text = "Choose: " + timeRemaining + "s";
            Invoke("_tick", 1f);
        }
        else
        {
            CountdownBox.GetComponentInChildren<TextMeshProUGUI>().text = "Choose: 30s";
            CountdownBox.SetActive(false);
            isCountingDown = false;
            Debug.Log("Time's Up!");
            if (!r)
            {
                int randomInt = Random.Range(0, 2);
                dialogueViewer.OnNodeSelected(randomInt);
            }
            r = false;
        }
    }

	public void reset()
	{
        timeRemaining = 1;
        CountdownBox.SetActive(false);
        r = true;
    }
}
