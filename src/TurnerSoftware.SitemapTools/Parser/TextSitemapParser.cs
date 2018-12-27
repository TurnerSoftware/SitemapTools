using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace TurnerSoftware.SitemapTools.Parser
{
	public class TextSitemapParser : ISitemapParser
	{
		public SitemapFile ParseSitemap(TextReader reader)
		{
			var result = new SitemapFile();
			var line = string.Empty;

			var sitemapEntries = new List<SitemapEntry>();

			while ((line = reader.ReadLine()) != null)
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
