/* 
  	* AtomContent.cs
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
using System.Text;
using System.Xml.XPath;
using SSCMS.Core.Utils.Serialization.Atom.Atom.Utils;
using SSCMS.Core.Utils.Serialization.MvpXml;
using SSCMS.Utils;

namespace SSCMS.Core.Utils.Serialization.Atom.Atom.Core
{
	/// <summary>
	/// The content of an Atom entry.
	/// <seealso cref="AtomEntry"/>
	/// </summary>
	[Serializable]
	public class AtomContent : AtomContentConstruct
	{
		#region Constructors

		/// <summary>
		/// Represents an <see cref="AtomContent"/> instance.
		/// Initialize the <see cref="AtomContentConstruct.Content"/> to <see cref="String.Empty"/>,
		/// <see cref="AtomContentConstruct.Type"/> to <see cref="DefaultValues.MediaType"/> and
		/// <see cref="AtomContentConstruct.Mode"/> to <see cref="DefaultValues.Mode"/>.
		/// </summary>
		public AtomContent() : this(String.Empty, DefaultValues.MediaType, DefaultValues.Mode)
		{
		}

		/// <summary>
		/// Represents an <see cref="AtomContent"/> instance initialized with the given content.
		/// </summary>
		/// <param name="content">The <see cref="AtomContentConstruct.Content"/> of the <see cref="AtomContent"/>.</param>
		public AtomContent(string content) : this(content, DefaultValues.MediaType, DefaultValues.Mode)
		{
		}

		/// <summary>
		/// Represents an <see cref="AtomContent"/> instance initialized with the given content and <see cref="MediaType"/>.
		/// </summary>
		/// <param name="content">The <see cref="AtomContentConstruct.Content"/> of the <see cref="AtomContent"/>.</param>
		/// <param name="type">The <see cref="MediaType"/> of the <see cref="AtomContent"/>.</param>
		public AtomContent(string content, MediaType type) : this(content, type, DefaultValues.Mode)
		{
		}

		/// <summary>
		/// Represents an <see cref="AtomContent"/> instance initialized with the given content and <see cref="Mode"/>.
		/// </summary>
		/// <param name="content">The <see cref="AtomContentConstruct.Content"/> of the <see cref="AtomContent"/>.</param>
		/// <param name="mode">The <see cref="Mode"/> of the <see cref="AtomContent"/>.</param>
		public AtomContent(string content, Mode mode) : this(content, DefaultValues.MediaType, mode)
		{
		}

		/// <summary>
		/// Represents an <see cref="AtomContent"/> instance initialized with the given content,
		/// <see cref="MediaType"/> and <see cref="Mode"/>.
		/// </summary>
		/// <param name="content">The <see cref="AtomContentConstruct.Content"/> of the <see cref="AtomContent"/>.</param>
		/// <param name="type">The <see cref="MediaType"/> of the <see cref="AtomContent"/>.</param>
		/// <param name="mode">The <see cref="Mode"/> of the <see cref="AtomContent"/>.</param>
		public AtomContent(string content, MediaType type, Mode mode)
		{
			Content = content;
			Type = type;
			Mode = mode;
		}

		#endregion Constructors

		/// <summary>
		/// Gets the local name of the content construct.
		/// </summary>
		public override string LocalName
		{
			get { return "content"; }
		}

		#region ToString helper methods
		/// <summary>
		/// Writes the start tag of the element.
		/// </summary>
		protected internal override void WriteStartElement()
		{
			Buffer.AppendFormat("<{0}", LocalName);

			if((Type == MediaType.UnknownType) ||
				(Type == MediaType.ApplicationAtomXml) ||
				(Type == MediaType.ApplicationXAtomXml))
				Type = DefaultValues.MediaType;
			
			WriteAttribute("xml:base", XmlBase, false, null);

			if(Type != DefaultValues.MediaType)
				WriteAttribute("type", Utils.Utils.ParseMediaType(Type), false, null);

			if(Type != MediaType.MultipartAlternative)
				if(Mode != DefaultValues.Mode)
					WriteAttribute("mode", StringUtils.ToLower(Mode.ToString()), false, null);

			WriteAttribute("xml:lang", Utils.Utils.ParseLanguage(XmlLang), false, null);
			Buffer.Append(">");
		}

		#endregion

		#region XPath parsing stuff
		internal new static AtomContent Parse(XPathNavigator navigator)
		{
			AtomContent contentElement = new AtomContent();
			string content = String.Empty;

			XPathNavigator nav = navigator.Clone();
			
			// select the element itself
			XPathNodeIterator iter = nav.SelectDescendants(XPathNodeType.Element, true);
			while(iter.MoveNext())
			{
				string name = StringUtils.ToLower(iter.Current.Name);
				int idx = name.IndexOf(":");
				if(idx != -1)
					name = name.Split(new char[] {':'}, 2)[1];

				switch(name)
				{
					case "content":
						try
						{
							XPathNavigatorReader navReader = new XPathNavigatorReader(nav);
							string baseUri = navReader.GetAttribute("base", XmlNamespaces.Xml);
							if(baseUri != null && baseUri.Length > 0)
								contentElement.XmlBase =  new Uri(baseUri);
						}
						catch {}

						try
						{
							contentElement.XmlLang = Utils.Utils.ParseLanguage(iter.Current.XmlLang);
						}
						catch {}
						contentElement.LocalName = name;
						XPathNavigatorReader reader = new XPathNavigatorReader(iter.Current);
						reader.Read();
						content = reader.ReadInnerXml();
						break;
				}
			}
			
			// select the attributes
			iter = nav.Select("@*");
			do
			{
				switch(StringUtils.ToLower(iter.Current.Name))
				{
					case "type":
						contentElement.Type = Utils.Utils.ParseMediaType(
							iter.Current.Value);
						break;

					case "mode":
					{
						switch(StringUtils.ToLower(iter.Current.Value))
						{
							case "escaped":
								contentElement.Mode = Mode.Escaped;
								break;

							case "base64":
								contentElement.Mode = Mode.Base64;
								break;
						}
						break;
					}
				}
			} while(iter.MoveNext());

			switch(contentElement.Mode)
			{
				case Mode.Escaped:
					content = Utils.Utils.Unescape(content);
					break;

				case Mode.Base64:
					content = Encoding.Unicode.GetString(
						Utils.Utils.Base64Decode(content));
					break;
			}

			contentElement.Content = content;
			return contentElement;
		}
		#endregion
	}
}
