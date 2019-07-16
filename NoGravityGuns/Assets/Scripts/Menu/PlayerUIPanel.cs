using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class PlayerUIPanel : MonoBehaviour
{
    public Image playerHealthBar;
    public TextMeshProUGUI playerStatusText;
    public TextMeshProUGUI playerAmmoGun;
    public Transform stockHolder;
    public GameObject headStock;

    public void setAll(float fillDamage, string statusMsg, string gunMsg, Color32 color)
    {
        setHealth(fillDamage);
        setStatusText(statusMsg);
        setGun(gunMsg);

        foreach (var text in GetComponentsInChildren<TextMeshProUGUI>())
        {
            text.color = color;
        }

    }
    public void setHealth(float fillDamage)
    {
        playerHealthBar.fillAmount = fillDamage;
    }
    public void setStatusText(string msg)
    {
        playerStatusText.text = msg;
    }
    public void setGun(string msg)
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
}
