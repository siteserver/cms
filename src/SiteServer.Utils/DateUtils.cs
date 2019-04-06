using System;
using System.Globalization;
using SiteServer.Utils.Enumerations;

namespace SiteServer.Utils
{
	public static class DateUtils
	{
        public const string FormatStringDateTime = "yyyy-MM-dd HH:mm:ss";
        public const string FormatStringDateOnly = "yyyy-MM-dd";

	    public static DateTime GetExpiresAt(TimeSpan expiresAt)
	    {
	        return DateTime.UtcNow.Add(expiresAt);
	    }

	    public static string GetRelatedDateTimeString(DateTimeOffset? offset)
	    {
	        return offset.HasValue ? GetRelatedDateTimeString(offset.Value.DateTime) : string.Empty;
	    }

        public static string GetRelatedDateTimeString(DateTime datetime)
        {
            string retVal;
            var interval = DateTime.Now - datetime;
            if (interval.Days > 0)
            {
                if (interval.Days >= 7 && interval.Days < 35)
                {
                    retVal = $"{interval.Days/7}周";
                }
                else
                {
                    retVal = $"{interval.Days}天";
                }
            }
            else if (interval.Hours > 0)
            {
                retVal = $"{interval.Hours}小时";
            }
            else if (interval.Minutes > 0)
            {
                retVal = $"{interval.Minutes}分钟";
            }
            else if (interval.Seconds > 0)
            {
                retVal = $"{interval.Seconds}秒";
            }
            else if (interval.Milliseconds > 0)
            {
                retVal = $"{interval.Milliseconds}毫秒";
            }
            else
            {
                retVal = "1毫秒";
            }
            return retVal;
        }

	    public static string GetDateAndTimeString(DateTimeOffset? offset, EDateFormatType dateFormat, ETimeFormatType timeFormat)
	    {
	        return offset.HasValue ? GetDateAndTimeString(offset.Value.DateTime, dateFormat, timeFormat) : string.Empty;
	    }

        public static string GetDateAndTimeString(DateTime datetime, EDateFormatType dateFormat, ETimeFormatType timeFormat)
        {
            return $"{GetDateString(datetime, dateFormat)} {GetTimeString(datetime, timeFormat)}";
        }

	    public static string GetDateAndTimeString(DateTimeOffset? offset)
	    {
	        return offset.HasValue ? GetDateAndTimeString(offset.Value.DateTime) : string.Empty;
	    }

        public static string GetDateAndTimeString(DateTime datetime)
        {
            if (datetime == SqlMinValue || datetime == DateTime.MinValue) return string.Empty;
            return GetDateAndTimeString(datetime, EDateFormatType.Day, ETimeFormatType.ShortTime);
        }

	    public static string GetDateString(DateTimeOffset? offset)
	    {
	        return offset.HasValue ? GetDateString(offset.Value.DateTime) : string.Empty;
        }

        public static string GetDateString(DateTime datetime)
        {
            if (datetime == SqlMinValue || datetime == DateTime.MinValue) return string.Empty;
            return GetDateString(datetime, EDateFormatType.Day);
        }

	    public static string GetDateString(DateTimeOffset? offset, EDateFormatType dateFormat)
	    {
	        return offset.HasValue ? GetDateString(offset.Value.DateTime, dateFormat) : string.Empty;
	    }

        public static string GetDateString(DateTime datetime, EDateFormatType dateFormat)
        {
            var format = string.Empty;
            if (dateFormat == EDateFormatType.Year)
            {
                format = "yyyy年MM月";
            }
            else if (dateFormat == EDateFormatType.Month)
            {
                format = "MM月dd日";
            }
            else if (dateFormat == EDateFormatType.Day)
            {
                format = "yyyy-MM-dd";
            }
            else if (dateFormat == EDateFormatType.Chinese)
            {
                format = "yyyy年M月d日";
            }
            return datetime.ToString(format);
        }

	    public static string GetTimeString(DateTimeOffset? offset)
	    {
	        return offset.HasValue ? GetTimeString(offset.Value.DateTime) : string.Empty;
	    }

        public static string GetTimeString(DateTime datetime)
		{
			return GetTimeString(datetime, ETimeFormatType.ShortTime);
		}

	    public static string GetTimeString(DateTimeOffset? offset, ETimeFormatType timeFormat)
	    {
	        return offset.HasValue ? GetTimeString(offset.Value.DateTime, timeFormat) : string.Empty;
	    }

        public static string GetTimeString(DateTime datetime, ETimeFormatType timeFormat)
        {
            var retVal = string.Empty;
            if (timeFormat == ETimeFormatType.LongTime)
            {
                retVal = datetime.ToLongTimeString();
            }
            else if (timeFormat == ETimeFormatType.ShortTime)
            {
                retVal = datetime.ToShortTimeString();
            }
            return retVal;
        }

