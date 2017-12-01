/* RssFeed.cs
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
using System.IO;
using System.Net;
using System.Text;

namespace BaiRong.Core.Rss
{
	/// <summary>The contents of a RssFeed</summary>
	[SerializableAttribute()]
	public class RssFeed
	{
		private RssChannelCollection channels = new RssChannelCollection();
		private RssModuleCollection modules = new RssModuleCollection();
		private ExceptionCollection exceptions = null;
		private DateTime lastModified = RssDefault.DateTime;
		private RssVersion rssVersion = RssVersion.Empty;
		private bool cached = false;
		private string etag = RssDefault.String;
		private string url = RssDefault.String;
		private Encoding encoding = null;
		/// <summary>Initialize a new instance of the RssFeed class.</summary>
		public RssFeed() {}
		/// <summary>Initialize a new instance of the RssFeed class with a specified encoding.</summary>
		public RssFeed(Encoding encoding)
		{ 
			this.encoding = encoding;
		}
		/// <summary>Returns a string representation of the current Object.</summary>
		/// <returns>The Url of the feed</returns>
		public override string ToString()
		{
			return url;
		}
		/// <summary>The channels that are contained in the feed.</summary>
		public RssChannelCollection Channels => channels;

	    /// <summary>The modules that the feed adhears to.</summary>
		public RssModuleCollection Modules => modules;

	    /// <summary>A collection of all exceptions encountered during the reading of the feed.</summary>
		public ExceptionCollection Exceptions => exceptions == null ? new ExceptionCollection() : exceptions;

	    /// <summary>The Version of the feed.</summary>
		public RssVersion Version
		{
			get { return rssVersion; }
			set { rssVersion = value; }
		}
		/// <summary>The server generated hash of the feed.</summary>
		public string ETag => etag;

	    /// <summary>The server generated last modfified date and time of the feed.</summary>
		public DateTime LastModified => lastModified;

	    /// <summary>Indicates this feed has not been changed on the server, and the local copy was returned.</summary>
		public bool Cached => cached;

	    /// <summary>Location of the feed</summary>
		public string Url => url;

	    /// <summary>Encoding of the feed</summary>
		public Encoding Encoding	
		{
			get { return encoding; }
			set { encoding = value; }
		}
		/// <summary>Reads the specified RSS feed</summary>
		/// <param name="url">The url or filename of the RSS feed</param>
		/// <returns>The contents of the feed</returns>
		public static RssFeed Read(string url)
		{
			return read(url, null, null);
		}
		/// <summary>Reads the specified RSS feed</summary>
		/// <param name="Request">The specified way to connect to the web server</param>
		/// <returns>The contents of the feed</returns>
		public static RssFeed Read(HttpWebRequest Request)
		{
			return read(Request.RequestUri.ToString(), Request, null);
		}
		/// <summary>Reads the specified RSS feed</summary>
		/// <param name="oldFeed">The cached version of the feed</param>
		/// <returns>The current contents of the feed</returns>
		/// <remarks>Will not download the feed if it has not been modified</remarks>
		public static RssFeed Read(RssFeed oldFeed)
		{
			return read(oldFeed.url, null, oldFeed);
		}
		/// <summary>Reads the specified RSS feed</summary>
		/// <param name="Request">The specified way to connect to the web server</param>
		/// <param name="oldFeed">The cached version of the feed</param>
		/// <returns>The current contents of the feed</returns>
		/// <remarks>Will not download the feed if it has not been modified</remarks>
		public static RssFeed Read(HttpWebRequest Request, RssFeed oldFeed)
		{
			return read(oldFeed.url, Request, oldFeed);
		}
		private static RssFeed read(string url, HttpWebRequest request, RssFeed oldFeed)
		{
			// ***** Marked for substantial improvement
			var feed = new RssFeed();
			RssElement element = null;
			Stream stream = null;
			var uri = new Uri(url);
			feed.url = url;
			
			switch (uri.Scheme)
			{	
				case "file":
					feed.lastModified = File.GetLastWriteTime(url);
					if ((oldFeed != null) && (feed.LastModified == oldFeed.LastModified))
					{
						oldFeed.cached = true;
						return oldFeed;
					}
					stream = new FileStream(url, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
					break;
				case "https":
					goto case "http";
				case "http":
					if (request == null)
						request = (HttpWebRequest)WebRequest.Create(uri);
					if (oldFeed != null)
					{
						request.IfModifiedSince = oldFeed.LastModified;
						request.Headers.Add("If-None-Match", oldFeed.ETag);
					}
					try
					{
						var response = (HttpWebResponse)request.GetResponse();
						feed.lastModified = response.LastModified;
						feed.etag = response.Headers["ETag"];
						try 
						{ 
							if (response.ContentEncoding != "")
								feed.encoding = Encoding.GetEncoding(response.ContentEncoding); 
						}
						catch {}
						stream = response.GetResponseStream();
					}
					catch (WebException we)
					{
						if (oldFeed != null)
						{
							oldFeed.cached = true;
							return oldFeed;
						}
						else throw we; // bad
					}
					break;
			}

			if (stream != null)
			{
				RssReader reader = null;
				try
				{
					reader = new RssReader(stream);
					do
					{
						element = reader.Read();
						if (element is RssChannel)
							feed.Channels.Add((RssChannel)element);
					}
					while (element != null);
					feed.rssVersion = reader.Version;
				}
				finally
				{
					feed.exceptions = reader.Exceptions;
					reader.Close();
				}
			}
			else
				throw new ApplicationException("Not a valid Url");

			return feed;
		}
		/// <summary>Writes the RSS feed to the specified stream.</summary>
		/// <param name="stream">specified Stream</param>
		/// <exception cref="ArgumentException">The Stream cannot be written to.</exception>
		/// <exception cref="InvalidOperationException">Feed must contain at least one channel.</exception>
		/// <exception cref="InvalidOperationException">Channel must contain at least one item.</exception>
		public void Write(Stream stream)
		{
			RssWriter writer;
			
			if (encoding == null)
				writer = new RssWriter(stream);
			else
				writer = new RssWriter(stream, encoding);
 			write(writer);
		}

        /// <summary>
        /// add by lxx
        /// </summary>
        public void Write(TextWriter textWriter)
        {
            var writer = new RssWriter(textWriter);
            write(writer);
        }

		/// <summary>Writes the RSS feed to the specified file.</summary>
		/// <remarks>The encoding is ISO-8859-1.</remarks>
		/// <exception cref="ArgumentException">The filename is empty, contains only white space, or contains one or more invalid characters.</exception>
		/// <exception cref="UnauthorizedAccessException">Access is denied.</exception>
		/// <exception cref="ArgumentNullException">The filename is a (null c#, Nothing vb) reference.</exception>
		/// <exception cref="DirectoryNotFoundException">The directory to write to is not found.</exception>
		/// <exception cref="IOException">The filename includes an incorrect or invalid syntax for file name, directory name, or volume label syntax.</exception>
		/// <exception cref="System.Security.SecurityException">The caller does not have the required permission.</exception>
		/// <param name="fileName">specified file (including path) If the file exists, it will be truncated with the new content.</param>
		/// <exception cref="InvalidOperationException">Feed must contain at least one channel.</exception>
		/// <exception cref="InvalidOperationException">Channel must contain at least one item.</exception>
		public void Write(string fileName)
		{
			var writer = new RssWriter(fileName);
			write(writer);
		}
		private void write(RssWriter writer)
		{
			try
			{
				if (channels.Count == 0)
					throw new InvalidOperationException("Feed must contain at least one channel.");
			
				writer.Version = rssVersion;

				writer.Modules = modules;

				foreach(RssChannel channel in channels)
				{
					if (channel.Items.Count == 0)
						throw new InvalidOperationException("Channel must contain at least one item.");

					writer.Write(channel);
				}
			}
			finally
			{
				if (writer != null)
					writer.Close();
			}
		}
	}
}
