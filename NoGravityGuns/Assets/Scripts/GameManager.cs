using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    private static GameManager _instance;

    public static GameManager Instance { get { return _instance; } }

    public bool isGameStarted;

    public List<PlayerScript> players;

    public CameraController cameraController;

    public JoiningPlayerScript joiningPlayerScript;

    public GunSO pistol;
    public GunSO shotgun;
    public GunSO LMG;
    public GunSO assaultRifle;
    public GunSO railGun;

    public GameObject playUIPrefab;
    public Transform playerUIParent;

    public float matchTime;

    public GUIManager guiManager;
    public EndGameScript EndGameScript;

    public float timer;

    public TextMeshProUGUI countdownText;

    public GameObject PlayerObject;

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

    DataManager dataManager;

    private void OnLevelWasLoaded(int level)
    {

        players.Clear();

        PlayerScript[] playersArray = FindObjectsOfType<PlayerScript>();

        for (int i = 0; i < playersArray.Length; i++)
        {
            players.Add(playersArray[i]);
        }

        joiningPlayerScript = FindObjectOfType<JoiningPlayerScript>();
        cameraController = FindObjectOfType<CameraController>();
        guiManager = FindObjectOfType<GUIManager>();
        EndGameScript = FindObjectOfType<EndGameScript>();
        playerUIParent = GameObject.FindGameObjectWithTag("UILayout").transform;
        countdownText = GameObject.Find("CountdownText").GetComponent<TextMeshProUGUI>();
        countdownText.gameObject.SetActive(false);

        isGameStarted = false;
        matchTime = 300;
        timer = 300;

        dataManager = GetComponent<DataManager>();
    }

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
            DontDestroyOnLoad(this.gameObject);
        }

        isGameStarted = false;
        countdownText.gameObject.SetActive(false);
        dataManager = GetComponent<DataManager>();

    }

    private void Start()
    {
        //loading delay to prevent fuckupery
        StartCoroutine(Delay());
    }

    IEnumerator Delay()
    {
        yield return new WaitForSeconds(0.5f);
    }


    public void StartGame()
    {

        for (int i = players.Count - 1; i >= 0; i--)
        {

            if (players[i].playerID < 1)
            {
                players[i].gameObject.SetActive(false);
                players.Remove(players[i]);
            }
            else
            {
                SpawnPlayerPanel(players[i]);
                players[i].OnGameStart();
            }
        }


        //camera does its shit after
        cameraController.OnGameStart();

        joiningPlayerScript.OnGameStart();

        guiManager.OnGameStart();

        dataManager.OnGameStart();

        StartCoroutine(Countdown());
    }

    IEnumerator Countdown()
    {     
        countdownText.text = "3";
        countdownText.gameObject.SetActive(true);
        yield return new WaitForSeconds(0.75f);
        countdownText.text = "2";
        yield return new WaitForSeconds(0.75f);
        countdownText.text = "1";
        yield return new WaitForSeconds(0.75f);
        countdownText.text = "Go!";
        isGameStarted = true;
        timer = matchTime;
        yield return new WaitForSeconds(0.25f);

        countdownText.gameObject.SetActive(false);
    }
    
    private void SpawnPlayerPanel(PlayerScript player)
    {
        PlayerUIPanel gO = Instantiate(playUIPrefab, playerUIParent).GetComponent<PlayerUIPanel>();
        gO.setAll((float)player.health / 100f, player.playerName, player.armsScript.GunInfo(), player.playerColor);
        player.playerUIPanel = gO;

    }
    private void Update()
    {
        if(isGameStarted)
            timer -= Time.deltaTime;

        if (timer <= 0 && isGameStarted)
        {
            isGameStarted = false;
            OnGameEnd();
        }
    }


    public void OnGameEnd()
    {

        List<PlayerScript> winner = new List<PlayerScript>();

        foreach (var player in players)
        {
            if (player.numLives > 0)
                winner.Add(player);
        }

        dataManager.OnGameEnd(winner);

        EndGameScript.StartEndGame(winner);

    }

    public void CheckForLastManStanding()
    {
        int leftAlive = 0;

        foreach (var player in players)
        {
            if (player.numLives > 0)
                leftAlive++;
        }

        if (leftAlive == 1)
        {
            OnGameEnd();
        }

    }

    List<PlayerScript> DetermineWinner()
    {
        int highestkills = 0;
        List<PlayerScript> highestKillPlayer = new List<PlayerScript>();

        for (int i = 0; i < players.Count; i++)
        {
            if (players[i].numKills == highestkills)
            {
                highestKillPlayer.Add(players[i]);
                highestkills = players[i].numKills;
            }
            else if (players[i].numKills > highestkills)
            {
                highestKillPlayer.Clear();
                highestKillPlayer.Add(players[i]);
                highestkills = players[i].numKills;
            }


        }

        return highestKillPlayer;
    }


}
