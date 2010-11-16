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
    public class NomadAllModulesLoadedMessage: NomadMessage
    {
        ///<summary>
        /// Initializes the instance of <see cref="NomadAllModulesLoadedMessage"/>.
        ///</summary>
        ///<param name="message">Optional message.</param>
        public NomadAllModulesLoadedMessage(string message) : base(message)
        {
        }

        /// <summary>
        /// Inherited
        /// </summary>
        /// <returns>Message in the exception.</returns>
        public override string ToString()
        {
            return base.Message;
        }
    }
}