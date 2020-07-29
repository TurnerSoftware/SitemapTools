using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.IO;
using TurnerSoftware.SitemapTools.Parser;
using System.Net.Http;
using TurnerSoftware.RobotsExclusionTools;
using System.Threading;

namespace TurnerSoftware.SitemapTools
{
	public class SitemapQuery
	{
		/// <summary>
		/// HTTP content type mapping against <see cref="SitemapType"/>.
		/// </summary>
		public static Dictionary<string, SitemapType> SitemapTypeMapping { get; }
		/// <summary>
		/// <see cref="SitemapType"/> mapping against <see cref="ISitemapParser"/>.
		/// </summary>
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

		/// <summary>
		/// Creates a <see cref="SitemapQuery"/> with a <see cref="global::System.Net.Http.HttpClient"/> configured
		/// for automatic decompression.
		/// </summary>
		public SitemapQuery()
		{
			var clientHandler = new HttpClientHandler
			{
				AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate
			};

			HttpClient = new HttpClient(clientHandler);
		}

		/// <summary>
		/// Creates a <see cref="SitemapQuery"/> with the provided <see cref="global::System.Net.Http.HttpClient"/>.
		/// </summary>
		/// <param name="client"></param>
		public SitemapQuery(HttpClient client)
		{
			HttpClient = client;
		}

		/// <summary>
		/// Some sites does not request on <see cref="global::System.Net.Http.HttpMethod.Head"/> so execute for them <see cref="global::System.Net.Http.HttpMethod.Get"/> request.
		/// </summary>
		public bool IsHeadMethodUnsupported { get; set; }

		/// <summary>
		/// Discovers available sitemaps for a given domain name, returning a list of sitemap URIs discovered.
		/// The sitemaps are discovered from a combination of the site root and looking through the robots.txt file.
		/// </summary>
		/// <param name="domainName">The domain name to search</param>
		/// <returns>List of found sitemap URIs</returns>
		public async Task<IEnumerable<Uri>> DiscoverSitemapsAsync(string domainName, CancellationToken cancellationToken = default)
		{
			var uriBuilder = new UriBuilder("http", domainName);
			var baseUri = uriBuilder.Uri;

			uriBuilder.Path = "sitemap.xml";
			var defaultSitemapUri = uriBuilder.Uri;

			var sitemapUris = new List<Uri>
			{
				defaultSitemapUri
			};
			
			var robotsFile = await new RobotsFileParser(HttpClient).FromUriAsync(baseUri, cancellationToken);
			
			sitemapUris.AddRange(robotsFile.SitemapEntries.Select(s => s.Sitemap));
			sitemapUris = sitemapUris.Distinct().ToList();
			
			var result = new HashSet<Uri>();
			foreach (var uri in sitemapUris)
			{
				cancellationToken.ThrowIfCancellationRequested();

				try
				{
					var requestMessage = new HttpRequestMessage(IsHeadMethodUnsupported? HttpMethod.Get : HttpMethod.Head, uri);
					var response = await HttpClient.SendAsync(requestMessage, cancellationToken);

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
		
		/// <summary>
		/// Retrieves a sitemap at the given URI, converting it to a <see cref="SitemapFile"/>.
		/// </summary>
		/// <param name="sitemapUrl">The URI where the sitemap exists.</param>
		/// <returns>The found and converted <see cref="SitemapFile"/></returns>
		public async Task<SitemapFile> GetSitemapAsync(Uri sitemapUrl, CancellationToken cancellationToken = default)
		{
			try
			{
				var response = await HttpClient.GetAsync(sitemapUrl, cancellationToken);
				
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
							var parser = SitemapParsers[sitemapType];

							using (var stream = await response.Content.ReadAsStreamAsync())
							{
								cancellationToken.ThrowIfCancellationRequested();
								var contentStream = stream;
								if (requiresManualDecompression)
								{
									contentStream = new GZipStream(contentStream, CompressionMode.Decompress);
								}

								using (var streamReader = new StreamReader(contentStream))
								{
									var sitemap = await parser.ParseSitemapAsync(streamReader, cancellationToken);
									if (sitemap != null)
									{
										sitemap.Location = sitemapUrl;
									}
									return sitemap;
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
		
		/// <summary>
		/// Retrieves all sitemaps for a given domain. This effectively combines <see cref="DiscoverSitemapsAsync(string)"/> and 
		/// <see cref="GetSitemapAsync(Uri)"/> while additionally finding any other sitemaps described in sitemap index files.
		/// </summary>
		/// <param name="domainName"></param>
		/// <returns></returns>
		public async Task<IEnumerable<SitemapFile>> GetAllSitemapsForDomainAsync(string domainName, CancellationToken cancellationToken = default)
		{
			var sitemapFiles = new Dictionary<Uri, SitemapFile>();
			var sitemapUris = new Stack<Uri>(await DiscoverSitemapsAsync(domainName, cancellationToken));

			while (sitemapUris.Count > 0)
			{
				var sitemapUri = sitemapUris.Pop();

				var sitemapFile = await GetSitemapAsync(sitemapUri, cancellationToken);
				sitemapFiles.Add(sitemapUri, sitemapFile);

				foreach (var indexFile in sitemapFile.Sitemaps)
				{
					if (!sitemapFiles.ContainsKey(indexFile.Location))
					{
						sitemapUris.Push(indexFile.Location);
					}
				}
			}

			return sitemapFiles.Values;
		}
	}
}
