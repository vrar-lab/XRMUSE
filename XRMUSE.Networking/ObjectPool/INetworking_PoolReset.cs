using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace XRMUSE.Networking
{
    /// <summary>
    /// Simple interface providing a "cleaning" function when the GameObject is "destroyed" by the PoolManager
    /// </summary>
    public interface INetworking_PoolReset
    {
        public void Reset();
    }
}