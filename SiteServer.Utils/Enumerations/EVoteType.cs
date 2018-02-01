using System;
using System.Web.UI.WebControls;

namespace SiteServer.Utils.Enumerations
{
	
	public enum EVoteType
	{
		RadioVote,				//单选投票
		CheckBoxVote			//复选投票
	}

	public class EVoteTypeUtils
	{
		public static string GetValue(EVoteType type)
		{
		    if (type == EVoteType.RadioVote)
			{
				return "RadioVote";
			}
		    if (type == EVoteType.CheckBoxVote)
		    {
		        return "CheckBoxVote";
		    }
		    throw new Exception();
		}

		public static string GetText(EVoteType type)
		{
		    if (type == EVoteType.CheckBoxVote)
			{
				return "复选";
			}
		    if (type == EVoteType.RadioVote)
		    {
		        return "单选";
		    }
		    throw new Exception();
		}

		public static EVoteType GetEnumType(string typeStr)
		{
			EVoteType retval = EVoteType.RadioVote;

			if (Equals(EVoteType.CheckBoxVote, typeStr))
			{
				retval = EVoteType.CheckBoxVote;
			}
			else if (Equals(EVoteType.RadioVote, typeStr))
			{
				retval = EVoteType.RadioVote;
			}

			return retval;
		}

		public static bool Equals(EVoteType type, string typeStr)
		{
			if (string.IsNullOrEmpty(typeStr)) return false;
			if (string.Equals(GetValue(type).ToLower(), typeStr.ToLower()))
			{
				return true;
			}
			return false;
		}

        public static bool Equals(string typeStr, EVoteType type)
        {
            return Equals(type, typeStr);
        }

		public static ListItem GetListItem(EVoteType type, bool selected)
		{
			ListItem item = new ListItem(GetText(type), GetValue(type));
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
				listControl.Items.Add(GetListItem(EVoteType.RadioVote, false));
				listControl.Items.Add(GetListItem(EVoteType.CheckBoxVote, false));
			}
		}

	}
}
