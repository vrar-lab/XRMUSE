using XRMUSE.Networking;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace XRMUSE.SceneDescription
{
    /// <summary>
    /// Component that manages a simple timeline for the task system.
    /// Basically it will just parse the description depending on the scene's SD_ParseMonoBehaviour parameter then feed a TIMELINE START log to the singleton logger.
    /// Edit of TS_Timeline to add network compatibility
    /// </summary>
    public class TS_TimelineNetworked : MonoBehaviour
    {
        public static float time
        {
            get => current == null ? -1 : current.timeSinceStartup - current.timeAtStart;
        }
        float timeSinceStartup = 0;
        public static TS_TimelineNetworked current = null;
        public float timeAtStart = -1;
        public bool loadOnStart = true;
        public bool startOnStart = false;
        
        public static UnityEvent StartProcess = new UnityEvent();
        private void OnEnable()
        {
            StartProcess.AddListener(() =>
            {
                StartTimeline();
            });
        }

        void Awake()
        {
            current = this;
            
            try
            { FindObjectOfType<SD_ParseMonoBehaviour>().Load(); }
            catch (Exception ) {}
            SpawnNetworked();

            if (loadOnStart)
                SendStartLoadLog();
            if (startOnStart)
                StartTimeline();
        }

        private bool flag = false;

        
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
            //Produces -> LOAD; -1; START
            SD_Logger.AddToProcess(new SD_Log(new SD_EventLogID("LOAD", -1), SD_Log.LogType.START));
        }

        public static Dictionary<string, GameObject> networkedInstantiables = new Dictionary<string, GameObject>();
        public PoolManager poolManager;
        public void SpawnNetworked()
        {
            networkedInstantiables.Clear();
            if (poolManager == null)
                poolManager = GameObject.FindObjectOfType<PoolManager>();
            foreach (var obj in SD_InstantiableType.instantiable_types)
            {
                if (Attribute.IsDefined(obj.Value.mainType.GetType(), typeof(SD_IsNetworkMainType)))
                {
                    GameObject go = SD_InstantiableType.instantiable_types[obj.Key].InstantiateInstantiableType(Vector3.zero);
                    go.SetActive(false);
                    poolManager.spawnablePrefabs.Add(go);
                    networkedInstantiables.Add(obj.Key, go);
                }
            }
        }
    }
}