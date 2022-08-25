using System.IO;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.FileProviders;

namespace TurnerSoftware.SitemapTools.Tests.Server;

public class Startup
{
	public void Configure(IApplicationBuilder app)
	{
		app.UseStaticFiles(new StaticFileOptions
		{
			FileProvider = new PhysicalFileProvider(
				Path.Combine(Directory.GetCurrentDirectory(), "Resources"))
		});
	}
}
