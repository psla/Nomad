using NUnit.Framework;

namespace TestsShared.FunctionalTests
{
    [SetUpFixture]
    public class GuiNunitHooks
    {
        [TestFixtureTearDown]
        public void ensure_application_is_shutdown()
        {
            GuiApplicationContainer.ShutdownApplication();
        }
    }
}