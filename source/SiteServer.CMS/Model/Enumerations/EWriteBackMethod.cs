using System;
using System.Web.UI.WebControls;

namespace SiteServer.CMS.Model.Enumerations
{

	public enum EWriteBackMethod
	{
        None,               //不需要回复
        ByWriteBackField,   //使用前台表字段回复
        ByEmail,            //使用电子邮件回复
        All                 //两种回复方式均可
    }

	public class EWriteBackMethodUtils
	{
		public static string GetValue(EWriteBackMethod type)
		{
			if (type == EWriteBackMethod.None)
			{
				return "None";
			}
			else if (type == EWriteBackMethod.ByWriteBackField)
			{
				return "ByWriteBackField";
			}
			else if (type == EWriteBackMethod.ByEmail)
			{
				return "ByEmail";
			}
			else if (type == EWriteBackMethod.All)
			{
				return "All";
			}
			else
			{
				throw new Exception();
			}
		}

		public static string GetText(EWriteBackMethod type)
		{
            if (type == EWriteBackMethod.None)
            {
                return "不回复信息";
            }
            if (type == EWriteBackMethod.ByWriteBackField)
            {
                return "直接回复信息";
            }
            if (type == EWriteBackMethod.ByEmail)
            {
                return "通过邮件回复信息";
            }
            if (type == EWriteBackMethod.All)
            {
                return "同时使用两种回复方式";
            }
            throw new Exception();
        }

		public static EWriteBackMethod GetEnumType(string typeStr)
		{
			var retval = EWriteBackMethod.None;

			if (Equals(EWriteBackMethod.None, typeStr))
			{
				retval = EWriteBackMethod.None;
			}
			else if (Equals(EWriteBackMethod.ByWriteBackField, typeStr))
			{
				retval = EWriteBackMethod.ByWriteBackField;
			}
			else if (Equals(EWriteBackMethod.ByEmail, typeStr))
			{
				retval = EWriteBackMethod.ByEmail;
			}
			else if (Equals(EWriteBackMethod.All, typeStr))
			{
				retval = EWriteBackMethod.All;
			}

			return retval;
		}

		public static bool Equals(EWriteBackMethod type, string typeStr)
		{
			if (string.IsNullOrEmpty(typeStr)) return false;
			if (string.Equals(GetValue(type).ToLower(), typeStr.ToLower()))
			{
				return true;
			}
			return false;
		}

        public static bool Equals(string typeStr, EWriteBackMethod type)
        {
            return Equals(type, typeStr);
        }

		public static ListItem GetListItem(EWriteBackMethod type, bool selected)
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
				listControl.Items.Add(GetListItem(EWriteBackMethod.None, false));
				listControl.Items.Add(GetListItem(EWriteBackMethod.ByWriteBackField, false));
				listControl.Items.Add(GetListItem(EWriteBackMethod.ByEmail, false));
				listControl.Items.Add(GetListItem(EWriteBackMethod.All, false));
			}
		}

	}
}
