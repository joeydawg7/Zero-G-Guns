using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private static GameManager _instance;

    public static GameManager Instance { get { return _instance; } }

    [Header("Debug")]
    public bool debugSkipCountdown;
    [Header("----------")]
    public bool isGameStarted;

    [HideInInspector]
    public List<PlayerScript> players;
    
    public CameraController cameraController;

    public JoiningPlayerScript joiningPlayerScript;
    public AddSinglePlayer singlePlayerAddingScript;

    [Header("Stores DebugManager scriptable object to easily turn debug options on / off")]
    public DebugManager debugManager;

    public Guns pistol;

    public float matchTime;

    public EndGameScript EndGameScript;   

    [HideInInspector]
    public float timer;

    [HideInInspector]
    public float timeSinceRoundStarted;

    public TextMeshProUGUI countdownText;

    [HideInInspector]
    public GameObject PlayerObject;

    public AudioClip countdownBeep;
    public AudioClip startingBeep;

    [HideInInspector]
    public AudioSource audioSource;

    public bool ranFromPersistentScene = false;

    [HideInInspector]
    public bool stopTimer;

    #region Unused Sprite Storage
    //[Header("Red Sprites")]
    //public Sprite redBody;
    //public Sprite redFrontLeg;
    //public Sprite redForeLeg;
    //public Sprite redFoot;
    //public Sprite redBackLeg;
    //public Sprite redBackForeLeg;
    //public Sprite redBackFoot;

    //[Header("Blue Sprites")]
    //public Sprite blueBody;
    //public Sprite blueFrontLeg;
    //public Sprite blueForeLeg;
    //public Sprite blueFoot;
    //public Sprite blueBackLeg;
    //public Sprite blueBackForeLeg;
    //public Sprite blueBackFoot;

    //[Header("green Sprites")]
    //public Sprite greenBody;
    //public Sprite greenFrontLeg;
    //public Sprite greenForeLeg;
    //public Sprite greenFoot;
    //public Sprite greenBackLeg;
    //public Sprite greenBackForeLeg;
    //public Sprite greenBackFoot;

    //[Header("yellow Sprites")]
    //public Sprite yellowBody;
    //public Sprite yellowFrontLeg;
    //public Sprite yellowForeLeg;
    //public Sprite yellowFoot;
    //public Sprite yellowBackLeg;
    //public Sprite yellowBackForeLeg;
    //public Sprite yellowBackFoot;
    #endregion
    /*
    #region Data Variables
    public float pistolKills;
    public float shotGunKills;
    public float railgunKills;
    public float minigunKills;
    public float assaultRifleKills;
    public float collisionKills;

    public float pistolDamage;
    public float shotGunDamage;
    public float railgunDamage;
    public float minigunDamage;
    public float assaultDamage;
    public float collisionDamage;
    public float healthPackHeals;

    public DataManager dataManager;
    #endregion
    */


    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }

        isGameStarted = false;
        countdownText.gameObject.SetActive(false);
        audioSource = GetComponent<AudioSource>();

        players = new List<PlayerScript>();
        cameraController = FindObjectOfType<CameraController>();
        if(FindObjectOfType<JoiningPlayerScript>())
        {
            joiningPlayerScript = FindObjectOfType<JoiningPlayerScript>();
        }
        if(FindObjectOfType<AddSinglePlayer>())
        {
            singlePlayerAddingScript = FindObjectOfType<AddSinglePlayer>();
        }
        
        EndGameScript = FindObjectOfType<EndGameScript>();
    }

    private void Start()
    {
    }

    PlayerSpawnPoint[] playerSpawnPoints;

    public void StartGame()
    {
        //RoundManager.Instance.SetAllPlayersDataIntoPlayerObjects();

        //get rid of players that nobody is playing as... again unless your a dummy
        //for (int i = players.Count - 1; i >= 0; i--)
        //{

        //    if (players[i].playerID < 1 && players[i].isDummy == false)
        //    {
        //        //players[i].transform.root.gameObject.SetActive(false);
        //        Debug.Log("removing " + i + "who has id of " + players[i].playerID);
        //        //players.Remove(players[i]);
        //    }
        //    else
        //    {
        //        SpawnPlayerPanel(players[i]);
        //        players[i].OnGameStart();
        //    }
        //}

        print("startGame called!");

        cameraController.OnGameStart();

        Time.timeScale = 1;
        if(joiningPlayerScript)
        {
            joiningPlayerScript.OnGameStart();
        }
        else if(singlePlayerAddingScript)
        {
            singlePlayerAddingScript.OnGameStart();
        }       

        PlayerScript[] ps = FindObjectsOfType<PlayerScript>();

        //make sure to add all players here, even if they are dummies
        for (int i = 0; i < ps.Length; i++)
        {
            ps[i].gameObject.SetActive(true);
            // if(ps[i].isDummy)
            players.Add(ps[i]);
        }
        StartCoroutine(Countdown());
    }

    //plays a short countdown
    IEnumerator Countdown()
    {
        if (debugSkipCountdown && debugManager.useDebugSettings)
        {
            audioSource.PlayOneShot(startingBeep);           
            timer = matchTime;
            countdownText.gameObject.SetActive(false);
        }
        else
        {
            yield return new WaitForSeconds(0.25f);
            countdownText.text = "3";
            countdownText.gameObject.SetActive(true);
            audioSource.PlayOneShot(countdownBeep);
            yield return new WaitForSeconds(0.75f);
            countdownText.text = "2";
            audioSource.PlayOneShot(countdownBeep);
            yield return new WaitForSeconds(0.75f);
            countdownText.text = "1";
            audioSource.PlayOneShot(countdownBeep);
            yield return new WaitForSeconds(0.75f);
            countdownText.text = "Go!";
            audioSource.PlayOneShot(startingBeep);
            timer = matchTime;
            yield return new WaitForSeconds(0.25f);
        }

        countdownText.gameObject.SetActive(false);
        isGameStarted = true;
        stopTimer = false;
    }

    //spawns GUI for every player in game
    private void SpawnPlayerPanel(PlayerScript player)
    {
        //PlayerUIPanel gO = Instantiate(playUIPrefab, playerUIParent).GetComponent<PlayerUIPanel>();
        //if (!player.isDummy)
        //{
        //    switch (player.playerID)
        //    {
        //        case 1:
        //            p1HUD.setAll((float)player.health / 100f, player.playerName, player.armsScript.AmmoText(), player.playerColor, player.healthBar);
        //            player.playerUIPanel = p1HUD;
        //            break;
        //        case 2:
        //            p2HUD.setAll((float)player.health / 100f, player.playerName, player.armsScript.AmmoText(), player.playerColor, player.healthBar);
        //            player.playerUIPanel = p2HUD;
        //            break;
        //        case 3:
        //            p3HUD.setAll((float)player.health / 100f, player.playerName, player.armsScript.AmmoText(), player.playerColor, player.healthBar);
        //            player.playerUIPanel = p3HUD;
        //            break;
        //        case 4:
        //            p4HUD.setAll((float)player.health / 100f, player.playerName, player.armsScript.AmmoText(), player.playerColor, player.healthBar);
        //            player.playerUIPanel = p4HUD;
        //            break;
        //        default:
        //            Debug.LogError("Should never get here!");
        //            break;
        //    }
        //}
    }

    private void Update()
    {
        //checks if we run out of time
        if (timer <= 0 && isGameStarted)
        {
            isGameStarted = false;
            OnGameEnd();
        }


        if (isGameStarted && !stopTimer)
            timeSinceRoundStarted += Time.deltaTime;

    }

    public void OnGameEnd()
    {


        //list of people who are still alive at match end
        List<PlayerScript> stillAlive = new List<PlayerScript>();

        //get all living players together, pass them along
        foreach (var player in players)
        {
            if (player.numLives > 0)
                stillAlive.Add(player);
        }

        //dataManager.OnGameEnd(stillAlive);

        EndGameScript.StartEndGame(stillAlive);

    }

    //every time somebody runs out of lives check if theres only 1 player left
    public bool CheckForLastManStanding()
    {
        int leftAlive = 0;

        foreach (var player in players)
        {
            if (player.numLives > 0)
                leftAlive++;
        }

        //last man standing, end the game!
        if (leftAlive == 1)
        {
            
            return true;
        }

        return false;

    }

}
