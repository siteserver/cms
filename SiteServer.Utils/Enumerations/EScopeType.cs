using System;
using System.Web.UI.WebControls;

namespace SiteServer.Utils.Enumerations
{
	public enum EScopeType
	{
		Self,					//本级栏目
		Children,				//子栏目
		SelfAndChildren,		//本级栏目及子栏目
		Descendant,				//所有子栏目
		All		                //全部
	}

	public class EScopeTypeUtils
	{
		public static string GetValue(EScopeType type)
		{
		    if (type == EScopeType.Self)
			{
				return "Self";
			}
		    if (type == EScopeType.Children)
		    {
		        return "Children";
		    }
		    if (type == EScopeType.SelfAndChildren)
		    {
		        return "SelfAndChildren";
		    }
		    if (type == EScopeType.Descendant)
		    {
		        return "Descendant";
		    }
		    if (type == EScopeType.All)
		    {
		        return "All";
		    }
		    throw new Exception();
		}

		public static string GetText(EScopeType type)
		{
		    if (type == EScopeType.Children)
			{
				return "子栏目";
			}
		    if (type == EScopeType.Descendant)
		    {
		        return "所有子栏目";
		    }
		    if (type == EScopeType.Self)
		    {
		        return "本级栏目";
		    }
		    if (type == EScopeType.SelfAndChildren)
		    {
		        return "本级栏目及子栏目";
		    }
		    if (type == EScopeType.All)
		    {
		        return "全部";
		    }
		    throw new Exception();
		}

		public static EScopeType GetEnumType(string typeStr)
		{
			var retVal = EScopeType.Self;

			if (Equals(EScopeType.Children, typeStr))
			{
				retVal = EScopeType.Children;
			}
			else if (Equals(EScopeType.Descendant, typeStr))
			{
				retVal = EScopeType.Descendant;
			}
			else if (Equals(EScopeType.Self, typeStr))
			{
				retVal = EScopeType.Self;
			}
			else if (Equals(EScopeType.SelfAndChildren, typeStr))
			{
				retVal = EScopeType.SelfAndChildren;
			}
			else if (Equals(EScopeType.All, typeStr))
			{
                retVal = EScopeType.All;
			}

			return retVal;
		}

		public static bool Equals(EScopeType type, string typeStr)
		{
			if (string.IsNullOrEmpty(typeStr)) return false;
			if (string.Equals(GetValue(type).ToLower(), typeStr.ToLower()))
			{
				return true;
			}
			return false;
		}

        public static bool Equals(string typeStr, EScopeType type)
        {
            return Equals(type, typeStr);
        }

		public static ListItem GetListItem(EScopeType type, bool selected)
		{
            var item = new ListItem(GetValue(type) + " (" + GetText(type) + ")", GetValue(type));
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
				listControl.Items.Add(GetListItem(EScopeType.Self, false));
				listControl.Items.Add(GetListItem(EScopeType.Children, false));
				listControl.Items.Add(GetListItem(EScopeType.SelfAndChildren, false));
				listControl.Items.Add(GetListItem(EScopeType.Descendant, false));
                listControl.Items.Add(GetListItem(EScopeType.All, false));
			}
		}


//		public static EScopeType GetEnumTypeForChannel(string typeStr)
//		{
//			EScopeType type = EScopeType.Children;
//			if (typeStr != null && EScopeTypeUtils.GetValue(EScopeType.Descendant).ToUpper().Equals(typeStr.Trim().ToUpper()))
//			{
//				type = EScopeType.Descendant;
//			}
//			return type;
//		}

	}
}
