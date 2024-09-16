using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using XRMUSE.ExampleScene;
using UnityEngine;

namespace XRMUSE.ExampleScene
{
    public class ObjectiveUpdateEvent
    {
        public int matType;
        public int haveProduced;
        public int goalNumber;

        public ObjectiveUpdateEvent(int matType, int haveProduced, int goalNumber)
        {
            this.matType = matType;
            this.haveProduced = haveProduced;
            this.goalNumber = goalNumber;
        }
    }

    public class ObjectivePanelController : MonoBehaviour
    {
        private Transform _title;
        private Transform _panel;
        private Transform _column;
        private bool isRollingout;
        private bool isTextRollingOut = false;
        public List<ObjectiveUpdateEvent> UpdateEvents = new List<ObjectiveUpdateEvent>();
        private void Awake()
        {
            _panel = transform.GetChild(0);
            _title = transform.GetChild(1);
            _column = transform.GetChild(2);
        }
        /// <summary>
        /// Add an event to update the objective panel. The event will be handled asynchronously.
        /// </summary>
        /// <param name="matType">The type of the product to be updated.</param>
        /// <param name="have_produced">The number of products that have been produced.</param>
        /// <param name="count">The total number of the products that need to be produced in total (incl. already produced).</param>
        public void AddObjectiveUpdateEvent(int matType, int have_produced, int count)
        {
            UpdateEvents.Add(new ObjectiveUpdateEvent(matType, have_produced, count));
        }
        /// <summary>
        /// Handle the update event whenever possible.
        /// </summary>
        private void Update()
        {
            if (UpdateEvents.Count > 0)
            {
                UpdateObjective(UpdateEvents[0]);
                UpdateEvents.RemoveAt(0);
            }
        }
        /// <summary>
        /// Update the objective UI panel based on provided information.
        /// </summary>
        /// <param name="e">The update event.</param>
        public void UpdateObjective(ObjectiveUpdateEvent e)
        {   
            for (int i = 0; i < transform.GetChild(2).childCount; i++)
            {
                var row = transform.GetChild(2).GetChild(i).GetComponent<TileRowController>();
                if (row.isBeingUsed && row.matType == e.matType)
                {
                    if(e.haveProduced < e.goalNumber)
                        row.GetComponentInChildren<TMP_Text>().text = $"Produce {e.goalNumber - e.haveProduced} {SD_EventMain_ScreenObjectivesIconLoad.materialsSprites[e.matType].name}s.";
                    else
                        row.GetComponentInChildren<TMP_Text>().text = $"Complete!";
                }
            }
        }
        /// <summary>
        /// A wrapper method for the _DisplayObjectives method.
        /// </summary>
        public void DisplayObjectives(List<int[]> objectives, float yScale)
        {
            if (objectives == null)
                return;
            StartCoroutine(_DisplayObjectives(objectives, yScale));
        }
        /// <summary>
        /// Clear all the objective information on the panel.
        /// </summary>
        public void Clear()
        {
            var tiles = GetComponentsInChildren<TileController>();
            foreach (var tile in tiles)
            {
                tile.transform.localScale = Vector3.zero;
            }
            for (int i = 0; i < _column.childCount; i++)
            {
                _column.GetChild(i).GetComponentInChildren<TMP_Text>().alpha = 0f;
            }
        }
        /// <summary>
        /// Reveal all the objective information with animations.
        /// </summary>
        /// <param name="objectives">All the objectives.</param>
        /// <param name="yScale">The y scale (height) of the objective UI panel.</param>
        /// <returns></returns>
        private IEnumerator _DisplayObjectives(List<int[]> objectives, float yScale)
        {
            int n = objectives.Count;
            float duration = 1f;
            float halfDuration = duration / 2.0f;
            float speed = 10f;
            float lapse = 0f;
            var tmpTex = _title.GetComponentInChildren<TMP_Text>();
            float panelScaleY = yScale;

            transform.localPosition = new Vector3(-0.05f, yScale, 0f);

            while (lapse < duration)
            {
                _panel.localScale = new Vector3(1f, Mathf.Lerp(panelScaleY, 0f, Mathf.Pow(0.9f, lapse * 60f)), 1f);
                tmpTex.alpha = lapse / duration;

                if (!isTextRollingOut && lapse > halfDuration)
                {
                    StartCoroutine(ShowObjectiveDescription(objectives));
                    isTextRollingOut = true;
                }
            
                lapse += Time.deltaTime;
                yield return null;
            }
            _panel.localScale = new Vector3(1f, panelScaleY, 1f);
            tmpTex.alpha = 1f;
        
            isRollingout = false;
            isTextRollingOut = false;
        }
        /// <summary>
        /// Show the textual description of each objective.
        /// </summary>
        /// <param name="objectives">All the objectives.</param>
        /// <returns></returns>
        private IEnumerator ShowObjectiveDescription(List<int[]> objectives)
        {
            for (int i = 0; i < objectives.Count; i++)
            {
                _column.GetChild(i).GetComponent<TileRowController>().SpawnAt(
                    objectives[i][0], 
                    0, 
                    new Tuple<TMP_Text, string>(
                        _column.GetChild(i).GetComponentInChildren<TMP_Text>(),
                        $"Produce (0 / {objectives[i][1]}) {SD_EventMain_ScreenObjectivesIconLoad.materialsSprites[objectives[i][0]].name}s.")
                );
                yield return new WaitForSeconds(0.03f);
            }
        }
    }
}