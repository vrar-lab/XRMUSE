using System.Collections;
using System.Collections.Generic;
using XRMUSE.ExampleScene;
using XRMUSE.SceneDescription;
using UnityEngine;

namespace XRMUSE.ExampleScene
{
    [SD_Event("setrecipes")]
    public class SD_Event_SetRecipes : SD_EventMain
    {
        public void EventInterrupt(SD_Event sd_event)
        {
            if (isEnded)
                return;
            isEnded = true;
            sd_event.currentState = SD_Event.State.CANCELLED;
            SD_Log log = new SD_Log();
            log.ID = sd_event.ID;
            log.logtype = SD_Log.LogType.END;
            log.message = "Event SRecipes " + log.ID + " ended";
            SD_Logger.AddToProcess(log);
        }

        public void EventStart(SD_Event sd_event)
        {
            if (isEnded || isStarted)
                return;
            isStarted = true;

            ES_Recipes.instance.recipesMat1 = mat1.ToArray();
            ES_Recipes.instance.recipesMat2 = mat2.ToArray();
            ES_Recipes.instance.recipesTool = tool.ToArray();
            ES_Recipes.instance.recipesProductionsSD = results.ToArray();
            ES_Recipes.instance.useSD = true;
            ES_Recipes.instance.Load();

            SD_Log log = new SD_Log();
            log.ID = sd_event.ID;
            log.logtype = SD_Log.LogType.START;
            log.message = "Event SetRecipes " + log.ID + " started";
            SD_Logger.AddToProcess(log);
        }

        bool isEnded = false;
        bool isStarted = false;
        List<int> mat1 = new List<int>(), mat2 = new List<int>(), tool = new List<int>();
        List<string> results = new List<string>();
        public void Parse(string[] lines, int fromLine)
        {
            for (int i = fromLine; i < lines.Length; i++)
            {
                if (lines[i][0] == '#')
                    break;
                var keys = lines[i].Split(':');
                switch (keys[0].ToLower().Trim())
                {
                    default:
                            mat1.Add(int.Parse(keys[0].Trim()));
                            mat2.Add(int.Parse(keys[1].Trim()));
                            tool.Add(int.Parse(keys[2].Trim()));
                            results.Add(keys[3].Trim());
                        break;
                }
            }
        }
    }
}