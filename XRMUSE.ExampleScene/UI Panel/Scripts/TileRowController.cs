using System;
using System.Collections.Generic;
using TMPro;
using XRMUSE.ExampleScene;
using UnityEngine;

namespace XRMUSE.ExampleScene
{
    public class TileRowController : MonoBehaviour
    {
        public bool isBeingUsed = false;
        public bool isObjective = false;
        public int matType = -1;
        private readonly Dictionary<int, TileType> _typeMap = new Dictionary<int, TileType>()
        {
            { 0, TileType.Material },
            { 1, TileType.Tool },
            { 2, TileType.Material },
            { 3, TileType.Arrow },
            { 4, TileType.Material }
        };
        /// <summary>
        /// Controls the spawn of a tile at the row level.
        /// </summary>
        /// <param name="id">Internal variable use to indicate the type of the tile.</param>
        /// <param name="at">At which column should the tile be spawned within the row.</param>
        /// <param name="objectiveDescription">The accompany text to be displayed to the right of the tile.</param>
        public void SpawnAt(int id, int at, Tuple<TMP_Text, string> objectiveDescription = null)
        {
            isBeingUsed = true;
            isObjective = objectiveDescription != null;
            if (isObjective)
            {
                matType = id;
            }
            transform.GetChild(at).GetComponent<TileController>().Spawn(id, _typeMap[at], objectiveDescription);
        }
    }
}
