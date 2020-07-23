using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TurnerSoftware.SitemapTools.Tests
{
	[TestClass]
	public class SitemapEntryTests
	{
		[TestMethod]
		public void Equals_EqualSitemaps()
		{
			var sameReferenceSitemap = new SitemapEntry();
			var x = new SitemapEntry
			{
				Location = new Uri("https://localhost/"),
				LastModified = new DateTime(2000, 1, 1),
				Priority = 0.7
			};
			var y = new SitemapEntry
			{
				Location = new Uri("https://localhost/"),
				LastModified = new DateTime(1970, 1, 1),
				Priority = 0.3
			};

#pragma warning disable CS1718 // Comparison made to same variable
			Assert.IsTrue(sameReferenceSitemap.Equals(sameReferenceSitemap));
#pragma warning restore CS1718 // Comparison made to same variable
			Assert.IsTrue(new SitemapEntry().Equals(new SitemapEntry()));
			Assert.IsTrue(x.Equals(y));
			Assert.IsTrue(y.Equals(x));
		}

		[TestMethod]
		public void Equals_NotEqualSitemaps()
		{
			var x = new SitemapEntry
			{
				Location = new Uri("https://localhost/abc")
			};
			var y = new SitemapEntry
			{
				Location = new Uri("https://localhost/def")
			};

			Assert.IsFalse(new SitemapEntry().Equals((SitemapEntry)null));
			Assert.IsFalse(x.Equals(y));
			Assert.IsFalse(y.Equals(x));
		}

		[TestMethod]
		public void EqualsOperator_EqualSitemaps()
		{
			var sameReferenceSitemap = new SitemapEntry();
			var x = new SitemapEntry
			{
				Location = new Uri("https://localhost/"),
				LastModified = new DateTime(2000, 1, 1),
				Priority = 0.7
			};
			var y = new SitemapEntry
			{
				Location = new Uri("https://localhost/"),
				LastModified = new DateTime(1970, 1, 1),
				Priority = 0.3
			};

#pragma warning disable CS1718 // Comparison made to same variable
			Assert.IsTrue(sameReferenceSitemap == sameReferenceSitemap);
#pragma warning restore CS1718 // Comparison made to same variable
			Assert.IsTrue(new SitemapEntry() == new SitemapEntry());
			Assert.IsTrue(x == y);
			Assert.IsTrue(y == x);
		}

		[TestMethod]
		public void EqualsOperator_NotEqualSitemaps()
		{
			var x = new SitemapEntry
			{
				Location = new Uri("https://localhost/abc")
			};
			var y = new SitemapEntry
			{
				Location = new Uri("https://localhost/def")
			};

			Assert.IsFalse((SitemapEntry)null == new SitemapEntry());
			Assert.IsFalse(new SitemapEntry() == (SitemapEntry)null);
			Assert.IsFalse(x == y);
			Assert.IsFalse(y == x);
		}

		[TestMethod]
		public void NotEqualsOperator_EqualSitemaps()
		{
			var sameReferenceSitemap = new SitemapEntry();
			var x = new SitemapEntry
			{
				Location = new Uri("https://localhost/"),
				LastModified = new DateTime(2000, 1, 1),
				Priority = 0.7
			};
			var y = new SitemapEntry
			{
				Location = new Uri("https://localhost/"),
				LastModified = new DateTime(1970, 1, 1),
				Priority = 0.3
			};

#pragma warning disable CS1718 // Comparison made to same variable
			Assert.IsFalse(sameReferenceSitemap != sameReferenceSitemap);
#pragma warning restore CS1718 // Comparison made to same variable
			Assert.IsFalse(new SitemapEntry() != new SitemapEntry());
			Assert.IsFalse(x != y);
			Assert.IsFalse(y != x);
		}

		[TestMethod]
		public void NotEqualsOperator_NotEqualSitemaps()
		{
			var x = new SitemapEntry
			{
				Location = new Uri("https://localhost/abc")
			};
			var y = new SitemapEntry
			{
				Location = new Uri("https://localhost/def")
			};

			Assert.IsTrue((SitemapEntry)null != new SitemapEntry());
			Assert.IsTrue(new SitemapEntry() != (SitemapEntry)null);
			Assert.IsTrue(x != y);
			Assert.IsTrue(y != x);
		}

		[TestMethod]
		public void Equals_EqualSitemapAndUri()
		{
			var x = new SitemapEntry
			{
				Location = new Uri("https://localhost/"),
				LastModified = new DateTime(2000, 1, 1),
				Priority = 0.7
			};
			var y = new Uri("https://localhost/");

			Assert.IsTrue(x.Equals(y));
		}

		[TestMethod]
		public void Equals_NotEqualSitemapAndUri()
		{
			var x = new SitemapEntry
			{
				Location = new Uri("https://localhost/abc")
			};
			var y = new Uri("https://localhost/def");

			Assert.IsFalse(x.Equals(y));
			Assert.IsFalse(x.Equals((Uri)null));
		}

		[TestMethod]
		public void EqualsOperator_EqualSitemapAndUri()
		{
			var x = new SitemapEntry
			{
				Location = new Uri("https://localhost/"),
				LastModified = new DateTime(2000, 1, 1),
				Priority = 0.7
			};
			var y = new Uri("https://localhost/");
			
			Assert.IsTrue(x == y);
			Assert.IsTrue(y == x);
		}

		[TestMethod]
		public void EqualsOperator_NotEqualSitemapAndUri()
		{
			var x = new SitemapEntry
			{
				Location = new Uri("https://localhost/abc")
			};
			var y = new Uri("https://localhost/def");

			Assert.IsFalse((Uri)null == x);
			Assert.IsFalse(x == (Uri)null);
			Assert.IsFalse((SitemapEntry)null == new Uri("https://localhost/"));
			Assert.IsFalse(new Uri("https://localhost/") == (SitemapEntry)null);
			Assert.IsFalse(x == y);
			Assert.IsFalse(y == x);
		}

		[TestMethod]
		public void NotEqualsOperator_EqualSitemapAndUri()
		{
			var x = new SitemapEntry
			{
				Location = new Uri("https://localhost/"),
				LastModified = new DateTime(2000, 1, 1),
				Priority = 0.7
			};
			var y = new Uri("https://localhost/");

			Assert.IsFalse(x != y);
			Assert.IsFalse(y != x);
		}

		[TestMethod]
		public void NotEqualsOperator_NotEqualSitemapAndUri()
		{
			var x = new SitemapEntry
			{
				Location = new Uri("https://localhost/abc")
			};
			var y = new Uri("https://localhost/def");

			Assert.IsTrue((Uri)null != x);
			Assert.IsTrue(x != (Uri)null);
			Assert.IsTrue((SitemapEntry)null != new Uri("https://localhost/"));
			Assert.IsTrue(new Uri("https://localhost/") != (SitemapEntry)null);
			Assert.IsTrue(x != y);
			Assert.IsTrue(y != x);
		}
	}
}
