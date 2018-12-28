using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using TurnerSoftware.SitemapTools.Parser;
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
				{ "application/xml", SitemapType.Xml },
				{ "text/plain", SitemapType.Text }
			};
			SitemapParsers = new Dictionary<SitemapType, ISitemapParser>
			{
				{ SitemapType.Xml, new XmlSitemapParser() },
				{ SitemapType.Text, new TextSitemapParser() }
			};
		}

		private HttpClient HttpClient { get; }

		public SitemapQuery()
		{
			var clientHandler = new HttpClientHandler
			{
				AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate
			};

			HttpClient = new HttpClient(clientHandler);
		}

		public SitemapQuery(HttpClient client)
		{
			HttpClient = client;
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

			//TODO: Need to release new version of RobotsParser with ability to pass a HttpClient
			//var robotsFile = await new RobotsParser().FromUriAsync(baseUri);
			//sitemapUris.AddRange(robotsFile.SitemapEntries.Select(s => s.Sitemap));
			//sitemapUris = sitemapUris.Distinct().ToList();
			
			var result = new HashSet<Uri>();
			foreach (var uri in sitemapUris)
			{
				try
				{
					//We perform a head request because we don't care about the content here
					var requestMessage = new HttpRequestMessage(HttpMethod.Head, uri);
					var response = await HttpClient.SendAsync(requestMessage);

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

			return result;
		}
		
		public async Task<SitemapFile> GetSitemap(Uri sitemapUrl)
		{
			try
			{
				var response = await HttpClient.GetAsync(sitemapUrl);
				
				if (response.IsSuccessStatusCode)
				{
					var contentType = response.Content.Headers.ContentType.MediaType;
					var requiresManualDecompression = false;

					if (contentType.Equals("application/x-gzip", StringComparison.InvariantCultureIgnoreCase))
					{
						requiresManualDecompression = true;
						var baseFileName = Path.GetFileNameWithoutExtension(sitemapUrl.AbsolutePath);
						contentType = MimeTypes.GetMimeType(baseFileName);
					}
					
					if (SitemapTypeMapping.ContainsKey(contentType))
					{
						var sitemapType = SitemapTypeMapping[contentType];
						if (SitemapParsers.ContainsKey(sitemapType))
						{
							var reader = SitemapParsers[sitemapType];

							using (var stream = await response.Content.ReadAsStreamAsync())
							{
								var contentStream = stream;
								if (requiresManualDecompression)
								{
									contentStream = new GZipStream(contentStream, CompressionMode.Decompress);
								}

								using (var streamReader = new StreamReader(contentStream))
								{
									return reader.ParseSitemap(streamReader);
								}
							}
						}
						else
						{
							throw new InvalidOperationException($"No sitemap readers for {sitemapType}");
						}
					}
					else
					{
						throw new InvalidOperationException($"Unknown sitemap content type {contentType}");
					}
				}

				return null;
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
