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
        /// Delivers event in any other thread
        /// </summary>
        AnyThread
    }
}