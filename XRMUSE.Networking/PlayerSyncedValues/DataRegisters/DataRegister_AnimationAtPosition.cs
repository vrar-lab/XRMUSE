using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace XRMUSE.Networking
{
    /// <summary>
    /// A generic dataRegister meant to play an animation passed as a UnityEvent, used as a pseudo remote procedure call
    /// </summary>
    public class DataRegister_AnimationAtPosition : MonoBehaviour, IPlayerSyncedValues_DataRegister
    {
        public string DataName => dataName;
        /// <summary>
        /// dataName is made public and should be unique to the component, can add the component multiple times to the same PSV as long as there's different dataNames
        /// </summary>
        public string dataName = "DisappearAnimation";

        /// <summary>
        /// The animation played by the pseudo RPC
        /// </summary>
        public UnityEvent<Vector3> playAnimationEvent;

        public void Read(PhotonStream stream, PhotonMessageInfo info)
        {
            float x, y, z;
            long timestamp;
            while ((timestamp = (long)stream.ReceiveNext()) != -1)
            {
                if (timestamp == -2)
                {
                    continue;
                }
                x = (float)stream.ReceiveNext();
                y = (float)stream.ReceiveNext();
                z = (float)stream.ReceiveNext();
                DoAnimation(timestamp,new Vector3(x, y, z));
            }
        }

        public void ResetValues()
        {
            toSend.Clear();
        }

        public void RewriteAll(PhotonStream stream, PhotonMessageInfo info)
        {
            TemporaryWrite(stream, info);
        }

        public void TemporaryWrite(PhotonStream stream, PhotonMessageInfo info)
        {
            foreach (var val in toSend)
            {
                stream.SendNext(val.Item2);
                stream.SendNext(val.Item1.x);
                stream.SendNext(val.Item1.y);
                stream.SendNext(val.Item1.z);
            }
            stream.SendNext((long)-1);//timestamp to tell stop
            toSend.Clear();
        }

        public bool TemporaryWriteCheck()
        {
            return toSend.Count > 0;
        }

        List<(Vector3,long)> toSend = new List<(Vector3, long)> ();

        public float ignoreDelayThreshold = 5f;
        public void DoAnimation(long timestamp, Vector3 position)
        {
            if ((timestamp - DateTime.UtcNow.Ticks) / 10000000f > ignoreDelayThreshold)
                return;
            playAnimationEvent.Invoke(position);
        }

        /// <summary>
        /// Accessible custom function, plays the animation at the position passed as a parameter
        /// </summary>
        [DataRegister_CustomFunction(0)]
        public object[] DoAnimation(object[] args)
        {
            Vector3 position = (Vector3)args[0];
            
            playAnimationEvent.Invoke(position);
            toSend.Add((position, DateTime.UtcNow.Ticks));
            return null;
        }

        
        /// <summary>
        /// Static function making the local player register a new networked animation pseudo-rpc through the PSV
        /// Plays the animation with the "dataName" at "position"
        /// </summary>
        public static void AddAnimation(Vector3 position, string dataName)
        {
            PlayerSyncedValues.localInstance.GetFunction(dataName, 0)(new object[] {position});
        }

        /*//Uncomment if need to test
        PhotonView pv;
        public void Start()
        {
            pv = GetComponent<PhotonView>();
            playAnimationEvent.AddListener((arg) => Debug.LogError("Playing animation " + dataName + " at " + arg));
        }
        public void Update()
        {
            if (!pv.IsMine)
                return;
            if (Input.GetKeyDown(KeyCode.Space))
                AddAnimation(transform.position, dataName);
        }
        */
    }
}