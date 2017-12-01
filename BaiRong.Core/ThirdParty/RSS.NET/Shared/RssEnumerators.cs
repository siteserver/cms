/* Enumerators.cs
 * ==============
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
	/// <summary>All valid Rss Cloud protocols, including Null</summary>
    [SerializableAttribute()]
	public enum RssCloudProtocol
	{
		/// <summary>Not defined</summary>
		Empty, 
		/// <summary>Protocol is not supported</summary>
		NotSupported,
		/// <summary>xml-rpc</summary>
		XmlRpc, 
		/// <summary>soap</summary>
		Soap, 
		/// <summary>http-post</summary>
		HttpPost
	}
	/// <summary>All RSS versions</summary>
    [SerializableAttribute()]
	public enum RssVersion
	{
		/// <summary>Not defined</summary>
		Empty,
		/// <summary>Version is not directly supported</summary>
		NotSupported,
		/// <summary>RDF Site Summary (RSS) 0.9</summary>
		RSS090,
		/// <summary>Rich Site Summary (RSS) 0.91</summary>
		RSS091,
		/// <summary>Rich Site Summary (RSS) 0.92</summary>
		RSS092,
		/// <summary>RDF Site Summary (RSS) 1.0</summary>
		RSS10,
		/// <summary>Really Simple Syndication (RSS) 2.0</summary>
		RSS20
	}
}
