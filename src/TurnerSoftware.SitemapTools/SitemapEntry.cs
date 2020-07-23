using System;

namespace TurnerSoftware.SitemapTools
{
	/// <summary>
	/// The individual entry in a sitemap file.
	/// </summary>
	public class SitemapEntry : IEquatable<SitemapEntry>, IEquatable<Uri>
	{
		/// <summary>
		/// The location of the resource pointed towards by the sitemap file.
		/// </summary>
		public Uri Location { get; set; }
		/// <summary>
		/// The last modified time of the resource.
		/// </summary>
		public DateTime? LastModified { get; set; }
		/// <summary>
		/// The change frequency of the resource. This describes how often the resource is updated.
		/// </summary>
		public ChangeFrequency? ChangeFrequency { get; set; }
		/// <summary>
		/// The priority of this resource. Default value is 0.5.
		/// </summary>
		public double Priority { get; set; }

		public SitemapEntry()
		{
			Priority = 0.5;
		}

		#region Equality comparisons

		public override int GetHashCode() => Location?.GetHashCode() ?? base.GetHashCode();

		public override bool Equals(object obj)
		{
			if (obj is SitemapEntry sitemapEntry)
			{
				return Equals(sitemapEntry);
			}

			if (obj is Uri locationUri)
			{
				return Equals(locationUri);
			}

			return false;
		}

		public bool Equals(SitemapEntry other)
		{
			if (other is null)
			{
				return false;
			}

			if (ReferenceEquals(this, other))
			{
				return true;
			}

			return Location == other.Location;
		}

		public bool Equals(Uri other) => Location == other;

		public static bool operator ==(SitemapEntry x, SitemapEntry y) => !(x is null) ? x.Equals(y) : y is null;

		public static bool operator !=(SitemapEntry x, SitemapEntry y) => !(x == y);

		public static bool operator ==(SitemapEntry x, Uri y) => !(x is null) ? x.Equals(y) : y is null;

		public static bool operator !=(SitemapEntry x, Uri y) => !(x == y);

		public static bool operator ==(Uri x, SitemapEntry y) => y == x;

		public static bool operator !=(Uri x, SitemapEntry y) => !(y == x);

		#endregion
	}
}
