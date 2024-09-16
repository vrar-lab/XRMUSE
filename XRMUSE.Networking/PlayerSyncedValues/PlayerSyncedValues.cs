using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System.Reflection;
using Unity.VisualScripting;

namespace XRMUSE.Networking
{
    /// <summary>
    /// A PlayerSyncedValues (PSV) is a Player-stored and shared values for a player, each player should have their own instance of the object that they will share over the networking.
    /// Values are not directly defined within this class, used either in conjunction with IPlayerSyncedValues_DataRegister to add data or by manually calling the AddData function
    /// Built upon IPunObservable and thus Photon PUN, porting to other networking solutions requires to change OnPhotonSerializeView, OnPhotonSerializeViewReading, RewriteAll, WriteTemporaryONLY and OnPlayerEnteredRoom to use dataStreams and callbacks of the target network solution
    /// </summary>
    [RequireComponent(typeof(PhotonView))]
    public class PlayerSyncedValues : MonoBehaviourPunCallbacks, IPunObservable
    {

        static PlayerSyncedValues _localInstance = null;
        /// <summary>
        /// PSV associated with the local player, typically accessed to update data
        /// </summary>
        public static PlayerSyncedValues localInstance { get { GENERATE_LOCAL_INSTANCE(); return _localInstance; } }
        /// <summary>
        /// All known PSVs, typically used to evaluate aggregated data from all clients (including local)
        /// </summary>
        public static List<PlayerSyncedValues> allInstances = new List<PlayerSyncedValues>();

        /// <summary>
        /// Used by pooling system
        /// </summary>
        public static GameObject prefab = null;

        //Following Dictionaries are internals of the PSV, access of functions to write/read data through a data name
        public Dictionary<string, Action<PhotonStream, PhotonMessageInfo>> temporaryWrite = new Dictionary<string, Action<PhotonStream, PhotonMessageInfo>>();
        public Dictionary<string, Action<PhotonStream, PhotonMessageInfo>> rewriteAll = new Dictionary<string, Action<PhotonStream, PhotonMessageInfo>>();
        public Dictionary<string, Action<PhotonStream, PhotonMessageInfo>> read = new Dictionary<string, Action<PhotonStream, PhotonMessageInfo>>();
        public Dictionary<string, Action> reset = new Dictionary<string, Action>();
        public Dictionary<string, Func<bool>> checkTemporaryWrite = new Dictionary<string, Func<bool>>();
        public Dictionary<string, Func<object[], object>[]> customFuncs = new Dictionary<string, Func<object[], object>[]>();

        public Dictionary<int, string> id_data = new Dictionary<int, string>();
        public Dictionary<string, int> data_id = new Dictionary<string, int>();
        int _id = 0;

        /// <summary>
        /// Adds data to the PSV, requires specific function/Actions
        /// </summary>
        /// <param name="dataName">Internal name of the data within the PSV to access its functions</param>
        /// <param name="temporaryWriteFunction">Sends the changes on the register data since last synchronization</param>
        /// <param name="rewriteAllFunction">Sends all the values of the registered data regardless of what have been previously sent. Primarly used when clients connects or when a new object is created</param>
        /// <param name="readFunction">Reads and update the values from a networked PSV. Should be read in the same order as RewriteAll and TemporaryWrite</param>
        /// <param name="temporaryWriteCheck">Verify if there's any new data ready to send over the network</param>
        /// <param name="reset">Resets all values of this registered data to its default state</param>
        /// <param name="customFunctions">Methods associated with the data that can be accessed by other objects through the dataName</param>
        public void AddData(string dataName, Action<PhotonStream, PhotonMessageInfo> temporaryWriteFunction, Action<PhotonStream, PhotonMessageInfo> rewriteAllFunction, Action<PhotonStream, PhotonMessageInfo> readFunction, Func<bool> temporaryWriteCheck, Action reset, Func<object[], object>[] customFunctions)
        {
            _id++;
            temporaryWrite.Add(dataName, temporaryWriteFunction);
            rewriteAll.Add(dataName, rewriteAllFunction);
            read.Add(dataName, readFunction);
            customFuncs.Add(dataName, customFunctions);
            checkTemporaryWrite.Add(dataName, temporaryWriteCheck);
            this.reset.Add(dataName, reset);
            id_data.Add(_id, dataName);
            data_id.Add(dataName, _id);
        }

        /// <summary>
        /// Gets all available internal functions of a custom data from its dataName
        /// </summary>
        public Func<object[], object>[] GetFunctionsOf(string name)
        {
            return customFuncs[name];
        }

        /// <summary>
        /// Gets an available internal function of a custom data from its dataName and its internal (known) id
        /// </summary>
        public Func<object[], object> GetFunction(string name, int id)
        {
            return customFuncs[name][id];
        }

