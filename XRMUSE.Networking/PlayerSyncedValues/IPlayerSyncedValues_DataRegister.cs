using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace XRMUSE.Networking
{
    /// <summary>
    /// Interface to implement for adding data to the per-player "PlayerSyncedValues"
    /// </summary>
    public interface IPlayerSyncedValues_DataRegister
    {
        /// <summary>
        /// Reads and update the values from a networked PSV. Should be read in the same order as RewriteAll and TemporaryWrite
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="info"></param>
        public void Read(PhotonStream stream, PhotonMessageInfo info);
        /// <summary>
        /// Sends all the values of the registered data regardless of what have been previously sent. Primarly used when clients connects or when a new object is created
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="info"></param>
        public void RewriteAll(PhotonStream stream, PhotonMessageInfo info);
        /// <summary>
        /// Sends the changes on the register data since last synchronization
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="info"></param>
        public void TemporaryWrite(PhotonStream stream, PhotonMessageInfo info);

        /// <summary>
        /// Resets all values of this registered data to its default state
        /// </summary>
        public void ResetValues();

        /// <summary>
        /// Verify if there's any new data ready to send over the network
        /// </summary>
        public bool TemporaryWriteCheck();

        /// <summary>
        /// The name of the data used to access the custom functions through reflection, should be unique within a PSV
        /// </summary>
        public string DataName { get; }
    }

    /// <summary>
    /// Attribute indicating function should be considered a custom method in PlayerSyncedValues
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class DataRegister_CustomFunction : Attribute
    {
        public int FunctionID { get; }
        public DataRegister_CustomFunction(int id) => FunctionID = id;
    }
}