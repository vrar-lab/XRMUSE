using System.Threading;

namespace XRMUSE.SceneDescription
{
    /// <summary>
    /// Event that simply generates logs after a timer
    /// </summary>
    [SD_EventAttribute("timer")]
    public class SD_EventMain_Timer : SD_EventMain
    {
        /// <summary>
        /// time in seconds before the log is produced once the event started
        /// </summary>
        public float time;
        private SD_Event local_event;
        public SD_EventMain_Timer() { }
        public SD_EventMain_Timer(float time) { this.time = time; }
        public void EventInterrupt(SD_Event sd_event)
        {
            sd_event.currentState = SD_Event.State.CANCELLED;

            SD_Log log = new SD_Log();
            log.ID = sd_event.ID;
            log.logtype = SD_Log.LogType.FAIL;
            log.message = "Event Timer " + log.ID + " interrupted";
            //SD_Logger.singleton.processLog(log);
            SD_Logger.AddToProcess(log);

            if (_t != null)
                _t.Abort();
        }

        Thread _t = null;
        public void EventStart(SD_Event sd_event)
        {
            local_event = sd_event;

            SD_Log log = new SD_Log();
            log.ID = sd_event.ID;
            log.logtype = SD_Log.LogType.START;
            log.message = "Event TriggerStimulus " + log.ID + " started";
            SD_Logger.AddToProcess(log);

            _t = new Thread(new ThreadStart(DoTimer));
            _t.Start();
        }

        public void DoTimer()
        {
            Thread.Sleep((int)(time * 1000f));
            if (local_event.currentState == SD_Event.State.CANCELLED)
                return;
            local_event.currentState = SD_Event.State.FINISHED;
            SD_Log log = new SD_Log();
            log.ID = local_event.ID;
            log.logtype = SD_Log.LogType.SUCCESS;
            log.message = "Event Timer " + log.ID + " success";
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
                    case "time":
                        time = float.Parse(keys[1].Trim());
                        break;
                }
            }
        }
    }
}