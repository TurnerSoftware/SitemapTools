using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace TurnerSoftware.SitemapTools.Parser;

public interface ISitemapParser
{
	Task<SitemapFile?> ParseSitemapAsync(Uri sitemapUrl, TextReader reader, CancellationToken cancellationToken = default);
}
