using System;

namespace Nomad.Communication.Messages
{
    /// <summary>
    ///     Abstract class that defines the message from kernel.
    /// </summary>
    [Serializable]
    public abstract class NomadKernelMessage
    {
        /// <summary>
        ///     Text message.
        /// </summary>
        public string Message { get; private set; }

        protected NomadKernelMessage(string message)
        {
            Message = message;
        }
    }
}