/* 
  	* AtomContentConstruct.cs
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
using Atom.Utils;
using MvpXml;

namespace Atom.Core
{
	/// <summary>
	/// The class representing all content constructs.
	/// </summary>
	[Serializable]
	public class AtomContentConstruct : AtomElement
	{
		private MediaType _type = DefaultValues.MediaType;
		private Mode _mode = DefaultValues.Mode;
		private string _content = String.Empty;

		#region Constructors

		/// <summary>
		/// Represents an <see cref="AtomContentConstruct"/> instance.
		/// Initialize the <see cref="AtomElement.LocalName"/> to "title" the <see cref="Content"/> to <see cref="String.Empty"/>,
		/// <see cref="Type"/> to <see cref="DefaultValues.MediaType"/> and
		/// <see cref="Mode"/> to <see cref="DefaultValues.Mode"/>.
		/// </summary>
		public AtomContentConstruct() : this("title")
		{
		}

		/// <summary>
		/// Represents an <see cref="AtomContentConstruct"/> instance initialized with the given content.
		/// </summary>
		/// <param name="localName">The not qualified name of the element.</param>
		public AtomContentConstruct(string localName) : this(localName, String.Empty)
		{
		}

		/// <summary>
		/// Represents an <see cref="AtomContentConstruct"/> instance initialized with the given content.
		/// </summary>
		/// <param name="localName">The not qualified name of the element.</param>
		/// <param name="content">The <see cref="Content"/> of the <see cref="AtomContentConstruct"/>.</param>
		public AtomContentConstruct(string localName, string content) : this(localName, content, DefaultValues.MediaType, DefaultValues.Mode)
		{
		}

		/// <summary>
		/// Represents an <see cref="AtomContentConstruct"/> instance initialized with the given content and <see cref="MediaType"/>.
		/// </summary>
		/// <param name="localName">The not qualified name of the element.</param>
		/// <param name="content">The <see cref="Content"/> of the <see cref="AtomContentConstruct"/>.</param>
		/// <param name="type">The <see cref="MediaType"/> of the <see cref="AtomContentConstruct"/>.</param>
		public AtomContentConstruct(string localName, string content, MediaType type) : this(localName, content, type, DefaultValues.Mode)
		{
		}

		/// <summary>
		/// Represents an <see cref="AtomContentConstruct"/> instance initialized with the given content and <see cref="Mode"/>.
		/// </summary>
		/// <param name="localName">The not qualified name of the element.</param>
		/// <param name="content">The <see cref="Content"/> of the <see cref="AtomContentConstruct"/>.</param>
		/// <param name="mode">The <see cref="Mode"/> of the <see cref="AtomContentConstruct"/>.</param>
		public AtomContentConstruct(string localName, string content, Mode mode) : this(localName, content, DefaultValues.MediaType, mode)
		{
		}

		/// <summary>
		/// Represents an <see cref="AtomContentConstruct"/> instance initialized with the given content,
		/// <see cref="MediaType"/> and <see cref="Mode"/>.
		/// </summary>
		/// <param name="localName">The not qualified name of the element.</param>
		/// <param name="content">The <see cref="Content"/> of the <see cref="AtomContentConstruct"/>.</param>
		/// <param name="type">The <see cref="MediaType"/> of the <see cref="AtomContentConstruct"/>.</param>
		/// <param name="mode">The <see cref="Mode"/> of the <see cref="AtomContentConstruct"/>.</param>
		public AtomContentConstruct(string localName, string content, MediaType type, Mode mode)
		{
			this.LocalName = localName;
			this.Content = content;
			this.Type = type;
			this.Mode = mode;
		}

		#endregion Constructors

		#region Properties

		/// <summary>
		/// The media type of the content.
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
		/// The encoding mode of the content.
		/// </summary>
		public Mode Mode
		{
			get { return _mode; }
			set { _mode = value; }
		}

		/// <summary>
		/// The content itself.
		/// </summary>
		public string Content
		{
			get { return _content; }
			set { _content = value; }
		}

		#endregion Properties

		#region ToString method
		
		/// <summary>
		/// Converts the <see cref="AtomContentConstruct"/> in a series of xml nodes.
		/// </summary>
		/// <returns>The string representation of <see cref="AtomContentConstruct"/> class.</returns>
		public override string ToString()
		{
			this.WriteStartElement();

			if(this.Content.Length == 0)
				throw new RequiredElementNotFoundException("The content cannot be empty.");

			if(this.Mode == Mode.Escaped)
				this.Content = Utils.Utils.Escape(this.Content);

			
			this.Buffer.Append(this.Content);
	
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

			if((this.Type == MediaType.UnknownType) ||
				(this.Type == MediaType.ApplicationAtomXml) ||
				(this.Type == MediaType.ApplicationXAtomXml))
				this.Type = DefaultValues.MediaType;

			if(this.Type != DefaultValues.MediaType)
				this.WriteAttribute("type", Utils.Utils.ParseMediaType(this.Type), false, null);

			if(this.Mode != DefaultValues.Mode)
				this.WriteAttribute("mode", this.Mode.ToString().ToLower(), false, null);
			this.WriteAttribute("xml:lang", Utils.Utils.ParseLanguage(this.XmlLang), false, null);

			this.Buffer.Append(">");
		}

		/// <summary>
		/// Writes the end tag of the element.
		/// </summary>
		protected internal override void WriteEndElement()
		{
			this.Buffer.AppendFormat("</{0}>", this.LocalName);
			this.Buffer.Append(Environment.NewLine);
		}

		#endregion

		#region XPath parsing stuff
		internal static AtomContentConstruct Parse(XPathNavigator navigator)
		{
			AtomContentConstruct contentElement = new AtomContentConstruct();
			string content = String.Empty;

			XPathNavigator nav = navigator.Clone();
			
			// select the element itself
			XPathNodeIterator iter = nav.SelectDescendants(XPathNodeType.Element, true);
			while(iter.MoveNext())
			{
				string name = iter.Current.Name.ToLower();
				int idx = name.IndexOf(":");
				if(idx != -1)
					name = name.Split(new char[] {':'}, 2)[1];

				switch(name)
				{
					case "title":
					case "copyright":
					case "info":
					case "tagline":
					case "summary":
					case "content":
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
				switch(iter.Current.Name.ToLower())
				{
					case "type":
						contentElement.Type = Utils.Utils.ParseMediaType(
							iter.Current.Value);
						break;

					case "mode":
					{
						switch(iter.Current.Value.ToLower())
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
