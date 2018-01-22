/* 
  	* AtomPersonConstruct.cs
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
	/// The class representing all person constructs.
	/// </summary>
	[Serializable]
	public class AtomPersonConstruct : AtomElement
	{
		private string _name = String.Empty;
		private Uri _url = DefaultValues.Uri;
		private string _email = String.Empty;

		#region Constructors

		/// <summary>
		/// Represents an <see cref="AtomPersonConstruct"/> instance.
		/// Initializes the <see cref="AtomElement.LocalName"/> to "author".
		/// </summary>
		public AtomPersonConstruct() : this("author")
		{
		}

		/// <summary>
		/// Represents an <see cref="AtomPersonConstruct"/> with the given local name.
		/// </summary>
		public AtomPersonConstruct(string localName) : this(localName, String.Empty)
		{
		}

		/// <summary>
		/// Represents an <see cref="AtomPersonConstruct"/> instance initialized with the given local name and the name of the person.
		/// </summary>
		/// <param name="localName">The not qualified name of the element.</param>
		/// <param name="name">The <see cref="Name"/> of the <see cref="AtomPersonConstruct"/>.</param>
		public AtomPersonConstruct(string localName, string name) : this(localName, name, DefaultValues.Uri)
		{
		}

		/// <summary>
		/// Represents an <see cref="AtomPersonConstruct"/> instance initialized with the given name and <see cref="Uri"/>.
		/// </summary>
		/// <param name="localName">The not qualified name of the element.</param>
		/// <param name="name">The <see cref="Name"/> of the <see cref="AtomPersonConstruct"/>.</param>
		/// <param name="url">The <see cref="Url"/> of the <see cref="AtomPersonConstruct"/>.</param>
		public AtomPersonConstruct(string localName, string name, Uri url) : this(localName, name, url, String.Empty)
		{
		}

		/// <summary>
		/// Represents an <see cref="AtomPersonConstruct"/> instance initialized with the given name, <see cref="Uri"/> and email.
		/// </summary>
		/// <param name="localName">The not qualified name of the element.</param>
		/// <param name="name">The <see cref="Name"/> of the <see cref="AtomPersonConstruct"/>.</param>
		/// <param name="url">The <see cref="Url"/> of the <see cref="AtomPersonConstruct"/>.</param>
		/// <param name="email">The <see cref="Email"/> of the <see cref="AtomPersonConstruct"/>.</param>
		public AtomPersonConstruct(string localName, string name, Uri url, string email)
		{
			this.LocalName = localName;
			this.Name = name;
			this.Url = url;
			this.Email = email;
		}

		#endregion Constructors

		#region Properties

		/// <summary>
		/// The name of the person.
		/// </summary>
		public string Name
		{
			get { return _name; }
			set { _name = value; }
		}

		/// <summary>
		/// The url associated with the person.
		/// </summary>
		public Uri Url
		{
			get { return _url; }
			set { _url = value; }
		}

		/// <summary>
		/// The email of the person.
		/// </summary>
		public string Email
		{
			get { return _email; }
			set
			{ 
				if(Utils.Utils.IsEmail(value))
					_email = value;
			}
		}

		#endregion Properties

		#region ToString methods
		
		/// <summary>
		/// Converts the <see cref="AtomPersonConstruct"/> in a series of xml nodes.
		/// </summary>
		/// <returns>The string representation of <see cref="AtomPersonConstruct"/> class.</returns>
		public override string ToString()
		{
            this.WriteStartElement();
			this.WriteAttribute("xml:lang", Utils.Utils.ParseLanguage(this.XmlLang), false, null);
			this.Buffer.Append(">");
			this.Buffer.Append(Environment.NewLine);
			this.WriteElement("name", this.Name, true, "The name of the author must be specified.");
			this.WriteElement("url", this.Url, false, null);
			this.WriteElement("email", this.Email, false, null);
			this.WriteEndElement();

			string output = this.Buffer.ToString();
			this.Buffer.Length = 0;

			return output;
		}

		#endregion ToString methods

		#region ToString helper methods

		/// <summary>
		/// Writes the start tag of the person construct element.
		/// </summary>
		protected internal override void WriteStartElement()
		{
			this.Buffer.AppendFormat("<{0}", this.LocalName);
		}

		/// <summary>
		/// Writes the end tag of the person construct element.
		/// </summary>
		protected internal override void WriteEndElement()
		{
			this.Buffer.AppendFormat("</{0}>", this.LocalName);
			this.Buffer.Append(Environment.NewLine);
		}

		#endregion

		#region XPath parsing stuff
		internal static AtomPersonConstruct Parse(XPathNavigator navigator)
		{
			AtomPersonConstruct personElement = new AtomPersonConstruct();

			XPathNavigator nav = navigator.Clone();
			XPathNodeIterator iter = nav.SelectDescendants(XPathNodeType.Element, true);
			
			while(iter.MoveNext())
			{
				string name = iter.Current.Name.ToLower();
				int idx = name.IndexOf(":");
				if(idx != -1)
					name = name.Split(new char[] {':'}, 2)[1];

				switch(name)
				{
					case "contributor":
					case "author":
						try
						{
							personElement.XmlLang = Utils.Utils.ParseLanguage(iter.Current.XmlLang);
						}
						catch {}
						personElement.LocalName = name;
						break;

					case "name":
						personElement.Name = iter.Current.Value;
						break;

					case "url":
						personElement.Url = resolveUri(xmlBaseRootUri, iter.Current.Value);
						break;

					case "email":
						personElement.Email = iter.Current.Value;
						break;
				}
			}

			return personElement;
		}

		#endregion
	}
}
