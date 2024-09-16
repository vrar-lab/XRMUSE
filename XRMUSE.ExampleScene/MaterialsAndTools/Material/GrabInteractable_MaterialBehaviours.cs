using XRMUSE.Utilities;
using UnityEngine;

namespace XRMUSE.ExampleScene
{
    [RequireComponent(typeof(GrabInteractable_AdditionalEvents))]
    /// <summary>
    /// Additional behaviours of the "materials" in the example scenario when grabbing its XRI_GrabInteractable, adds the behaviours to the attached GrabInteractable_AdditionalEvents at start
    /// </summary>
    public class GrabInteractable_MaterialBehaviours : MonoBehaviour
    {
        [HideInInspector] public GrabInteractable_AdditionalEvents additionalEvents;

        [HideInInspector] public ES_Material esMaterial;

        void Start()
        {
            if (additionalEvents == null)
                additionalEvents = gameObject.GetComponent<GrabInteractable_AdditionalEvents>();
            if (esMaterial == null)
                esMaterial = gameObject.GetComponentInParent<ES_Material>();

            additionalEvents.eventGrabbed.AddListener(eventGrabbed);
            esMaterial.eventOnNoLongerMine.AddListener(eventNotMine);
        }

        void eventGrabbed()
        {
            if (esMaterial.material_lock)
            {
                additionalEvents.ForceDrop();
                return;
            }

            if (!esMaterial.photonView.IsMine)
                esMaterial.photonView.RequestOwnership();
        }

        void eventNotMine()
        {
            additionalEvents.ForceDrop();
        }
    }
}