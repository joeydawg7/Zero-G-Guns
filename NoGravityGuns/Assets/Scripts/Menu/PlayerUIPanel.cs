using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class PlayerUIPanel : MonoBehaviour
{
    //public Image playerPortrait;
    public Image playerHealthBar;
    //public TextMeshProUGUI currentWeaponText;
    //public TextMeshProUGUI playerAmmoGun;
    //public Transform stockHolder;
    //public GameObject headStock;
    //public Image gunImage;
    //public GridLayoutGroup playerUILayout;
    ////public TextMeshProUGUI kills;
    //public Image ammoFront;
    //public Transform killTags;
    //public GameObject killTag;

    GameManager gameManager;
    Animator animator;

    //public Image pistolImage;
    //public Image assaultRifleImage;
    //public Image minigunImage;
    //public Image railgunImage;
    //public Image shotgunImage;
    //public Image rocketLauncherImage;

    private void Awake()
    {
        gameManager = GameManager.Instance;
    }

    public void setAll(float fillDamage, string statusMsg, string gunMsg, Color32 color, Sprite healthbar)
    {
        gameObject.SetActive(true);

        animator = GetComponent<Animator>();
        gameManager = GameManager.Instance;
        setHealth(fillDamage);
        //SetGunText(statusMsg);
        SetAmmoText(gunMsg, 1f);
        //kills.text = "0";
        foreach (var text in GetComponentsInChildren<TextMeshProUGUI>())
        {
            text.color = color;
        }

        //this.playerPortrait.sprite = playerPortrait;
        this.playerHealthBar.sprite = healthbar;
        //this.playerUILayout = transform.parent.GetComponent<GridLayoutGroup>();

    }
    public void setHealth(float fillDamage)
    {
        fillAmount = fillDamage;


        if (GameManager.Instance.isGameStarted)
        {
            if (fillDamage <= 0.3f)
            {
                animator.SetBool("isHPCritical", true);
            }
            else
            {
                animator.SetBool("isHPCritical", false);
            }

            animator.SetTrigger("takeDamage");
        }
    }
    //public void SetKills(int kills)
    //{
    //   // this.kills.text = kills.ToString();
    //}

    public void AddKill(PlayerScript kill)
    {
        //Image sr = GameObject.Instantiate(killTag, killTags).GetComponent<Image>();
        //sr.sprite = kill.killTag;
    }

    const float HEALTH_ANIM_RATE = 10f;

    float fillAmount;

    private void Update()
    {
       
        if (GameManager.Instance.isGameStarted)
        {
            if (fillAmount != playerHealthBar.fillAmount)
            {
                playerHealthBar.fillAmount = Mathf.Lerp(playerHealthBar.fillAmount, fillAmount, Time.deltaTime * HEALTH_ANIM_RATE);
            }
        }
        
    }

    public void Disable()
    {
        gameObject.SetActive(false);
    }

    public void SetGunText(GunSO gun)
    {
        //gunImage.sprite = gun.EquipSprite;
        /*
        currentWeaponText.text = gun.name;

        switch (gun.GunType)
        {
            case PlayerScript.GunType.pistol:
                gunImage = pistolImage;
                break;
            case PlayerScript.GunType.assaultRifle:
                gunImage = assaultRifleImage;
                break;
            case PlayerScript.GunType.LMG:
                gunImage = minigunImage;
                break;
            case PlayerScript.GunType.shotgun:
                gunImage = shotgunImage;
                break;
            case PlayerScript.GunType.railGun:
                gunImage = railgunImage;
                break;
            case PlayerScript.GunType.healthPack:
                break;
            case PlayerScript.GunType.RPG:
                gunImage = rocketLauncherImage;
                break;
            default:
                break;
        }

        DisableAllGunImagesExceptParamter(gunImage);
        */
    }

    void DisableAllGunImagesExceptParamter(Image gunImage)
    {
        /*
        pistolImage.enabled = false;
        assaultRifleImage.enabled = false;
        minigunImage.enabled = false;
        shotgunImage.enabled = false;
        railgunImage.enabled = false;
        rocketLauncherImage.enabled = false;

        gunImage.enabled = true;
        */
    }

    public void SetAmmoText(string msg, float fillAmount)
    {
        /*
        fillAmount = Mathf.Min(fillAmount, 0.9f);

        playerAmmoGun.text = msg;
        ammoFront.fillAmount = fillAmount;
        */
    }
    public void SetLives(int numLives, Sprite headSprite)
    {
        /*
        foreach (Transform child in stockHolder)
        {
            GameObject.Destroy(child.gameObject);
        }

        for (int i = 0; i < numLives; i++)
        {
            GameObject go = GameObject.Instantiate(headStock, stockHolder);
            go.GetComponent<Image>().sprite = headSprite;
            go.GetComponent<Image>().enabled = true;
        }
        */

    }
    public void LoseStock()
    {
        //   stockHolder.GetChild(stockHolder.childCount - 1).GetComponent<Animator>().SetTrigger("LoseStock");
    }


    public void Destroy()
    {
        // playerUILayout.enabled = false;
        GetComponent<Animator>().SetTrigger("Destroy");
    }

    public void EnableLayoutGroup()
    {
        Debug.Log(gameObject.name);
        // playerUILayout.enabled = true;
    }

    public void KillMeAfterAnim()
    {
        EnableLayoutGroup();
        Destroy(gameObject);
    }
}
