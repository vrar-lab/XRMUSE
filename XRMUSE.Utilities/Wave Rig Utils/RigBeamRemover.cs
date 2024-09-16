using UnityEngine;

namespace XRMUSE.Utilities
{
    /// <summary>
    /// A utility class for removing the default beamers on Wave XR Rig.
    /// </summary>
    public class RigBeamRemover : MonoBehaviour
    {
        private bool _isLeftBeamRemoved;
        private bool _isRightBeamRemoved;
        private bool _isLeftPointerRemoved;
        private bool _isRightPointerRemoved;
    
        void Update()
        {
            if (!_isRightBeamRemoved)
            {
                if (GameObject.Find("DominantBeam") != null)
                {
                    GameObject.Find("DominantBeam").SetActive(false);
                    _isRightBeamRemoved = true;
                }
            }
        
            if (!_isLeftBeamRemoved)
            {
                if (GameObject.Find("LeftBeam") != null)
                {
                    GameObject.Find("LeftBeam").SetActive(false);
                    _isLeftBeamRemoved = true;
                }
            }
        
            if (!_isRightPointerRemoved)
            {
                if (GameObject.Find("DominantPointer") != null)
                {
                    GameObject.Find("DominantPointer").SetActive(false);
                    _isRightPointerRemoved = true;
                }
            }
        
            if (!_isLeftPointerRemoved)
            {
                if (GameObject.Find("LeftPointer") != null)
                {
                    GameObject.Find("LeftPointer").SetActive(false);
                    _isLeftPointerRemoved = true;
                }
            }
        }
    }
}
