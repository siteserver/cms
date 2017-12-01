/* RssItemCollection.cs
 * ====================
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
	/// <summary>A strongly typed collection of <see cref="RssItem"/> objects</summary>
	public class RssItemCollection : System.Collections.CollectionBase
	{
		private DateTime latestPubDate = RssDefault.DateTime;
		private DateTime oldestPubDate = RssDefault.DateTime;
		private bool pubDateChanged = true;
		/// <summary>Gets or sets the item at a specified index.<para>In C#, this property is the indexer for the class.</para></summary>
		/// <param name="index">The index of the collection to access.</param>
		/// <value>An item at each valid index.</value>
		/// <remarks>This method is an indexer that can be used to access the collection.</remarks>
		/// <exception cref="ArgumentOutOfRangeException">index is not a valid index.</exception>
		public RssItem this[int index]
		{
			get { return ((RssItem)(List[index])); }
			set 
			{ 
				pubDateChanged = true;
				List[index] = value;
			}
		}
		/// <summary>Adds a specified item to this collection.</summary>
		/// <param name="item">The item to add.</param>
		/// <returns>The zero-based index of the added item.</returns>
		public int Add(RssItem item)
		{
			pubDateChanged = true;
			return List.Add(item);
		}
		/// <summary>Determines whether the RssItemCollection contains a specific element.</summary>
		/// <param name="rssItem">The RssItem to locate in the RssItemCollection.</param>
		/// <returns>true if the RssItemCollection contains the specified value; otherwise, false.</returns>
		public bool Contains(RssItem rssItem)
		{
			return List.Contains(rssItem);
		}
		/// <summary>Copies the entire RssItemCollection to a compatible one-dimensional <see cref="Array"/>, starting at the specified index of the target array.</summary>
		/// <param name="array">The one-dimensional RssItem Array that is the destination of the elements copied from RssItemCollection. The Array must have zero-based indexing.</param>
		/// <param name="index">The zero-based index in array at which copying begins.</param>
		/// <exception cref="ArgumentNullException">array is a null reference (Nothing in Visual Basic).</exception>
		/// <exception cref="ArgumentOutOfRangeException">index is less than zero.</exception>
		/// <exception cref="ArgumentException">array is multidimensional. -or- index is equal to or greater than the length of array.-or-The number of elements in the source RssItemCollection is greater than the available space from index to the end of the destination array.</exception>
		public void CopyTo(RssItem[] array, int index)
		{
			List.CopyTo(array, index);
		}
		/// <summary>Searches for the specified RssItem and returns the zero-based index of the first occurrence within the entire RssItemCollection.</summary>
		/// <param name="rssItem">The RssItem to locate in the RssItemCollection.</param>
		/// <returns>The zero-based index of the first occurrence of RssItem within the entire RssItemCollection, if found; otherwise, -1.</returns>
		public int IndexOf(RssItem rssItem)
		{
			return List.IndexOf(rssItem);
		}
		/// <summary>Inserts an item into this collection at a specified index.</summary>
		/// <param name="index">The zero-based index of the collection at which to insert the item.</param>
		/// <param name="item">The item to insert into this collection.</param>
		public void Insert(int index, RssItem item)
		{
			pubDateChanged = true;
			List.Insert(index, item);
		}
		/// <summary>Removes a specified item from this collection.</summary>
		/// <param name="item">The item to remove.</param>
		public void Remove(RssItem item)
		{
			pubDateChanged = true;
			List.Remove(item);
		}
		/// <summary>The latest pubDate in the items collection</summary>
		/// <value>The latest pubDate -or- RssDefault.DateTime if all item pubDates are not defined</value>
		public DateTime LatestPubDate()
		{
			CalculatePubDates();
			return latestPubDate;
		}
		/// <summary>The oldest pubDate in the items collection</summary>
		/// <value>The oldest pubDate -or- RssDefault.DateTime if all item pubDates are not defined</value>
		public DateTime OldestPubDate()
		{
			CalculatePubDates();
			return oldestPubDate;
		}
		/// <summary>Calculates the oldest and latest pubdates</summary>
		private void CalculatePubDates()
		{
			if (pubDateChanged)
			{
				pubDateChanged = false;
				latestPubDate = DateTime.MinValue;
				oldestPubDate = DateTime.MaxValue;

				foreach(RssItem item in List)
					if ((item.PubDate != RssDefault.DateTime) & (item.PubDate > latestPubDate))
						latestPubDate = item.PubDate;
				if (latestPubDate == DateTime.MinValue)
					latestPubDate = RssDefault.DateTime;
				
				foreach(RssItem item in List)
					if ((item.PubDate != RssDefault.DateTime) & (item.PubDate < oldestPubDate))
						oldestPubDate = item.PubDate;
				if (oldestPubDate == DateTime.MaxValue)
					oldestPubDate = RssDefault.DateTime;
			}
		}
	}
}
