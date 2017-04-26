/* 
  	* AtomGenerator.cs
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
using Atom.Utils;

namespace Atom.Core
{
	/// <summary>
	/// The generator program of an atom feed.
	/// <seealso cref="AtomFeed"/>
	/// </summary>
	[Serializable]
	internal class AtomGenerator : AtomElement
	{
		private string _content = String.Empty;
		private Uri _url = DefaultValues.Uri;
		private string _version = String.Empty;

		#region Constructor

		internal AtomGenerator()
		{
			this._content = DefaultValues.GeneratorMessage;
			this._url = DefaultValues.GeneratorUri;
			this._version = DefaultValues.GeneratorVersion;
		}

		#endregion Constructor

		#region ToString method

		/// <summary>
		/// Converts the <see cref="AtomGenerator"/> in a series of xml nodes.
		/// </summary>
		/// <returns>The string representation of <see cref="AtomGenerator"/> class.</returns>
		public override string ToString()
		{
			this.Buffer.Append("<generator");

			this.WriteAttribute("xml:lang", Utils.Utils.ParseLanguage(this.XmlLang), false, null);
			this.WriteAttribute("url", this._url, false, null);
			this.WriteAttribute("version", this._version, false, null);

			this.Buffer.Append(">");
			this.Buffer.Append(this._content);

			this.Buffer.Append("</generator>");
			this.Buffer.Append(Environment.NewLine);

			string output = this.Buffer.ToString();
			this.Buffer.Length = 0;

			return output;
		}

		#endregion
	}
}