        public static bool IsSince(string val)
        {
            if (!string.IsNullOrEmpty(val))
            {
                val = val.Trim().ToLower();
                if (val.EndsWith("y") || val.EndsWith("m") || val.EndsWith("d") || val.EndsWith("h"))
                {
                    return true;
                }
            }
            return false;
        }

        public static int GetSinceHours(string intWithUnitString)
        {
            var hours = 0;
            if (!string.IsNullOrEmpty(intWithUnitString))
            {
                intWithUnitString = intWithUnitString.Trim().ToLower();
                if (intWithUnitString.EndsWith("y"))
                {
                    hours = 8760 * TranslateUtils.ToInt(intWithUnitString.TrimEnd('y'));
                }
                else if (intWithUnitString.EndsWith("m"))
                {
                    hours = 720 * TranslateUtils.ToInt(intWithUnitString.TrimEnd('m'));
                }
                else if (intWithUnitString.EndsWith("d"))
                {
                    hours = 24 * TranslateUtils.ToInt(intWithUnitString.TrimEnd('d'));
                }
                else if (intWithUnitString.EndsWith("h"))
                {
                    hours = TranslateUtils.ToInt(intWithUnitString.TrimEnd('h'));
                }
                else
                {
                    hours = TranslateUtils.ToInt(intWithUnitString);
                }
            }
            return hours;
        }

	    public static string Format(DateTimeOffset? offset, string formatString)
	    {
	        return offset.HasValue ? Format(offset.Value.DateTime, formatString) : string.Empty;
	    }

        public static string Format(DateTime datetime, string formatString)
        {
            string retVal;
            if (!string.IsNullOrEmpty(formatString))
            {
                retVal = formatString.IndexOf("{0:", StringComparison.Ordinal) != -1 ? string.Format(DateTimeFormatInfo.InvariantInfo, formatString, datetime) : datetime.ToString(formatString, DateTimeFormatInfo.InvariantInfo);
            }
            else
            {
                retVal = GetDateString(datetime);
            }
            return retVal;
        }

		public static DateTime SqlMinValue => new DateTime(1754, 1, 1, 0, 0, 0, 0);

        public static string ParseThisMoment(DateTime dateTime)
        {
            if (dateTime <= SqlMinValue) return string.Empty;

            var now = DateTime.Now;

            if (now.Year == dateTime.Year && now.Month == dateTime.Month)
            {
                if (DateDiff("hour", dateTime, now) <= 10)//如果date和当前时间间隔在10小时内
                {
                    if (DateDiff("hour", dateTime, now) > 0)
                    {
                        return DateDiff("hour", dateTime, now) + "小时前";
                    }

                    if (DateDiff("minute", dateTime, now) > 0)
                    {
                        return DateDiff("minute", dateTime, now) + "分钟前";
                    }

                    if (DateDiff("second", dateTime, now) >= 0)
                    {
                        return DateDiff("second", dateTime, now) + "秒前";
                    }
                    return "刚才";
                }

                if (now.Day - dateTime.Day == 0)
                {
                    return "今天 " + dateTime.ToString("HH") + ":" + dateTime.ToString("mm");
                }

                if (now.Day - dateTime.Day == 1)
                {
                    return "昨天 " + dateTime.ToString("HH") + ":" + dateTime.ToString("mm");
                }

                if (now.Day - dateTime.Day == 2)
                {
                    return "前天 " + dateTime.ToString("HH") + ":" + dateTime.ToString("mm");
                }
            }

            return dateTime.ToString("yyyy-MM-dd HH:mm");
        }

        /// <summary>
        /// 两个时间的差值，可以为秒，小时，天，分钟
        /// </summary>
        /// <returns></returns>
        public static long DateDiff(string interval, DateTime startDate, DateTime endDate)
        {
            long retVal = 0;
            var ts = new TimeSpan(endDate.Ticks - startDate.Ticks);
            if (interval == "second")
            {
                retVal = (long)ts.TotalSeconds;
            }
            else if (interval == "minute")
            {
                retVal = (long) ts.TotalMinutes;
            }
            else if (interval == "hour")
            {
                retVal = (long) ts.TotalHours;
            }
            else if (interval == "day")
            {
                retVal = ts.Days;
            }
            else if (interval == "week")
            {
                retVal = ts.Days / 7;
            }
            else if (interval == "month")
            {
                retVal = ts.Days / 30;
            }
            else if (interval == "quarter")
            {
                retVal = ts.Days / 30 / 3;
            }
            else if (interval == "year")
            {
                retVal = ts.Days / 365;
            }
            return retVal;
        }

	    private static readonly DateTime UnixEpoch = new DateTime(1970, 1, 1);

	    public static long ToUnixTime(DateTimeOffset? offset)
	    {
	        return offset.HasValue ? ToUnixTime(offset.Value.DateTime) : 0;
	    }

        public static long ToUnixTime(DateTime dateTime)
	    {
	        return (dateTime - UnixEpoch).Ticks / TimeSpan.TicksPerMillisecond;
	    }
    }
}
