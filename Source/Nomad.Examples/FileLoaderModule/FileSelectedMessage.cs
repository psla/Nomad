namespace FileLoaderModule
{
    /// <summary>
    /// File was selected from file selection window
    /// </summary>
    public class FileSelectedMessage
    {
        /// <summary>
        /// Path to file which was selected
        /// </summary>
        public string FilePath { get; private set; }


        public FileSelectedMessage(string filePath)
        {
            FilePath = filePath;
        }
    }
}