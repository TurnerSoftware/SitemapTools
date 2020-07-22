using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace TurnerSoftware.SitemapTools.Parser
{
	public class TextSitemapParser : ISitemapParser
	{
		public async Task<SitemapFile> ParseSitemapAsync(TextReader reader, CancellationToken cancellationToken = default)
		{
			var sitemapEntries = new List<SitemapEntry>();

			string line;
			while ((line = await reader.ReadLineAsync()) != null)
			{
				if (cancellationToken.IsCancellationRequested)
					throw new OperationCanceledException();
				if (Uri.TryCreate(line, UriKind.Absolute, out var tmpUri))
				{
					sitemapEntries.Add(new SitemapEntry
					{
						Location = tmpUri
					});
				}
			}

			return new SitemapFile
			{
				Urls = sitemapEntries
			};
		}
	}
}
