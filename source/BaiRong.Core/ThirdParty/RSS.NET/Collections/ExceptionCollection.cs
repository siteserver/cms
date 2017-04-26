/* ExceptionCollection.cs
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
using System.Collections;

namespace BaiRong.Core.Rss
{
	/// <summary>A strongly typed collection of <see cref="Exception"/> objects</summary>
    [SerializableAttribute()]
	public class ExceptionCollection : CollectionBase
	{
		private Exception lastException = null;

		/// <summary>Gets or sets the exception at a specified index.<para>In C#, this property is the indexer for the class.</para></summary>
		/// <param name="index">The index of the collection to access.</param>
		/// <value>A exception at each valid index.</value>
		/// <remarks>This method is an indexer that can be used to access the collection.</remarks>
		public Exception this[int index]
		{
			get { return ((Exception)(List[index])); }
			set { List[index] = value; }
		}
		/// <summary>Adds a specified exception to this collection.</summary>
		/// <param name="exception">The exception to add.</param>
		/// <returns>The zero-based index of the added exception -or- -1 if the exception already exists.</returns>
		public int Add(Exception exception)
		{
			foreach(Exception e in List)
				if (e.Message == exception.Message)
					return -1;
			lastException = exception;
			return List.Add(exception);
		}
		/// <summary>Determines whether the ExceptionCollection contains a specific element.</summary>
		/// <param name="exception">The Exception to locate in the ExceptionCollection.</param>
		/// <returns>true if the ExceptionCollection contains the specified value; otherwise, false.</returns>
		public bool Contains(Exception exception)
		{
			return List.Contains(exception);
		}
		/// <summary>Copies the entire ExceptionCollection to a compatible one-dimensional <see cref="Array"/>, starting at the specified index of the target array.</summary>
		/// <param name="array">The one-dimensional Exception Array that is the destination of the elements copied from ExceptionCollection. The Array must have zero-based indexing.</param>
		/// <param name="index">The zero-based index in array at which copying begins.</param>
		/// <exception cref="ArgumentNullException">array is a null reference (Nothing in Visual Basic).</exception>
		/// <exception cref="ArgumentOutOfRangeException">index is less than zero.</exception>
		/// <exception cref="ArgumentException">array is multidimensional. -or- index is equal to or greater than the length of array.-or-The number of elements in the source ExceptionCollection is greater than the available space from index to the end of the destination array.</exception>
		public void CopyTo(Exception[] array, int index)
		{
			List.CopyTo(array, index);
		}
		/// <summary>Searches for the specified Exception and returns the zero-based index of the first occurrence within the entire ExceptionCollection.</summary>
		/// <param name="exception">The Exception to locate in the ExceptionCollection.</param>
		/// <returns>The zero-based index of the first occurrence of RssChannel within the entire ExceptionCollection, if found; otherwise, -1.</returns>
		public int IndexOf(Exception exception)
		{
			return List.IndexOf(exception);
		}
		/// <summary>Inserts an Exception into this collection at a specified index.</summary>
		/// <param name="index">The zero-based index of the collection at which to insert the Exception.</param>
		/// <param name="exception">The Exception to insert into this collection.</param>
		public void Insert(int index, Exception exception)
		{
			List.Insert(index, exception);
		}

		/// <summary>Removes a specified Exception from this collection.</summary>
		/// <param name="exception">The Exception to remove.</param>
		public void Remove(Exception exception)
		{
			List.Remove(exception);
		}
		/// <summary>Returns the last exception added through the Add method.</summary>
		/// <value>The last exception -or- null if no exceptions exist</value>
		public Exception LastException => lastException;
	}
}
