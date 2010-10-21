using System;

namespace Nomad.KeysGenerator
{
    public class KeysGeneratorProgram
    {
        public static void Main(string[] args)
        {
            try
            {
                var arguments = new ArgumentsParser(args);
                var keyGenerator = new RsaKeyFilesGenerator(arguments.PublicFile, arguments.TargetFile);
                keyGenerator.GenerateSignature();
                Console.WriteLine("Successfuly generated signature file");
            }
            catch (Exception e)
            {
                Console.WriteLine("RsaKeyFilesGenerator.exe path_to_non_existing_xml");
                Console.WriteLine(e.Message);
            }
        }
    }
}