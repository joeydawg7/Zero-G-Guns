using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine.EventSystems;
using Steamworks;
using System.Text;
using System.IO;

public class NetworkLauncher : MonoBehaviourPunCallbacks
{
    #region Public Fields

    public TextMeshProUGUI connectionStatusText;
    public TextMeshProUGUI YourUsernameText;
    public TextMeshProUGUI friendsNamesPrefab;
    public GameObject friendsPanel;

    public Button btnStartGame;

    #endregion

    #region Private Fields

    private HAuthTicket steamAuthSessionTicket;
    private List<KeyValuePair<string, CSteamID>> friendsList;
    private bool isLoggedIntoSteam;

    private List<Player> reservedPlayer;

    #endregion

    #region Private Fields

    /// <summary>
    /// This client's version number. Users are separated from each other by gameVersion (which allows you to make breaking changes).
    /// </summary>
    string gameVersion = "1";

    /// <summary>
    /// The maximum number of players per room. When a room is full, it can't be joined by new players, and so new room will be created.
    /// </summary>
    [Tooltip("The maximum number of players per room. When a room is full, it can't be joined by new players, and so new room will be created")]
    [SerializeField]
    private byte maxPlayersPerRoom = 4;

    private int playerCount;


    #endregion

    #region Unity Code

    void Awake()
    {
        reservedPlayer = new List<Player>();
        isLoggedIntoSteam = false;
        // #Critical
        // this makes sure we can use PhotonNetwork.LoadLevel() on the master client and all clients in the same room sync their level automatically
        
        playerCount = 0;
        //btnStartGame.Select();
        //EventSystem.current.SetSelectedGameObject(btnStartGame.gameObject);
        try
        {            
            SteamAPI.Init();            
            connectionStatusText.color = Color.white;            
        }
        catch
        {
            IsSteamNotLoggedIn();
            Debug.Log("Awake failed to Init Steam");
        }
       
        friendsList = new List<KeyValuePair<string, CSteamID>>();
    }

