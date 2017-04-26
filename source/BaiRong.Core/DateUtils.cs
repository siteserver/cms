using System;
using System.Globalization;
using BaiRong.Core.Model.Enumerations;
using static System.String;

namespace BaiRong.Core
{
	public class DateUtils
	{
        private DateUtils()
		{
		}

        public const string FormatStringDateTime = "yyyy-MM-dd HH:mm";
        public const string FormatStringDateOnly = "yyyy-MM-dd";

        public static string GetRelatedDateTimeString(DateTime datetime)
        {
            string retval;
            var interval = DateTime.Now - datetime;
            if (interval.Days > 0)
            {
                if (interval.Days >= 7 && interval.Days < 35)
                {
                    retval = $"{interval.Days/7}周";
                }
                else
                {
                    retval = $"{interval.Days}天";
                }
            }
            else if (interval.Hours > 0)
            {
                retval = $"{interval.Hours}小时";
            }
            else if (interval.Minutes > 0)
            {
                retval = $"{interval.Minutes}分钟";
            }
            else if (interval.Seconds > 0)
            {
                retval = $"{interval.Seconds}秒";
            }
            else if (interval.Milliseconds > 0)
            {
                retval = $"{interval.Milliseconds}毫秒";
            }
            else
            {
                retval = "1毫秒";
            }
            return retval;
        }

        public static string GetRelatedDateTimeString(DateTime datetime, string postfix)
        {
            return $"{GetRelatedDateTimeString(datetime)}{postfix}";
        }

        public static string GetDateAndTimeString(DateTime datetime, EDateFormatType dateFormat, ETimeFormatType timeFormat)
        {
            return $"{GetDateString(datetime, dateFormat)} {GetTimeString(datetime, timeFormat)}";
        }

        public static string GetDateAndTimeString(DateTime datetime)
        {
            if (datetime == SqlMinValue) return Empty;
            return GetDateAndTimeString(datetime, EDateFormatType.Day, ETimeFormatType.ShortTime);
        }

        public static string GetDateString(DateTime datetime)
        {
            if (datetime == SqlMinValue) return Empty;
            return GetDateString(datetime, EDateFormatType.Day);
        }

        public static string GetDateString(DateTime datetime, EDateFormatType dateFormat)
        {
            var format = Empty;
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

		public static string GetTimeString(DateTime datetime)
		{
			return GetTimeString(datetime, ETimeFormatType.ShortTime);
		}

        public static string GetTimeString(DateTime datetime, ETimeFormatType timeFormat)
        {
            var retval = Empty;
            if (timeFormat == ETimeFormatType.LongTime)
            {
                retval = datetime.ToLongTimeString();
            }
            else if (timeFormat == ETimeFormatType.ShortTime)
            {
                retval = datetime.ToShortTimeString();
            }
            return retval;
        }

		public static int GetSeconds(string intWithUnitString)
		{
			var seconds = 0;
			if (!IsNullOrEmpty(intWithUnitString))
			{
				intWithUnitString = intWithUnitString.Trim().ToLower();
				if (intWithUnitString.EndsWith("h"))
				{
					seconds = 60 * 60 * TranslateUtils.ToInt(intWithUnitString.TrimEnd('h'));
				}
				else if (intWithUnitString.EndsWith("m"))
				{
					seconds = 60 * TranslateUtils.ToInt(intWithUnitString.TrimEnd('m'));
				}
				else if (intWithUnitString.EndsWith("s"))
				{
					seconds = TranslateUtils.ToInt(intWithUnitString.TrimEnd('s'));
				}
				else
				{
					seconds = TranslateUtils.ToInt(intWithUnitString);
				}
			}
			return seconds;
		}

        public static bool IsSince(string val)
        {
            if (!IsNullOrEmpty(val))
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
            if (!IsNullOrEmpty(intWithUnitString))
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

        public static bool IsTheSameDay(DateTime d1, DateTime d2)
        {
            if (d1.Year == d2.Year && d1.Month == d2.Month && d1.Day == d2.Day)
            {
                return true;
            }
            return false;
        }

        public static string Format(DateTime datetime, string formatString)
        {
            string retval;
            if (!IsNullOrEmpty(formatString))
            {
                if (formatString.IndexOf("{0:", StringComparison.Ordinal) != -1)
                {
                    retval = string.Format(DateTimeFormatInfo.InvariantInfo, formatString, datetime);
                }
                else
                {
                    retval = datetime.ToString(formatString, DateTimeFormatInfo.InvariantInfo);
                }
            }
            else
            {
                retval = GetDateString(datetime);
            }
            return retval;
        }

		public static DateTime SqlMinValue => new DateTime(1754, 1, 1, 0, 0, 0, 0);

	    //Task used
        public static int GetDayOfWeek(DateTime dateTime)
        {
            switch (dateTime.DayOfWeek)
            {
                case DayOfWeek.Monday:
                    return 1;

                case DayOfWeek.Tuesday:
                    return 2;

                case DayOfWeek.Wednesday:
                    return 3;

                case DayOfWeek.Thursday:
                    return 4;

                case DayOfWeek.Friday:
                    return 5;

                case DayOfWeek.Saturday:
                    return 6;

                default:
                    return 7;
            }
        }

        public static string ParseThisMoment(DateTime dateTime)
        {
            if (dateTime != SqlMinValue)
            {
                return ParseThisMoment(dateTime, DateTime.Now);
            }
            return Empty;
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
        public static long DateDiff(string interval, DateTime startDate, DateTime endDate)
        {
            long retval = 0;
            var ts = new TimeSpan(endDate.Ticks - startDate.Ticks);
            if (interval == "second")
            {
                retval = (long)ts.TotalSeconds;
            }
            else if (interval == "minute")
            {
                retval = (long) ts.TotalMinutes;
            }
            else if (interval == "hour")
            {
                retval = (long) ts.TotalHours;
            }
            else if (interval == "day")
            {
                retval = ts.Days;
            }
            else if (interval == "week")
            {
                retval = ts.Days / 7;
            }
            else if (interval == "month")
            {
                retval = ts.Days / 30;
            }
            else if (interval == "quarter")
            {
                retval = ts.Days / 30 / 3;
            }
            else if (interval == "year")
            {
                retval = ts.Days / 365;
            }
            return retval;
        }
	}
}
