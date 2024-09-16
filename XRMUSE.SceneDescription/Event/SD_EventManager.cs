using System;
using System.Collections.Generic;

namespace XRMUSE.SceneDescription
{
    /// <summary>
    /// Class with an always accessible singleton used to manage produced SD_Events
    /// </summary>
    public class SD_EventManager
    {
        public static SD_EventManager singleton = new SD_EventManager();
        /// <summary>
        /// All waiting events indexed by their (partial) ID. May have multiple events per ID as the list of object is not used for referencing.
        /// List of event of an SD_EventLogID id is accessed with waiting[id.ID_name][id.ID_count]
        /// </summary>
        public Dictionary<string, Dictionary<int, List<SD_Event>>> waiting = new Dictionary<string, Dictionary<int, List<SD_Event>>>();
        /// <summary>
        /// All started events indexed by their (partial) ID. May have multiple events per ID as the list of object is not used for referencing.
        /// List of event of an SD_EventLogID id is accessed with started[id.ID_name][id.ID_count]
        /// </summary>
        public Dictionary<string, Dictionary<int, List<SD_Event>>> started = new Dictionary<string, Dictionary<int, List<SD_Event>>>();

        /// <summary>
        /// Clears the singleton logger from all its "to_process" Logs. Each processed log are removed from the waiting events.
        /// </summary>
        public void clearLogger()
        {
            foreach (var log in SD_Logger.singleton.to_process)
            {
                SD_Logger.singleton.processLog(log);
                foreach (var value in waiting)
                {
                    foreach (var value2 in value.Value)
                    {
                        foreach (var sd_event in value2.Value)
                        {
                            sd_event.clearCondition(log);
                        }
                    }
                }
            }
            SD_Logger.singleton.to_process.Clear();
        }

        private List<SD_Event> sd_eventsTMP = new List<SD_Event>();//Just small optimization to reduce gc calls
        /// <summary>
        /// Activates all the waiting events that are ready to start and transfers those to the started dict.
        /// </summary>
        public void clearWaiting()
        {
            sd_eventsTMP.Clear();
            foreach (var value in waiting)
                foreach (var value2 in value.Value)
                    foreach (var sd_event in value2.Value)
                        if (sd_event.Activate())
                            sd_eventsTMP.Add(sd_event);
            foreach (var e in sd_eventsTMP)
            {
                addStarted(e);
                removeWaiting(e);
            }
            sd_eventsTMP.Clear();
        }
        /// <summary>
        /// Adds an SD_Event e to the Dictionnary d indexed by it's SD_EventLogID
        /// </summary>
        /// <param name="e"></param>
        /// <param name="d"></param>
        private void _add(SD_Event e, Dictionary<string, Dictionary<int, List<SD_Event>>> d)
        {
            if (!d.ContainsKey(e.ID.ID_name))
                d.Add(e.ID.ID_name, new Dictionary<int, List<SD_Event>>());
            if (!d[e.ID.ID_name].ContainsKey(e.ID.ID_count))
                d[e.ID.ID_name].Add(e.ID.ID_count, new List<SD_Event>());
            d[e.ID.ID_name][e.ID.ID_count].Add(e);
        }
        /// <summary>
        /// Removes an SD_Event e from the Dictionnary d indexed by it's SD_EventLogID
        /// </summary>
        /// <param name="e"></param>
        /// <param name="d"></param>
        private bool _remove(SD_Event e, Dictionary<string, Dictionary<int, List<SD_Event>>> d)
        {
            if (!d.ContainsKey(e.ID.ID_name))
                return false;
            if (!d[e.ID.ID_name].ContainsKey(e.ID.ID_count))
                return false;
            if (!d[e.ID.ID_name][e.ID.ID_count].Contains(e))
                return false;
            d[e.ID.ID_name][e.ID.ID_count].Remove(e);
            return true;
        }

        /// <summary>
        /// Gets an SD_Event e from the Dictionary d from its eventlogID
        /// </summary>
        /// <param name="id">Id to retrieve the event from</param>
        /// <param name="d">the dictionnary to search from</param>
        /// <returns></returns>
        public SD_Event _get(SD_EventLogID id, Dictionary<string, Dictionary<int, List<SD_Event>>> d)
        {
            try
            {
                var tmp = d[id.ID_name][id.ID_count];
                foreach (var tmp2 in tmp)
                {
                    if (tmp2.ID.Equals(id))
                        return tmp2;
                }
            }
            catch (Exception) { }
            return null;
        }
        public SD_Event getWaiting(SD_EventLogID id)
        {
            return _get(id, waiting);
        }
        public SD_Event getStarted(SD_EventLogID id)
        {
            return _get(id, started);
        }
        /// <summary>
        /// Adds an event e to the list of waiting events
        /// </summary>
        /// <param name="e"></param>
        public void addWaiting(SD_Event e)
        {
            _add(e, waiting);
        }
        /// <summary>
        /// Removes an event e to the list of waiting events
        /// </summary>
        /// <param name="e"></param>
        public void removeWaiting(SD_Event e)
        {
            _remove(e, waiting);
        }
        /// <summary>
        /// Adds an event e to the list of started events
        /// </summary>
        /// <param name="e"></param>
        public void addStarted(SD_Event e)
        {
            _add(e, started);
        }
        /// <summary>
        /// Removes an event e to the list of started events
        /// </summary>
        /// <param name="e"></param>
        public void removeStarted(SD_Event e)
        {
            _remove(e, started);
        }
        /// <summary>
        /// Completely wipes the event manager and logger singletons
        /// </summary>
        public static void Reset()
        {
            singleton.waiting.Clear();
            singleton.started.Clear();
            SD_Logger.singleton.processed.Clear();
            SD_Logger.singleton.to_process.Clear();
        }

        /// <summary>
        /// Clears the logger and starts the resulting ready events
        /// </summary>
        public static void Process()
        {
            singleton.clearLogger();
            singleton.clearWaiting();
        }
    }
}