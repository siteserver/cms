using System;
using System.Web.UI.WebControls;

namespace SiteServer.CMS.Model.Enumerations
{
	
	public enum ETableRule
	{
	    Choose,
	    HandWrite,
	    Create
    }

	public class ETableRuleUtils
    {
		public static string GetValue(ETableRule type)
		{
		    if (type == ETableRule.Choose)
			{
				return "Choose";
			}
		    if (type == ETableRule.HandWrite)
		    {
		        return "HandWrite";
		    }
		    if (type == ETableRule.Create)
		    {
		        return "Create";
		    }
		    throw new Exception();
		}

		public static string GetText(ETableRule type)
		{
		    if (type == ETableRule.Choose)
			{
				return "选择内容表";
			}
		    if (type == ETableRule.HandWrite)
		    {
		        return "指定内容表";
		    }
		    if (type == ETableRule.Create)
		    {
		        return "创建新的内容表";
		    }
		    throw new Exception();
		}

		public static ETableRule GetEnumType(string typeStr)
		{
            var retval = ETableRule.Create;

            if (Equals(ETableRule.Choose, typeStr))
			{
                retval = ETableRule.Choose;
			}
			else if (Equals(ETableRule.HandWrite, typeStr))
			{
				retval = ETableRule.HandWrite;
			}
			else if (Equals(ETableRule.Create, typeStr))
			{
				retval = ETableRule.Create;
			}

			return retval;
		}

		public static bool Equals(ETableRule type, string typeStr)
		{
			if (string.IsNullOrEmpty(typeStr)) return false;
			if (string.Equals(GetValue(type).ToLower(), typeStr.ToLower()))
			{
				return true;
			}
			return false;
		}

        public static bool Equals(string typeStr, ETableRule type)
        {
            return Equals(type, typeStr);
        }

		public static ListItem GetListItem(ETableRule type, bool selected)
		{
			var item = new ListItem(GetText(type), GetValue(type));
			if (selected)
			{
				item.Selected = true;
			}
			return item;
		}
	}
}
