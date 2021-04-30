using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class MainMenuScript : MonoBehaviour
{
    public MenuBtnController arenaButton;

    public TextMeshProUGUI options;
    public TextMeshProUGUI quit;

    public AudioClip errorSFX;


    public void OpenMainMenu()
    {
        gameObject.SetActive(true);
        arenaButton.selected = true;
        EventSystem.current.SetSelectedGameObject(arenaButton.gameObject);
        MainMenuCameraView();
    }

    public void MainMenuCameraView()
    {
        //anim to main menu
    }

    public void OptionsMenuCameraView()
    {
        //anim to options menu
    }

    //TODO: other camera views

    // Update is called once per frame
    void Update()
    {

    }
}
