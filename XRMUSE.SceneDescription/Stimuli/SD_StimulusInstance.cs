using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace XRMUSE.SceneDescription
{
    /// <summary>
    /// Generic interface for an instanced Stimulus. Any stimulus needs to be able to be activated or interrupted.
    /// </summary>
    public interface SD_StimulusInstance
    {
        /// <summary>
        /// Activates an instanced stimulus
        /// </summary>
        public void Activate();
        /// <summary>
        /// Interrupts an ongoing an instanced stimulus
        /// </summary>
        public void Interrupt();
    }
}
