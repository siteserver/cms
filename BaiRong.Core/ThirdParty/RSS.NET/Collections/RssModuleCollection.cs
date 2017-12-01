/* RssModuleCollection.cs
 * ======================
 * 
 * RSS.NET (http://rss-net.sf.net/)
 * Copyright ?2002, 2003 George Tsiokos. All Rights Reserved.
 * 
 * RSS 2.0 (http://blogs.law.harvard.edu/tech/rss)
 * RSS 2.0 is offered by the Berkman Center for Internet & Society at 
 * Harvard Law School under the terms of the Attribution/Share Alike 
 * Creative Commons license.
 * 
 * Permission is hereby granted, free of charge, to any person obtaining 
 * a copy of this software and associated documentation files (the "Software"), 
 * to deal in the Software without restriction, including without limitation 
 * the rights to use, copy, modify, merge, publish, distribute, sublicense, 
 * and/or sell copies of the Software, and to permit persons to whom the 
 * Software is furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in all
 * copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR 
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, 
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL 
 * THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER 
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, 
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN 
 * THE SOFTWARE.
*/
using System;

namespace BaiRong.Core.Rss
{
	/// <summary>A strongly typed collection of <see cref="RssModule"/> objects</summary>
	public class RssModuleCollection : System.Collections.CollectionBase
	{
		/// <summary>Gets or sets the item at a specified index.<para>In C#, this property is the indexer for the class.</para></summary>
		/// <param name="index">The index of the collection to access.</param>
		/// <value>An item at each valid index.</value>
		/// <remarks>This method is an indexer that can be used to access the collection.</remarks>
		/// <exception cref="ArgumentOutOfRangeException">index is not a valid index.</exception>
		public RssModule this[int index]
		{
			get { return ((RssModule)(List[index])); }
			set { List[index] = value; }
		}

		/// <summary>Adds a specified item to this collection.</summary>
		/// <param name="rssModule">The item to add.</param>
		/// <returns>The zero-based index of the added item.</returns>
		public int Add(RssModule rssModule)
		{
			return List.Add(rssModule);
		}

		/// <summary>Determines whether the RssModuleCollection contains a specific element.</summary>
		/// <param name="rssModule">The RssModule to locate in the RssModuleCollection.</param>
		/// <returns>true if the RssModuleCollection contains the specified value; otherwise, false.</returns>
		public bool Contains(RssModule rssModule)
		{
			return List.Contains(rssModule);
		}

		/// <summary>Copies the entire RssModuleCollection to a compatible one-dimensional <see cref="Array"/>, starting at the specified index of the target array.</summary>
		/// <param name="array">The one-dimensional RssModule Array that is the destination of the elements copied from RssModuleCollection. The Array must have zero-based indexing.</param>
		/// <param name="index">The zero-based index in array at which copying begins.</param>
		/// <exception cref="ArgumentNullException">array is a null reference (Nothing in Visual Basic).</exception>
		/// <exception cref="ArgumentOutOfRangeException">index is less than zero.</exception>
		/// <exception cref="ArgumentException">array is multidimensional. -or- index is equal to or greater than the length of array.-or-The number of elements in the source RssModuleCollection is greater than the available space from index to the end of the destination array.</exception>
		public void CopyTo(RssModule[] array, int index)
		{
			List.CopyTo(array, index);
		}

		/// <summary>Searches for the specified RssModule and returns the zero-based index of the first occurrence within the entire RssModuleCollection.</summary>
		/// <param name="rssModule">The RssModule to locate in the RssModuleCollection.</param>
		/// <returns>The zero-based index of the first occurrence of RssModule within the entire RssModuleCollection, if found; otherwise, -1.</returns>
		public int IndexOf(RssModule rssModule)
		{
			return List.IndexOf(rssModule);
		}

		/// <summary>Inserts an item into this collection at a specified index.</summary>
		/// <param name="index">The zero-based index of the collection at which to insert the item.</param>
		/// <param name="rssModule">The item to insert into this collection.</param>
		public void Insert(int index, RssModule rssModule)
		{
			List.Insert(index, rssModule);
		}

		/// <summary>Removes a specified item from this collection.</summary>
		/// <param name="rssModule">The item to remove.</param>
		public void Remove(RssModule rssModule)
		{
			List.Remove(rssModule);
		}
	}
}
