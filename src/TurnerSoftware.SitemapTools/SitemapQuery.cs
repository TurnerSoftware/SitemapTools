using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using TurnerSoftware.SitemapTools.Reader;
using System.Net.Http;
using TurnerSoftware.RobotsExclusionTools;

namespace TurnerSoftware.SitemapTools
{
	public class SitemapQuery
	{
		public static Dictionary<string, SitemapType> SitemapTypeMapping { get; }
		public static Dictionary<SitemapType, ISitemapParser> SitemapParsers { get; }

		static SitemapQuery()
		{
			SitemapTypeMapping = new Dictionary<string, SitemapType>
			{
				{ "text/xml", SitemapType.Xml },
				{ "application/xml", SitemapType.Xml }
			};
			SitemapParsers = new Dictionary<SitemapType, ISitemapParser>
			{
				{ SitemapType.Xml, new XmlSitemapParser() }
			};
		}
		
		public async Task<IEnumerable<Uri>> DiscoverSitemaps(string domainName)
		{
			var uriBuilder = new UriBuilder("http", domainName);
			var baseUri = uriBuilder.Uri;

			uriBuilder.Path = "sitemap.xml";
			var defaultSitemapUri = uriBuilder.Uri;

			var sitemapUris = new List<Uri>
			{
				defaultSitemapUri
			};

			var robotsFile = await new RobotsParser().FromUriAsync(baseUri);
			sitemapUris.AddRange(robotsFile.SitemapEntries.Select(s => s.Sitemap));
			sitemapUris = sitemapUris.Distinct().ToList();
			
			var result = new HashSet<Uri>();
			using (var httpClient = new HttpClient())
			{
				foreach (var uri in sitemapUris)
				{
					try
					{
						//We perform a head request because we don't care about the content here
						var requestMessage = new HttpRequestMessage(HttpMethod.Head, uri);
						var response = await httpClient.SendAsync(requestMessage);
						
						if (response.IsSuccessStatusCode)
						{
							result.Add(uri);
						}
					}
					catch (WebException ex)
					{
						if (ex.Response != null)
						{
							continue;
						}
						
						throw;
					}
				}
			}

			return result;
		}
		
		public async Task<SitemapFile> GetSitemap(Uri sitemapUrl)
		{
			var request = WebRequest.CreateHttp(sitemapUrl);
			request.AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip;

			try
			{
				using (var response = await request.GetResponseAsync())
				using (var responseStream = response.GetResponseStream())
				using (var streamReader = new StreamReader(responseStream))
				{
					if (SitemapTypeMapping.ContainsKey(response.ContentType))
					{
						var sitemapType = SitemapTypeMapping[response.ContentType];
						if (SitemapParsers.ContainsKey(sitemapType))
						{
							var reader = SitemapParsers[sitemapType];
							return reader.ParseSitemap(streamReader);
						}
						else
						{
							throw new InvalidOperationException($"No sitemap readers for {sitemapType}");
						}
					}
					else
					{
						throw new InvalidOperationException($"Unknown sitemap content type {response.ContentType}");
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

		public async Task<IEnumerable<SitemapFile>> GetAllSitemapsForDomain(string domainName)
		{
			var sitemapFiles = new Dictionary<Uri, SitemapFile>();
			var sitemapsUris = new Stack<Uri>(await DiscoverSitemaps(domainName));

			while (sitemapsUris.Count > 0)
			{
				var sitemapUri = sitemapsUris.Pop();
				
				if (!sitemapFiles.ContainsKey(sitemapUri))
				{
					var sitemapFile = await GetSitemap(sitemapUri);
					sitemapFiles.Add(sitemapUri, sitemapFile);

					foreach (var indexFile in sitemapFile.Sitemaps)
					{
						sitemapsUris.Push(indexFile.Location);
					}
				}
			}

			return sitemapFiles.Values;
		}
	}
}
