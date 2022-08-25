﻿using System;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TurnerSoftware.SitemapTools.Tests;

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

			Assert.IsNotNull(sitemap);
			Assert.AreEqual(0, sitemap.Sitemaps.Count());
			Assert.AreEqual(12, sitemap.Urls.Count());
		}
	}

	[TestMethod]
	public async Task GetSitemapAsync_NotFound()
	{
		var sitemapQuery = GetSitemapQuery();
		var uriBuilder = GetTestServerUriBuilder();

		uriBuilder.Path = "basic-sitemapNotFound.xml";
		var sitemap = await sitemapQuery.GetSitemapAsync(uriBuilder.Uri);

		Assert.IsNull(sitemap);
	}

	[TestMethod]
	public async Task GetSitemapAsync_WrongFormat()
	{
		var sitemapQuery = GetSitemapQuery();
		var uriBuilder = GetTestServerUriBuilder();

		uriBuilder.Path = "basic-sitemap-WrongFormat.xml";
		var sitemap = await sitemapQuery.GetSitemapAsync(uriBuilder.Uri);

		Assert.IsNull(sitemap);
	}

	[TestMethod]
	public async Task GetSitemapAsync_WrongFormatTxt()
	{
		var sitemapQuery = GetSitemapQuery();
		var uriBuilder = GetTestServerUriBuilder();

		uriBuilder.Path = "basic-sitemap-WrongFormat.txt";
		var sitemap = await sitemapQuery.GetSitemapAsync(uriBuilder.Uri);

		Assert.IsNotNull(sitemap);
		Assert.AreEqual(0, sitemap.Sitemaps.Count());
		Assert.AreEqual(0, sitemap.Urls.Count());
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
	public async Task SupportsGzippedSitemap()
	{
		foreach (var culture in CultureInfo.GetCultures(CultureTypes.AllCultures))
		{
			Thread.CurrentThread.CurrentCulture = culture;

			var sitemapQuery = GetSitemapQuery();
			var uriBuilder = GetTestServerUriBuilder();

			uriBuilder.Path = "gzipped-sitemap.xml.gz";
			var sitemap = await sitemapQuery.GetSitemapAsync(uriBuilder.Uri);

			Assert.IsNotNull(sitemap);
			var gzipSitemapReference = new Uri("http://www.example.com/gzipped/");
			Assert.IsTrue(sitemap.Urls.Any(u => u.Location == gzipSitemapReference));
		}
	}
}
