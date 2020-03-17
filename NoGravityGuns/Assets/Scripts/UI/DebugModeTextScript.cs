using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class DebugModeTextScript : MonoBehaviour
{
    public DebugManager debugManager;
    TextMeshProUGUI debugModeText;

    // Start is called before the first frame update
    void Start()
    {
        debugModeText = GetComponent<TextMeshProUGUI>();


        if (!debugManager.useDebugSettings)
            debugModeText.gameObject.SetActive(false);
    }

}
