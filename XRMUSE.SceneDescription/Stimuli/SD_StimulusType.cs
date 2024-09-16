using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace XRMUSE.SceneDescription
{
    /// <summary>
    /// Defines a reusable "Stimulus" which can be retrieved by it's ID in a static dictionary
    /// Any Stimulus can then be instantied with InstantiateStimulusType
    /// Differs from SD_Instantiable, only a MainType, this MainType will also add a SD_StimulusInstance to the generated stimuli which gives access to an Activate() and Interrupt() function
    /// </summary>
    public class SD_StimulusType
    {
        /// <summary>
        /// All available stimuli types
        /// Key: ID of the stimulus type
        /// Value: an instantiable stimulus type
        /// </summary>
        public static Dictionary<string, SD_StimulusType> stimulus_types = new Dictionary<string, SD_StimulusType>();
        private string _ID = null;
        public string ID
        {
            get => _ID;
            set
            {
                if (_ID != null)
                    return;
                _ID = value;
                stimulus_types.Add(_ID, this);
            }
        }

        public SD_StimulusMain mainType;
        public SD_StimulusType() { }
        public SD_StimulusType(string ID, SD_StimulusMain mainType)
        {
            this.ID = ID;
            this.mainType = mainType;
        }

        /// <summary>
        /// Creates an instance of the Stimulus Type
        /// </summary>
        /// <param name="zone">optional zone the stimulus should be instanced in</param>
        /// <param name="agentParent">>optional GameObject the returned instance will be a child of</param>
        /// <returns></returns>
        public (GameObject, SD_StimulusInstance) InstantiateStimulusType(TaskSystem_Zone zone = null, GameObject agentParent = null)
        {
            (GameObject go, SD_StimulusInstance instance) = mainType.instantiateOn(agentParent);
            if (zone != null)
                zone.currentLinked = go;
            if (zone != null)
                go.transform.position = new Vector3(zone.Center_World().x, zone.y_ground, zone.Center_World().z);
            return (go, instance);
        }

    }
}