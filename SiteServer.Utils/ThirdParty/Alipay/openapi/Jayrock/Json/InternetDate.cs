#region License, Terms and Conditions
//
// Jayrock - JSON and JSON-RPC for Microsoft .NET Framework and Mono
// Written by Atif Aziz (atif.aziz@skybow.com)
// Copyright (c) 2005 Atif Aziz. All rights reserved.
//
// This library is free software; you can redistribute it and/or modify it under
// the terms of the GNU Lesser General Public License as published by the Free
// Software Foundation; either version 2.1 of the License, or (at your option)
// any later version.
//
// This library is distributed in the hope that it will be useful, but WITHOUT
// ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS
// FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License for more
// details.
//
// You should have received a copy of the GNU Lesser General Public License
// along with this library; if not, write to the Free Software Foundation, Inc.,
// 59 Temple Place, Suite 330, Boston, MA 02111-1307 USA 
//
#endregion

namespace Jayrock
{
    #region Imports

    using System;
    using System.Globalization;

    #endregion

    /// <summary>
    /// Provides date and time parsing according to the formats described in 
    /// RFC 822/1123 specification.
    /// </summary>

    public sealed class InternetDate
    {
        private static readonly string[] _formats =
        {
                "dd MMM yyyy HH':'mm",
                "dd MMM yyyy HH':'mm':'ss",
                "ddd, dd MMM yyyy HH':'mm",
                "ddd, dd MMM yyyy HH':'mm':'ss",
        };

        public static DateTime Parse(string input)
        {
            if (input == null)
                throw new ArgumentNullException("input");

            if (input.Length < _formats[0].Length)
                throw new ArgumentException("input");

            //
            // Parse according to the following syntax:
            //
            //   date-time   =  [ day "," ] date time ; dd mm yy
            //                                        ;  hh:mm:ss zzz
            //  
            //   day         =  "Mon"  / "Tue" /  "Wed"  / "Thu"
            //               /  "Fri"  / "Sat" /  "Sun"
            //  
            //   date        =  1*2DIGIT month 2DIGIT ; day month year
            //                                        ;  e.g. 20 Jun 82
            //  
            //   month       =  "Jan"  /  "Feb" /  "Mar"  /  "Apr"
            //               /  "May"  /  "Jun" /  "Jul"  /  "Aug"
            //               /  "Sep"  /  "Oct" /  "Nov"  /  "Dec"
            //  
            //   time        =  hour zone             ; ANSI and Military
            //  
            //   hour        =  2DIGIT ":" 2DIGIT [":" 2DIGIT]
            //                                        ; 00:00:00 - 23:59:59
            //  
            //   zone        =  "UT"  / "GMT"         ; Universal Time
            //                                        ; North American : UT
            //               /  "EST" / "EDT"         ;  Eastern:  - 5/ - 4
            //               /  "CST" / "CDT"         ;  Central:  - 6/ - 5
            //               /  "MST" / "MDT"         ;  Mountain: - 7/ - 6
            //               /  "PST" / "PDT"         ;  Pacific:  - 8/ - 7
            //               /  1ALPHA                ; Military: Z = UT;
            //                                        ;  A:-1; (J not used)
            //                                        ;  M:-12; N:+1; Y:+12
            //               / ( ("+" / "-") 4DIGIT ) ; Local differential
            //                                        ;  hours+min. (HHMM)
            //
            // For more information, see:
            // http://www.w3.org/Protocols/rfc822/#z28
            //

            //
            // Start by processing the time zone component, which is the 
            // part that cannot be delegated to DateTime.ParseExact.
            //

            int zzz; // time zone offset stored as HH * 100 + MM

            int zoneSpaceIndex = input.LastIndexOf(' ');

            if (zoneSpaceIndex <= 0)
                throw new FormatException();

            string zone = input.Substring(zoneSpaceIndex + 1);

            if (zone.Length == 0)
                throw new FormatException("Missing time zone.");

            switch (zone)
            {
                //
                // Greenwich Mean Time (GMT) or Universal Time (UT)
                //

                case "UT" :
                case "GMT" : zzz = +0000; break;

                //
                // Common North American time zones
                //
                
                case "EDT" : zzz = -0400; break;
                case "EST" : 
                case "CDT" : zzz = -0500; break;
                case "CST" : 
                case "MDT" : zzz = -0600; break;
                case "MST" : 
                case "PDT" : zzz = -0700; break;
                case "PST" : zzz = -0800; break;

                //
                // Local differential = ( "+" / "-" ) HHMM
                //

                default :
                {
                    if (zone.Length < 4)
                        throw new FormatException("Length of local differential component must be at least 4 characters (HHMM).");

                    try
                    {
                        zzz = int.Parse(zone, NumberStyles.AllowLeadingSign, CultureInfo.InvariantCulture);
                    }
                    catch (FormatException e)
                    {
                        throw new FormatException("Invalid local differential.", e);
                    }

                    break;
                }
            }                

            //
            // Strip the time zone component along with any trailing space 
            // and parse out just the time piece by simply delegating to 
            // DateTime.ParseExact.
            //

            input = input.Substring(0, zoneSpaceIndex).TrimEnd();
            DateTime time = DateTime.ParseExact(input, _formats, CultureInfo.InvariantCulture, DateTimeStyles.AllowInnerWhite);

            //
            // Subtract the offset to produce zulu time and then return the 
            // result as local time.
            //

            TimeSpan offset = new TimeSpan(zzz / 100, zzz % 100, 0);                    
            return time.Subtract(offset).ToLocalTime();
        }

        private InternetDate()
        {
            throw new NotSupportedException();   
        }
    }
}
