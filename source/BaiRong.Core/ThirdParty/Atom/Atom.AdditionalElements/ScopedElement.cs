/* 
  	* ScopedElement.cs
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
using Atom.Core;
using Atom.Utils;

namespace Atom.AdditionalElements
{
	/// <summary>
	/// The base class of all non Atom core elements.
	/// <seealso cref="AtomElement"/>
	/// </summary>
	[Serializable]
	public abstract class ScopedElement : AtomElement
	{
		private string _content = String.Empty;

		/// <summary>
		/// Initialize a new instance of the <see cref="ScopedElement"/> class.
		/// </summary>
		protected ScopedElement() {}

		#region Public properties

		/// <summary>
		/// The content of the element.
		/// </summary>
		public virtual string Content
		{
			get { return _content; }
			set { _content = value; }
		}

		#endregion

		#region ToString helper methods

		/// <summary>
		/// Writes the start tag of the element.
		/// </summary>
		protected internal override void WriteStartElement()
		{
			if(this.LocalName.Length == 0)
				throw new RequiredElementNotFoundException("The name of the element is required");

			this.Buffer.AppendFormat("<{0} xmlns:{1}=\"{2}\">", this.FullName,
				this.NamespacePrefix, this.NamespaceUri);
		}

		/// <summary>
		/// Writes the end tag of the element.
		/// </summary>
		protected internal override void WriteEndElement()
		{
			this.Buffer.AppendFormat("</{0}>", this.FullName);
		}

		#endregion

		#region ToString

		/// <summary>
		/// Converts the <see cref="ScopedElement"/> in a series of xml nodes.
		/// </summary>
		/// <returns>The string representation of <see cref="ScopedElement"/> class.</returns>
		public override string ToString()
		{
			this.WriteStartElement();

			if(this.Content.Length == 0)
				throw new RequiredElementNotFoundException("The content cannot be empty.");

			this.Buffer.Append(this.Content);

			this.WriteEndElement();
			this.Buffer.Append(Environment.NewLine);

			string output = this.Buffer.ToString();
			this.Buffer.Length = 0;

			return output;
		}

		#endregion
	}
}
