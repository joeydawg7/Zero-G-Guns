using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

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

    float fillAmount;

    const float HEALTH_ANIM_RATE =10f;

    public void SetPlayerCanvas(Sprite hpFront, Sprite hpBack, Sprite hpFlash, PlayerScript playerScript)
    {
        this.hpFront.sprite = hpFront;
        this.hpBack.sprite = hpBack;
        this.hpFlash.sprite = hpFlash;

        this.playerScript = playerScript;

        setHealth(playerScript.health);

        foreach (var text in GetComponentsInChildren<TextMeshProUGUI>())
        {
            text.color = playerScript.playerColor;
        }
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

    void Update()
    {
        if (GameManager.Instance.isGameStarted)
        {
            if (playerScript != null)
                transform.position = playerScript.transform.position;


            if (fillAmount != hpFront.fillAmount)
            {
                hpFront.fillAmount = Mathf.Lerp(hpFront.fillAmount, fillAmount, Time.deltaTime * HEALTH_ANIM_RATE);
            }
        }
    }
}
