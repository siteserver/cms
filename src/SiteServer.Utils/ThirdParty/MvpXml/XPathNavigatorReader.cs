/* 
  	* XPathNavigatorReader.cs
	* [ part of MVP-XML library: http://sourceforge.net/projects/mvp-xml ]
	* Author: Daniel Cazzulino, kzu@aspnet2.com
	* License: BSD-License (see below)
    
	Copyright (c) 2003, 2004 Daniel Cazzulino, kzu@aspnet2.com
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
#region using

using System;
using System.Collections;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using System.Xml.XPath;

#endregion using

namespace MvpXml
{
	/// <summary>
	/// Provides an <see cref="XmlReader"/> over an 
	/// <see cref="XPathNavigator"/>.
	/// </summary>
	/// <remarks>
	/// Reader is positioned at the current navigator position. Reading 
	/// it completely is similar to querying for the <see cref="XmlNode.OuterXml"/> 
	/// property. An additional option is to specify that the reader should 
	/// expose an XML fragment with the current navigator and all its following siblings. 
	/// This is done at construction time.
	/// <para>The navigator is cloned at construction time to avoid side-effects 
	/// in calling code.</para>
	/// <para>Author: Daniel Cazzulino, kzu@aspnet2.com</para>
	/// See: http://weblogs.asp.net/cazzu/archive/2004/04/19/115966.aspx and 
	/// http://weblogs.asp.net/cazzu/archive/2004/05/10/129101.aspx.
	/// </remarks>
	internal class XPathNavigatorReader : XmlTextReader, IXmlSerializable
	{
		#region Fields

		// Cursor that will be moved by the reader methods.
		XPathNavigator _navigator;
		// Cursor remaining in the original position, to determine EOF.
		XPathNavigator _original;
		// Whether the	reader should expose an XML fragment.
		bool _fragment = false;
		// Will track whether we're at a faked end element
		bool _isendelement = false;

		#endregion Fields

		#region Ctors

		/// <summary>
		/// Parameterless constructor for XML serialization.
		/// </summary>
		/// <remarks>Supports the .NET serialization infrastructure. Don't use this 
		/// constructor in your regular application.</remarks>
		[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
		public XPathNavigatorReader()
		{
		}

		/// <summary>
		/// Initializes the reader.
		/// </summary>
		/// <param name="navigator">The navigator to expose as a reader.</param>
		public XPathNavigatorReader(XPathNavigator navigator)
			: this(navigator, false)
		{
		}

		/// <summary>
		/// Initializes the reader.
		/// </summary>
		/// <param name="navigator">The navigator to expose as a reader.</param>
		/// <param name="readFragment">Specifies that the reader should expose 
		/// as an XML fragment the current navigator node and all its following siblings.</param>
		public XPathNavigatorReader(XPathNavigator navigator, bool readFragment)
			: base(new StringReader(String.Empty))
		{
			_navigator = navigator.Clone();
			_original = navigator.Clone();
			_fragment = readFragment;
		}

		#endregion Ctors

		#region Private Members

		/// <summary>
		/// Retrieves and caches node positions and their name/ns
		/// </summary>
		private ArrayList OrderedAttributes
		{
			get
			{
				// List contains the following values: string[] { name, namespaceURI }
				if (_orderedattributes != null) return _orderedattributes;

				// Cache attributes position and names.
				// We do this because when an attribute is accessed by index, it's 
				// because of a usage pattern using a for loop as follows:
				// for (int i = 0; i < reader.AttributeCount; i++)
				//   Console.WriteLine(reader[i]);

				// Init list. 
				_orderedattributes = new ArrayList();

				// Return empty list for end elements.
				if (_isendelement) return _orderedattributes;

				// Add all regular attributes.
				if (_navigator.HasAttributes)
				{
					var attrnav = _navigator.Clone();
					_orderedattributes = new ArrayList();
					if (attrnav.NodeType == XPathNodeType.Attribute ||
						attrnav.NodeType == XPathNodeType.Namespace)
						attrnav.MoveToParent();

					if (attrnav.MoveToFirstAttribute())
					{
						_orderedattributes.Add(new string[] { attrnav.LocalName, attrnav.NamespaceURI });
						while (attrnav.MoveToNextAttribute())
						{
							_orderedattributes.Add(new string[] { attrnav.LocalName, attrnav.NamespaceURI });
						}
					}
				}

				// Add all namespace attributes declared at the current node.
				var nsnav = _navigator.Clone();
				if (nsnav.MoveToFirstNamespace(XPathNamespaceScope.Local))
				{
					// Don't add the xmlns declaration itself.
					if (nsnav.Value != XmlNamespaces.Xml)
						_orderedattributes.Add(new string[] { nsnav.LocalName, XmlNamespaces.XmlNs });
					// Add other non-built-in namespaces.
					while (nsnav.MoveToNextNamespace(XPathNamespaceScope.Local))
					{
						if (nsnav.Value != XmlNamespaces.Xml)
							_orderedattributes.Add(new string[] { nsnav.LocalName, XmlNamespaces.XmlNs });
					}
				}

				return _orderedattributes;
			}
		} ArrayList _orderedattributes;

		#endregion Private Members

		#region ReadFragmentXml

		/// <summary>
		/// Reads the current node and all its siblings, and 
		/// returns the XML for them as a fragment.
		/// </summary>
		/// <returns></returns>
		public string ReadFragmentXml()
		{
			var current = _navigator.Clone();

			var sw = new StringWriter(System.Globalization.CultureInfo.CurrentCulture);
			var tw = new XmlTextWriter(sw);

			do
			{
				tw.WriteNode(this, false);
			} while (current.MoveToNext() && !EOF);

			sw.Flush();
			return sw.ToString();
		}

		#endregion ReadFragmentXml

		#region XmlTextReader Overrides

		#region Properties

		/// <summary>See <see cref="XmlReader.AttributeCount"/></summary>
		public override int AttributeCount => OrderedAttributes.Count;

	    /// <summary>See <see cref="XmlReader.BaseURI"/></summary>
		public override string BaseURI => _navigator.BaseURI;

	    /// <summary>See <see cref="XmlReader.Depth"/></summary>
		public override int Depth => _depth;

	    int _depth = 0;

		/// <summary>See <see cref="XmlReader.EOF"/></summary>
		public override bool EOF => _eof;

	    bool _eof = false;

		/// <summary>See <see cref="XmlReader.HasValue"/></summary>
		public override bool HasValue => (
		    _navigator.NodeType == XPathNodeType.Namespace ||
		    _navigator.NodeType == XPathNodeType.Attribute ||
		    _navigator.NodeType == XPathNodeType.Comment ||
		    _navigator.NodeType == XPathNodeType.ProcessingInstruction ||
		    _navigator.NodeType == XPathNodeType.SignificantWhitespace ||
		    _navigator.NodeType == XPathNodeType.Text ||
		    _navigator.NodeType == XPathNodeType.Whitespace);

	    /// <summary>See <see cref="XmlReader.HasAttributes"/></summary>
		public override bool HasAttributes => _navigator.HasAttributes;

	    /// <summary>See <see cref="XmlReader.IsDefault"/></summary>
		public override bool IsDefault => false;

	    /// <summary>See <see cref="XmlReader.IsDefault"/></summary>
		public override bool IsEmptyElement => !_navigator.HasChildren;

	    /// <summary>See <see cref="XmlReader.this"/></summary>
		public override string this[string name, string namespaceURI]
		{
			get
			{
				var attribute = String.Empty;
				// Attribute requested may be a namespaces prefix mapping.
				if (namespaceURI == XmlNamespaces.XmlNs)
				{
					attribute = _navigator.GetNamespace(name);
				}
				else
				{
					attribute = _navigator.GetAttribute(name, namespaceURI);
				}
				// The navigator will return String.Empty instead of null for non-existent attributes.
				if (attribute.Length != 0)
					return attribute;
				return null;
			}
		}

		/// <summary>See <see cref="XmlReader.this"/></summary>
		public override string this[string name] => this[name, String.Empty];

	    /// <summary>See <see cref="XmlReader.this"/></summary>
		public override string this[int i]
		{
			get
			{
				// List contains the following values: string[] { name, namespaceURI }
				var values = (string[])OrderedAttributes[i];
				return this[values[0], values[1]];
			}
		}

		/// <summary>See <see cref="XmlReader.LocalName"/></summary>
		public override string LocalName
		{
			get
			{
				// Special handling for namespaces.
				if (_navigator.NodeType == XPathNodeType.Namespace &&
					_navigator.LocalName.Length == 0)
					return XmlNamespaces.XmlNsPrefix;

				// If reading attribute value, name is empty too.
				if (_attributevalueread) return String.Empty;

				return _navigator.LocalName;
			}
		}

		/// <summary>See <see cref="XmlReader.Name"/></summary>
		public override string Name
		{
			get
			{
				// Special handling for namespaces.
				if (_navigator.NodeType == XPathNodeType.Namespace)
				{
					// Actually retrieves the prefix for the namespace.
					var name = _navigator.Name;
					if (name.Length == 0)
						name = XmlNamespaces.XmlNsPrefix;
					else
						name = XmlNamespaces.XmlNsPrefix + ":" + name;
					return name;
				}

				// If reading attribute value, name is empty too.
				if (_attributevalueread) return String.Empty;

				return _navigator.Name;
			}
		}

		/// <summary>See <see cref="XmlReader.NamespaceURI"/></summary>
		public override string NamespaceURI => _navigator.NodeType == XPathNodeType.Namespace ?
		    XmlNamespaces.XmlNs : _navigator.NamespaceURI;

	    /// <summary>See <see cref="XmlReader.NameTable"/></summary>
		public override XmlNameTable NameTable => _navigator.NameTable;

	    /// <summary>See <see cref="XmlReader.NodeType"/></summary>
		public override XmlNodeType NodeType
		{
			get
			{
				// Special states.
				if (_state != ReadState.Interactive) return XmlNodeType.None;
				if (_isendelement) return XmlNodeType.EndElement;
				if (_attributevalueread) return XmlNodeType.Text;

				switch (_navigator.NodeType)
				{
					case XPathNodeType.Attribute:
					// Namespaces are exposed by the XmlReader as attributes too.
					case XPathNodeType.Namespace:
						return XmlNodeType.Attribute;
					case XPathNodeType.Comment:
						return XmlNodeType.Comment;
					case XPathNodeType.Element:
						return XmlNodeType.Element;
					case XPathNodeType.ProcessingInstruction:
						return XmlNodeType.ProcessingInstruction;
					case XPathNodeType.Root:
						return XmlNodeType.Document;
					case XPathNodeType.SignificantWhitespace:
						return XmlNodeType.SignificantWhitespace;
					case XPathNodeType.Text:
						return XmlNodeType.Text;
					case XPathNodeType.Whitespace:
						return XmlNodeType.Whitespace;
					default:
						return XmlNodeType.None;
				}
			}
		}

		/// <summary>See <see cref="XmlReader.Prefix"/></summary>
		public override string Prefix => _navigator.Prefix;

	    /// <summary>See <see cref="XmlReader.QuoteChar"/></summary>
		public override char QuoteChar => '"';

	    /// <summary>See <see cref="XmlReader.ReadState"/></summary>
		public override ReadState ReadState => _state;

	    ReadState _state = ReadState.Initial;

		/// <summary>See <see cref="XmlReader.Value"/></summary>
		public override string Value => HasValue ? _navigator.Value : String.Empty;

	    /// <summary>See <see cref="XmlReader.XmlLang"/></summary>
		public override string XmlLang => _navigator.XmlLang;

	    /// <summary>See <see cref="XmlReader.XmlSpace"/></summary>
		public override XmlSpace XmlSpace => XmlSpace.Default;

	    #endregion Properties

		#region Methods

		/// <summary>See <see cref="XmlReader.Close"/></summary>
		public override void Close()
		{
			_state = ReadState.Closed;
			_eof = true;
		}

		/// <summary>See <see cref="XmlReader.GetAttribute"/></summary>
		public override string GetAttribute(string name, string namespaceURI)
		{
			return this[name, namespaceURI];
		}

		/// <summary>See <see cref="XmlReader.GetAttribute"/></summary>
		public override string GetAttribute(string name)
		{
			return this[name];
		}

		/// <summary>See <see cref="XmlReader.GetAttribute"/></summary>
		public override string GetAttribute(int i)
		{
			return this[i];
		}

		/// <summary>See <see cref="XmlReader.LookupNamespace"/></summary>
		public override string LookupNamespace(string prefix)
		{
			var ns = _navigator.GetNamespace(prefix);
			return ns.Length == 0 ? null : ns;
		}

		/// <summary>See <see cref="XmlReader.MoveToAttribute"/></summary>
		public override bool MoveToAttribute(string name, string ns)
		{
			if (_navigator.NodeType == XPathNodeType.Attribute ||
				_navigator.NodeType == XPathNodeType.Namespace)
			{
				_navigator.MoveToParent();
				_depth--;
			}
			var moved = _navigator.MoveToAttribute(name, ns);
			// Attributes are 1 step deeper than the parent element. According to XmlTextReader
			if (moved) _depth++;
			return moved;
		}

		/// <summary>See <see cref="XmlReader.MoveToAttribute"/></summary>
		public override bool MoveToAttribute(string name)
		{
			return MoveToAttribute(name, String.Empty);
		}

		/// <summary>See <see cref="XmlReader.MoveToAttribute"/></summary>
		public override void MoveToAttribute(int i)
		{
			var values = (string[])OrderedAttributes[i];
			MoveToAttribute(values[0], values[1]);
		}

		/// <summary>See <see cref="XmlReader.MoveToElement"/></summary>
		public override bool MoveToElement()
		{
			if (_navigator.NodeType == XPathNodeType.Attribute ||
				_navigator.NodeType == XPathNodeType.Namespace)
			{
				_navigator.MoveToParent();
				// Return to element depth.
				_depth--;
				// Reset flag for ReadAttributeValue.
				_attributevalueread = false;
				return true;
			}

			return false;
		}

		/// <summary>See <see cref="XmlReader.MoveToFirstAttribute"/></summary>
		public override bool MoveToFirstAttribute()
		{
			if (_isendelement) return false;
			var moved = _navigator.MoveToFirstAttribute();
			if (!moved) moved = _navigator.MoveToFirstNamespace(XPathNamespaceScope.Local);

			// Skip the xmlns declaration itself.
			if (moved && _navigator.Value == XmlNamespaces.Xml)
				return MoveToNextAttribute();

			if (moved)
			{
				_depth++;
				// Reset flag for ReadAttributeValue method.
				_attributevalueread = false;
			}

			return moved;
		}

		/// <summary>See <see cref="XmlReader.MoveToNextAttribute"/></summary>
		public override bool MoveToNextAttribute()
		{
			var moved = false;
			if (_navigator.NodeType == XPathNodeType.Attribute)
			{
				moved = _navigator.MoveToNextAttribute();
				if (!moved)
				{
					// Keep position because moving to namespaces may not succeed, and 
					// in this case we need to stand where we were, at the last attribute.
					var lastattr = _navigator.Clone();
					_navigator.MoveToParent();
					// We ended regular attributes. Start with namespaces if we can.
					moved = _navigator.MoveToFirstNamespace(XPathNamespaceScope.Local);
					// If there were no namespaces, return to the last attribute we were at.
					if (!moved) _navigator.MoveTo(lastattr);
				}
			}
			else if (_navigator.NodeType == XPathNodeType.Namespace)
			{
				moved = _navigator.MoveToNextNamespace(XPathNamespaceScope.Local);
			}
			else if (_navigator.NodeType == XPathNodeType.Element)
			{
				return MoveToFirstAttribute();
			}

			// Skip the xmlns declaration itself.
			if (moved && _navigator.Value == XmlNamespaces.Xml)
				return MoveToNextAttribute();

			if (moved)
			{
				// Reset flag for ReadAttributeValue method.
				_attributevalueread = false;
			}

			return moved;
		}

		/// <summary>See <see cref="XmlReader.Read"/></summary>
		public override bool Read()
		{
			// Return fast if state is not appropriate.
			if (_state == ReadState.Closed || _state == ReadState.EndOfFile)
				return false;

			if (_state == ReadState.Initial)
			{
				_state = ReadState.Interactive;
				if (_navigator.NodeType == XPathNodeType.Root)
				{
					// Sync to the real first node.
					_original.MoveToFirstChild();
					return _navigator.MoveToFirstChild();
				}
				return true;
			}

			// Reset temp state.
			_orderedattributes = null;
			// Reset flag for ReadAttributeValue method.
			_attributevalueread = false;

			// Reposition if we moved to attributes.
			if (_navigator.NodeType == XPathNodeType.Attribute ||
				_navigator.NodeType == XPathNodeType.Namespace)
			{
				_navigator.MoveToParent();
				// Return to element depth.
				_depth--;
			}

			if (_isendelement)
			{
				if (_fragment)
				{
					// If XML fragment is allowed, we'll let movements 
					// outside the original location, but only to sibling nodes.
					// We reposition the original ("root") node to the next sibling.
					if (_navigator.IsSamePosition(_original) && _original.MoveToNext())
					{
						_isendelement = false;
						_navigator.MoveToNext();
						return true;
					}
				}
				else if (_navigator.IsSamePosition(_original))
				{
					// If we're at the same position we started, it's eof.
					_eof = true;
					_state = ReadState.EndOfFile;
					return false;
				}

				// If we're at the faked end element, move to next sibling.
				if (_navigator.MoveToNext())
				{
					_isendelement = false;
					return true;
				}
				else
				{
					// Otherwise, move to the parent and set as the 
					// end element of it (we already read all children therefore).
					_navigator.MoveToParent();
					_depth--;
					// _isendelement remains true.
					return true;
				}
			}
			else
			{
				// We're not at a faked end element. Check if there are children to move to.
				if (_navigator.HasChildren)
				{
					_depth++;
					// Move to child node.
					return _navigator.MoveToFirstChild();
				}

				// Otherwise, try to move to sibling.
				if (_navigator.MoveToNext())
				{
					return true;
				}
				else
				{
					// Otherwise, move to the parent and set as the 
					// end element of it (we already read all children therefore).
					_navigator.MoveToParent();
					_depth--;
					_isendelement = true;
					return true;
				}
			}
		}

		/// <summary>See <see cref="XmlReader.ReadAttributeValue"/></summary>
		public override bool ReadAttributeValue()
		{
			// If this method hasn't been called yet for the attribute.
			if (!_attributevalueread &&
				(_navigator.NodeType == XPathNodeType.Attribute ||
				_navigator.NodeType == XPathNodeType.Namespace))
			{
				_attributevalueread = true;
				return true;
			}

			return false;

		} bool _attributevalueread = false;

		/// <summary>See <see cref="XmlReader.ReadInnerXml"/></summary>
		public override string ReadInnerXml()
		{
			if (Read())
				return ReadFragmentXml();
			else
				return String.Empty;
		}

		/// <summary>See <see cref="XmlReader.ReadOuterXml"/></summary>
		public override string ReadOuterXml()
		{
			if (_state != ReadState.Interactive)
				return String.Empty;
			else
			{
				var sw = new StringWriter(System.Globalization.CultureInfo.CurrentCulture);
				var tw = new XmlTextWriter(sw);
				tw.WriteNode(this, false);

				sw.Flush();
				return sw.ToString();
			}
		}

		/// <summary>See <see cref="XmlReader.Read"/></summary>
		public override void ResolveEntity()
		{
			// Not supported.
		}

		#endregion Methods

		#endregion XmlTextReader Overrides

		#region IXmlSerializable Members

		/// <summary>
		/// See <see cref="IXmlSerializable.WriteXml"/>.
		/// </summary>
		void IXmlSerializable.WriteXml(XmlWriter writer)
		{
			writer.WriteNode(this, false);
		}

		/// <summary>
		/// See <see cref="IXmlSerializable.GetSchema"/>.
		/// </summary>
		System.Xml.Schema.XmlSchema IXmlSerializable.GetSchema()
		{
			return null;
		}

		/// <summary>
		/// See <see cref="IXmlSerializable.ReadXml"/>.
		/// </summary>
		void IXmlSerializable.ReadXml(XmlReader reader)
		{
			var doc = new XPathDocument(reader);
			_navigator = doc.CreateNavigator();
		}

		#endregion
	}
}
