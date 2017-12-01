/* 
  	* ScopedElementCollection.cs
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
using System.Collections.Specialized;
using System.Text;
using Atom.Utils;
using BaiRong.Core;

namespace Atom.AdditionalElements
{
	/// <summary>
	/// A strongly typed collection of <see cref="ScopedElement"/> objects.
	/// <seealso cref="ScopedElement"/>
	/// </summary>
	[Serializable]
	public class ScopedElementCollection : CollectionBase
	{
		#region Collection methods

		/// <summary>
		/// Gets or sets the element at the specified index.
		/// </summary>
		/// <param name="index">The zero-based index of the element to get or set.</param>
		public ScopedElement this[int index]
		{
			get { return ((ScopedElement)this.List[index]); }
			set { this.List[index] = value as ScopedElement; }
		}

		/// <summary>
		/// Adds an object to the end of the <see cref="ScopedElementCollection"/>.
		/// </summary>
		/// <param name="element">The <see cref="ScopedElement"/> to be added to the end of
		/// the <see cref="ScopedElementCollection"/>.</param>
		/// <returns>The <see cref="ScopedElementCollection"/> index at which the value has been added.</returns>
		public int Add(ScopedElement element)
		{
			return this.List.Add(element);
		}

		/// <summary>
		/// Determines whether the <see cref="ScopedElementCollection"/> contains a specific element.
		/// </summary>
		/// <param name="element">The <see cref="ScopedElement"/> to locate in the <see cref="ScopedElementCollection"/>.</param>
		/// <returns>true if the <see cref="ScopedElementCollection"/> contains the specified item, otherwise false.</returns>
		public bool Contains(ScopedElement element)
		{
			return this.List.Contains(element);
		}

		/// <summary>
		/// Copies the entire <see cref="ScopedElementCollection"/> to a compatible one-dimensional <see cref="Array"/>,
		/// starting at the specified index of the target array.
		/// </summary>
		/// <param name="array">The one-dimensional <see cref="Array"/> that is the destination of the elements copied
		/// from <see cref="ScopedElementCollection"/>. The <see cref="Array"/> must have zero-based indexing. </param>
		/// <param name="index">The zero-based index in <i>array</i> at which copying begins.</param>
		public void CopyTo(ScopedElementCollection[] array, int index)
		{
			this.List.CopyTo(array, index);
		}

		/// <summary>
		/// Searches for the specified <see cref="ScopedElement"/> and returns the zero-based index of the first occurrence
		/// within the entire <see cref="ScopedElementCollection"/>.
		/// </summary>
		/// <param name="element">The <see cref="ScopedElement"/> to locate in the <see cref="ScopedElementCollection"/>.</param>
		/// <returns>The zero-based index of the first occurrence of <i>element</i> within the entire <see cref="ScopedElementCollection"/>,
		/// if found; otherwise, -1.</returns>
		public int IndexOf(ScopedElement element)
		{
			return List.IndexOf(element);
		}

		/// <summary>Inserts a <see cref="ScopedElement"/> into this collection at the specified index.</summary>
		/// <param name="index">The zero-based index of the collection at which <i>element</i> should be inserted.</param>
		/// <param name="element">The <see cref="ScopedElement"/> to insert into this collection.</param>
		public void Insert(int index, ScopedElement element)
		{
			List.Insert(index, element);
		}

		/// <summary>Removes the first occurrence of a specific <see cref="ScopedElement"/>
		/// from the <see cref="ScopedElementCollection"/>.</summary>
		/// <param name="element">The <see cref="ScopedElement"/> to remove from the <see cref="ScopedElementCollection"/>.</param>
		public void Remove(ScopedElement element)
		{
			List.Remove(element);
		}


		public ScopedElement FindScopedElementByLocalName(string localName)
		{
			foreach (ScopedElement element in List)
			{
				if (StringUtils.EqualsIgnoreCase(element.LocalName, localName))
				{
					return element;
				}
			}
			return null;
		}


		public NameValueCollection GetNameValueCollection(string localNamePrefix)
		{
			NameValueCollection collection = new NameValueCollection();
			foreach (ScopedElement element in List)
			{
				string name = element.LocalName;
				if (name.StartsWith(localNamePrefix))
				{
					name = name.Replace(localNamePrefix, "");
				}
				collection.Add(name, element.Content);
			}
			return collection;
		}


		#endregion Collection methods
	}
}
