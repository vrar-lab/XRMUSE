using XRMUSE.SceneDescription;
using UnityEngine;

namespace XRMUSE.ExampleScene
{
    /// <summary>
    /// A utility component that relocates a tool to its spawn position when users release the tool.
    /// </summary>
    public class ToolPosReset : MonoBehaviour
    {
        public Vector3 _originalPos;
        public Quaternion _originalRot;

        /// <summary>
        /// Unity lifecycle function. Looks for the spawn position in the scene.
        /// </summary>
        private void Start()
        {
            if (SD_Settings.playerNum == 1)
            {
                _originalPos = GameObject.FindWithTag("tool-zone-p1").transform.localPosition;
                _originalRot = GameObject.FindWithTag("tool-zone-p1").transform.localRotation;
            }
            else if (SD_Settings.playerNum == 2)
            {
                _originalPos = GameObject.FindWithTag("tool-zone-p2").transform.localPosition;
                _originalRot = GameObject.FindWithTag("tool-zone-p2").transform.localRotation * Quaternion.Euler(0, 180, 0);
            }
            SnapBacktoOriginalPose();
        }

        /// <summary>
        /// Reset the tool's location
        /// </summary>
        public void SnapBacktoOriginalPose()
        {
            transform.position = _originalPos;
            transform.rotation = _originalRot;
            transform.Rotate(0f, 90f, 0f);
        }
    }
}
