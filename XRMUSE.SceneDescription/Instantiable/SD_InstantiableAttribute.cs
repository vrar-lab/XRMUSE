using System;

namespace XRMUSE.SceneDescription
{
    /// <summary>
    /// Attribute indicating the name of the instantiable type as used in description files, necessary for SD_Parse
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class SD_InstantiableAttribute : Attribute
    {
        public string ParseName { get; }
        public SD_InstantiableAttribute(string name) => ParseName = name;
    }

    /// <summary>
    /// Attribute indicating the name of the instantiable sub type as used in description files, necessary for SD_Parse
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class SD_InstantiableSubAttribute : Attribute
    {
        public string ParseName { get; }
        public SD_InstantiableSubAttribute(string name) => ParseName = name;
    }
}
