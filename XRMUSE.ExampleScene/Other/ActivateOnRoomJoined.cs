using UnityEngine;
using Photon.Pun;

namespace XRMUSE.ExampleScene
{
    public class ActivateOnRoomJoined : MonoBehaviourPunCallbacks
    {
        public GameObject[] toActivate;
        // Start is called before the first frame update
        public override void OnJoinedRoom()
        {
            foreach (var item in toActivate)
                item.SetActive(true);
            SessionManager.IsPlayerConnected = true;
        }
    }
}