namespace EventAggregatorCommunicationTypes
{
    /// <summary>
    /// Simple counter message class
    /// </summary>
    public class CounterMessageType
    {
        public CounterMessageType(int counter)
        {
            Counter = counter;
        }


        public int Counter { get; private set; }
    }
}