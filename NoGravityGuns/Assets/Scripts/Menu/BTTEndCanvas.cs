using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class BTTEndCanvas : MonoBehaviour
{
    public TextMeshProUGUI time;
    public TextMeshProUGUI newRecord;

    // Start is called before the first frame update
    void Start()
    {
        time.gameObject.SetActive(false);
        newRecord.gameObject.SetActive(false);

        SetAllTextAlphas(0);

    }

    private void SetAllTextAlphas(float alpha)
    {
        foreach (Transform child in transform)
        {
            TextMeshProUGUI t = child.GetComponent<TextMeshProUGUI>();

            if (t)
            {
                t.alpha = alpha;
            }
        }
    }

    public void ShowEndScreen(float t)
    {
        SetAllTextAlphas(1);
        string tString = Extensions.FloatToTime(t, "#0:00.000");

        //TODO: do new record stuff here!
        if (BTT_Manager.Instance.currentRoom.bestTime <= 0)
            BTT_Manager.Instance.currentRoom.bestTime = t;
        else if (t < BTT_Manager.Instance.currentRoom.bestTime)
            BTT_Manager.Instance.currentRoom.bestTime = t;

        StartCoroutine(AnimateEndScreen(tString));
    }

    IEnumerator AnimateEndScreen(string t)
    {
        yield return new WaitForSeconds(0.25f);

        time.gameObject.SetActive(true);
        time.text = "Time: " + t;

        yield return new WaitForSeconds(0.25f);

        newRecord.gameObject.SetActive(true);
    }
}
