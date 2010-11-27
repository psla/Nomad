using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Nomad.Updater
{
    /// <summary>
    /// Cotnains information about modules 
    /// </summary>
    public class UpdateArguments
    {
        public UpdateArguments(string[] args) : this(args[0], args.Skip(1))
        {
        }


        public UpdateArguments(string pluginsPath, IEnumerable<string> moduleNames)
        {
            PluginsPath = pluginsPath;
            foreach (string moduleName in moduleNames)
            {
                ModulesToUpdate.Add(moduleName);
            }

            ValidateArguments();
        }


        /// <summary>
        ///     Path to plugins.
        /// </summary>
        public string PluginsPath { get; private set; }

        /// <summary>
        ///     Collection of unique names of modules.
        /// </summary>
        public ICollection<string> ModulesToUpdate { get; set; }


        private void ValidateArguments()
        {
            if (Directory.Exists(PluginsPath))
                throw new ArgumentException("Plugins path should point to existing directory");
        }
    }
}