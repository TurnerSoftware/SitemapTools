using System;
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

		protected SitemapQuery GetSitemapQuery()
		{
			var client = TestConfiguration.GetHttpClient();
			return new SitemapQuery(client);
		}

		protected UriBuilder GetTestServerUriBuilder()
		{
			var client = TestConfiguration.GetHttpClient();
			return new UriBuilder(client.BaseAddress);
		}
	}
}
