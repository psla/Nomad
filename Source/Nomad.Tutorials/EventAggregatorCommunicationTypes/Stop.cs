using System;

namespace EventAggregatorCommunicationTypes
{
    /// <summary>
    ///     Example of a control message.
    /// </summary>
    /// <remarks>
    ///     In this tutorial this class will be used as simple control message carrier.
    /// </remarks>
    [Serializable]
    public class StopPublishingMessageType
    {
        public readonly string Message;


        public StopPublishingMessageType(string message)
        {
            Message = message;
        }
    }
}