using System;
using System.Collections.Generic;
using System.Text;

namespace TurnerSoftware.SitemapTools
{
	public class SitemapIndexEntry
	{
		public Uri Location { get; set; }
		public DateTime? LastModified { get; set; }
	}
}
