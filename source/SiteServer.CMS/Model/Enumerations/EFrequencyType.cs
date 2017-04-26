using System;
using System.Web.UI.WebControls;

namespace SiteServer.CMS.Model.Enumerations
{
    public enum EFrequencyType
	{
        Month,          //每月一次
        Week,	        //每周一次
        Day,			//每天一次
        Hour,			//每小时一次
        Period,	        //每周期一次
        JustInTime,      //实时监控
        OnlyOnce        //只做一次
	}

    public class EFrequencyTypeUtils
	{
		public static string GetValue(EFrequencyType type)
		{
            if (type == EFrequencyType.Month)
			{
                return "Month";
            }
            else if (type == EFrequencyType.Week)
            {
                return "Week";
            }
            else if (type == EFrequencyType.Day)
			{
                return "Day";
			}
            else if (type == EFrequencyType.Hour)
			{
                return "Hour";
            }
            else if (type == EFrequencyType.Period)
            {
                return "Period";
            }
            else if (type == EFrequencyType.JustInTime)
            {
                return "JustInTime";
            }
            else if (type == EFrequencyType.OnlyOnce)
            {
                return "OnlyOnce";
            }
			else
			{
				throw new Exception();
			}
		}

		public static string GetText(EFrequencyType type)
		{
            if (type == EFrequencyType.Month)
            {
                return "每月一次";
            }
            else if (type == EFrequencyType.Week)
            {
                return "每周一次";
            }
            else if (type == EFrequencyType.Day)
			{
                return "每天一次";
			}
            else if (type == EFrequencyType.Hour)
			{
                return "每小时一次";
            }
            else if (type == EFrequencyType.Period)
            {
                return "每周期一次";
            }
            else if (type == EFrequencyType.JustInTime)
            {
                return "实时监控";
            }
            else if (type == EFrequencyType.OnlyOnce)
            {
                return "只执行一次";
            }
			else
			{
				throw new Exception();
			}
		}

		public static EFrequencyType GetEnumType(string typeStr)
		{
            var retval = EFrequencyType.Month;

            if (Equals(EFrequencyType.Month, typeStr))
			{
                retval = EFrequencyType.Month;
            }
            else if (Equals(EFrequencyType.Week, typeStr))
            {
                retval = EFrequencyType.Week;
            }
            else if (Equals(EFrequencyType.Day, typeStr))
			{
                retval = EFrequencyType.Day;
			}
            else if (Equals(EFrequencyType.Hour, typeStr))
			{
                retval = EFrequencyType.Hour;
            }
            else if (Equals(EFrequencyType.Period, typeStr))
            {
                retval = EFrequencyType.Period;
            }
            else if (Equals(EFrequencyType.JustInTime, typeStr))
            {
                retval = EFrequencyType.JustInTime;
            }
            else if (Equals(EFrequencyType.OnlyOnce, typeStr))
            {
                retval = EFrequencyType.OnlyOnce;
            }

			return retval;
		}

		public static bool Equals(EFrequencyType type, string typeStr)
		{
			if (string.IsNullOrEmpty(typeStr)) return false;
			if (string.Equals(GetValue(type).ToLower(), typeStr.ToLower()))
			{
				return true;
			}
			return false;
		}

		public static bool Equals(string typeStr, EFrequencyType type)
		{
			return Equals(type, typeStr);
		}

        public static ListItem GetListItem(EFrequencyType type, bool selected)
        {
            var item = new ListItem(GetText(type), GetValue(type));
            if (selected)
            {
                item.Selected = true;
            }
            return item;
        }

        public static void AddListItems(ListControl listControl, bool withJustInTime)
        {
            if (listControl != null)
            {
                listControl.Items.Add(GetListItem(EFrequencyType.Month, false));
                listControl.Items.Add(GetListItem(EFrequencyType.Week, false));
                listControl.Items.Add(GetListItem(EFrequencyType.Day, false));
                listControl.Items.Add(GetListItem(EFrequencyType.Hour, false));
                listControl.Items.Add(GetListItem(EFrequencyType.Period, false));
                //listControl.Items.Add(GetListItem(EFrequencyType.OnlyOnce, false));
                if (withJustInTime)
                {
                    listControl.Items.Add(GetListItem(EFrequencyType.JustInTime, false));
                }
            }
        }

	}
}
