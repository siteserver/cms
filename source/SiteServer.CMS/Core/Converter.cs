using System;
using System.Collections;
using System.Collections.Specialized;
using System.Data;
using System.Web.UI.WebControls;
using BaiRong.Core;
using SiteServer.CMS.Model;

namespace SiteServer.CMS.Core
{
	/// <summary>
	/// Convert 的摘要说明。
	/// </summary>
	public sealed class Converter
	{
		private Converter()
		{
		}		

		/// <summary>
		/// 将以2004-1-20或2005-9-3 12:12:32为格式的字符串转换为日期
		/// </summary>
		public static DateTime ToDateTime(string dateTimeString)
		{
			if (string.IsNullOrEmpty(dateTimeString))
			{
				return DateTime.Now;
			}
			var dateTimeStr = dateTimeString.Trim();

			var year = DateTime.Now.Year;
			var month = DateTime.Now.Month;
			var day = DateTime.Now.Day;
			var hour = DateTime.Now.Hour;
			var minute = DateTime.Now.Minute;
			var second = DateTime.Now.Second;

			if (dateTimeStr.IndexOf(" ") != -1)
			{
				var dateAndTimeArray = dateTimeStr.Split(' ');
				if (dateAndTimeArray.Length == 2)
				{
					try
					{
						var arrayDate = dateAndTimeArray[0].Split('-');
						year = int.Parse(arrayDate[0]);
						month = int.Parse(arrayDate[1]);
						day = int.Parse(arrayDate[2]);

						var arrayTime = dateAndTimeArray[1].Split(':');
						hour = int.Parse(arrayTime[0]);
						minute = int.Parse(arrayTime[1]);
						second = int.Parse(arrayTime[2]);
					}
					catch{}
				}
			}
			else
			{
				var array = dateTimeStr.Split('-');
				if (array.Length == 3)
				{
					try
					{
						year = int.Parse(array[0]);
						month = int.Parse(array[1]);
						day = int.Parse(array[2]);
					}
					catch{}
				}
			}

			var dateTime = new DateTime(year, month, day, hour, minute, second);
			return dateTime;
		}


		

		public static HorizontalAlign ToHorizontalAlign(string typeStr)
		{
			return (HorizontalAlign)TranslateUtils.ToEnum(typeof(HorizontalAlign), typeStr, HorizontalAlign.Left);
		}

		public static VerticalAlign ToVerticalAlign(string typeStr)
		{
			return (VerticalAlign)TranslateUtils.ToEnum(typeof(VerticalAlign), typeStr, VerticalAlign.Middle);
		}

		public static GridLines ToGridLines(string typeStr)
		{
			return (GridLines)TranslateUtils.ToEnum(typeof(GridLines), typeStr, GridLines.None);
		}

		public static RepeatDirection ToRepeatDirection(string typeStr)
		{
			return (RepeatDirection)TranslateUtils.ToEnum(typeof(RepeatDirection), typeStr, RepeatDirection.Vertical);
		}

		public static RepeatLayout ToRepeatLayout(string typeStr)
		{
			return (RepeatLayout)TranslateUtils.ToEnum(typeof(RepeatLayout), typeStr, RepeatLayout.Table);
		}


		/// <summary>
		/// 处理数据源，转换成IEnumerable，方便进行下一步操作
		/// </summary>
		/// <param name="source">数据源，先统一装箱</param>
		/// <returns>数据源的IEnumerable形式</returns>
		public static IEnumerable ToIEnumerable(object source)
		{
			if(source is IEnumerable)
				return (IEnumerable)source;
			else if(source is IList)
				return (IEnumerable)source;
			else if(source is DataSet)
				return ((DataSet)source).Tables[0].DefaultView;
			else if (source is DataTable)
				return ((DataTable)source).DefaultView;
			else
				return null;
		}


        public static BackgroundContentInfo ToBackgroundContentInfo(NameValueCollection collection, NameValueCollection columnsMap)
        {
            BackgroundContentInfo contentInfo = null;
            if (collection != null && columnsMap != null)
            {
                contentInfo = new BackgroundContentInfo();
                foreach (string attributeName in collection.Keys)
                {
                    if (columnsMap[attributeName] != null)
                    {
                        var columnsToMatch = columnsMap[attributeName];
                        contentInfo.SetExtendedAttribute(columnsToMatch, collection[attributeName]);
                    }
                }
            }
            return contentInfo;
        }

	}
}