    // Start is called before the first frame update
    void Start()
    {        
        if(!PhotonNetwork.IsConnected)
        {
            if(isLoggedIntoSteam)
            {
                try
                {
                    //// #Critical, we must first and foremost connect to Photon Online Server.
                    //PhotonNetwork.ConnectUsingSettings();
                    //PhotonNetwork.GameVersion = gameVersion;           

                    var ticketReturnString = GetSteamAuthTicket(out steamAuthSessionTicket);

                    // do not set AuthValues.Token or authentication will fail
                    // connect
                    Debug.Log("Friend Count = " + SteamFriends.GetFriendCount(EFriendFlags.k_EFriendFlagAll));
                    PhotonNetwork.GameVersion = gameVersion;
                    PhotonNetwork.NickName = SteamFriends.GetPersonaName();                    
                    //GetFriendsList();                    
                    PhotonNetwork.ConnectUsingSettings();
                    Debug.Log("Connected = " + PhotonNetwork.IsConnected);
                    Debug.LogError("username " + SteamFriends.GetPersonaName() + " PhotonNetwork connected = " + PhotonNetwork.IsConnected);
                }
                catch
                {
                    IsSteamNotLoggedIn();
                }
                
            }
            else
            {
                Debug.Log("Start steam is logged in  = " + isLoggedIntoSteam);
            }          
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(PhotonNetwork.IsConnected)
        {
            if(connectionStatusText.text != "Connected")
            {
                connectionStatusText.text = "Connected";
            }            
        }        
        else
        {
            if(isLoggedIntoSteam)
            {
                if (connectionStatusText.text != "Not Connected")
                {
                    connectionStatusText.text = "Not Connected";
                }
            }
            Debug.Log("Update steam is logged in  = " + isLoggedIntoSteam);
        }

        if (!isLoggedIntoSteam)
        {
            IsSteamNotLoggedIn();           
            try
            {
                SteamAPI.Init();
                ConnectOnCommand();
            }
            catch
            {
                Debug.Log("Update Failed to Init Steam");
            }
            
        }

    }
    #endregion

    #region Custom Methods

    public void ButtonReadyandSelected()
    {
        btnStartGame.interactable = true;
        btnStartGame.Select();
        EventSystem.current.SetSelectedGameObject(btnStartGame.gameObject);
    }

    public void ConnectOnCommand()
    {
        isLoggedIntoSteam = true;
        PhotonNetwork.AutomaticallySyncScene = true;
        connectionStatusText.color = Color.white;
        //// #Critical, we must first and foremost connect to Photon Online Server.
        //PhotonNetwork.ConnectUsingSettings();
        //PhotonNetwork.GameVersion = gameVersion;           

        var ticketReturnString = GetSteamAuthTicket(out steamAuthSessionTicket);

        // do not set AuthValues.Token or authentication will fail
        // connect        
        PhotonNetwork.GameVersion = gameVersion;
        PhotonNetwork.NickName = SteamFriends.GetPersonaName();
        //GetFriendsList();
        PhotonNetwork.ConnectUsingSettings();     
        
    }


    public void StartGameOnClick()
    {
        if(reservedPlayer.Count == 0)
        {
            PhotonNetwork.JoinRandomRoom();            
        }
        else
        {
            
            //PhotonNetwork.CreateRoom(PhotonNetwork.NickName + Random.Range(1000,2000),new RoomOptions { MaxPlayers = maxPlayersPerRoom })
        }
    }

    public void InvitePlayer(string invitedPlayer)
    {
        var  playerList = PhotonNetwork.PlayerList;
        foreach(var player in playerList)
        {
            if(player.NickName == invitedPlayer)
            {
                if(reservedPlayer.Count == 0 || !reservedPlayer.Contains(player))
                {
                    reservedPlayer.Add(player);
                    break;
                }
                else if (reservedPlayer.Contains(player))
                {
                    break;
                }                
            }
        }
    }

    public void AddPlayerOnClick()
    {
        playerCount++;

    }

    public void PopulateFriendsList()
    {
        friendsList = GetFriendsList();
        if (friendsList.Count > 0)
        {
            friendsPanel.SetActive(true);
            for(int i = 0; i < friendsList.Count;i++)
            {
                var f = Instantiate(friendsNamesPrefab);
                f.transform.SetParent(friendsPanel.transform);
                var theText = f.GetComponent<TextMeshProUGUI>();
                theText.text = friendsList[i].Key;
                f.transform.localPosition = Vector3.zero;
                f.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);

                EPersonaState friendState = SteamFriends.GetFriendPersonaState(friendsList[i].Value);
                if (friendState == EPersonaState.k_EPersonaStateOnline || friendState == EPersonaState.k_EPersonaStateLookingToPlay)
                {
                    theText.color = Color.green;
                }
                if (friendState == EPersonaState.k_EPersonaStateAway || friendState == EPersonaState.k_EPersonaStateBusy || friendState == EPersonaState.k_EPersonaStateSnooze)
                {
                    theText.color = Color.yellow;
                }
                if(friendState == EPersonaState.k_EPersonaStateOffline || friendState == EPersonaState.k_EPersonaStateInvisible)
                {
                    theText.color = Color.red;
                }               
            }
        }        
    }

    public List<KeyValuePair<string, CSteamID>> SortFriendsList(List<KeyValuePair<string, CSteamID>> oldFriendsList)
    {
        List<KeyValuePair<string, CSteamID>> tempOldFriendsList = new List<KeyValuePair<string, CSteamID>>();
        tempOldFriendsList.AddRange(oldFriendsList);

        List<KeyValuePair<string, CSteamID>> olineFriendsList = new List<KeyValuePair<string, CSteamID>>();
        List<KeyValuePair<string, CSteamID>> awayFriendsList = new List<KeyValuePair<string, CSteamID>>();
        List<KeyValuePair<string, CSteamID>> offlineFriendsList = new List<KeyValuePair<string, CSteamID>>();

        foreach (KeyValuePair<string, CSteamID> kVp in tempOldFriendsList)
        {
            EPersonaState friendState = SteamFriends.GetFriendPersonaState(kVp.Value);
            if (friendState == EPersonaState.k_EPersonaStateOnline || friendState == EPersonaState.k_EPersonaStateLookingToPlay)
            {
                olineFriendsList.Add(kVp);
            }
            if (friendState == EPersonaState.k_EPersonaStateAway || friendState == EPersonaState.k_EPersonaStateBusy || friendState == EPersonaState.k_EPersonaStateSnooze)
            {
                awayFriendsList.Add(kVp);
            }
            if (friendState == EPersonaState.k_EPersonaStateOffline || friendState == EPersonaState.k_EPersonaStateInvisible)
            {
                offlineFriendsList.Add(kVp);
            }
            
        }
        List<KeyValuePair<string, CSteamID>> sortedFriendsList = new List<KeyValuePair<string, CSteamID>>();
        sortedFriendsList.AddRange(olineFriendsList);
        sortedFriendsList.AddRange(awayFriendsList);
        sortedFriendsList.AddRange(offlineFriendsList);        
        friendsList.Clear();
        return sortedFriendsList;
    }


