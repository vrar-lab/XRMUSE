using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace XRMUSE.SceneDescription
{
    /// <summary>
    /// Component that manages a simple timeline for the task system.
    /// Basically it will just parse the description depending on the scene's SD_ParseMonoBehaviour parameter then feed a TIMELINE START log to the singleton logger.
    /// </summary>
    public class TS_Timeline : MonoBehaviour
    {
        public static float time
        {
            get => current == null ? -1 : current.timeSinceStartup - current.timeAtStart;
        }
        float timeSinceStartup = 0;
        public static TS_Timeline current = null;
        public float timeAtStart = -1;
        public bool loadOnStart = true;
        public bool startOnStart = false;

        void Start()
        {
            try
            { FindObjectOfType<SD_ParseMonoBehaviour>().Load(); }
            catch (Exception) { }
            if (loadOnStart)
                SendStartLoadLog();
            if (startOnStart)
                StartTimeline();
        }
        
        void Update()
        {
            SD_EventManager.Process();
            timeSinceStartup = Time.realtimeSinceStartup;
        }

        public void StartTimeline()
        {
            SendStartTimelineLog();
            current = this;
            timeAtStart = Time.realtimeSinceStartup;
        }
        public static void StartTimelineInstance()
        {
            GameObject.FindFirstObjectByType<TS_Timeline>().StartTimeline();
        }

        public static void SendStartTimelineLog()
        {
            //Produces -> TIMELINE; -1; START
            SD_Logger.AddToProcess(new SD_Log(new SD_EventLogID("TIMELINE", -1), SD_Log.LogType.START));
        }

        public static void SendStartLoadLog()
        {
            //Produces -> TIMELINE; -1; START
            SD_Logger.AddToProcess(new SD_Log(new SD_EventLogID("LOAD", -1), SD_Log.LogType.START));
        }
    }
}