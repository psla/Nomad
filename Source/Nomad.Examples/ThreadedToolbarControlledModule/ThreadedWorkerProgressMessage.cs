namespace ThreadedToolbarControlledModule
{
    public class ThreadedWorkerProgressMessage
    {
        public int Value { get; private set; }

        public ThreadedWorkerProgressMessage(int value)
        {
            Value = value;
        }
    }
}