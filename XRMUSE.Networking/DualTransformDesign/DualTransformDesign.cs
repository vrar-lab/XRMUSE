using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using XRMUSE.Utilities;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace XRMUSE.Networking
{
    /// <summary>
    /// Considering a parent-child pair of transforms, traditionally in Unity a child transform will follow the parent's transform
    /// The dual transform design allows to have the inverse relationship temporarily as part of Networking behaviours
    /// Should be placed on the "controlling child" transform
    /// </summary>
    public class DualTransformDesign : MonoBehaviour
    {
        /// <summary>
        /// Is the childTransform controlling its parent's transform?
        /// </summary>
        public bool childAuthoring = true;
        Rigidbody body;
        /// <summary>
        /// Do the childTransform uses gravity when authoring?
        /// </summary>
        public bool controlGravity = true;
        bool isGravity;
        /// <summary>
        /// Is the childTransform kinematic when authoring?
        /// </summary>
        public bool controlKinematic = true;
        [HideInInspector]
        public bool isKinematic;

        /// <summary>
        /// Toggle to force no gravity on the child transform when authoring, can be useful when triggering animations
        /// </summary>
        public bool temporaryNoGravity = false;
        bool checkNG => !temporaryNoGravity;

        /// <summary>
        /// Usually grabbed on the parent object, the custom networked object that will trigger the authoring depending on network ownership
        /// </summary>
        public INetworkingOwnershipBehaviour photonObject;

        Transform parentTransform;

        private GrabInteractable_AdditionalEvents GI_AE = null;

        private XRGrabInteractable _grabInteractable;
        
        void Start()
        {
            body = GetComponent<Rigidbody>();
            isGravity = body.useGravity;
            isKinematic = body.isKinematic;
            photonObject = transform.parent.GetComponent<INetworkingOwnershipBehaviour>();

            GI_AE = GetComponent<GrabInteractable_AdditionalEvents>();
            GI_AE.eventGrabbed.AddListener(() =>
            {
                if (controlKinematic)
                {
                    body.isKinematic = false;
                }
            });

            if (photonObject != null)
            {
                photonObject.eventOnMine.AddListener(() => childAuthoring = true);
                photonObject.eventOnNoLongerMine.AddListener(() => childAuthoring = false);
                childAuthoring = (photonObject as MonoBehaviourPun).photonView.IsMine;
            }

            _grabInteractable = GetComponent<XRGrabInteractable>();
            getPrivateXRGrabAttributes();

            parentTransform = transform.parent;
        }

        //Checks should be made here to cancel or apply eventual position changes due to FixedUpdates being called before physics
        private void FixedUpdate()
        {
            try
            {
                if (GI_AE.isGrabbed)
                {
                    if (childAuthoring)
                    {
                        parentTransform.position = transform.position;
                        parentTransform.rotation = transform.rotation;

                        _forceGravityKinematicXRGrab();
                    }
                    return;
                }
            }catch(Exception){}

            if (!childAuthoring) {
                transform.localPosition = Vector3.zero;
                transform.localRotation = Quaternion.identity;
                if (!body.isKinematic)
                {
                    body.velocity = Vector3.zero;
                    body.angularVelocity = Vector3.zero;
                }
                if(controlGravity)
                    body.useGravity = false;
                if (controlKinematic)
                    body.isKinematic = true;
                return;
            }
            if (controlGravity)
            {
                if((!(body.useGravity = isGravity && checkNG)) && (!body.isKinematic))
                {
                    body.velocity = Vector3.zero;
                    body.angularVelocity = Vector3.zero;
                }
            }

            if (controlKinematic)
            {
                body.isKinematic = isKinematic;
            }

            Vector3 pos = transform.position;
            Quaternion rot = transform.rotation;
            parentTransform.position = pos;
            parentTransform.rotation = rot;
            transform.position = pos;
            transform.rotation = rot;
        }

        private FieldInfo _wasKinematicXRGrab;
        private FieldInfo _wasGravityXRGrab;
        void getPrivateXRGrabAttributes()
        {
            _wasKinematicXRGrab = typeof(XRGrabInteractable).GetField("m_WasKinematic", BindingFlags.NonPublic | BindingFlags.Instance);
            _wasGravityXRGrab = typeof(XRGrabInteractable).GetField("m_UsedGravity", BindingFlags.NonPublic | BindingFlags.Instance);
        }

        void _forceGravityKinematicXRGrab()
        {
            if(controlGravity)
                _wasGravityXRGrab.SetValue(_grabInteractable, isGravity);
            if(controlKinematic)
                _wasKinematicXRGrab.SetValue(_grabInteractable, isKinematic);
        }
    }
}