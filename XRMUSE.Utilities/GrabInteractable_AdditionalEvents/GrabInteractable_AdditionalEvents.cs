using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.Interaction.Toolkit;

namespace XRMUSE.Utilities
{
    /// <summary>
    /// An Helper class to use in pair with XRGrabInteractable.
    /// Simply replaces the use of the specific XRI_Events types with normal Unity Events and add some functions (i.e. force drop)
    /// 2 effects from it: can easily clear and modify at runtime the events without risking to touch other events, simplifies the functions arguments
    /// </summary>
    [RequireComponent(typeof(XRGrabInteractable))]
    public class GrabInteractable_AdditionalEvents : MonoBehaviour
    {
        private XRGrabInteractable _xrGrabInteractable = null;

        /// <summary>
        /// Add listener to eventGrabbedActivate to change behaviour when object is grabbed and pressed
        /// </summary>
        public UnityEvent eventGrabbedActivate = new UnityEvent();

        /// <summary>
        /// Add listener to eventGrabbed to change behaviour when object is grabbed
        /// </summary>
        public UnityEvent eventGrabbed = new UnityEvent();

        /// <summary>
        /// Add listener to eventDropped to change behaviour when object is dropped
        /// </summary>
        public UnityEvent eventDropped = new UnityEvent();

        public UnityEvent eventHoverEnter = new UnityEvent();
        public UnityEvent eventHoverExit = new UnityEvent();

        /// <summary>
        /// Simple bool to check if the object is currently grabbed
        /// </summary>
        public bool isGrabbed = false;
        void Start()
        {
            if (_xrGrabInteractable == null)
                _xrGrabInteractable = gameObject.GetComponent<XRGrabInteractable>();

            _xrGrabInteractable.firstHoverEntered.AddListener(OnHoverEnter);
            _xrGrabInteractable.lastHoverExited.AddListener(OnHoverExit);
            _xrGrabInteractable.activated.AddListener(OnActivatedEntered);
            _xrGrabInteractable.selectEntered.AddListener(OnSelectEntered);
            _xrGrabInteractable.selectExited.AddListener(OnSelectExited);
        }

        void OnHoverEnter(HoverEnterEventArgs args)
        {
            eventHoverEnter.Invoke();
        }
        void OnHoverExit(HoverExitEventArgs args)
        {
            eventHoverExit.Invoke();
        }

        void OnActivatedEntered(ActivateEventArgs args)
        {
            if (_xrGrabInteractable.interactorsSelecting.Contains((IXRSelectInteractor)args.interactorObject))
                eventGrabbedActivate.Invoke();
        }

        void OnSelectEntered(SelectEnterEventArgs args)
        {
            isGrabbed = true;
            if (_xrGrabInteractable.interactorsSelecting.Contains((IXRSelectInteractor)args.interactorObject))
                eventGrabbed.Invoke();
        }

        void OnSelectExited(SelectExitEventArgs args)
        {
            isGrabbed = false;
            eventDropped.Invoke();
        }

        List<IXRSelectInteractor> tmp_interactors = new List<IXRSelectInteractor>();
        public void ForceDrop()
        {
            isGrabbed = false;
            tmp_interactors.Clear();//Doing that to optimize by avoiding memory allocation
            foreach (var interactor in _xrGrabInteractable.interactorsSelecting)
                tmp_interactors.Add(interactor);

            foreach (var interactor in tmp_interactors)
                _xrGrabInteractable.interactionManager.SelectExit(interactor, _xrGrabInteractable);
        }
    }
}
