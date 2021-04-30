using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Rewired;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using UnityEngine.Audio;


public class SplashScreenController : MonoBehaviour
{
    private bool devilsCiderLogoPlayed;
    private bool controllerRequiredInPosition;
    private float logoTrans;

    private Vector3 startPosition;
    private Vector3 endPosition;

    public Image logo;
    public RectTransform bulletCanvas;
    public GameObject mainMenu;
    //public Button arenaBtn;
    public AudioSource soundSource;
    public AudioClip[] sounds;

    public Camera mainMenuCamera;

    public MainMenuScript mainMenuScript;

    public AudioMixer audioMixer;


    private bool starting;


    // Start is called before the first frame update
    void Start()
    {
        SetInitialVolumesFromPrefs();
        logoTrans = 0.0f;
        startPosition = bulletCanvas.transform.position;

        endPosition = new Vector3(0.0f, 0.0f, startPosition.z);

        LoadingBar.Instance.StopLoadingBar();


        StartCoroutine(FadeInLogo());
    }

    void SetInitialVolumesFromPrefs()
    {
        //get a saved volume from a playerPrefs string, and set that volume we got to our slider value so it displays correctly
        SetVolumeFromPref("MasterVolume");
        SetVolumeFromPref("SFXVolume");
        SetVolumeFromPref("MusicVolume");
    }

    private float SetVolumeFromPref(string pref)
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
        audioMixer.SetFloat(pref, vol);

        PlayerPrefs.Save();

        return oldVol;
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

    public void OpenStartMenu()
    {

        //mainMenu.SetActive(true);
        soundSource.Stop();

        //new method of playing music, use this from now on :D
        //MusicManager.PlaySong(sounds[2], true);

        //arenaBtn.Select();
        //EventSystem.current.SetSelectedGameObject(arenaBtn.gameObject);


        //ControllerLayoutManager.SwapToUIMaps(true);

        //stop that error about having more than 1 audio listener because it bugs me
        if (FindObjectsOfType<Camera>().Length > 1)
        {
            mainMenuCamera.GetComponent<AudioListener>().enabled = false;
        }

        //mainMenuScript.OpenMainMenu();

        SceneManager.LoadScene("Menu");

    }

    IEnumerator FadeInLogo()
    {

        soundSource.PlayOneShot(sounds[0]);
        while (logoTrans < 1.0f)
        {
            logo.color = Color.Lerp(new Color(1.0f, 1.0f, 1.0f, 0.0f), new Color(1.0f, 1.0f, 1.0f, 1.0f), logoTrans);
            logoTrans += Time.deltaTime;
            yield return null;
        }
        yield return new WaitForSeconds(1.0f);
        while (logoTrans > 0.0f)
        {
            logo.color = Color.Lerp(new Color(1.0f, 1.0f, 1.0f, 0.0f), new Color(1.0f, 1.0f, 1.0f, 1.0f), logoTrans);
            logoTrans -= Time.deltaTime;
            yield return null;
        }
        float t = 0.0f;
        soundSource.Stop();
        soundSource.PlayOneShot(sounds[1]);
        while (t < 0.5f)
        {
            bulletCanvas.transform.position = Vector3.Lerp(startPosition, endPosition, t * 2.0f);
            t += Time.deltaTime;
            yield return null;
        }
        yield return new WaitForSeconds(1.0f);
        soundSource.Stop();
        //pressAText.enabled = true;    
        //pressAEnabled = true;
        bulletCanvas.gameObject.SetActive(false);
        OpenStartMenu();
    }
}
