namespace EventAggregatorCommunicationTypes
{
    /// <summary>
    /// Example of a control message
    /// </summary>
    public class StopPublishingMessageType
    {
        public readonly string Message;


        public StopPublishingMessageType(string message)
        {
            Message = message;
        }
    }
}