using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace TurnerSoftware.SitemapTools.Parser
{
	/// <summary>
	/// Based on the Sitemap specification described here: http://www.sitemaps.org/protocol.html
	/// </summary>
	public class XmlSitemapParser : ISitemapParser
	{
#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
		public async Task<SitemapFile> ParseSitemapAsync(TextReader reader, CancellationToken cancellationToken = default)
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
		{
			var result = new SitemapFile();
			XDocument document;

			try
			{
#if (NETSTANDARD2_1 || NETCOREAPP)
				document = await XDocument.LoadAsync(reader, LoadOptions.None, cancellationToken);
#else
				document = XDocument.Load(reader, LoadOptions.None);
				if (cancellationToken.IsCancellationRequested)
					throw new OperationCanceledException();
#endif
			}
			catch (XmlException)
			{
				return null;
			}

			foreach (var topNode in document.Elements())
			{
				var nodeName = topNode.Name.LocalName;

				if (nodeName.Equals("urlset", StringComparison.InvariantCultureIgnoreCase))
				{
					var urls = new List<SitemapEntry>();

					foreach (var urlNode in topNode.Elements())
					{
						var sitemapEntry = ParseSitemapEntry(urlNode);
						urls.Add(sitemapEntry);
					}

					result.Urls = urls;
				}
				else if (nodeName.Equals("sitemapindex", StringComparison.InvariantCultureIgnoreCase))
				{
					var indexedSitemaps = new List<SitemapIndexEntry>();

					foreach (var sitemapNode in topNode.Elements())
					{
						var indexedSitemap = ParseSitemapIndex(sitemapNode);
						indexedSitemaps.Add(indexedSitemap);
					}

					result.Sitemaps = indexedSitemaps;
				}
			}

			return result;
		}

		private SitemapIndexEntry ParseSitemapIndex(XElement sitemapNode)
		{
			var result = new SitemapIndexEntry();
			foreach (var urlDetail in sitemapNode.Elements())
			{
				var nodeName = urlDetail.Name.LocalName;
				var nodeValue = urlDetail.Value;

				if (nodeName.Equals("loc", StringComparison.InvariantCultureIgnoreCase))
				{
					if (Uri.TryCreate(nodeValue, UriKind.Absolute, out var tmpUri))
					{
						result.Location = tmpUri;
					}
				}
				else if (nodeName.Equals("lastmod", StringComparison.InvariantCultureIgnoreCase))
				{
					if (DateTime.TryParse(nodeValue, CultureInfo.InvariantCulture, DateTimeStyles.None, out var tmpLastModified))
					{
						result.LastModified = tmpLastModified;
					}
				}
			}
			return result;
		}

		private SitemapEntry ParseSitemapEntry(XElement urlNode)
		{
			var result = new SitemapEntry();
			foreach (var urlDetail in urlNode.Elements())
			{
				var nodeName = urlDetail.Name.LocalName;
				var nodeValue = urlDetail.Value;

				if (nodeName.Equals("loc", StringComparison.InvariantCultureIgnoreCase))
				{
					if (Uri.TryCreate(nodeValue, UriKind.Absolute, out var tmpUri))
					{
						result.Location = tmpUri;
					}
				}
				else if (nodeName.Equals("lastmod", StringComparison.InvariantCultureIgnoreCase))
				{
					if (DateTime.TryParse(nodeValue, CultureInfo.InvariantCulture, DateTimeStyles.None, out var tmpLastModified))
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
					if (double.TryParse(nodeValue, NumberStyles.Float, CultureInfo.InvariantCulture, out var tmpPriority))
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
