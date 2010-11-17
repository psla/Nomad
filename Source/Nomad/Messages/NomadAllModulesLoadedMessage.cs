using System;
using System.Collections.Generic;
using System.Text;
using Nomad.Modules;

namespace Nomad.Messages
{
    ///<summary>
    /// Message published when all modules from given module discovery are loaded.
    ///</summary>
    [Serializable]
    public class NomadAllModulesLoadedMessage : NomadMessage
    {
        ///<summary>
        /// Initializes the instance of <see cref="NomadAllModulesLoadedMessage"/>
        ///</summary>
        /// <param name="moduleInfos">Collection of moduleInfos concerning loaded modules.</param>
        ///<param name="message">Optional message.</param>
        public NomadAllModulesLoadedMessage(IEnumerable<ModuleInfo> moduleInfos, string message)
            : base(message)
        {
            ModuleInfos = moduleInfos;
        }


        /// <summary>
        /// Collection containing <see cref="ModuleInfo"/> objects of loaded assemblies.
        /// </summary>
        public IEnumerable<ModuleInfo> ModuleInfos { get; private set; }


        /// <summary>
        /// Inherited
        /// </summary>
        /// <returns>Message with list of moduleInfos.</returns>
        public override string ToString()
        {
            var stringBuilder = new StringBuilder();
            stringBuilder.Append(base.Message + " List of loaded modules: ");
            foreach (ModuleInfo moduleInfo in ModuleInfos)
            {
                stringBuilder.Append(moduleInfo.Manifest.ModuleName + ' ' +
                                     moduleInfo.Manifest.ModuleVersion + '\n');
            }
            return stringBuilder.ToString();
        }
    }
}