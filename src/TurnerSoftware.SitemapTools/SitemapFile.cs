using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TurnerSoftware.SitemapTools
{
	public class SitemapFile
	{
		/// <summary>
		/// The sitemap location.
		/// </summary>
		public Uri Location { get; set; }
		/// <summary>
		/// List of additional sitemaps.
		/// </summary>
		public IEnumerable<SitemapIndexEntry> Sitemaps { get; set; }
		/// <summary>
		/// List of sitemap entries.
		/// </summary>
		public IEnumerable<SitemapEntry> Urls { get; set; }

		public SitemapFile()
		{
			Sitemaps = Enumerable.Empty<SitemapIndexEntry>();
			Urls = Enumerable.Empty<SitemapEntry>();
		}
	}
}
