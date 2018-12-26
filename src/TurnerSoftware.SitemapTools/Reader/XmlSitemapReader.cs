using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace TurnerSoftware.SitemapTools.Reader
{
	/// <summary>
	/// Based on the Sitemap specification described here: http://www.sitemaps.org/protocol.html
	/// </summary>
	public class XmlSitemapReader : ISitemapReader
	{
		public SitemapFile ParseSitemap(string rawSitemap)
		{
			var result = new SitemapFile();
			var document = new XmlDocument();
			
			try
			{
				document.LoadXml(rawSitemap);
			}
			catch (XmlException)
			{
				return null;
			}

			foreach (XmlNode topNode in document.ChildNodes)
			{
				if (topNode.Name.ToLower() == "urlset")
				{
					var urls = new List<SitemapEntry>();

					foreach (XmlNode urlNode in topNode.ChildNodes)
					{
						var sitemapEntry = ParseSitemapEntry(urlNode);
						urls.Add(sitemapEntry);
					}

					result.Urls = urls;
				}
				else if (topNode.Name.ToLower() == "sitemapindex")
				{
					var indexedSitemaps = new List<SitemapFile>();

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

		private SitemapFile ParseSitemapIndex(XmlNode sitemapNode)
		{
			var result = new SitemapFile();
			foreach (XmlNode urlDetail in sitemapNode.ChildNodes)
			{
				var nodeName = urlDetail.Name.ToLower();
				var nodeValue = urlDetail.InnerText;

				if (nodeName == "loc")
				{
					Uri tmpUri;
					if (Uri.TryCreate(nodeValue, UriKind.Absolute, out tmpUri))
					{
						result.Location = tmpUri;
					}
				}
				else if (nodeName == "lastmod")
				{
					DateTime tmpLastModified;
					if (DateTime.TryParse(nodeValue, out tmpLastModified))
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

				if (nodeName == "loc")
				{
					Uri tmpUri;
					if (Uri.TryCreate(nodeValue, UriKind.Absolute, out tmpUri))
					{
						result.Location = tmpUri;
					}
				}
				else if (nodeName == "lastmod")
				{
					DateTime tmpLastModified;
					if (DateTime.TryParse(nodeValue, out tmpLastModified))
					{
						result.LastModified = tmpLastModified;
					}
				}
				else if (nodeName == "changefreq")
				{
					result.ChangeFrequency = ParseChangeFrequency(nodeValue);
				}
				else if (nodeName == "priority")
				{
					decimal tmpPriority;
					if (decimal.TryParse(nodeValue, out tmpPriority))
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
