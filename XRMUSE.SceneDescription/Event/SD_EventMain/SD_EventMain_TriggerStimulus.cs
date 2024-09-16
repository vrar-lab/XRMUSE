using UnityEngine;

namespace XRMUSE.SceneDescription
{
    /// <summary>
    /// Event that instantiate a stimulus instance from the ID of its type
    /// </summary>
    [SD_EventAttribute("stimulus")]
    public class SD_EventMain_TriggerStimulus : SD_EventMain
    {
        /// <summary>
        /// ID of the SD_StimulusMain to instantiate
        /// </summary>
        public string stimulusType;
        public SD_StimulusInstance stimulus_instance = null;
        /// <summary>
        /// optional zone where the stimulus is instancied 
        /// </summary>
        public TaskSystem_Zone zone = null;
        public GameObject stimulusParent = null;
        public GameObject go_instance;
        public void EventInterrupt(SD_Event sd_event)
        {
            if (stimulus_instance != null)
                stimulus_instance.Interrupt();
            sd_event.currentState = SD_Event.State.CANCELLED;

            SD_Log log = new SD_Log();
            log.ID = sd_event.ID;
            log.logtype = SD_Log.LogType.END;
            log.message = "Event TriggerStimulus " + log.ID + " ended";
            //SD_Logger.singleton.processLog(log);
            SD_Logger.AddToProcess(log);
        }

        public void EventStart(SD_Event sd_event)
        {
            (go_instance, stimulus_instance) = SD_StimulusType.stimulus_types[stimulusType].InstantiateStimulusType(zone, stimulusParent);
            stimulus_instance.Activate();

            SD_Log log = new SD_Log();
            log.ID = sd_event.ID;
            log.logtype = SD_Log.LogType.START;
            log.message = "Event TriggerStimulus " + log.ID + " started";
            //SD_Logger.singleton.processLog(log);
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
                    case "stimulus_type":
                        stimulusType = keys[1].Trim();
                        break;
                    case "zone":
                        zone = TaskSystem_Zone.GetZoneById(keys[1].Trim());
                        break;
                }
            }
        }
    }
}