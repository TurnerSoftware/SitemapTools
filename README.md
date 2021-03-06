# Sitemap Tools

A sitemap (sitemap.xml) querying and parsing library in C#

[![AppVeyor](https://img.shields.io/appveyor/ci/Turnerj/sitemaptools/master.svg)](https://ci.appveyor.com/project/Turnerj/sitemaptools)
[![Codecov](https://img.shields.io/codecov/c/github/turnersoftware/sitemaptools/master.svg)](https://codecov.io/gh/TurnerSoftware/SitemapTools)
[![NuGet](https://img.shields.io/nuget/v/TurnerSoftware.SitemapTools.svg)](https://www.nuget.org/packages/TurnerSoftware.SitemapTools)

## Key features
- Parses both XML sitemaps and [sitemap index files](http://www.sitemaps.org/protocol.html#index)
- Handles GZ-compressed XML sitemaps
- Supports TXT sitemaps

## Notes
- Does not enforce sitemap standards [as described at sitemaps.org](http://www.sitemaps.org/protocol.html)
- Does not validate the sitemaps
- Does not support RSS sitemaps

## Example
```csharp
using TurnerSoftware.SitemapTools;

var sitemapQuery = new SitemapQuery();
var sitemapEntries = await sitemapQuery.GetAllSitemapsForDomainAsync("example.org");
```