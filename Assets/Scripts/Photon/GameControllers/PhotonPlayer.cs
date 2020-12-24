using Photon.Pun;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System;

public class PhotonPlayer : MonoBehaviour
{
    public PhotonView PV;
    public GameObject mySheet;
    public static GameObject myAvatar;
    public static PhotonPlayer player;


    public int myTeam;
    public int MyTeam { get => myTeam; set => myTeam = value; }


    //random list
    //public int[] myRandomNumbers = new int[4];
    public List<int> AllRandomNumbers = new List<int>();

    //create Random generator and binary formatter
    private static System.Random rng = new System.Random();
    public string myData;
    private BinaryFormatter bf = new BinaryFormatter();

    public bool getList = false;
    public bool getTeam = false;
    public bool founded = false;
    public bool randomList;

    private static int playerViewsCounter = 2;


    void Awake()
    {
        PhotonPlayer.player = this;

    }

    // Start is called before the first frame update
    void Start()
    {
        if (PhotonNetwork.LocalPlayer.HasRejoined)
        {
            Debug.Log("PhotonPlayer has rejoined");
        }
        else
        {
            PV = GetComponent<PhotonView>();
            //Debug.LogWarning("counter " + playerViewsCounter);
            if (PhotonNetwork.IsMasterClient)
            {
                if (PV.IsMine)
                {
                    //randomize list
                    AllRandomNumbers = randomizeList(randomList);
                    //send gameSetup list of numbers
                    GameSetup.GS.SetActiveList(AllRandomNumbers, 0);
                    GameSetup.GS.SetList(AllRandomNumbers);

                    //Create something to hold the data
                    var o = new MemoryStream();
                    //Save the list
                    bf.Serialize(o, AllRandomNumbers);
                    //Convert the data to a string
                    var data = Convert.ToBase64String(o.GetBuffer());

                    //PV.RPC("RPC_GetList", RpcTarget.OthersBuffered, data);
                }
            }
            //call master client to get team
            if (PV.IsMine)
            {
                PV.RPC("RPC_GetTeam", RpcTarget.MasterClient);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    private List<int> randomizeList(bool random)
    {
        List<int> tempList = new List<int>();
        if (random)
        {
            while (true)
            {
                int num = rng.Next(0, (PhotonNetwork.CurrentRoom.MaxPlayers - 1) * playerViewsCounter);
                if (!tempList.Contains(num))
                {
                    tempList.Add(num);
                }
                if (tempList.Count == ((PhotonNetwork.CurrentRoom.MaxPlayers - 1) * playerViewsCounter))
                {
                    return tempList;
                }
            }
        }
        else
        {
            switch (PhotonNetwork.CurrentRoom.MaxPlayers - 1)
            {
                case 4:
                case 5:
                case 6:
                    tempList = new List<int>(12) { 1, 9, 0, 5, 2, 7, 4, 18, 8, 6, 16, 10 };
                    break;
                case 7:
                    tempList = new List<int>(14) { 1, 9, 0, 19, 2, 6, 4, 18, 8, 17, 16, 7, 5, 10 };
                    break;
                case 8:
                    tempList = new List<int>(16) { 1, 9, 0, 19, 2, 5, 3, 6, 4, 18, 8, 17, 16, 7, 11, 10 };
                    break;
                case 9:
                    tempList = new List<int>(18) { 1, 9, 0, 19, 2, 12, 3, 6, 4, 18, 8, 19, 16, 13, 11, 7, 5, 10 };
                    break;
                case 10:
                    tempList = new List<int>(20) { 1, 19, 0, 12, 2, 6, 3, 18, 4, 17, 8, 13, 16, 7, 11, 10, 5, 14, 9, 15 };
                    break;
                default:
                    Debug.LogWarning("Nije podeseno za 4");
                    break;
            }
            return tempList;
        }
    }

    //call rpc to update view on all clients
    public void sendAll()
    {
        PV.RPC("RPC_GameSetupSetVisible", RpcTarget.Others);
        /*if (PhotonNetwork.IsMasterClient)
        {
            //Debug.LogWarning("Send all recive");
            PV.RPC("RPC_GameSetupSetVisible", RpcTarget.Others);
        }*/
    }

    [PunRPC]
    void RPC_GetTeam()
    {
        if (!PhotonNetwork.LocalPlayer.HasRejoined)
        {
            MyTeam = GameSetup.GS.nextPlayerTeam;
            GameSetup.GS.UpdateTeam();

            //Debug.LogWarning("MASTER GET TEAM" + myTeam.ToString());

            PV.RPC("RPC_SentTeam", RpcTarget.OthersBuffered, MyTeam);
            if (PV.IsMine)
            {
                //Create something to hold the data
                var o = new MemoryStream();
                //Save the list
                bf.Serialize(o, AllRandomNumbers);
                var data = Convert.ToBase64String(o.GetBuffer());
                //send list to other players
                PV.RPC("RPC_GetList", RpcTarget.OthersBuffered, data);
            }
        }
    }

    [PunRPC]
    void RPC_SentTeam(int whichTeam)
    {
        //set Player team
        MyTeam = whichTeam;
    }

    [PunRPC]
    void RPC_GetList(string data)
    {
        if (!PhotonNetwork.LocalPlayer.HasRejoined)
        {
            if (AllRandomNumbers.Count == 0)
            {
                //Create an input stream from the string
                var ins = new MemoryStream(Convert.FromBase64String(data));

                //Read back the data
                List<int> x = (List<int>)bf.Deserialize(ins);

                //set AllRandomNumbers
                AllRandomNumbers = x;

                //set List for Players
                GameSetup.GS.SetActiveList(AllRandomNumbers, PhotonRoomCustomMatch.room.myNumberInRoom - 1);

                //GameSetup.GS.SetActiveList(AllRandomNumbers, myTeam);
                getList = true;
            }
        }
        else
        {
            Debug.Log("Rejoined player try to get list");
        }
    }

    [PunRPC]
    public void RPC_GameSetupSetVisible()
    {
        //Debug.LogWarning("rpc Send all recive");
        GameSetup.GS.setView();
    }
}
