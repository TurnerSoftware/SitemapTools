using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TurnerSoftware.SitemapTools;

namespace TurnerSoftware.SitemapTools.Tests
{
	[TestClass]
	public class ExampleTests : TestBase
	{
		[TestMethod]
		public async Task CheckController()
		{
			var client = TestConfiguration.GetHttpClient();
			var uriBuilder = new UriBuilder(client.BaseAddress);

			var sitemapQuery = new SitemapQuery(client);

			uriBuilder.Path = "example-sitemap.xml";
			var sitemap = await sitemapQuery.GetSitemap(uriBuilder.Uri);
		}
	}
}
