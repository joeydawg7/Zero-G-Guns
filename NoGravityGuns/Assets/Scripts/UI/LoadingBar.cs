using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadingBar : MonoBehaviour
{
    #region singleton stuff
    private static LoadingBar _instance;
    public static LoadingBar Instance { get { return _instance; } }
    #endregion

    private Image loadingBarImage;

    private void Start()
    {
        foreach(var lb in this.gameObject.GetComponentsInChildren<Image>())
        {
            if(lb.gameObject.name == "LoadingIndicator")
            {
                loadingBarImage = lb;
            }
        }
        this.gameObject.SetActive(false);
    }

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
            this.gameObject.DontDestroyOnLoad();
        }
    }


        public void StartLoadingBar()
    {
        this.gameObject.SetActive(true);
        StartCoroutine(PlayLoadingImage());
    }

    public void StopLoadingBar()
    {
        StopAllCoroutines();
        this.gameObject.SetActive(false);
    }

    private IEnumerator PlayLoadingImage()
    {
        float timer = 0.0f;
        while(true)
        {
            timer += Time.deltaTime;
            loadingBarImage.fillAmount = timer;
            if(timer > 1.0)
            {
                timer = 0.0f;
            }
            yield return null;
        }
    }   
}
