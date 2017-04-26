/* RssChannel.cs
 * =============
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
	/// <summary>Grouping of related content items on a site</summary>
    [SerializableAttribute()]
	public class RssChannel : RssElement
	{
		private string title = RssDefault.String;
		private Uri link = RssDefault.Uri;
		private string description = RssDefault.String;
		private string language = RssDefault.String;
		private string copyright = RssDefault.String;
		private string managingEditor = RssDefault.String;
		private string webMaster = RssDefault.String;
		private DateTime pubDate = RssDefault.DateTime;
		private DateTime lastBuildDate = RssDefault.DateTime;
		private RssCategoryCollection categories = new RssCategoryCollection();
		private string generator = RssDefault.String;
		private string docs = RssDefault.String;
		private RssCloud cloud = null;
		private int timeToLive = RssDefault.Int;
		private RssImage image = null;
		private RssTextInput textInput = null;
		private bool[] skipHours = new bool[24];
		private bool[] skipDays = new bool[7];
		private string rating = RssDefault.String;
		private RssItemCollection items = new RssItemCollection();
		/// <summary>Initialize a new instance of the RssChannel class.</summary>
		public RssChannel() {}
		/// <summary>Returns a string representation of the current Object.</summary>
		/// <returns>The channel's title, description, or "RssChannel" if the title and description are blank.</returns>
		public override string ToString()
		{
			if (title != null)
				return title;
			else
				if (description != null)
					return description;
				else
					return "RssChannel";
		}
		/// <summary>The name of the channel</summary>
		/// <remarks>Maximum length is 100 characters (For RSS 0.91)</remarks>
		public string Title
		{
			get { return title; }
			set { title = RssDefault.Check(value); }
		}
		/// <summary>URL of the website named in the title</summary>
		/// <remarks>Maximum length is 500 characters (For RSS 0.91)</remarks>
		public Uri Link
		{
			get { return link; }
			set { link = RssDefault.Check(value); }
		}
		/// <summary>Description of the channel</summary>
		/// <remarks>Maximum length is 500 characters (For RSS 0.91)</remarks>
		public string Description
		{
			get { return description; }
			set { description = RssDefault.Check(value); }
		}
		/// <summary>Language the channel is written in</summary>
		public string Language
		{
			get { return language; }
			set { language = RssDefault.Check(value); }
		}
		/// <summary>A link and description for a graphic icon that represent a channel</summary>
		public RssImage Image
		{
			get { return image; }
			set { image = value; }
		}
		/// <summary>Copyright notice for content in the channel</summary>
		/// <remarks>Maximum length is 100 (For RSS 0.91)</remarks>
		public string Copyright
		{
			get { return copyright; }
			set { copyright = RssDefault.Check(value); }
		}
		/// <summary>The email address of the managing editor of the channel, the person to contact for editorial inquiries</summary>
		/// <remarks>
		/// <para>Maximum length is 100 (For RSS 0.91)</para>
		/// <para>The suggested format for email addresses in RSS elements is</para>
		/// <para>bull@mancuso.com (Bull Mancuso)</para>
		/// </remarks>
		public string ManagingEditor
		{
			get { return managingEditor; }
			set { managingEditor = RssDefault.Check(value); }
		}
		/// <summary>The email address of the webmaster for the channel</summary>
		/// <remarks>
		/// <para>Person to contact if there are technical problems</para>
		/// <para>Maximum length is 100 (For RSS 0.91)</para>
		/// <para>The suggested format for email addresses in RSS elements is</para>
		/// <para>bull@mancuso.com (Bull Mancuso)</para>
		/// </remarks>
		public string WebMaster
		{
			get { return webMaster; }
			set { webMaster = RssDefault.Check(value); }
		}
		/// <summary>The PICS rating for the channel</summary>
		/// <remarks>Maximum length is 500 (For RSS 0.91)</remarks>
		public string Rating
		{
			get { return rating; }
			set { rating = RssDefault.Check(value); }
		}
		/// <summary>The publication date for the content in the channel, expressed as the coordinated universal time (UTC)</summary>
		public DateTime PubDate
		{
			get { return pubDate; }
			set { pubDate = value; }
		}
		/// <summary>The date-time the last time the content of the channel changed, expressed as the coordinated universal time (UTC)</summary>
		public DateTime LastBuildDate
		{
			get { return lastBuildDate; }
			set { lastBuildDate = value; }
		}
		/// <summary>One or more categories the channel belongs to.</summary>
		public RssCategoryCollection Categories => categories;

	    /// <summary>A string indicating the program used to generate the channel</summary>
		public string Generator
		{
			get { return generator; }
			set { generator = RssDefault.Check(value); }
		}
		/// <summary>A URL, points to the documentation for the format used in the RSS file</summary>
		/// <remarks>Maximum length is 500 (For RSS 0.91).</remarks>
		public string Docs
		{
			get { return docs; }
			set { docs = RssDefault.Check(value); }
		}
		/// <summary>Provides information about an HTTP GET feature, typically for a search or subscription</summary>
		public RssTextInput TextInput
		{
			get { return textInput; }
			set { textInput = value; }
		}
		/// <summary>Readers should not read the channel during days listed. (UTC)</summary>
		/// <remarks>Days are listed in the array in the following order:<list type="number">
		/// <item><description>Monday</description></item>
		/// <item><description>Tuesday</description></item>
		/// <item><description>Wednesday</description></item>
		/// <item><description>Thursday</description></item>
		/// <item><description>Friday</description></item>
		/// <item><description>Saturday</description></item>
		/// <item><description>Sunday</description></item>
		/// <item><description>Monday</description></item>
		/// </list></remarks>
		public bool[] SkipDays
		{
			get { return skipDays; }
			set { skipDays = value; }
		}
		/// <summary>Readers should not read the channel during hours listed (UTC)</summary>
		/// <remarks>Represents a time in UTC - 1.</remarks>
		public bool[] SkipHours
		{
			get { return skipHours; }
			set { skipHours = value; }
		}
		/// <summary>Allow processes to register with a cloud to be notified of updates to the channel</summary>
		public RssCloud Cloud
		{
			get { return cloud; }
			set { cloud = value; }
		}
		/// <summary>The number of minutes that a channel can be cached.</summary>
		public int TimeToLive
		{
			get { return timeToLive; }
			set { timeToLive = RssDefault.Check(value); }
		}
		/// <summary>All items within the channel</summary>
		public RssItemCollection Items => items;
	}
}
