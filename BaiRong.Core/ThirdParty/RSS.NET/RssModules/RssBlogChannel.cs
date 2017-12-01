/* RssBlogChannel.cs
 * =================
 * 
 * RSS.NET (http://rss-net.sf.net/)
 * Copyright ?2002, 2003 George Tsiokos. All Rights Reserved.
 * 
 * RSS 2.0 (http://blogs.law.harvard.edu/tech/rss)
 * RSS 2.0 is offered by the Berkman Center for Internet & Society at 
 * Harvard Law School under the terms of the Attribution/Share Alike 
 * Creative Commons license.
 * 
 * blogChannel RSS Module (http://backend.userland.com/blogChannelModule)
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
	/// <summary>A RSS module that adds elements at the channel level that are common to weblogs.</summary>
	public sealed class RssBlogChannel : RssModule
	{
		/// <summary>Initialize a new instance of the </summary>
		/// <param name="blogRoll">The URL of an OPML file containing the blogroll for the site.</param>
		/// <param name="mySubscriptions">The URL of an OPML file containing the author's RSS subscriptions.</param>
		/// <param name="blink">
		///		The URL of a weblog that the author of the weblog is promoting per Mark Pilgrim's description.
		///		<remarks>"http://diveintomark.org/archives/2002/09/17.html#blink_and_youll_miss_it"</remarks>
		///	</param>
		/// <param name="changes">
		///		The URL of a changes.xml file. When the feed that contains this element updates, it pings a server that updates this file. The presence of this element says to aggregators that they only have to read the changes file to see if this feed has updated. If several feeds point to the same changes file, the aggregator has to do less polling, resulting in better use of server bandwidth, and the Internet as a whole; and resulting in faster scans. Everyone wins. For more technical information, see the howto on the XML-RPC site.
		///		<remarks>"http://www.xmlrpc.com/weblogsComForRss"</remarks>
		/// </param>
		public RssBlogChannel(Uri blogRoll, Uri mySubscriptions, Uri blink, Uri changes)
		{
			NamespacePrefix = "blogChannel";
			NamespaceURL = new Uri("http://backend.userland.com/blogChannelModule");

			ChannelExtensions.Add(new RssModuleItem("blogRoll", true, RssDefault.Check(blogRoll.ToString())));
			ChannelExtensions.Add(new RssModuleItem("mySubscriptions", true, RssDefault.Check(mySubscriptions.ToString())));
			ChannelExtensions.Add(new RssModuleItem("blink", true, RssDefault.Check(blink.ToString())));
			ChannelExtensions.Add(new RssModuleItem("changes", true, RssDefault.Check(changes.ToString())));
		}
	}
}