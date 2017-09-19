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

    #endregion

    public sealed class UnixTime
    {
        private static readonly int[] _days = { -1, 30, 58, 89, 119, 150, 180, 211, 242, 272, 303, 333, 364 };
        private static readonly int[] _leapDays = { -1, 30, 59, 90, 120, 151, 181, 212, 243, 273, 304, 334, 365 };
        
        /// <summary>
        /// Converts a 64-bit Unix time (UTC) into a DateTime instance that
        /// represents the same time in local time.
        /// </summary>
        /// <remarks>
        /// See <a href="http://en.wikipedia.org/wiki/Unix_time">Unix time on Wikipedia</a>
        /// for more information.
        /// </remarks>

        public static DateTime ToDateTime(long time)
        {
            const long secondsPerDay = 24 * 60 * 60;
            const long secondsPerYear = (365 * secondsPerDay);

            //
            // Determine the years since 1900. Start by ignoring leap years.
            //
            
            int tmp = (int) (time / secondsPerYear) + 70;
            time -= ((tmp - 70) * secondsPerYear);

            //
            // Correct for elapsed leap years
            //
            
            time -= (ElapsedLeapYears(tmp) * secondsPerDay);

            //
            // If we have underflowed the long range (i.e., if time < 0),
            // back up one year, adjusting the correction if necessary.
            //
            
            bool isLeapYear = false;

            if (time < 0)
            {
                time += secondsPerYear;
                tmp--;
                
                if (IsLeapYear(tmp))
                {
                    time += secondsPerDay;
                    isLeapYear = true;
                }
            }
            else if (IsLeapYear(tmp))
            {
                isLeapYear = true;
            }

            //
            // tmp now holds the value for year. time now holds the
            // number of elapsed seconds since the beginning of that year.
            //
            
            int year = tmp;

            //
            // Determine days since January 1 (0 - 365). 
            // Leave time with number of elapsed seconds in that day.
            //
            
            int yearDay = (int) (time / secondsPerDay);
            time -= yearDay * secondsPerDay;

            //
            // Determine months since January (0 - 11) and day of month (1 - 31).
            //
            
            int[] yearDaysByMonth = isLeapYear ? _leapDays : _days;
            int month = 1;
            while (yearDaysByMonth[month] < yearDay) month++;

            int mday = yearDay - yearDaysByMonth[month - 1];

            //
            // Determine hours since midnight (0 - 23), minutes after the hour
            // (0 - 59), and seconds after the minute (0 - 59).
            //
            
            int hour = (int) (time / 3600);
            time -= hour * 3600L;

            int min = (int) (time / 60);
            int sec = (int) (time - (min) * 60);
            
            //
            // Finally construct a DateTime instance in UTC from the various 
            // components and then return it adjusted to local time.
            // Note that this could throw an ArgumentException or 
            // ArgumentOutOfRangeException, which is fine by us and we'll
            // let it propagate.
            //

            return (new DateTime(year + 1900, month, mday, hour, min, sec)).ToLocalTime();
        }

        public static long ToInt64(DateTime time)
        {
            return (long) (time.ToUniversalTime() - new DateTime(1970, 1, 1)).TotalSeconds;
        }

        /// <summary>
        /// Determine if a given year, expressed as the number of years since
        /// 1900, is a leap year.
        /// </summary>

        private static bool IsLeapYear(int y)
        {
            return (((y % 4 == 0) && (y % 100 != 0)) || ((y + 1900) % 400 == 0));
        }

        /// <summary>
        /// Number of leap years from 1970 up to, but not including, 
        /// the specified year expressed as the number of years since 1900.
        /// </summary>
        
        private static long ElapsedLeapYears(int y)
        {
            return (((y - 1) / 4) - ((y - 1) / 100) + ((y + 299) / 400) - /* Leap years 1900 - 1970 = */ 17);
        }

        private UnixTime()
        {
            throw new NotSupportedException();
        }
    }
}
