using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace XRMUSE.SceneDescription
{
    public interface SD_StimulusMain
    {
        /// <summary>
        /// Location under Assets/Resources for StimulusMain related prefabs
        /// </summary>
        public const string STIMULI_PREFABS_LOCATION = "PrecisionFarming/SceneDescription/StimuliPrefabs";
        /// <summary>
        /// Generates a gameobject and a StimulusInstance out of the main behaviour, most likely using a prefab in resources
        /// </summary>
        /// <param name="parent_go">optional GameObject the returned instance will be a child of</param>
        /// <returns></returns>
        public (GameObject, SD_StimulusInstance) instantiateOn(GameObject parent_go);
        /// <summary>
        /// Parses the Stimulus' main type specific values from the lines of the text file, stops when reaching a '#'
        /// </summary>
        /// <param name="lines">All lines of the text file</param>
        /// <param name="fromLine">line to start reading the specific values from</param>
        public void Parse(string[] lines, int fromLine);
    }
}
