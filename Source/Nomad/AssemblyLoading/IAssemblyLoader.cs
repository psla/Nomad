namespace Nomad.AssemblyLoading
{
    /// <summary>
    /// Loading new assemblies to application
    /// </summary>
    public interface IAssemblyLoader
    {
        /// <summary>
        /// Loads assembly to current application domain
        /// </summary>
        /// <param name="name">Assembly name to load</param>
        void LoadAssembly(string name);
    }
}