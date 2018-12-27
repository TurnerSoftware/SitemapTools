using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TurnerSoftware.SitemapTools
{
	public class SitemapFile
	{
		public IEnumerable<SitemapIndexEntry> Sitemaps { get; set; }
		public IEnumerable<SitemapEntry> Urls { get; set; }

		public SitemapFile()
		{
			Sitemaps = Enumerable.Empty<SitemapIndexEntry>();
			Urls = Enumerable.Empty<SitemapEntry>();
		}
	}
}
