/* 
  	* AtomFeed.cs
	* [ part of Atom.NET library: http://atomnet.sourceforge.net ]
	* Author: Lawrence Oluyede
	* License: BSD-License (see below)
    
	Copyright (c) 2003, 2004 Lawrence Oluyede
    All rights reserved.
	
	Contains portions of
	RSS.NET (http://rss-net.sf.net/)
	Copyright ?2002, 2003 George Tsiokos. All Rights Reserved.

    Redistribution and use in source and binary forms, with or without
    modification, are permitted provided that the following conditions are met:

    * Redistributions of source code must retain the above copyright notice,
    * this list of conditions and the following disclaimer.
    * Redistributions in binary form must reproduce the above copyright
    * notice, this list of conditions and the following disclaimer in the
    * documentation and/or other materials provided with the distribution.
    * Neither the name of the copyright owner nor the names of its
    * contributors may be used to endorse or promote products derived from
    * this software without specific prior written permission.

    THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS"
    AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE
    IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE
    ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE
    LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR
    CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF
    SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS
    INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN
    CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE)
    ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE
    POSSIBILITY OF SUCH DAMAGE.
*/
using System;
using System.IO;
using System.Text;
using System.Net;
using System.Xml;
using System.Xml.XPath;
using Atom.Core.Collections;
using Atom.AdditionalElements;
using Atom.Utils;
using Atom.AdditionalElements.DublinCore;
using MvpXml;

namespace Atom.Core
{
	/// <summary>
	/// The Atom feed.
	/// <seealso cref="AtomEntry"/>
	/// </summary>
	[Serializable]
	public class AtomFeed : AtomElement
	{
		private AtomContentConstruct _title = null;
		private AtomLinkCollection _links = new AtomLinkCollection();
		private AtomPersonConstruct _author = null;
		private AtomPersonConstructCollection _contributors = new AtomPersonConstructCollection();
		private AtomContentConstruct _tagline = null;
		private Uri _id = DefaultValues.Uri;
		private Uri _feedUri = null;
		private AtomGenerator _generator = new AtomGenerator();
		private AtomContentConstruct _copyright = null;
		private AtomContentConstruct _info = null;
		private AtomDateConstruct _modified = null;
		private AtomEntryCollection _entries = new AtomEntryCollection();
		private ScopedElementCollection _additionalElements = new ScopedElementCollection();
		private Encoding _encoding = DefaultValues.Encoding;
		private static AtomReader _reader = null;
		private static AtomWriter _writer = null;

		/// <summary>
		/// Initialize a new instance of the <see cref="AtomFeed"/> class.
		/// </summary>
		public AtomFeed() {}

		#region Properties
		/// <summary>
		/// Gets the version of the Atom specification.
		/// </summary>
		public string Version
		{
			get { return DefaultValues.AtomVersion; }
		}

		/// <summary>
		/// Gets or sets the title of the feed.
		/// </summary>
		public AtomContentConstruct Title
		{
			get { return _title; }
			set { _title = value; }
		}

		/// <summary>
		/// Gets the links of the feed.
		/// </summary>
		public AtomLinkCollection Links
		{
			get { return _links; }
		}

		/// <summary>
		/// Gets or sets the author of the feed.
		/// </summary>
		public AtomPersonConstruct Author
		{
			get { return _author; }
			set { _author = value; }
		}

		/// <summary>
		/// Gets the contributors of the feed.
		/// </summary>
		public AtomPersonConstructCollection Contributors
		{
			get { return _contributors; }
		}

		/// <summary>
		/// Gets or sets tagline of the feed.
		/// </summary>
		public AtomContentConstruct Tagline
		{
			get { return _tagline; }
			set { _tagline = value; }
		}

		/// <summary>
		/// Gets or sets the global unique identifier of the feed.
		/// </summary>
		public Uri Id
		{
			get { return _id; }
			set { _id = value; }
		}

		/// <summary>
		/// Gets or sets the copyright of the feed.
		/// </summary>
		public AtomContentConstruct Copyright
		{
			get { return _copyright; }
			set { _copyright = value; }
		}

