<div align="center">

![Icon](images/icon.png)
# Sitemap Tools

A sitemap (sitemap.xml) querying and parsing library for .NET

![Build](https://img.shields.io/github/actions/workflow/status/TurnerSoftware/sitemaptools/build.yml?branch=main)
[![Codecov](https://img.shields.io/codecov/c/github/turnersoftware/sitemaptools/master.svg)](https://codecov.io/gh/TurnerSoftware/SitemapTools)
[![NuGet](https://img.shields.io/nuget/v/TurnerSoftware.SitemapTools.svg)](https://www.nuget.org/packages/TurnerSoftware.SitemapTools)
</div>

## Key features
- Parses both XML sitemaps and [sitemap index files](http://www.sitemaps.org/protocol.html#index)
- Handles GZ-compressed XML sitemaps
- Supports TXT sitemaps


## Licensing and Support

Sitemap Tools is licensed under the MIT license. It is free to use in personal and commercial projects.

There are [support plans](https://turnersoftware.com.au/support-plans) available that cover all active [Turner Software OSS projects](https://github.com/TurnerSoftware).
Support plans provide private email support, expert usage advice for our projects, priority bug fixes and more.
These support plans help fund our OSS commitments to provide better software for everyone.


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