/* RssCloud.cs
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
	/// <summary>Allow processes to register with a cloud to be notified of updates to the channel.</summary>
    [SerializableAttribute()]
	public class RssCloud : RssElement
	{
		private RssCloudProtocol protocol = RssCloudProtocol.Empty;
		private string domain = RssDefault.String;
		private string path = RssDefault.String;
		private string registerProcedure = RssDefault.String;
		private int port = RssDefault.Int;
		/// <summary>Initialize a new instance of the RssCloud class.</summary>
		public RssCloud() {}
		/// <summary>Domain name or IP address of the cloud</summary>
		public string Domain
		{
			get { return domain; }
			set { domain = RssDefault.Check(value); }
		}
		/// <summary>TCP port that the cloud is running on</summary>
		public int Port
		{
			get { return port; }
			set { port = RssDefault.Check(value); }
		}
		/// <summary>Location of its responder</summary>
		public string Path
		{
			get { return path; }
			set { path = RssDefault.Check(value); }
		}

		/// <summary>Name of the procedure to call to request notification</summary>
		public string RegisterProcedure
		{
			get { return registerProcedure; }
			set { registerProcedure = RssDefault.Check(value); }
		}
		/// <summary>Protocol used</summary>
		public RssCloudProtocol Protocol
		{
			get { return protocol; }
			set { protocol = value; }
		}
	}
}
