/* 
  	* AtomElement.cs
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
using System.Xml;
using Atom.Utils;

namespace Atom.Core
{
	/// <summary>
	/// The base class for all elements.
	/// </summary>
	[Serializable]
    public abstract class AtomElement
    {
		private StringBuilder _buffer;
		private string _name = String.Empty;
		private Language _xmllang = DefaultValues.Language;
		private Uri _xmlbase = DefaultValues.Uri;
		/// <summary>
		/// The root xml:base uri. It's for internal use.
		/// </summary>
		protected internal static string xmlBaseRootUri = String.Empty;

		/// <summary>
		/// Initialize a new instance of the <see cref="AtomElement"/> class.
		/// </summary>
		protected AtomElement()
		{
			_buffer = new StringBuilder();
		}

		#region Properties
		/// <summary>
		/// The local name of the element.
		/// </summary>
		public virtual string LocalName
		{
			get { return _name; }
			set { _name = value; }
		}

		/// <summary>
		/// The namespace prefix of the element.
		/// </summary>
		public virtual string NamespacePrefix
		{
			get { return DefaultValues.AtomNSPrefix; }
		}

		/// <summary>
		/// The namespace uri of the element.
		/// </summary>
		public virtual Uri NamespaceUri
		{
			get { return DefaultValues.AtomNSUri; }
		}

		/// <summary>
		/// The full qualified name of the element.
		/// </summary>
		public virtual string FullName
		{
			get { return String.Concat(NamespacePrefix, ":", LocalName); }
		}

		/// <summary>
		/// The content's language of the current element.
		/// </summary>
		public Language XmlLang
		{
			get { return _xmllang; }
			set { _xmllang = value; }
		}

		/// <summary>
		/// The xml:base attribute uri.
		/// </summary>
		public Uri XmlBase
		{
			get { return _xmlbase; }
			set
			{
				_xmlbase = value;
				if(this is AtomFeed)
					xmlBaseRootUri = value.ToString();
			}
		}

		/// <summary>
		/// The serialization buffer.
		/// Use it in your own subclasses for serialization purposes.
		/// </summary>
		protected internal StringBuilder Buffer
		{
			get { return _buffer; }
		}
		#endregion

		#region Writing stuff

		/// <summary>
		/// Override this to write the start tag of the atom element.
		/// </summary>
		protected internal virtual void WriteStartElement() {}
		/// <summary>
		/// Override this to write the end tag of the atom element.
		/// </summary>
		protected internal virtual void WriteEndElement() {}

		#region WriteElement helper methods

		/// <summary>
		/// Writes an element into the flow.
		/// </summary>
		/// <param name="localName">The local name of the element.</param>
		/// <param name="input">The <see cref="DateTime"/> to write.</param>
		/// <param name="required">true if the element is required, false otherwise</param>
		/// <param name="message">The error message if the element is missing.</param>
		protected internal void WriteElement(string localName, DateTime input, bool required, string message)
		{
			if(input != DefaultValues.DateTime)
			{
				this.Buffer.AppendFormat("<{0}>", localName);
				this.Buffer.Append(Convert.ToString(input));
				this.Buffer.AppendFormat("</{0}>", localName);
				this.Buffer.Append(Environment.NewLine);
			}
			else if(required)
				throw new RequiredElementNotFoundException(localName + " " + message);
		}

		/// <summary>
		/// Writes an element into the flow.
		/// </summary>
		/// <param name="localName">The local name of the element.</param>
		/// <param name="input">The int value to write.</param>
		/// <param name="required">true if the element is required, false otherwise</param>
		/// <param name="message">The error message if the element is missing.</param>
		protected internal void WriteElement(string localName, int input, bool required, string message)
		{
			if(input != DefaultValues.Int)
			{
				this.Buffer.AppendFormat("<{0}>", localName);
				this.Buffer.Append(Convert.ToString(input));
				this.Buffer.AppendFormat("</{0}>", localName);
				this.Buffer.Append(Environment.NewLine);
			}
			else if(required)
				throw new RequiredElementNotFoundException(localName + " " + message);
		}

		/// <summary>
		/// Writes an element into the flow.
		/// </summary>
		/// <param name="localName">The local name of the element.</param>
		/// <param name="input">The string to write.</param>
		/// <param name="required">true if the element is required, false otherwise</param>
		/// <param name="message">The error message if the element is missing.</param>
		protected internal void WriteElement(string localName, string input, bool required, string message)
		{
			if(input.Length != 0)
			{
				this.Buffer.AppendFormat("<{0}>", localName);
				this.Buffer.Append(input);
				this.Buffer.AppendFormat("</{0}>", localName);
				this.Buffer.Append(Environment.NewLine);
			}
			else if(required)
				throw new RequiredElementNotFoundException(localName + " " + message);
		}

		/// <summary>
		/// Writes an element into the flow.
		/// </summary>
		/// <param name="localName">The local name of the element.</param>
		/// <param name="input">The <see cref="Uri"/> to write.</param>
		/// <param name="required">true if the element is required, false otherwise</param>
		/// <param name="message">The error message if the element is missing.</param>
		protected internal void WriteElement(string localName, Uri input, bool required, string message)
		{
			if(input != DefaultValues.Uri)
			{
				this.Buffer.AppendFormat("<{0}>", localName);
				this.Buffer.Append(Convert.ToString(input));
				this.Buffer.AppendFormat("</{0}>", localName);
				this.Buffer.Append(Environment.NewLine);
			}
			else if(required)
				throw new RequiredElementNotFoundException(localName + " " + message);
		}

		/// <summary>
		/// Writes an element into the flow.
		/// </summary>
		/// <param name="localName">The local name of the element.</param>
		/// <param name="input">The <see cref="object"/> to write.</param>
		/// <param name="required">true if the element is required, false otherwise</param>
		/// <param name="message">The error message if the element is missing.</param>
		protected internal void WriteElement(string localName, object input, bool required, string message)
		{
			if(input != DefaultValues.Uri)
			{
				this.Buffer.AppendFormat("<{0}>", localName);
				this.Buffer.Append(Convert.ToString(input));
				this.Buffer.AppendFormat("</{0}>", localName);
				this.Buffer.Append(Environment.NewLine);
			}
			else if(required)
				throw new RequiredElementNotFoundException(localName + " " + message);
		}

		#endregion WriteElement helper methods

		#region WriteAttribute helper methods

		/// <summary>
		/// Writes an attribute into the flow.
		/// </summary>
		/// <param name="localName">The local name of the attribute.</param>
		/// <param name="input">The <see cref="DateTime"/> to write.</param>
		/// <param name="required">true if the element is required, false otherwise</param>
		/// <param name="message">The error message if the attribute is missing.</param>
		protected internal void WriteAttribute(string localName, DateTime input, bool required, string message)
		{
			if(input != DefaultValues.DateTime)
				this.Buffer.AppendFormat(" {0}=\"{1}\"", localName, Convert.ToString(input));
			else if(required)
				throw new RequiredAttributeNotFoundException(localName + " " + message);
		}

		/// <summary>
		/// Writes an attribute into the flow.
		/// </summary>
		/// <param name="localName">The local name of the attribute.</param>
		/// <param name="input">The int value to write.</param>
		/// <param name="required">true if the element is required, false otherwise</param>
		/// <param name="message">The error message if the attribute is missing.</param>
		protected internal void WriteAttribute(string localName, int input, bool required, string message)
		{
			if(input != DefaultValues.Int)
				this.Buffer.AppendFormat(" {0}=\"{1}\"", localName, Convert.ToString(input));
			else if(required)
				throw new RequiredAttributeNotFoundException(localName + " " + message);
		}

		/// <summary>
		/// Writes an attribute into the flow.
		/// </summary>
		/// <param name="localName">The local name of the attribute.</param>
		/// <param name="input">The string to write.</param>
		/// <param name="required">true if the element is required, false otherwise</param>
		/// <param name="message">The error message if the attribute is missing.</param>
		protected internal void WriteAttribute(string localName, string input, bool required, string message)
		{
			if(input.Length != 0)
				this.Buffer.AppendFormat(" {0}=\"{1}\"", localName, input);
			else if(required)
				throw new RequiredAttributeNotFoundException(localName + " " + message);
		}

		/// <summary>
		/// Writes an attribute into the flow.
		/// </summary>
		/// <param name="localName">The local name of the attribute.</param>
		/// <param name="input">The <see cref="Uri"/> to write.</param>
		/// <param name="required">true if the element is required, false otherwise</param>
		/// <param name="message">The error message if the attribute is missing.</param>
		protected internal void WriteAttribute(string localName, Uri input, bool required, string message)
		{
			if(input != DefaultValues.Uri)
				this.Buffer.AppendFormat(" {0}=\"{1}\"", localName, Convert.ToString(input));
			else if(required)
				throw new RequiredAttributeNotFoundException(localName + " " + message);
		}

		/// <summary>
		/// Writes an attribute into the flow.
		/// </summary>
		/// <param name="localName">The local name of the attribute.</param>
		/// <param name="input">The <see cref="object"/> to write.</param>
		/// <param name="required">true if the element is required, false otherwise</param>
		/// <param name="message">The error message if the attribute is missing.</param>
		protected internal void WriteAttribute(string localName, object input, bool required, string message)
		{
			if(input != null)
				this.Buffer.AppendFormat(" {0}=\"{1}\"", localName, Convert.ToString(input));
			else if(required)
				throw new RequiredAttributeNotFoundException(localName + " " + message);
		}

		#endregion WriteAttribute helper methods

		#endregion

		#region Xml Base processing
		/// <summary>
		/// Resolves relative URIs.
		/// </summary>
		/// <param name="baseURI">The base uri.</param>
		/// <param name="path">The path to resolve.</param>
		/// <returns>The resolved <see cref="Uri"/>.</returns>
		protected internal static Uri resolveUri(string baseURI, string path)
		{
			if(baseURI.Length > 0)
			{
				XmlUrlResolver resolver = new XmlUrlResolver();
				return resolver.ResolveUri(new Uri(baseURI), path);
			}
			return new Uri(path);
		}
		#endregion
    }
}
