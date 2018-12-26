using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TurnerSoftware.SitemapTools
{
	public class SitemapFile
	{
		public Uri Location { get; set; }

		public DateTime? LastModified { get; set; }
		public IEnumerable<SitemapFile> Sitemaps { get; set; }

		public IEnumerable<SitemapEntry> Urls { get; set; }

		public SitemapFile()
		{
			Sitemaps = Enumerable.Empty<SitemapFile>();
			Urls = Enumerable.Empty<SitemapEntry>();
		}
	}
}
