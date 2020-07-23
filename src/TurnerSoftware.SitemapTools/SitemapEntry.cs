using System;

namespace TurnerSoftware.SitemapTools
{
	/// <summary>
	/// The individual entry in a sitemap file.
	/// </summary>
	public class SitemapEntry: IEquatable<SitemapEntry>, IEquatable<Uri>
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

		public override int GetHashCode()
		{
			if (ReferenceEquals(this, null))
				return default(Uri).GetHashCode();
			return Location.GetHashCode();
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(this, obj))
				return true;
		
			{
				if (ReferenceEquals(this, null))
					return (obj is SitemapEntry other) && other.Location == null;
			}

			if (ReferenceEquals(obj, null))
				return Location == null;

			{
				if (obj is SitemapEntry other)
					return Location == other.Location;
			}

			{
				if (obj is Uri other)
					return Location == other;
			}
			return false;
		}

		public bool Equals(SitemapEntry other) => this == other;

		public bool Equals(Uri other) => this == other;

		public static bool operator ==(SitemapEntry x, SitemapEntry y) => x?.Location == y?.Location;

		public static bool operator !=(SitemapEntry x, SitemapEntry y) => !(x == y);

		public static bool operator ==(SitemapEntry x, Uri y) => x?.Location == y;

		public static bool operator !=(SitemapEntry x, Uri y) => !(x == y);

		public static bool operator ==(Uri x, SitemapEntry y) => y == x;

		public static bool operator !=(Uri x, SitemapEntry y) => !(y == x);

		#endregion
	}
}
