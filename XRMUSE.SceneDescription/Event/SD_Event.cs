using System.Collections.Generic;

namespace XRMUSE.SceneDescription
{
    /// <summary>
    /// An SD_Event is an event that is supposed to starts when All the start_conditions are met
    /// It has an ID and may generate logs related to that ID for the singleton of SD_Logger
    /// </summary>
    public class SD_Event
    {
        /// <summary>
        /// Identifier used in pair with logs for even management
        /// </summary>
        public SD_EventLogID ID = new SD_EventLogID();
        /// <summary>
        /// The logs that needs to be cleared before starting the event, once a log is processed => should be removed from the list, event starts when list is empty
        /// </summary>
        public List<SD_Log> start_condition = new List<SD_Log>();
        public enum State { WAITING, STARTED, CANCELLED, FINISHED }
        /// <summary>
        /// State of the event, it's either waiting to start, started, cancelled (so not properly finished) or finished
        /// </summary>
        public State currentState = State.WAITING;
        /// <summary>
        /// behaviour of the event, what it does
        /// </summary>
        public SD_EventMain behaviour;

        /// <summary>
        /// Tries to activate the event, fails if the conditions are not met
        /// </summary>
        /// <returns>If the event started (also true if started prior to the call) or not</returns>
        public bool Activate()
        {
            if ((start_condition.Count == 0) && (currentState == State.WAITING))
            {
                behaviour.EventStart(this);
                currentState = State.STARTED;
            }
            return currentState != State.WAITING;
        }
        /// <summary>
        /// Interrupts an event early and returns if it is effectively ended
        /// </summary>
        /// <param name="cancelWaiting">Should the event be cancelled/discarded even if it was not started?</param>
        /// <returns>Is the event finished/cancelled (and thus, can be discarded?)</returns>
        public bool Interrupt(bool cancelWaiting = false)
        {
            if (currentState == State.FINISHED || currentState == State.CANCELLED)
                return true;
            if (currentState != State.WAITING || cancelWaiting)
            {
                behaviour.EventInterrupt(this);
            }
            return currentState == State.CANCELLED || currentState == State.FINISHED;
        }

        /// <summary>
        /// Removes a condition from the SD_Event, once all conditions are removed the event manager will start it
        /// </summary>
        /// <param name="sdlog"></param>
        public void clearCondition(SD_Log sdlog)
        {
            if (start_condition.Contains(sdlog))
                start_condition.Remove(sdlog);
        }
    }
}