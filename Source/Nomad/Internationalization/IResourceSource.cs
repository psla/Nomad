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
        /// <returns>translated request (i.e. text, graphic, sound) or null when not found</returns>
        object Retrieve(string request);
    }
}