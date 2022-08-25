using System;

namespace TurnerSoftware.SitemapTools;

public static class Constants
{
	public const string DefaultSitemapFilename = "sitemap.xml";

	private static bool CaseInsensitiveEquality(string x, string y) => x.Equals(y, StringComparison.OrdinalIgnoreCase);

	public static class ChangeFrequency
	{
		public const string Always = "always";
		public const string Hourly = "hourly";
		public const string Daily = "daily";
		public const string Weekly = "weekly";
		public const string Monthly = "monthly";
		public const string Yearly = "yearly";
		public const string Never = "never";

		/// <summary>
		/// Converts a change frequency <see cref="string"/> into a <see cref="SitemapTools.ChangeFrequency"/>.
		/// </summary>
		/// <param name="changeFrequency">The change frequency to parse.</param>
		/// <returns>A <see cref="SitemapTools.ChangeFrequency"/> if successful; otherwise <see langword="null"/>.</returns>
		public static SitemapTools.ChangeFrequency? ToEnum(string changeFrequency)
		{
			return changeFrequency switch
			{
				_ when CaseInsensitiveEquality(Always, changeFrequency) => SitemapTools.ChangeFrequency.Always,
				_ when CaseInsensitiveEquality(Hourly, changeFrequency) => SitemapTools.ChangeFrequency.Hourly,
				_ when CaseInsensitiveEquality(Daily, changeFrequency) => SitemapTools.ChangeFrequency.Daily,
				_ when CaseInsensitiveEquality(Weekly, changeFrequency) => SitemapTools.ChangeFrequency.Weekly,
				_ when CaseInsensitiveEquality(Monthly, changeFrequency) => SitemapTools.ChangeFrequency.Monthly,
				_ when CaseInsensitiveEquality(Yearly, changeFrequency) => SitemapTools.ChangeFrequency.Yearly,
				_ when CaseInsensitiveEquality(Never, changeFrequency) => SitemapTools.ChangeFrequency.Never,
				_ => null
			};
		}
	}
}
