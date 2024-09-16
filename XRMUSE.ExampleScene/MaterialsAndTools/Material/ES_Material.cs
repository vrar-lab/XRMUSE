using Photon.Pun;
using XRMUSE.Networking;
using XRMUSE.Utilities;
using UnityEngine;
using UnityEngine.Events;

namespace XRMUSE.ExampleScene
{
    /// <summary>
    /// Behaviour of the materials which can be fused into others
    /// </summary>
    [RequireComponent(typeof(TypedCollision))]
    [RequireComponent(typeof(PhotonView))]
    public class ES_Material : MonoBehaviourPun, IPunObservable, INetworkingSyncedTransform, INetworkingOwnershipBehaviour, INetworking_PoolReset
    {
        public const int TYPED_IS_MATERIAL = TypedCollision.TYPED_IS_MATERIAL;
        Vector3 _networkPosition; Quaternion _networkRotation; public Vector3 networkPosition { get => _networkPosition; set => _networkPosition = value; }
        float _networkDistance, _networkAngle; bool _isFirstMovement; public float networkDistance { get => _networkDistance; set => _networkDistance = value; }
        public float networkAngle { get => _networkAngle; set => _networkAngle = value; }
        public bool isFirstMovement { get => _isFirstMovement; set => _isFirstMovement = value; }
        public Quaternion networkRotation { get => _networkRotation; set => _networkRotation = value; }

        public TypedCollision collision;
        public int materialType;

        //IPhotonOwnershipBehaviour
        bool _was_mine = true; public bool was_mine { get => _was_mine; set => _was_mine = value; }
        UnityEvent _eventOnMine = new UnityEvent(); public UnityEvent eventOnMine { get => _eventOnMine; set => _eventOnMine = value; }
        UnityEvent _eventOnNoLongerMine = new UnityEvent(); public UnityEvent eventOnNoLongerMine { get => _eventOnNoLongerMine; set => _eventOnNoLongerMine = value; }

        [HideInInspector] 
        public MaterialController materialController;

        private void Start()
        {
            if (collision == null)
                collision = gameObject.GetComponent<TypedCollision>();
            collision.collisionType = TYPED_IS_MATERIAL;
            collision.eventsEnter.AddListener(OnTypedCollisionEnter);
            collision.eventsExit.AddListener(OnTypedCollisionExit);

            if (materialController == null)
                materialController = GetComponentInChildren<MaterialController>();
        }

#if UNITY_EDITOR
        public bool editorAskOwner = false;

        public void Update()
        {
            if (editorAskOwner)
            {
                editorAskOwner = false;
                photonView.RequestOwnership();
            }
        }
#endif

        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            (this as INetworkingSyncedTransform).OnPhotonSerializeView_SyncTransform(stream, info);
            (this as INetworkingOwnershipBehaviour).OnPhotonSerializeView_OwnershipBehaviour(photonView);
            if(stream.IsWriting)
                stream.SendNext(material_lock);
            else
                material_lock = (bool) stream.ReceiveNext();
        }

        public void FixedUpdate()
        {
            if (material_lock)
                return;
            (this as INetworkingSyncedTransform).FixedUpdate_SyncTransform();
        }

        public void OnTypedCollisionEnter()
        {

        }

        public void OnTypedCollisionExit()
        {

        }
        /// <summary>
        /// May be used by other functions/objects to prevent the use of the material
        /// </summary>
        public bool material_lock = false;
        bool checkedDualTransform = false;
        DualTransformDesign dualTransform;
        public void Reset()
        {
            material_lock = false;
            AnimationManager.instance.UnregisterISMaterialProximity(collision);
            if (!checkedDualTransform)
            {
                checkedDualTransform = true;
                dualTransform = GetComponentInChildren<DualTransformDesign>();
            }
            dualTransform.temporaryNoGravity = false;
            (this as INetworkingSyncedTransform).Reset_SyncTransform();
        }
    }
}