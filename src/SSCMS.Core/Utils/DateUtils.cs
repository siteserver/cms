using System;
using System.Globalization;
using SSCMS.Configuration;
using SSCMS.Enums;
using SSCMS.Utils;

namespace SSCMS.Core.Utils
{
	public static class DateUtils
	{
        public const string FormatStringDateTime = "yyyy-MM-dd HH:mm:ss";
        public const string FormatStringDateOnly = "yyyy-MM-dd";
        private static readonly DateTime JanFirst1970 = new DateTime(1970, 1, 1);

        public static int GetUnixTimestamp(DateTime dateTime)
        {
            return (int)dateTime.Subtract(JanFirst1970).TotalSeconds;
        }

        public static DateTime GetExpiresAt(TimeSpan expiresAt)
	    {
	        return DateTime.UtcNow.Add(expiresAt);
	    }

        public static string GetRelatedDateTimeString(DateTime datetime)
        {
            string retrieval;
            var interval = DateTime.Now - datetime;
            if (interval.Days > 0)
            {
                if (interval.Days >= 7 && interval.Days < 35)
                {
                    retrieval = $"{interval.Days/7}周";
                }
                else
                {
                    retrieval = $"{interval.Days}天";
                }
            }
            else if (interval.Hours > 0)
            {
                retrieval = $"{interval.Hours}小时";
            }
            else if (interval.Minutes > 0)
            {
                retrieval = $"{interval.Minutes}分钟";
            }
            else if (interval.Seconds > 0)
            {
                retrieval = $"{interval.Seconds}秒";
            }
            else if (interval.Milliseconds > 0)
            {
                retrieval = $"{interval.Milliseconds}毫秒";
            }
            else
            {
                retrieval = "1毫秒";
            }
            return retrieval;
        }

        private static string GetDateAndTimeString(DateTime datetime, DateFormatType dateFormat, TimeFormatType timeFormat)
        {
            return $"{GetDateString(datetime, dateFormat)} {GetTimeString(datetime, timeFormat)}";
        }

        public static string GetDateAndTimeString(DateTime datetime)
        {
            if (datetime == Constants.SqlMinValue || datetime == DateTime.MinValue) return string.Empty;
            return GetDateAndTimeString(datetime, DateFormatType.Day, TimeFormatType.ShortTime);
        }

        public static string GetDateString(DateTime datetime)
        {
            if (datetime == Constants.SqlMinValue || datetime == DateTime.MinValue) return string.Empty;
            return GetDateString(datetime, DateFormatType.Day);
        }

        public static string GetDateAndTimeString(DateTimeOffset? offset)
	    {
	        return offset.HasValue ? GetDateAndTimeString(offset.Value.DateTime) : string.Empty;
	    }

        public static string GetDateString(DateTime datetime, DateFormatType dateFormat)
        {
            var format = string.Empty;
            if (dateFormat == DateFormatType.Year)
            {
                format = "yyyy年MM月";
            }
            else if (dateFormat == DateFormatType.Month)
            {
                format = "MM月dd日";
            }
            else if (dateFormat == DateFormatType.Day)
            {
                format = "yyyy-MM-dd";
            }
            else if (dateFormat == DateFormatType.Chinese)
            {
                format = "yyyy年M月d日";
            }
            return datetime.ToString(format);
        }

		public static string GetTimeString(DateTime datetime)
		{
			return GetTimeString(datetime, TimeFormatType.ShortTime);
		}

        private static string GetTimeString(DateTime datetime, TimeFormatType timeFormat)
        {
            var retVal = string.Empty;
            if (timeFormat == TimeFormatType.LongTime)
            {
                retVal = datetime.ToLongTimeString();
            }
            else if (timeFormat == TimeFormatType.ShortTime)
            {
                retVal = datetime.ToShortTimeString();
            }
            return retVal;
        }

        public static bool IsSince(string val)
        {
            if (!string.IsNullOrEmpty(val))
            {
                val = StringUtils.TrimAndToLower(val);
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
                intWithUnitString = StringUtils.TrimAndToLower(intWithUnitString);
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
            string retrieval;
            if (!string.IsNullOrEmpty(formatString))
            {
                retrieval = formatString.IndexOf("{0:", StringComparison.Ordinal) != -1 ? string.Format(DateTimeFormatInfo.InvariantInfo, formatString, datetime) : datetime.ToString(formatString, DateTimeFormatInfo.InvariantInfo);
            }
            else
            {
                retrieval = GetDateString(datetime);
            }
            return retrieval;
        }

        /// <summary>
        /// 把两个时间差，三天内的时间用今天，昨天，前天表示，后跟时间，无日期
        /// </summary>
        public static string ParseThisMoment(DateTime dateTime, DateTime currentDateTime)
        {
            string result;
            if (currentDateTime.Year == dateTime.Year && currentDateTime.Month == dateTime.Month)//如果date和当前时间年份或者月份不一致，则直接返回"yyyy-MM-dd HH:mm"格式日期
            {
                if (DateDiff("hour", dateTime, currentDateTime) <= 10)//如果date和当前时间间隔在10小时内(曾经是3小时)
                {
                    if (DateDiff("hour", dateTime, currentDateTime) > 0)
                        return DateDiff("hour", dateTime, currentDateTime) + "小时前";

                    if (DateDiff("minute", dateTime, currentDateTime) > 0)
                        return DateDiff("minute", dateTime, currentDateTime) + "分钟前";

                    if (DateDiff("second", dateTime, currentDateTime) >= 0)
                        return DateDiff("second", dateTime, currentDateTime) + "秒前";
                    else
                        return "刚才";//为了解决时间精度不够导致发帖时间问题的兼容
                }
                else
                {
                    switch (currentDateTime.Day - dateTime.Day)
                    {
                        case 0:
                            result = "今天 " + dateTime.ToString("HH") + ":" + dateTime.ToString("mm");
                            break;
                        case 1:
                            result = "昨天 " + dateTime.ToString("HH") + ":" + dateTime.ToString("mm");
                            break;
                        case 2:
                            result = "前天 " + dateTime.ToString("HH") + ":" + dateTime.ToString("mm");
                            break;
                        default:
                            result = dateTime.ToString("yyyy-MM-dd HH:mm");
                            break;
                    }
                }
            }
            else
                result = dateTime.ToString("yyyy-MM-dd HH:mm");
            return result;
        }

        /// <summary>
        /// 两个时间的差值，可以为秒，小时，天，分钟
        /// </summary>
        /// <returns></returns>
        private static long DateDiff(string interval, DateTime startDate, DateTime endDate)
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
    }
}