    #endregion

    #region MonoBehaviourPunCallbacks Callbacks

    public override void OnConnected()
    {
        isLoggedIntoSteam = true;        
        SteamUser.CancelAuthTicket(steamAuthSessionTicket);
        PopulateFriendsList();
        ButtonReadyandSelected();
    }

    public override void OnConnectedToMaster()
    {        
        Debug.Log("PUN Basics tutorial/Launcher: OnConnectedToMaster() was called by PUN");
        Debug.Log("Connection status = " + PhotonNetwork.IsConnected + " player count = " + PhotonNetwork.CountOfPlayers);
        //PhotonNetwork.JoinRandomRoom();     
        YourUsernameText.text = PhotonNetwork.NickName;
        playerCount++;
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.LogWarningFormat("PUN Basics Tutorial/Launcher: OnDisconnected() was called by PUN with reason {0}", cause);
        
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {       
        Debug.Log("PUN Basics Tutorial/Launcher:OnJoinRandomFailed() was called by PUN. No random room available, so we create one.\nCalling: PhotonNetwork.CreateRoom");

        // #Critical: we failed to join a random room, maybe none exists or they are all full. No worries, we create a new room.
        PhotonNetwork.CreateRoom(null, new RoomOptions { MaxPlayers = maxPlayersPerRoom });
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("PUN Basics Tutorial/Launcher: OnJoinedRoom() called by PUN. Now this client is in a room.");
        Debug.Log("# of Players in Room = " + PhotonNetwork.CurrentRoom.PlayerCount + "Current # of Rooms = " + PhotonNetwork.CountOfRooms);
        // #Critical: We only load if we are the first player, else we rely on `PhotonNetwork.AutomaticallySyncScene` to sync our instance scene.
        if (PhotonNetwork.CurrentRoom.PlayerCount == 1)
        {
            Debug.Log("Load NetworkPlayerJoinScreen");

            // #Critical
            // Load the Room Level.
            PhotonNetwork.LoadLevel("NetworkPlayerJoinScreen");
        }
    }

    #endregion

    #region Steam Methods

    // hAuthTicket should be saved so you can use it to cancel the ticket as soon as you are done with it
    public string GetSteamAuthTicket(out HAuthTicket hAuthTicket)
    {
        try
        {
            byte[] ticketByteArray = new byte[1024];
            uint ticketSize;
            hAuthTicket = SteamUser.GetAuthSessionTicket(ticketByteArray, ticketByteArray.Length, out ticketSize);
            System.Array.Resize(ref ticketByteArray, (int)ticketSize);
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < ticketSize; i++)
            {
                sb.AppendFormat("{0:x2}", ticketByteArray[i]);
            }
            return sb.ToString();
        }
        catch
        {
            hAuthTicket = new HAuthTicket();
            IsSteamNotLoggedIn();
            return string.Empty;            
        }
       
    }

    public List<KeyValuePair<string, CSteamID>> GetFriendsList()
    {
        int friendCount = SteamFriends.GetFriendCount(EFriendFlags.k_EFriendFlagImmediate);
        Debug.Log("[STEAM-FRIENDS] Listing " + friendCount + " Friends.");        
        for (int i = 0; i < friendCount; ++i)
        {
            CSteamID friendSteamId = SteamFriends.GetFriendByIndex(i, EFriendFlags.k_EFriendFlagImmediate);
            string friendName = SteamFriends.GetFriendPersonaName(friendSteamId);            
            var friend = new KeyValuePair<string, CSteamID>(friendName, friendSteamId);
            
            friendsList.Add(friend);            
        }

        return SortFriendsList(friendsList);
    }

    public void IsSteamNotLoggedIn()
    {
        connectionStatusText.text = "Steam Login Required";
        connectionStatusText.color = Color.red;
        isLoggedIntoSteam = false;
        btnStartGame.interactable = false;
    }

    #endregion
}
