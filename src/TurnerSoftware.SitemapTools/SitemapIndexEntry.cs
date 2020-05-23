using System;
using System.Collections.Generic;
using System.Text;

namespace TurnerSoftware.SitemapTools
{
	public class SitemapIndexEntry
	{
		/// <summary>
		/// The sitemap location.
		/// </summary>
		public Uri Location { get; set; }
		/// <summary>
		/// Last modified time for sitemap.
		/// </summary>
		public DateTime? LastModified { get; set; }
	}
}
