
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;
using Photon.Pun;

namespace XRMUSE.Networking
{
    /// <summary>
    /// An interface to manage syncing transforms between synced object. Assumes MonoBehaviourPun, IPunObservable on the object.
    /// TO USE DO THE FOLLOWING:
    /// Add to the class: Vector3 _networkPosition; Quaternion _networkRotation; public Vector3 networkPosition { get => _networkPosition; set => _networkPosition = value; } public Quaternion networkRotation { get => _networkRotation; set => _networkRotation = value; }
    /// Add to the class: float _networkDistance, _networkAngle; bool _isFirstMovement; public float networkDistance { get => _networkDistance; set => _networkDistance = value; }; public float networkAngle { get => _networkAngle; set => _networkAngle = value; }; public bool isFirstMovement { get => _isFirstMovement; set => _isFirstMovement = value; }
    /// Add to OnPhotonSerializeView(): (this as INetworkingSyncedTransform).OnPhotonSerializeView_SyncTransform(stream, info);
    /// Add to FixedUpdate(): (this as INetworkingSyncedTransform).FixedUpdate_SyncTransform(); XOR to Update(): (this as INetworkingSyncedTransform).Update_SyncTransform();
    /// Add to Reset() (INetworking_PoolReset): (this as INetworkingSyncedTransform).Reset_SyncTransform();
    /// </summary>
    public interface INetworkingSyncedTransform
    {
        public Vector3 networkPosition { get; set; }
        public Quaternion networkRotation { get; set; }
        public float networkDistance { get; set; }
        public float networkAngle { get; set; }
        public bool isFirstMovement { get; set; }
        void TestDefault()
        {
            Debug.Log("Is it working?");
        }

        void OnPhotonSerializeView_SyncTransform(PhotonStream stream, PhotonMessageInfo info)
        {
            if (stream.IsWriting)
            {
                stream.SendNext((this as MonoBehaviourPun).transform.position);
                stream.SendNext((this as MonoBehaviourPun).transform.rotation);
                networkPosition = (this as MonoBehaviourPun).transform.position;
                networkRotation = (this as MonoBehaviourPun).transform.rotation;
            }
            else
            {
                Vector3 previousPos = networkPosition;

                networkPosition = (Vector3)stream.ReceiveNext();
                networkRotation = (Quaternion)stream.ReceiveNext();
                if (isFirstMovement)
                {
                    (this as MonoBehaviourPun).transform.position = networkPosition;
                    (this as MonoBehaviourPun).transform.rotation = networkRotation;
                }
                else
                {
                    float lag = Mathf.Abs((float)(PhotonNetwork.Time - info.SentServerTime));//lag compensation
                    networkPosition += (networkPosition - previousPos) * lag;
                    networkDistance = Vector3.Distance((this as MonoBehaviourPun).transform.position, networkPosition);
                    networkAngle = Quaternion.Angle((this as MonoBehaviourPun).transform.rotation, networkRotation);
                }
            }

            isFirstMovement = false;
        }

        void FixedUpdate_SyncTransform()
        {
            if (!(this as MonoBehaviourPun).photonView.IsMine)
            {
                /*(this as MonoBehaviourPun).transform.position = networkPosition;
                (this as MonoBehaviourPun).transform.rotation = networkRotation;*/

                (this as MonoBehaviourPun).transform.position = Vector3.MoveTowards((this as MonoBehaviourPun).transform.position, networkPosition, networkDistance * Time.fixedDeltaTime * PhotonNetwork.SerializationRate);
                (this as MonoBehaviourPun).transform.rotation = Quaternion.RotateTowards((this as MonoBehaviourPun).transform.rotation, networkRotation, networkAngle * Time.fixedDeltaTime * PhotonNetwork.SerializationRate);
            }
        }

        void Update_SyncTransform()
        {
            if (!(this as MonoBehaviourPun).photonView.IsMine)
            {
                (this as MonoBehaviourPun).transform.position = Vector3.MoveTowards((this as MonoBehaviourPun).transform.position, networkPosition, networkDistance * Time.deltaTime * PhotonNetwork.SerializationRate);
                (this as MonoBehaviourPun).transform.rotation = Quaternion.RotateTowards((this as MonoBehaviourPun).transform.rotation, networkRotation, networkAngle * Time.deltaTime * PhotonNetwork.SerializationRate);
            }
        }

        void Reset_SyncTransform()
        {
            networkDistance = float.MaxValue;
            networkAngle = float.MaxValue;
            isFirstMovement = true;
        }
    }

}