		/// <summary>
		/// Gets or sets the info of the feed.
		/// </summary>
		public AtomContentConstruct Info
		{
			get { return _info; }
			set { _info = value; }
		}

		/// <summary>
		/// Gets or sets last time when the feed has been modified.
		/// </summary>
		public AtomDateConstruct Modified
		{
			get { return _modified; }
			set { _modified = value; }
		}

		/// <summary>
		/// Gets the entries of the feed.
		/// </summary>
		public AtomEntryCollection Entries
		{
			get { return _entries; }
		}

		/// <summary>
		/// Gets or sets the encoding of the feed.
		/// </summary>
		public Encoding Encoding
		{
			get { return _encoding; }
			set { _encoding = value; }
		}

		/// <summary>
		/// Gets the additional elements of the feed.
		/// </summary>
		public ScopedElementCollection AdditionalElements
		{
			get { return _additionalElements; }
		}

		/// <summary>
		/// Gets the local name of the xml feed element.
		/// </summary>
		public override string LocalName
		{
			get { return "feed"; }
		}

		/// <summary>
		/// Gets or sets the uri of the atom feed if available, otherwise return null.
		/// </summary>
		public Uri Uri
		{
			get { return _feedUri; }
			set { _feedUri = value; }
		}
		#endregion Properties

		#region ToString method

		/// <summary>
		/// Converts the <see cref="AtomEntry"/> in a series of xml nodes.
		/// </summary>
		/// <returns>The string representation of <see cref="AtomEntry"/> class.</returns>
		public override string ToString()
		{
			this.Buffer.AppendFormat("<{0}", this.LocalName);

			this.WriteAttribute("version", DefaultValues.AtomVersion, false, null);
            this.WriteAttribute("xml:lang", Utils.Utils.ParseLanguage(this.XmlLang), false, null);
			this.WriteAttribute("xmlns", this.NamespaceUri, false, null);

			this.Buffer.Append(">");
			this.Buffer.Append(Environment.NewLine);
			
			#region AtomTitle

			if(this.Title == null)
				throw new RequiredElementNotFoundException("The title element must be specified.");

			this.Buffer.Append(this.Title.ToString());

			#endregion

			#region AtomLink

			foreach(AtomLink link in this.Links)
				this.Buffer.Append(link.ToString());

			#endregion

			#region AtomAuthor

			// check if all the contained entries have an author
			foreach(AtomEntry entry in this.Entries)
			{
				if(!entry.doAuthor)
				{
					if(this.Author == null)
						throw new RequiredElementNotFoundException("The author element must be specified.");

					this.Buffer.Append(this.Author.ToString());
					break;
				}
				else
					break;
			}

			#endregion

			#region AtomContributor

			foreach(AtomPersonConstruct contributor in this.Contributors)
				this.Buffer.Append(contributor.ToString());

			#endregion

			#region AtomTagline

			if(this.Tagline != null)
				this.Buffer.Append(this.Tagline.ToString());

			#endregion

			#region Id

			if(this.Id != DefaultValues.Uri)
				this.WriteElement("id", this.Id, false, null);

			#endregion

			#region AtomGenerator

			this.Buffer.Append(this._generator.ToString());
            
			#endregion

			#region AtomCopyright

			if(this.Copyright != null)
				this.Buffer.Append(this.Copyright.ToString());

			#endregion

			#region AtomInfo

			if(this.Info != null)
				this.Buffer.Append(this.Info.ToString());

			#endregion

			#region AtomModified

			if(this.Modified == null)
				throw new RequiredElementNotFoundException("The modified element must be specified.");

			this.Buffer.Append(this.Modified.ToString());

			#endregion

			#region AdditionalElements
			foreach(ScopedElement element in AdditionalElements)
				this.Buffer.Append(element.ToString());
			#endregion

			#region AtomEntry

			foreach(AtomEntry entry in this.Entries)
				this.Buffer.Append(entry.ToString());

			#endregion

			this.Buffer.AppendFormat("</{0}>", this.LocalName);
			this.Buffer.Append(Environment.NewLine);

			string output = this.Buffer.ToString();
			this.Buffer.Length = 0;

			return output;
		}

		#endregion

		#region Save methods

