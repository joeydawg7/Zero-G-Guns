using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Experimental.Rendering.LWRP;

//quick script to keep the worldspace canvas attached to player without inheriting rotation like it would if it were a child
public class PlayerCanvasScript : MonoBehaviour
{
    [HideInInspector]
    public PlayerScript playerScript;

    public Transform floatingDamageTextSpawnPoint;
    public Animator hpContainerAnimator;
    public Image hpFront;
    public Image hpBack;
    public Image hpFlash;
    public Image winnerCrown;

    public Image gunTimeFront;
    public Image gunTimeBack;
    public Image gunTimeFlash;

    float fillAmount;

    const float HEALTH_ANIM_RATE =10f;

    public void SetPlayerCanvas(Sprite hpFront, Sprite hpBack, Sprite hpFlash, PlayerScript playerScript)
    {
        this.hpFront.sprite = hpFront;
        this.hpBack.sprite = hpBack;
        this.hpFlash.sprite = hpFlash;

        //this.gunTimeFront.sprite = hpFront;
        //this.gunTimeBack.sprite = hpBack;
        //this.gunTimeFlash.sprite = hpFlash;

        this.playerScript = playerScript;

        setHealth(playerScript.health);

        foreach (var text in GetComponentsInChildren<TextMeshProUGUI>())
        {
            text.color = playerScript.playerColor;
        }

        winnerCrown.gameObject.SetActive(false);
    }

    public void setHealth(float fillDamage)
    {
        fillAmount = fillDamage;

        if (GameManager.Instance.isGameStarted)
        {
            if (fillDamage <= 0.3f)
            {
                hpContainerAnimator.SetBool("isHPCritical", true);
            }
            else
            {
                hpContainerAnimator.SetBool("isHPCritical", false);
            }

            hpContainerAnimator.SetTrigger("takeDamage");
        }
    }

    public void ShowCurrentWinnerCrown(bool show)
    {
        if(show)
        {
            winnerCrown.gameObject.SetActive(true);
            winnerCrown.color = playerScript.playerColor;
            winnerCrown.GetComponent<Light2D>().color = playerScript.playerColor;
        }
        else
        {
            winnerCrown.gameObject.SetActive(false);
        }
    }

    public void ResetGunTimer()
    {
        gunTimeFront.fillAmount = 1;
    }

    public void ShowGunTimer()
    {
        gunTimeBack.gameObject.SetActive(false);
        gunTimeFlash.gameObject.SetActive(false);
    }

    public void HideGunTimer()
    {
        gunTimeBack.gameObject.SetActive(true);
        gunTimeFlash.gameObject.SetActive(false);
    }


    void Update()
    {
        if (GameManager.Instance.isGameStarted)
        {
            if (playerScript != null)
                transform.position = playerScript.rb.transform.position;


            if (fillAmount != hpFront.fillAmount)
            {
                hpFront.fillAmount = Mathf.Lerp(hpFront.fillAmount, fillAmount, Time.deltaTime * HEALTH_ANIM_RATE);
            }

            //if(((float)playerScript.armsScript.timeYouCanHoldGun /playerScript.armsScript.currentWeapon.time) != gunTimeFront.fillAmount)
            //{
            //    gunTimeFront.fillAmount = Mathf.Lerp(gunTimeFront.fillAmount, (float)playerScript.armsScript.timeYouCanHoldGun / playerScript.armsScript.currentWeapon.time, Time.deltaTime * HEALTH_ANIM_RATE);
            //}
        }
    }
}
