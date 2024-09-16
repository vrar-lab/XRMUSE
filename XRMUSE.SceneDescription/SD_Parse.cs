using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Unity.VisualScripting;
using UnityEngine;

namespace XRMUSE.SceneDescription
{
    /// <summary>
    /// Class with static methods to parse user-defined scene elements from text files.
    /// </summary>
    public class SD_Parse
    {
        /// <summary>
        /// Key: (lowered) text used by the user in the description file
        /// Value: MainType of a plant
        /// static variable used as a pseudo-constant Dictionnary
        /// </summary>
        public static Dictionary<string, Type> instantiableKeys = initInstantiableKeys();
        /// <summary>
        /// Builds the instantiableKeys dictionnary, new instantiables types should be added there
        /// </summary>
        /// <returns>default instantiableKeys dictionnary</returns>
        public static Dictionary<string, Type> initInstantiableKeys()
        {
            Dictionary<string, Type> ret = new Dictionary<string, Type>();
            /*ret.Add("plantwater", typeof(SD_PlantWater));
            ret.Add("material", typeof(SD_InstantiableMain_Material));
            ret.Add("materialbox", typeof(SD_InstantiableMain_MaterialBox));*/

            var types = Assembly.GetExecutingAssembly().GetTypes().Where(t => t.IsDefined(typeof(SD_InstantiableAttribute)));
            foreach (var type in types)
            {
                ret.Add(type.GetAttribute<SD_InstantiableAttribute>().ParseName, type);
            }
            return ret;
        }
        /// <summary>
        /// Key: (lowered) text used by the user in the description file
        /// Value: SubType of an instantiable
        /// static variable used as a pseudo-constant Dictionnary
        /// </summary>
        public static Dictionary<string, Type> instantiableSubKeys = initInstantiableSubKeys();
        /// <summary>
        /// Builds the instantiableSubKeys dictionnary, new instantiable subtypes should be added there
        /// </summary>
        /// <returns>default instantiableSubKeys dictionnary</returns>
        public static Dictionary<string, Type> initInstantiableSubKeys()
        {
            Dictionary<string, Type> ret = new Dictionary<string, Type>();
            var types = Assembly.GetExecutingAssembly().GetTypes().Where(t => t.IsDefined(typeof(SD_InstantiableSubAttribute)));
            foreach (var type in types)
            {
                ret.Add(type.GetAttribute<SD_InstantiableSubAttribute>().ParseName, type);
            }
            return ret;
        }
        /// <summary>
        /// Parse a user-defined instantiable text file from its lines.
        /// </summary>
        /// <param name="lines">All the lines of a plant instantiable text file</param>
        /// <returns>the parsed instantiable type</returns>
        public static SD_InstantiableType ParseInstantiable(string[] lines)
        {
            SD_InstantiableType sdpt = new SD_InstantiableType();
            SD_InstantiableMain sdpm = null;
            List<SD_InstantiableSubmodule> sdpss = new List<SD_InstantiableSubmodule>();
            bool reached_hash = false;
            for (int i = 0; i < lines.Length; i++)
            {
                if (lines[i][0] == '#')
                {
                    reached_hash = true;
                    var keys = lines[i].Split(':');
                    switch (keys[0].ToLower().Trim())
                    {
                        case "#main":
                            if (instantiableKeys.ContainsKey(keys[1].ToLower().Trim()))
                                sdpm = (SD_InstantiableMain)Activator.CreateInstance(instantiableKeys[keys[1].ToLower().Trim()]);
                            if (sdpm != null)
                                sdpm.Parse(lines, i + 1);
                            break;
                        case "#sub":
                            SD_InstantiableSubmodule tmp = null;
                            if (instantiableSubKeys.ContainsKey(keys[1].ToLower().Trim()))
                                tmp = (SD_InstantiableSubmodule)Activator.CreateInstance(instantiableSubKeys[keys[1].ToLower().Trim()]);
                            if (tmp != null)
                            {
                                tmp.Parse(lines, i + 1);
                                sdpss.Add(tmp);
                            }
                            break;
                    }
                }
                if (!reached_hash)
                {
                    var keys = lines[i].Split(':');
                    if (keys[0].ToLower().Equals("id"))
                        sdpt.ID = keys[1];
                }
            }
            sdpt.mainType = sdpm;
            sdpt.submodules = sdpss.ToArray();
            return sdpt;
        }

