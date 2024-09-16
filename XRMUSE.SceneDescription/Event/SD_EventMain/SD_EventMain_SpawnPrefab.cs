using System.Collections.Generic;
using UnityEngine;

namespace XRMUSE.SceneDescription
{
    [SD_EventAttribute("spawngameobject")]
    public class SD_EventMain_SpawnPrefab : SD_EventMain
    {
        private static Dictionary<string, GameObject> _PREFAB = new Dictionary<string, GameObject>();
        public static GameObject PREFAB(string s)
        {
            if (!_PREFAB.ContainsKey(s))
            {
                _PREFAB.Add(s, (GameObject)Resources.Load(SD_EventMain.EVENTS_PREFABS_LOCATION + "/" + s, typeof(GameObject)));
            }
            return _PREFAB[s];
        }

        public string prefabType;
        /// <summary>
        /// optional zone where the stimulus is instancied 
        /// </summary>
        public TaskSystem_Zone zone = null;
        public GameObject go_instance;
        public void EventInterrupt(SD_Event sd_event)
        {
            if (isEnded)
                return;
            isEnded = true;
            sd_event.currentState = SD_Event.State.CANCELLED;
            if (go_instance != null)
                go_instance.SetActive(false);
            SD_Log log = new SD_Log();
            log.ID = sd_event.ID;
            log.logtype = SD_Log.LogType.END;
            log.message = "Event SpawnPrefab " + log.ID + " ended";
            SD_Logger.AddToProcess(log);
        }

        bool isEnded = false;
        bool isStarted = false;
        public void EventStart(SD_Event sd_event)
        {
            if (isEnded || isStarted)
                return;
            isStarted = true;
            go_instance = GameObject.Instantiate(PREFAB(prefabType));
            if (zone != null)
                go_instance.transform.position = zone.Center_World();
            go_instance.SetActive(true);

            SD_Log log = new SD_Log();
            log.ID = sd_event.ID;
            log.logtype = SD_Log.LogType.START;
            log.message = "Event SpawnGameObject " + log.ID + " started";
            SD_Logger.AddToProcess(log);
        }

        public void Parse(string[] lines, int fromLine)
        {
            for (int i = fromLine; i < lines.Length; i++)
            {
                if (lines[i][0] == '#')
                    break;
                var keys = lines[i].Split(':');
                switch (keys[0].ToLower().Trim())
                {
                    case "prefabtype":
                    case "prefab":
                        prefabType = keys[1].Trim();
                        break;
                    case "zone":
                        zone = TaskSystem_Zone.GetZoneById(keys[1].Trim());
                        break;
                }
            }
        }
    }
}