		/// <summary>
		/// Saves the <see cref="AtomFeed"/> to the specified <see cref="Stream"/>.
		/// </summary>
		/// <param name="stream">The <see cref="Stream"/> to write to.</param>
		/// <exception cref="ArgumentException">The encoding is not supported or the stream cannot be written to.</exception>
		/// <exception cref="ArgumentNullException">stream is null.</exception>
		/// <exception cref="RequiredElementNotFoundException">A required element is not found.</exception>
		/// <exception cref="InvalidOperationException">The internal writ</exception>
		public void Save(Stream stream)
		{
			_writer = new AtomWriter(stream, this.Encoding);
			_writer.Write(this);
		}

		/// <summary>
		/// Saves the <see cref="AtomFeed"/> to the specified filename.
		/// </summary>
		/// <exception cref="ArgumentException">The encoding is not supported; the filename is empty, contains only white space, or contains one or more invalid characters.</exception>
		/// <exception cref="UnauthorizedAccessException">Access is denied.</exception>
		/// <exception cref="ArgumentNullException">The filename is a null reference.</exception>
		/// <exception cref="DirectoryNotFoundException">The directory to write to is not found.</exception>
		/// <exception cref="IOException">The filename includes an incorrect or invalid syntax for file name, directory name, or volume label syntax.</exception>
		/// <exception cref="System.Security.SecurityException">The caller does not have the required permission.</exception>
		/// <param name="filename">The file name to write to.</param>
		public void Save(string filename)
		{
			_writer = new AtomWriter(filename, this.Encoding);
			_writer.Write(this);
		}

		/// <summary>
		/// Saves the <see cref="AtomFeed"/> to the specified <see cref="TextWriter"/>.
		/// </summary>
		/// <param name="tw">The <see cref="TextWriter"/> to write to.</param>
		public void Save(TextWriter tw)
		{
			_writer = new AtomWriter(tw);
			_writer.Write(this);
		}

		/// <summary>
		/// Saves the <see cref="AtomFeed"/> to the specified <see cref="TextWriter"/>.
		/// </summary>
		/// <param name="xw">The <see cref="XmlWriter"/> to write to.</param>
		public void Save(XmlWriter xw)
		{
			_writer = new AtomWriter(xw);
			_writer.Write(this);
		}

		/// <summary>
		/// Saves the <see cref="AtomFeed"/> to the specified <see cref="HttpWebResponse"/>.
		/// </summary>
		/// <param name="response">The <see cref="WebResponse"/> to write to.</param>
		public void Save(WebResponse response)
		{
			_writer = new AtomWriter(response.ResponseUri.ToString(), this.Encoding);
			_writer.Write(this);
		}

		#endregion

		#region Load methods

		/// <summary>
		/// Loads the <see cref="AtomFeed"/> from the specified URI.
		/// </summary>
		/// <returns>An <see cref="AtomFeed"/> instance.</returns>
		/// <param name="uri">The URI of the resource containing the Atom xml data.</param>
		/// <exception cref="XmlException">Invalid xml syntax in Atom resource or uri is null.</exception>
		/// <exception cref="ArgumentException">The resource cannot be find.</exception>
		/// <exception cref="InvalidOperationException"><see cref="AtomReader"/> has been closed, and can not be read.</exception>
		/// <exception cref="FileNotFoundException">Atom xml resource not found.</exception>
		public static AtomFeed Load(string uri)
		{
			_reader = new AtomReader(uri);
			return Parse(_reader.Navigator);
		}

		/// <summary>
		/// Loads the <see cref="AtomFeed"/> from the specified <see cref="System.Uri"/>.
		/// </summary>
		/// <returns>An <see cref="AtomFeed"/> instance.</returns>
		/// <param name="uri">The URI of the resource containing the Atom xml data.</param>
		/// <exception cref="XmlException">Invalid xml syntax in Atom resource or uri is null.</exception>
		/// <exception cref="ArgumentException">The resource cannot be find.</exception>
		/// <exception cref="InvalidOperationException"><see cref="AtomReader"/> has been closed, and can not be read.</exception>
		/// <exception cref="FileNotFoundException">Atom xml resource not found.</exception>
		public static AtomFeed Load(Uri uri)
		{
			return Load(uri.ToString());
		}

