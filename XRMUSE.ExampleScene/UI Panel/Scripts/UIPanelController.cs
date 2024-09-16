using System;
using System.Collections.Generic;
using XRMUSE.ExampleScene;
using UnityEngine;

namespace XRMUSE.ExampleScene
{
    /// <summary>
    /// This class controls the behaviors of the combination and objective UI panels.
    /// </summary>
    public class UIPanelController : MonoBehaviour
    {
        public CombinationPanelController combinationPanelController;
        public ObjectivePanelController objectivePanelController;
        /// <summary>
        /// Activate both UI panels.
        /// </summary>
        public void ActivateSelf()
        {
            combinationPanelController.gameObject.SetActive(true);
            objectivePanelController.gameObject.SetActive(true);
        }
        /// <summary>
        /// Display combination and objective information.
        /// </summary>
        /// <param name="combs">The combination information.</param>
        /// <param name="objectives">The objective information.</param>
        public void DisplayCombsAndObjectives(List<int[]> combs, List<int[]> objectives)
        {
            combinationPanelController.Clear();
            objectivePanelController.Clear();
            float yScale = Mathf.Max(DetermineCombinationPanelScaleY(combs.Count), DetermineObjectivePanelScaleY(objectives.Count));
            combinationPanelController.DisplayCombinations(combs, yScale);
            objectivePanelController.DisplayObjectives(objectives, yScale);
        }
        /// <summary>
        /// Determine the height of the objective UI panels depending on the amount of information to be displayed.
        /// </summary>
        /// <param name="n">The number of objectives to be displayed.</param>
        /// <returns></returns>
        private float DetermineObjectivePanelScaleY(int n)
        {
            if (n <= 5) return 1.10f;
            if (n == 6) return 1.27f;
            if (n == 7) return 1.45f;
            throw new Exception();
        }
        /// <summary>
        /// Determine the height of the combination UI panels depending on the amount of information to be displayed.
        /// </summary>
        /// <param name="n">The number of combinations to be displayed.</param>
        /// <returns></returns>
        private float DetermineCombinationPanelScaleY(int n)
        {
            if (n <= 5) return 1.10f;
            if (n == 6) return 1.27f;
            if (n == 7) return 1.45f;
            if (n <= 10) return 1.10f;
            if (n <= 12) return 1.27f;
            if (n <= 14) return 1.45f;
            throw new Exception();
        }
        /// <summary>
        /// An internal utility method.
        /// </summary>
        public static List<int[]> Convert(int[] mat1, int[] mat2, int[] tool, GameObject[] product)
        {
            List<int[]> toReturn = new List<int[]>();
            for (int i = 0; i < mat1.Length; i++)
            {
                toReturn.Add(new int[] {mat1[i], tool[i], mat2[i], product[i].GetComponent<ES_Material>().materialType});
            }
            return toReturn;
        }
    }
}
