using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Rewired;
using System.Linq;

public class RoundEndCanvasScript : MonoBehaviour
{
    public GameObject endRoundPanel;
    public TextMeshProUGUI winnerText;
    public TextMeshProUGUI loserText;

    public Image bulletImage;

    public List<EndGameScoreStatus> endGameScoreStatuses;

    bool tickTimer = false;
    float timer = 0;
    bool weHaveAWinner = false;
    bool gameIsOver = false;

    public ParticleSystem firework1;
    public ParticleSystem firework2;

    CameraController cameraController;

    Animator animator;

    private void Awake()
    {

    }

    // Start is called before the first frame update
    void Start()
    {
        endRoundPanel.SetActive(false);

        cameraController = Camera.main.transform.root.GetComponent<CameraController>();
        animator = gameObject.transform.root.GetComponent<Animator>();

    }

    private void Update()
    {
        if (tickTimer)
            timer += Time.deltaTime;

        if(!gameIsOver)
        {
            firework1.Stop();
            firework2.Stop();
        }

        if (weHaveAWinner)
        {
            foreach (var player in ReInput.players.AllPlayers)
            {
                //A button end of round screen
                if (player.GetButtonDown("Join"))
                {
                    if (gameIsOver)
                    {
                        tickTimer = false;
                        timer = 0;
                        Debug.Log("starting new game!");
                        weHaveAWinner = false;
                        gameIsOver = false;
                        RoundManager.Instance.NewRound(true);
                    }
                    else
                    {
                        tickTimer = false;
                        timer = 0;
                        weHaveAWinner = false;
                        //SceneManager.LoadScene(SceneManager.GetActiveScene().name, LoadSceneMode.Single);
                        RoundManager.Instance.NewRound(false);
                    }

                    cameraController.ResetAllSlowdownEffects();
                }

                //timer ending of round endscreen
                if (timer >= 5f && gameIsOver == false)
                {
                    tickTimer = false;
                    weHaveAWinner = false;
                    timer = 0;
                    RoundManager.Instance.NewRound(false);
                }

                if (player.GetButtonDown("Drop"))
                {
                    //cameraController.ResetAllSlowdownEffects();
                    //SceneManager.LoadScene(mainMenuScene, LoadSceneMode.Single);
                }
            }
        }

    }


    public void EndRoundCanvasDisplay(Transform playerWhoWasHit, PlayerScript.DamageType damageType, Guns gunWhoShotYou)
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


        PlayerDataScript winningPlayerData = getWinningPlayerData(winningPlayer);

        if (winningPlayerData != null)
        {
            //give the winning player more round wins, check if they won for real
            winningPlayerData.IncreaseRoundWins();
            if (winningPlayerData.roundWins >= RoundManager.Instance.roundsToWin)
                gameIsOver = true;
        }

        //Sorts and displays round wins for end round screen
        SortAndDisplayRoundWins();

        if (!winningPlayer)
        {
            winnerText.text = "No one wins round!";
            loserText.text = "you all suck!";

            bulletImage.color = Color.white;
        }
        else
        {
            if (gameIsOver)
            {
                winnerTextString = winningPlayer.playerName + " wins the game!";

                firework1.GetComponent<UIParticleSystem>().enabled = true;
                firework2.GetComponent<UIParticleSystem>().enabled = true;

                var main1 = firework1.main;
                var main2 = firework2.main;

                main1.startColor = new ParticleSystem.MinMaxGradient(winningPlayer.playerColor);
                main2.startColor = new ParticleSystem.MinMaxGradient(winningPlayer.playerColor);

                firework1.Play();
                firework2.Play();

            }
            else
            {
                winnerTextString = winningPlayer.playerName + " wins the round!";
                firework1.Stop();
                firework2.Stop();
            }

            var winnerColour = winningPlayer.playerColor;


            PlayerScript playerWhoWasHitScript = playerWhoWasHit.gameObject.GetComponent<PlayerScript>();

            if (playerWhoWasHitScript)
            {

                if (playerWhoWasHitScript.playerLastHitBy)
                {
                    looserTextString = GetWittyCommentOnLastHitPoint(damageType, gunWhoShotYou);
                }
                //nobody hit you last, you killed yourself
                else
                {
                    looserTextString = GetWittyCommentOnLastHitPoint(PlayerScript.DamageType.self, null);
                }

            }

            winnerText.text = winnerTextString;
            //winnerText.color = winnerColour;
            loserText.text = looserTextString;
            //loserText.color = winnerColour;

            //sets as winner color with less opacity
            bulletImage.color = new Color32(winnerColour.r, winnerColour.g, winnerColour.b, 180);
        }

