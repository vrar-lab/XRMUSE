using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace XRMUSE.Networking
{
    /// <summary>
    /// An example of the implementation of INetworkingSyncedTransform
    /// </summary>
    public class BehaviourTransformSync : MonoBehaviourPun, IPunObservable, INetworkingSyncedTransform
    {
        Vector3 _networkPosition; Quaternion _networkRotation; public Vector3 networkPosition { get => _networkPosition; set => _networkPosition = value; }
        public Quaternion networkRotation { get => _networkRotation; set => _networkRotation = value; }
        float _networkDistance, _networkAngle; bool _isFirstMovement; 
        public float networkDistance { get => _networkDistance; set => _networkDistance = value; }
        public float networkAngle { get => _networkAngle; set => _networkAngle = value; }
        public bool isFirstMovement { get => _isFirstMovement; set => _isFirstMovement = value; }

        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            (this as INetworkingSyncedTransform).OnPhotonSerializeView_SyncTransform(stream, info);
        }

        public void FixedUpdate()
        {
            (this as INetworkingSyncedTransform).FixedUpdate_SyncTransform();
        }
    }
}