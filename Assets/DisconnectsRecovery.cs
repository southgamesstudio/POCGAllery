using ExitGames.Client.Photon;
using Photon.Realtime;
using TMPro;
using UnityEngine;


namespace Photon.Pun.UtilityScripts
{
    /// <summary>
    /// Unexpected disconnects recovery
    /// </summary>
    public class DisconnectsRecovery : MonoBehaviourPunCallbacks
    {
        public static DisconnectsRecovery recovery;

        public bool rejoinCalled;

        public bool reconnectCalled;

        public bool inRoom;

        public TMP_InputField errorMessage;

        private DisconnectCause previousDisconnectCause;



        private void Awake()
        {
            //set up singleton
            if (DisconnectsRecovery.recovery == null)
            {
                DisconnectsRecovery.recovery = this;
            }
            else
            {
                if (DisconnectsRecovery.recovery != this)
                {
                    Object.Destroy(DisconnectsRecovery.recovery);
                    // Destroy(PhotonRoom.room.gameObject);
                    DisconnectsRecovery.recovery = this;
                }
            }
            DontDestroyOnLoad(this.gameObject);
        }

        public override void OnDisconnected(DisconnectCause cause)
        {
            
            if (!PhotonRoomCustomMatch.room.backButtonCreateRoomPressed)
            {
                Debug.LogFormat("OnDisconnected(cause={0}) ClientState={1} PeerState={2}",
                                cause,
                                PhotonNetwork.NetworkingClient.State,
                                PhotonNetwork.NetworkingClient.LoadBalancingPeer.PeerState);
                if (this.rejoinCalled)
                {
                    Debug.LogWarningFormat("Rejoin failed, client disconnected, causes; prev.:{0} current:{1}", this.previousDisconnectCause, cause);
                    this.rejoinCalled = false;
                }
                else if (this.reconnectCalled)
                {
                    Debug.LogWarningFormat("Reconnect failed, client disconnected, causes; prev.:{0} current:{1}", this.previousDisconnectCause, cause);
                    this.reconnectCalled = false;
                }
                if (GameSetup.GS != null)
                {
                    if (!GameSetup.GS.logoutCalled && !GameSetup.GS.disconnectCalled)
                    {
                        GameSetup.GS.setError(true, "Player disconnected. \nTrying to reconnect...");

                        this.HandleDisconnect(cause); // add attempts counter? to avoid infinite retries?
                    }
                    else
                    {
                        GameSetup.GS.setError(true, "Player disconnected. \nClick reconnect button.");
                    }
                }
                this.inRoom = false;
                this.previousDisconnectCause = cause;

            }

        }

