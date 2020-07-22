using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace TurnerSoftware.SitemapTools.Parser
{
	public class TextSitemapParser : ISitemapParser
	{
		public async Task<SitemapFile> ParseSitemapAsync(TextReader reader)
		{
			var sitemapEntries = new List<SitemapEntry>();

			string line;
			while ((line = await reader.ReadLineAsync()) != null)
			{
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
