using System;
using System.Web.UI.WebControls;

namespace SiteServer.CMS.Model.Enumerations
{
	
	public enum EMenuDisplayType
	{
		UseChildrenNodeToDisplay,	//显示此栏目的子栏目
		UseNodeGroupToDisplay,		//显示属于栏目组中的栏目
		Both						//显示同时满足以上两条件的栏目
	}

	public class EMenuDisplayTypeUtils
	{
		public static string GetValue(EMenuDisplayType type)
		{
			if (type == EMenuDisplayType.UseChildrenNodeToDisplay)
			{
				return "UseChildrenNodeToDisplay";
			}
			else if (type == EMenuDisplayType.UseNodeGroupToDisplay)
			{
				return "UseNodeGroupToDisplay";
			}
			else if (type == EMenuDisplayType.Both)
			{
				return "Both";
			}
			else
			{
				throw new Exception();
			}
		}

		public static string GetText(EMenuDisplayType type)
		{
			if (type == EMenuDisplayType.UseChildrenNodeToDisplay)
			{
				return "显示此栏目的子栏目";
			}
			else if (type == EMenuDisplayType.UseNodeGroupToDisplay)
			{
				return "显示属于栏目组中的栏目";
			}
			else if (type == EMenuDisplayType.Both)
			{
				return "显示同时满足以上两条件的栏目";
			}
			else
			{
				throw new Exception();
			}
		}

		public static EMenuDisplayType GetEnumType(string typeStr)
		{
			var retval = EMenuDisplayType.UseChildrenNodeToDisplay;

			if (Equals(EMenuDisplayType.UseChildrenNodeToDisplay, typeStr))
			{
				retval = EMenuDisplayType.UseChildrenNodeToDisplay;
			}
			else if (Equals(EMenuDisplayType.UseNodeGroupToDisplay, typeStr))
			{
				retval = EMenuDisplayType.UseNodeGroupToDisplay;
			}
			else if (Equals(EMenuDisplayType.Both, typeStr))
			{
				retval = EMenuDisplayType.Both;
			}

			return retval;
		}

		public static bool Equals(EMenuDisplayType type, string typeStr)
		{
			if (string.IsNullOrEmpty(typeStr)) return false;
			if (string.Equals(GetValue(type).ToLower(), typeStr.ToLower()))
			{
				return true;
			}
			return false;
		}

        public static bool Equals(string typeStr, EMenuDisplayType type)
        {
            return Equals(type, typeStr);
        }

		public static ListItem GetListItem(EMenuDisplayType type, bool selected)
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
				listControl.Items.Add(GetListItem(EMenuDisplayType.UseChildrenNodeToDisplay, false));
				listControl.Items.Add(GetListItem(EMenuDisplayType.UseNodeGroupToDisplay, false));
				listControl.Items.Add(GetListItem(EMenuDisplayType.Both, false));
			}
		}

	}
}
