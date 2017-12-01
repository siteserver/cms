/* RssPhotoAlbum.cs
 * ================
 * 
 * RSS.NET (http://rss-net.sf.net/)
 * Copyright ?2002, 2003 George Tsiokos. All Rights Reserved.
 * 
 * RSS 2.0 (http://blogs.law.harvard.edu/tech/rss)
 * RSS 2.0 is offered by the Berkman Center for Internet & Society at 
 * Harvard Law School under the terms of the Attribution/Share Alike 
 * Creative Commons license.
 * 
 * Photo Album 1.0 (http://www.innothinx.com)
 * Copyright ?2001-2003 Robert A. Wlodarczyk and Inno Thinx, LLC. All Rights Reserved.
 * 
 * Permission is hereby granted, free of charge, to any person obtaining 
 * a copy of this software and associated documentation files (the "Software"), 
 * to deal in the Software without restriction, including without limitation 
 * the rights to use, copy, modify, merge, publish, distribute, sublicense, 
 * and/or sell copies of the Software, and to permit persons to whom the 
 * Software is furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in all
 * copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR 
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, 
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL 
 * THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER 
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, 
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN 
 * THE SOFTWARE.
*/
using System;

namespace BaiRong.Core.Rss
{
	/// <summary>People in a photo</summary>
	public sealed class RssPhotoAlbumCategoryPhotoPeople : RssModuleItemCollection
	{
		/// <summary>Initialize a new instance of the RssPhotoAlbumItemPhotoPeople class</summary>
		public RssPhotoAlbumCategoryPhotoPeople()
		{
		}

		/// <summary>Initialize a new instance of the RssPhotoAlbumItemPhotoPeople class</summary>
		/// <param name="value">Name of person</param>
		public RssPhotoAlbumCategoryPhotoPeople(string value)
		{
			Add(value);
		}

		/// <summary>Add a person to the photo</summary>
		/// <param name="value">Name of person</param>
		/// <returns>The zero-based index of the added item</returns>
		public int Add(string value)
		{
			return base.Add(new RssModuleItem("person", true, value));
		}
	}


	/// <summary>A collection of photos in a category</summary>
	public sealed class RssPhotoAlbumCategoryPhotos : RssModuleItemCollectionCollection
	{
		/// <summary>Initialize a new instance of the RssPhotoAlbumItemPhoto class</summary>
		public RssPhotoAlbumCategoryPhotos()
		{
		}

		/// <summary>Adds a sepecified photo to this collection.</summary>
		/// <param name="photo">The photo to add.</param>
		/// <returns>The zero-based index of the added item.</returns>
		public int Add(RssPhotoAlbumCategoryPhoto photo)
		{
			return base.Add(photo);
		}
	}


	/// <summary>A photo in the category</summary>
	public sealed class RssPhotoAlbumCategoryPhoto : RssModuleItemCollection
	{
		/// <summary>Initialize a new instance of the RssPhotoAlbumItemPhoto class</summary>
		/// <param name="photoDate">Date of the Photo</param>
		/// <param name="photoDescription">Description of the photo.</param>
		/// <param name="photoLink">Direct link of the photo.</param>
		public RssPhotoAlbumCategoryPhoto(DateTime photoDate, string photoDescription, Uri photoLink)
		{
			Add(photoDate, photoDescription, photoLink);
		}

		/// <summary>Initialize a new instance of the RssPhotoAlbumItemPhoto class</summary>
		/// <param name="photoDate">Date of the Photo</param>
		/// <param name="photoDescription">Description of the photo.</param>
		/// <param name="photoPeople">People to add to the photo.</param>
		/// <param name="photoLink">Direct link of the photo.</param>
		public RssPhotoAlbumCategoryPhoto(DateTime photoDate, string photoDescription, Uri photoLink, RssPhotoAlbumCategoryPhotoPeople photoPeople)
		{
			Add(photoDate, photoDescription, photoLink, photoPeople);
		}

		/// <summary>Adds a specified item to this collection.</summary>
		/// <param name="photoDate">Date of the Photo</param>
		/// <param name="photoDescription">Description of the photo.</param>
		/// <param name="photoPeople">People to add to the photo.</param>
		/// <param name="photoLink">Direct link of the photo.</param>
		/// <returns>The zero-based index of the added item.</returns>
		private int Add(DateTime photoDate, string photoDescription, Uri photoLink, RssPhotoAlbumCategoryPhotoPeople photoPeople)
		{
			Add(photoDate, photoDescription, photoLink);
			base.Add(new RssModuleItem("photoPeople", true, "", photoPeople));
			return -1;
		}

