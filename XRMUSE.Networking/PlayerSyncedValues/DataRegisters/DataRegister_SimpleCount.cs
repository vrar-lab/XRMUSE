using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace XRMUSE.Networking
{
    /// <summary>
    /// Example usage of IPlayerSyncedValues_DataRegister with a simple integer synchronization and associated custom functions
    /// Value is locally increased by pressing "space" in-game
    /// </summary>
    public class DataRegister_SimpleCount : MonoBehaviour, IPlayerSyncedValues_DataRegister
    {
        public string DataName => "SimpleCount";
        /// <summary>
        /// The data as it is registered on the network
        /// </summary>
        public int dataCount = 0;
        /// <summary>
        /// The data with potential local changes
        /// </summary>
        public int dataCountTMP = 0;

        public void Read(PhotonStream stream, PhotonMessageInfo info)
        {
            dataCount = (int)stream.ReceiveNext();
        }

        public void RewriteAll(PhotonStream stream, PhotonMessageInfo info)
        {
            dataCount = dataCountTMP;
            stream.SendNext(dataCount);
        }

        public void TemporaryWrite(PhotonStream stream, PhotonMessageInfo info)
        {
            dataCount = dataCountTMP;
            stream.SendNext(dataCount);
        }

        public bool TemporaryWriteCheck()
        {
            return dataCount != dataCountTMP;
        }

        /// <summary>
        /// A custom function accesible from the PSV that returns this entire component
        /// </summary>
        [DataRegister_CustomFunction(0)]
        public object[] ReturnSelf(object[] args) => new object[]{this};

        /// <summary>
        /// A custom function accesible from the PSV that simply increments the dataCount locally
        /// </summary>
        [DataRegister_CustomFunction(1)]
        public object[] Increment(object[] args)
        {
            dataCountTMP++;
            return null;
        } 

        public void ResetValues()
        {
            dataCount = 0;
            dataCountTMP = 0;
        }

        PhotonView pv;
        void Start()
        {
            pv = GetComponent<PhotonView>();
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                if (!pv.IsMine)
                    return;
                //example call of the custom function "increment", which could be done outside this class
                PlayerSyncedValues.localInstance.GetFunction("SimpleCount", 1)(null);
            }
        }
    }
}