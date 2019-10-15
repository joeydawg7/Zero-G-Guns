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

    public void EndRoundCanvasDisplay(Transform playerWhoWasHit, PlayerScript.DamageType damageType, GunSO gunWhoShotYou)
    {

        string winnerTextString = string.Empty;
        string looserTextString = string.Empty;

        var players = FindObjectsOfType<PlayerScript>();
        PlayerScript winningPlayer = null;
        foreach (var p in players)
        {
            if (p.numLives > 0)
            {
                winningPlayer = p;
            }
        }

        if (!winningPlayer)
        {
            winnerText.text = "No one wins round!";
            loserText.text = "you all suck!";
            var bulletTrails = endRoundPanel.GetComponent<Image>();
            bulletTrails.color = Color.white;
        }
        else
        {
            winnerTextString = winningPlayer.playerName + " wins round!";
            var winnerColour = winningPlayer.playerColor;

            PlayerScript playerWhoWasHitScript = playerWhoWasHit.gameObject.GetComponent<PlayerScript>();


            if (playerWhoWasHitScript.playerLastHitBy)
            {
                looserTextString = GetWittyCommentOnLastHitPoint(damageType, gunWhoShotYou);
            }
            //nobody hit you last, you killed yourself
            else
            {
                looserTextString = GetWittyCommentOnLastHitPoint(PlayerScript.DamageType.self, null);
            }


            winnerText.text = winnerTextString;
            //winnerText.color = winnerColour;
            loserText.text = looserTextString;
            //loserText.color = winnerColour;

            var bulletTrails = endRoundPanel.GetComponent<Image>();
            bulletTrails.color = winnerColour;
        }      

        endRoundPanel.SetActive(true);

    }

    public string GetWittyCommentOnLastHitPoint(PlayerScript.DamageType damageType, GunSO gunWhoShotYou)
    {
        string wit = string.Empty;
        string[] options = new[] { "Error! emptyString!" };

        int rand = 0;

        //roll a dice if you were shot by a gun to determine if we should comment on the gun instead
        if(gunWhoShotYou !=null)
        {
            rand = Random.Range(0, 100);
        }

        if (rand > 75)
        {
            if (gunWhoShotYou.name == "RailGun")
            {
                options = new[] { "Pew! Pew!", "Zap!", "Watch out for bounce shots!"};
            }
            else if (gunWhoShotYou.name == "Pistol")
            {
                options = new[] { "How embarrasing", "Killed by a pea shooter!"};

            }
            else if (gunWhoShotYou.name == "Shotgun")
            {
                options = new[] { "PULL!", "Hunting season!"};

            }
            //TODO: be funnier, add more
        }

        //didn't find any options from the above attempt at making witty gun comments... current system demands we have at least 2 comments for each death type because of the error string
        if(options.Length <=1)
        {
            if (damageType == PlayerScript.DamageType.head)
            {

                options = new[] { "In the face!", "Oh no not in the brain", "Helmets only do so much", "Bullets and your head a deadly combination" };

            }
            else if (damageType == PlayerScript.DamageType.torso)
            {
                options = new[] { "Gut shot for the win", "That's gonna cause a tummy ache", "Oh that's gonna sting", "Who needs a heart anyhow" };

            }
            else if (damageType == PlayerScript.DamageType.legs)
            {
                options = new[] { "Took a bullet in the knee", "That's gonna cause a limp", "Good thing you have a second leg", "who needs knees anyhow", "T'is but a scratch" };

            }
            else if (damageType == PlayerScript.DamageType.feet)
            {
                options = new[] { "A foot shot how embarrassing", "A shoelace kill", "It's just a flesh wound" };

            }
            //this seems to only come up on collision damage
            else if (damageType == PlayerScript.DamageType.self)
            {
                options = new[] { "Whoo slow  it down", "Smack!", "that left a small crater", "No need to rush" };

            }
            else if (damageType == PlayerScript.DamageType.explosive)
            {
                options = new[] { "BOOM baby!", "Kablowie!", "Explosive!", "Boom da boom" };
            }
            else
            {
                options = new[] { "Whoa this should never come up!", "You shouldn't see this!", "This code sucks!" };
            }
        }

        int r = Random.Range(0, options.Length);
        wit = options[r];

        return wit;
    }


    public void ClearEndRoundCanvasDisplay()
    {

        winnerText.text = string.Empty;
        loserText.text = string.Empty;
        endRoundPanel.SetActive(false);
    }



}
