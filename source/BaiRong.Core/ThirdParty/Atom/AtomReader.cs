/* 
  	* AtomReader.cs
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
using System.Xml;
using System.Xml.XPath;
using Atom.AdditionalElements.DublinCore;
using Atom.Core;
using Atom.Utils;

namespace Atom
{
	internal class AtomReader
	{
		private XPathDocument _document = null;
		private XPathNavigator _navigator = null;

		#region Properties
		internal XPathNavigator Navigator
		{
			get { return _navigator; }
		}
		#endregion

		#region Constructors
		internal AtomReader(Stream stream)
		{
			this._document = new XPathDocument(stream);
			Init();
		}

		internal AtomReader(string uri)
		{
			this._document = new XPathDocument(uri);
			Init();
		}

		internal AtomReader(TextReader textReader)
		{
			this._document = new XPathDocument(textReader);
			Init();
		}

		internal AtomReader(XmlReader xmlReader)
		{
			this._document = new XPathDocument(xmlReader);
			Init();
		}
		#endregion

		#region Private methods
		private void Init()
		{
			this._navigator = this._document.CreateNavigator();
			XmlNamespaceManager nsm = new XmlNamespaceManager(this._navigator.NameTable);
			nsm.AddNamespace(DefaultValues.AtomNSPrefix, DefaultValues.AtomNSUri.ToString());
			nsm.AddNamespace(DefaultValues.DCNSPrefix, DefaultValues.DCNSUri.ToString());

			XPathExpression expr = this._navigator.Compile("/atom:feed");
			expr.SetContext(nsm);
            XPathNodeIterator iterator = this._navigator.Select(expr);
            if(iterator.CurrentPosition == 0)
				iterator.MoveNext();
			this._navigator = iterator.Current;
		}
		#endregion
	}
}
