using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace XRMUSE.Utilities
{
    /// <summary>
    /// Defining the behaviors of the UI prompt during ArUco code anchoring.
    /// </summary>
    public class PromptController : MonoBehaviour
    {
        public GameObject canvas;
        public TMP_Text text;
        public static readonly UnityEvent Activate = new UnityEvent();
        public static readonly UnityEvent Deactivate = new UnityEvent();
        public static readonly UnityEvent<string> SetText = new UnityEvent<string>();
    
        /// <summary>
        /// Move the UI 2 meters in front of the user.
        /// </summary>
        private void Start()
        {
            if (Camera.main != null)
            {
                var cameraTransform = Camera.main.transform;
                transform.position = cameraTransform.position + cameraTransform.forward * 2.0f;
            }
        }

        /// <summary>
        /// A bunch of UnityEvents for controlling the UI prompt. These events are called by the session manager.
        /// </summary>
        private void OnEnable()
        {
            Activate.AddListener(() => {canvas.SetActive(true);});
            Deactivate.AddListener(() => {canvas.SetActive(false);});
            SetText.AddListener((newText) => { text.text = newText; });
        }

        /// <summary>
        /// Event unsubscription.
        /// </summary>
        private void OnDisable()
        {
            Activate.RemoveAllListeners();
            Deactivate.RemoveAllListeners();
        }
    }
}
