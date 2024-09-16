using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using XRMUSE.ExampleScene;
using UnityEngine;

namespace XRMUSE.ExampleScene
{
    /// <summary>
    /// Controlling the behavior of an individual tile.
    /// </summary>
    public class TileController : MonoBehaviour
    {
        private static Dictionary<TileType, Color> edgeColor = new Dictionary<TileType, Color>()
        {
            {TileType.Tool, new Color(189/255f, 27f/255f, 122f/255f)},
            {TileType.Material , new Color(27f/255f, 66f/255f, 189f/255f)},
            {TileType.Arrow , new Color(0.7f, 0.7f, 0.7f)},
        };
        private MeshRenderer _edgeRenderer;
        private MeshRenderer _iconRenderer;
        private Vector3 _targetScale = new Vector3(1.5f, 1.5f, 2.25f);
        private void Awake()
        {
            transform.localScale = Vector3.zero;
            _edgeRenderer = transform.GetChild(0).GetComponent<MeshRenderer>();
            _iconRenderer = transform.GetChild(2).GetComponent<MeshRenderer>();
        }
        /// <summary>
        /// An wrapper for the _Spawn method.
        /// </summary>
        public void Spawn(int id, TileType type, Tuple<TMP_Text, string> objectiveDescription = null)
        {
            StartCoroutine(_Spawn(id, type, objectiveDescription));
        }
        /// <summary>
        /// Configure the color, texture, and position of the tile according to the combination information.
        /// </summary>
        /// <param name="id">The id used internally pointing to the corresponding texture file.</param>
        /// <param name="type">Whether the tile represents a material or a product.</param>
        /// <param name="objectiveDescription">The accompanying text. It will be displayed to the right of the tile. This is only used for the objective UI panel.</param>
        /// <returns></returns>
        IEnumerator _Spawn(int id, TileType type, Tuple<TMP_Text, string> objectiveDescription = null)
        { 
            _edgeRenderer.material.color = edgeColor[type];

            if (type != TileType.Arrow)
            {
                if (type == TileType.Material)
                    _iconRenderer.material.mainTexture = SD_EventMain_ScreenObjectivesIconLoad.materialsSprites[id].texture;
                else if (type == TileType.Tool)
                    _iconRenderer.material.mainTexture = SD_EventMain_ScreenObjectivesIconLoad.toolsSprites[id].texture;
            }
        
            float duration = 0.25f;
            float lapse = 0f;
            while (lapse < duration)
            {
                transform.localScale = Vector3.Lerp(Vector3.zero, _targetScale, lapse / duration);
                lapse += Time.deltaTime;
                yield return null;
            }
            transform.localScale = _targetScale;

            if (objectiveDescription != null)
            {
                var TMPText = objectiveDescription.Item1;
                TMPText.alpha = 0f;
                TMPText.text = objectiveDescription.Item2;
                StartCoroutine(ShowText(TMPText));
            }
        }
        /// <summary>
        /// Show the accompanying text with animations.
        /// </summary>
        /// <param name="tmpText"></param>
        /// <returns></returns>
        private IEnumerator ShowText(TMP_Text tmpText)
        {
            float duration = 0.25f;
            float lapse = 0f;
            while (lapse < duration)
            {
                tmpText.alpha = lapse / duration;
                lapse += Time.deltaTime;
                yield return null;
            }
            tmpText.alpha = 1f;
        }
    }
}
