using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace TurnerSoftware.SitemapTools.Parser
{
	/// <summary>
	/// Based on the Sitemap specification described here: http://www.sitemaps.org/protocol.html
	/// </summary>
	public class XmlSitemapParser : ISitemapParser
	{
		public SitemapFile ParseSitemap(TextReader reader)
		{
			var result = new SitemapFile();
			var document = new XmlDocument();
			
			try
			{
				document.Load(reader);
			}
			catch (XmlException)
			{
				return null;
			}

			foreach (XmlNode topNode in document.ChildNodes)
			{
				var nodeName = topNode.Name;

				if (nodeName.Equals("urlset", StringComparison.InvariantCultureIgnoreCase))
				{
					var urls = new List<SitemapEntry>();

					foreach (XmlNode urlNode in topNode.ChildNodes)
					{
						var sitemapEntry = ParseSitemapEntry(urlNode);
						urls.Add(sitemapEntry);
					}

					result.Urls = urls;
				}
				else if (nodeName.Equals("sitemapindex", StringComparison.InvariantCultureIgnoreCase))
				{
					var indexedSitemaps = new List<SitemapIndexEntry>();

					foreach (XmlNode sitemapNode in topNode.ChildNodes)
					{
						var indexedSitemap = ParseSitemapIndex(sitemapNode);
						indexedSitemaps.Add(indexedSitemap);
					}

					result.Sitemaps = indexedSitemaps;
				}
			}

			return result;
		}

		private SitemapIndexEntry ParseSitemapIndex(XmlNode sitemapNode)
		{
			var result = new SitemapIndexEntry();
			foreach (XmlNode urlDetail in sitemapNode.ChildNodes)
			{
				var nodeName = urlDetail.Name;
				var nodeValue = urlDetail.InnerText;

				if (nodeName.Equals("loc", StringComparison.InvariantCultureIgnoreCase))
				{
					if (Uri.TryCreate(nodeValue, UriKind.Absolute, out var tmpUri))
					{
						result.Location = tmpUri;
					}
				}
				else if (nodeName.Equals("lastmod", StringComparison.InvariantCultureIgnoreCase))
				{
					if (DateTime.TryParse(nodeValue, out var tmpLastModified))
					{
						result.LastModified = tmpLastModified;
					}
				}
			}
			return result;
		}

		private SitemapEntry ParseSitemapEntry(XmlNode urlNode)
		{
			var result = new SitemapEntry();
			foreach (XmlNode urlDetail in urlNode.ChildNodes)
			{
				var nodeName = urlDetail.Name.ToLower();
				var nodeValue = urlDetail.InnerText;

				if (nodeName.Equals("loc", StringComparison.InvariantCultureIgnoreCase))
				{
					if (Uri.TryCreate(nodeValue, UriKind.Absolute, out var tmpUri))
					{
						result.Location = tmpUri;
					}
				}
				else if (nodeName.Equals("lastmod", StringComparison.InvariantCultureIgnoreCase))
				{
					if (DateTime.TryParse(nodeValue, out var tmpLastModified))
					{
						result.LastModified = tmpLastModified;
					}
				}
				else if (nodeName.Equals("changefreq", StringComparison.InvariantCultureIgnoreCase))
				{
					result.ChangeFrequency = ParseChangeFrequency(nodeValue);
				}
				else if (nodeName.Equals("priority", StringComparison.InvariantCultureIgnoreCase))
				{
					if (double.TryParse(nodeValue, out var tmpPriority))
					{
						result.Priority = tmpPriority;
					}
				}
			}
			return result;
		}

		private ChangeFrequency? ParseChangeFrequency(string frequency)
		{
			frequency = frequency.ToLower();
			switch (frequency)
			{
				case "always":
					return ChangeFrequency.Always;
				case "hourly":
					return ChangeFrequency.Hourly;
				case "daily":
					return ChangeFrequency.Daily;
				case "weekly":
					return ChangeFrequency.Weekly;
				case "monthly":
					return ChangeFrequency.Monthly;
				case "yearly":
					return ChangeFrequency.Yearly;
				case "never":
					return ChangeFrequency.Never;
				default:
					return null;
			}
		}
	}
}
