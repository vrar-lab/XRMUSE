using UnityEngine;

namespace XRMUSE.Utilities
{
    /// <summary>
    /// Copies the boxcollider values on the child object to the grandparent while taking into account the scale
    /// Used mostly for SceneDescription objects tied to DualTransformDesign
    /// </summary>
    [RequireComponent(typeof(BoxCollider))]
    public class CloneBoxColliderTool : MonoBehaviour
    {
        void Start()
        {
            BoxCollider myBox = GetComponent<BoxCollider>();
            BoxCollider tranferTo = transform.parent.parent.gameObject.GetComponent<BoxCollider>();
            tranferTo.center = new Vector3(
                myBox.center.x * transform.parent.localScale.x * transform.parent.parent.localScale.x,
                myBox.center.y * transform.parent.localScale.y * transform.parent.parent.localScale.y,
                myBox.center.z * transform.parent.localScale.z * transform.parent.parent.localScale.z);
            tranferTo.size = new Vector3(
                tranferTo.center.x * transform.parent.localScale.x * transform.parent.parent.localScale.x,
                tranferTo.center.y * transform.parent.localScale.y * transform.parent.parent.localScale.y,
                tranferTo.center.z * transform.parent.localScale.z * transform.parent.parent.localScale.z);
            GameObject.Destroy(this);
        }
    }
}