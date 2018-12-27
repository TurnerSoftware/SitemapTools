using Microsoft.VisualStudio.TestTools.UnitTesting;
using TurnerSoftware.SitemapTools.Tests.Server;

namespace TurnerSoftware.SitemapTools.Tests
{
	[TestClass]
	public class TestBase
	{
		[AssemblyInitialize]
		public static void AssemblyInitialize(TestContext context)
		{
			TestConfiguration.StartupServer();
		}

		[AssemblyCleanup]
		public static void AssemblyCleanup()
		{
			TestConfiguration.ShutdownServer();
		}
	}
}
