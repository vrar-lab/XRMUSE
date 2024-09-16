using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using XRMUSE.ExampleScene;
using UnityEngine;

namespace XRMUSE.ExampleScene
{
    public enum TileType
    {
        Material,
        Tool,
        Arrow
    }
    /// <summary>
    /// This class controls the behaviors of the UI panel displaying information on possible combinations.
    /// The panel automatically adapts its size depending on the number of combinations available.
    /// </summary>
    public class CombinationPanelController : MonoBehaviour
    {
        private Transform _title;
        private Transform _panel;
        private Transform _leftColumn;
        private Transform _rightColumn;
        private bool _isRollingout;
        private bool _isTileRollingOut = false;
        private void Awake()
        {
            _panel = transform.GetChild(0);
            _title = transform.GetChild(1);
            _leftColumn = transform.GetChild(2);
            _rightColumn = transform.GetChild(3);
        }
        public void DisplayCombinations(List<int[]> combs, float yScale)
        {
            StartCoroutine(_DisplayCombinations(combs, yScale));
        }
        /// <summary>
        /// Clear all the combination information on the panel.
        /// </summary>
        public void Clear()
        {
            var tiles = GetComponentsInChildren<TileController>();
            foreach (var tile in tiles)
            {
                tile.transform.localScale = Vector3.zero;
            }
        }
        /// <summary>
        /// Set up the UI panel (e.g., position, size) based on the number of combinations to be displayed.
        /// Depending on the number, the combinations may be displayed in single or dual columns.
        /// </summary>
        /// <param name="combs">A list of all possible combinations.</param>
        /// <param name="yScale">The y scale (i.e., height) of the UI panel.</param>
        private IEnumerator _DisplayCombinations(List<int[]> combs, float yScale)
        {
            int n = combs.Count;
            bool isDoubleColumn = n > 7;
            float duration = 1f;
            float halfDuration = duration / 2f;
            float speed = 10f;
            float lapse = 0f;
            var tmpTex = _title.GetComponentInChildren<TMP_Text>();

            float panelScaleX = isDoubleColumn ? 2f : 1.0f;
            float panelScaleY = yScale;
            float titlePosX = isDoubleColumn ? 1f : 0.5f;
            _title.localPosition = new Vector3(titlePosX, 0f, 0f);
            transform.localPosition = new Vector3(0.05f, yScale, 0f);

            if (isDoubleColumn)
            {
                _leftColumn.localPosition = new Vector3(1.5f, -0.2f, 0f);
                _rightColumn.localPosition = new Vector3(0.5f, -0.2f, 0f);
                _rightColumn.gameObject.SetActive(true);
            }
            else
            {
                _leftColumn.localPosition = new Vector3(0.5f, -0.2f, 0f);
                _rightColumn.localPosition = new Vector3(0.5f, -0.2f, 0f);
                _rightColumn.gameObject.SetActive(false);
            }

            var loads = LoadBalance(n);

            while (lapse < duration)
            {
                _panel.localScale = new Vector3(panelScaleX, Mathf.Lerp(panelScaleY, 0f, Mathf.Pow(0.9f, lapse * 60f)), 1f);
                tmpTex.alpha = lapse / duration;
                if (!_isTileRollingOut && lapse > halfDuration)
                {
                    StartCoroutine(RollOutTiles(loads, combs));
                    _isTileRollingOut = true;
                }
                lapse += Time.deltaTime;
                yield return null;
            }
            _panel.localScale = new Vector3(panelScaleX, panelScaleY, 1f);
            tmpTex.alpha = 1f;

            _isRollingout = false;
            _isTileRollingOut = false;
        }
        /// <summary>
        /// Reveal all the combinations with animations.
        /// </summary>
        /// <param name="loads">The number of combinations arranged in each column.</param>
        /// <param name="combs">All the combinations to be displayed.</param>
        private IEnumerator RollOutTiles(Tuple<int, int> loads, List<int[]> combs)
        {
            for (int i = 0; i < loads.Item1; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    if (j == 3)
                    {
                        _leftColumn.GetChild(i).GetComponent<TileRowController>().SpawnAt(-1, j);
                        _leftColumn.GetChild(i).GetComponent<TileRowController>().SpawnAt(combs[i][j], j + 1);
                    }
                    else
                    {
                        _leftColumn.GetChild(i).GetComponent<TileRowController>().SpawnAt(combs[i][j], j);
                    }
                    yield return new WaitForSeconds(0.03f);
                }
            }
        
            for (int i = 0; i < loads.Item2; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    if (j == 3)
                    {
                        _rightColumn.GetChild(i).GetComponent<TileRowController>().SpawnAt(-1, j);
                        _rightColumn.GetChild(i).GetComponent<TileRowController>().SpawnAt(combs[i][j], j + 1);
                    }
                    else
                    {
                        _rightColumn.GetChild(i).GetComponent<TileRowController>().SpawnAt(combs[i][j], j);
                    }
                    yield return new WaitForSeconds(0.03f);
                }
            }
        }
        /// <summary>
        /// A utility function for calculating the number of combinations to be displayed in each column.
        /// </summary>
        /// <param name="n">The number of all possible combinations.</param>
        /// <returns>A tuple containing the number of combinations to be displayed in each column.</returns>
        private Tuple<int, int> LoadBalance(int n)
        {
            if (n <= 7) return new Tuple<int, int>(n, 0);
            if (n <= 10) return new Tuple<int, int>(5, n - 5);
            if (n == 11) return new Tuple<int, int>(6, 5);
            if (n == 12) return new Tuple<int, int>(6, 6);
            if (n == 13) return new Tuple<int, int>(7, 6);
            if (n == 14) return new Tuple<int, int>(7, 7);
            throw new Exception();
        }
    }
}