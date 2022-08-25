using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace TurnerSoftware.SitemapTools.Parser;

/// <summary>
/// Based on the Sitemap specification described here: http://www.sitemaps.org/protocol.html
/// </summary>
public class XmlSitemapParser : ISitemapParser
{
#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
	public async Task<SitemapFile?> ParseSitemapAsync(Uri sitemapUrl, TextReader reader, CancellationToken cancellationToken = default)
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
	{
		XDocument document;

		try
		{
#if NETSTANDARD2_1
			document = await XDocument.LoadAsync(reader, LoadOptions.None, cancellationToken);
#else
			document = XDocument.Load(reader, LoadOptions.None);
			cancellationToken.ThrowIfCancellationRequested();
#endif
		}
		catch (XmlException)
		{
			return null;
		}

		var result = new SitemapFile(sitemapUrl);
		foreach (var topNode in document.Elements())
		{
			var nodeName = topNode.Name.LocalName;

			if (nodeName.Equals("urlset", StringComparison.InvariantCultureIgnoreCase))
			{
				var sitemapEntries = new List<SitemapEntry>();

				foreach (var urlNode in topNode.Elements())
				{
					cancellationToken.ThrowIfCancellationRequested();
					if (TryParseSitemapEntry(urlNode, out var sitemapEntry))
					{
						sitemapEntries.Add(sitemapEntry!);
					}
				}

				result = result with
				{
					Urls = sitemapEntries
				};
			}
			else if (nodeName.Equals("sitemapindex", StringComparison.InvariantCultureIgnoreCase))
			{
				var sitemapIndexEntries = new List<SitemapIndexEntry>();

				foreach (var sitemapNode in topNode.Elements())
				{
					cancellationToken.ThrowIfCancellationRequested();
					if (TryParseSitemapIndex(sitemapNode, out var indexedSitemap))
					{
						sitemapIndexEntries.Add(indexedSitemap!);
					}
				}

				result = result with
				{
					Sitemaps = sitemapIndexEntries
				};
			}
		}

		return result;
	}

	private bool TryParseSitemapIndex(XElement sitemapNode, out SitemapIndexEntry? value)
	{
		Uri? location = null;
		DateTime? lastModified = null;
		foreach (var urlDetail in sitemapNode.Elements())
		{
			var nodeName = urlDetail.Name.LocalName;
			var nodeValue = urlDetail.Value;

			if (nodeName.Equals("loc", StringComparison.InvariantCultureIgnoreCase))
			{
				if (Uri.TryCreate(nodeValue, UriKind.Absolute, out var tmpUri))
				{
					location = tmpUri;
				}
			}
			else if (nodeName.Equals("lastmod", StringComparison.InvariantCultureIgnoreCase))
			{
				if (DateTime.TryParse(nodeValue, CultureInfo.InvariantCulture, DateTimeStyles.None, out var tmpLastModified))
				{
					lastModified = tmpLastModified;
				}
			}
		}

		if (location is null)
		{
			value = default;
			return false;
		}

		value = new(location, lastModified);
		return true;
	}

	private bool TryParseSitemapEntry(XElement urlNode, out SitemapEntry? value)
	{
		Uri? location = null;
		DateTime? lastModified = null;
		ChangeFrequency? changeFrequency = null;
		var priority = SitemapEntry.DefaultPriority;

		foreach (var urlDetail in urlNode.Elements())
		{
			var nodeName = urlDetail.Name.LocalName;
			var nodeValue = urlDetail.Value;

			if (nodeName.Equals("loc", StringComparison.InvariantCultureIgnoreCase))
			{
				if (Uri.TryCreate(nodeValue, UriKind.Absolute, out var tmpUri))
				{
					location = tmpUri;
				}
			}
			else if (nodeName.Equals("lastmod", StringComparison.InvariantCultureIgnoreCase))
			{
				if (DateTime.TryParse(nodeValue, CultureInfo.InvariantCulture, DateTimeStyles.None, out var tmpLastModified))
				{
					lastModified = tmpLastModified;
				}
			}
			else if (nodeName.Equals("changefreq", StringComparison.InvariantCultureIgnoreCase))
			{
				changeFrequency = Constants.ChangeFrequency.ToEnum(nodeValue);
			}
			else if (nodeName.Equals("priority", StringComparison.InvariantCultureIgnoreCase))
			{
				if (double.TryParse(nodeValue, NumberStyles.Float, CultureInfo.InvariantCulture, out var tmpPriority))
				{
					priority = tmpPriority;
				}
			}
		}

		if (location is null)
		{
			value = default;
			return false;
		}

		value = new(location, lastModified, changeFrequency, priority);
		return true;
	}
}