        //On awake the PSV adds to itself all the attached IPlayerSyncedValues_DataRegister and its functions
        void Awake()
        {
            allInstances.Add(this);

            //Register all attached IPlayerSyncedValues_DataRegister
            foreach (IPlayerSyncedValues_DataRegister dataRegister in GetComponents<IPlayerSyncedValues_DataRegister>())
            {
                //Getting custom functions through reflection
                MethodInfo[] methods = dataRegister.GetType().GetMethods();
                List<(MethodInfo,int)> list = new List<(MethodInfo, int)>();
                foreach(var method in methods)
                {
                    if (method.HasAttribute<DataRegister_CustomFunction>())
                    {
                        list.Add((method,method.GetAttribute<DataRegister_CustomFunction>().FunctionID));
                    }
                }
                Func<object[], object>[] funcs = new Func<object[], object>[list.Count];
                foreach(var func in list)
                {
                    funcs[func.Item2] = (args) => func.Item1.Invoke(dataRegister, new object[]{ args});
                }

                //Adding the custom type to the internal dictionaries
                AddData(dataRegister.DataName, dataRegister.TemporaryWrite, dataRegister.RewriteAll, dataRegister.Read, dataRegister.TemporaryWriteCheck, dataRegister.ResetValues, funcs);
            }
        }

        public static void GENERATE_LOCAL_INSTANCE()
        {
            if (_localInstance != null)
                return;
            if (prefab == null)
            {
                var pm = PhotonNetwork.PrefabPool as PoolManager;
                foreach (var spawnable in pm.spawnablePrefabs)
                {
                    if (spawnable.GetComponent<PlayerSyncedValues>() != null)
                    {
                        prefab = spawnable;
                        break;
                    }
                }
            }
            if (prefab == null)
            {
                Debug.LogError("PLAYER_SYNCED_VALUES PREFAB NOT AVAILABLE, PLEASE CHECK IT IS ADDED TO PREFAB POOL");
                return;
            }
            var gm = PoolManager.SpawnPrefab(prefab, Vector3.zero, Quaternion.identity);
            _localInstance = gm.GetComponent<PlayerSyncedValues>();
        }

        /// <summary>
        /// IPunObservable callback, if owning the object will write the new data, if not will read the data from network
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="info"></param>
        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            if (stream.IsReading)
                OnPhotonSerializeViewReading(stream, info);
            else
                OnPhotonSerializeViewWriting(stream, info);
        }

        /// <summary>
        /// Reads the data from all internal registered dataTypes
        /// </summary>
        public void OnPhotonSerializeViewReading(PhotonStream stream, PhotonMessageInfo info)
        {
            try
            {
                while (true)
                {
                    int dataType = (int)stream.ReceiveNext(); //=>can probably serialize better but eh...
                    read[id_data[dataType]](stream, info);
                }
            }
            catch (System.IndexOutOfRangeException) { }//finished reading
        }

        /// <summary>
        /// Writes the data of all internal registered dataTypes
        /// </summary>
        public void OnPhotonSerializeViewWriting(PhotonStream stream, PhotonMessageInfo info)
        {
            if (_RewriteNextSerialize)
                RewriteAll(stream, info);
            else
                WriteTemporaryONLY(stream, info);
        }

        /// <summary>
        /// Reserizalizes the entire data through the network, regardless of previous synchronizations
        /// </summary>
        public void RewriteAll(PhotonStream stream, PhotonMessageInfo info)
        {
            _RewriteNextSerialize = false;
            foreach(int id in id_data.Keys)
            {
                stream.SendNext(id);
                rewriteAll[id_data[id]](stream, info);
            }
        }

        /// <summary>
        /// Serializes only the new unsynchronized data through the network
        /// </summary>
        public void WriteTemporaryONLY(PhotonStream stream, PhotonMessageInfo info)
        {
            foreach (string dataName in data_id.Keys)
            {   
                if (checkTemporaryWrite[dataName]())
                {
                    stream.SendNext(data_id[dataName]);
                    temporaryWrite[dataName](stream, info); 
                }
            }
        }

        bool _RewriteNextSerialize = false;

        /// <summary>
        /// IPunObservable callback, used here to force a reserialization when someone connects to the application
        /// </summary>
        public override void OnPlayerEnteredRoom(Player newPlayer)
        {
            _RewriteNextSerialize = true;
        }

        /// <summary>
        /// Resets all registered data's internal values on the local PSV
        /// </summary>
        public void ResetValues()
        {
            if (!photonView.IsMine)
                return;

            foreach (var res in reset.Values)
                res.Invoke();

            _RewriteNextSerialize = true;
        }
    }
}