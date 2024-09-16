using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace XRMUSE.SceneDescription
{
    /// <summary>
    /// Example of a StimuliMain loading the entire CPC receiver with it, CPC receiver should NOT be created like this but for the purpose of the example it's fine
    /// </summary>
    [SD_StimulusAttribute("cpcstimulusexample")]
    public class SD_StimuliMain_ExampleCPC : SD_StimulusMain
    {
        public const string PREFAB_LOCATION = SD_StimulusMain.STIMULI_PREFABS_LOCATION + "/SD_CPCStimulusExample";

        private static GameObject _PREFAB;
        public static GameObject PREFAB
        {
            get => _PREFAB == null ? (_PREFAB = (GameObject)Resources.Load(PREFAB_LOCATION, typeof(GameObject))) : _PREFAB;
        }

        public (GameObject, SD_StimulusInstance) instantiateOn(GameObject parent_go)
        {
            GameObject go = GameObject.Instantiate(PREFAB);
            if (parent_go != null)
                go.transform.parent = parent_go.transform;
            SD_StimulusInstance s = go.GetComponent<SD_StimulusInstance>();
            return (go, s);
        }

        public void Parse(string[] lines, int fromLine)
        {
            //nothing to parse here
        }
    }
}