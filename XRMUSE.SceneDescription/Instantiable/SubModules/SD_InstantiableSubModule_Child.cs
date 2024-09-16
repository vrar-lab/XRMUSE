using System.Collections.Generic;
using UnityEngine;

namespace XRMUSE.SceneDescription
{
    /// <summary>
    /// Submodules that adds a child to the instantiable.
    /// Prefabs must be under Resources/SubModuleChilds/path OR added to the LOADED_GENERIC Dictionary beforehand
    /// Primarly used to add a visualizer gameobject
    /// </summary>
    [SD_InstantiableSub("child")]
    public class SD_InstantiableSubModule_Child : SD_InstantiableSubmodule
    {
        //PrefabLocation
        public const string PREFAB_LOCATION = SD_InstantiableMain.INSTANTIABLES_PREFABS_LOCATION + "/SubModuleChilds";

        public static Dictionary<string, GameObject> LOADED_GENERIC = new Dictionary<string, GameObject>();
        public static GameObject PREFAB(string unknownpath = null)
        {
            try
            {
                if (!LOADED_GENERIC.ContainsKey(unknownpath))
                    LOADED_GENERIC.Add(unknownpath, (GameObject)Resources.Load(PREFAB_LOCATION + "/" + unknownpath, typeof(GameObject)));
                return LOADED_GENERIC[unknownpath];
            }
            catch (System.Exception) { Debug.LogError("UNAVAILABLE PREFAB"); }
            return null;
        }

        public void addSubmodule(GameObject addTo)
        {

            GameObject go = GameObject.Instantiate(PREFAB(path), addTo.transform);
            go.transform.localScale = new Vector3(go.transform.localScale.x / addTo.transform.lossyScale.x, go.transform.localScale.y / addTo.transform.lossyScale.y, go.transform.localScale.z / addTo.transform.lossyScale.z);
        }

        string path = "";
        public void Parse(string[] lines, int fromLine)
        {
            for (int i = fromLine; i < lines.Length; i++)
            {
                if (lines[i][0] == '#')
                    break;
                var keys = lines[i].Split(':');
                switch (keys[0].ToLower().Trim())//switch in case need to add functionalities later
                {
                    case "visualizer":
                        switch (keys[1].ToLower().Trim())
                        {
                            default:
                                path = keys[1].Trim();
                                break;
                        }
                        break;
                }
            }
        }
    }
}