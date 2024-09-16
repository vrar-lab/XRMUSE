using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace XRMUSE.Networking
{
    /// <summary>
    /// An interface to manage syncing transforms between synced object. Assumes MonoBehaviourPun, IPunObservable on the object.
    /// TO USE DO THE FOLLOWING:
    /// Add to the class:
    /// bool _was_mine = true; public bool was_mine { get => _was_mine; set => _was_mine = value; }
    /// UnityEvent _eventOnMine = new UnityEvent(); public UnityEvent eventOnMine { get => _eventOnMine; set => _eventOnMine = value; }
    /// UnityEvent _eventOnNoLongerMine = new UnityEvent(); public UnityEvent eventOnNoLongerMine { get => _eventOnNoLongerMine; set => _eventOnNoLongerMine = value; }
    /// 
    /// Add to OnPhotonSerializeView(): (this as INetworkingOwnershipBehaviour).OnPhotonSerializeView_OwnershipBehaviour(photonView);
    /// </summary>
    public interface INetworkingOwnershipBehaviour
    {
        public bool was_mine { get; set; }
        /// <summary>
        /// Events that triggers when we get ownership of the object, used for syncing stuff such as activating/deactivating grabs
        /// </summary>
        public UnityEvent eventOnMine { get; set; }
        /// <summary>
        /// Events that triggers when we lose ownership of the object, used for syncing stuff such as activating/deactivating grabs
        /// </summary>
        public UnityEvent eventOnNoLongerMine { get; set; }

        void OnPhotonSerializeView_OwnershipBehaviour(PhotonView photonView)
        {
            if (photonView.IsMine)
            {
                if (!was_mine)
                    eventOnMine.Invoke();
            }
            else if (was_mine)
                eventOnNoLongerMine.Invoke();
            was_mine = photonView.IsMine;
        }
    }
}
