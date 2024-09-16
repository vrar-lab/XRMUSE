using System;

namespace XRMUSE.SceneDescription
{
    /// <summary>
    /// Attribute indicating the name of the event type as used in description files, necessary for SD_Parse
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class SD_EventAttribute : Attribute
    {
        public string ParseName { get; }
        public SD_EventAttribute(string name) => ParseName = name;
    }

}