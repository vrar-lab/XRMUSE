using Photon.Pun;
using System;
using System.Collections.Generic;
using UnityEngine;
using XRMUSE.Networking;

namespace XRMUSE.ExampleScene
{
    /// <summary>
    /// Manual DataRegister that is not using IPhotonSyncedValues_DataRegister, replaced by PhotonPlayerSyncedValues_AnimationAtPosition
    /// </summary>
    public class PlayerSyncedValues_AnimationFuse : MonoBehaviour
    {
        void Awake()
        {
            //AddAnimationFuse
            GetComponent<PlayerSyncedValues>().AddData("AnimationFuse",
                TemporaryWrite, RewriteAll, Read,
                () => _animationListNewValues,
                ResetAnimationList,
                new Func<object[], object>[] { args => this, args => AddAnimation(args) });

            //RemoveAnimation Fuse
            photonView = GetComponent<PhotonView>();
            GetComponent<PlayerSyncedValues>().AddData("RemoveAnimationFuse",
                WriteTemporary_Delete, (v1, v2) => { }, Read_Delete,
                () => _animationListRemoveValues,
                Reset_Delete,
                new Func<object[], object>[] { args => this, args => RemoveAnimation(args) });

            enabled = false;
        }

        //ADD ANIMATION FUSE FUNC
        public void Read(PhotonStream stream, PhotonMessageInfo info)
        {
            int mat1, mat2, tool1;
            long timestamp;
            while ((mat1 = (int)stream.ReceiveNext()) != -1)
            {
                if (mat1 == -2)
                {
                    animationList.Clear();
                    continue;
                }

                mat2 = (int)stream.ReceiveNext();
                tool1 = (int)stream.ReceiveNext();
                timestamp = (long)stream.ReceiveNext();
                if (!animationList.Contains((mat1, mat2, tool1, timestamp)))
                    animationList.Add((mat1, mat2, tool1, timestamp));
                AnimationManager.ActivateAnimationManager(); //we at least added one animation => needs to be active
            }
        }

        public void RewriteAll(PhotonStream stream, PhotonMessageInfo info)
        {
            stream.SendNext(-2); //=>clear dict
            foreach (var val in animationList)
            {
                stream.SendNext(val.Item1);
                stream.SendNext(val.Item2);
                stream.SendNext(val.Item3);
                stream.SendNext(val.Item4);
            }

            stream.SendNext((int)-1);
        }

        public void TemporaryWrite(PhotonStream stream, PhotonMessageInfo info)
        {
            foreach (var val in animationListTMP)
            {
                if (!animationList.Contains(val))
                    animationList.Add(val);
                stream.SendNext(val.Item1);
                stream.SendNext(val.Item2);
                stream.SendNext(val.Item3);
                stream.SendNext(val.Item4);
            }

            stream.SendNext((int)-1); //photonViewIDs are 1-999, so -1 works as stop
            animationListTMP.Clear();
            _animationListNewValues = false;
        }

        public List<(int, int, int, long)> animationList = new List<(int, int, int, long)>();
        public List<(int, int, int, long)> animationListTMP = new List<(int, int, int, long)>();
        bool _animationListNewValues = false;

        public object AddAnimation(object[] args)
        {
            (int, int, int, long) toAdd = ((int)args[0], (int)args[1], (int)args[2], (long)args[3]);
            AddAnimation(toAdd);
            return null;
        }

        public void AddAnimation((int, int, int, long) toAdd)
        {
            if (!(animationList.Contains(toAdd) || animationListTMP.Contains(toAdd)))
                animationListTMP.Add(toAdd);
            AnimationManager.ActivateAnimationManager(); //We have at least one animation => needs to be active
            _animationListNewValues = true;
        }

        public void ResetAnimationList()
        {
            animationList.Clear();
            animationListTMP.Clear();
            _animationListNewValues = false;
        }

        //REMOVE ANIMATION FUSE FUNCTIONS
        bool _animationListRemoveValues = false;
        PhotonView photonView;
        public List<(int, int, int, long)> animationRemoveList = new List<(int, int, int, long)>();

        public object RemoveAnimation(object[] args)
        {
            (int, int, int, long) toRemove = ((int)args[0], (int)args[1], (int)args[2], (long)args[3]);
            RemoveAnimation(toRemove);
            return null;
        }

        public void RemoveAnimation((int, int, int, long) toRemove)
        {
            if (animationList.Contains(toRemove))
                animationList.Remove(toRemove);
            if (animationListTMP.Contains(toRemove))
                animationListTMP.Remove(toRemove);
            if (photonView.IsMine)
            {
                _animationListRemoveValues = true;
                animationRemoveList.Add(toRemove);
            }
        }

        public void Read_Delete(PhotonStream stream, PhotonMessageInfo info)
        {
            int mat1, mat2, tool1;
            long timestamp;
            while ((mat1 = (int)stream.ReceiveNext()) != -1)
            {
                mat2 = (int)stream.ReceiveNext();
                tool1 = (int)stream.ReceiveNext();
                timestamp = (long)stream.ReceiveNext();
                if (animationList.Contains((mat1, mat2, tool1, timestamp)))
                    animationList.Remove((mat1, mat2, tool1, timestamp));
            }
        }

        public void WriteTemporary_Delete(PhotonStream stream, PhotonMessageInfo info)
        {
            foreach (var val in animationRemoveList)
            {
                stream.SendNext(val.Item1);
                stream.SendNext(val.Item2);
                stream.SendNext(val.Item3);
                stream.SendNext(val.Item4);
            }

            stream.SendNext((int)-1);
            animationRemoveList.Clear();
            _animationListRemoveValues = false;
        }

        public void Reset_Delete()
        {
            animationRemoveList.Clear();
            _animationListRemoveValues = false;
        }
    }
}