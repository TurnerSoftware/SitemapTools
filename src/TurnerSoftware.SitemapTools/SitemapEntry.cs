using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TurnerSoftware.SitemapTools
{
	public class SitemapEntry
	{
		public Uri Location { get; set; }
		public DateTime? LastModified { get; set; }
		public ChangeFrequency? ChangeFrequency { get; set; }
		public decimal Priority { get; set; }

		public SitemapEntry()
		{
			Priority = 0.5M;
		}
	}
}
