using Photon.Pun;
using Photon.Pun.UtilityScripts;
using Photon.Realtime;
using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class PhotonRoomCustomMatch : MonoBehaviourPunCallbacks, IInRoomCallbacks
{
    public static PhotonRoomCustomMatch room;
    private PhotonView PV;

    public bool isGameLoaded;
    public int currentScene;

    public string myRoomCode;

    //Player info
    private Player[] photonPlayers;
    public int playersInRoom;
    public int myNumberInRoom;

    public int playerInGame;

    //Delayed start
    private bool readyToCount;
    private bool readyToStart;
    public float startingTime;
    private float lessThanMaxPlayers;
    private float atMaxPlayer;
    private float timeToStart;

    public GameObject lobbyGameObject;
    public GameObject roomGO;
    public Transform playersPanel;
    public GameObject playerListingPrefab;
    public GameObject startButton;
    public GameObject copyButton;
    public Text roomCode;
    public InputField roomCodeField;
    public GameObject lobbyController;

    public GameObject roomCodeText;
   
    public GameObject waitGameText;
    public Button startGameButton;
    public GameObject gameLobbyText;
    public GameObject waitMasterText;

    public bool backButtonCreateRoomPressed = false;

    private void Awake()
    {
        //set up singleton
        if (PhotonRoomCustomMatch.room == null)
        {
            PhotonRoomCustomMatch.room = this;
        }
        else
        {
            if (PhotonRoomCustomMatch.room != this)
            {
                Object.Destroy(PhotonRoomCustomMatch.room);
                // Destroy(PhotonRoom.room.gameObject);
                PhotonRoomCustomMatch.room = this;
            }
        }
        DontDestroyOnLoad(this.gameObject);
    }

    //Use this for initialization
    void Start()
    {
        //set private variables
        PV = GetComponent<PhotonView>();
        readyToCount = false;
        readyToStart = false;
        lessThanMaxPlayers = startingTime;
        atMaxPlayer = 6;
        timeToStart = startingTime;
    }

    void Update()
    {
        //For delay start only, count down to start
        if (MultiplayerSettings.multiplayerSettings.delayStart)
        {
            if (playersInRoom == 1) RestartTimer();
            if (!isGameLoaded)
            {
                if (readyToStart)
                {
                    atMaxPlayer -= Time.deltaTime;
                    lessThanMaxPlayers = atMaxPlayer;
                    timeToStart = atMaxPlayer;
                }
                else if (readyToCount)
                {
                    lessThanMaxPlayers -= Time.deltaTime;
                    timeToStart = lessThanMaxPlayers;
                }
                Debug.Log("Display time to start to the players" + timeToStart);
                if (timeToStart <= 0)
                {
                    StartGame();
                }
            }
        }
    }

    public override void OnEnable()
    {
        //subscribe to functions
        base.OnEnable();
        PhotonNetwork.AddCallbackTarget(this);
        SceneManager.sceneLoaded += OnSceneFinishedLoading;
    }

    public override void OnDisable()
    {
        base.OnDisable();
        PhotonNetwork.RemoveCallbackTarget(this);
        SceneManager.sceneLoaded -= OnSceneFinishedLoading;
    }

    public GameObject GamePanel;
    //player join room
    public override void OnJoinedRoom()
    {


        if (PhotonNetwork.LocalPlayer.HasRejoined)
        {
            Debug.Log("Rejoin successful match");
        }
        else
        {
            if (DisconnectsRecovery.recovery.rejoinCalled)
            {
                Debug.Log("Rejoin successful match");
            }
            else
            {
                //PhotonLobbyCustomMatch.lobby.errorMessage.gameObject.SetActive(false);
                //sets player data when we join the room
                base.OnJoinedRoom();
                Debug.Log("We are in room now");
                lobbyGameObject.SetActive(false);
                roomGO.SetActive(true);

                gameLobbyText.SetActive(true);
                

                if (PhotonNetwork.IsMasterClient)
                {
                    startButton.SetActive(true);
                    //copyButton.SetActive(true);
                    //roomCode.gameObject.SetActive(true);
                    GamePanel.SetActive(true);
                    GamePanel.transform.GetChild(0).gameObject.SetActive(true);
                    GamePanel.transform.GetChild(1).gameObject.SetActive(true);
                    GamePanel.transform.GetChild(2).gameObject.SetActive(true);
                    
                    roomCode.text = PhotonNetwork.CurrentRoom.Name;

                    //enable text for master
                    roomCodeText.gameObject.SetActive(true);
                    

                    roomCodeField.gameObject.SetActive(true);
                    roomCodeField.text = PhotonNetwork.CurrentRoom.Name;
                }   
                else
                {
                    waitMasterText.SetActive(true);
                    GamePanel.SetActive(true);
                    GamePanel.transform.GetChild(0).gameObject.SetActive(false);
                    GamePanel.transform.GetChild(1).gameObject.SetActive(false);
                    GamePanel.transform.GetChild(2).gameObject.SetActive(false);
                    
                }
                myRoomCode = PhotonNetwork.CurrentRoom.Name;
                //clear current player list
                ClearPlayerListings();
                //add all players
                ListAllPlayers();

                photonPlayers = PhotonNetwork.PlayerList;
                playersInRoom = photonPlayers.Length;
                //PlayerID
                myNumberInRoom = playersInRoom;
               

                //for delay start only
                if (MultiplayerSettings.multiplayerSettings.delayStart)
                {
                    Debug.Log("Displayer players in room out of max players possible (" + playersInRoom + " : " + MultiplayerSettings.multiplayerSettings.maxPlayers + ")");
                    if (playersInRoom > 1)
                    {
                        readyToCount = true;
                    }
                    if (playersInRoom == MultiplayerSettings.multiplayerSettings.maxPlayers)
                    {
                        readyToStart = true;
                        if (!PhotonNetwork.IsMasterClient) return;
                        PhotonNetwork.CurrentRoom.IsOpen = false;
                    }
                }
            }
        }
    }

    //clear player list when play leave
    void ClearPlayerListings()
    {
        if (playersPanel)
        {
            for (int i = playersPanel.childCount - 1; i >= 0; i--)
            {
                if (playersPanel.GetChild(i).gameObject != null)
                    Destroy(playersPanel.GetChild(i).gameObject);
            }
        }
    }

    //add all players in room on list
    void ListAllPlayers()
    {
        if (!PhotonNetwork.LocalPlayer.HasRejoined)
            if (PhotonNetwork.InRoom)
            {
                foreach (Player player in PhotonNetwork.PlayerList)
                {
                    GameObject tempListing = Instantiate(playerListingPrefab, playersPanel);
                    Text tempText = tempListing.transform.GetChild(0).GetComponent<Text>();
                    tempText.text = player.NickName;
                }
            }
    }

    //add player to room list
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        if (!PhotonNetwork.LocalPlayer.HasRejoined)
        {
            base.OnPlayerEnteredRoom(newPlayer);
            Debug.Log("A new player has joinded the room");
            ClearPlayerListings();
            ListAllPlayers();

            photonPlayers = PhotonNetwork.PlayerList;
            playersInRoom++;
            if (MultiplayerSettings.multiplayerSettings.delayStart)
            {
                Debug.Log("Displayer players in room out of max players possible (" + playersInRoom + " : " + MultiplayerSettings.multiplayerSettings.maxPlayers + ")");
                if (playersInRoom > 1)
                {
                    readyToCount = true;
                }
                if (playersInRoom == MultiplayerSettings.multiplayerSettings.maxPlayers)
                {
                    readyToStart = true;
                    if (!PhotonNetwork.IsMasterClient) return;
                    //PhotonNetwork.CurrentRoom.IsOpen = false;
                }
            }
        }
    }

    public void StartGame()
    {
        //disable master load game if not all player in the room
        /*if (PhotonNetwork.CurrentRoom.MaxPlayers != playersInRoom)
        {
            Debug.LogWarning("Not enough players, room size is: " + PhotonNetwork.CurrentRoom.MaxPlayers);
            return;
        }*/
        waitGameText.gameObject.SetActive(true);
        startGameButton.interactable = false;
        isGameLoaded = true;
        if (!PhotonNetwork.IsMasterClient) return;
        if (MultiplayerSettings.multiplayerSettings.delayStart)
        {
            PhotonNetwork.CurrentRoom.IsOpen = false;
        }
        else
        {
            //PhotonNetwork.CurrentRoom.IsOpen = false;
        }
        PhotonNetwork.LoadLevel(MultiplayerSettings.multiplayerSettings.multiplayerScene);
    }

    void RestartTimer()
    {
        //restarts the time for when players leave the room(DelayStart)
        lessThanMaxPlayers = startingTime;
        timeToStart = startingTime;
        atMaxPlayer = 6;
        readyToCount = false;
        readyToStart = false;
    }

    void OnSceneFinishedLoading(Scene scene, LoadSceneMode mode)
    {
        //caled when multiplayer scene is loaded
        currentScene = scene.buildIndex;
        if (currentScene == MultiplayerSettings.multiplayerSettings.multiplayerScene)
        {
            isGameLoaded = true;
            //for delay start game 
            if (MultiplayerSettings.multiplayerSettings.delayStart)
            {
                //Master client load game scene
                PV.RPC("RPC_LoadedGameScene", RpcTarget.MasterClient);
            }
            else
            {
                RPC_CreatePlayer();
            }
        }
    }

    [PunRPC]
    private void RPC_LoadedGameScene()
    {
        playerInGame++;
        if (playerInGame == PhotonNetwork.PlayerList.Length)
        {
            Debug.LogWarning("CALL CREATING PLAYER FOR ALL");
            PV.RPC("RPC_CreatePlayer", RpcTarget.All);
        }
    }

    [PunRPC]
    private void RPC_CreatePlayer()
    {
        //creates player network controller but not player charachter
        PhotonNetwork.InstantiateRoomObject(Path.Combine("PhotonPrefabs", "PhotonNetworkPlayer"), transform.position, Quaternion.identity, 0);
    }

    //disconnect player
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        base.OnPlayerLeftRoom(otherPlayer);
        Debug.Log(otherPlayer.NickName + " has left the game");
        playersInRoom--;
        ClearPlayerListings();
        ListAllPlayers();
    }

    //button Copy clicked
    public void OnCopyButtonClicked()
    {
        //PhotonNetwork.CurrentRoom.Name.CopyToClipboard();

        WebGLCopyAndPaste webGLCopy = new WebGLCopyAndPaste();
        webGLCopy.ReceivePaste(PhotonNetwork.CurrentRoom.Name);
        //webGLCopy.GetClipboard("C");
        //webGLCopy.ReceivePaste(PhotonNetwork.CurrentRoom.Name);

        Debug.Log(PhotonNetwork.CurrentRoom.Name);
    }

    
}

//class for Clipboard
public static class ClipboardExtension
{
    //copy room Code to clipboard
    public static void CopyToClipboard(this string str)
    {
        GUIUtility.systemCopyBuffer = str;
    }
}



