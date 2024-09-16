using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace XRMUSE.SceneDescription
{
    /// <summary>
    /// Simple StimuliInstance that is activated/interrupted through the SetActive function of the stimulus' gameobject
    /// </summary>
    public class SD_StimuliInstance_ExampleCPC : MonoBehaviour, SD_StimulusInstance
    {
        public GameObject toDisable;
        public void Activate()
        {
            toDisable.SetActive(true);
        }

        public void Interrupt()
        {
            toDisable.SetActive(false);
        }
    }
}