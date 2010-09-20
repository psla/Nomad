using System.Collections.Generic;

namespace Nomad.Core
{
    /// <summary>
    /// Contains all informations concerning Nomad configuration. Also provides default configuration.
    /// </summary>
    public class NomadConfiguration
    {
        /// <summary>
        /// Contains string based configuration dictionary.
        /// </summary>
        public IDictionary<string, string> SettingsDictionary;

        //TODO: ADD rest of configuration-dependent classes/interfaces.

        /// <summary>
        /// Provides default and user-modifiable configuration for Nomad framework.
        /// </summary>
        public static NomadConfiguration Default
        {
            get
            {
                return new NomadConfiguration
                           {
                               SettingsDictionary = new Dictionary<string, string>
                                                        {
                                                            {"setting1", "value1"}
                                                        }
                           };
            }
        }
    }
}