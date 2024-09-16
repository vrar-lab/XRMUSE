namespace XRMUSE.SceneDescription
{
    /// <summary>
    /// An SD Log is a log that "testimony" something that happened with an event that has the same ID.
    /// Thus, there might be multiple logs of the same ID
    /// </summary>
    public class SD_Log
    {
        public enum LogType { START, END, SUCCESS, FAIL, PENDING }
        /// <summary>
        /// ID it is associated to 
        /// </summary>
        public SD_EventLogID ID = new SD_EventLogID();
        /// <summary>
        /// What is the log about regarding the event of the same ID?
        /// </summary>
        public LogType logtype;
        /// <summary>
        /// optional message of the log, may be used by experimenter, is not taken into account in processing
        /// </summary>
        public string message = "";

        public SD_Log() { }
        public SD_Log(SD_EventLogID ID, LogType logType, string message = "") { this.ID = ID; this.logtype = logType; this.message = message; }

        public override bool Equals(object obj)
        {
            if (obj is SD_Log)
            {
                var o = obj as SD_Log;
                return o.ID.Equals(ID) && o.logtype == logtype;
            }
            return false;
        }
        public override int GetHashCode()
        {
            return ID.GetHashCode() + ((int)logtype) * 11;
        }
    }
}