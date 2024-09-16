using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XRMUSE.SceneDescription;

namespace XRMUSE.ExampleScene
{
    /// <summary>
    /// Main Instantiable Type describing a materialbox that produces a material, said material is the ID of the material instantiable
    /// </summary>
    [SD_Instantiable("materialbox")]
    public class SD_InstantiableMain_MaterialBox : SD_InstantiableMain, SD_InstantiableDependencies
    {
        //PrefabLocation
        public const string PREFAB_LOCATION = SD_InstantiableMain.INSTANTIABLES_PREFABS_LOCATION + "/SD_MaterialBox";

        private static GameObject _PREFAB;
        public static GameObject PREFAB
        {
            get => _PREFAB == null ? (_PREFAB = (GameObject)Resources.Load(PREFAB_LOCATION, typeof(GameObject))) : _PREFAB;
        }

        //Parameters value
        Vector3 spawnpoint = new Vector3(0, 0.5f, 0);
        string producedMaterial;
        int playerOwner = 1;

        public GameObject instantiateOn(GameObject parent_go)
        {
            GameObject go = GameObject.Instantiate(PREFAB);
            if (parent_go != null)
                go.transform.parent = parent_go.transform;

            //MaterialBox parameters
            ES_MaterialBox matBox = go.GetComponentInChildren<ES_MaterialBox>();
            matBox.producedMaterial = SD_InstantiableType.instantiable_types[producedMaterial].InstantiateInstantiableType(Vector3.zero);
            matBox.playerOwner = playerOwner;

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
                    case "produced_material":
                        producedMaterial = keys[1].Trim();
                        break;
                    case "playerowner":
                    case "player_owner":
                        playerOwner = int.Parse(keys[1].Trim());
                        break;
                }
            }
        }

        public string[] getDependencies()
        {
            return new string[] { producedMaterial };
        }
    }
}