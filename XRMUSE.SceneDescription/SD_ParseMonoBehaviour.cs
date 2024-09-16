using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace XRMUSE.SceneDescription
{
    /// <summary>
    /// Component to put in a scene that will read user-defined "scene description" text files and initiate the task/timeline system based on it
    /// </summary>
    public class SD_ParseMonoBehaviour : MonoBehaviour
    {
        /// <summary>
        /// Path of the folder containing all the instantiables files
        /// </summary>
        public string PathInstantiables;
        /// <summary>
        /// Path of the folder containing all the stimuli files
        /// </summary>
        public string PathStimuli;
        /// <summary>
        /// Path of the folder containing all the events files
        /// </summary>
        public string PathEvents;
        /// <summary>
        /// Should the parser (re) read the Instantiables/Stimuli?
        /// Can be useful if previous scene was using the same instantiables/stimuli but differs in events
        /// </summary>
        public bool rereadIS = true;
        /// <summary>
        /// Use paths from a params.txt file next to the .exe, impractical for editor
        /// </summary>
        #if UNITY_EDITOR
            private bool useParamsFile = false;
        #else
            private bool useParamsFile = true;
        #endif
        /// <summary>
        /// Loads the scene from the descriptions
        /// </summary>
        public void Load()
        {
            Parse();
        }
        /// <summary>
        /// Parses all files in the specified folders and populates the Timeline/Task/Events system
        /// </summary>
        public void Parse()
        {
            if (useParamsFile)
            {
#if UNITY_ANDROID && !UNITY_EDITOR
                SD_Settings.loadGeneralParams("/params.txt");
#endif
#if UNITY_EDITOR
                SD_Settings.loadGeneralParams();
#endif
                if (SD_Settings.pathInstantiables != null)
                    PathInstantiables = SD_Settings.pathInstantiables;
                if (SD_Settings.pathStimuli != null)
                    PathStimuli = SD_Settings.pathStimuli;
                if (SD_Settings.pathEvents != null)
                    PathEvents = SD_Settings.pathEvents;
            }
            if (SD_InstantiableType.instantiable_types.Count == 0 || rereadIS)
            {
                SD_InstantiableType.instantiable_types.Clear();
                foreach (string file in Directory.EnumerateFiles(PathInstantiables))
                {
                    string[] lines = File.ReadAllLines(file);
                    SD_InstantiableType tmp = SD_Parse.ParseInstantiable(lines);
                }
            }

            if (SD_StimulusType.stimulus_types.Count == 0 || rereadIS)
            {
                SD_StimulusType.stimulus_types.Clear();
                foreach (string file in Directory.EnumerateFiles(PathStimuli))
                {
                    string[] lines = File.ReadAllLines(file);
                    SD_StimulusType tmp = SD_Parse.ParseStimuli(lines);
                }
            }
            //Events are always reread
            SD_EventManager.Reset();
            foreach (string file in Directory.EnumerateFiles(PathEvents))
            {
                string[] lines = File.ReadAllLines(file);
                SD_Event tmp = SD_Parse.ParseEvent(lines);
                SD_EventManager.singleton.addWaiting(tmp);
            }

        }

    }
}