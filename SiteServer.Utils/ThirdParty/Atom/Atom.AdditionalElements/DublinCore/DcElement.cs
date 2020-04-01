/* 
  	* DcElement.cs
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
using Atom.Core;
using Atom.AdditionalElements;
using Atom.Utils;

namespace Atom.AdditionalElements.DublinCore
{
	/// <summary>
	/// The class representing any of the Dublin Core elements.
	/// <seealso cref="ScopedElement"/>
	/// </summary>
	public class DcElement : ScopedElement
	{
		#region Constructors
		/// <summary>
		/// Represents an <see cref="DcElement"/> instance.
		/// Initialize the <see cref="Atom.Core.AtomElement.LocalName"/> to <see cref="String.Empty"/>
		/// and <see cref="ScopedElement.Content"/> to <see cref="String.Empty"/>
		/// </summary>
		public DcElement() : this(String.Empty, String.Empty)
		{
		}

		/// <summary>
		/// Represents an <see cref="DcElement"/> instance initialized with the given name.
		/// </summary>
		/// <param name="name">The <see cref="Atom.Core.AtomElement.LocalName"/> of the <see cref="DcElement"/>.</param>
		public DcElement(string name) : this(name, String.Empty)
		{
		}

		/// <summary>
		/// Represents an <see cref="DcElement"/> instance initialized with the given name and content.
		/// </summary>
		/// <param name="name">The <see cref="Atom.Core.AtomElement.LocalName"/> of the <see cref="DcElement"/>.</param>
		/// <param name="content">The <see cref="ScopedElement.Content"/> of the <see cref="DcElement"/>.</param>
		public DcElement(string name, string content)
		{
			this.LocalName = name;
			base.Content = content;
		}
		#endregion Constructors

		#region Properties
		/// <summary>
		/// Gets the Dublin Core namespace prefix
		/// </summary>
		public override string NamespacePrefix
		{
			get { return DefaultValues.DCNSPrefix; }
		}

		/// <summary>
		/// Gets the Dublin Core namespace prefix
		/// </summary>
		public override Uri NamespaceUri
		{
			get { return DefaultValues.DCNSUri; }
		}

		#endregion

		#region XPath parsing stuff
		internal static DcElement Parse(XPathNavigator navigator)
		{
			DcElement element = new DcElement();

			XPathNavigator nav = navigator.Clone();
			
			// select the element itself
			XPathNodeIterator iter = nav.SelectDescendants(XPathNodeType.Element, true);
			while(iter.MoveNext())
			{
				string name = iter.Current.Name;
				int idx = name.IndexOf(":");
				if(idx != -1)
					name = name.Split(new char[] {':'}, 2)[1];

				try
				{
					element.XmlLang = Utils.Utils.ParseLanguage(iter.Current.XmlLang);
				}
				catch {}
				element.LocalName = name;
				element.Content = iter.Current.Value;
			}

			return element;
		}
		#endregion
	}
}
