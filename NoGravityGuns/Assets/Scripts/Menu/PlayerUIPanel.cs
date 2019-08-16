using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class PlayerUIPanel : MonoBehaviour
{
    public Image playerPortrait;
    public Image playerHealthBar;
    public TextMeshProUGUI currentWeaponText;
    public TextMeshProUGUI playerAmmoGun;
    public Transform stockHolder;
    public GameObject headStock;
    public Image gunImage;
    public HorizontalLayoutGroup playerUILayout;
    public TextMeshProUGUI kills;
    GameManager gameManager;

    private void Awake()
    {
        gameManager = GameManager.Instance;
    }

    public void setAll(float fillDamage, string statusMsg, string gunMsg, Color32 color, Sprite playerPortrait, Sprite healthbar)
    {

        setHealth(fillDamage);
        //SetGunText(statusMsg);
        SetAmmoText(gunMsg);
        kills.text = "0";
        foreach (var text in GetComponentsInChildren<TextMeshProUGUI>())
        {
            text.color = color;
        }

        this.playerPortrait.sprite = playerPortrait;
        this.playerHealthBar.sprite = healthbar;
        this.playerUILayout = transform.parent.GetComponent<HorizontalLayoutGroup>();

    }
    public void setHealth(float fillDamage)
    {
        fillAmount = fillDamage;
    }
    public void SetKills(int kills)
    {
        this.kills.text = kills.ToString();
    }

    const float HEALTH_ANIM_RATE = 10f;

    float fillAmount;

    private void Update()
    {
        if (gameManager.isGameStarted)
        {
            if (fillAmount != playerHealthBar.fillAmount)
            {
                playerHealthBar.fillAmount = Mathf.Lerp(playerHealthBar.fillAmount, fillAmount, Time.deltaTime * HEALTH_ANIM_RATE);
            }
        }

    }

    //IEnumerator AnimateHealthBar(float fillToPoint, float fillAmount)
    //{
    //    //should never happen :D
    //    if(fillAmount == fillToPoint)
    //    {
    //        yield break;
    //    }

    //    if (fillAmount < fillToPoint)
    //    {
    //        while (fillAmount < fillToPoint)
    //        {
    //            playerHealthBar.fillAmount -= HEALTH_CHANGE_RATE;
    //        }
    //    }
    //    else if (fillAmount > fillToPoint)
    //    {

    //    }
    //}

    public void SetGunText(GunSO gun)
    {
        gunImage.sprite = gun.EquipSprite;
        currentWeaponText.text = gun.name;
    }
    public void SetAmmoText(string msg)
    {
        playerAmmoGun.text = msg;
    }
    public void SetLives(int numLives, Sprite headSprite)
    {
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

    }
    public void LoseStock()
    {
        stockHolder.GetChild(stockHolder.childCount - 1).GetComponent<Animator>().SetTrigger("LoseStock");
    }


    public void Destroy()
    {
        playerUILayout.enabled = false;
        GetComponent<Animator>().SetTrigger("Destroy");
    }

    public void EnableLayoutGroup()
    {
        playerUILayout.enabled = true;
    }

    public void KillMeAfterAnim()
    {
        Destroy(gameObject);
    }
}
