using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace TurnerSoftware.SitemapTools.Request
{
	public class SitemapRequestService : ISitemapRequestService
	{
		public IEnumerable<Uri> GetAvailableSitemapsForDomain(string domainName)
		{
			var httpDefaultSitemap = new UriBuilder("http", domainName)
			{
				Path = "sitemap.xml"
			}.Uri.ToString();
			var httpsDefaultSitemap = new UriBuilder("https", domainName)
			{
				Path = "sitemap.xml"
			}.Uri.ToString();

			var sitemapFilePaths = new[] { httpDefaultSitemap, httpsDefaultSitemap };

			//Parse each of the paths and check that the file exists
			var result = new List<Uri>();
			using (var httpClient = new HttpClient())
			{
				foreach (var sitemapPath in sitemapFilePaths)
				{
					try
					{
						if (Uri.TryCreate(sitemapPath, UriKind.Absolute, out Uri tmpUri))
						{
							//We perform a head request because we don't care about the content here
							var requestMessage = new HttpRequestMessage(HttpMethod.Head, tmpUri);
							var response = httpClient.SendAsync(requestMessage).Result;

							//If it is successful, add to our results list
							if (response.IsSuccessStatusCode)
							{
								result.Add(tmpUri);
							}
						}
					}
					catch (WebException ex)
					{
						//If it throws an exception but we have a response, just skip the sitemap
						if (ex.Response != null)
						{
							continue;
						}

						//If no response, throw the exception up
						throw;
					}
				}
			}
			return result;
		}

		public string RetrieveRawSitemap(Uri sitemapLocation)
		{
			var request = WebRequest.Create(sitemapLocation);
			
			try
			{
				using (var response = request.GetResponse())
				using (var responseStream = response.GetResponseStream())
				{
					var stream = responseStream;

					//If the path looks like it is GZipped, automatically decompress it
					if (sitemapLocation.AbsolutePath.Contains(".gz"))
					{
						stream = new GZipStream(stream, CompressionMode.Decompress);
					}

					using (var streamReader = new StreamReader(stream))
					{
						var result = streamReader.ReadToEnd();
						return result;
					}
				}
			}
			catch (WebException ex)
			{
				if (ex.Response != null)
				{
					return null;
				}

				throw;
			}
		}
	}
}
