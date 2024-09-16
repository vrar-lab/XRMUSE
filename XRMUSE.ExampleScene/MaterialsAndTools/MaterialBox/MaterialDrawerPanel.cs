using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace XRMUSE.ExampleScene
{
    /// <summary>
    /// This component defines the behaviors of a material box.
    /// </summary>
    public class MaterialDrawerPanel : MonoBehaviour
    {
        public Transform spawnPosition;
        public GameObject industrialMaterial;
        private GameObject m_CandidateMaterial;
        private bool m_IsMaterialFetched = false;
        private Vector3 m_PreviousObjPos;
        public void Start()
        {
            m_CandidateMaterial = SpawnNewIndustrialMaterial(true);
            m_PreviousObjPos = m_CandidateMaterial.transform.position;
        }

        /// <summary>
        /// Check if a material has been fetched based on its position.
        /// </summary>
        private void Update()
        {
            if (!m_IsMaterialFetched && m_CandidateMaterial.transform.position != m_PreviousObjPos)
            {
                m_IsMaterialFetched = true;
            }
        }

        /// <summary>
        /// Spawn a new material once the material being fetched has exited the trigger area.
        /// </summary>
        private void OnTriggerExit(Collider other)
        {
            Debug.Log(other.tag);
            if (m_IsMaterialFetched && other.CompareTag("Material"))
            {
                m_IsMaterialFetched = false;
                m_CandidateMaterial = SpawnNewIndustrialMaterial();
                m_PreviousObjPos = m_CandidateMaterial.transform.position;
            }
        }

        /// <summary>
        /// Spawn a new material at the indicated spawn position.
        /// </summary>
        /// <param name="spin">Determines if the spawned material should spin initially.</param>
        /// <returns>The reference of the spawned game object.</returns>
        private GameObject SpawnNewIndustrialMaterial(bool spin = false)
        {
            GameObject obj = Instantiate(industrialMaterial, spawnPosition.position, Quaternion.identity);
            obj.GetComponent<Rigidbody>().useGravity = false;
            obj.GetComponent<XRGrabInteractable>().forceGravityOnDetach = true;
            return obj;
        }
    }
}
