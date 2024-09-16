using UnityEngine;

namespace XRMUSE.SceneDescription
{
    /// <summary>
    /// Generic interface for instantiable main behaviour
    /// </summary>
    public interface SD_InstantiableMain
    {
        /// <summary>
        /// Location under Assets/Resources for InstantiableMain related prefabs
        /// </summary>
        public const string INSTANTIABLES_PREFABS_LOCATION = "SceneDescription/InstantiablesPrefabs";

        /// <summary>
        /// Generates a gameobject out of the main behaviour, most likely using a prefab in resources
        /// </summary>
        /// <param name="parent_go">optional GameObject the returned instance will be a child of</param>
        /// <returns></returns>
        public GameObject instantiateOn(GameObject parent_go);

        /// <summary>
        /// Parses the Instantiable's main type specific values from the lines of the text file, stops when reaching a '#'
        /// </summary>
        /// <param name="lines">All lines of the text file</param>
        /// <param name="fromLine">line to start reading the specific values from</param>
        public void Parse(string[] lines, int fromLine);
    }
}
