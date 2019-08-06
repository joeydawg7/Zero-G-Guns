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

    public void setAll(float fillDamage, string statusMsg, string gunMsg, Color32 color, Sprite playerPortrait)
    {
        setHealth(fillDamage);
        //SetGunText(statusMsg);
        SetAmmoText(gunMsg);

        foreach (var text in GetComponentsInChildren<TextMeshProUGUI>())
        {
            text.color = color;
        }

        this.playerPortrait.sprite = playerPortrait;

    }
    public void setHealth(float fillDamage)
    {
        playerHealthBar.fillAmount = fillDamage;
    }
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
        foreach (Transform child in stockHolder)
        {
            child.GetComponent<Animator>().SetTrigger("LoseStock");
            return;
        }
    }


    public void Destroy()
    {
        GetComponent<Animator>().SetTrigger("Destroy");
    }

    public void KillMeAfterAnim()
    {
        Destroy(gameObject);
    }
}
