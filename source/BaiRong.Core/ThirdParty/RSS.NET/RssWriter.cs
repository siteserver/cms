/* RssWriter.cs
 * ============
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
using System.Xml;
using System.Text;
using System.IO;

namespace BaiRong.Core.Rss
{
	/// <summary>Writes an RSS XML file.</summary>
	/// <remarks>Represents a writer that provides a fast, non-cached, forward-only way of generating streams or files containing RSS XML data that conforms to the W3C Extensible Markup Language (XML) 1.0 and the Namespaces in XML recommendations.</remarks>
	public class RssWriter
	{
		private XmlTextWriter writer = null;
		
		// functional var
		private bool wroteStartDocument = false;
		private bool wroteChannel = false;

		// prefrences
		private RssVersion rssVersion = RssVersion.RSS20;
		private Formatting xmlFormat = Formatting.Indented;
		private int xmlIndentation = 2;

		// constants
		private const string DateTimeFormatString = "ddd, dd MMM yyyy HH':'mm':'ss 'GMT'";

		// modules
		private RssModuleCollection _rssModules = new RssModuleCollection();

		#region Constructors

		/// <summary>Creates an instance of the RssWriter class using the specified TextWriter.</summary>
		/// <param name="textWriter">specified TextWriter</param>
		public RssWriter(TextWriter textWriter)
		{
			writer = new XmlTextWriter(textWriter);
		}

		/// <summary>Creates an instance of the RssWriter class using the specified Stream and Encoding.</summary>
		/// <exception cref="ArgumentException">The encoding is not supported or the stream cannot be written to.</exception>
		/// <param name="stream">Stream to output to</param>
		/// <param name="encoding">The encoding to use. If encoding is (null c#, Nothing vb) it writes out the stream as UTF-8.</param>
		public RssWriter(Stream stream, Encoding encoding)
		{
			writer = new XmlTextWriter(stream, encoding);
		}

		/// <summary>Creates an instance of the RssWriter class using the specified Stream.</summary>
		/// <remarks>The encoding is ISO-8859-1.</remarks>
		/// <exception cref="ArgumentException">The Stream cannot be written to.</exception>
		/// <param name="stream">specified Stream</param>
		public RssWriter(Stream stream)
		{
			writer = new XmlTextWriter(stream, Encoding.GetEncoding("ISO-8859-1"));
		}

		/// <summary>Creates an instance of the RssWriter class using the specified file and Encoding.</summary>
		/// <exception cref="ArgumentException">The encoding is not supported; the filename is empty, contains only white space, or contains one or more invalid characters.</exception>
		/// <exception cref="UnauthorizedAccessException">Access is denied.</exception>
		/// <exception cref="ArgumentNullException">The filename is a (null c#, Nothing vb) reference.</exception>
		/// <exception cref="DirectoryNotFoundException">The directory to write to is not found.</exception>
		/// <exception cref="IOException">The filename includes an incorrect or invalid syntax for file name, directory name, or volume label syntax.</exception>
		/// <exception cref="System.Security.SecurityException">The caller does not have the required permission.</exception>
		/// <param name="fileName">specified file (including path) If the file exists, it will be truncated with the new content.</param>
		/// <param name="encoding">specified Encoding</param>
		public RssWriter(string fileName, Encoding encoding)
		{
			writer = new XmlTextWriter(fileName, encoding);
		}

		/// <summary>Creates an instance of the RssWriter class using the specified file.</summary>
		/// <remarks>The encoding is ISO-8859-1.</remarks>
		/// <exception cref="ArgumentException">The filename is empty, contains only white space, or contains one or more invalid characters.</exception>
		/// <exception cref="UnauthorizedAccessException">Access is denied.</exception>
		/// <exception cref="ArgumentNullException">The filename is a (null c#, Nothing vb) reference.</exception>
		/// <exception cref="DirectoryNotFoundException">The directory to write to is not found.</exception>
		/// <exception cref="IOException">The filename includes an incorrect or invalid syntax for file name, directory name, or volume label syntax.</exception>
		/// <exception cref="System.Security.SecurityException">The caller does not have the required permission.</exception>
		/// <param name="fileName">specified file (including path) If the file exists, it will be truncated with the new content.</param>
		public RssWriter(string fileName)
		{
			writer = new XmlTextWriter(fileName, Encoding.GetEncoding("ISO-8859-1"));
		}
		#endregion

		/// <summary>Writes the begining data to the RSS file</summary>
		/// <remarks>This routine is called from the WriteChannel and WriteItem subs</remarks>
		/// <exception cref="NotSupportedException">RDF Site Summary (RSS) 1.0 is not currently supported.</exception>
		private void BeginDocument()
		{
			if (!wroteStartDocument)
			{
				if (rssVersion == RssVersion.Empty)
					rssVersion = RssVersion.RSS20;
				writer.Formatting = xmlFormat;
				writer.Indentation = xmlIndentation;
				writer.WriteStartDocument();
				if (rssVersion != RssVersion.RSS20)
					writer.WriteComment("Generated by RSS.NET: http://rss-net.sf.net");

				//exc: The xml:space or xml:lang attribute value is invalid.
				switch (rssVersion)
				{
					case RssVersion.RSS090:
						//<rdf:RDF xmlns:rdf="http://www.w3.org/1999/02/22-rdf-syntax-ns#" xmlns="http://my.netscape.com/rdf/simple/0.9/">
						writer.WriteStartElement("RDF", "rdf", "http://www.w3.org/1999/02/22-rdf-syntax-ns#");						
						break;
					case RssVersion.RSS091:
						writer.WriteStartElement("rss");
						writer.WriteDocType("rss", "-//Netscape Communications//DTD RSS 0.91//EN", "http://my.netscape.com/publish/formats/rss-0.91.dtd", null);
						writer.WriteAttributeString("version", "0.91");
						break;
					case RssVersion.RSS092:
						writer.WriteStartElement("rss");
						writer.WriteAttributeString("version", "0.92");
						break;
					case RssVersion.RSS10:
						throw new NotSupportedException("RDF Site Summary (RSS) 1.0 is not currently supported.");
					case RssVersion.RSS20:
						writer.WriteStartElement("rss");
						writer.WriteAttributeString("version", "2.0");
						// RSS Modules
						foreach(RssModule rssModule in _rssModules)
						{
							WriteAttribute("xmlns:" + rssModule.NamespacePrefix, rssModule.NamespaceURL.ToString(), true);
						}
						break;
				}
				wroteStartDocument = true;
			}
		}
		private void writeChannel(RssChannel channel)
		{
			if (writer == null)
				throw new InvalidOperationException("RssWriter has been closed, and can not be written to.");
			if (channel == null)
				throw new ArgumentNullException("Channel must be instanciated with data to be written.");
			
			if (wroteChannel)
				writer.WriteEndElement();
			else
				wroteChannel = true;

			BeginDocument();

			writer.WriteStartElement("channel");
			WriteElement("title", channel.Title, true);
			WriteElement("description", channel.Description, true);
			WriteElement("link", channel.Link, true);
			if (channel.Image != null)
			{
				writer.WriteStartElement("image");
				WriteElement("title", channel.Image.Title, true);
				WriteElement("url", channel.Image.Url, true);
				WriteElement("link", channel.Image.Link, true);
				switch (rssVersion)
				{
					case RssVersion.RSS091:
					case RssVersion.RSS092:
					case RssVersion.RSS20:
						WriteElement("description", channel.Image.Description, false);
						WriteElement("width", channel.Image.Width, false);
						WriteElement("height", channel.Image.Height, false);
						break;
				}
				writer.WriteEndElement();
			}
			switch (rssVersion)
			{
				case RssVersion.RSS091:
				case RssVersion.RSS092:
				case RssVersion.RSS20:
					WriteElement("language", channel.Language, rssVersion == RssVersion.RSS091);
					WriteElement("copyright", channel.Copyright, false);
					WriteElement("managingEditor", channel.ManagingEditor, false);
					WriteElement("webMaster", channel.WebMaster, false);
					WriteElement("pubDate", channel.PubDate, false);
					WriteElement("lastBuildDate", channel.LastBuildDate, false);
					if (channel.Docs != RssDefault.String)
						WriteElement("docs", channel.Docs, false);
					else
						switch (rssVersion)
						{
							case RssVersion.RSS091:
								WriteElement("docs", "http://my.netscape.com/publish/formats/rss-spec-0.91.html", false);
								break;
							case RssVersion.RSS092:
								WriteElement("docs", "http://backend.userland.com/rss092", false);
								break;
							case RssVersion.RSS20:
								WriteElement("docs", "http://backend.userland.com/rss", false);
								break;
						}
					WriteElement("rating", channel.Rating, false);
					string[] Days = {"monday","tuesday","wednesday","thursday","friday","saturday","sunday"};
					for (var i = 0; i <= 6; i++)
						if (channel.SkipDays[i])
						{
							writer.WriteStartElement("skipDays");
							for (var i2 = 0; i2 <= 6; i2++)
								if (channel.SkipDays[i2])
									WriteElement("day", Days[i2], false);
							writer.WriteEndElement();
							break;
						}
					for (var i = 0; i <= 23; i++)
						if (channel.SkipHours[i])
						{
							writer.WriteStartElement("skipHours");
							for (var i2 = 0; i2<= 23; i2++)
								if (channel.SkipHours[i2])
									WriteElement("hour", i2+1, false);
							writer.WriteEndElement();
							break;
						}
					break;
			}
			switch (rssVersion)
			{
				case RssVersion.RSS092:
				case RssVersion.RSS20:
					if (channel.Categories != null)
						foreach(RssCategory category in channel.Categories)
							if (category.Name != RssDefault.String)
							{
								writer.WriteStartElement("category");
								WriteAttribute("domain", category.Domain, false);
								writer.WriteString(category.Name);
								writer.WriteEndElement();
							}
					if (channel.Cloud != null)
					{
						writer.WriteStartElement("cloud");
						WriteElement("domain", channel.Cloud.Domain, false);
						WriteElement("port", channel.Cloud.Port, false);
						WriteElement("path", channel.Cloud.Path, false);
						WriteElement("registerProcedure", channel.Cloud.RegisterProcedure, false);
						if (channel.Cloud.Protocol != RssCloudProtocol.Empty)
							WriteElement("Protocol", channel.Cloud.Protocol, false);
						writer.WriteEndElement();
					}
					break;
			}
			if (rssVersion == RssVersion.RSS20)
			{
				if (channel.Generator != RssDefault.String)
					WriteElement("generator", channel.Generator, false);
				else
					WriteElement("generator", "RSS.NET: http://www.rssdotnet.com/", false);
				WriteElement("ttl", channel.TimeToLive, false);

				// RSS Modules
				foreach(RssModule rssModule in _rssModules)
				{
					if(rssModule.IsBoundTo(channel.GetHashCode()))
					{
						foreach(RssModuleItem rssModuleItem in rssModule.ChannelExtensions)
						{
							if(rssModuleItem.SubElements.Count == 0)
								WriteElement(rssModule.NamespacePrefix + ":" + rssModuleItem.Name, rssModuleItem.Text, rssModuleItem.IsRequired);
							else
								writeSubElements(rssModuleItem.SubElements, rssModule.NamespacePrefix);
						}
					}
				}
			}
			if (channel.TextInput != null)
			{
				writer.WriteStartElement("textinput");
				WriteElement("title", channel.TextInput.Title, true);
				WriteElement("description", channel.TextInput.Description, true);
				WriteElement("name", channel.TextInput.Name, true);
				WriteElement("link", channel.TextInput.Link, true);
				writer.WriteEndElement();
			}
			foreach (RssItem item in channel.Items)
			{
				writeItem(item, channel.GetHashCode());
			}
			writer.Flush();
		}
		private void writeItem(RssItem item, int channelHashCode)
		{
			if (writer == null)
				throw new InvalidOperationException("RssWriter has been closed, and can not be written to.");
			if (item == null)
				throw new ArgumentNullException("Item must be instanciated with data to be written.");
			if (!wroteChannel)
				throw new InvalidOperationException("Channel must be written first, before writing an item.");

			BeginDocument();

			writer.WriteStartElement("item");
			switch (rssVersion)
			{
				case RssVersion.RSS090:
				case RssVersion.RSS10:
				case RssVersion.RSS091:
					WriteElement("title", item.Title, true);
					WriteElement("description", item.Description, false);
					WriteElement("link", item.Link, true);
					break;
				case RssVersion.RSS20:
					if ((item.Title == RssDefault.String) && (item.Description == RssDefault.String))
						throw new ArgumentException("item title and description cannot be null");
					goto case RssVersion.RSS092;
				case RssVersion.RSS092:
					WriteElement("title", item.Title, false);
					WriteElement("description", item.Description, false);
					WriteElement("link", item.Link, false);
					if (item.Source != null)
					{
						writer.WriteStartElement("source");
						WriteAttribute("url", item.Source.Url, true);
						writer.WriteString(item.Source.Name);
						writer.WriteEndElement();
					}
					if (item.Enclosure != null)
					{
						writer.WriteStartElement("enclosure");
						WriteAttribute("url", item.Enclosure.Url, true);
						WriteAttribute("length", item.Enclosure.Length, true);
						WriteAttribute("type", item.Enclosure.Type, true);
						writer.WriteEndElement();
					}
					foreach(RssCategory category in item.Categories)
						if (category.Name != RssDefault.String)
						{
							writer.WriteStartElement("category");
							WriteAttribute("domain", category.Domain, false);
							writer.WriteString(category.Name);
							writer.WriteEndElement();
						}
					break;
			}
			if (rssVersion == RssVersion.RSS20)
			{
				WriteElement("author", item.Author, false);
				WriteElement("comments", item.Comments, false);
				if ((item.Guid != null) && (item.Guid.Name != RssDefault.String))
				{
					writer.WriteStartElement("guid");
					try {
					WriteAttribute("isPermaLink", (bool)item.Guid.PermaLink, false);
					} catch {}
					writer.WriteString(item.Guid.Name);
					writer.WriteEndElement();
				}
				WriteElement("pubDate", item.PubDate, false);

				foreach(RssModule rssModule in _rssModules)
				{
					if(rssModule.IsBoundTo(channelHashCode))
					{
						foreach(RssModuleItemCollection rssModuleItemCollection in rssModule.ItemExtensions)
						{
							if(rssModuleItemCollection.IsBoundTo(item.GetHashCode()))
								writeSubElements(rssModuleItemCollection, rssModule.NamespacePrefix);
						}
					}
				}
			}
			writer.WriteEndElement();
			writer.Flush();
		}
		/// <summary>Closes instance of RssWriter.</summary>
		/// <remarks>Writes end elements, and releases connections</remarks>
		/// <exception cref="InvalidOperationException">Occurs if the RssWriter is already closed or the caller is attempting to close before writing a channel.</exception>
		public void Close()
		{
			if (writer == null)
				throw new InvalidOperationException("RssWriter has been closed, and can not be closed again.");
			
			if (!wroteChannel)
				throw new InvalidOperationException("Can't close RssWriter without first writing a channel.");
			else
				writer.WriteEndElement(); // </channel>

			writer.WriteEndElement(); // </rss> or </rdf>
			writer.Close();
			writer = null;
		}
		/// <summary>Gets or sets the RSS version to write.</summary>
		/// <exception cref="InvalidOperationException">Can't change version number after data has been written.</exception>
		public RssVersion Version
		{
			get { return rssVersion; }
			set
			{
				if(wroteStartDocument)
					throw new InvalidOperationException("Can't change version number after data has been written.");
				else
					rssVersion = value;
			}
		}
		/// <summary>Gets or sets the <see cref="System.Xml.Formatting"/> of the XML output.</summary>
		/// <exception cref="InvalidOperationException">Can't change XML formatting after data has been written.</exception>
		public Formatting XmlFormat
		{
			get { return xmlFormat; }
			set
			{
				if(wroteStartDocument)
					throw new InvalidOperationException("Can't change XML formatting after data has been written.");
				else
					xmlFormat = value;
			}
		}
		/// <summary>Gets or sets how indentation to write for each level in the hierarchy when XmlFormat is set to <see cref="Formatting.Indented"/></summary>
		/// <exception cref="InvalidOperationException">Can't change XML formatting after data has been written.</exception>
		/// <exception cref="ArgumentException">Setting this property to a negative value.</exception>
		public int XmlIndentation
		{
			get { return xmlIndentation; }
			set
			{
				if(wroteStartDocument)
					throw new InvalidOperationException("Can't change XML indentation after data has been written.");
				else
					if (value < 0)
						throw new ArgumentException("Setting this property to a negative value.");
					else
						xmlIndentation = value;
			}
		}
		/// <summary>Writes an RSS channel</summary>
		/// <exception cref="InvalidOperationException">RssWriter has been closed, and can not be written to.</exception>
		/// <exception cref="ArgumentNullException">Channel must be instanciated with data, before calling Write.</exception>
		/// <param name="channel">RSS channel to write</param>
		public void Write(RssChannel channel)
		{
			writeChannel(channel);
		}
		/// <summary>Writes an RSS item</summary>
		/// <exception cref="InvalidOperationException">Either the RssWriter has already been closed, or the caller is attempting to write an RSS item before an RSS channel.</exception>
		/// <exception cref="ArgumentNullException">Item must be instanciated with data, before calling Write.</exception>
		/// <param name="item">RSS item to write</param>
		public void Write(RssItem item)
		{
			// NOTE: Standalone items cannot adhere to modules, hence -1 is passed. This may not be the case, however, no examples have been seen where this is legal.
			writeItem(item, -1);
		}

		/// <summary>RSS modules</summary>
		public RssModuleCollection Modules
		{
			get { return _rssModules; }
			set { _rssModules = value; }
		}
		#region WriteElement
		/// <summary>Writes an element with the specified local name and value</summary>
		/// <param name="localName">the localname of the element</param>
		/// <param name="input">the value of the element</param>
		/// <param name="required">boolean that determines if input cannot be null</param>
		private void WriteElement(string localName, DateTime input, bool required)
		{
			if (input != RssDefault.DateTime)
				writer.WriteElementString(localName, XmlConvert.ToString(input,DateTimeFormatString));
			else if (required)
					throw new ArgumentException(localName + " can not be null.");
		}
		/// <summary>Writes an element with the specified local name and value</summary>
		/// <param name="localName">the localname of the element</param>
		/// <param name="input">the value of the element</param>
		/// <param name="required">boolean that determines if input cannot be null</param>
		private void WriteElement(string localName, int input, bool required)
		{
			if (input != RssDefault.Int)
				writer.WriteElementString(localName, XmlConvert.ToString(input));
			else if (required)
				throw new ArgumentException(localName + " can not be null.");
		}
		/// <summary>Writes an element with the specified local name and value</summary>
		/// <param name="localName">the localname of the element</param>
		/// <param name="input">the value of the element</param>
		/// <param name="required">boolean that determines if input cannot be null</param>
		private void WriteElement(string localName, string input, bool required)
		{
			if (input != RssDefault.String)
				writer.WriteElementString(localName, input);
			else if (required)
				throw new ArgumentException(localName + " can not be null.");
		}
		/// <summary>Writes an element with the specified local name and value</summary>
		/// <param name="localName">the localname of the element</param>
		/// <param name="input">the value of the element</param>
		/// <param name="required">boolean that determines if input cannot be null</param>
		private void WriteElement(string localName, Uri input, bool required)
		{
			if (input != RssDefault.Uri)
				writer.WriteElementString(localName, input.ToString());
			else if (required)
				throw new ArgumentException(localName + " can not be null.");
		}
		/// <summary>Writes an element with the specified local name and value</summary>
		/// <param name="localName">the localname of the element</param>
		/// <param name="input">the value of the element</param>
		/// <param name="required">boolean that determines if input cannot be null</param>
		private void WriteElement(string localName, object input, bool required)
		{
			if (input != null)
				writer.WriteElementString(localName, input.ToString());
			else if (required)
				throw new ArgumentException(localName + " can not be null.");
		}
		#endregion
		#region WriteAttribute
		/// <summary>Writes an attribute with the specified local name and value</summary>
		/// <param name="localName">the localname of the element</param>
		/// <param name="input">the value of the element</param>
		/// <param name="required">boolean that determines if input cannot be null</param>
		private void WriteAttribute(string localName, DateTime input, bool required)
		{
			if (input != RssDefault.DateTime)
				writer.WriteAttributeString(localName, XmlConvert.ToString(input,DateTimeFormatString));
			else if (required)
				throw new ArgumentException(localName + " can not be null.");
		}
		/// <summary>Writes an attribute with the specified local name and value</summary>
		/// <param name="localName">the localname of the element</param>
		/// <param name="input">the value of the element</param>
		/// <param name="required">boolean that determines if input cannot be null</param>
		private void WriteAttribute(string localName, int input, bool required)
		{
			if (input != RssDefault.Int)
				writer.WriteAttributeString(localName, XmlConvert.ToString(input));
			else if (required)
				throw new ArgumentException(localName + " can not be null.");
		}
		/// <summary>Writes an attribute with the specified local name and value</summary>
		/// <param name="localName">the localname of the element</param>
		/// <param name="input">the value of the element</param>
		/// <param name="required">boolean that determines if input cannot be null</param>
		private void WriteAttribute(string localName, string input, bool required)
		{
			if (input != RssDefault.String)
				writer.WriteAttributeString(localName, input);
			else if (required)
				throw new ArgumentException(localName + " can not be null.");
		}
		/// <summary>Writes an attribute with the specified local name and value</summary>
		/// <param name="localName">the localname of the element</param>
		/// <param name="input">the value of the element</param>
		/// <param name="required">boolean that determines if input cannot be null</param>
		private void WriteAttribute(string localName, Uri input, bool required)
		{
			if (input != RssDefault.Uri)
				writer.WriteAttributeString(localName, input.ToString());
			else if (required)
				throw new ArgumentException(localName + " can not be null.");
		}
		/// <summary>Writes an attribute with the specified local name and value</summary>
		/// <param name="localName">the localname of the element</param>
		/// <param name="input">the value of the element</param>
		/// <param name="required">boolean that determines if input cannot be null</param>
		private void WriteAttribute(string localName, object input, bool required)
		{
			if (input != null)
				writer.WriteAttributeString(localName, input.ToString());
			else if (required)
				throw new ArgumentException(localName + " can not be null.");
		}
		#endregion
		#region WriteSubElements
		private void writeSubElements(RssModuleItemCollection items, string NamespacePrefix)
		{
			foreach(RssModuleItem rssModuleItem in items)
			{
				if(rssModuleItem.SubElements.Count == 0)
					WriteElement(NamespacePrefix + ":" + rssModuleItem.Name, rssModuleItem.Text, rssModuleItem.IsRequired);
				else
				{
					writer.WriteStartElement(NamespacePrefix + ":" + rssModuleItem.Name);
					writeSubElements(rssModuleItem.SubElements, NamespacePrefix);
					writer.WriteEndElement();
				}
			}
		}
		#endregion
	}
}
