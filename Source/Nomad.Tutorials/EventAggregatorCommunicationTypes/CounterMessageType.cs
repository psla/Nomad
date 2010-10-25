namespace EventAggregatorCommunicationTypes
{
    /// <summary>
    ///     Simple counter message class.
    /// </summary>
    /// <remarks>
    ///     In this tutorial this class will be used as message carrier.
    /// </remarks>
    public class CounterMessageType
    {
        public CounterMessageType(int counter)
        {
            Counter = counter;
        }

        public int Counter { get; private set; }
    }
}