		/// <summary>Adds a specified item to this collection.</summary>
		/// <param name="photoDate">Date of the Photo</param>
		/// <param name="photoDescription">Description of the photo.</param>
		/// <param name="photoLink">Direct link of the photo.</param>
		/// <returns>The zero-based index of the added item.</returns>
		private int Add(DateTime photoDate, string photoDescription, Uri photoLink)
		{
			base.Add(new RssModuleItem("photoDate", true, RssDefault.Check(photoDate.ToUniversalTime().ToString("r"))));
			base.Add(new RssModuleItem("photoDescription", false, RssDefault.Check(photoDescription)));
			base.Add(new RssModuleItem("photoLink", true, RssDefault.Check(photoLink).ToString()));
			return -1;
		}

		/// <summary>Initialize a new instance of the RssPhotoAlbumItemPhoto class</summary>
		/// <param name="photoDate">Date of the Photo</param>
		/// <param name="photoDescription">Description of the photo.</param>
		/// <param name="photoLink">Direct link of the photo.</param>
		public RssPhotoAlbumCategoryPhoto(string photoDate, string photoDescription, Uri photoLink)
		{
			Add(photoDate, photoDescription, photoLink);
		}

		/// <summary>Initialize a new instance of the RssPhotoAlbumItemPhoto class</summary>
		/// <param name="photoDate">Date of the Photo</param>
		/// <param name="photoDescription">Description of the photo.</param>
		/// <param name="photoPeople">People to add to the photo.</param>
		/// <param name="photoLink">Direct link of the photo.</param>
		public RssPhotoAlbumCategoryPhoto(string photoDate, string photoDescription, Uri photoLink, RssPhotoAlbumCategoryPhotoPeople photoPeople)
		{
			Add(photoDate, photoDescription, photoLink, photoPeople);
		}

		/// <summary>Adds a specified item to this collection.</summary>
		/// <param name="photoDate">Date of the Photo</param>
		/// <param name="photoDescription">Description of the photo.</param>
		/// <param name="photoPeople">People to add to the photo.</param>
		/// <param name="photoLink">Direct link of the photo.</param>
		/// <returns>The zero-based index of the added item.</returns>
		private int Add(string photoDate, string photoDescription, Uri photoLink, RssPhotoAlbumCategoryPhotoPeople photoPeople)
		{
			Add(photoDate, photoDescription, photoLink);
			base.Add(new RssModuleItem("photoPeople", true, "", photoPeople));
			return -1;
		}

		/// <summary>Adds a specified item to this collection.</summary>
		/// <param name="photoDate">Date of the Photo</param>
		/// <param name="photoDescription">Description of the photo.</param>
		/// <param name="photoLink">Direct link of the photo.</param>
		/// <returns>The zero-based index of the added item.</returns>
		private int Add(string photoDate, string photoDescription, Uri photoLink)
		{
			base.Add(new RssModuleItem("photoDate", true, RssDefault.Check(photoDate)));
			base.Add(new RssModuleItem("photoDescription", false, RssDefault.Check(photoDescription)));
			base.Add(new RssModuleItem("photoLink", true, RssDefault.Check(photoLink).ToString()));
			return -1;
		}
	}


	/// <summary>A collection of categories in a photo album</summary>
	public sealed class RssPhotoAlbumCategories : RssModuleItemCollectionCollection
	{
		/// <summary>Initialize a new instance of the RssPhotoAlbumItemPhoto class</summary>
		public RssPhotoAlbumCategories()
		{
		}

		/// <summary>Adds a sepecified category to this collection.</summary>
		/// <param name="category">The category to add.</param>
		/// <returns>The zero-based index of the added item.</returns>
		public int Add(RssPhotoAlbumCategory category)
		{
			return base.Add(category);
		}
	}


	/// <summary>A Photo Album category</summary>
	public sealed class RssPhotoAlbumCategory : RssModuleItemCollection
	{
		/// <summary>Initialize a new instance of the RssPhotoAlbumItem class</summary>
		/// <param name="categoryName">Name of the category.</param>
		/// <param name="categoryDescription">Description of the category.</param>
		/// <param name="categoryDateFrom">From date of the category.</param>
		/// <param name="categoryDateTo">To date of the category.</param>
		/// <param name="categoryPhoto">Photos of the category.</param>
		public RssPhotoAlbumCategory(string categoryName, string categoryDescription, DateTime categoryDateFrom, DateTime categoryDateTo, RssPhotoAlbumCategoryPhoto categoryPhoto)
		{
			Add(categoryName, categoryDescription, categoryDateFrom, categoryDateTo, categoryPhoto);
		}

