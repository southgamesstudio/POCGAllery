using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhotonLobby : MonoBehaviourPunCallbacks
{ 
    //lobby instance
    public static PhotonLobby lobby;

    //button for start game
    public GameObject startButton;
    public GameObject cancelButton;

    private void Awake()
    {
        //creates the singleton, lives withing the Main menu scene.
        lobby = this;
    }

    // Use this for initialization
    void Start()
    {
        //Connect to Master photon server
       
            PhotonNetwork.ConnectUsingSettings();
                

    }

    //Connect to Master server
    public override void OnConnectedToMaster()
    {
        Debug.Log("Player has conntected to the Photon master server");
        PhotonNetwork.AutomaticallySyncScene = true;
        startButton.SetActive(true);
    }

    //
    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log("Tried to join a random game but failed. There must be no open games available");
        CreateRoom();
    }

    //create new Room
    void CreateRoom()
    {
        int randomRoomName = Random.Range(0, 1000);
        RoomOptions roomOps = new RoomOptions() { IsVisible = true, IsOpen = true, MaxPlayers =(byte) MultiplayerSettings.multiplayerSettings.maxPlayers };
        PhotonNetwork.CreateRoom("Room" + randomRoomName, roomOps);
    }


    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        Debug.Log("Tried to create a new room but failed, there must be a room with the same name");
        CreateRoom();
    }

  
    //start game button
    public void OnStartButtonClicked()
    {
        startButton.SetActive(false);
        cancelButton.SetActive(true);
        PhotonNetwork.JoinRandomRoom();
    }

    public void OnCancelButtonClicked()
    {
        cancelButton.SetActive(false);
        startButton.SetActive(true);
        PhotonNetwork.LeaveRoom();
    }
    

    // Update is called once per frame
    void Update()
    {
        
    }
}
