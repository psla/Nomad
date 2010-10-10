using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nomad.KeysGenerator
{
    public class KeysGeneratorProgram
    {
        public static void Main(string[] args)
        {
            try
            {
                var arguments = new ArgumentsParser(args);
                var keyGenerator = new KeysGenerator(arguments);
                keyGenerator.GenerateSignature();
                Console.WriteLine("Successfuly generated signature file");
            }
            catch(Exception e)
            {
                Console.WriteLine("KeysGenerator.exe path_to_non_existing_xml");
                Console.WriteLine(e.Message);
            }
        }
    }
}
