using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Rewired;

public class MenuBtnController : MonoBehaviour, ISelectHandler, IDeselectHandler
{

    private bool selected;
    float timer = 0.0f;
    private Button thisButton;
    private MenuBtnSpriteHolder sprt;
    private Image thisImage;
    // Start is called before the first frame update
    void Start()
    {
        sprt = this.gameObject.GetComponent<MenuBtnSpriteHolder>();
        thisImage = this.gameObject.GetComponent<Image>();
        if(this.gameObject.name != "ArenaBtn")
        {
            selected = false;
        }


    }

    // Update is called once per frame
    void Update()
    {
        if (selected)
        { 
            if (timer < 0.25f)
            {
                if(thisImage.sprite.name != sprt.activeSprite.name)
                {                   
                    thisImage.sprite = sprt.activeSprite;
                }                
            }
            else
            {               
                if (thisImage.sprite.name != sprt.inActiveSprite.name)
                {                   
                    thisImage.sprite = sprt.inActiveSprite;
                }
            }

            if (timer > 0.5f)
            {
                timer = 0.0f;
            }
            timer += Time.deltaTime;            
        }       
    }

    public void OnSelect(BaseEventData eventData)
    {  
        if(!selected)
        {
            selected = true;
        }
                    
    }

   public void OnDeselect(BaseEventData data)
   {   
        if(selected)
        {
            selected = false;
            thisImage.sprite = sprt.inActiveSprite;
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
                if (traningLevelsScreens.Length > 0)
                {
                    var traningLevelsScreen = traningLevelsScreens[0];
                    traningLevelsScreen.ShowTrainingLevelsScreen();
                }
                else
                    Debug.LogError("Couldn't Find a training levels screen!");
               
                break;
            case "ArenaBtn":
                if(RoundManager.Instance == null)
                {
                    Debug.Log("Click, Round Manger Null");                       
                    SceneManager.LoadSceneAsync("Arena_PersistentScene", LoadSceneMode.Single);
                    LoadingBar.Instance.StartLoadingBar();
                }
                else
                {
                    Debug.Log("Click, HAS Round Manger");
                    PersistenceGiverScript.Instance.PersistenceTaker();
                    SceneManager.LoadSceneAsync("Arena_PersistentScene", LoadSceneMode.Single);
                    LoadingBar.Instance.StartLoadingBar();
                }
                
                break;
            case "OptionsBtn":
                //options
                break;
            case "QuitBtn":
                Application.Quit();
                break;
            default:
                break;
        }        
        
    }

}
