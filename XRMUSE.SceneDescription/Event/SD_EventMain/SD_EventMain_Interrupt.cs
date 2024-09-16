using System;

namespace XRMUSE.SceneDescription
{
    [SD_EventAttribute("interrupt")]
    public class SD_EventMain_Interrupt : SD_EventMain
    {
        private SD_Event local_event;
        private SD_EventLogID toCancel = new SD_EventLogID();

        bool isDone = false;
        bool cancelWaiting = false;
        public void EventInterrupt(SD_Event sd_event)
        {
            if (isDone)
                return;

            local_event = sd_event;
            isDone = true;

            //log
            SD_Log log = new SD_Log();
            log.ID = sd_event.ID;
            log.logtype = SD_Log.LogType.FAIL;
            log.message = "Event Interrupt " + log.ID + " cancelled";
            SD_Logger.AddToProcess(log);
        }

        public void EventStart(SD_Event sd_event)
        {
            local_event = sd_event;

            if (isDone)
                return;
            isDone = true;

            //log
            SD_Log log = new SD_Log();
            log.ID = sd_event.ID;
            log.logtype = SD_Log.LogType.END;
            log.message = "Event Interrupt " + log.ID + " done";
            SD_Logger.AddToProcess(log);

            //interrupt the event
            try
            {
                SD_EventManager.singleton.getStarted(toCancel).Interrupt(cancelWaiting);
            }
            catch (Exception) { }
            try
            {
                SD_EventManager.singleton.getWaiting(toCancel).Interrupt(cancelWaiting);
            }
            catch (Exception) { }
        }

        public void Parse(string[] lines, int fromLine)
        {
            for (int i = fromLine; i < lines.Length; i++)
            {
                if (lines[i][0] == '#')
                    break;
                var keys = lines[i].Split(':');
                if (keys[0].ToLower().Equals("tocancel_id"))
                {
                    var keys2 = keys[1].Split(';');
                    toCancel.ID_name = keys2[0];
                    if (!int.TryParse(keys2[1], out toCancel.ID_count))
                        toCancel.ID_count = -1;
                }
                if (keys[0].ToLower().Equals("cancelwaiting"))
                {
                    bool.TryParse(keys[1], out cancelWaiting);
                }
            }
        }

    }
}