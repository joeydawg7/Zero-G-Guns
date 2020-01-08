using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuBtnController : MonoBehaviour, ISelectHandler, IDeselectHandler
{
    private bool selected;
    float timer = 0.0f;
    private Button thisButton;    
    // Start is called before the first frame update
    void Start()
    {
        if(this.gameObject.name != "TrainingBtn")
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
                if(this.gameObject.GetComponentInChildren<TMPro.TextMeshProUGUI>().color != new Color(1.0f, 1.0f, 1.0f, 1.0f))
                {
                    this.gameObject.GetComponentInChildren<TMPro.TextMeshProUGUI>().color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
                }                
            }
            else
            {
                if(this.gameObject.GetComponentInChildren<TMPro.TextMeshProUGUI>().color != new Color(1.0f, 1.0f, 1.0f, 0.0f))
                {
                    this.gameObject.GetComponentInChildren<TMPro.TextMeshProUGUI>().color = new Color(1.0f, 1.0f, 1.0f, 0.0f);
                }                
            }

            if (timer > 0.5f)
            {
                timer = 0.0f;
            }
            timer += Time.deltaTime;            
        }
        else
        {
            if(this.gameObject.GetComponentInChildren<TMPro.TextMeshProUGUI>().color != new Color(1.0f, 1.0f, 1.0f, 1.0f))
            {
                this.gameObject.GetComponentInChildren<TMPro.TextMeshProUGUI>().color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
            }            
            if(timer!=0.0f)
            {
                timer = 0.0f;
            }
            
        }
    }

    public void OnSelect(BaseEventData eventData)
    {
       
        selected = true;
            
    }

   public void OnDeselect(BaseEventData data)
   {
       
       selected = false;
      
   }
    
    public void ThisOnClick()
    {
        var button = this.gameObject.name;
        switch (button)
        {
            case "TrainingBtn":
                SceneManager.LoadScene("BreakTheShit");
                break;
            case "ArenaBtn":
                SceneManager.LoadScene("PersistentScene");
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
