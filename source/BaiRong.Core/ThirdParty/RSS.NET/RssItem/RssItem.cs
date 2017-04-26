/* RssItem.cs
 * ==========
 * 
 * RSS.NET (http://rss-net.sf.net/)
 * Copyright ?2002, 2003 George Tsiokos. All Rights Reserved.
 * 
 * RSS 2.0 (http://blogs.law.harvard.edu/tech/rss)
 * RSS 2.0 is offered by the Berkman Center for Internet & Society at 
 * Harvard Law School under the terms of the Attribution/Share Alike 
 * Creative Commons license.
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
	/// <summary>A channel may contain any number of items, each of which links to more information about the item, with an optional description</summary>
    [SerializableAttribute()]
	public class RssItem : RssElement
	{
		private string title = RssDefault.String;
		private Uri link = RssDefault.Uri;
		private string description = RssDefault.String;
		private string author = RssDefault.String;
		private RssCategoryCollection categories = new RssCategoryCollection();
		private string comments = RssDefault.String;
		private RssEnclosure enclosure = null;
		private RssGuid guid = null;
		private DateTime pubDate = RssDefault.DateTime;
		private RssSource source = null;
		/// <summary>Initialize a new instance of the RssItem class</summary>
		public RssItem() {}
		/// <summary>Returns a string representation of the current Object.</summary>
		/// <returns>The item's title, description, or "RssItem" if the title and description are blank.</returns>
		public override string ToString()
		{
			if (title != null)
				return title;
			else
				if (description != null)
				return description;
			else
				return "RssItem";
		}
		/// <summary>Title of the item</summary>
		/// <remarks>Maximum length is 100 (For RSS 0.91)</remarks>
		public string Title
		{
			get { return title; }
			set { title = RssDefault.Check(value); }
		}
		/// <summary>URL of the item</summary>
		/// <remarks>Maximum length is 500 (For RSS 0.91)</remarks>
		public Uri Link
		{
			get { return link; }
			set { link = RssDefault.Check(value); }
		}
		/// <summary>Item synopsis</summary>
		/// <remarks>Maximum length is 500 (For RSS 0.91)</remarks>
		public string Description
		{
			get { return description; }
			set { description = RssDefault.Check(value); }
		}
		/// <summary>Email address of the author of the item</summary>
		public string Author
		{
			get { return author; }
			set { author = RssDefault.Check(value); }
		}
		/// <summary>Provide information regarding the location of the subject matter of the channel in a taxonomy</summary>
		public RssCategoryCollection Categories => categories;

	    /// <summary>URL of a page for comments relating to the item</summary>
		public string Comments
		{
			get { return comments; }
			set { comments = RssDefault.Check(value); }
		}
		/// <summary>Describes an items source</summary>
		public RssSource Source
		{
			get { return source; }
			set { source = value; }
		}
		/// <summary>A reference to an attachment to the item</summary>
		public RssEnclosure Enclosure
		{
			get { return enclosure; }
			set { enclosure = value; }
		}
		/// <summary>A string that uniquely identifies the item</summary>
		public RssGuid Guid
		{
			get { return guid; }
			set { guid = value; }
		}
		/// <summary>Indicates when the item was published</summary>
		public DateTime PubDate
		{
			get { return pubDate; }
			set { pubDate = value; }
		}
	}
}
