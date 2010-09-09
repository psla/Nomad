using NUnit.Framework;

namespace TestsShared.FunctionalTests
{
    /// <summary>
    ///     Called by NUnit after all fixtures from test assembly
    ///     were executed. Ensures that test application is properly
    ///     shutdown before NUnit tries to exit.
    /// </summary>
    [SetUpFixture]
    public class GuiNunitHooks
    {
        /// <summary>
        ///     Shutdowns application if it was launched
        /// </summary>
        [TestFixtureTearDown]
        public void ensure_application_is_shutdown()
        {
            GuiApplicationContainer.ShutdownApplication();
        }
    }
}