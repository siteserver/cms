/* RssImage.cs
 * ===========
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
	/// <summary>A link and description for a graphic that represent a channel</summary>
    [SerializableAttribute()]
	public class RssImage : RssElement
	{
		private string title = RssDefault.String;
		private string description = RssDefault.String;
		private Uri uri = RssDefault.Uri;
		private Uri link = RssDefault.Uri;
		private int width = RssDefault.Int;
		private int height = RssDefault.Int;

		/// <summary>Initialize a new instance of the RssImage class.</summary>
		public RssImage() {}

		/// <summary>The URL of a GIF, JPEG or PNG image that represents the channel.</summary>
		/// <remarks>Maximum length is 500 (For RSS 0.91).</remarks>
		public Uri Url
		{
			get { return uri; }
			set { uri = RssDefault.Check(value); }
		}
		/// <summary>Describes the image, it's used in the ALT attribute of the HTML img tag when the channel is rendered in HTML.</summary>
		/// <remarks>Maximum length is 100 (For RSS 0.91).</remarks>
		public string Title
		{
			get { return title; }
			set { title = RssDefault.Check(value); }
		}
		/// <summary>The URL of the site, when the channel is rendered, the image is a link to the site.</summary>
		/// <remarks>Maximum length is 500 (For RSS 0.91).</remarks>
		public Uri Link
		{
			get { return link; }
			set { link = RssDefault.Check(value); }
		}
		/// <summary>Contains text that is included in the TITLE attribute of the link formed around the image in the HTML rendering.</summary>
		public string Description
		{
			get { return description; }
			set { description = RssDefault.Check(value); }
		}
		/// <summary>Width of image in pixels</summary>
		/// <remarks>Maximum value for height is 400 (For RSS 0.91)</remarks>
		public int Width
		{
			get { return width; }
			set { width = RssDefault.Check(value); }
		}
		/// <summary>Height of image in pixels</summary>
		/// <remarks>Maximum value for width is 144 (For RSS 0.91)</remarks>
		public int Height
		{
			get { return height; }
			set { height = RssDefault.Check(value); }
		}
	}
}
