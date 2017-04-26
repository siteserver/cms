/* 
  	* AtomLinkCollection.cs
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
	/// A strongly typed collection of <see cref="AtomLink"/> objects.
	/// <seealso cref="AtomLink"/>
	/// </summary>
	[Serializable]
	public class AtomLinkCollection : CollectionBase
	{
		#region Collection methods

		/// <summary>
		/// Gets or sets the element at the specified index.
		/// </summary>
		/// <param name="index">The zero-based index of the element to get or set.</param>
		public AtomLink this[int index]
		{
			get { return ((AtomLink)this.List[index]); }
			set
			{
				CheckInsertion(value as AtomLink);
				this.List[index] = value as AtomLink;
			}
		}

		/// <summary>
		/// Adds an object to the end of the <see cref="AtomLinkCollection"/>.
		/// </summary>
		/// <param name="link">The <see cref="AtomLink"/> to be added to the end of the <see cref="AtomLinkCollection"/>.</param>
		/// <returns>The <see cref="AtomLinkCollection"/> index at which the value has been added.</returns>
		public int Add(AtomLink link)
		{
			CheckInsertion(link);
			return this.List.Add(link);
		}

		/// <summary>
		/// Determines whether the <see cref="AtomLinkCollection"/> contains a specific element.
		/// </summary>
		/// <param name="link">The <see cref="AtomLink"/> to locate in the <see cref="AtomLinkCollection"/>.</param>
		/// <returns>true if the <see cref="AtomLinkCollection"/> contains the specified item, otherwise false.</returns>
		public bool Contains(AtomLink link)
		{
			return this.List.Contains(link);
		}

		/// <summary>
		/// Copies the entire <see cref="AtomLinkCollection"/> to a compatible one-dimensional <see cref="Array"/>,
		/// starting at the specified index of the target array.
		/// </summary>
		/// <param name="array">The one-dimensional <see cref="Array"/> that is the destination of the elements copied
		/// from <see cref="AtomLinkCollection"/>. The <see cref="Array"/> must have zero-based indexing. </param>
		/// <param name="index">The zero-based index in <i>array</i> at which copying begins.</param>
		public void CopyTo(AtomLinkCollection[] array, int index)
		{
			this.List.CopyTo(array, index);
		}

		/// <summary>
		/// Searches for the specified <see cref="AtomLink"/> and returns the zero-based index of the first occurrence
		/// within the entire <see cref="AtomLinkCollection"/>.
		/// </summary>
		/// <param name="link">The <see cref="AtomLink"/> to locate in the <see cref="AtomLinkCollection"/>.</param>
		/// <returns>The zero-based index of the first occurrence of <i>link</i> within the entire <see cref="AtomLinkCollection"/>,
		/// if found; otherwise, -1.</returns>
		public int IndexOf(AtomLink link)
		{
			return List.IndexOf(link);
		}

		/// <summary>Inserts a <see cref="AtomLink"/> into this collection at the specified index.</summary>
		/// <param name="index">The zero-based index of the collection at which <i>link</i> should be inserted.</param>
		/// <param name="link">The <see cref="AtomLink"/> to insert into this collection.</param>
		public void Insert(int index, AtomLink link)
		{
			CheckInsertion(link);
			List.Insert(index, link);
		}

		/// <summary>Removes the first occurrence of a specific <see cref="AtomLink"/> from the <see cref="AtomLinkCollection"/>.</summary>
		/// <param name="link">The <see cref="AtomLink"/> to remove from the <see cref="AtomLinkCollection"/>.</param>
		public void Remove(AtomLink link)
		{
			List.Remove(link);
		}

		#endregion Collection methods

		#region Private methods

		/// <summary>
		/// Performs some checks inside the collection. Used for compliancy to the standard.
		/// </summary>
		/// <param name="link">The <see cref="AtomLink"/> to check.</param>
		private void CheckInsertion(AtomLink link)
		{
			if(this.List.Count != 0)
				if(link.Rel == Relationship.Alternate)
					foreach(AtomLink item in this)
						if(item.Rel == Relationship.Alternate)
							if(item.Type == link.Type)
								throw new DuplicateLinkException(
									"There can't be two links with the same relationship and type.");
		}

		#endregion Private methods
	}
}
