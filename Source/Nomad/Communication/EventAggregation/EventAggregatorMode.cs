namespace Nomad.Communication.EventAggregation
{
    /// <summary>
    ///     Describes the mode in which the <see cref="IEventAggregator"/> is working.
    /// </summary>
    public enum EventAggregatorMode
    {
        ///<summary>
        ///     Calls will be transported <c>only</c> to domain on which code has been called.
        ///</summary>
        MyDomain,
        /// <summary>
        ///     Calls will be transported <c>only</c> to domain the other domain.
        /// </summary>
        OtherDomain,
        /// <summary>
        ///     Calls will be transported to <c>all</c> available domains.
        /// </summary>
        AllDomain
    }
}