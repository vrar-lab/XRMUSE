using Photon.Pun;
using XRMUSE.SceneDescription;
using Unity.Mathematics;
using UnityEngine;
using XRMUSE.Networking;
using XRMUSE.Utilities;

namespace XRMUSE.ExampleScene
{
    /// <summary>
    /// Behaviours of the "material box" in the example scenario. Which generates networked interactable materials to fuse.
    /// Produces objects in respect to network on one of the players' machine if the latest object is far enough from the spawning position.
    /// </summary>
    [RequireComponent(typeof(ES_MaterialBox))]
    public class MaterialBoxInteractions : MonoBehaviourPunCallbacks
    {
        ES_MaterialBox materialBox;
        bool room_joined = false;
        GrabInteractable_AdditionalEvents grabInteractableCurrentProduced;
        DualTransformDesign dualTransform;
        GameObject lastProduced = null;
        bool is_produced = false;

        /// <summary>
        /// Distance with the previous object necassary before producing a new one
        /// </summary>
        public float min_dist = 0.5f;



        public Vector3 spawnPosition => materialBox.transform.position + materialBox.spawnPoint;


        public void Start()
        {
            materialBox = GetComponent<ES_MaterialBox>();
        }

        private void Update()
        {
            if (!is_produced)
                DoProduce();
            else
            {
                if (grabInteractableCurrentProduced.isGrabbed ||
                    (lastProduced.transform.position - spawnPosition).sqrMagnitude > min_dist * min_dist ||
                    lastProduced.activeSelf == false)
                {
                    is_produced = false;
                    dualTransform.temporaryNoGravity = false;
                }
                else
                {
                    SpinningCore(lastProduced.transform);
                    lastProduced.transform.position = spawnPosition;
                }
            }
        }

        void DoProduce()
        {
            if (!(lastProduced == null ||
                  (lastProduced.transform.position - spawnPosition).sqrMagnitude > min_dist * min_dist ||
                  lastProduced.activeSelf == false))
                return;
            if (materialBox.playerOwner != SD_Settings.playerNum)
                return;

            lastProduced = materialBox.Produce();
            grabInteractableCurrentProduced = lastProduced.GetComponentInChildren<GrabInteractable_AdditionalEvents>();
            dualTransform = lastProduced.GetComponentInChildren<DualTransformDesign>();
            dualTransform.temporaryNoGravity = true;
            is_produced = true;
        }

        private const float SPINNING_RATE = 20f;

        private static void SpinningCore(Transform transform)
        {
            transform.rotation = Quaternion.Euler(0f,
                math.lerp(0f, 359f,
                    (UnityEngine.Time.time / SPINNING_RATE) - (int)(UnityEngine.Time.time / SPINNING_RATE)), 0F);
        }

        public override void OnJoinedRoom()
        {
            if (materialBox.playerOwner != SD_Settings.playerNum)
                enabled = false;

            room_joined = true;
        }
    }
}