		/// <summary>Adds a specified category to this collection.</summary>
		/// <param name="categoryName">Name of the category.</param>
		/// <param name="categoryDescription">Description of the category.</param>
		/// <param name="categoryDateFrom">From date of the category.</param>
		/// <param name="categoryDateTo">To date of the category.</param>
		/// <param name="categoryPhoto">Photos of the category.</param>
		/// <returns>The zero-based index of the added item.</returns>
		private int Add(string categoryName, string categoryDescription, DateTime categoryDateFrom, DateTime categoryDateTo, RssPhotoAlbumCategoryPhoto categoryPhoto)
		{
			var categoryDataRange = new RssModuleItemCollection();
			categoryDataRange.Add(new RssModuleItem("from", true, RssDefault.Check(categoryDateFrom.ToUniversalTime().ToString("r"))));
			categoryDataRange.Add(new RssModuleItem("to", true, RssDefault.Check(categoryDateTo.ToUniversalTime().ToString("r"))));

			base.Add(new RssModuleItem("categoryName", true, RssDefault.Check(categoryName)));
			base.Add(new RssModuleItem("categoryDescription", true, RssDefault.Check(categoryDescription)));
			base.Add(new RssModuleItem("categoryDateRange", true, "", categoryDataRange));
			base.Add(new RssModuleItem("categoryPhoto", true, "", categoryPhoto));

			return -1;
		}

		/// <summary>Initialize a new instance of the RssPhotoAlbumItem class</summary>
		/// <param name="categoryName">Name of the category.</param>
		/// <param name="categoryDescription">Description of the category.</param>
		/// <param name="categoryDateFrom">From date of the category.</param>
		/// <param name="categoryDateTo">To date of the category.</param>
		/// <param name="categoryPhoto">Photos of the category.</param>
		public RssPhotoAlbumCategory(string categoryName, string categoryDescription, string categoryDateFrom, string categoryDateTo, RssPhotoAlbumCategoryPhoto categoryPhoto)
		{
			Add(categoryName, categoryDescription, categoryDateFrom, categoryDateTo, categoryPhoto);
		}

		/// <summary>Adds a specified category to this collection.</summary>
		/// <param name="categoryName">Name of the category.</param>
		/// <param name="categoryDescription">Description of the category.</param>
		/// <param name="categoryDateFrom">From date of the category.</param>
		/// <param name="categoryDateTo">To date of the category.</param>
		/// <param name="categoryPhoto">Photos of the category.</param>
		/// <returns>The zero-based index of the added item.</returns>
		private int Add(string categoryName, string categoryDescription, string categoryDateFrom, string categoryDateTo, RssPhotoAlbumCategoryPhoto categoryPhoto)
		{
			var categoryDataRange = new RssModuleItemCollection();
			categoryDataRange.Add(new RssModuleItem("from", true, RssDefault.Check(categoryDateFrom)));
			categoryDataRange.Add(new RssModuleItem("to", true, RssDefault.Check(categoryDateTo)));

			base.Add(new RssModuleItem("categoryName", true, RssDefault.Check(categoryName)));
			base.Add(new RssModuleItem("categoryDescription", true, RssDefault.Check(categoryDescription)));
			base.Add(new RssModuleItem("categoryDateRange", true, "", categoryDataRange));
			base.Add(new RssModuleItem("categoryPhoto", true, "", categoryPhoto));

			return -1;
		}

		/// <summary>Initialize a new instance of the RssPhotoAlbumItem class</summary>
		/// <param name="categoryName">Name of the category.</param>
		/// <param name="categoryDescription">Description of the category.</param>
		/// <param name="categoryDateFrom">From date of the category.</param>
		/// <param name="categoryDateTo">To date of the category.</param>
		/// <param name="categoryPhotos">Photos of the category.</param>
		public RssPhotoAlbumCategory(string categoryName, string categoryDescription, DateTime categoryDateFrom, DateTime categoryDateTo, RssPhotoAlbumCategoryPhotos categoryPhotos)
		{
			Add(categoryName, categoryDescription, categoryDateFrom, categoryDateTo, categoryPhotos);
		}

		/// <summary>Adds a specified category to this collection.</summary>
		/// <param name="categoryName">Name of the category.</param>
		/// <param name="categoryDescription">Description of the category.</param>
		/// <param name="categoryDateFrom">From date of the category.</param>
		/// <param name="categoryDateTo">To date of the category.</param>
		/// <param name="categoryPhotos">Photos of the category.</param>
		/// <returns>The zero-based index of the added item.</returns>
		private int Add(string categoryName, string categoryDescription, DateTime categoryDateFrom, DateTime categoryDateTo, RssPhotoAlbumCategoryPhotos categoryPhotos)
		{
			var categoryDataRange = new RssModuleItemCollection();
			categoryDataRange.Add(new RssModuleItem("from", true, RssDefault.Check(categoryDateFrom.ToUniversalTime().ToString("r"))));
			categoryDataRange.Add(new RssModuleItem("to", true, RssDefault.Check(categoryDateTo.ToUniversalTime().ToString("r"))));

			base.Add(new RssModuleItem("categoryName", true, RssDefault.Check(categoryName)));
			base.Add(new RssModuleItem("categoryDescription", true, RssDefault.Check(categoryDescription)));
			base.Add(new RssModuleItem("categoryDateRange", true, "", categoryDataRange));
			foreach(RssPhotoAlbumCategoryPhoto categoryPhoto in categoryPhotos)
				base.Add(new RssModuleItem("categoryPhoto", true, "", categoryPhoto));

			return -1;
		}

