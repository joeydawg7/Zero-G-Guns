using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;


public class OptionsMenuScreenFlicker : MonoBehaviour
{

    Image screenImage;
    public GameObject deselectedObj;
    MenuOptionsScript menuOptionsScript;

    public bool viewing;

    // Start is called before the first frame update
    void Start()
    {
        screenImage = GetComponent<Image>();
        menuOptionsScript = FindObjectOfType<MenuOptionsScript>();
        viewing = false;
    }

    //Update is called once per frame
    void Update()
    {

        if (menuOptionsScript.menuState == MenuOptionsScript.MenuState.SelectScreen)
            viewing = false;

        //only show screen as selected when it really do be like that
        if (EventSystem.current.currentSelectedGameObject == gameObject)
        {
            deselectedObj.SetActive(false);
        }
        else
        {
            if(!viewing)
                deselectedObj.SetActive(true);
            else
                deselectedObj.SetActive(false);
        }

    }

}