        private void HandleDisconnect(DisconnectCause cause)
        {
            if (!this.inRoom)
            {
                Debug.Log("calling PhotonNetwork.ReconnectAndRejoin()");
                this.rejoinCalled = PhotonNetwork.ReconnectAndRejoin();
                if (!this.rejoinCalled)
                {
                    Debug.LogWarning("PhotonNetwork.ReconnectAndRejoin returned false, PhotonNetwork.Reconnect is called instead.");
                    this.reconnectCalled = PhotonNetwork.Reconnect();
                }
            }
            else
            {
                Debug.Log("calling PhotonNetwork.Reconnect()");
                this.reconnectCalled = PhotonNetwork.Reconnect();
            }
            if (!this.rejoinCalled && !this.reconnectCalled)
            {
                Debug.LogWarning("PhotonNetwork.ReconnectAndRejoin() or PhotonNetwork.Reconnect() returned false, client stays disconnected.");
            }
            /* switch (cause)
             {
                 // cases that we can recover from
                 case DisconnectCause.ServerTimeout:
                 case DisconnectCause.Exception:
                 case DisconnectCause.ClientTimeout:
                 case DisconnectCause.DisconnectByServerLogic:
                 case DisconnectCause.AuthenticationTicketExpired:
                 case DisconnectCause.DisconnectByServerReasonUnknown:
                     if (!this.inRoom)
                     {
                         Debug.Log("calling PhotonNetwork.ReconnectAndRejoin()");
                         this.rejoinCalled = PhotonNetwork.ReconnectAndRejoin();
                         if (!this.rejoinCalled)
                         {
                             Debug.LogWarning("PhotonNetwork.ReconnectAndRejoin returned false, PhotonNetwork.Reconnect is called instead.");
                             this.reconnectCalled = PhotonNetwork.Reconnect();
                         }
                     }
                     else
                     {
                         Debug.Log("calling PhotonNetwork.Reconnect()");
                         this.reconnectCalled = PhotonNetwork.Reconnect();
                     }
                     if (!this.rejoinCalled && !this.reconnectCalled)
                     {
                         Debug.LogWarning("PhotonNetwork.ReconnectAndRejoin() or PhotonNetwork.Reconnect() returned false, client stays disconnected.");
                     }

                     break;
                 case DisconnectCause.None:
                 case DisconnectCause.OperationNotAllowedInCurrentState:
                 case DisconnectCause.CustomAuthenticationFailed:
                 case DisconnectCause.DisconnectByClientLogic:
                     if (!this.inRoom)
                     {
                         Debug.Log("calling PhotonNetwork.ReconnectAndRejoin()");
                         this.rejoinCalled = PhotonNetwork.ReconnectAndRejoin();
                         if (!this.rejoinCalled)
                         {
                             Debug.LogWarning("PhotonNetwork.ReconnectAndRejoin returned false, PhotonNetwork.Reconnect is called instead.");
                             this.reconnectCalled = PhotonNetwork.Reconnect();
                         }
                     }
                     else
                     {
                         Debug.Log("calling PhotonNetwork.Reconnect()");
                         this.reconnectCalled = PhotonNetwork.Reconnect();
                     }
                     if (!this.rejoinCalled && !this.reconnectCalled)
                     {
                         Debug.LogWarning("PhotonNetwork.ReconnectAndRejoin() or PhotonNetwork.Reconnect() returned false, client stays disconnected.");
                     }

                     break;
                 case DisconnectCause.InvalidAuthentication:
                 case DisconnectCause.ExceptionOnConnect:
                 case DisconnectCause.MaxCcuReached:
                 case DisconnectCause.InvalidRegion:
                     Debug.LogWarningFormat("Disconnection we cannot automatically recover from, cause: {0}, report it if you think auto recovery is still possible", cause);
                     break;
             }*/
        }

        public override void OnJoinRoomFailed(short returnCode, string message)
        {
            if (this.rejoinCalled)
            {
                Debug.LogWarningFormat("Quick rejoin failed with error code: {0} & error message: {1}", returnCode, message);
                this.rejoinCalled = false;

            }
            if (returnCode == 32758 && GameSetup.GS != null)
            {
                GameSetup.GS.DisconnectPlayer();
                errorMessage.text = "Unable to join the game, " + message;
                errorMessage.gameObject.SetActive(true);
            }
        }

        public override void OnJoinedRoom()
        {
            this.inRoom = true;
            if (PhotonNetwork.LocalPlayer.HasRejoined)
            {
                Debug.Log("Player: " + PhotonNetwork.LocalPlayer.NickName + " rejoined room successful");
                GameSetup.GS.logoutCalled = false;
                GameSetup.GS.disconnectCalled = false;
                GameSetup.GS.setError(false, "Successfully reconnected!");
            }
            if (this.rejoinCalled)
            {
                Debug.Log("Rejoin successful");
                this.rejoinCalled = false;
            }
        }

        public override void OnLeftRoom()
        {
            this.inRoom = false;
        }

        public void recconect()
        {
            if (!PhotonNetwork.IsConnected)
            {
                Debug.LogWarning("Reconnecting");
                this.HandleDisconnect(DisconnectCause.DisconnectByServerReasonUnknown);

            }
            else
            {
                Debug.Log("Player is already connected");
            }
        }

        public override void OnJoinedLobby()
        {
            base.OnJoinedLobby();
            if (this.reconnectCalled)
            {
                Debug.LogWarning("OnLeftRoom inactive:  " + PhotonNetwork.LocalPlayer.IsInactive);
                Debug.LogWarning("TRY TO REJOIN ROOM");
                PhotonNetwork.RejoinRoom("room");
            }
        }

        public override void OnPlayerEnteredRoom(Player newPlayer)
        {
            base.OnPlayerEnteredRoom(newPlayer);
            /*if (PhotonNetwork.LocalPlayer.HasRejoined)
            {
                Debug.LogWarning("TRY TO REJOIN ROOM");
                PhotonNetwork.RejoinRoom("room");
            }*/
        }

        public override void OnConnectedToMaster()
        {
            if (this.reconnectCalled)
            {
                Debug.Log("Reconnect successful");
                this.reconnectCalled = false;
            }
        }
    }
}