        /// <summary>
        /// Key: (lowered) text used by the user in the description file
        /// Value: MainType of an Event
        /// static variable used as a pseudo-constant Dictionnary
        /// </summary>
        public static Dictionary<string, Type> eventKeys = initEventKeys();
        /// <summary>
        /// Builds the eventKeys dictionnary, new event types should be added there
        /// </summary>
        /// <returns>default eventKeys dictionnary</returns>
        public static Dictionary<string, Type> initEventKeys()
        {
            Dictionary<string, Type> ret = new Dictionary<string, Type>();
            var types = Assembly.GetExecutingAssembly().GetTypes().Where(t => t.IsDefined(typeof(SD_EventAttribute)));
            foreach (var type in types)
            {
                ret.Add(type.GetAttribute<SD_EventAttribute>().ParseName, type);
            }
            return ret;
        }
        /// <summary>
        /// Parse a user-defined event text file from its lines.
        /// </summary>
        /// <param name="lines">All the lines of an event description text file</param>
        /// <returns>the parsed event type</returns>
        public static SD_Event ParseEvent(string[] lines)
        {
            SD_Event sd_event = new SD_Event();
            SD_EventMain sd_eventMain = null;
            bool reached_hash = false;
            for (int i = 0; i < lines.Length; i++)
            {
                if (lines[i][0] == '#')
                {
                    reached_hash = true;
                    var keys = lines[i].Split(':');
                    switch (keys[0].ToLower().Trim())
                    {
                        case "#main":
                            if (eventKeys.ContainsKey(keys[1].ToLower().Trim()))
                                sd_eventMain = (SD_EventMain)Activator.CreateInstance(eventKeys[keys[1].ToLower().Trim()]);
                            if (sd_eventMain != null)
                                sd_eventMain.Parse(lines, i + 1);
                            break;
                    }
                }
                if (!reached_hash)
                {
                    var keys = lines[i].Split(':');
                    if (keys[0].ToLower().Equals("id"))
                    {
                        var keys2 = keys[1].Split(';');
                        sd_event.ID.ID_name = keys2[0];
                        if (!int.TryParse(keys2[1], out sd_event.ID.ID_count))
                            sd_event.ID.ID_count = -1;
                    }
                    if (keys[0].ToLower().Equals("startcond"))
                    {
                        for (int j = 1; j < keys.Length; j++)
                        {
                            var keys2 = keys[j].Split(';');
                            SD_EventLogID cond_id = new SD_EventLogID();
                            cond_id.ID_name = keys2[0];
                            if (!int.TryParse(keys2[1], out cond_id.ID_count))
                                cond_id.ID_count = -1;
                            SD_Log log = new SD_Log();
                            log.ID = cond_id;
                            switch (keys2[2].ToLower())
                            {
                                case "start":
                                    log.logtype = SD_Log.LogType.START; break;
                                case "end":
                                    log.logtype = SD_Log.LogType.END; break;
                                case "waiting":
                                case "pending":
                                    log.logtype = SD_Log.LogType.PENDING; break;
                                case "success":
                                    log.logtype = SD_Log.LogType.SUCCESS; break;
                                case "fail":
                                    log.logtype = SD_Log.LogType.FAIL; break;
                            }
                            sd_event.start_condition.Add(log);
                        }
                    }

                }
            }
            sd_event.behaviour = sd_eventMain;
            return sd_event;
        }
        /// <summary>
        /// Key: (lowered) text used by the user in the description file
        /// Value: MainType of a Stilulus
        /// static variable used as a pseudo-constant Dictionnary
        /// </summary>
        public static Dictionary<string, Type> stimuliKeys = initStimuliKeys();
        /// <summary>
        /// Builds the stimuliKeys dictionnary, new stimuli types should be added there
        /// </summary>
        /// <returns>default stimuliKeys dictionnary</returns>
        public static Dictionary<string, Type> initStimuliKeys()
        {
            Dictionary<string, Type> ret = new Dictionary<string, Type>();
            //ret.Add("cpcstimulusexample", typeof(SD_StimuliMain_ExampleCPC));
            var types = Assembly.GetExecutingAssembly().GetTypes().Where(t => t.IsDefined(typeof(SD_StimulusAttribute)));
            foreach (var type in types)
            {
                ret.Add(type.GetAttribute<SD_StimulusAttribute>().ParseName, type);
            }
            return ret;
        }
        /// <summary>
        /// Parse a user-defined stimulus text file from its lines.
        /// </summary>
        /// <param name="lines">All the lines of an stimulus description text file</param>
        /// <returns>the parsed stimulus type</returns>
        public static SD_StimulusType ParseStimuli(string[] lines)
        {
            SD_StimulusType sd_stimulusType = new SD_StimulusType();
            SD_StimulusMain sd_stimulusMain = null;
            bool reached_hash = false;
            for (int i = 0; i < lines.Length; i++)
            {
                if (lines[i][0] == '#')
                {
                    reached_hash = true;
                    var keys = lines[i].Split(':');
                    switch (keys[0].ToLower().Trim())
                    {
                        case "#main":
                            if (stimuliKeys.ContainsKey(keys[1].ToLower().Trim()))
                                sd_stimulusMain = (SD_StimulusMain)Activator.CreateInstance(stimuliKeys[keys[1].ToLower().Trim()]);
                            if (sd_stimulusMain != null)
                                sd_stimulusMain.Parse(lines, i + 1);
                            break;
                    }
                }
                if (!reached_hash)
                {
                    var keys = lines[i].Split(':');
                    if (keys[0].ToLower().Equals("id"))
                    {
                        sd_stimulusType.ID = keys[1];
                    }
                }
            }
            sd_stimulusType.mainType = sd_stimulusMain;
            return sd_stimulusType;
        }
    }
}