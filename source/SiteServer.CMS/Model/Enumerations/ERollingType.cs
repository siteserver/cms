using System;
using System.Web.UI.WebControls;

namespace SiteServer.CMS.Model.Enumerations
{
	/// <summary>
	/// FloatDivInfo 滚动方式
	/// </summary>
	public enum ERollingType
	{
		Static,							//静止不动
		FollowingScreen,				//跟随窗体滚动
		FloatingInWindow				//在窗体中不断移动
	}

	public class ERollingTypeUtils
	{
		public static string GetValue(ERollingType type)
		{
			if (type == ERollingType.Static)
			{
				return "Static";
			}
			else if (type == ERollingType.FollowingScreen)
			{
				return "FollowingScreen";
			}
			else if (type == ERollingType.FloatingInWindow)
			{
				return "FloatingInWindow";
			}
			else
			{
				throw new Exception();
			}
		}

		public static string GetText(ERollingType type)
		{
			if (type == ERollingType.Static)
			{
				return "静止不动";
			}
			else if (type == ERollingType.FollowingScreen)
			{
				return "跟随窗体滚动";
			}
			else if (type == ERollingType.FloatingInWindow)
			{
				return "在窗体中不断移动";
			}
			else
			{
				throw new Exception();
			}
		}

		public static ERollingType GetEnumType(string typeStr)
		{
			var retval = ERollingType.Static;

			if (Equals(ERollingType.Static, typeStr))
			{
				retval = ERollingType.Static;
			}
			else if (Equals(ERollingType.FollowingScreen, typeStr))
			{
				retval = ERollingType.FollowingScreen;
			}
			else if (Equals(ERollingType.FloatingInWindow, typeStr))
			{
				retval = ERollingType.FloatingInWindow;
			}

			return retval;
		}

		public static bool Equals(ERollingType type, string typeStr)
		{
			if (string.IsNullOrEmpty(typeStr)) return false;
			if (string.Equals(GetValue(type).ToLower(), typeStr.ToLower()))
			{
				return true;
			}
			return false;
		}

        public static bool Equals(string typeStr, ERollingType type)
        {
            return Equals(type, typeStr);
        }

		public static ListItem GetListItem(ERollingType type, bool selected)
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
				listControl.Items.Add(GetListItem(ERollingType.Static, false));
				listControl.Items.Add(GetListItem(ERollingType.FollowingScreen, false));
				listControl.Items.Add(GetListItem(ERollingType.FloatingInWindow, false));
			}
		}

	}
}
