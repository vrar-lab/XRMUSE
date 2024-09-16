using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

namespace XRMUSE.Networking
{
    /// <summary>
    /// Simple MonoBehaviour which can be put in a scene to connect to a PhotonRoom.
    /// </summary>
    public class ConnectPhoton : MonoBehaviourPunCallbacks
    {
        public static ConnectPhoton instance = null;
        string roomName = "DEFAULTROOM";
        public bool roomJoined = false;

        public int sendRate = 30;
        public int serializationRate = 10;

        public static UnityEvent ConnectServer = new UnityEvent();
        // Start is called before the first frame update
        void Start()
        {
            instance = this;
            PhotonNetwork.SendRate = sendRate;
            PhotonNetwork.SerializationRate = serializationRate;
            PhotonNetwork.ConnectUsingSettings();
        }

        public override void OnConnectedToMaster()
        {
            Debug.Log("OnConnectedToMaster() was called by PUN.");
            PhotonNetwork.CreateRoom(roomName);
        }

        public override void OnCreateRoomFailed(short returnCode, string message)
        {
            Debug.Log("RoomCreate fail => let's try to join it");
            PhotonNetwork.JoinRoom(roomName);
        }

        public override void OnJoinedRoom()
        {
            OnCreatedRoom();
        }

        public override void OnCreatedRoom()
        {
            Debug.Log("Room joined!");
            roomJoined = true;
        }

        public override void OnLeftRoom()
        {
            Debug.Log("Room left!");
            roomJoined = false;
        }
    }
}