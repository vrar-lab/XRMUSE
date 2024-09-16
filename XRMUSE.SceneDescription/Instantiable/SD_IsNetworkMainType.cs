using System;

namespace XRMUSE.SceneDescription
{
    /// <summary>
    /// Attribute that indicates if the instantiable maintype is networked and needs to be instantiated prior to starting the timeline.
    /// Necessary due to the network synchronization and pool system
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class SD_IsNetworkMainType : Attribute
    {

    }
}