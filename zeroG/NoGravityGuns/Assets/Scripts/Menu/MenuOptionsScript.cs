using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using UnityEngine.EventSystems;
using Rewired;

public class MenuOptionsScript : MonoBehaviour
{
    private static MenuOptionsScript _instance;

    public static MenuOptionsScript Instance { get { return _instance; } }

    public Slider masterVolumeSlider;
    public Slider sfxVolumeSlider;
    public Slider musicVolumeSlider;
    public AudioMixer audioMixer;

    public Button resolutionButton;

    private float holdTimer;
    public Image holdTimerIndicator;
    public GameObject holdToGoBackObject;

    public GameObject optionsButton;
    public GameObject arenaButton;
    public GameObject mainMenuParent;

    public Button defaultSelectedObject;

    public AudioClip menuMusic;

    Animator cameraAnimator;

    public enum MenuState { SelectScreen, Audio, Visual, Controls, Other }
    public MenuState menuState;

    ResolutionScript resScript;

    bool initialVolumeSetup = false;

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
            gameObject.DontDestroyOnLoad();
        }

        resScript = resolutionButton.GetComponent<ResolutionScript>();

    }

    private void Start()
    {
        ControllerLayoutManager.SwapToUIMaps(true);
        EventSystem.current.SetSelectedGameObject(arenaButton);
        arenaButton.GetComponent<MenuBtnController>().selected = true;
        MusicManager.PlaySong(menuMusic, true);
        holdToGoBackObject.SetActive(false);
        LoadingBar.Instance.StopLoadingBar();

        //get volume preferences from player prefs
        //SetInitialVolumesFromPrefs();
        initialVolumeSetup = true;

        menuState = MenuState.Other;
    }

    private void Update()
    {
        if (this.gameObject.activeInHierarchy)
        {
            if (Input.GetButton("Cancel"))
            {
                holdTimer += Time.deltaTime;
                holdTimerIndicator.fillAmount = holdTimer;
            }
            if (Input.GetButtonUp("Cancel"))
            {
                holdTimer = 0;
                holdTimerIndicator.fillAmount = holdTimer;
            }
            if (holdTimer > 1.0f)
            {
                //where to go back to when you hold B changes based on where you are
                switch (menuState)
                {
                    case MenuState.SelectScreen:
                        ReturnFromSelectScreen();
                        break;
                    case MenuState.Audio:
                        ReturnFromAudio();
                        break;
                    case MenuState.Visual:
                        ReturnFromVisuals();
                        break;
                    case MenuState.Controls:
                        ReturnFromControls();
                        break;
                    default:
                        //do nothing
                        break;
                }


            }
        }

    }

    private void ReturnFromSelectScreen()
    {
        holdTimer = 0;
        holdTimerIndicator.fillAmount = holdTimer;
        EventSystem.current.SetSelectedGameObject(optionsButton);
        optionsButton.GetComponent<MenuBtnController>().selected = true;
        Camera.main.GetComponent<Animator>().SetBool("RotateToOptionsMenu", false);
        holdToGoBackObject.SetActive(false);
    }

    void ReturnFromAudio()
    {
        menuState = MenuState.SelectScreen;
        holdTimer = 0;
        holdTimerIndicator.fillAmount = holdTimer;
        EventSystem.current.SetSelectedGameObject(defaultSelectedObject.gameObject);
        defaultSelectedObject.Select();
        cameraAnimator.SetTrigger("AudioMenu");
    }

    void ReturnFromVisuals()
    {
        menuState = MenuState.SelectScreen;
        holdTimer = 0;
        holdTimerIndicator.fillAmount = holdTimer;
        EventSystem.current.SetSelectedGameObject(defaultSelectedObject.gameObject);
        defaultSelectedObject.Select();
        cameraAnimator.SetTrigger("VisualsMenu");
    }

    void ReturnFromControls()
    {
        menuState = MenuState.SelectScreen;
        holdTimer = 0;
        holdTimerIndicator.fillAmount = holdTimer;
        EventSystem.current.SetSelectedGameObject(defaultSelectedObject.gameObject);
        defaultSelectedObject.Select();
        cameraAnimator.SetTrigger("ControlsScreen");
    }


    public static void OpenOptionsMenu()
    {
        Debug.Log("opening options menu!");

        Instance.Open();
    }

    protected void Open()
    {
        holdToGoBackObject.SetActive(true);
        cameraAnimator = Camera.main.GetComponent<Animator>();
        cameraAnimator.SetBool("RotateToOptionsMenu", true);
        menuState = MenuState.SelectScreen;

        defaultSelectedObject.Select();
        EventSystem.current.SetSelectedGameObject(defaultSelectedObject.gameObject);
    }

    void SetInitialVolumesFromPrefs()
    {
        //get a saved volume from a playerPrefs string, and set that volume we got to our slider value so it displays correctly
        Instance.masterVolumeSlider.value = SetVolumeFromPref("MasterVolume");
        Instance.sfxVolumeSlider.value = SetVolumeFromPref("SFXVolume");
        Instance.musicVolumeSlider.value = SetVolumeFromPref("MusicVolume");
    }

    //sets the volume based on the given playerPref
    private static float SetVolumeFromPref(string pref)
    {
        float vol = 0;

        //if the key has been set use it as the current volume
        if (PlayerPrefs.HasKey(pref))
        {
            vol = PlayerPrefs.GetFloat(pref, vol);
            print(pref + vol);
        }
        //else max it as default
        else
        {
            Debug.Log("defaulting " + pref + " volume to 1");
            vol = 1;
        }

        //change to a decibel format, save the old format to set as our slider value
        float oldVol = vol;
        vol = LinearToDecibel(vol);

        //set mixer to vol in decibels
        Instance.audioMixer.SetFloat(pref, vol);

        PlayerPrefs.Save();

        return oldVol;
    }

    //what happens when somebody moves a volume slider around
    public void OnVolumeChange()
    {
        if (initialVolumeSetup)
        {
            //master
            //sets the player pref volume = to slider value
            PlayerPrefs.SetFloat("MasterVolume", Instance.masterVolumeSlider.value);
            //sets the volume using the pref
            SetVolumeFromPref("MasterVolume");
            Debug.Log("master: " + Instance.masterVolumeSlider.value);

            //sfx
            PlayerPrefs.SetFloat("SFXVolume", Instance.sfxVolumeSlider.value);
            SetVolumeFromPref("SFXVolume");
            Debug.Log("sfx: " + Instance.sfxVolumeSlider.value);

            //music
            PlayerPrefs.SetFloat("MusicVolume", Instance.musicVolumeSlider.value);
            SetVolumeFromPref("MusicVolume");
            Debug.Log("music: " + Instance.musicVolumeSlider.value);

            PlayerPrefs.Save();
        }
    }

    //converts a linearvalue (0-1) to a decibel value (-144-20 or whatever)
    private static float LinearToDecibel(float linear)
    {
        float dB;

        if (linear > 0)
            dB = 20.0f * Mathf.Log10(linear);
        else
            dB = -144.0f;

        return dB;
    }

    public void OpenAudioOptions()
    {
        cameraAnimator.SetTrigger("AudioMenu");
        menuState = MenuState.Audio;
        EventSystem.current.currentSelectedGameObject.GetComponent<OptionsMenuScreenFlicker>().viewing = true;
        masterVolumeSlider.Select();
        EventSystem.current.SetSelectedGameObject(masterVolumeSlider.gameObject);
    }

    public void OpenVisualOptions()
    {
        cameraAnimator.SetTrigger("VisualsMenu");
        EventSystem.current.currentSelectedGameObject.GetComponent<OptionsMenuScreenFlicker>().viewing = true;
        menuState = MenuState.Visual;
        resolutionButton.Select();
        EventSystem.current.SetSelectedGameObject(resolutionButton.gameObject);
    }

    public void OpenControlsScreen()
    {
        cameraAnimator.SetTrigger("ControlsScreen");
        EventSystem.current.currentSelectedGameObject.GetComponent<OptionsMenuScreenFlicker>().viewing = true;
        menuState = MenuState.Controls;
        EventSystem.current.SetSelectedGameObject(null);
    }

    //sets fullscreen status to the opposite of what it was
    public void ToggleFullScreen()
    {
        Screen.fullScreen = !Screen.fullScreen;
    }

}
