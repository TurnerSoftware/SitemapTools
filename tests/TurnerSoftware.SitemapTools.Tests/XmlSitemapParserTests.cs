using System;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TurnerSoftware.SitemapTools.Parser;

namespace TurnerSoftware.SitemapTools.Tests
{
	[TestClass]
	public class XmlSitemapParserTests : TestBase
	{
		[TestMethod]
		public async Task ChangeFrequenciesAreSetCorrectly()
		{
			foreach (var culture in CultureInfo.GetCultures(CultureTypes.AllCultures))
			{
				Thread.CurrentThread.CurrentCulture = culture;

				using (var reader = LoadResource("basic-sitemap.xml"))
				{
					var parser = new XmlSitemapParser();
					var sitemapFile = await parser.ParseSitemapAsync(reader);

					var entries = sitemapFile.Urls.Where(e => e.Location.AbsolutePath.Contains("frequency/"));

					var alwaysEntry = entries.FirstOrDefault(e => e.Location.AbsolutePath.Contains("always"));
					Assert.IsNotNull(alwaysEntry);
					Assert.AreEqual(ChangeFrequency.Always, alwaysEntry.ChangeFrequency);

					var hourlyEntry = entries.FirstOrDefault(e => e.Location.AbsolutePath.Contains("hourly"));
					Assert.IsNotNull(hourlyEntry);
					Assert.AreEqual(ChangeFrequency.Hourly, hourlyEntry.ChangeFrequency);

					var dailyEntry = entries.FirstOrDefault(e => e.Location.AbsolutePath.Contains("daily"));
					Assert.IsNotNull(dailyEntry);
					Assert.AreEqual(ChangeFrequency.Daily, dailyEntry.ChangeFrequency);

					var weeklyEntry = entries.FirstOrDefault(e => e.Location.AbsolutePath.Contains("weekly"));
					Assert.IsNotNull(weeklyEntry);
					Assert.AreEqual(ChangeFrequency.Weekly, weeklyEntry.ChangeFrequency);

					var monthlyEntry = entries.FirstOrDefault(e => e.Location.AbsolutePath.Contains("monthly"));
					Assert.IsNotNull(monthlyEntry);
					Assert.AreEqual(ChangeFrequency.Monthly, monthlyEntry.ChangeFrequency);

					var yearlyEntry = entries.FirstOrDefault(e => e.Location.AbsolutePath.Contains("yearly"));
					Assert.IsNotNull(yearlyEntry);
					Assert.AreEqual(ChangeFrequency.Yearly, yearlyEntry.ChangeFrequency);

					var neverEntry = entries.FirstOrDefault(e => e.Location.AbsolutePath.Contains("never"));
					Assert.IsNotNull(neverEntry);
					Assert.AreEqual(ChangeFrequency.Never, neverEntry.ChangeFrequency);
				}
			}
		}

		[TestMethod]
		public async Task ParseIndexFileAsync()
		{
			foreach (var culture in CultureInfo.GetCultures(CultureTypes.AllCultures))
			{
				Thread.CurrentThread.CurrentCulture = culture;

				using (var reader = LoadResource("another-indexed-sitemap.xml"))
				{
					var parser = new XmlSitemapParser();
					var sitemapFile = await parser.ParseSitemapAsync(reader);

					Assert.AreEqual(1, sitemapFile.Sitemaps.Count());

					var indexEntry = sitemapFile.Sitemaps.FirstOrDefault();
					Assert.AreEqual(new Uri("http://localhost/last-text-sitemap.txt"), indexEntry.Location);
					Assert.AreEqual(new DateTime(2005, 1, 1), indexEntry.LastModified);
				}
			}
		}

		[TestMethod]
		public async Task ParseSitemapFileAsync()
		{
			foreach (var culture in CultureInfo.GetCultures(CultureTypes.AllCultures))
			{
				Thread.CurrentThread.CurrentCulture = culture;

				using (var reader = LoadResource("basic-sitemap.xml"))
				{
					var parser = new XmlSitemapParser();
					var sitemapFile = await parser.ParseSitemapAsync(reader);

					Assert.AreEqual(12, sitemapFile.Urls.Count());

					var sitemapEntry = sitemapFile.Urls.FirstOrDefault();
					Assert.AreEqual(new Uri("http://www.example.com/"), sitemapEntry.Location);
					Assert.AreEqual(new DateTime(2005, 1, 2), sitemapEntry.LastModified);
					Assert.AreEqual(0.8, sitemapEntry.Priority);

					sitemapEntry = sitemapFile.Urls.ElementAt(1);
					Assert.AreEqual(new Uri("http://www.example.com/catalog?item=12&desc=vacation_hawaii"), sitemapEntry.Location);
					Assert.AreEqual(0.5, sitemapEntry.Priority);
				}
			}
		}

		[TestMethod]
		public async Task ParseSitemapFileAsync_Cancellation()
		{
			using (var reader = LoadResource("basic-sitemap.xml"))
			{
				var parser = new XmlSitemapParser();
				try
				{
					await parser.ParseSitemapAsync(reader, new CancellationToken(true));
				}
				catch (Exception ex) when (ex is TaskCanceledException || ex is OperationCanceledException)
				{
					return;
				}
				Assert.Fail("Expected exception not thrown");
			}
		}
	}
}
