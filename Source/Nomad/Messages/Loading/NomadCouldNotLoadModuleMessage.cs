using System;

namespace Nomad.Messages.Loading
{
    /// <summary>
    ///     Class that represents type of event raised when loading of specific module cannot be done.
    /// </summary>
    [Serializable]
    public class NomadCouldNotLoadModuleMessage : NomadMessage
    {
        /// <summary>
        ///     Gets the name of the module which invoked the message.
        /// </summary>
        public string  ModuleName { get; private set; }

        /// <summary>
        ///     Initializes the instance of <see cref="NomadCouldNotLoadModuleMessage"/> class.
        /// </summary>
        /// <param name="message">Message to be passed.</param>
        /// <param name="moduleName">Name of the module which invoked the message.</param>
        public NomadCouldNotLoadModuleMessage(string message, string moduleName) : base(message)
        {
            ModuleName = moduleName;
        }

        /// <summary>
        ///     Initializes the instance of <see cref="NomadCouldNotLoadModuleMessage"/> class.
        /// </summary>
        /// <param name="message">Message to be passed.</param>
        public NomadCouldNotLoadModuleMessage(string message) : this(message, null)
        {
        }

        /// <summary>
        ///     Inherrited.
        /// </summary>
        public override string ToString()
        {
            return string.Format("The module '{0}' could not be loaded.", ModuleName);
        }
    }
}