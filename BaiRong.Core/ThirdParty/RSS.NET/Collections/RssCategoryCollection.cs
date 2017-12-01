/* RssCategoryCollection.cs
 * ========================
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
using System.Collections;

namespace BaiRong.Core.Rss
{
	/// <summary>A strongly typed collection of <see cref="RssCategory"/> objects</summary>
    [SerializableAttribute()]
	public class RssCategoryCollection : CollectionBase
	{
		/// <summary>Gets or sets the category at a specified index.<para>In C#, this property is the indexer for the class.</para></summary>
		/// <param name="index">The index of the collection to access.</param>
		/// <value>A category at each valid index.</value>
		/// <remarks>This method is an indexer that can be used to access the collection.</remarks>
		/// <exception cref="ArgumentOutOfRangeException">index is not a valid index.</exception>
		public RssCategory this[int index]
		{
			get { return ((RssCategory)(List[index])); }
			set { List[index] = value; }
		}
		/// <summary>Adds a specified category to this collection.</summary>
		/// <param name="rssCategory">The category to add.</param>
		/// <returns>The zero-based index of the added category.</returns>
		public int Add(RssCategory rssCategory)
		{
			return List.Add(rssCategory);
		}
		/// <summary>Determines whether the RssCategoryCollection contains a specific element.</summary>
		/// <param name="rssCategory">The RssCategory to locate in the RssCategoryCollection.</param>
		/// <returns>true if the RssCategoryCollection contains the specified value; otherwise, false.</returns>
		public bool Contains(RssCategory rssCategory)
		{
			return List.Contains(rssCategory);
		}
		/// <summary>Copies the entire RssCategoryCollection to a compatible one-dimensional <see cref="Array"/>, starting at the specified index of the target array.</summary>
		/// <param name="array">The one-dimensional RssCategory Array that is the destination of the elements copied from RssCategoryCollection. The Array must have zero-based indexing.</param>
		/// <param name="index">The zero-based index in array at which copying begins.</param>
		/// <exception cref="ArgumentNullException">array is a null reference (Nothing in Visual Basic).</exception>
		/// <exception cref="ArgumentOutOfRangeException">index is less than zero.</exception>
		/// <exception cref="ArgumentException">array is multidimensional. -or- index is equal to or greater than the length of array.-or-The number of elements in the source RssCategoryCollection is greater than the available space from index to the end of the destination array.</exception>
		public void CopyTo(RssCategory[] array, int index)
		{
			List.CopyTo(array, index);
		}
		/// <summary>Searches for the specified RssCategory and returns the zero-based index of the first occurrence within the entire RssCategoryCollection.</summary>
		/// <param name="rssCategory">The RssCategory to locate in the RssCategoryCollection.</param>
		/// <returns>The zero-based index of the first occurrence of RssCategory within the entire RssCategoryCollection, if found; otherwise, -1.</returns>
		public int IndexOf(RssCategory rssCategory)
		{
			return List.IndexOf(rssCategory);
		}
		/// <summary>Inserts an category into this collection at a specified index.</summary>
		/// <param name="index">The zero-based index of the collection at which to insert the category.</param>
		/// <param name="rssCategory">The category to insert into this collection.</param>
		public void Insert(int index, RssCategory rssCategory)
		{
			List.Insert(index, rssCategory);
		}

		/// <summary>Removes a specified category from this collection.</summary>
		/// <param name="rssCategory">The category to remove.</param>
		public void Remove(RssCategory rssCategory)
		{
			List.Remove(rssCategory);
		}
	}
}
