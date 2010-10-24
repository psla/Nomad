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
        public string PluginsPath { get; private set; }

        public ICollection<string> ModulesToUpdate { get; set; }

        public UpdateArguments(string[] args) : this(args[0], args.Skip(1))
        { }

        public UpdateArguments(string pluginsPath, IEnumerable<string> moduleNames )
        {
            PluginsPath = pluginsPath;
            foreach (var moduleName in moduleNames)
            {
                ModulesToUpdate.Add(moduleName);
            }

            ValidateArguments();
        }

        void ValidateArguments()
        {
            if (Directory.Exists(PluginsPath))
                throw new ArgumentException("Plugins path should point to existing directory");
        }
    }
}