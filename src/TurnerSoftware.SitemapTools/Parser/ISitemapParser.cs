﻿using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace TurnerSoftware.SitemapTools.Parser
{
	public interface ISitemapParser
	{
		Task<SitemapFile> ParseSitemapAsync(TextReader reader, CancellationToken cancellationToken = default);
	}
}
