using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TurnerSoftware.SitemapTools.Reader
{
	public interface ISitemapParser
	{
		SitemapFile ParseSitemap(TextReader reader);
	}
}
