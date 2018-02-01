/* 
  	* AtomPersonConstructCollection.cs
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
	/// A strongly typed collection of <see cref="AtomPersonConstruct"/> objects.
	/// <seealso cref="AtomPersonConstruct"/>
	/// </summary>
	[Serializable]
	public class AtomPersonConstructCollection : CollectionBase
	{
		#region Collection methods

		/// <summary>
		/// Gets or sets the element at the specified index.
		/// </summary>
		/// <param name="index">The zero-based index of the element to get or set.</param>
		public AtomPersonConstruct this[int index]
		{
			get { return ((AtomPersonConstruct)this.List[index]); }
			set { this.List[index] = value as AtomPersonConstruct; }
		}

		/// <summary>
		/// Adds an object to the end of the <see cref="AtomPersonConstructCollection"/>.
		/// </summary>
		/// <param name="contributor">The <see cref="AtomPersonConstruct"/> to be added to the end of
		/// the <see cref="AtomPersonConstructCollection"/>.</param>
		/// <returns>The <see cref="AtomPersonConstructCollection"/> index at which the value has been added.</returns>
		public int Add(AtomPersonConstruct contributor)
		{
			return this.List.Add(contributor);
		}

		/// <summary>
		/// Determines whether the <see cref="AtomPersonConstructCollection"/> contains a specific element.
		/// </summary>
		/// <param name="contributor">The <see cref="AtomPersonConstruct"/> to locate in the <see cref="AtomPersonConstructCollection"/>.</param>
		/// <returns>true if the <see cref="AtomPersonConstructCollection"/> contains the specified item, otherwise false.</returns>
		public bool Contains(AtomPersonConstruct contributor)
		{
			return this.List.Contains(contributor);
		}

		/// <summary>
		/// Copies the entire <see cref="AtomPersonConstructCollection"/> to a compatible one-dimensional <see cref="Array"/>,
		/// starting at the specified index of the target array.
		/// </summary>
		/// <param name="array">The one-dimensional <see cref="Array"/> that is the destination of the elements copied
		/// from <see cref="AtomPersonConstructCollection"/>. The <see cref="Array"/> must have zero-based indexing. </param>
		/// <param name="index">The zero-based index in <i>array</i> at which copying begins.</param>
		public void CopyTo(AtomPersonConstructCollection[] array, int index)
		{
			this.List.CopyTo(array, index);
		}

		/// <summary>
		/// Searches for the specified <see cref="AtomPersonConstruct"/> and returns the zero-based index of the first occurrence
		/// within the entire <see cref="AtomPersonConstructCollection"/>.
		/// </summary>
		/// <param name="contributor">The <see cref="AtomPersonConstruct"/> to locate in the <see cref="AtomPersonConstructCollection"/>.</param>
		/// <returns>The zero-based index of the first occurrence of <i>contributor</i> within the entire <see cref="AtomPersonConstructCollection"/>,
		/// if found; otherwise, -1.</returns>
		public int IndexOf(AtomPersonConstruct contributor)
		{
			return List.IndexOf(contributor);
		}

		/// <summary>Inserts a <see cref="AtomPersonConstruct"/> into this collection at the specified index.</summary>
		/// <param name="index">The zero-based index of the collection at which <i>contributor</i> should be inserted.</param>
		/// <param name="contributor">The <see cref="AtomPersonConstruct"/> to insert into this collection.</param>
		public void Insert(int index, AtomPersonConstruct contributor)
		{
			List.Insert(index, contributor);
		}

		/// <summary>Removes the first occurrence of a specific <see cref="AtomPersonConstruct"/>
		/// from the <see cref="AtomPersonConstructCollection"/>.</summary>
		/// <param name="contributor">The <see cref="AtomPersonConstruct"/> to remove from the <see cref="AtomPersonConstructCollection"/>.</param>
		public void Remove(AtomPersonConstruct contributor)
		{
			List.Remove(contributor);
		}

		#endregion Collection methods
	}
}
