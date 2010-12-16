using System;

namespace Nomad.ManifestCreator
{
    /// <summary>
    ///     Application responsible for creating manifest for all files in provided directory
    /// </summary>
    /// 
    public class ManifestCreatorProgram
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="args">
        /// first argument contains path to issuer
        /// second argument contains path to directory
        /// third argument contains module assembly name
        /// fourth - issuer name
        /// </param>
        public static void Main(string[] args)
        {
            try
            {
                var argumentsParser = new ArgumentsParser(args);
                var manifestCreator = argumentsParser.GetManifestCreator();

                manifestCreator.CreateAndPublish();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                if (args.Length != 5)
                {
                    Console.WriteLine(
                        "manifestCreator.exe path_to_issuer_xml path_to_directory assembly_name.dll issuer_name");
                    Console.WriteLine(e.Message);
                    return;
                }
                throw;
            }
        }
    }
}