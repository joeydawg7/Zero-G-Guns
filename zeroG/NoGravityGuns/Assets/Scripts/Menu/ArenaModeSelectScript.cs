using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using Photon.Pun;

public class ArenaModeSelectScript : MonoBehaviour
{
    private static ArenaModeSelectScript _instance;
    public static ArenaModeSelectScript Instance { get { return _instance; } }

    Animator cameraAnimator;

    public Button onlineButton;
    public Button mainMenuArenaButton;
    public Button launchGame;

    private float holdTimer;
    public Image holdTimerIndicator;
    public GameObject holdToGoBackObject;

    public GameObject networkLauncher;

    bool inArenaModeSelect = false;

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

        cameraAnimator = Camera.main.GetComponent<Animator>();
    }

    public static void Open()
    {
        Instance.AnimToArenaModeSelect();       
    }


    protected void AnimToArenaModeSelect()
    {
        cameraAnimator.SetTrigger("ArenaModeSelect");
        onlineButton.Select();
        EventSystem.current.SetSelectedGameObject(onlineButton.gameObject);
        holdToGoBackObject.SetActive(true);
        inArenaModeSelect = true;
    }

    protected void AnimToLobbySelect()
    {
        cameraAnimator.SetTrigger("LobbyModeSelect");
        //onlineButton.Select();
        //EventSystem.current.SetSelectedGameObject(onlineButton.gameObject);
        holdToGoBackObject.SetActive(true);
        networkLauncher.SetActive(true);
    }

    protected void AnimBackFromLobbySelect()
    {
        cameraAnimator.SetTrigger("LobbyModeSelect");
        onlineButton.Select();
        EventSystem.current.SetSelectedGameObject(onlineButton.gameObject);
        holdToGoBackObject.SetActive(true);
        networkLauncher.SetActive(false);
    }

    protected void AnimFromArenaModeSelect()
    {
        cameraAnimator.SetTrigger("ArenaModeSelect");
        mainMenuArenaButton.Select();
        EventSystem.current.SetSelectedGameObject(mainMenuArenaButton.gameObject);
        holdToGoBackObject.SetActive(false);
        inArenaModeSelect = false;
    }

    public void OpenLocalArena()
    {
        //starts loading bar
        LoadingBar.StartLoading();
        //loads player joining screen
        SceneManager.LoadScene("LocalPlayerJoinScreen");
    }

    public void OpenMultiplayerArena()
    {
        //TODO: all of netcode      
            AnimToLobbySelect();        
    }

    private void Update()
    {
        if (this.gameObject.activeInHierarchy && inArenaModeSelect)
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
                if(networkLauncher.activeInHierarchy)
                {                    
                    AnimBackFromLobbySelect();  
                    if(PhotonNetwork.IsConnected)
                    {                        
                        PhotonNetwork.Disconnect();
                        Debug.Log("Disconnect Called! Connection Status = " + PhotonNetwork.IsConnected);
                    }        
                }
                else
                {
                    AnimFromArenaModeSelect();
                }
                holdTimer = 0;
            }
        }

    }

}
