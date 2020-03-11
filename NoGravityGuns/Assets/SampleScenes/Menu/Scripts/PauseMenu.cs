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

    public Slider quitSlider;
    public Slider restartSlider;


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
        pauseCanvas.gameObject.SetActive(true);

        m_TimeScaleRef = Time.timeScale;
        Time.timeScale = 0f;

        m_VolumeRef = AudioListener.volume;
        AudioListener.volume = 0f;

        m_Paused = true;

        quitSlider.value = 0f;
        restartSlider.value = 0f;

        ControllerLayoutManager.SwapToUIMaps(false);


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

        if (ReInput.players.GetSystemPlayer().GetButton("UICancel"))
        {
            quitSlider.value += 0.03f;

            holdingQuit = true;

            if (quitSlider.value >= 1f)
            {
                ExitGame();
            }
        }
        else
            holdingQuit = false;


        if (ReInput.players.GetSystemPlayer().GetButton("UISubmit"))
        {
            restartSlider.value += 0.03f;

            holdingRestart = true;

            if (restartSlider.value >= 1f)
            {
                QuitMatch();
            }
        }
        else
            holdingRestart = false;

        if (ReInput.players.GetSystemPlayer().GetButtonDown("UIStart"))
        {
            MenuOff();
        }


        if (!holdingQuit)
        {
            quitSlider.value -= 0.01f;
            if (quitSlider.value < 0)
                quitSlider.value = 0;
        }
    }

    //public void OnMenuStatusChange ()
    //{
    //    if (m_MenuToggle.isOn && !m_Paused)
    //    {
    //        MenuOn();
    //    }
    //    else if (!m_MenuToggle.isOn && m_Paused)
    //    {
    //        MenuOff();
    //    }
    //}


    //#if !MOBILE_INPUT
    //	void Update()
    //	{
    //		if(Input.GetKeyUp(KeyCode.Escape))
    //		{
    //		    m_MenuToggle.isOn = !m_MenuToggle.isOn;
    //            Cursor.visible = m_MenuToggle.isOn;//force the cursor visible if anythign had hidden it
    //		}
    //	}
    //#endif



}
