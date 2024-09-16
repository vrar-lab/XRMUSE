using UnityEngine;
#if XRMUSE_XRE
using Wave.Native;
#endif

namespace XRMUSE.Utilities
{
    /// <summary>
    /// Activate the passthrough mode on the XR Elite headset.
    /// </summary>
    public class PassthroughSetup : MonoBehaviour
    {
#if XRMUSE_XRE
        [SerializeField] private Camera hmd;
        private void Start()
        {
            hmd.clearFlags = CameraClearFlags.SolidColor;
            hmd.backgroundColor = new Color(0, 0, 0, 0);
            Interop.WVR_ShowPassthroughUnderlay(true);
            Interop.WVR_SetPassthroughImageFocus(WVR_PassthroughImageFocus.Scale);
        }
#endif
    }
}
