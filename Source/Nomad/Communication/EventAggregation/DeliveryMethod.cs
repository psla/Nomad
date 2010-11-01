namespace Nomad.Communication.EventAggregation
{
    /// <summary>
    /// Specifies delivery method for subscription
    /// </summary>
    public enum DeliveryMethod
    {
        /// <summary>
        /// Delivers event in gui thread.
        /// </summary>
        GuiThread,
        /// <summary>
        /// Delivers event in any other thread.
        /// </summary>
        /// <remarks>
        /// Assumes, that execution of task is quick. Does not create new thread during invokation. 
        /// </remarks>
        AnyThread,
        /// <summary>
        /// Creates separate, independent thread for execution.
        /// </summary>
        SeparateThread
    }
}