using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TurnerSoftware.SitemapTools
{
	/// <summary>
	/// The individual entry in a sitemap file.
	/// </summary>
	public class SitemapEntry
	{
		/// <summary>
		/// The location of the resource pointed towards by the sitemap file.
		/// </summary>
		public Uri Location { get; set; }
		/// <summary>
		/// The last modified time of the resource.
		/// </summary>
		public DateTime? LastModified { get; set; }
		/// <summary>
		/// The change frequency of the resource. This describes how often the resource is updated.
		/// </summary>
		public ChangeFrequency? ChangeFrequency { get; set; }
		/// <summary>
		/// The priority of this resource. Default value is 0.5.
		/// </summary>
		public double Priority { get; set; }

		public SitemapEntry()
		{
			Priority = 0.5;
		}
	}
}
