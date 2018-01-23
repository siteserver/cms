using System;
using System.Web.UI.WebControls;

namespace SiteServer.Utils.Enumerations
{
	
	public enum EVoteRestrictType
	{
		NoRestrict,				//允许重复投票
		RestrictOneDay,			//一天内禁止同一IP重复投票
		RestrictOnlyOnce,		//每台机只能投一票
        RestrictUser    		//每用户只能投一票
	}

	public class EVoteRestrictTypeUtils
	{
		public static string GetValue(EVoteRestrictType type)
		{
		    if (type == EVoteRestrictType.NoRestrict)
			{
				return "NoRestrict";
			}
		    if (type == EVoteRestrictType.RestrictOneDay)
		    {
		        return "RestrictOneDay";
		    }
		    if (type == EVoteRestrictType.RestrictOnlyOnce)
		    {
		        return "RestrictOnlyOnce";
		    }
		    if (type == EVoteRestrictType.RestrictUser)
		    {
		        return "RestrictUser";
		    }
		    throw new Exception();
		}

		public static string GetText(EVoteRestrictType type)
		{
		    if (type == EVoteRestrictType.NoRestrict)
			{
				return "允许重复投票";
			}
		    if (type == EVoteRestrictType.RestrictOneDay)
		    {
		        return "一天内禁止重复投票";
		    }
		    if (type == EVoteRestrictType.RestrictOnlyOnce)
		    {
		        return "每台机只能投一票";
		    }
		    if (type == EVoteRestrictType.RestrictUser)
		    {
		        return "每用户只能投一票";
		    }
		    throw new Exception();
		}

		public static EVoteRestrictType GetEnumType(string typeStr)
		{
			EVoteRestrictType retval = EVoteRestrictType.NoRestrict;

			if (Equals(EVoteRestrictType.NoRestrict, typeStr))
			{
				retval = EVoteRestrictType.NoRestrict;
			}
			else if (Equals(EVoteRestrictType.RestrictOneDay, typeStr))
			{
				retval = EVoteRestrictType.RestrictOneDay;
			}
			else if (Equals(EVoteRestrictType.RestrictOnlyOnce, typeStr))
			{
				retval = EVoteRestrictType.RestrictOnlyOnce;
            }
            else if (Equals(EVoteRestrictType.RestrictUser, typeStr))
            {
                retval = EVoteRestrictType.RestrictUser;
            }

			return retval;
		}

		public static bool Equals(EVoteRestrictType type, string typeStr)
		{
			if (string.IsNullOrEmpty(typeStr)) return false;
			if (string.Equals(GetValue(type).ToLower(), typeStr.ToLower()))
			{
				return true;
			}
			return false;
		}

        public static bool Equals(string typeStr, EVoteRestrictType type)
        {
            return Equals(type, typeStr);
        }

		public static ListItem GetListItem(EVoteRestrictType type, bool selected)
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
				listControl.Items.Add(GetListItem(EVoteRestrictType.NoRestrict, false));
				listControl.Items.Add(GetListItem(EVoteRestrictType.RestrictOneDay, false));
				listControl.Items.Add(GetListItem(EVoteRestrictType.RestrictOnlyOnce, false));
                listControl.Items.Add(GetListItem(EVoteRestrictType.RestrictUser, false));
			}
		}

	}
}
