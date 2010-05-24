using System.Collections.Generic;
using WPFHostApp.DomainLogic.CodeTests;

namespace WPFHostApp.DomainLogic
{
    public class CodeToTestSimpleFactory
    {
        public List<ICodeToTest> ListOfCodeTests { set; get; }

        public CodeToTestSimpleFactory()
        {
            ListOfCodeTests = new List<ICodeToTest>();
            ListOfCodeTests.Add(new ConsoleInjectingCodeTest() );
            ListOfCodeTests.Add(new ConsoleStandaloneCodeTest());
            ListOfCodeTests.Add(new WpfGuestAppCodeTest());
        }
    }
}