using System;
using System.Web.UI.WebControls;

namespace SiteServer.CMS.Model.Enumerations
{
	
	public enum ELayout
	{
		Table,					//表格
		Flow,					//流
		None,					//无布局
	}

	public class ELayoutUtils
	{
		public static string GetValue(ELayout type)
		{
		    if (type == ELayout.Table)
			{
				return "Table";
			}
		    if (type == ELayout.Flow)
		    {
		        return "Flow";
		    }
		    if (type == ELayout.None)
		    {
		        return "None";
		    }
		    throw new Exception();
		}

		public static string GetText(ELayout type)
		{
		    if (type == ELayout.Table)
			{
				return "表格";
			}
		    if (type == ELayout.Flow)
		    {
		        return "流";
		    }
		    if (type == ELayout.None)
		    {
		        return "无布局";
		    }
		    throw new Exception();
		}

		public static ELayout GetEnumType(string typeStr)
		{
            var retval = ELayout.None;

            if (Equals(ELayout.Table, typeStr))
			{
                retval = ELayout.Table;
			}
			else if (Equals(ELayout.Flow, typeStr))
			{
				retval = ELayout.Flow;
			}
			else if (Equals(ELayout.None, typeStr))
			{
				retval = ELayout.None;
			}

			return retval;
		}

		public static bool Equals(ELayout type, string typeStr)
		{
			if (string.IsNullOrEmpty(typeStr)) return false;
			if (string.Equals(GetValue(type).ToLower(), typeStr.ToLower()))
			{
				return true;
			}
			return false;
		}

        public static bool Equals(string typeStr, ELayout type)
        {
            return Equals(type, typeStr);
        }

		public static ListItem GetListItem(ELayout type, bool selected)
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
                listControl.Items.Add(GetListItem(ELayout.Table, false));
				listControl.Items.Add(GetListItem(ELayout.Flow, false));
				listControl.Items.Add(GetListItem(ELayout.None, false));
			}
		}

	}
}
