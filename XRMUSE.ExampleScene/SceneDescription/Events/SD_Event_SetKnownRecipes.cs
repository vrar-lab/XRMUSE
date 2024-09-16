using System.Collections.Generic;
using UnityEngine;
using XRMUSE.SceneDescription;

namespace XRMUSE.ExampleScene
{
    [SD_Event("setknownrecipes")]
    public class SD_Event_SetKnownRecipes : SD_EventMain
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
            log.message = "Event SetKnownRecipes " + log.ID + " ended";
            SD_Logger.AddToProcess(log);
        }

        public void EventStart(SD_Event sd_event)
        {
            if (isEnded || isStarted)
                return;
            isStarted = true;
            
            GameObject screengo = GameObject.Find("ScreenObjectives");
            if (screengo == null)
                screengo = GameObject.FindObjectOfType<ScreenObjectives>(true).gameObject;
            ScreenObjectives screenObjectives = screengo.GetComponent<ScreenObjectives>();
            if (!playersDiffers || SD_Settings.playerNum == 1)
            {
                screenObjectives.recipesMat1 = mat1P1.ToArray();
                screenObjectives.recipesMat2 = mat2P1.ToArray();
                screenObjectives.recipesTool = toolP1.ToArray();
                screenObjectives.recipesResult = resultsP1.ToArray();
            }
            else
            {
                screenObjectives.recipesMat1 = mat1P2.ToArray();
                screenObjectives.recipesMat2 = mat2P2.ToArray();
                screenObjectives.recipesTool = toolP2.ToArray();
                screenObjectives.recipesResult = resultsP2.ToArray();
            }

            SD_Log log = new SD_Log();
            log.ID = sd_event.ID;
            log.logtype = SD_Log.LogType.START;
            log.message = "Event SetKnownRecipes " + log.ID + " started";
            SD_Logger.AddToProcess(log);
        }

        bool isEnded = false;
        bool isStarted = false;
        bool playersDiffers = false;
        List<int> mat1P1 = new List<int>(), mat2P1 = new List<int>(), toolP1 = new List<int>(), resultsP1 = new List<int>();
        List<int> mat1P2 = new List<int>(), mat2P2 = new List<int>(), toolP2 = new List<int>(), resultsP2 = new List<int>();
        public void Parse(string[] lines, int fromLine)
        {
            for (int i = fromLine; i < lines.Length; i++)
            {
                if (lines[i][0] == '#')
                    break;
                var keys = lines[i].Split(':');
                switch (keys[0].ToLower().Trim())
                {
                    case "p2":
                    case "p2_recipes":
                    case "p2recipes":
                        playersDiffers = true;
                        break;
                    default:
                        if (!playersDiffers)
                        {
                            mat1P1.Add(int.Parse(keys[0].Trim()));
                            mat2P1.Add(int.Parse(keys[1].Trim()));
                            toolP1.Add(int.Parse(keys[2].Trim()));
                            resultsP1.Add(int.Parse(keys[3].Trim()));
                        }
                        else
                        {
                            mat1P2.Add(int.Parse(keys[0].Trim()));
                            mat2P2.Add(int.Parse(keys[1].Trim()));
                            toolP2.Add(int.Parse(keys[2].Trim()));
                            resultsP2.Add(int.Parse(keys[3].Trim()));
                        }
                        break;
                }
            }
        }
    }
}