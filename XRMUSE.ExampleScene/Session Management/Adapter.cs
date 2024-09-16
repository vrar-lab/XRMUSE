using UnityEngine;
using UnityEngine.Events;

namespace XRMUSE.ExampleScene
{
    /// <summary>
    /// Arrange the positions of digital elements based on the dimension of the real table.
    /// </summary>
    public class Adapter : MonoBehaviour
    {
        public Transform center, front, back;
        public Transform leftCavity, rightCavity;
        public Transform UiPanelP1, UiPanelP2;
        public Transform[] materialBoxesP1, materialBoxesP2;
        public float matboxHeight = 0.2f;
        public static UnityEvent ActivateSelf = new UnityEvent();
        public static UnityEvent<float, float> AdaptDimension = new UnityEvent<float, float>();

        private void OnEnable()
        {
            ActivateSelf.AddListener(ActiavteSelfHandler);
            AdaptDimension.AddListener(Adapt);
        }
        private void OnDisable()
        {
            ActivateSelf.RemoveAllListeners();
            AdaptDimension.RemoveAllListeners();
        }
    
        /// <summary>
        /// Activate the invisible virtual table (the invisible virtual table is inactive by default). 
        /// </summary>
        public void ActiavteSelfHandler()
        {
            center.gameObject.SetActive(true);
            front.gameObject.SetActive(true);
            back.gameObject.SetActive(true);
            leftCavity.gameObject.SetActive(true);
            rightCavity.gameObject.SetActive(true);
        }

        /// <summary>
        /// Adapts the invisible virtual table to the dimension of the real table.
        /// </summary>
        /// <param name="width">The width of the real table.</param>
        /// <param name="length">The length of the real table.</param>
        public void Adapt(float width, float length)
        {

            // Set the virtual table's width
            #region table-width

            var tmp = center.localScale;
            center.localScale = new Vector3(width - 1f, tmp.y, tmp.z);

            tmp = rightCavity.localPosition;
            rightCavity.localPosition = new Vector3((width - 1f) / 2f + 0.2f, tmp.y, tmp.z);
            leftCavity.localPosition = new Vector3(-(width - 1f) / 2f - 0.2f, tmp.y, tmp.z);

            tmp = front.localScale;
            front.localScale = new Vector3(width, tmp.y, tmp.z);
            back.localScale = new Vector3(width, tmp.y, tmp.z);

            #endregion

            // Set the virtual table's height
            #region table-height

            tmp = front.localScale;
            front.localScale = new Vector3(tmp.x, tmp.y, (length - 0.4f) / 2f);
            back.localScale = new Vector3(tmp.x, tmp.y, (length - 0.4f) / 2f);

            tmp = front.localPosition;
            front.localPosition = new Vector3(tmp.x, tmp.y, 0.2f + (length - 0.4f) / 4f);
            back.localPosition = new Vector3(tmp.x, tmp.y, -0.2f - (length - 0.4f) / 4f);

            #endregion

            // Set the locations of all material boxes
            #region material-box

            for (int i = 0; i < materialBoxesP1.Length; i++)
            {
                materialBoxesP1[i].localPosition = new Vector3(width / 2f, matboxHeight, -length / 2f - 0.3f - i * 0.3f);
            }

            for (int i = 0; i < materialBoxesP2.Length; i++)
            {
                materialBoxesP2[i].localPosition = new Vector3(-width / 2f, matboxHeight, length / 2f + 0.3f + i * 0.3f);
            }

            #endregion

            // Set the locations of all UI panels.
            #region ui-panel

            UiPanelP1.localPosition = new Vector3(-width / 2f - 0.3f, 0.3f, 0f);
            UiPanelP2.localPosition = new Vector3(width / 2f + 0.3f, 0.3f, 0f);

            #endregion
        
            SessionManager.IsAdaptationFinished = true;
        }
    }
}
