using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

namespace XRMUSE.Networking
{
    /// <summary>
    /// Object managing the recycling and spawning of networked game objects. 
    /// Static method "SpawnPrefab" can instantiate any GameObject reference as long as it was added to the list of spawnables.
    /// List of spawnables can be expanded on a loading phase by creating inactive gameobjects in real time or already be in the scene.
    /// </summary>
    public class PoolManager : MonoBehaviour, IPunPrefabPool
    {
        private Dictionary<string, Queue<GameObject>> pools;
        public List<GameObject> spawnablePrefabs = new List<GameObject>();

        /// <summary>Contains a GameObject per prefabId, to speed up instantiation.</summary>
        public readonly Dictionary<string, GameObject> ResourceCache = new Dictionary<string, GameObject>();

        /// <summary>
        /// Spawns a networked GameObject by reference to a prefab, said prefab must have been added to the pool on a loading phase.
        /// GameObjects MUST NOT BE RENAMED, they will get the name PI:id which is used by the destroy process.
        /// </summary>
        /// <param name="prefab">Reference to spawn</param>
        /// <param name="position">Position at spawn</param>
        /// <param name="rotation">Rotation at spawn, use either Quaternion.identity or prefab.rotation if the rotation should be default</param>
        /// <returns></returns>
        public static GameObject SpawnPrefab(GameObject prefab, Vector3 position, Quaternion rotation)
        {
            PoolManager pm = (PhotonNetwork.PrefabPool as PoolManager);
            int id = pm.spawnablePrefabs.IndexOf(prefab);
            //int id = Array.IndexOf(pm.spawnablePrefabs, prefab);
            if (id < 0)
            {
                Debug.LogError("Prefab not found! Make sure you also added the prefab to the list of spawnables in PoolManager");
                return null;
            }
            return PhotonNetwork.Instantiate(""+id, position, rotation);
        }

        public void Awake()
        {
            PhotonNetwork.PrefabPool = this;
            pools = new Dictionary<string, Queue<GameObject>>();
        }

        //public GameObject Prefab;
        public GameObject Instantiate(string prefabId, Vector3 position, Quaternion rotation)
        {
            GameObject res = null;
            //Get from cache
            if (!int.TryParse(prefabId, out int result))
            {
                bool cached = this.ResourceCache.TryGetValue(prefabId, out res);
                if (!cached)
                {
                    res = Resources.Load<GameObject>(prefabId);
                    if (res == null)
                    {
                        Debug.LogError("DefaultPool failed to load \"" + prefabId + "\". Make sure it's in a \"Resources\" folder. Or use a custom IPunPrefabPool.");
                    }
                    else
                    {
                        this.ResourceCache.Add(prefabId, res);
                    }
                }
            }
            else //null id => not from resources folder! Consider HashCode as key
            {
                res = spawnablePrefabs[result];
                prefabId = result+"";
            }

            //Pool instantiation check
            if (!pools.ContainsKey(prefabId))
            {
                pools.Add(prefabId, new Queue<GameObject>());
            }
            
            if (pools[prefabId].Count > 0)
            {
                GameObject go = pools[prefabId].Dequeue();
                go.transform.position = position;
                go.transform.rotation = rotation;
                go.SetActive(true);
                return go;
            }

            var tmp = GameObject.Instantiate(res, position, rotation);
            tmp.name = "PI:" + prefabId;
            return tmp;
        }

        public void Destroy(GameObject gameObject)
        {
            //Name not PI: => not a pooled object so DESTROY
            if (!gameObject.name.Substring(0, 3).Equals("PI:"))
            {
                GameObject.Destroy(gameObject);
                return;
            }

            //Object cleaning
            foreach(INetworking_PoolReset reset in gameObject.GetComponents<INetworking_PoolReset>())
                reset.Reset();

            //Pool instantiation check
            string id = gameObject.name.Substring(3);//get prefabid
            if (!pools.ContainsKey(id))
            {
                pools.Add(id, new Queue<GameObject>());
            }

            gameObject.SetActive(false);
            pools[id].Enqueue(gameObject);
        }
    }
}