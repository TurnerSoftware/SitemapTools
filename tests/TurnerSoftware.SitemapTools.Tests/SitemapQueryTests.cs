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
		public async Task GetSitemapAsync()
		{
			var sitemapQuery = GetSitemapQuery();
			var uriBuilder = GetTestServerUriBuilder();

			uriBuilder.Path = "basic-sitemap.xml";
			var sitemap = await sitemapQuery.GetSitemapAsync(uriBuilder.Uri);

			Assert.AreEqual(0, sitemap.Sitemaps.Count());
			Assert.AreEqual(12, sitemap.Urls.Count());
		}

		[TestMethod]
		public async Task DiscoverSitemapsAsync()
		{
			var sitemapQuery = GetSitemapQuery();
			var discoveredSitemaps = await sitemapQuery.DiscoverSitemapsAsync("localhost");

			Assert.AreEqual(3, discoveredSitemaps.Count());
		}

		[TestMethod]
		public async Task GetAllSitemapsForDomainAsync()
		{
			var sitemapQuery = GetSitemapQuery();
			var sitemaps = await sitemapQuery.GetAllSitemapsForDomainAsync("localhost");

			Assert.AreEqual(7, sitemaps.Count());
		}

		[TestMethod]
		public async Task SupportsGzippedSitemapAsync()
		{
			var sitemapQuery = GetSitemapQuery();
			var uriBuilder = GetTestServerUriBuilder();

			uriBuilder.Path = "gzipped-sitemap.xml.gz";
			var sitemap = await sitemapQuery.GetSitemapAsync(uriBuilder.Uri);

			var gzipSitemapReference = new Uri("http://www.example.com/gzipped/");
			Assert.IsTrue(sitemap.Urls.Any(u => u.Location == gzipSitemapReference));
		}
	}
}