		/// <summary>
		/// Loads the <see cref="AtomFeed"/> from the specified <see cref="Stream"/>.
		/// </summary>
		/// <returns>An <see cref="AtomFeed"/> instance.</returns>
		/// <param name="stream">The <see cref="Stream"/> containing the Atom xml data.</param>
		/// <exception cref="XmlException">Invalid xml syntax in Atom resource or uri is null.</exception>
		/// <exception cref="ArgumentException">The resource cannot be find.</exception>
		/// <exception cref="InvalidOperationException"><see cref="AtomReader"/> has been closed, and can not be read.</exception>
		/// <exception cref="FileNotFoundException">Atom xml resource not found.</exception>
		public static AtomFeed Load(Stream stream)
		{
			_reader = new AtomReader(stream);
			return Parse(_reader.Navigator);
		}

		/// <summary>
		/// Loads the <see cref="AtomFeed"/> from the specified <see cref="TextReader"/>.
		/// </summary>
		/// <returns>An <see cref="AtomFeed"/> instance.</returns>
		/// <param name="tr">The <see cref="TextReader"/> from which read the Atom xml data.</param>
		/// <exception cref="XmlException">Invalid xml syntax in Atom resource or uri is null.</exception>
		/// <exception cref="ArgumentException">The resource cannot be find.</exception>
		/// <exception cref="InvalidOperationException"><see cref="AtomReader"/> has been closed, and can not be read.</exception>
		/// <exception cref="FileNotFoundException">Atom xml resource not found.</exception>
		public static AtomFeed Load(TextReader tr)
		{
			_reader = new AtomReader(tr);
			return Parse(_reader.Navigator);
		}

		/// <summary>
		/// Loads the <see cref="AtomFeed"/> from the specified <see cref="HttpWebRequest"/>.
		/// </summary>
		/// <returns>An <see cref="AtomFeed"/> instance.</returns>
		/// <param name="request">The <see cref="WebRequest"/> from which read the Atom xml data.</param>
		/// <exception cref="XmlException">Invalid xml syntax in Atom resource or uri is null.</exception>
		/// <exception cref="ArgumentException">The resource cannot be find.</exception>
		/// <exception cref="InvalidOperationException"><see cref="AtomReader"/> has been closed, and can not be read.</exception>
		/// <exception cref="FileNotFoundException">Atom xml resource not found.</exception>
		public static AtomFeed Load(WebRequest request)
		{
			_reader = new AtomReader(request.RequestUri.ToString());
			return Parse(_reader.Navigator);
		}

		/// <summary>
		/// Loads the <see cref="AtomFeed"/> from the specified <see cref="XmlReader"/>.
		/// </summary>
		/// <returns>An <see cref="AtomFeed"/> instance.</returns>
		/// <param name="reader">The <see cref="XmlReader"/> from which read the Atom xml data.</param>
		/// <exception cref="XmlException">Invalid xml syntax in Atom resource or uri is null.</exception>
		/// <exception cref="ArgumentException">The resource cannot be find.</exception>
		/// <exception cref="InvalidOperationException"><see cref="AtomReader"/> has been closed, and can not be read.</exception>
		/// <exception cref="FileNotFoundException">Atom xml resource not found.</exception>
		public static AtomFeed Load(XmlReader reader)
		{
			_reader = new AtomReader(reader);
			return Parse(_reader.Navigator);
		}

		/// <summary>
		/// Loads the <see cref="AtomFeed"/> from the specified xml fragment.
		/// </summary>
		/// <returns>An <see cref="AtomFeed"/> instance.</returns>
		/// <param name="xmlFragment">The string containing the Atom xml data.</param>
		/// <exception cref="XmlException">Invalid xml syntax in Atom resource or uri is null.</exception>
		/// <exception cref="ArgumentException">The resource cannot be find.</exception>
		/// <exception cref="InvalidOperationException"><see cref="AtomReader"/> has been closed, and can not be read.</exception>
		/// <exception cref="FileNotFoundException">Atom xml resource not found.</exception>
		public static AtomFeed LoadXml(string xmlFragment)
		{
			XmlTextReader xmlReader = new XmlTextReader(xmlFragment, XmlNodeType.Element, null);
			_reader = new AtomReader(xmlReader);
			return Parse(_reader.Navigator);
		}

