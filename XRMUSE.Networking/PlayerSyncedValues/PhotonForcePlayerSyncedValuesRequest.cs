using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using XRMUSE.SceneDescription;
using UnityEngine;

namespace XRMUSE.Networking
{
    /// <summary>
    /// A simple class to force a call on the local PSV when joining the room. Which calls the pool system to generate it if its null
    /// </summary>
    public class PhotonForcePlayerSyncedValuesRequest : MonoBehaviourPunCallbacks
    {
        public override void OnJoinedRoom()
        {
            PlayerSyncedValues tmp = PlayerSyncedValues.localInstance;
            GameObject.Destroy(gameObject);
        }
    }
}
