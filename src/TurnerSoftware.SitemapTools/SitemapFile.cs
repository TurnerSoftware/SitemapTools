using System;
using System.Collections.Generic;

namespace TurnerSoftware.SitemapTools;

/// <summary>
/// Represents a sitemap that can contain references to other sitemaps or the sitemap entries themselves.
/// </summary>
/// <param name="Location">The sitemap location.</param>
/// <param name="Sitemaps">List of additional sitemaps.</param>
/// <param name="Urls">List of sitemap entries.</param>
public record SitemapFile(Uri Location, IReadOnlyList<SitemapIndexEntry> Sitemaps, IReadOnlyList<SitemapEntry> Urls)
{
	/// <summary>
	/// Create a sitemap file with no references to other sitemaps or any sitemap entries.
	/// </summary>
	/// <param name="location">The sitemap location</param>
	public SitemapFile(Uri location) : this(location, Array.Empty<SitemapIndexEntry>(), Array.Empty<SitemapEntry>()) { }
}

/// <summary>
/// The individual entry in a sitemap file.
/// </summary>
/// <param name="Location">The location of the resource pointed towards by the sitemap file.</param>
/// <param name="LastModified">The last modified time of the resource.</param>
/// <param name="ChangeFrequency">The change frequency of the resource. This describes how often the resource is updated.</param>
/// <param name="Priority">The priority of this resource. Default value is <c>0.5</c>.</param>
public record SitemapEntry(Uri Location, DateTime? LastModified, ChangeFrequency? ChangeFrequency, double Priority = SitemapEntry.DefaultPriority)
{
	/// <summary>
	/// The default priority for a <see cref="SitemapEntry"/>.
	/// </summary>
	public const double DefaultPriority = 0.5;

	/// <summary>
	/// Creates a <see cref="SitemapEntry"/> with the specified location.
	/// </summary>
	/// <param name="location">The location of the resource pointed towards by the sitemap file.</param>
	public SitemapEntry(Uri location) : this(location, default, default, DefaultPriority) { }
}

/// <summary>
/// A sitemap entry that points to another sitemap.
/// </summary>
/// <param name="Location">The location of the sitemap.</param>
/// <param name="LastModified">The last modified time of the sitemap.</param>
public record SitemapIndexEntry(Uri Location, DateTime? LastModified)
{
	/// <summary>
	/// Creates a <see cref="SitemapIndexEntry"/> with the specified location.
	/// </summary>
	/// <param name="location">The location of the sitemap.</param>
	public SitemapIndexEntry(Uri location) : this(location, default) { }
}