		/// <summary>Initialize a new instance of the RssPhotoAlbumItem class</summary>
		/// <param name="categoryName">Name of the category.</param>
		/// <param name="categoryDescription">Description of the category.</param>
		/// <param name="categoryDateFrom">From date of the category.</param>
		/// <param name="categoryDateTo">To date of the category.</param>
		/// <param name="categoryPhotos">Photos of the category.</param>
		public RssPhotoAlbumCategory(string categoryName, string categoryDescription, string categoryDateFrom, string categoryDateTo, RssPhotoAlbumCategoryPhotos categoryPhotos)
		{
			Add(categoryName, categoryDescription, categoryDateFrom, categoryDateTo, categoryPhotos);
		}

		/// <summary>Adds a specified category to this collection.</summary>
		/// <param name="categoryName">Name of the category.</param>
		/// <param name="categoryDescription">Description of the category.</param>
		/// <param name="categoryDateFrom">From date of the category.</param>
		/// <param name="categoryDateTo">To date of the category.</param>
		/// <param name="categoryPhotos">Photos of the category.</param>
		/// <returns>The zero-based index of the added item.</returns>
		private int Add(string categoryName, string categoryDescription, string categoryDateFrom, string categoryDateTo, RssPhotoAlbumCategoryPhotos categoryPhotos)
		{
			var categoryDataRange = new RssModuleItemCollection();
			categoryDataRange.Add(new RssModuleItem("from", true, RssDefault.Check(categoryDateFrom)));
			categoryDataRange.Add(new RssModuleItem("to", true, RssDefault.Check(categoryDateTo)));

			base.Add(new RssModuleItem("categoryName", true, RssDefault.Check(categoryName)));
			base.Add(new RssModuleItem("categoryDescription", true, RssDefault.Check(categoryDescription)));
			base.Add(new RssModuleItem("categoryDateRange", true, "", categoryDataRange));
			foreach(RssPhotoAlbumCategoryPhoto categoryPhoto in categoryPhotos)
				base.Add(new RssModuleItem("categoryPhoto", true, "", categoryPhoto));

			return -1;
		}
	}

	
	/// <summary>RSS syndication for Robert A. Wlodarczyk's Photo Album application (to be sold by Inno Thinx LLC)</summary>
	public sealed class RssPhotoAlbum : RssModule
	{
		/// <summary>Initialize a new instance of the RssPhotoAlbum class</summary>
		/// <param name="link">Link to the Photo Album</param>
		/// <param name="photoAlbumCategory">The category of the Photo Album to add</param>
		public RssPhotoAlbum(Uri link, RssPhotoAlbumCategory photoAlbumCategory)
		{
			NamespacePrefix = "photoAlbum";
			NamespaceURL = new Uri("http://xml.innothinx.com/photoAlbum");

			ChannelExtensions.Add(new RssModuleItem("link", true, RssDefault.Check(link).ToString()));

			ItemExtensions.Add(photoAlbumCategory);
		}

		/// <summary>Initialize a new instance of the RssPhotoAlbum class</summary>
		/// <param name="link">Link to the Photo Album</param>
		/// <param name="photoAlbumCategories">A collection of categories in the Photo Album to add</param>
		public RssPhotoAlbum(Uri link, RssPhotoAlbumCategories photoAlbumCategories)
		{
			NamespacePrefix = "photoAlbum";
			NamespaceURL = new Uri("http://xml.innothinx.com/photoAlbum");

			ChannelExtensions.Add(new RssModuleItem("link", true, RssDefault.Check(link).ToString()));

			foreach(RssModuleItemCollection photoAlbumCategory in photoAlbumCategories)
			{
				ItemExtensions.Add(photoAlbumCategory);
			}
		}

		/// <summary>Link element for channel</summary>
		public Uri Link
		{
			get { return (RssDefault.Check(ChannelExtensions[0].Text) == RssDefault.String) ? null : new Uri(ChannelExtensions[0].Text); }
			set { ChannelExtensions[0].Text = (RssDefault.Check(value) == RssDefault.Uri) ? "" : value.ToString(); }
		}
	}
}
