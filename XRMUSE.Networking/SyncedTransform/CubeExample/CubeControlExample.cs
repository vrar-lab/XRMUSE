using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace XRMUSE.Networking
{
    /// <summary>
    /// Example of a networkedObject which can be rotated and spawn objects
    /// </summary>
    public class CubeControlExample : MonoBehaviourPun, IPunObservable, INetworkingSyncedTransform
    {
        public string SpawnFromResources = "PrefabTest";
        public GameObject SpawnFromField;
        float _networkDistance, _networkAngle; bool _isFirstMovement; public float networkDistance { get => _networkDistance; set => _networkDistance = value; }
        public float networkAngle { get => _networkAngle; set => _networkAngle = value; }
        public bool isFirstMovement { get => _isFirstMovement; set => _isFirstMovement = value; }
        void Start()
        {
            (this as INetworkingSyncedTransform).TestDefault();
        }

        List<GameObject> spawned = new List<GameObject>();

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                if (!photonView.IsMine)
                    photonView.RequestOwnership();
                transform.Rotate(new Vector3(10, 0, 0));
            }
            if (Input.GetKeyDown(KeyCode.A))
            {
                Debug.Log("Spawn Res");
                if (!photonView.IsMine)
                    photonView.RequestOwnership();
                spawned.Add(
                    PhotonNetwork.Instantiate(SpawnFromResources, transform.position, transform.rotation));
            }
            if (Input.GetKeyDown(KeyCode.Z))
            {
                Debug.Log("Spawn Field");
                if (!photonView.IsMine)
                    photonView.RequestOwnership();
                spawned.Add(
                   PoolManager.SpawnPrefab(SpawnFromField, transform.position, transform.rotation));
            }
            if (Input.GetKeyDown(KeyCode.E))
            {
                Debug.Log("Spawns Clear");
                if (!photonView.IsMine)
                    photonView.RequestOwnership();
                foreach (GameObject go in spawned)
                {
                    if (go != null)
                        PhotonNetwork.Destroy(go);
                }
                spawned.Clear();
            }
        }

        /// INetworkingSyncedTransform required line
        Vector3 _networkPosition; Quaternion _networkRotation; public Vector3 networkPosition { get => _networkPosition; set => _networkPosition = value; }
        public Quaternion networkRotation { get => _networkRotation; set => _networkRotation = value; }

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