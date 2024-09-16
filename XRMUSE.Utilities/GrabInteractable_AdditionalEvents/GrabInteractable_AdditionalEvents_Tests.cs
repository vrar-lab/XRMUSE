using System;
using XRMUSE.Utilities;
using UnityEngine;
using UnityEngine.InputSystem;

namespace XRMUSE.Utilities
{
    /// <summary>
    /// Example of a simple usage of GrabInteractable_AdditionalEvents, adds simple Debug logging when an object is grabbed or dropped
    /// </summary>
    [RequireComponent(typeof(GrabInteractable_AdditionalEvents))]
    public class GrabInteractable_AdditionalEvents_Tests : MonoBehaviour
    {
        GrabInteractable_AdditionalEvents grabevents;

        void Start()
        {
            grabevents = gameObject.GetComponent<GrabInteractable_AdditionalEvents>();

            grabevents.eventGrabbed.AddListener(() => Debug.Log("TEST: GRABBED"));
            grabevents.eventDropped.AddListener(() => Debug.Log("TEST: DROPPED"));
        }

        void Update()
        {
            try
            {
                if (Keyboard.current.qKey.isPressed)
                {
                    Debug.Log("Test force drop");
                    grabevents.ForceDrop();
                }
            }
            catch (Exception e)
            {
                ;
            }

        }
    }
}