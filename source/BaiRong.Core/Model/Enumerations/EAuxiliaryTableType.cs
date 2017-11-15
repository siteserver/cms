using System;
using System.Web.UI.WebControls;

namespace BaiRong.Core.Model.Enumerations
{
	public enum EAuxiliaryTableType
	{
        BackgroundContent,	    //内容
        Custom                  //自定义
    }

	public class EAuxiliaryTableTypeUtils
	{
		public static string GetValue(EAuxiliaryTableType type)
		{
		    if (type == EAuxiliaryTableType.BackgroundContent)
			{
				return "BackgroundContent";
            }
		    if (type == EAuxiliaryTableType.Custom)
		    {
		        return "Custom";
		    }
		    throw new Exception();
		}

		public static string GetText(EAuxiliaryTableType type)
		{
		    if (type == EAuxiliaryTableType.BackgroundContent)
			{
                return "内容";
            }
		    if (type == EAuxiliaryTableType.Custom)
		    {
		        return "自定义";
		    }
		    throw new Exception();
		}

		public static EAuxiliaryTableType GetEnumType(string typeStr)
		{
            var retval = EAuxiliaryTableType.BackgroundContent;

			if (Equals(EAuxiliaryTableType.BackgroundContent, typeStr))
			{
				retval = EAuxiliaryTableType.BackgroundContent;
            }
            else if (Equals(EAuxiliaryTableType.Custom, typeStr))
            {
                retval = EAuxiliaryTableType.Custom;
            }

			return retval;
		}

		public static bool Equals(EAuxiliaryTableType type, string typeStr)
		{
			if (string.IsNullOrEmpty(typeStr)) return false;
			if (string.Equals(GetValue(type).ToLower(), typeStr.ToLower()))
			{
				return true;
			}
			return false;
		}

        public static bool Equals(string typeStr, EAuxiliaryTableType type)
        {
            return Equals(type, typeStr);
        }

		public static ListItem GetListItem(EAuxiliaryTableType type, bool selected)
		{
			var item = new ListItem(GetText(type), GetValue(type));
			if (selected)
			{
				item.Selected = true;
			}
			return item;
		}

        public static ETableStyle GetTableStyle(EAuxiliaryTableType tableType)
        {
            if (tableType == EAuxiliaryTableType.Custom)
            {
                return ETableStyle.Custom;
            }
            return ETableStyle.BackgroundContent;
        }

        public static string GetDefaultTableName(EAuxiliaryTableType tableType)
        {
            if (tableType == EAuxiliaryTableType.Custom)
            {
                return "model_Custom";
            }
            return "model_Content";
        }
	}
}
