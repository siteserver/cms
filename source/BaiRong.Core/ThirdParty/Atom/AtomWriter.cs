/* 
  	* AtomWriter.cs
	* [ part of Atom.NET library: http://atomnet.sourceforge.net ]
	* Author: Lawrence Oluyede
	* License: BSD-License (see below)
    
	Copyright (c) 2003, 2004 Lawrence Oluyede
    All rights reserved.
	
	Contains portions of
	RSS.NET (http://rss-net.sf.net/)
	Copyright © 2002, 2003 George Tsiokos. All Rights Reserved.

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
using System.Xml;
using Atom.Core;
using Atom.Utils;

namespace Atom
{
	internal class AtomWriter
	{
		private XmlTextWriter _writer = null;

		#region Constructors
		internal AtomWriter(TextWriter w)
		{
			this._writer = new XmlTextWriter(w);
			Init();
		}

		internal AtomWriter(Stream w, Encoding encoding)
		{
			this._writer = new XmlTextWriter(w, encoding);
			Init();
		}

		internal AtomWriter(string filename, Encoding encoding)
		{
			this._writer = new XmlTextWriter(filename, encoding);
			Init();
		}

		internal AtomWriter(XmlWriter writer)
		{
			this._writer = (XmlTextWriter)writer;
			Init();
		}
		#endregion Constructors

		#region Private methods

		private void WriteHeader()
		{
            this._writer.WriteStartDocument();
			this._writer.WriteComment(DefaultValues.GeneratorMessage);
			this._writer.WriteRaw(Environment.NewLine);
		}

		/// <summary>
		/// Writes the Atom feed body.
		/// </summary>
		private void WriteFeed(AtomFeed feed)
		{
			if(this._writer == null)
			 throw new InvalidOperationException("AtomWriter has been closed, and can not be written to.");


			this.WriteHeader();

			if(feed == null)
				throw new RequiredElementNotFoundException("AtomFeed cannot be null.");

			this._writer.WriteRaw(feed.ToString());

			// finalize the writer
			this._writer.Flush();
			this._writer.Close();
			this._writer = null;
		}

		/// <summary>
		/// Writes the Atom entry body.
		/// </summary>
		private void WriteEntry(AtomEntry entry)
		{
			if(this._writer == null)
				throw new InvalidOperationException("AtomWriter has been closed, and can not be written to.");

			if(entry == null)
				throw new RequiredElementNotFoundException("AtomEntry cannot be null.");

			this._writer.WriteRaw(entry.ToString());

			this._writer.Flush();
			this._writer.Close();
			this._writer = null;
		}

		private void Init()
		{
			this._writer.Formatting = Formatting.Indented;
			this._writer.Indentation = 2;
		}

		#endregion Private methods

		#region Internal methods

		internal void Write(AtomFeed feed)
		{
			this.WriteFeed(feed);
		}

		internal void Write(AtomEntry entry)
		{
			this.WriteEntry(entry);
		}

		#endregion Internal methods
	}
}