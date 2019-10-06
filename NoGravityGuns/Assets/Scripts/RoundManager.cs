using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;

public class RoundManager : MonoBehaviour
{
    #region singleton stuff
    private static RoundManager _instance;
    public static RoundManager Instance { get { return _instance; } }
    #endregion

    [Header("DEBUG")]
    public bool debugStayOnThisScene;
    [Header("----------")]


    public int maxRounds;

    [Header("Round-Fluid elements")]
    GameObject newRoundElementBacker;
    TextMeshProUGUI newRoundText;
    TextMeshProUGUI roundNumText;

    Animator newRoundTextAnimator;

    public List<RoomSO> rooms;

    public bool finishedControllerSetup;

    [HideInInspector]
    public int currentRound;
    [HideInInspector]
    public float timeSinceRoundStarted;

    public List<PlayerDataScript> playerDataList;
    public GameObject playerDataPrefab;
    public Image loadingImage;
    public GameObject persistentCanvas;

    JoiningPlayerScript joiningPlayerScript;

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
            DontDestroyOnLoad(persistentCanvas);
            DontDestroyOnLoad(loadingImage.gameObject);

            foreach (var room in rooms)
            {
                room.isPlayable = true;
            }

                // ObjectPooler.Instance.StartUp();
            }

        finishedControllerSetup = false;

        currentRound = 0;
        loadingImage.gameObject.SetActive(false);

        Cursor.visible = false;

        joiningPlayerScript = FindObjectOfType<JoiningPlayerScript>();

    }

    public void NewRound(bool startOver)
    {
        Debug.Log("starting new round");
        loading = true;
        loadingImage.fillAmount = 0;
        currentRound++;
        timeSinceRoundStarted = 0;


        if (startOver)
        {
            currentRound = 0;
            finishedControllerSetup = false;

            if (playerDataList.Count > 0)
            {
                for (int i = playerDataList.Count-1; i >=0; i--)
                {
                    Destroy(playerDataList[i]);
                }

                playerDataList.Clear();
            }

        }

        //TODO: only grab from a list of playable rooms so player can check off maps they dont want to play
        //RoomSO nextRoom = rooms[Random.Range(0, rooms.Count)];

        List<RoomSO> tempRooms = new List<RoomSO>();

        RoomSO nextRoom = null;

        //we have no room to go to!
        while(nextRoom == null)
        {
            //make a list of all possible rooms we could go to that are playable
            foreach (var room in rooms)
            {
                if (room.isPlayable)
                {
                    tempRooms.Add(room);
                   // Debug.Log(room.name);
                }
            }

            //Debug.Log("---------------");

            //if our list has no playable rooms make everything playable
            if (tempRooms.Count < 1)
            {
                foreach (var room in rooms)
                {
                    room.isPlayable = true;
                }
            }
            else
            {
                //then pick a random room from everything playable
                nextRoom = tempRooms[Random.Range(0, tempRooms.Count)];
            }

        }


        StartCoroutine(AddLevel(nextRoom.sceneName, nextRoom, startOver));
       
        ClearEndRoundCanvasDisplay();
    }

    bool loading = false;

    IEnumerator AddLevel(string lvl, RoomSO nextRoom, bool startOver)
    {
        loadingImage.gameObject.SetActive(true);


        //DEBUG: use original scene
        if (debugStayOnThisScene && GameManager.Instance.debugManager.useDebugSettings)
        {
            lvl = SceneManager.GetActiveScene().name;

            //figure out which room we are in an set that as the real next room 
            for (int i = 0; i < rooms.Count; i++)
            {
                if (rooms[i].sceneName == lvl)
                {
                    nextRoom = rooms[i];
                    break;
                }
            }

        }

        ObjectPooler.Instance.ResetRound();

        AsyncOperation asyncLoadLevel = SceneManager.LoadSceneAsync(lvl); 

        while (!asyncLoadLevel.isDone )
        {
            yield return null;
        }

        

        LevelLoaded(nextRoom, startOver);
        yield return new WaitForSeconds(0.5f);

    }

    void LevelLoaded(RoomSO nextRoom, bool startOver)
    {
        if (!startOver)
        {
            newRoundElementBacker = GameObject.Find("roomSetupBacker");
            newRoundTextAnimator = newRoundElementBacker.GetComponent<Animator>();
            newRoundText = newRoundElementBacker.transform.Find("RoomNamePopup").GetComponent<TextMeshProUGUI>();

            newRoundText.alpha = 1;
            newRoundText.text = nextRoom.roomName;
            roundNumText = newRoundText.transform.Find("RoundNum").GetComponent<TextMeshProUGUI>();
            roundNumText.alpha = 1;
            roundNumText.text = "Round " + currentRound + "";

            newRoundTextAnimator.SetTrigger("NewRound");

            Debug.Log("spawning players");
            foreach (var PD in playerDataList)
            {
                PD.SpawnAtMatchingPoint();
            }


            GameManager.Instance.StartGame();

           
        }


       

    }



    //public void SetPlayer(PlayerScript player)
    //{

    //    //only do setup if its the first round
    //    if (currentRound == 0)
    //    {

    //        PlayerDataScript PD = GameObject.Instantiate(playerDataPrefab).GetComponent<PlayerDataScript>();

    //        DontDestroyOnLoad(PD);

    //        PD.playerID = player.playerID;
    //        PD.controller = player.controller;
    //        PD.player = player.player;
    //        PD.hexColorCode = player.hexColorCode;
    //        PD.playerName = player.playerName;

    //        playerDataList.Add(PD);

    //    }

    //}

    public void SpawnPlayerManager(PlayerControllerData playerControllerData, GlobalPlayerSettingsSO globalPlayerSettings)
    {
        PlayerDataScript PD = GameObject.Instantiate(playerDataPrefab).GetComponent<PlayerDataScript>();
        DontDestroyOnLoad(PD);

        PD.SetPlayerInfoAfterRoundStart(playerControllerData, globalPlayerSettings);

        PD.SpawnAtMatchingPoint();

        playerDataList.Add(PD);

    }



    private void Update()
    {

        //spins a fun lil' loading spinner
        if (loading)
        {
            loadingImage.fillAmount += Time.deltaTime;

            if(loadingImage.fillAmount>=1)
            {
                loadingImage.fillAmount = 0;
                loading = false;
            }

        }
        else
        {
            loadingImage.gameObject.SetActive(false);

            timeSinceRoundStarted += Time.deltaTime;

        }
    }

    public void EndRoundCanvasDisplay(Transform playerWhoWasHit)
    {
        var endRoundPanel = GameObject.FindGameObjectWithTag("EndRoundPanel");
        var winnerText = GameObject.FindGameObjectWithTag("EndRoundWinnerText").GetComponent<TextMeshProUGUI>();
        var loserText = GameObject.FindGameObjectWithTag("EndRoundLoserText").GetComponent<TextMeshProUGUI>();
        string winnerTextString = string.Empty;
        string looserTextString = string.Empty;

        var players = GameObject.FindGameObjectsWithTag("Player");
        PlayerScript winningPlayer = null;
        foreach(var p in players)
        {
            if(p.GetComponent<PlayerScript>().numLives > 0)
            {
                winningPlayer = p.GetComponent<PlayerScript>();
            }
        }

        if(!winningPlayer)
        {
            winnerTextString = "No one wins round!";
        }
        else
        {
            winnerTextString = winningPlayer.playerName + " wins round!";
        }
       

        if (playerWhoWasHit.gameObject.GetComponent<PlayerScript>().playerLastHitBy)
        {
            looserTextString = GetWittyCommentOnLastHitPoint(playerWhoWasHit.gameObject.GetComponent<PlayerScript>().playerLastHitBy.lastHitDamageType);
        }
        else
        {
            looserTextString = GetWittyCommentOnLastHitPoint(PlayerScript.DamageType.self);
        }
        

        var winnerColour = winningPlayer.playerColor;

        winnerText.text = winnerTextString;
        //winnerText.color = winnerColour;
        loserText.text = looserTextString;
        //loserText.color = winnerColour;

        var bulletTrails = endRoundPanel.GetComponentsInChildren<Image>();
        foreach(var bt in bulletTrails)
        {
            if(bt.sprite.name == "BulletPaneTraill")
            {
                bt.color = winnerColour;
            }
        }


        endRoundPanel.SetActive(true);
    }

    public void ClearEndRoundCanvasDisplay()
    {
        var endRoundPanel = GameObject.FindGameObjectWithTag("EndRoundPanel");
        var winnerText = GameObject.FindGameObjectWithTag("EndRoundWinnerText").GetComponent<TextMeshProUGUI>();
        var loserText = GameObject.FindGameObjectWithTag("EndRoundLoserText").GetComponent<TextMeshProUGUI>();
       
        winnerText.text = string.Empty;
        loserText.text = string.Empty;
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
        /*else*/ if (damageType == PlayerScript.DamageType.head)
        {

            string[] options = new[] { "Right in the face", "Oh his brain", "Helmets only do so much", "Bullets and your head a deadly combination" };
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


}
