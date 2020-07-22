﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TurnerSoftware.SitemapTools.Tests
{
	[TestClass]
	public class SitemapQueryTests : TestBase
	{
		[TestMethod]
		public async Task GetSitemapAsync()
		{
			foreach (var culture in CultureInfo.GetCultures(CultureTypes.AllCultures))
			{
				Thread.CurrentThread.CurrentCulture = culture;

				var sitemapQuery = GetSitemapQuery();
				var uriBuilder = GetTestServerUriBuilder();

				uriBuilder.Path = "basic-sitemap.xml";
				var sitemap = await sitemapQuery.GetSitemapAsync(uriBuilder.Uri);

				Assert.AreEqual(0, sitemap.Sitemaps.Count());
				Assert.AreEqual(12, sitemap.Urls.Count());
			}
		}

		[TestMethod]
		public async Task GetSitemapAsyncNotFound()
		{
			var sitemapQuery = GetSitemapQuery();
			var uriBuilder = GetTestServerUriBuilder();

			uriBuilder.Path = "basic-sitemapNotFound.xml";
			var sitemap = await sitemapQuery.GetSitemapAsync(uriBuilder.Uri);

			Assert.AreEqual(null, sitemap);
		}

		[TestMethod]
		public async Task GetSitemapAsyncWrongFormat()
		{
			var sitemapQuery = GetSitemapQuery();
			var uriBuilder = GetTestServerUriBuilder();

			uriBuilder.Path = "basic-sitemap-WrongFormat.xml";
			var sitemap = await sitemapQuery.GetSitemapAsync(uriBuilder.Uri);

			Assert.AreEqual(null, sitemap);
		}

		[TestMethod]
		public async Task GetSitemapAsyncWrongFormatTxt()
		{
			var sitemapQuery = GetSitemapQuery();
			var uriBuilder = GetTestServerUriBuilder();

			uriBuilder.Path = "basic-sitemap-WrongFormat.txt";
			var sitemap = await sitemapQuery.GetSitemapAsync(uriBuilder.Uri);

			Assert.AreEqual(0, sitemap.Sitemaps.Count());
			Assert.AreEqual(0, sitemap.Urls.Count());
		}

		[TestMethod]
		public async Task GetSitemapAsyncCancelation()
		{
			var cts = new CancellationTokenSource(0);
			var sitemapQuery = GetSitemapQuery();
			var uriBuilder = GetTestServerUriBuilder();

			uriBuilder.Path = "basic-sitemap.xml";
			SitemapFile sitemap = null;
			await Assert.ThrowsExceptionAsync<OperationCanceledException>(async () => sitemap = await sitemapQuery.GetSitemapAsync(uriBuilder.Uri, cts.Token));
			Assert.AreEqual(null, sitemap);
		}

		[TestMethod]
		public async Task DiscoverSitemapsAsync()
		{
			foreach (var culture in CultureInfo.GetCultures(CultureTypes.AllCultures))
			{
				Thread.CurrentThread.CurrentCulture = culture;

				var sitemapQuery = GetSitemapQuery();
				var discoveredSitemaps = await sitemapQuery.DiscoverSitemapsAsync("localhost");

				Assert.AreEqual(3, discoveredSitemaps.Count());
			}
		}

		[TestMethod]
		public async Task DiscoverSitemapsAsyncCancelation()
		{
			var cts = new CancellationTokenSource(0);
			var sitemapQuery = GetSitemapQuery();
			IEnumerable<Uri> discoveredSitemaps = null;
			await Assert.ThrowsExceptionAsync<OperationCanceledException>(async () => discoveredSitemaps = await sitemapQuery.DiscoverSitemapsAsync("localhost", cts.Token));
			Assert.AreEqual(null, discoveredSitemaps);
		}

		[TestMethod]
		public async Task GetAllSitemapsForDomainAsync()
		{
			foreach (var culture in CultureInfo.GetCultures(CultureTypes.AllCultures))
			{
				Thread.CurrentThread.CurrentCulture = culture;

				var sitemapQuery = GetSitemapQuery();
				var sitemaps = await sitemapQuery.GetAllSitemapsForDomainAsync("localhost");

				Assert.AreEqual(7, sitemaps.Count());
			}
		}

		[TestMethod]
		public async Task GetAllSitemapsForDomainAsyncCancelation()
		{
			var cts = new CancellationTokenSource(0);
			var sitemapQuery = GetSitemapQuery();
			IEnumerable<SitemapFile> sitemaps = null;
			await Assert.ThrowsExceptionAsync<OperationCanceledException>(async () => sitemaps = await sitemapQuery.GetAllSitemapsForDomainAsync("localhost", cts.Token));
			Assert.AreEqual(null, sitemaps);
		}

		[TestMethod]
		public async Task SupportsGzippedSitemapAsync()
		{
			foreach (var culture in CultureInfo.GetCultures(CultureTypes.AllCultures))
			{
				Thread.CurrentThread.CurrentCulture = culture;

				var sitemapQuery = GetSitemapQuery();
				var uriBuilder = GetTestServerUriBuilder();

				uriBuilder.Path = "gzipped-sitemap.xml.gz";
				var sitemap = await sitemapQuery.GetSitemapAsync(uriBuilder.Uri);

				var gzipSitemapReference = new Uri("http://www.example.com/gzipped/");
				Assert.IsTrue(sitemap.Urls.Any(u => u.Location == gzipSitemapReference));
			}
		}
	}
}
