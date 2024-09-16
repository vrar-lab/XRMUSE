using UnityEngine;
using XRMUSE.SceneDescription;
using System.Threading;
using XRMUSE.ExampleScene;
using XRMUSE.Networking;

/// <summary>
/// Event that ask the user to produce a minimum amount of a specific material in the example scenario
/// </summary>
[SD_EventAttribute("produceobjective")]
public class SD_EventMain_ProduceObjective : SD_EventMain
{
    public void EventInterrupt(SD_Event sd_event)
    {
        sd_event.currentState = SD_Event.State.CANCELLED;

        SD_Log log = new SD_Log();
        log.ID = sd_event.ID;
        log.logtype = SD_Log.LogType.FAIL;
        log.message = "Event Produce Objective " + log.ID + " interrupted";
        SD_Logger.AddToProcess(log);

        keep_udpate = false;
        if (_t != null)
            _t.Abort();
    }

    private SD_Event local_event;
    Thread _t = null;
    public void EventStart(SD_Event sd_event)
    {
        local_event = sd_event;
        SD_Log log = new SD_Log();
        log.ID = sd_event.ID;
        log.logtype = SD_Log.LogType.START;
        log.message = "Event Produce Objective " + log.ID + " started";
        SD_Logger.AddToProcess(log);

        keep_udpate = true;
        _t = new Thread(new ThreadStart(SlowUpdate));//Using slowupdate pattern to avoid overheading too much on Unity's updates
        _t.Start();
    }

    bool keep_udpate = true;
    /// <summary>
    /// Slow update pattern, works same as Unity's update but do it less often to reduce compute time
    /// Could be replaced with cleaner listeners, but sufficiently efficient for our example scenario
    /// </summary>
    public void SlowUpdate()
    {
        while (keep_udpate)
        {
            Thread.Sleep(SLOWUPDATERATE);
            DoUpdate();
        }
    }

    int have_produced = 0;
    /// <summary>
    /// Behaviour within the SlowUpdate, looks at the MaterialConsume in PSVs to see if the targeted items are being produced and update the displays
    /// </summary>
    public void DoUpdate()
    {
        have_produced = 0;
        foreach (var psv in PlayerSyncedValues.allInstances)
        {
            have_produced += (int) psv.GetFunction("MaterialConsume", 1)(new object[] { mat_type });
        }
        SessionManager.Instance.UpdateObjective(mat_type, have_produced, count); 
        if( have_produced >= count) {
            keep_udpate = false;

            SD_Log log = new SD_Log();
            log.ID = local_event.ID;
            log.logtype = SD_Log.LogType.SUCCESS;
            log.message = "Event Produce Objective " + log.ID + " SUCCESS";
            SD_Logger.AddToProcess(log);
        }
    }

    public int mat_type = -1;
    int count = -1;
    const int SLOWUPDATERATE = 1200;
    public void Parse(string[] lines, int fromLine)
    {
        for (int i = fromLine; i < lines.Length; i++)
        {
            if (lines[i][0] == '#')
                break;
            var keys = lines[i].Split(':');
            switch (keys[0].ToLower().Trim())
            {
                case "type":
                case "toproduce":
                case "to_produce":
                case "material":
                    mat_type = int.Parse(keys[1].Trim());
                    break;
                case "amount":
                case "count":
                    count = int.Parse(keys[1].Trim());
                    break;
            }
        }
        SessionManager.Instance.initialObjectives.Add(new int[] {mat_type, count});
    }
}
