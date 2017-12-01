/* RssTextInput.cs
 * ===============
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
	/// <summary>Multi-purpose channel element for the purpose of allowing users to submit queries back to the publisher's site</summary>
	/// <remarks>Typically for a search or subscription</remarks>
    [SerializableAttribute()]
	public class RssTextInput : RssElement
	{
		private string title = RssDefault.String;
		private string description = RssDefault.String;
		private string name = RssDefault.String;
		private Uri link = RssDefault.Uri;
		
		/// <summary>Initialize a new instance of the RssTextInput class</summary>
		public RssTextInput() {}

		/// <summary>The label of the submit button in the text input area</summary>
		///	<remarks>Maximum length is 100 (For RSS 0.91)</remarks>
		public string Title
		{
			get { return title; }
			set { title = RssDefault.Check(value); }
		}
		/// <summary>Explains the text input area</summary>
		/// <remarks>Maximum length is 500 (For RSS 0.91)</remarks>
		public string Description
		{
			get { return description; }
			set { description = RssDefault.Check(value); }
		}
		/// <summary>The name of the text object in the text input area</summary>
		/// <remarks>Maximum length is 20 (For RSS 0.91).</remarks>
		public string Name
		{
			get { return name; }
			set { name = RssDefault.Check(value); }
		}
		/// <summary>The URL of the script that processes text input requests</summary>
		/// <remarks>Maximum length is 500 (For RSS 0.91)</remarks>
		public Uri Link
		{
			get { return link; }
			set { link = RssDefault.Check(value); }
		}
	}
}
