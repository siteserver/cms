/* 
  	* AtomDateConstruct.cs
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
using System.Xml.XPath;
using Atom.Utils;

namespace Atom.Core
{
	/// <summary>
	/// The class representing all date contructs.
	/// </summary>
	[Serializable]
	public class AtomDateConstruct : AtomElement
	{
		#region Constructors
		
		/// <summary>
		/// Represents an <see cref="AtomDateConstruct"/> instance.
		/// Initialize the <see cref="AtomElement.LocalName"/> to modified, the <see cref="DateTime"/> to <see cref="System.DateTime.Now"/>
		/// and <see cref="UtcOffset"/> to <see cref="DefaultValues.UtcOffset"/>.
		/// </summary>
		public AtomDateConstruct() : this("modified")
		{
		}

		/// <summary>
		/// Represents an <see cref="AtomDateConstruct"/> instance.
		/// Initialize the <see cref="DateTime"/> to <see cref="System.DateTime.Now"/>
		/// and <see cref="UtcOffset"/> to <see cref="DefaultValues.UtcOffset"/>.
		/// </summary>
		public AtomDateConstruct(string localName) : this(localName, DateTime.Now)
		{
		}
		
		/// <summary>
		/// Represents an <see cref="AtomDateConstruct"/> instance initialized with the given <see cref="DateTime"/>.
		/// The <see cref="AtomDateConstruct.UtcOffset"/> defaults to <see cref="DefaultValues.UtcOffset"/>.
		/// </summary>
		/// <param name="localName">The not qualified name of the element.</param>
		/// <param name="dateTime">The <see cref="DateTime"/> of the <see cref="AtomDateConstruct"/> instance.</param>
		public AtomDateConstruct(string localName, DateTime dateTime) : this(localName, dateTime, DefaultValues.UtcOffset)
		{
		}

		/// <summary>
		/// Represents an <see cref="AtomDateConstruct"/> instance initialized with the given <see cref="DateTime"/> and offset.
		/// </summary>
		/// <param name="localName">The not qualified name of the element.</param>
		/// <param name="dateTime">The <see cref="DateTime"/> of the <see cref="AtomDateConstruct"/> instance.</param>
		/// <param name="utcOffset">The <see cref="UtcOffset"/> of the <see cref="AtomDateConstruct"/> instance.</param>
		public AtomDateConstruct(string localName, DateTime dateTime, TimeSpan utcOffset)
		{
			this.LocalName = localName;
			this.DateTime = dateTime;
			this.UtcOffset = utcOffset;
		}
	
		#endregion

		private DateTime _dateTime = DefaultValues.DateTime;
		private TimeSpan _utcOffset = DefaultValues.UtcOffset;

		#region Properties

		/// <summary>
		/// The date time of the date construct.
		/// </summary>
		public DateTime DateTime
		{
			get { return _dateTime; }
			set { _dateTime = value; }
		}

		/// <summary>
		/// The UTC offset of the date.
		/// 
		/// Example:
		/// <code>TimeSpan utcOffset = TimeZone.CurrentTimeZone.GetUtcOffset(DateTime.Today)</code>
		/// </summary>
		public TimeSpan UtcOffset
		{
			get { return _utcOffset; }
			set { _utcOffset = value; }
		}

		#endregion

		#region ToString method
		
		/// <summary>
		/// Converts the <see cref="AtomDateConstruct"/> in a series of xml nodes.
		/// </summary>
		/// <returns>The string representation of <see cref="AtomDateConstruct"/> class.</returns>
		public override string ToString()
		{
			this.WriteStartElement();

			if(this.DateTime == DefaultValues.DateTime)
				throw new RequiredElementNotFoundException("The date/time is missing.");

			this.WriteAttribute("xml:lang", Utils.Utils.ParseLanguage(this.XmlLang), false, null);
			this.Buffer.Append(">");

			string utcOffset = String.Empty;
			string sign = String.Empty;

			if(this.UtcOffset == DefaultValues.UtcOffset)
				utcOffset = "Z";
			else
			{

				utcOffset = this.UtcOffset.ToString().Substring(0,
					this.UtcOffset.ToString().Length - 3);
				sign = "+";

				// check if time zone is before GMT
				if(utcOffset[0].Equals('-'))
					sign = String.Empty;
			}

			string dateTime = this.DateTime.ToString("s");

			string tempStr = string.Format("{0}{1}{2}",
				dateTime,
				sign,
				utcOffset);
			
			this.Buffer.Append(tempStr);

			this.WriteEndElement();

			string output = this.Buffer.ToString();
			this.Buffer.Length = 0;

			return output;
		}

		#endregion ToString method

		#region ToString helper methods

		/// <summary>
		/// Writes the start tag of the element.
		/// </summary>
		protected internal override void WriteStartElement()
		{
			this.Buffer.AppendFormat("<{0}", this.LocalName);
		}

		/// <summary>
		/// Writes the end tag of the element.
		/// </summary>
		protected internal override void WriteEndElement()
		{
			this.Buffer.AppendFormat("</{0}>", this.LocalName);
			this.Buffer.Append(Environment.NewLine);
		}

		#endregion

		#region XPath parsing stuff
		internal static AtomDateConstruct Parse(XPathNavigator navigator)
		{
			AtomDateConstruct dateElement = new AtomDateConstruct();
			string temp = String.Empty;

			XPathNavigator nav = navigator.Clone();
			XPathNodeIterator iter = nav.SelectDescendants(XPathNodeType.Element, true);
			
			while(iter.MoveNext())
			{
				string name = iter.Current.Name.ToLower();
				int idx = name.IndexOf(":");
				if(idx != -1)
					name = name.Split(new char[] {':'}, 2)[1];

				switch(name)
				{
					case "modified":
					case "issued":
					case "created":
						try
						{
							dateElement.XmlLang = Utils.Utils.ParseLanguage(iter.Current.XmlLang);
						}
						catch {}
						dateElement.LocalName = name;
						temp = iter.Current.Value;
						break;
				}
			}

			switch(dateElement.LocalName)
			{
				case "modified":
				case "created":
					ComputeModifiedCreatedDate(ref dateElement, temp);
					break;

				case "issued":
					ComputeIssuedDate(ref dateElement, temp);
					break;
			}

			return dateElement;
		}
		#endregion

		#region Private methods
		private static void ComputeModifiedCreatedDate(ref AtomDateConstruct element, string temp)
		{
			string dateTime = String.Empty;
			string offset = String.Empty;
			char[] chrs = {'+', '-'};
			bool hasOffset = true;

			if(Atom.Utils.Utils.IsIso8601DateLocal(temp))
			{
				try
				{
					dateTime = temp.Substring(0, temp.LastIndexOfAny(chrs));
				}
				catch(ArgumentNullException)
				{
					dateTime = temp;
					hasOffset = false;
				}

				if(hasOffset)
				{
					int index = temp.LastIndexOfAny(chrs);
					int length = temp.Length - index;
					offset = temp.Substring(index, length);

					// remove the leading + cause TimeSpan.Parse gives
					// an exception
					if(offset.StartsWith("+"))
						offset = offset.Substring(1, offset.Length - 1);

					element.UtcOffset = TimeSpan.Parse(offset);
				}
			}
			else if(Atom.Utils.Utils.IsIso8601DateTZ(temp))
			{
				dateTime = temp.Substring(0, temp.IndexOf('Z'));
			}
			else
				throw new FormatException("The given date is not in ISO 8601 format.");

			element.DateTime = DateTime.Parse(dateTime);
		}

		private static void ComputeIssuedDate(ref AtomDateConstruct element, string temp)
		{
			string dateTime = String.Empty;
			string offset = String.Empty;
			char[] chrs = {'+', '-'};
			bool hasOffset = true;

			if(Atom.Utils.Utils.IsIso8601DateLocalNoTZ(temp))
			{
				dateTime = temp;
			}
			else if(Atom.Utils.Utils.IsIso8601DateLocal(temp))
			{
				try
				{
					dateTime = temp.Substring(0, temp.LastIndexOfAny(chrs));
				}
				catch(ArgumentNullException)
				{
					dateTime = temp;
					hasOffset = false;
				}

				if(hasOffset)
				{
					int index = temp.LastIndexOfAny(chrs);
					int length = temp.Length - index;
					offset = temp.Substring(index, length);

					// remove the leading + cause TimeSpan.Parse gives
					// an exception
					if(offset.StartsWith("+"))
						offset = offset.Substring(1, offset.Length - 1);

					element.UtcOffset = TimeSpan.Parse(offset);
				}
			}
			else if(Atom.Utils.Utils.IsIso8601DateTZ(temp))
			{
				dateTime = temp.Substring(0, temp.IndexOf('Z'));
			}
			else
				throw new FormatException("The given date is not in ISO 8601 format.");

			element.DateTime = DateTime.Parse(dateTime);
		}
		#endregion
	}
}
