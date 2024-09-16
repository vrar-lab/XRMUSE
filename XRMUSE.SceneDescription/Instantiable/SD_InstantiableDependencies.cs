namespace XRMUSE.SceneDescription
{
    /// <summary>
    /// Should be added to an SD_InstantiableMain or SubModule if there's any dependencies to an other instantiable
    /// For instance, a factory that produces other instantiables might need said instantiable
    /// NOT IN USE OR NEEDED FOR NOW, MAY BE IN USE LATER WITH EDITED TS_TIMELINE
    /// </summary>
    public interface SD_InstantiableDependencies
    {
        /// <summary>
        /// What are the dependencies for said instantiable?
        /// </summary>
        /// <returns>arrays of user defined IDs of dependent instantiables</returns>
        public string[] getDependencies();
    }
}
