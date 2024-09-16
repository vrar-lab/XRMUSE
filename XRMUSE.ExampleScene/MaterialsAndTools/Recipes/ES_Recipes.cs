using Photon.Pun;
using XRMUSE.Networking;
using System;
using System.Collections.Generic;
using UnityEngine;
using XRMUSE.SceneDescription;

namespace XRMUSE.ExampleScene
{
    /// <summary>
    /// Lists of all recipes known which fuses materials together with the use of a specific tool.
    /// Uses only integer ids for the recipes mat and tool, but a gameobject for the production, assumes the production gameobject is in the pool
    /// basically for recipe "i", the recipe is recipesMat1[i] + recipesMat2[i] using recipesTool[i] => recipesProductions[i]
    /// If trying to do something not in the recipes => garbage prefab is given
    /// </summary>
    public class ES_Recipes : MonoBehaviour
    {
        public static ES_Recipes instance;
        public int[] recipesMat1;
        public int[] recipesMat2;
        public int[] recipesTool;
        public GameObject[] recipesProductions;
        public GameObject garbagePrefab;

        public bool useSD = true;
        public string[] recipesProductionsSD;

        void Start()
        {
            Load();
        }

        public void Load()
        {
            instance = this;
            loadFromSD();
            ComputeRecipes();
            PoolManager pm = (PhotonNetwork.PrefabPool as PoolManager);
            if (!pm.spawnablePrefabs.Contains(garbagePrefab))
                pm.spawnablePrefabs.Add(garbagePrefab);
            foreach (GameObject go in recipesProductions)
                if (!pm.spawnablePrefabs.Contains(go))
                    pm.spawnablePrefabs.Add(go);
        }

        void loadFromSD()
        {
            if (!useSD)
                return;
            recipesProductions = new GameObject[recipesProductionsSD.Length];
            for (int i = 0; i < recipesProductions.Length; i++)
                recipesProductions[i] = TS_TimelineNetworked.networkedInstantiables[recipesProductionsSD[i]];
        }

        public Dictionary<int, Dictionary<(int, int), GameObject>> recipes = new Dictionary<int, Dictionary<(int, int), GameObject>>();
        /// <summary>
        /// Computes the recipes in a series of dictionaries for faster access
        /// </summary>
        void ComputeRecipes()
        {
            recipes.Clear();
            for (int i = 0; i < recipesMat1.Length; i++)
            {
                if (!recipes.ContainsKey(recipesTool[i]))
                    recipes.Add(recipesTool[i], new Dictionary<(int, int), GameObject>());
                recipes[recipesTool[i]].Add((recipesMat1[i], recipesMat2[i]), recipesProductions[i]);
            }
        }

        public static GameObject getRecipe(int matA, int matB, int tool)
        {
            return instance._getRecipe(matA, matB, tool);
        }
        public GameObject _getRecipe(int matA, int matB, int tool)
        {
            try
            {
                return recipes[tool][(matA, matB)];

            }
            catch (Exception)
            {
                try
                {
                    return recipes[tool][(matB, matA)];
                }
                catch (Exception)
                {
                    return garbagePrefab;
                }
            }
        }
    }
}