		#endregion

		#region XPath parsing stuff
		internal static AtomFeed Parse(XPathNavigator navigator)
		{
			AtomFeed feed = new AtomFeed();

			XPathNavigator nav = navigator.Clone();
			XPathNodeIterator iter = nav.SelectChildren(XPathNodeType.All);
			
			do
			{
				string name = iter.Current.Name.ToLower();
				int idx = name.IndexOf(":");
				if(idx != -1)
					name = name.Split(new char[] {':'}, 2)[0];

				switch(name)
				{
					case "feed":
					{
						try
						{
							XPathNavigatorReader reader = new XPathNavigatorReader(nav);
							string baseUri = reader.GetAttribute("base", XmlNamespaces.Xml);
							if(baseUri != null && baseUri.Length > 0)
								feed.XmlBase =  new Uri(baseUri);
						}
						catch {}

						try
						{
							feed.Uri = FindAlternateUri(iter.Current);
						}
						catch {}

						try
						{
							feed.XmlLang = Utils.Utils.ParseLanguage(iter.Current.XmlLang);
						}
						catch {}

						XPathNodeIterator attrIterator = nav.Select("@*");
						while(attrIterator.MoveNext())
						{
							if(attrIterator.Current.Name.ToLower() == "version")
							{
								if(attrIterator.Current.Value != DefaultValues.AtomVersion)
								{
									string msg = string.Format("Atom {0} version is not supported!", attrIterator.Current.Value);
									throw new InvalidOperationException(msg);
								}
							}
						}
						break;
					}

					case "title":
						AtomContentConstruct content = AtomContentConstruct.Parse(iter.Current);
						feed.Title = content;
						break;

					case "link":
						feed.Links.Add(AtomLink.Parse(iter.Current));
						break;

					case "author":
						feed.Author = AtomPersonConstruct.Parse(iter.Current);
						break;

					case "contributor":
						feed.Contributors.Add(AtomPersonConstruct.Parse(iter.Current));
						break;

					case "tagline":
						feed.Tagline = AtomContentConstruct.Parse(iter.Current);
						break;

					case "id":
						feed.Id = new Uri(iter.Current.Value);
						break;
			
					case "copyright":
						feed.Copyright = AtomContentConstruct.Parse(iter.Current);
						break;

					case "info":
						feed.Info = AtomContentConstruct.Parse(iter.Current);
						break;

					case "modified":
						feed.Modified = AtomDateConstruct.Parse(iter.Current);
						break;

					case "entry":
						feed.Entries.Add(AtomEntry.Parse(iter.Current));
						break;

					case "dc":
						feed.AdditionalElements.Add(DcElement.Parse(iter.Current));
						break;

				}
			} while(iter.MoveNext());

			return feed;
		}
		#endregion

		#region private stuff
		private static Uri FindAlternateUri(XPathNavigator navigator)
		{
			Uri uri = null;
			XPathNavigator nav = navigator.Clone();
			XPathNodeIterator iter;

			XmlNamespaceManager nsm = new XmlNamespaceManager(nav.NameTable);
			nsm.AddNamespace(DefaultValues.AtomNSPrefix, DefaultValues.AtomNSUri.ToString());

			XPathExpression expr = nav.Compile("child::atom:link[@type=\"text/html\" and @rel=\"alternate\"]");
			expr.SetContext(nsm);
			iter = nav.Select(expr);

			if(iter.Count == 0)
			{
				expr = nav.Compile("child::atom:link[@type=\"text/plain\" and @rel=\"alternate\"]");
				expr.SetContext(nsm);
				iter = nav.Select(expr);
			}

			if(iter.CurrentPosition == 0)
				iter.MoveNext();

			// select the attributes
			iter = iter.Current.Select("@*");
			do
			{
				switch(iter.Current.Name.ToLower())
				{
					case "href":
						try
						{
							uri = resolveUri(xmlBaseRootUri, iter.Current.Value);
						}
						catch {}
						break;
				}
			} while(iter.MoveNext());


			return uri;
		}
		#endregion
	}
}
