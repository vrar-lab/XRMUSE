using UnityEngine;

namespace XRMUSE.ExampleScene
{
    /// <summary>
    /// Controlling the behavior of the valve in the virtual cavity.
    /// </summary>
    public class CavityAnimationTrigger : MonoBehaviour
    {
        public Animator _animator;
        private bool isObjectInside = false;

        /// <summary>
        /// Open the valve when there is a material object in the upper part of the cavity.
        /// </summary>
        private void Update()
        {
            _animator.SetBool("isValveOpen", isObjectInside);
        }
        /// <summary>
        /// Close the valve if there is no object in the upper part of the cavity.
        /// </summary>
        private void FixedUpdate()
        {
            isObjectInside = false;
        }
        /// <summary>
        /// Set the flag to true when there is a material object in the upper part of the cavity.
        /// </summary>
        /// <param name="other"></param>
        private void OnTriggerStay(Collider other)
        {
            isObjectInside = true;
        }
    }
}
