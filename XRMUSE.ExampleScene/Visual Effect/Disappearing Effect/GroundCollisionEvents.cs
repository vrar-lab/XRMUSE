using Photon.Pun;
using UnityEngine;
using XRMUSE.Networking;

namespace XRMUSE.ExampleScene
{
    /// <summary>
    /// Handles the collision when an object falls on the ground.
    /// </summary>
    public class GroundCollisionEvents : MonoBehaviour
    {
        private void OnCollisionEnter(Collision collision)
        {
            if (collision.transform.CompareTag("Material"))
            {
                if (!collision.gameObject.GetComponentInParent<PhotonView>().IsMine)
                    return;
                PhotonNetwork.Destroy(collision.transform.parent.gameObject);
                DataRegister_AnimationAtPosition.AddAnimation(collision.contacts[0].point, "DisappearAnimation");
            }
        }
    }
}