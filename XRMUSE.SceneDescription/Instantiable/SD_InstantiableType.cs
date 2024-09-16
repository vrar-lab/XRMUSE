using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace XRMUSE.SceneDescription
{
    /// <summary>
    /// Defines a reusable "Instantiable" which can be retrieved by it's ID in a static dictionnary
    /// Any Instantiable can then be instantied with InstantiateInstantiableType
    /// </summary>
    public class SD_InstantiableType
    {
        /// <summary>
        /// All available Instantiable types
        /// Key: ID of the Instantiable type
        /// Value: an instantiable Instantiable type
        /// </summary>
        public static Dictionary<string, SD_InstantiableType> instantiable_types = new Dictionary<string, SD_InstantiableType>();

        private string _ID = null;
        /// <summary>
        /// Immutable ID, once the ID is assigned, the instance is added to the instantiable_types dictionnary
        /// </summary>
        public string ID
        {
            get => _ID;
            set
            {
                if (_ID != null)
                    return;
                _ID = value;
                instantiable_types.Add(_ID, this);
            }
        }
        /// <summary>
        /// The main behaviour of the Instantiable, usually made out of a prefab in resources
        /// </summary>
        public SD_InstantiableMain mainType;

        /// <summary>
        /// Additional behaviours that will be added to the mainType
        /// </summary>
        public SD_InstantiableSubmodule[] submodules;
        public SD_InstantiableType()
        {
        }
        public SD_InstantiableType(string ID, SD_InstantiableMain mainType, SD_InstantiableSubmodule[] submodules = null)
        {
            this.ID = ID; this.mainType = mainType; this.submodules = submodules;
        }
        /// <summary>
        /// Creates an instance of the Instantiable Type
        /// </summary>
        /// <param name="zone">optional zone the Instantiable should be instanced in</param>
        /// <param name="plant_parent">>optional GameObject the returned instance will be a child of</param>
        /// <returns></returns>
        public GameObject InstantiateInstantiableType(TaskSystem_Zone zone, GameObject plant_parent = null)
        {
            GameObject go = mainType.instantiateOn(plant_parent);
            if (submodules != null)
                foreach (var sub in submodules)
                    sub.addSubmodule(go);
            zone.currentLinked = go;
            go.transform.position = new Vector3(zone.Center_World().x, zone.y_ground, zone.Center_World().z);
            return go;
        }

        /// <summary>
        /// Creates an instance of the Instantiable Type
        /// </summary>
        /// <param name="position">optional position the Instantiable should be instanced in</param>
        /// <param name="plant_parent">>optional GameObject the returned instance will be a child of</param>
        /// <returns></returns>
        public GameObject InstantiateInstantiableType(Vector3 position, GameObject plant_parent = null)
        {
            GameObject go = mainType.instantiateOn(plant_parent);
            if (submodules != null)
                foreach (var sub in submodules)
                    sub.addSubmodule(go);
            if (go != null)
                go.transform.position = position;
            return go;
        }

        bool _isNetworkedChecked = false;
        bool _isNetworked = false;
        public bool IsNetworkedMain()
        {
            if (_isNetworkedChecked)
                return _isNetworked;
            if (mainType.GetType().GetCustomAttribute<SD_IsNetworkMainType>() != null)
                _isNetworked = true;
            _isNetworkedChecked = true;
            return _isNetworked;
        }
    }
}