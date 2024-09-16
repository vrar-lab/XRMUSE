using Photon.Pun;
using XRMUSE.Networking;
using UnityEngine;

namespace XRMUSE.ExampleScene
{
    /// <summary>
    /// A Material Box can produce a specific material, on load it automatically adds its produced material gameobject to the PoolManager
    /// </summary>
    public class ES_MaterialBox : MonoBehaviour
    {
        public GameObject producedMaterial;
        public Vector3 spawnPoint = new Vector3(0f, 0.5f);
        public int playerOwner = 1;

        void Start()
        {
            PoolManager pm = (PhotonNetwork.PrefabPool as PoolManager);
            if (!pm.spawnablePrefabs.Contains(producedMaterial))
                pm.spawnablePrefabs.Add(producedMaterial);
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.P))
            {
                Produce();
            }
        }

        public GameObject Produce()
        {
            return PoolManager.SpawnPrefab(producedMaterial, transform.position + spawnPoint, Quaternion.identity);
        }
    }
}