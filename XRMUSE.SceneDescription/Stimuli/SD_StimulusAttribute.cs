using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace XRMUSE.SceneDescription
{

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class SD_StimulusAttribute : Attribute
    {
        public string ParseName { get; }
        public SD_StimulusAttribute(string name) => ParseName = name;
    }
}