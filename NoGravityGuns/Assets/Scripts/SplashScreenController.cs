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
    private bool pressAEnabled;

    private Vector3 startPosition;
    private Vector3 endPosition;

    public Image logo;
    public RectTransform bulletCanvas;
    public TextMeshProUGUI pressAText;
    public GameObject loadingRing;

    private float timer;
    private bool starting;

    
    // Start is called before the first frame update
    void Start()
    {
        logoTrans = 0.0f;
        pressAText.enabled = false;
        pressAEnabled = false;
        startPosition = bulletCanvas.transform.position;
        timer = 0;
        endPosition = new Vector3(0.0f, 0.0f, startPosition.z);
        
        StartCoroutine(FadeInLogo());    
    }

    // Update is called once per frame
    void Update()
    {        
        if(pressAEnabled)
        {
            if (!starting)
            {
                if (timer < 1.0f)
                {
                    if (!pressAText.enabled)
                    {
                        pressAText.enabled = true;
                    }
                }
                else
                {
                    if (pressAText.enabled)
                    {
                        pressAText.enabled = false;
                    }
                }

                if (timer > 2.0f)
                {
                    timer = 0.0f;
                }
                timer += Time.deltaTime;
            }

            if (Input.anyKey)
            {
                starting = true;
                pressAText.enabled = false;
                bulletCanvas.gameObject.SetActive(false);
                loadingRing.SetActive(true);
                SceneManager.LoadScene("PersistentScene");
            }
        }
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
        pressAText.enabled = true;    
        pressAEnabled = true;
   }

    
}
