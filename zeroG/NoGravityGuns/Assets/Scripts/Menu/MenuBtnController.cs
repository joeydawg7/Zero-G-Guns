using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Rewired;

public class MenuBtnController : MonoBehaviour, ISelectHandler, IDeselectHandler
{

    public bool selected;
    float timer = 0.0f;
    private Button thisButton;
    MenuOptionsScript optionsScript;
    public Sprite activeSprite;
    public Sprite inactiveSprite;
    private Image thisImage;

    private void Awake()
    {
        optionsScript = FindObjectOfType<MenuOptionsScript>();
    }

    // Start is called before the first frame update
    void Start()
    {
        thisImage = this.gameObject.GetComponent<Image>();
        if (this.gameObject.name != "ArenaBtn")
        {
            selected = false;
            thisImage.sprite = inactiveSprite;
        }




    }

    // Update is called once per frame
    void Update()
    {
        FlickerSprite();
    }

    private void FlickerSprite()
    {
        if (selected)
        {
            if (timer < 0.5f)
            {
                if (thisImage.sprite.name != activeSprite.name)
                {
                    thisImage.sprite = activeSprite;
                }
            }
            else
            {
                if (thisImage.sprite.name != inactiveSprite.name)
                {
                    thisImage.sprite = inactiveSprite;
                }
            }

            if (timer > 1f)
            {
                timer = 0.0f;
            }
            timer += Time.deltaTime;
        }
    }

    public void OnSelect(BaseEventData eventData)
    {
        if (!selected)
        {
            selected = true;
        }

    }

    public void OnDeselect(BaseEventData data)
    {
        if (selected)
        {
            selected = false;
            thisImage.sprite = inactiveSprite;
            timer = 0.0f;
        }
    }

    public void ThisOnClick()
    {
        var button = this.gameObject.name;

        switch (button)
        {
            case "TrainingBtn":
                var traningLevelsScreens = Resources.FindObjectsOfTypeAll<TraningLevelsScreen>();
                //SoundPooler.Instance.PlaySoundEffect(errorSFX);
                if (traningLevelsScreens.Length > 0)
                {
                    var traningLevelsScreen = traningLevelsScreens[0];
                    traningLevelsScreen.ShowTrainingLevelsScreen();

                }
                else
                    Debug.LogError("Couldn't Find a training levels screen!");

                break;
            case "ArenaBtn":
                if (RoundManager.Instance == null)
                {
                    Debug.Log("Click, Round Manger Null");
                    //SceneManager.LoadSceneAsync("Arena_PersistentScene", LoadSceneMode.Single);
                    //LoadingBar.Instance.StartLoadingBar();

                    //uncomment to work on netplay again!
                    //ArenaModeSelectScript.Open();

                    LoadingBar.StartLoading();
                    SceneManager.LoadScene("LocalPlayerJoinScreen");

                }
                else
                {
                    //{
                    //    Debug.Log("Click, HAS Round Manger");
                    //    PersistenceGiverScript.Instance.PersistenceTaker();
                    //    //SceneManager.LoadSceneAsync("Arena_PersistentScene", LoadSceneMode.Single);
                    //    LoadingBar.Instance.StartLoadingBar();

                    Debug.Log("Click, Round Manger Null");
                    //SceneManager.LoadSceneAsync("Arena_PersistentScene", LoadSceneMode.Single);
                    //LoadingBar.Instance.StartLoadingBar();

                    //uncomment to work on netplay again!
                    //ArenaModeSelectScript.Open();

                    LoadingBar.StartLoading();
                    SceneManager.LoadScene("LocalPlayerJoinScreen");

                }

                break;
            case "OptionsBtn":
                MenuOptionsScript.OpenOptionsMenu();

                break;
            case "QuitBtn":
                Application.Quit();
                break;
            default:
                break;
        }

    }

}
