/* 
  	* AtomContentCollection.cs
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
using System.Collections;
using Atom.Utils;

namespace Atom.Core.Collections
{
	/// <summary>
	/// A strongly typed collection of <see cref="AtomContent"/> objects.
	/// <seealso cref="AtomContent"/>
	/// </summary>
	[Serializable]
	public class AtomContentCollection : CollectionBase
	{
		#region Collection methods

		/// <summary>
		/// Gets or sets the element at the specified index.
		/// </summary>
		/// <param name="index">The zero-based index of the element to get or set.</param>
		public AtomContent this[int index]
		{
			get { return ((AtomContent)this.List[index]); }
			set
			{
				CheckInsertion(value as AtomContent);
				this.List[index] = value as AtomContent;
			}
		}

		/// <summary>
		/// Adds an object to the end of the <see cref="AtomContentCollection"/>.
		/// </summary>
		/// <param name="content">The <see cref="AtomContent"/> to be added to the end of the <see cref="AtomContentCollection"/>.</param>
		/// <returns>The <see cref="AtomContentCollection"/> index at which the value has been added.</returns>
		public int Add(AtomContent content)
		{
			CheckInsertion(content);
			return this.List.Add(content);
		}

		/// <summary>
		/// Determines whether the <see cref="AtomContentCollection"/> contains a specific element.
		/// </summary>
		/// <param name="content">The <see cref="AtomContent"/> to locate in the <see cref="AtomContentCollection"/>.</param>
		/// <returns>true if the <see cref="AtomContentCollection"/> contains the specified item, otherwise false.</returns>
		public bool Contains(AtomContent content)
		{
			return this.List.Contains(content);
		}

		/// <summary>
		/// Copies the entire <see cref="AtomContentCollection"/> to a compatible one-dimensional <see cref="Array"/>,
		/// starting at the specified index of the target array.
		/// </summary>
		/// <param name="array">The one-dimensional <see cref="Array"/> that is the destination of the elements
		/// copied from <see cref="AtomContentCollection"/>. The <see cref="Array"/> must have zero-based indexing. </param>
		/// <param name="index">The zero-based index in <i>array</i> at which copying begins.</param>
		public void CopyTo(AtomContentCollection[] array, int index)
		{
			this.List.CopyTo(array, index);
		}

		/// <summary>
		/// Searches for the specified <see cref="AtomContent"/> and returns the zero-based index of the first occurrence
		/// within the entire <see cref="AtomContentCollection"/>.
		/// </summary>
		/// <param name="content">The <see cref="AtomContent"/> to locate in the <see cref="AtomContentCollection"/>.</param>
		/// <returns>The zero-based index of the first occurrence of <i>content</i> within the entire <see cref="AtomContentCollection"/>,
		/// if found; otherwise, -1.</returns>
		public int IndexOf(AtomContent content)
		{
			return List.IndexOf(content);
		}

		/// <summary>Inserts a <see cref="AtomContent"/> into this collection at the specified index.</summary>
		/// <param name="index">The zero-based index of the collection at which <i>content</i> should be inserted.</param>
		/// <param name="content">The <see cref="AtomContent"/> to insert into this collection.</param>
		public void Insert(int index, AtomContent content)
		{
			CheckInsertion(content);
			List.Insert(index, content);
		}

		/// <summary>Removes the first occurrence of a specific <see cref="AtomContent"/> from the <see cref="AtomContentCollection"/>.</summary>
		/// <param name="content">The <see cref="AtomContent"/> to remove from the <see cref="AtomContentCollection"/>.</param>
		public void Remove(AtomContent content)
		{
			List.Remove(content);
		}

		#endregion CollectionMethods

		#region Private methods

		/// <summary>
		/// Performs some checks inside the collection. Used for compliancy to the standard.
		/// </summary>
		/// <param name="content">The <see cref="AtomContent"/> to check.</param>
		private void CheckInsertion(AtomContent content)
		{
			if(this.List.Count != 0)
				if(content.Type == MediaType.MultipartAlternative)
					foreach(AtomContent item in this)
						if(item.Type == MediaType.MultipartAlternative)
							if(item.Type == content.Type)
								throw new OnlyOneMultipartContentAllowedException(
									"There can't be more than one content with multipart/alternative media type.");
		}

		#endregion Private methods
	}
}
