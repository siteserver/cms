/* 
  	* AtomLink.cs
	* [ part of Atom.NET library: http://atomnet.sourceforge.net ]
	* Author: Lawrence Oluyede
	* License: BSD-License (see below)
    
	Copyright (c) 2003, 2004 Lawrence Oluyede
    All rights reserved.

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
using System.Xml.XPath;
using Atom.Utils;

namespace Atom.Core
{
	/// <summary>
	/// The link of an Atom feed or an entry.
	/// <seealso cref="AtomFeed"/>
	/// <seealso cref="AtomEntry"/>
	/// </summary>
	[Serializable]
	public class AtomLink : AtomElement
	{
		private Relationship _rel = DefaultValues.Rel;
		private MediaType _type = DefaultValues.MediaType;
		private Uri _href = DefaultValues.Uri;
		private string _title = String.Empty;

		#region Constructors

		/// <summary>
		/// Represents an <see cref="AtomLink"/> instance.
		/// Initialize the <see cref="HRef"/> to <see cref="DefaultValues.Uri"/>,
		/// <see cref="Title"/> to <see cref="String.Empty"/>,
		/// <see cref="Rel"/> to <see cref="DefaultValues.Rel"/>
		/// and <see cref="Type"/> to <see cref="DefaultValues.MediaType"/>.
		/// </summary>
		public AtomLink() : this(DefaultValues.Uri, DefaultValues.Rel, DefaultValues.MediaType, String.Empty)
		{
		}

		/// <summary>
		/// Represents an <see cref="AtomLink"/> instance initialized with the given <see cref="Uri"/> and title.
		/// </summary>
		/// <param name="href">The <see cref="Uri"/> of the link.</param>
		/// <param name="rel">The <see cref="Relationship"/> of the link.</param>
		/// <param name="type">The <see cref="Type"/> of the link.</param>
		public AtomLink(Uri href, Relationship rel, MediaType type) : this(href, rel, type, String.Empty)
		{
		}

		/// <summary>
		/// Represents an <see cref="AtomLink"/> instance initialized with the given <see cref="Uri"/>,
		/// title, <see cref="Relationship"/> and <see cref="Type"/>.
		/// </summary>
		/// <param name="href">The <see cref="Uri"/> of the link.</param>
		/// <param name="rel">The <see cref="Relationship"/> of the link.</param>
		/// <param name="type">The <see cref="Type"/> of the link.</param>
		/// <param name="title">The <see cref="Title"/> of the link.</param>
		public AtomLink(Uri href, Relationship rel, MediaType type, string title)
		{
			this.HRef = href;
			this.Title = title;
			this.Rel = rel;
			this.Type = type;
		}

		#endregion Constructors

		/// <summary>
		/// Gets the local name of the link element.
		/// </summary>
		public override string LocalName
		{
			get { return "link"; }
		}

		#region Properties

		/// <summary>
		/// The relationship of the link.
		/// </summary>
		public Relationship Rel
		{
			get { return _rel; }
			set { _rel = value; }
		}

		/// <summary>
		/// The media type of the link.
		/// </summary>
		public MediaType Type
		{
			get { return _type; }
			set
			{
				if(value == MediaType.UnknownType)
					value = MediaType.TextPlain;
				else
					_type = value;
			}
		}

		/// <summary>
		/// The url of the link.
		/// </summary>
		public Uri HRef
		{
			get { return _href; }
			set { _href = value; }
		}

		/// <summary>
		/// The title of the link.
		/// </summary>
		public string Title
		{
			get { return _title; }
			set { _title = value; }
		}

		#endregion Properties

		#region ToString method
		
		/// <summary>
		/// Converts the <see cref="AtomLink"/> in a series of xml nodes.
		/// </summary>
		/// <returns>The string representation of <see cref="AtomLink"/> class.</returns>
		public override string ToString()
		{
			this.WriteStartElement();
			this.WriteAttribute("xml:lang", Utils.Utils.ParseLanguage(this.XmlLang), false, null);
			this.WriteAttribute("rel", Utils.Utils.ParseRelationship(this.Rel), false, "");

			if((this.Type == MediaType.UnknownType) ||
				(this.Type == MediaType.ApplicationAtomXml) ||
				(this.Type == MediaType.ApplicationXAtomXml))
				this.Type = DefaultValues.MediaType;

			this.WriteAttribute("type", Utils.Utils.ParseMediaType(this.Type), false, null);
			this.WriteAttribute("href", this.HRef, false, "");
			this.WriteAttribute("title", this.Title, false, "");
			
			this.WriteEndElement();

			string output = this.Buffer.ToString();
			this.Buffer.Length = 0;

			return output;
		}

		#endregion ToString method

		#region ToString helper methods

		/// <summary>
		/// Writes the start tag of the element.
		/// </summary>
		protected internal override void WriteStartElement()
		{
			this.Buffer.AppendFormat("<{0}", this.LocalName);
		}

		/// <summary>
		/// Writes the end tag of the element.
		/// </summary>
		protected internal override void WriteEndElement()
		{
			this.Buffer.Append(" />");
			this.Buffer.Append(Environment.NewLine);
		}


		#endregion

		#region XPath parsing stuff
		internal static AtomLink Parse(XPathNavigator navigator)
		{
			AtomLink linkElement = new AtomLink();

			XPathNavigator nav = navigator.Clone();
			XPathNodeIterator iter = nav.SelectDescendants(XPathNodeType.Element, true);

			while(iter.MoveNext())
			{
				switch(iter.Current.Name.ToLower())
				{
					case "link":
						try
						{
							linkElement.XmlLang = Utils.Utils.ParseLanguage(iter.Current.XmlLang);
						}
						catch {}
						break;
				}
			}

			// select the attributes
			iter = nav.Select("@*");
			do
			{
				switch(iter.Current.Name.ToLower())
				{
					case "rel":
						linkElement.Rel = Utils.Utils.ParseRelationship(iter.Current.Value);
						break;

					case "type":
						linkElement.Type = Utils.Utils.ParseMediaType(iter.Current.Value);
						break;

					case "href":
						linkElement.HRef = resolveUri(xmlBaseRootUri, iter.Current.Value);
						break;

					case "title":
						linkElement.Title = iter.Current.Value;
						break;
				}
			} while(iter.MoveNext());

			return linkElement;
		}
		#endregion
	}
}
