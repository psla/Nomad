namespace Nomad.Internationalization
{
    /// <summary>
    /// Supports resource finding
    /// </summary>
    public interface IResourceSource
    {
        /// <summary>
        /// Retrieves <see cref="request"/> from resource.
        /// </summary>
        /// <param name="request">request too search for</param>
        /// <returns>translated request or null when not found</returns>
        string Retrieve(string request);
    }
}