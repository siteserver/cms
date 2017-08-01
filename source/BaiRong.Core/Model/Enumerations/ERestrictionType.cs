using System;
using System.Web.UI.WebControls;

namespace BaiRong.Core.Model.Enumerations
{
	public enum ERestrictionType
	{
        NoRestriction,
        BlackList,
		WhiteList
	}

	public class ERestrictionTypeUtils
	{
		public static string GetValue(ERestrictionType type)
		{
            if (type == ERestrictionType.NoRestriction)
			{
                return "NoRestriction";
			}
            else if (type == ERestrictionType.BlackList)
			{
                return "BlackList";
            }
            else if (type == ERestrictionType.WhiteList)
            {
                return "WhiteList";
            }
			else
			{
				throw new Exception();
			}
		}

		public static string GetText(ERestrictionType type)
		{
            if (type == ERestrictionType.NoRestriction)
			{
                return "无访问限制";
			}
            else if (type == ERestrictionType.BlackList)
			{
                return "启用黑名单，禁止黑名单中的IP进行访问，其余允许访问";
            }
            else if (type == ERestrictionType.WhiteList)
            {
                return "启用白名单，允许白名单中的IP进行访问，其余禁止访问";
            }
			else
			{
				throw new Exception();
			}
		}

        public static string GetShortText(ERestrictionType type)
        {
            if (type == ERestrictionType.NoRestriction)
            {
                return "无";
            }
            else if (type == ERestrictionType.BlackList)
            {
                return "启用黑名单";
            }
            else if (type == ERestrictionType.WhiteList)
            {
                return "启用白名单";
            }
            else
            {
                throw new Exception();
            }
        }

		public static ERestrictionType GetEnumType(string typeStr)
		{
            var retval = ERestrictionType.NoRestriction;

            if (Equals(ERestrictionType.NoRestriction, typeStr))
			{
                retval = ERestrictionType.NoRestriction;
			}
            else if (Equals(ERestrictionType.BlackList, typeStr))
			{
                retval = ERestrictionType.BlackList;
            }
            else if (Equals(ERestrictionType.WhiteList, typeStr))
            {
                retval = ERestrictionType.WhiteList;
            }
			return retval;
		}

		public static bool Equals(ERestrictionType type, string typeStr)
		{
			if (string.IsNullOrEmpty(typeStr)) return false;
			if (string.Equals(GetValue(type).ToLower(), typeStr.ToLower()))
			{
				return true;
			}
			return false;
		}

        public static bool Equals(string typeStr, ERestrictionType type)
        {
            return Equals(type, typeStr);
        }

		public static ListItem GetListItem(ERestrictionType type, bool selected)
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
                listControl.Items.Add(GetListItem(ERestrictionType.NoRestriction, false));
                listControl.Items.Add(GetListItem(ERestrictionType.BlackList, false));
                listControl.Items.Add(GetListItem(ERestrictionType.WhiteList, false));
			}
		}

	}
}
