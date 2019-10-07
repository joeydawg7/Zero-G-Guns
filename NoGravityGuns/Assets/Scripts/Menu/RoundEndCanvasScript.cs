using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class RoundEndCanvasScript : MonoBehaviour
{
    public GameObject endRoundPanel;
    public TextMeshProUGUI winnerText;
    public TextMeshProUGUI loserText;

    // Start is called before the first frame update
    void Start()
    {
        endRoundPanel.SetActive(false);
    }

    public void EndRoundCanvasDisplay(Transform playerWhoWasHit, PlayerScript.DamageType damageType)
    {

        string winnerTextString = string.Empty;
        string looserTextString = string.Empty;

        var players = GameObject.FindGameObjectsWithTag("Player");
        PlayerScript winningPlayer = null;
        foreach (var p in players)
        {
            if (p.GetComponent<PlayerScript>().numLives > 0)
            {
                winningPlayer = p.GetComponent<PlayerScript>();
            }
        }

        if (!winningPlayer)
        {
            winnerTextString = "No one wins round!";
        }
        else
        {
            winnerTextString = winningPlayer.playerName + " wins round!";
        }


        PlayerScript playerWhoWasHitScript = playerWhoWasHit.gameObject.GetComponent<PlayerScript>();

        
        if (playerWhoWasHitScript.playerLastHitBy)
        {
            looserTextString = GetWittyCommentOnLastHitPoint(damageType);
        }
        //nobody hit you last, you killed yourself
        else
        {
            looserTextString = GetWittyCommentOnLastHitPoint(PlayerScript.DamageType.self);
        }


        var winnerColour = winningPlayer.playerColor;

        winnerText.text = winnerTextString;
        //winnerText.color = winnerColour;
        loserText.text = looserTextString;
        //loserText.color = winnerColour;

        var bulletTrails = endRoundPanel.GetComponent<Image>();

        bulletTrails.color = winnerColour;
        endRoundPanel.SetActive(true);

    }

    public string GetWittyCommentOnLastHitPoint(PlayerScript.DamageType damageType)
    {
        string wit = string.Empty;

        //if (damageType == PlayerScript.DamageType.none)
        //{

        //    string[] options = new[] { "Act of God", "Wha...happened", "Huh...what the what?", "Don't ask me", "Your guess is as good as ours" };
        //    int r = Random.Range(0, options.Length - 1);
        //    wit = options[r];
        //}
        /*else*/
        if (damageType == PlayerScript.DamageType.head)
        {

            string[] options = new[] { "In the face!", "Oh his brain", "Helmets only do so much", "Bullets and your head a deadly combination" };
            int r = Random.Range(0, options.Length - 1);
            wit = options[r];
        }
        else if (damageType == PlayerScript.DamageType.torso)
        {
            string[] options = new[] { "Gut shot for the win", "That's gonna cause a tummy ache", "Oh that's gonna sting", "Who needs a heart to live" };
            int r = Random.Range(0, options.Length - 1);
            wit = options[r];
        }
        if (damageType == PlayerScript.DamageType.legs)
        {
            string[] options = new[] { "You took a bullet in the knee", "That's gonna cause a limp", "Good thing you have a second leg", "who needs knee anyhow", "Tis but a scratch" };
            int r = Random.Range(0, options.Length - 1);
            wit = options[r];
        }
        if (damageType == PlayerScript.DamageType.feet)
        {
            string[] options = new[] { "A foot shot how embarrassing", "A shoelace kill", "It's just a flesh wound" };
            int r = Random.Range(0, options.Length - 1);
            wit = options[r];
        }
        if (damageType == PlayerScript.DamageType.self)
        {
            string[] options = new[] { "That was all you", "They just gave up on life", "Good by crule world" };
            int r = Random.Range(0, options.Length - 1);
            wit = options[r];
        }

        return wit;
    }


    public void ClearEndRoundCanvasDisplay()
    {

        winnerText.text = string.Empty;
        loserText.text = string.Empty;
        endRoundPanel.SetActive(false);
    }



}
