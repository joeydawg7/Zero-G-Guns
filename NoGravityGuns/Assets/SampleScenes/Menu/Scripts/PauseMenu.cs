using Rewired;
using System;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class PauseMenu : MonoBehaviour
{
    #region singleton stuff
    private static PauseMenu _instance;
    public static PauseMenu Instance { get { return _instance; } }
    #endregion

    private Toggle m_MenuToggle;
    private float m_TimeScaleRef = 1f;
    private float m_VolumeRef = 1f;
    private bool m_Paused;

    private Canvas pauseCanvas;

    public Image quitSlider;
    public Image restartSlider;

    float quitTimer;
    float submitTimer;

    void Awake()
    {


        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
            transform.parent.gameObject.DontDestroyOnLoad();
        }

        if (!m_MenuToggle)
        {
            m_MenuToggle = GetComponent<Toggle>();
        }
        if (!pauseCanvas)
        {
            pauseCanvas = this.gameObject.GetComponent<Canvas>();
        }

        this.gameObject.SetActive(false);


    }

    //private void Update()
    //{
    //    if (pauseCanvas.gameObject.activeInHierarchy)
    //    {
    //        if (ReInput.players.GetSystemPlayer().GetButtonDown("Join"))
    //        {
    //            Application.Quit();
    //            Debug.Log("you Quit");
    //        }
    //    }
    //}


    private void OnEnable()
    {
        if (!m_MenuToggle)
        {
            m_MenuToggle = GetComponent<Toggle>();
        }
        if (!pauseCanvas)
        {
            pauseCanvas = this.gameObject.GetComponent<Canvas>();
        }

    }

    public void MenuOn()
    {
        if (GameManager.Instance.isGameStarted)
        {

            pauseCanvas.gameObject.SetActive(true);

            m_TimeScaleRef = Time.timeScale;
            Time.timeScale = 0f;

            m_VolumeRef = AudioListener.volume;
            AudioListener.volume = 0f;

            m_Paused = true;

            quitSlider.fillAmount = 0f;
            restartSlider.fillAmount = 0f;
            quitTimer = 0f;
            submitTimer = 0f;

            ControllerLayoutManager.SwapToUIMaps(false);

        }


    }


    public void MenuOff()
    {
        Time.timeScale = m_TimeScaleRef;
        AudioListener.volume = m_VolumeRef;
        m_Paused = false;

        pauseCanvas.gameObject.SetActive(false);

        ControllerLayoutManager.SwapToGameplayMaps();
    }

    public void QuitMatch()
    {
        MenuOff();

        //do the break the targets reset instead
        if(FindObjectOfType<BTT_Manager>())
        {
            //TODO: create a quick reset for BTT levels that doesn't need to load so much stuff
            //TODO: add a method of quitting to menu from BTT levels
            BTT_Manager.Instance.BackToPersistentScene();

        }

        RoundManager.Instance.NewRound(true);
    }

    public void ExitGame()
    {
        Debug.Log("Qutting the game!");
        Application.Quit();
    }

    bool holdingQuit = false;
    bool holdingRestart = false;

    private void Update()
    {

        //if (ReInput.players.GetSystemPlayer().GetButton("UICancel"))
        //{
        //    quitSlider.value += 0.03f;

        //    holdingQuit = true;

        //    if (quitSlider.value >= 1f)
        //    {
        //        ExitGame();
        //    }
        //}
        //else
        //    holdingQuit = false;


        //if (ReInput.players.GetSystemPlayer().GetButton("UISubmit"))
        //{
        //    restartSlider.value += 0.03f;

        //    holdingRestart = true;

        //    if (restartSlider.value >= 1f)
        //    {
        //        QuitMatch();
        //    }
        //}
        //else
        //    holdingRestart = false;

        //if (ReInput.players.GetSystemPlayer().GetButtonDown("UIStart"))
        //{
        //    MenuOff();
        //}


        //if (!holdingQuit)
        //{
        //    quitSlider.value -= 0.01f;
        //    if (quitSlider.value < 0)
        //        quitSlider.value = 0;
        //}

        if (ReInput.players.GetSystemPlayer().GetButton("UICancel"))
        {
            quitTimer += Time.unscaledDeltaTime;
            quitSlider.fillAmount = quitTimer;
        }
        // if (ReInput.players.GetSystemPlayer().GetButtonUp("UICancel"))
        else
        {
            quitTimer = 0;
            quitSlider.fillAmount = quitTimer;
        }
        if (quitTimer > 1.0f)
        {
            quitTimer = 0;
            quitSlider.fillAmount = quitTimer;
            ExitGame();
        }

        if (ReInput.players.GetSystemPlayer().GetButton("UISubmit"))
        {
            submitTimer += Time.unscaledDeltaTime;
            restartSlider.fillAmount = submitTimer;
        }
        //if (ReInput.players.GetSystemPlayer().GetButtonUp("UISubmit"))
        else
        {
            submitTimer = 0;
            restartSlider.fillAmount = submitTimer;
        }
        if (submitTimer > 1.0f)
        {
            submitTimer = 0;
            restartSlider.fillAmount = submitTimer;
            QuitMatch();
        }


    }


}