        weHaveAWinner = true;
        endRoundPanel.SetActive(true);


    }

    private PlayerDataScript getWinningPlayerData(PlayerScript winningPlayer)
    {
        foreach (var pd in RoundManager.Instance.playerDataList)
        {
            //figure out the playerData of our winner
            if (winningPlayer != null && pd.playerControllerData.ID == winningPlayer.playerID)
            {
                return pd;
            }
        }

        return null;
    }

    private void SortAndDisplayRoundWins()
    {
        animator.SetBool("ShowEndRoundPanel", true);

        //sort by round wins descending
        List<PlayerDataScript> SortedListOfPlayerDatas = RoundManager.Instance.playerDataList.OrderByDescending(o => o.roundWins).ToList();

        //turn off all slide-in text
        for (int i = 0; i < endGameScoreStatuses.Count; i++)
        {
            endGameScoreStatuses[i].gameObject.SetActive(false);
        }

        //turn on and change the text of all slide in components as required in order of round wins
        for (int i = 0; i < SortedListOfPlayerDatas.Count; i++)
        {
            if (SortedListOfPlayerDatas[i] != null)
            {
                endGameScoreStatuses[i].gameObject.SetActive(true);
                endGameScoreStatuses[i].SetNameAndScore("<color=" + SortedListOfPlayerDatas[i].hexColorCode + ">" + SortedListOfPlayerDatas[i].playerName + "</color>", "Rounds won: " + SortedListOfPlayerDatas[i].roundWins);
            }

        }
    }

    public string GetWittyCommentOnLastHitPoint(PlayerScript.DamageType damageType, Guns gunWhoShotYou)
    {
        string wit = string.Empty;
        string[] options = new[] { "Error! emptyString!" };

        int rand = 0;

        //roll a dice if you were shot by a gun to determine if we should comment on the gun instead
        if (gunWhoShotYou != null)
        {
            rand = Random.Range(0, 100);
        }

        //25% chance gun text
        if (rand > 75)
        {
            if (gunWhoShotYou.name == "RailGun")
            {
                options = new[] { "Pew! Pew!", "Zap!", "Watch out for bounce shots!" };
            }
            else if (gunWhoShotYou.name == "Pistol")
            {
                options = new[] { "How embarrasing", "Killed by a pea shooter!" };

            }
            else if (gunWhoShotYou.name == "Shotgun")
            {
                options = new[] { "PULL!", "Hunting season!" };

            }
            else if (gunWhoShotYou.name == "Minigun")
            {
                options = new[] { "Clever Minigun text!", "Shot to bits!" };

            }
            //TODO: be funnier, add more
        }

        //didn't find any options from the above attempt at making witty gun comments... current system demands we have at least 2 comments for each death type because of the error string
        if (options.Length <= 1)
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
                options = new[] { "A foot shot. How embarrassing", "A shoelace kill", "It's just a flesh wound" };

            }
            //this seems to only come up on collision damage
            else if (damageType == PlayerScript.DamageType.self)
            {
                options = new[] { "Whoo slow it down", "Smack!", "That left a small crater", "No need to rush", "Oof!" };

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
        if (animator != null)
            animator.SetBool("ShowEndRoundPanel", false);

        winnerText.text = string.Empty;
        loserText.text = string.Empty;
        endRoundPanel.SetActive(false);
    }



}
