using Photon.Pun;
using UnityEngine;
using XRMUSE.Utilities;

namespace XRMUSE.ExampleScene
{
    /// <summary>
    /// A behaviour which, when colliding with an IS_Material, destroys said material and log it to the PlayerSyncedValues
    /// </summary>
    [RequireComponent(typeof(TypedCollision))]
    public class ES_MaterialConsume : MonoBehaviour
    {
        public const int TYPED_IS_MATERIALCONSUME = TypedCollision.TYPED_IS_MATCONSUME;
        public TypedCollision collision;
        // Start is called before the first frame update
        void Start()
        {
            if (collision == null)
                collision = gameObject.GetComponent<TypedCollision>();
            collision.collisionType = TYPED_IS_MATERIALCONSUME;
            collision.eventsEnter.AddListener(OnTypedCollisionEnter);
            collision.eventsExit.AddListener(OnTypedCollisionExit);
        }

        public void OnTypedCollisionEnter()
        {
            if (collision._lastEnterType.collisionType == TypedCollision.TYPED_IS_MATERIAL)
            {
                var mat = collision._lastEnterType.GetComponent<ES_Material>();
                if (mat.photonView.IsMine)
                {
                    DataRegister_MaterialConsumption.AddCount(mat.materialType);
                    PhotonNetwork.Destroy(mat.gameObject);
                    collision.currentlyColliding.Remove(collision._lastEnterType);
                    collision._lastEnterType = null;
                }
            }
        }

        public void OnTypedCollisionExit()
        {

        }
    }
}