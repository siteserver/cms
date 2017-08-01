using System;
using System.Web.UI.WebControls;

namespace BaiRong.Core.Model.Enumerations
{
	public enum EDataFormat
	{
		String,
		Json,
		Xml
	}

    public class EDataFormatUtils
	{
        public static string GetValue(EDataFormat type)
		{
			if (type == EDataFormat.String)
			{
                return "String";
			}
			else if (type == EDataFormat.Json)
			{
                return "Json";
			}
			else if (type == EDataFormat.Xml)
			{
                return "Xml";
			}
			else
			{
				throw new Exception();
			}
		}

		public static string GetText(EDataFormat type)
		{
			if (type == EDataFormat.String)
			{
				return "默认";
			}
			else if (type == EDataFormat.Json)
			{
				return "Json";
			}
			else if (type == EDataFormat.Xml)
			{
				return "Xml";
			}
			else
			{
				throw new Exception();
			}
		}

		public static EDataFormat GetEnumType(string typeStr)
		{
            var retval = EDataFormat.String;

            if (Equals(EDataFormat.Json, typeStr))
			{
                retval = EDataFormat.Json;
			}
            else if (Equals(EDataFormat.Xml, typeStr))
			{
                retval = EDataFormat.Xml;
			}

			return retval;
		}

		public static bool Equals(EDataFormat type, string typeStr)
		{
			if (string.IsNullOrEmpty(typeStr)) return false;
			if (string.Equals(GetValue(type).ToLower(), typeStr.ToLower()))
			{
				return true;
			}
			return false;
		}

        public static bool Equals(string typeStr, EDataFormat type)
        {
            return Equals(type, typeStr);
        }

		public static ListItem GetListItem(EDataFormat type, bool selected)
		{
			var item = new ListItem(GetText(type), GetValue(type));
			if (selected)
			{
				item.Selected = true;
			}
			return item;
		}

        public static void AddListItems(ListControl listControl)
        {
            if (listControl != null)
            {
                listControl.Items.Add(GetListItem(EDataFormat.String, false));
                listControl.Items.Add(GetListItem(EDataFormat.Json, false));
                listControl.Items.Add(GetListItem(EDataFormat.Xml, false));
            }
        }
	}
}
