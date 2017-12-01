/* RssCreativeCommons.cs
 * =====================
 * 
 * RSS.NET (http://rss-net.sf.net/)
 * Copyright ?2002, 2003 George Tsiokos. All Rights Reserved.
 * 
 * RSS 2.0 (http://blogs.law.harvard.edu/tech/rss)
 * RSS 2.0 is offered by the Berkman Center for Internet & Society at 
 * Harvard Law School under the terms of the Attribution/Share Alike 
 * Creative Commons license.
 * 
 * creativeCommons RSS Module (http://backend.userland.com/creativeCommonsRssModule)
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
	/// <summary>A RSS module that adds elements at the channel or item level that specifies which Creative Commons license applies.</summary>
	public sealed class RssCreativeCommons : RssModule
	{
		/// <summary>Initialize a new instance of the </summary>
		/// <param name="license">
		///		If present as a sub-element of channel, indicates that the content of the RSS file is available under a license, indicated by a URL, which is the value of the license element. A list of some licenses that may be used in this context is on the Creative Commons website on this page, however the license element may point to licenses not authored by Creative Commons.
		///		You may also use the license element as a sub-element of item. When used this way it applies only to the content of that item. If an item has a license, and the channel does too, the license on the item applies, i.e. the inner license overrides the outer one.
		///		Multiple license elements are allowed, in either context, indicating that the content is available under multiple licenses.
		///		<remarks>"http://www.creativecommons.org/licenses/"</remarks>
		///	</param>
		/// <param name="isChannelSubElement">If present as a sub-element of channel then true, otherwise false</param>
		public RssCreativeCommons(Uri license, bool isChannelSubElement)
		{
			NamespacePrefix = "creativeCommons";
			NamespaceURL = new Uri("http://backend.userland.com/creativeCommonsRssModule");

			if(isChannelSubElement)
			{
				ChannelExtensions.Add(new RssModuleItem("license", true, RssDefault.Check(license.ToString())));
			}
			else
			{
				var rssItems = new RssModuleItemCollection();

				rssItems.Add(new RssModuleItem("license", true, RssDefault.Check(license.ToString())));

				ItemExtensions.Add(rssItems);
			}
		}
	}
}