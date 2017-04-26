/* DBBool.cs
 * =========
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
	/// <summary>Represents Null, False, and True</summary>
	/// <remarks>Source: Microsoft c# example</remarks>
    [SerializableAttribute()]
	public struct DBBool
	{
		/// <summary>A DBBool containing 'Null'.</summary>
		/// <remarks>One of three possible DBBool values.</remarks>
		public static readonly DBBool Null = new DBBool(0);
		/// <summary>A DBBool containing 'False'.</summary>
		/// <remarks>One of three possible DBBool values.</remarks>
		public static readonly DBBool False = new DBBool(-1);
		/// <summary>A DBBool containing 'True'.</summary>
		/// <remarks>One of three possible DBBool values.</remarks>
		public static readonly DBBool True = new DBBool(1);
		/// <summary>Private field that stores ?, 0, 1 for False, Null, True.</summary>
		sbyte value;
		/// <summary>Private instance constructor. The value parameter must be ?, 0, or 1.</summary>
		DBBool(int value) 
		{
			this.value = (sbyte)value;
		}
		/// <summary>Properties to examine the value of a DBBool.</summary>
		/// <remarks>Return true if this DBBool has the given value, false otherwise.</remarks>
		public bool IsNull => value == 0;

	    /// <summary>Properties to examine the value of a DBBool.</summary>
		/// <remarks>Return true if this DBBool has the given value, false otherwise.</remarks>
		public bool IsFalse => value < 0;

	    /// <summary>Properties to examine the value of a DBBool.</summary>
		/// <remarks>Return true if this DBBool has the given value, false otherwise.</remarks>
		public bool IsTrue => value > 0;

	    /// <summary>Implicit conversion from bool to DBBool. Maps true to DBBool.True and false to DBBool.False.</summary>
		/// <param name="x">a DBBool</param>
		public static implicit operator DBBool(bool x) 
		{
			return x? True: False;
		}
		/// <summary>Explicit conversion from DBBool to bool.</summary>
		/// <exception cref="InvalidOperationException">The given DBBool is Null</exception>
		/// <param name="x">a DBBool</param>
		/// <returns>true or false</returns>
		public static explicit operator bool(DBBool x) 
		{
			if (x.value == 0) throw new InvalidOperationException();
			return x.value > 0;
		}
		/// <summary>Equality operator.</summary>
		/// <param name="x">a DBBool</param>
		/// <param name="y">a DBBool</param>
		/// <returns>Returns Null if either operand is Null, otherwise returns True or False.</returns>
		public static DBBool operator ==(DBBool x, DBBool y) 
		{
			if (x.value == 0 || y.value == 0) return Null;
			return x.value == y.value? True: False;
		}
		/// <summary>Inequality operator.</summary>
		/// <param name="x">a DBBool</param>
		/// <param name="y">a DBBool</param>
		/// <returns>Returns Null if either operand is Null, otherwise returns True or False.</returns>
		public static DBBool operator !=(DBBool x, DBBool y) 
		{
			if (x.value == 0 || y.value == 0) return Null;
			return x.value != y.value? True: False;
		}
		/// <summary>Logical negation operator.</summary>
		/// <param name="x">a DBBool</param>
		/// <returns>Returns True if the operand is False, Null if the operand is Null, or False if the operand is True.</returns>
		public static DBBool operator !(DBBool x) 
		{
			return new DBBool(-x.value);
		}
		/// <summary>Logical AND operator.</summary>
		/// <param name="x">a DBBool</param>
		/// <param name="y">a DBBool</param>
		/// <returns>Returns False if either operand is False, otherwise Null if either operand is Null, otherwise True.</returns>
		public static DBBool operator &(DBBool x, DBBool y) 
		{
			return new DBBool(x.value < y.value? x.value: y.value);
		}
		/// <summary>Logical OR operator.</summary>
		/// <param name="x">a DBBool</param>
		/// <param name="y">a DBBool</param>
		/// <returns>Returns True if either operand is True, otherwise Null if either operand is Null, otherwise False.</returns>
		public static DBBool operator |(DBBool x, DBBool y) 
		{
			return new DBBool(x.value > y.value? x.value: y.value);
		}
		/// <summary>Definitely true operator.</summary>
		/// <param name="x">a DBBool</param>
		/// <returns>Returns true if the operand is True, false otherwise.</returns>
		public static bool operator true(DBBool x) 
		{
			return x.value > 0;
		}
		/// <summary>Definitely false operator.</summary>
		/// <param name="x">a DBBool</param>
		/// <returns>Returns true if the operand is False, false otherwise.</returns>
		public static bool operator false(DBBool x) 
		{
			return x.value < 0;
		}
		/// <summary>Determines whether two DBBool instances are equal.</summary>
		/// <param name="o">The object to check.</param>
		/// <returns>True if the two DBBools are equal.</returns>
		public override bool Equals(object o) 
		{
			try 
			{
				return (bool) (this == (DBBool) o);
			}
			catch 
			{
				return false;
			}
		}
		/// <summary>Serves as a hash function for a particular type, suitable for use in hashing algorithms and data structures like a hash table.</summary>
		/// <returns>A hash code for the current DBBool.</returns>
		public override int GetHashCode() 
		{
			return value;
		}
		/// <summary>Returns a string representation of the current Object.</summary>
		/// <exception cref="InvalidOperationException">Object has not been initialized.</exception>
		/// <returns>A string containing DBBool.False, DBBool.Null, or DBBool.True</returns>
		public override string ToString() 
		{
			switch (value) 
			{
				case -1:
					return "false";
				case 0:
					return "DBBool.Null";
				case 1:
					return "true";
				default:
					throw new InvalidOperationException();
			}
		}
	}
}
