using System.Collections;
using System.Collections.Generic;
using XRMUSE.ExampleScene;
using XRMUSE.SceneDescription;
using XRMUSE.Utilities;
using UnityEngine;

namespace XRMUSE.ExampleScene
{
    [RequireComponent(typeof(GrabInteractable_AdditionalEvents))]
    /// <summary>
    /// Additional behaviours of the "tools" in the example scenario when grabbing its XRI_GrabInteractable, adds the behaviours to the attached GrabInteractable_AdditionalEvents at start
    /// </summary>
    public class GrabInteractable_ToolBehaviour : MonoBehaviour
    {

        [HideInInspector] public GrabInteractable_AdditionalEvents additionalEvents;

        [HideInInspector] public ES_Tool esTool;

        void Start()
        {
            if (additionalEvents == null)
                additionalEvents = gameObject.GetComponent<GrabInteractable_AdditionalEvents>();
            if (esTool == null)
                esTool = gameObject.GetComponentInParent<ES_Tool>();

            additionalEvents.eventGrabbed.AddListener(eventGrabbed);
            esTool.eventOnNoLongerMine.AddListener(eventNotMine);
            additionalEvents.eventDropped.AddListener(() => enabled = false);
        }

        void eventGrabbed()
        {
            if (SD_Settings.playerNum != esTool.playerOwner)
            {
                additionalEvents.ForceDrop();
                return;
            }

            if (!esTool.photonView.IsMine)
                esTool.photonView.RequestOwnership();
        }

        void eventNotMine()
        {
            additionalEvents.ForceDrop();
        }
    }
}