using XRMUSE.Networking;
using UnityEngine;

namespace XRMUSE.SceneDescription
{
    [SD_EventAttribute("spawnnetworkedinstantiable")]
    public class SD_EventMain_SpawnNetworkedInstantiable : SD_EventMain
    {
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
            log.message = "Event SpawnNetworkedInstantiable " + log.ID + " ended";
            SD_Logger.AddToProcess(log);
        }

        public void EventStart(SD_Event sd_event)
        {
            if (isEnded || isStarted)
                return;
            isStarted = true;

            if (SD_Settings.playerNum == playerId)
            {
                Debug.LogWarning(instantiableId);
                go_instance = PoolManager.SpawnPrefab(TS_TimelineNetworked.networkedInstantiables[instantiableId], zone != null ? zone.Center_World() : spawnPosition, Quaternion.identity);
                go_instance.SetActive(true);
            }

            SD_Log log = new SD_Log();
            log.ID = sd_event.ID;
            log.logtype = SD_Log.LogType.START;
            log.message = "Event SpawnNetworkedInstantiable " + log.ID + " started";
            SD_Logger.AddToProcess(log);
        }

        string instantiableId;
        TaskSystem_Zone zone = null;
        Vector3 spawnPosition = Vector3.zero;
        bool isEnded = false;
        bool isStarted = false;
        public GameObject go_instance;
        int playerId = 1;
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
                    case "instantiable_id":
                        instantiableId = keys[1].Trim();
                        break;
                    case "zone":
                        zone = TaskSystem_Zone.GetZoneById(keys[1].Trim());
                        break;
                    case "positionx":
                    case "position_x":
                    case "position.x":
                    case "x":
                        spawnPosition = new Vector3(float.Parse(keys[1].Trim()), spawnPosition.y, spawnPosition.z);
                        break;
                    case "positiony":
                    case "position_y":
                    case "position.y":
                    case "y":
                        spawnPosition = new Vector3(spawnPosition.x, float.Parse(keys[1].Trim()), spawnPosition.z);
                        break;
                    case "positionz":
                    case "position_z":
                    case "position.z":
                    case "z":
                        spawnPosition = new Vector3(spawnPosition.x, spawnPosition.y, float.Parse(keys[1].Trim()));
                        break;
                    case "position":
                        spawnPosition = new Vector3(float.Parse(keys[1].Trim()), float.Parse(keys[2].Trim()), float.Parse(keys[3].Trim()));
                        break;
                    case "playerid":
                    case "player_id":
                    case "player":
                    case "owner_id":
                    case "ownerid":
                    case "owner":
                        playerId = int.Parse(keys[1].Trim());
                        break;
                }
            }
        }
    }
}