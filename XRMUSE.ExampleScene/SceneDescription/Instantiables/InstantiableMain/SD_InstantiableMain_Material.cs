using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XRMUSE.SceneDescription;

namespace XRMUSE.ExampleScene
{
    /// <summary>
    /// Main Instantiable Type describing a material that has a type used in collisions/recipes
    /// </summary>
    [SD_IsNetworkMainType]
    [SD_Instantiable("material")]
    public class SD_InstantiableMain_Material : SD_InstantiableMain
    {
        //PrefabLocation
        public const string PREFAB_LOCATION = SD_InstantiableMain.INSTANTIABLES_PREFABS_LOCATION + "/SD_Material";

        private static GameObject _PREFAB;
        public static GameObject PREFAB
        {
            get => _PREFAB == null ? (_PREFAB = (GameObject)Resources.Load(PREFAB_LOCATION, typeof(GameObject))) : _PREFAB;
        }

        //Parameters value
        int material_type = -1;

        public GameObject instantiateOn(GameObject parent_go)
        {
            GameObject go = GameObject.Instantiate(PREFAB);
            if (parent_go != null)
                go.transform.parent = parent_go.transform;
            //Material parameters
            ES_Material mat = go.GetComponentInChildren<ES_Material>();
            mat.materialType = material_type;

            return go;
        }

        public void Parse(string[] lines, int fromLine)
        {
            for (int i = fromLine; i < lines.Length; i++)
            {
                if (lines[i][0] == '#')
                    break;
                var keys = lines[i].Split(':');
                switch (keys[0].ToLower().Trim())
                {
                    case "material_type":
                        material_type = int.Parse(keys[1].Trim());
                        break;
                }
            }
        }
    }
}