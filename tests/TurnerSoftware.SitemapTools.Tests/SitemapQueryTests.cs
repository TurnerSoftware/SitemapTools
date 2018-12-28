using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TurnerSoftware.SitemapTools;

namespace TurnerSoftware.SitemapTools.Tests
{
	[TestClass]
	public class SitemapQueryTests : TestBase
	{
		[TestMethod]
		public async Task GetSitemap()
		{
			var sitemapQuery = GetSitemapQuery();
			var uriBuilder = GetTestServerUriBuilder();

			uriBuilder.Path = "basic-sitemap.xml";
			var sitemap = await sitemapQuery.GetSitemap(uriBuilder.Uri);

			Assert.AreEqual(0, sitemap.Sitemaps.Count());
			Assert.AreEqual(5, sitemap.Urls.Count());
		}

		[TestMethod]
		public async Task DiscoverSitemaps()
		{
			var sitemapQuery = GetSitemapQuery();
			var discoveredSitemaps = await sitemapQuery.DiscoverSitemaps("localhost");

			Assert.AreEqual(3, discoveredSitemaps.Count());
		}

		[TestMethod]
		public async Task GetAllSitemapsForDomain()
		{
			var sitemapQuery = GetSitemapQuery();
			var sitemaps = await sitemapQuery.GetAllSitemapsForDomain("localhost");

			Assert.AreEqual(7, sitemaps.Count());
		}

		[TestMethod]
		public async Task SupportsGzippedSitemap()
		{
			var sitemapQuery = GetSitemapQuery();
			var uriBuilder = GetTestServerUriBuilder();

			uriBuilder.Path = "gzipped-sitemap.xml.gz";
			var sitemap = await sitemapQuery.GetSitemap(uriBuilder.Uri);

			var gzipSitemapReference = new Uri("http://www.example.com/gzipped/");
			Assert.IsTrue(sitemap.Urls.Any(u => u.Location == gzipSitemapReference));
		}
	}
}
