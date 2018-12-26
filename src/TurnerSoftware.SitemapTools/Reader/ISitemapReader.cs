using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TurnerSoftware.SitemapTools.Reader
{
	interface ISitemapReader
	{
		SitemapFile ParseSitemap(string rawSitemap);
	}
}
