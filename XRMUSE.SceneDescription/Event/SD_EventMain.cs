namespace XRMUSE.SceneDescription
{
    /// <summary>
    /// EventMain will be the behaviour of that event, which may produce new events, create tasks/stimuli/...
    /// An EventMain is attached to an SD_Event, it's behaviour may also be to cancel other SD_Events
    /// </summary>
    public interface SD_EventMain
    {
        public const string EVENTS_PREFABS_LOCATION = "SceneDescription/EventsPrefabs";
        public void EventStart(SD_Event sd_event);
        public void EventInterrupt(SD_Event sd_event);
        /// <summary>
        /// Parses the Event's main type specific values from the lines of the text file, stops when reaching a '#'
        /// </summary>
        /// <param name="lines">All lines of the text file</param>
        /// <param name="fromLine">line to start reading the specific values from</param>
        public void Parse(string[] lines, int fromLine);
    }
}