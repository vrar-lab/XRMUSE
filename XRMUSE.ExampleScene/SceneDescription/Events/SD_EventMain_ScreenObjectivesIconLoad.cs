using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using XRMUSE.SceneDescription;

namespace XRMUSE.ExampleScene
{
    [SD_Instantiable("screenobjectivesiconload")]
    public class SD_EventMain_ScreenObjectivesIconLoad : SD_InstantiableMain
    {
        public static Sprite[] materialsSprites = { };
        public static Sprite[] toolsSprites = { };
        /// <summary>
        /// Returns a blank gameobject for simple compatibility, only the parsing is really important here
        /// </summary>
        /// <param name="parent_go"></param>
        /// <returns></returns>
        public GameObject instantiateOn(GameObject parent_go)
        {
            return null;
            //return new GameObject();
        }

        public void Parse(string[] lines, int fromLine)
        {
            bool isMaterials = true;
            materialsSprites = new Sprite[0];
            toolsSprites = new Sprite[0];
            for (int i = fromLine; i < lines.Length; i++)
            {
                if (lines[i][0] == '#')
                    break;
                var keys = lines[i].Split(':');
                switch (keys[0].ToLower().Trim())
                {
                    case "ismaterial":
                    case "is_material":
                    case "ismaterials":
                    case "is_materials":
                    case "material":
                    case "materials":
                        isMaterials = true;
                        break;
                    case "istool":
                    case "is_tool":
                    case "istools":
                    case "is_tools":
                    case "tool":
                    case "tools":
                        isMaterials = false;
                        break;
                    default:
                        int key = int.Parse(keys[0].Trim());
                        string value = keys[1].Trim();
                        Sprite sp = Resources.Load<Sprite>(value);
                        if (key >= (isMaterials ? materialsSprites.Length : toolsSprites.Length)) //dirty and inefficient but easiest way to implement
                        {
                            if (isMaterials)
                                Array.Resize(ref materialsSprites, key + 1);
                            else
                                Array.Resize(ref toolsSprites, key + 1);
                        }
                        if (isMaterials)
                            materialsSprites[key] = sp;
                        else
                            toolsSprites[key] = sp;

                        break;
                }
            }
        }
    }
}