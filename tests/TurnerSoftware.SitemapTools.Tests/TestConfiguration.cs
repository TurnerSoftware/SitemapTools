using System.Net.Http;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using TurnerSoftware.SitemapTools.Tests.Server;

namespace TurnerSoftware.SitemapTools.Tests;

static class TestConfiguration
{
	private static TestServer Server { get; set; }

	private static HttpClient Client { get; set; }
	public static HttpClient GetHttpClient()
	{
		if (Client == null)
		{
			Client = Server.CreateClient();
		}
		return Client;
	}

	public static void StartupServer()
	{
		if (Server != null)
		{
			return;
		}

		var builder = new WebHostBuilder()
			.UseStartup<Startup>();

		Server = new TestServer(builder);
	}

	public static void ShutdownServer()
	{
		Server.Dispose();
	}
}
