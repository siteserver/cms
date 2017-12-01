using System;

namespace SiteServer.CMS.Model.Enumerations
{
	public enum ETrackerStyle
	{
		Number,					
		Style1,
		Style2,
		Style3,
		Style4,
		Style5,
		Style6,
		Style7,
		Style8,
		Style9
	}

	public class ETrackerStyleUtils
	{
		public static string GetValue(ETrackerStyle type)
		{
		    if (type == ETrackerStyle.Number)
			{
				return "Number";
			}
		    if (type == ETrackerStyle.Style1)
		    {
		        return "Style1";
		    }
		    if (type == ETrackerStyle.Style2)
		    {
		        return "Style2";
		    }
		    if (type == ETrackerStyle.Style3)
		    {
		        return "Style3";
		    }
		    if (type == ETrackerStyle.Style4)
		    {
		        return "Style4";
		    }
		    if (type == ETrackerStyle.Style5)
		    {
		        return "Style5";
		    }
		    if (type == ETrackerStyle.Style6)
		    {
		        return "Style6";
		    }
		    if (type == ETrackerStyle.Style7)
		    {
		        return "Style7";
		    }
		    if (type == ETrackerStyle.Style8)
		    {
		        return "Style8";
		    }
		    if (type == ETrackerStyle.Style9)
		    {
		        return "Style9";
		    }
		    throw new Exception();
		}

        public static string GetText(ETrackerStyle type)
        {
            if (type == ETrackerStyle.Number)
            {
                return "显示数字";
            }
            if (type == ETrackerStyle.Style1)
            {
                return "样式1";
            }
            if (type == ETrackerStyle.Style2)
            {
                return "样式2";
            }
            if (type == ETrackerStyle.Style3)
            {
                return "样式3";
            }
            if (type == ETrackerStyle.Style4)
            {
                return "样式4";
            }
            if (type == ETrackerStyle.Style5)
            {
                return "样式5";
            }
            if (type == ETrackerStyle.Style6)
            {
                return "样式6";
            }
            if (type == ETrackerStyle.Style7)
            {
                return "样式7";
            }
            if (type == ETrackerStyle.Style8)
            {
                return "样式8";
            }
            if (type == ETrackerStyle.Style9)
            {
                return "样式9";
            }
            throw new Exception();
        }

		public static ETrackerStyle GetEnumType(string typeStr)
		{
			var retval = ETrackerStyle.Number;

			if (Equals(ETrackerStyle.Number, typeStr))
			{
				retval = ETrackerStyle.Number;
			}
			else if (Equals(ETrackerStyle.Style1, typeStr))
			{
				retval = ETrackerStyle.Style1;
			}
			else if (Equals(ETrackerStyle.Style2, typeStr))
			{
				retval = ETrackerStyle.Style2;
			}
			else if (Equals(ETrackerStyle.Style3, typeStr))
			{
				retval = ETrackerStyle.Style3;
			}
			else if (Equals(ETrackerStyle.Style4, typeStr))
			{
				retval = ETrackerStyle.Style4;
			}
			else if (Equals(ETrackerStyle.Style5, typeStr))
			{
				retval = ETrackerStyle.Style5;
			}
			else if (Equals(ETrackerStyle.Style6, typeStr))
			{
				retval = ETrackerStyle.Style6;
			}
			else if (Equals(ETrackerStyle.Style7, typeStr))
			{
				retval = ETrackerStyle.Style7;
			}
			else if (Equals(ETrackerStyle.Style8, typeStr))
			{
				retval = ETrackerStyle.Style8;
			}
			else if (Equals(ETrackerStyle.Style9, typeStr))
			{
				retval = ETrackerStyle.Style9;
			}

			return retval;
		}

		public static bool Equals(ETrackerStyle type, string typeStr)
		{
			if (string.IsNullOrEmpty(typeStr)) return false;
			if (string.Equals(GetValue(type).ToLower(), typeStr.ToLower()))
			{
				return true;
			}
			return false;
		}

        public static bool Equals(string typeStr, ETrackerStyle type)
        {
            return Equals(type, typeStr);
        }
	}
}
