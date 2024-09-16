using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XRMUSE.SceneDescription;

namespace XRMUSE.ExampleScene
{
    /// <summary>
    /// Main Instantiable Type describing a tool that has an id for collision/recipes and a "playerowner" to give access only to one of the players
    /// </summary>
    [SD_IsNetworkMainType]
    [SD_Instantiable("tool")]
    public class SD_InstantiableMain_Tool : SD_InstantiableMain
    {
        //PrefabLocation
        public const string PREFAB_LOCATION = SD_InstantiableMain.INSTANTIABLES_PREFABS_LOCATION + "/SD_Tool";

        private static GameObject _PREFAB;
        public static GameObject PREFAB
        {
            get => _PREFAB == null ? (_PREFAB = (GameObject)Resources.Load(PREFAB_LOCATION, typeof(GameObject))) : _PREFAB;
        }

        //Parameters value
        int toolType = -1;
        int playerOwner = 1;

        public GameObject instantiateOn(GameObject parent_go)
        {
            GameObject go = GameObject.Instantiate(PREFAB);
            if (parent_go != null)
                go.transform.parent = parent_go.transform;
            //Material parameters
            ES_Tool mat = go.GetComponentInChildren<ES_Tool>();
            mat.toolType = toolType;
            mat.playerOwner = playerOwner;
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
                    case "tool_type":
                        toolType = int.Parse(keys[1].Trim());
                        break;
                    case "playerowner":
                    case "player_owner":
                        playerOwner = int.Parse(keys[1].Trim());
                        break;
                }
            }
        }
    }
}