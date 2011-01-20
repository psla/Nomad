namespace ThreadedToolbarControlledModule
{
    public class ThreadedWorkerProgressMessage
    {
        /// <summary>
        /// Range 0-100
        /// </summary>
        public int Value { get; private set; }

        public ThreadedWorkerProgressMessage(int value)
        {
            Value = value;
        }
    }
}