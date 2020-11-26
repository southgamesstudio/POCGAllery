using UnityEngine;
using Photon.Pun;
using Photon.Realtime;


namespace com.PT.contest
{

    public class Launcher : MonoBehaviourPunCallbacks
    {
       
        bool on;


        private void Start()
        {
            PhotonNetwork.AutomaticallySyncScene = true;
            PhotonNetwork.ConnectUsingSettings();
        }

        public override void OnConnectedToMaster()
        {
            if (Screen.fullScreen) Screen.fullScreen = false;
            PhotonNetwork.JoinRandomRoom();
        }


        public override void OnCreateRoomFailed(short returnCode, string message)
        {
            Debug.Log("FAILED TO CREATE ROOM");
        }
        public override void OnJoinRandomFailed(short returnCode, string message)
        {
            Debug.Log("Failed to join room");
            Create();
        }
        public override void OnCreatedRoom()
        {
            PhotonNetwork.JoinRandomRoom();
        }

        public override void OnJoinedRoom()
        {
            StartGame_POC(1);
        }
        public void Create()
        {
            // max player of rooms is set to 3
            RoomOptions roomOps = new RoomOptions() { IsVisible = true, IsOpen = true, MaxPlayers = (byte)3 };
            int roomRandomNumber = Random.Range(0, 999);
            PhotonNetwork.CreateRoom("Room " + roomRandomNumber, roomOps);
        }


        public void Connect()
        {
            PhotonNetwork.GameVersion = "0.0.0";
            PhotonNetwork.ConnectUsingSettings();
        }


      


        public void StartGame_POC(int value)
        {

            PhotonNetwork.LoadLevel(value);

        }
    }


}