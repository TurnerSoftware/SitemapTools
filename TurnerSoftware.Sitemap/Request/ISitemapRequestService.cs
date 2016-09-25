using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TurnerSoftware.Sitemap.Request
{
    public interface ISitemapRequestService
    {
        IEnumerable<Uri> GetAvailableSitemapsForDomain(string domainName);
        string RetrieveRawSitemap(Uri sitemapLocation);
    }
}
