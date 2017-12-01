/* 
  	* Utils.cs
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
using Atom.Core;
using System;
using System.Collections;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Reflection;
using System.Web;

namespace Atom.Utils
{
	/// <summary>
	/// A class with some useful methods.
	/// </summary>
	public sealed class Utils
	{
		private static Hashtable _mediaTypes = null;

		#region Constructors
		private Utils() {}

		/// <summary>
		/// Read in the hash table the contents of the embedded text file
		/// containing the media types
		/// </summary>
		static Utils()
		{
			Stream stream =
				Assembly.GetExecutingAssembly().GetManifestResourceStream("BaiRong.Core.ThirdParty.Atom.mediatypes.txt");
			_mediaTypes = new Hashtable();

			using(StreamReader reader = new StreamReader(stream))
			{
				string line = String.Empty;
				string type = String.Empty;
				int i = 0;

				line = reader.ReadLine();
				while(line != null)
				{
					type = line.Split(new char[] {' '}, 2)[0];
					_mediaTypes.Add(i, type);
					i++;
					line = reader.ReadLine();
				}
			}
            
		}
		#endregion
		
		#region Public methods

		/// <summary>
		/// Escapes the given <see cref="String"/>.
		/// </summary>
		/// <param name="buffer">The <see cref="String"/> to escape.</param>
		/// <returns>The escaped <see cref="String"/>.</returns>
		public static string Escape(string buffer)
		{
			return HttpUtility.HtmlEncode(buffer);
		}

		/// <summary>
		/// Unescapes the given <see cref="String"/>.
		/// </summary>
		/// <param name="buffer">The <see cref="String"/> to unescape.</param>
		/// <returns>The unescaped <see cref="String"/>.</returns>
		public static string Unescape(string buffer)
		{
			return HttpUtility.HtmlDecode(buffer);
		}

		/// <summary>
		/// Base64-encodes the given byte array.
		/// </summary>
		/// <param name="array">The byte array to encode.</param>
		/// <returns>A base64-encoded string.</returns>
		public static string Base64Encode(byte[] array)
		{
			return Convert.ToBase64String(array);
		}

		/// <summary>
		/// Base64-encodes the given byte array from the given offset to "len" number of bytes.
		/// </summary>
		/// <param name="array">The byte array to encode.</param>
		/// <param name="offset">The offset from which the encoding starts.</param>
		/// <param name="length">The number of bytes from the offset to encode.</param>
		/// <returns>A base64-encoded string.</returns>
		public static string Base64Encode(byte[] array, int offset, int length)
		{
			return Convert.ToBase64String(array, offset, length);
		}

		/// <summary>
		/// Base64-encodes the given <see cref="String"/>.
		/// </summary>
		/// <param name="buffer">The string to encode.</param>
		/// <returns>A base64-encoded string.</returns>
		public static string Base64Encode(string buffer)
		{
			byte[] arr = Encoding.ASCII.GetBytes(buffer);
			return Base64Encode(arr);
		}

		/// <summary>
		/// Base64-decodes the given byte array.
		/// </summary>
		/// <param name="array">The byte array to decode.</param>
		/// <returns>A base64-decoded array of bytes.</returns>
		public static byte[] Base64Decode(char[] array)
		{
			return Convert.FromBase64CharArray(array, 0, array.Length);
		}

		/// <summary>
		/// Base64-decodes the given byte array from the given offset to "len" number of bytes.
		/// </summary>
		/// <param name="array">The byte array to decode.</param>
		/// <param name="offset">The offset from which the decoding starts.</param>
		/// <param name="length">The number of bytes from the offset to decode.</param>
		/// <returns>A base64-encoded array of bytes.</returns>
		public static byte[] Base64Decode(char[] array, int offset, int length)
		{
			return Convert.FromBase64CharArray(array, offset, length);
		}

		/// <summary>
		/// Base64-decodes the given <see cref="String"/>.
		/// </summary>
		/// <param name="buffer">The string to decode.</param>
		/// <returns>A base64-decoded array of bytes.</returns>
		public static byte[] Base64Decode(string buffer)
		{
			return Convert.FromBase64String(buffer);
		}

		/// <summary>
		/// Checks if the given string representation of a date matches the ISO 8601 format.
		/// </summary>
		/// <param name="theDate">The datetime to check.</param>
		/// <returns>true if the given date is in ISO 8601 format, false otherwise.</returns>
		public static bool IsIso8601Date(string theDate)
		{
			string regExp = @"\d\d\d\d(-\d\d(-\d\d(T\d\d:\d\d(:\d\d(\.\d*)?)?(Z|([+-]\d\d:\d\d))?)?)?)?$";

            Regex rx = new Regex(regExp, RegexOptions.IgnoreCase);
			if(rx.IsMatch(theDate))
				return true;

			return false;
		}

		/// <summary>
		/// Checks if the given string representation of a date matches the ISO 8601 format with the UTC time zone.
		/// </summary>
		/// <param name="theDate">The datetime to check.</param>
		/// <returns>true if the given date is in ISO 8601 format with the UTC time zone, false otherwise.</returns>
		public static bool IsIso8601DateTZ(string theDate)
		{
			if(IsIso8601Date(theDate))
			{
				string regExp = @"Z|([+-]\d\d:\d\d)$";

				Regex rx = new Regex(regExp, RegexOptions.IgnoreCase | RegexOptions.Multiline);
				if(rx.IsMatch(theDate))
					return true;
				else if(theDate.IndexOf('Z') != -1)
					return true;
			}

			return false;
		}

		/// <summary>
		/// Checks if the given string representation of a date matches the ISO 8601 format without the UTC time zone.
		/// </summary>
		/// <param name="theDate">The datetime to check.</param>
		/// <returns>true if the given date is in ISO 8601 format with the UTC time zone, false otherwise.</returns>
		public static bool IsIso8601DateLocalNoTZ(string theDate)
		{
			bool val = IsIso8601DateTZ(theDate);
			
			return !val;
		}

		/// <summary>
		/// Checks if the given string representation of a date matches the ISO 8601 format with a local time zone.
		/// </summary>
		/// <param name="theDate">The datetime to check.</param>
		/// <returns>true if the given date is in ISO 8601 format with a local time zone, false otherwise.</returns>
		public static bool IsIso8601DateLocal(string theDate)
		{
			if(IsIso8601Date(theDate))
			{
				if(theDate.IndexOf('Z') != -1)
					return false;

				return true;
			}

			return false;
		}

		/// <summary>
		/// Checks if the given parameter is a valid email address.
		/// </summary>
		/// <param name="email">The email to check.</param>
		/// <returns>true if the given email is valid, false otherwise.</returns>
		public static bool IsEmail(string email)
		{
			string regExp = @"([a-zA-Z0-9_\-\+\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$";

			Regex rx = new Regex(regExp, RegexOptions.IgnoreCase | RegexOptions.Multiline);
			if(rx.IsMatch(email))
				return true;

			return false;
		}

		/// <summary>
		/// Gets the versions of the library.
		/// </summary>
		/// <returns>The major, minor, revision and build numbers for the library.</returns>
		public static string GetVersion()
		{
			Assembly assembly = Assembly.GetExecutingAssembly();
			return assembly.GetName().Version.ToString();
		}

		#endregion

		#region Internal methods

		/// <summary>
		/// Parses the <see cref="Relationship"/> enum.
		/// </summary>
		/// <param name="rel">The relationship value to parse.</param>
		/// <returns>The string representation of the relationship value.</returns>
		internal static string ParseRelationship(Relationship rel)
		{
			string val = String.Empty;

			switch(rel)
			{
				case Relationship.Alternate:
					val = "alternate";
					break;

				case Relationship.Next:
					val = "next";
					break;

				case Relationship.Prev:
					val = "prev";
					break;

				case Relationship.ServiceEdit:
					val = "service.edit";
					break;

				case Relationship.ServiceFeed:
					val = "service.feed";
					break;

				case Relationship.ServicePost:
					val = "service.post";
					break;

				case Relationship.Start:
					val = "start";
					break;
			}
			
			return val;
		}

		/// <summary>
		/// Parses a rel attribute.
		/// </summary>
		/// <param name="rel">The relationship value to parse.</param>
		/// <returns>The enum value of the given relationship string.</returns>
		internal static Relationship ParseRelationship(string rel)
		{
			switch(rel)
			{
				case "alternate":
					return Relationship.Alternate;

				case "next":
					return Relationship.Next;

				case "prev":
					return Relationship.Prev;

				case "service.edit":
					return Relationship.ServiceEdit;

				case "service.feed":
					return Relationship.ServiceFeed;

				case "service.post":
					return Relationship.ServicePost;

				case "start":
					return Relationship.Start;

				default:
					return Relationship.Alternate;
			}
		}

		/// <summary>
		/// Parses the <see cref="MediaType"/> enum.
		/// </summary>
		/// <param name="type">The media type to parse.</param>
		/// <returns>The string representation of the media type.</returns>
		internal static string ParseMediaType(MediaType type)
		{
			string val = String.Empty;

			try
			{
				val = (string) _mediaTypes[Convert.ToInt32(type)];
			}
			catch
			{
				val = "text/plain";
			}
			
			return val;
		}

		/// <summary>
		/// Parses a media type string.
		/// </summary>
		/// <param name="type">The media type to parse.</param>
		/// <returns>The enum value of the given media type.</returns>
		internal static MediaType ParseMediaType(string type)
		{
			if(_mediaTypes.ContainsValue(type))
			{
				MediaType mtype = MediaType.UnknownType;
				foreach(DictionaryEntry entry in _mediaTypes)
				{
					string val = (string) entry.Value;
					if(val == type)
					{
						try
						{
							int key = Convert.ToInt32(entry.Key.ToString());
							mtype = (MediaType) key;
						}
						catch
						{
							return MediaType.UnknownType;
						}
					}
				}
				return mtype;
			}
			return MediaType.UnknownType;
		}

		/// <summary>
		/// Parses the <see cref="Language"/> enum.
		/// </summary>
		/// <param name="lang">The language to parse.</param>
		/// <returns>The string representation of the language.</returns>
		internal static string ParseLanguage(Language lang)
		{
			if(lang == Language.UnknownLanguage)
				return String.Empty;

			return lang.ToString().Replace("_", "-");
		}

		/// <summary>
		/// Parses the string representation of a language
		/// </summary>
		/// <param name="lang">The language to parse.</param>
		/// <returns>The <see cref="Language"/> enum value.</returns>
		internal static Language ParseLanguage(string lang)
		{
			if(lang != null && lang.Length > 0)
			{
				string temp = lang.Replace("-", "_");
				return (Language) Enum.Parse(typeof(Language), temp, true);
			}
			return Language.UnknownLanguage;
		}

		#endregion
	}
}