using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TurnerSoftware.Sitemap
{
    public class SitemapFetchOptions
    {
        /// <summary>
        /// When true, any sitemaps specified in a sitemap index will be fetched. Default is true.
        /// </summary>
        public bool FetchInnerSitemaps { get; set; }
        /// <summary>
        /// When true, only sitemap entries that have the same domain as the sitemap itself will be in the results. Default is true.
        /// </summary>
        public bool ApplyDomainRestrictions { get; set; }
        /// <summary>
        /// When true, if the type of sitemap is unknown, it will throw a <see cref="NotSupportedException" />. Default is true.
        /// </summary>
        public bool ThrowExceptionOnUnknownType { get; set; }
        /// <summary>
        /// When true, flattens all found sitemaps from a sitemap index into a master sitemap. Default is false.
        /// </summary>
        public bool FlattenSitemap { get; set; }

        public SitemapFetchOptions()
        {
            FetchInnerSitemaps = true;
            DecompressGZipSitemaps = true;
            ApplyDomainRestrictions = true;
            ThrowExceptionOnUnknownType = true;
            FlattenSitemap = false;
        }
    }
}
