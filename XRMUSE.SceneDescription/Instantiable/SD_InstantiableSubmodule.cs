using UnityEngine;

namespace XRMUSE.SceneDescription
{
    /// <summary>
    /// Generic interface for instantiable submodules
    /// </summary>
    public interface SD_InstantiableSubmodule
    {
        /// <summary>
        /// Instantiates the submodule to addTo
        /// </summary>
        /// <param name="addTo">Instantiable instance the submodule should be added to</param>
        public void addSubmodule(GameObject addTo);

        /// <summary>
        /// Parses the Instantiable's submodule type specific values from the lines of the text file, stops when reaching a '#'
        /// </summary>
        /// <param name="lines">All lines of the text file</param>
        /// <param name="fromLine">line to start reading the specific values from</param>
        public void Parse(string[] lines, int fromLine);
    }
}