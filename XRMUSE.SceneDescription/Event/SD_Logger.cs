using System.Collections.Generic;

namespace XRMUSE.SceneDescription
{
    /// <summary>
    /// Class with an always accessible singleton used to record produced SD_Logs
    /// </summary>
    public class SD_Logger
    {
        public static SD_Logger singleton = new SD_Logger();

        public static bool ADD_TIME_LOGS = true;

        public SD_Logger() : base()
        {
            singleton = this;
        }
        /// <summary>
        /// Logs added to the singleton that have not be processed, some events may be triggered only when the SD_Log is freshly processed and may not look at past logs
        /// </summary>
        public List<SD_Log> to_process = new List<SD_Log>();
        /// <summary>
        /// Processed logs, classified with ID for easy/fast access and search
        /// </summary>
        public Dictionary<string, Dictionary<int, List<SD_Log>>> processed = new Dictionary<string, Dictionary<int, List<SD_Log>>>();

        /// <summary>
        /// Makes sure an ID is an accessible key in the processed dict
        /// </summary>
        /// <param name="ID"></param>
        public void processID(SD_EventLogID ID)
        {
            if (!processed.ContainsKey(ID.ID_name))
                processed.Add(ID.ID_name, new Dictionary<int, List<SD_Log>>());
            if (!processed[ID.ID_name].ContainsKey(ID.ID_count))
                processed[ID.ID_name].Add(ID.ID_count, new List<SD_Log>());
        }
        /// <summary>
        /// adds a log to the "processed" dict
        /// </summary>
        /// <param name="log"></param>
        public void processLog(SD_Log log)
        {
            processID(log.ID);
            if (processed[log.ID.ID_name][log.ID.ID_count].Contains(log))
                processed[log.ID.ID_name][log.ID.ID_count].Remove(log);
            processed[log.ID.ID_name][log.ID.ID_count].Add(log);
        }

        /// <summary>
        /// Adds a log to be processed later 
        /// </summary>
        /// <param name="log"></param>
        public static void AddToProcess(SD_Log log)
        {
            if (ADD_TIME_LOGS) { log.message = "[TIME:" + TS_Timeline.time + "] " + log.message; }
            singleton.to_process.Add(log);
            if (log.logtype == SD_Log.LogType.SUCCESS || log.logtype == SD_Log.LogType.FAIL)//SUCCESS/FAIL => Also END
            {
                SD_Log log_end = new SD_Log();
                log_end.ID = log.ID;
                log_end.logtype = SD_Log.LogType.END;
                log_end.message = "Automatic END log";
                if (ADD_TIME_LOGS) { log_end.message = "[TIME:" + TS_Timeline.time + "] " + log_end.message; }
                singleton.to_process.Add(log_end);
            }
        }
    }
}