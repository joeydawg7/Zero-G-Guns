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
    public AudioSource soundSource;
    public AudioClip[] sounds;

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

        LoadingBar.Instance.StopLoadingBar();
    }

   public void OpenStartMenu()
    {
        mainMenu.SetActive(true);
        soundSource.Stop();
        GameModeFlag.Instance.PlayMusic();
        trainingBtn.Select();
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
            bulletCanvas.transform.position = Vector3.Lerp(startPosition, endPosition, t*2.0f);
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
