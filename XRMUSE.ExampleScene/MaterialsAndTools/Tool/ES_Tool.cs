using Photon.Pun;
using XRMUSE.Networking;
using UnityEngine;
using UnityEngine.Events;
using System;
using XRMUSE.Utilities;

namespace XRMUSE.ExampleScene
{
    /// <summary>
    /// Behaviour of a "Tool", which is used by one of the player (playerOwner) to fuse materials together. 
    /// </summary>
    [RequireComponent(typeof(TypedCollision))]
    [RequireComponent(typeof(PhotonView))]
    public class ES_Tool : MonoBehaviourPun, IPunObservable, INetworkingSyncedTransform, INetworkingOwnershipBehaviour, INetworking_PoolReset
    {
        public const int TYPED_IS_TOOL = TypedCollision.TYPED_IS_TOOL;
        Vector3 _networkPosition; Quaternion _networkRotation; public Vector3 networkPosition { get => _networkPosition; set => _networkPosition = value; }
        public Quaternion networkRotation { get => _networkRotation; set => _networkRotation = value; }
        float _networkDistance, _networkAngle; bool _isFirstMovement; public float networkDistance { get => _networkDistance; set => _networkDistance = value; }
        public float networkAngle { get => _networkAngle; set => _networkAngle = value; }
        public bool isFirstMovement { get => _isFirstMovement; set => _isFirstMovement = value; }

        public TypedCollision collision;
        public int toolType;
        public int playerOwner = 1;

        //INetworkingOwnershipBehaviour
        bool _was_mine = false; public bool was_mine { get => _was_mine; set => _was_mine = value; }
        UnityEvent _eventOnMine = new UnityEvent(); public UnityEvent eventOnMine { get => _eventOnMine; set => _eventOnMine = value; }
        UnityEvent _eventOnNoLongerMine = new UnityEvent(); public UnityEvent eventOnNoLongerMine { get => _eventOnNoLongerMine; set => _eventOnNoLongerMine = value; }

#if UNITY_EDITOR
        public bool editorAskOwner = false;

        public void Update()
        {
            if (editorAskOwner)
            {
                editorAskOwner = false;
                RequestOwnerShip();
            }
        }
#endif

        private void Start()
        {
            if (collision == null)
                collision = gameObject.GetComponent<TypedCollision>();
            collision.collisionType = TYPED_IS_TOOL;
            collision.eventsEnter.AddListener(OnTypedCollisionEnter);
            collision.eventsExit.AddListener(OnTypedCollisionExit);
        }
        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            (this as INetworkingSyncedTransform).OnPhotonSerializeView_SyncTransform(stream, info);
            (this as INetworkingOwnershipBehaviour).OnPhotonSerializeView_OwnershipBehaviour(photonView);
        }

        public void FixedUpdate()
        {
            (this as INetworkingSyncedTransform).FixedUpdate_SyncTransform();
        }
        
        public void OnTypedCollisionEnter()
        {
            if (photonView.IsMine && collision._lastEnterType.collisionType == TypedCollision.TYPED_IS_MATERIAL)
            {
                try
                {
                    if (collision._lastEnterType == AnimationManager.instance.authorizedISMaterialProximityCollision[AnimationManager.instance.authorizedISMaterialProximityCollision[collision._lastEnterType]])
                    {
                        ES_Material mat1 = collision._lastEnterType.GetComponent<ES_Material>();
                        ES_Material mat2 = AnimationManager.instance.authorizedISMaterialProximityCollision[collision._lastEnterType].GetComponent<ES_Material>();
                        if (mat1.material_lock || mat2.material_lock)
                            return;//material_lock => should not be touched
                        AnimationManager.instance.FuseIndustrialMaterial(mat1, mat2, this);
                    }
                }
                catch (Exception) { }
            }
        }

        public void OnTypedCollisionExit()
        {

        }

        public void RequestOwnerShip()
        {
            photonView.RequestOwnership();
        }

        public void Reset()
        {
            (this as INetworkingSyncedTransform).Reset_SyncTransform();
        }
    }
}