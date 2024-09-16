using System.IO;
using XRMUSE.ExampleScene;
using UnityEngine;

namespace XRMUSE.SceneDescription
{
    /// <summary>
    /// Global settings of the industrial scenario which shall be loaded from a text file.
    /// Can set up stuff such as player number, ip to connect, ...
    /// </summary>
    public class SD_Settings : MonoBehaviour
    {
        public static int playerNum = 1;

        public int _playerNum = 1;
        void Awake()
        {
            //TODO: load settings from a .txt file + change read paths for Task System
            loadGeneralParams(force_not_read);

            if(force_not_read)
                playerNum = _playerNum;
            gameObject.SetActive(false);
        }
        public bool force_not_read = true;
        const string paramsFileName = "/../params.txt";
        static bool already_read = false;
        public static string pathInstantiables, pathStimuli, pathEvents;

        public static void loadGeneralParams(bool forcenotread = false)
        {
            loadGeneralParams(Application.dataPath + paramsFileName);
        }
        public static void loadGeneralParams(string path, bool forcenotread=false)
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            path = Application.persistentDataPath + path;
#endif
            if (already_read||forcenotread)
                return;
            already_read = true;
            if (!File.Exists(path))
            {
                Debug.LogError("Missing params file at: " + path);
                return;
            }
            string[] lines = File.ReadAllLines(path);
            foreach (string line in lines)
            {
                string[] ls = line.Split(':');
                if (ls.Length < 2)
                    continue;
                switch (ls[0].ToLower())
                {
                    case "path_instantiables":
                        pathInstantiables = ls[1];
                        break;
                    case "path_stimuli":
                        pathStimuli = ls[1];
                        break;
                    case "path_events":
                        pathEvents = ls[1];
                        break;
                    case "player_num":
                    case "playernum":
                    case "player":
                        playerNum = int.Parse(ls[1]);
                        break;
                }
            }
            SessionManager.IsPlayerIdLoaded = true;
#if UNITY_ANDROID && !UNITY_EDITOR
            pathInstantiables = Application.persistentDataPath + pathInstantiables;
            pathStimuli = Application.persistentDataPath + pathStimuli;
            pathEvents = Application.persistentDataPath + pathEvents;
#endif
        }
    }
}