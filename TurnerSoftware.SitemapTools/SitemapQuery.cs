using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using TurnerSoftware.SitemapTools.Request;
using TurnerSoftware.SitemapTools.Reader;

namespace TurnerSoftware.SitemapTools
{
	public class SitemapQuery
	{
		private ISitemapRequestService requestService { get; set; }

		public SitemapQuery() : this(new SitemapRequestService()) { }
		public SitemapQuery(ISitemapRequestService requestService)
		{
			this.requestService = requestService;
		}

		/// <summary>
		/// Finds the available sitemaps for the domain, retrieving each sitemap.
		/// </summary>
		/// <param name="domain"></param>
		/// <returns></returns>
		public IEnumerable<SitemapFile> RetrieveSitemapsForDomain(string domainName)
		{
			return RetrieveSitemapsForDomain(domainName, new SitemapFetchOptions());
		}

		/// <summary>
		/// Finds the available sitemaps for the domain, retrieving each sitemap with the specified fetch options.
		/// </summary>
		/// <param name="domain"></param>
		/// <returns></returns>
		public IEnumerable<SitemapFile> RetrieveSitemapsForDomain(string domainName, SitemapFetchOptions options)
		{
			var sitemapLocations = requestService.GetAvailableSitemapsForDomain(domainName);
			var result = new List<SitemapFile>();

			foreach (var location in sitemapLocations)
			{
				var tmpSitemap = RetrieveSitemap(location, options);
				if (tmpSitemap != null)
				{
					result.Add(tmpSitemap);
				}
			}

			return result;
		}

		/// <summary>
		/// Retrieves a Sitemap from the specified location.
		/// </summary>
		/// <param name="sitemapLocation"></param>
		/// <returns></returns>
		public SitemapFile RetrieveSitemap(Uri sitemapLocation)
		{
			return RetrieveSitemap(sitemapLocation, new SitemapFetchOptions());
		}
		/// <summary>
		/// Retrieves a Sitemap from the specified location.
		/// </summary>
		/// <param name="sitemapLocationString"></param>
		/// <returns></returns>
		public SitemapFile RetrieveSitemap(string sitemapLocationString)
		{
			var sitemapLocation = new Uri(sitemapLocationString);
			return RetrieveSitemap(sitemapLocation, new SitemapFetchOptions());
		}

		/// <summary>
		/// Retrieves a Sitemap from the specified location with the specified fetch options.
		/// </summary>
		/// <param name="sitemapLocation"></param>
		/// <param name="options"></param>
		/// <returns></returns>
		public SitemapFile RetrieveSitemap(Uri sitemapLocation, SitemapFetchOptions options)
		{
			var type = GetSitemapType(sitemapLocation);

			//Perform sitemap type-check
			if (type == SitemapType.Unknown)
			{
				if (options.ThrowExceptionOnUnknownType)
				{
					throw new NotSupportedException("Specified sitemap is unsupported!");
				}
				else
				{
					return null;
				}
			}

			var rawSitemap = requestService.RetrieveRawSitemap(sitemapLocation);
			var parsedSitemap = ParseSitemap(type, rawSitemap);

			if (parsedSitemap == null)
			{
				return null;
			}

			//Set the location of the parsed sitemap
			parsedSitemap.Location = sitemapLocation;

			if (options.ApplyDomainRestrictions)
			{
				var validEntries = new List<SitemapEntry>();

				//For every entry, check the host matches the sitemap it is specified in
				foreach (var entry in parsedSitemap.Urls)
				{
					if (entry.Location.Host == sitemapLocation.Host)
					{
						validEntries.Add(entry);
					}
				}

				parsedSitemap.Urls = validEntries;
			}

			if (options.FetchInnerSitemaps)
			{
				var fetchedInnerSitemaps = new List<SitemapFile>();

				//For every sitemap index, fetch the sitemap
				foreach (var indexedSitemap in parsedSitemap.Sitemaps)
				{
					var tmpInnerSitemap = RetrieveSitemap(indexedSitemap.Location, options);

					//Copy over the last modified from the sitemap index
					tmpInnerSitemap.LastModified = indexedSitemap.LastModified;

					fetchedInnerSitemaps.Add(tmpInnerSitemap);
				}

				parsedSitemap.Sitemaps = fetchedInnerSitemaps;
			}

			return parsedSitemap;
		}

		/// <summary>
		/// Parse a sitemap with the <see cref="SitemapType"/> specified. 
		/// </summary>
		/// <param name="type"></param>
		/// <param name="rawSitemap"></param>
		/// <returns></returns>
		public SitemapFile ParseSitemap(SitemapType type, string rawSitemap)
		{
			if (rawSitemap == null)
			{
				return null;
			}

			ISitemapReader reader;
			if (type == SitemapType.Xml)
			{
				reader = new XmlSitemapReader();
				return reader.ParseSitemap(rawSitemap);
			}
			else
			{
				return null;
			}
		}

		/// <summary>
		/// Flattens a list of sitemaps, taking all of the sitemap entries and combining into a single list. 
		/// </summary>
		/// <param name="sitemaps"></param>
		/// <returns></returns>
		public IEnumerable<SitemapEntry> FlattenSitemaps(IEnumerable<SitemapFile> sitemaps)
		{
			var sitemapEntries = new List<SitemapEntry>();

			foreach (var sitemap in sitemaps)
			{
				var currentEntries = new List<SitemapEntry>();

				if (sitemap.Sitemaps.Count() > 0)
				{
					//If there are inner sitemaps, grab their flattened sitemaps
					var flattenedEntries = FlattenSitemaps(sitemap.Sitemaps);
					currentEntries.AddRange(flattenedEntries);
				}

				if (sitemap.Urls != null)
				{
					currentEntries.AddRange(sitemap.Urls);
				}

				sitemapEntries.AddRange(currentEntries);
			}

			//De-dupe entries based on location
			var dedupedEntries = sitemapEntries
				.GroupBy(se => se.Location)
				.Select(g => g.FirstOrDefault());

			return dedupedEntries;
		}

		/// <summary>
		/// From a given sitemap location, return the type of sitemap file.
		/// </summary>
		/// <param name="sitemapLocation"></param>
		/// <returns></returns>
		public SitemapType GetSitemapType(Uri sitemapLocation)
		{
			var path = sitemapLocation.AbsolutePath;

			if (path.Contains(".xml") || path.Contains(".xml.gz"))
			{
				return SitemapType.Xml;
			}
			else
			{
				return SitemapType.Unknown;
			}
		}
	}
}
