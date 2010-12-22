namespace FileLoaderModule
{
    /// <summary>
    /// You may use this event type to inform rest of modules that new region has been registered
    /// </summary>
    public class FileLoaderMenuRegionRegisteredMessage
    {
        public FileLoaderMenuRegionRegisteredMessage(string regionName)
        {
            RegionName = regionName;
        }


        /// <summary>
        /// Name of the region which was registered
        /// </summary>
        public string RegionName { get; private set; }
    }
}