using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace TurnerSoftware.SitemapTools.Parser;

public class TextSitemapParser : ISitemapParser
{
	public async Task<SitemapFile?> ParseSitemapAsync(Uri sitemapUrl, TextReader reader, CancellationToken cancellationToken = default)
	{
		var sitemapEntries = new List<SitemapEntry>();

		string line;
		while ((line = await reader.ReadLineAsync()) != null)
		{
			cancellationToken.ThrowIfCancellationRequested();
			if (Uri.TryCreate(line, UriKind.Absolute, out var tmpUri))
			{
				sitemapEntries.Add(new SitemapEntry(tmpUri));
			}
		}

		return new SitemapFile(sitemapUrl)
		{
			Urls = sitemapEntries
		};
	}
}
