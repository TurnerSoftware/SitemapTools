using System;
using System.Globalization;
using System.Linq;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TurnerSoftware.SitemapTools.Parser;

namespace TurnerSoftware.SitemapTools.Tests
{
	[TestClass]
	public class TextSitemapParserTests : TestBase
	{
		[TestMethod]
		public void ParseTextSitemap()
		{
			foreach (var culture in CultureInfo.GetCultures(CultureTypes.AllCultures))
			{
				Thread.CurrentThread.CurrentCulture = culture;

				using (var reader = LoadResource("text-sitemap.txt"))
				{
					var parser = new TextSitemapParser();
					var sitemapFile = parser.ParseSitemap(reader);

					Assert.AreEqual(3, sitemapFile.Urls.Count());

					var entry = sitemapFile.Urls.ElementAt(0);
					Assert.AreEqual(new Uri("http://www.example.com/"), entry.Location);
					entry = sitemapFile.Urls.ElementAt(1);
					Assert.AreEqual(new Uri("http://www.example.com/about"), entry.Location);
					entry = sitemapFile.Urls.ElementAt(2);
					Assert.AreEqual(new Uri("http://www.example.com/contact-us"), entry.Location);
				}
			}
		}
	}
}
