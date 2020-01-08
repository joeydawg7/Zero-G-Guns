using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Rewired;
using UnityEngine.SceneManagement;

public class SplashScreenController : MonoBehaviour
{

    private bool devilsCiderLogoPlayed;
    private bool controllerRequiredInPosition;
    private float logoTrans;
    
   

    private Vector3 startPosition;
    private Vector3 endPosition;

    public Image logo;
    public RectTransform bulletCanvas;    
    public GameObject loadingRing;
    public GameObject mainMenu;
    public Button trainingBtn;
    
    private bool starting;
    
    // Start is called before the first frame update
    void Start()
    {
        logoTrans = 0.0f;       
        startPosition = bulletCanvas.transform.position;
        
        endPosition = new Vector3(0.0f, 0.0f, startPosition.z);        
        
        if(RoundManager.Instance == null)
        {
            StartCoroutine(FadeInLogo());
        }
        else
        {
            OpenStartMenu();
        }            
    }

   public void OpenStartMenu()
    {
        mainMenu.SetActive(true);
        trainingBtn.Select();
    }

   IEnumerator FadeInLogo()
   {
        while(logoTrans < 1.0f)
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
        while (t < 1.0f)
        {
            bulletCanvas.transform.position = Vector3.Lerp(startPosition, endPosition, t);
            t += Time.deltaTime;
            yield return null;
        }
        yield return new WaitForSeconds(1.0f);
        //pressAText.enabled = true;    
        //pressAEnabled = true;
        bulletCanvas.gameObject.SetActive(false);
        OpenStartMenu();
   }   
    
    
}
