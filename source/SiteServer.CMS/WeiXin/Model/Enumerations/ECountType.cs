using System;
using System.Web.UI.WebControls;

namespace SiteServer.CMS.WeiXin.Model.Enumerations
{
	public enum ECountType
	{
        UserSubscribe,
        UserUnsubscribe,
        RequestText,
        RequestNews,

	}

    public class ECountTypeUtils
	{
        public static string GetValue(ECountType type)
		{
            if (type == ECountType.UserSubscribe)
            {
                return "UserSubscribe";
            }
            else if (type == ECountType.UserUnsubscribe)
            {
                return "UserUnsubscribe";
            }
            else if (type == ECountType.RequestText)
            {
                return "RequestText";
            }
            else if (type == ECountType.RequestNews)
            {
                return "RequestNews";
            }
			else
			{
				throw new Exception();
			}
		}

		public static string GetText(ECountType type)
		{
            if (type == ECountType.UserSubscribe)
            {
                return "�û���ע";
            }
            else if (type == ECountType.UserUnsubscribe)
            {
                return "�û�ȡ����ע";
            }
            else if (type == ECountType.RequestText)
            {
                return "�ı��ظ�";
            }
            else if (type == ECountType.RequestNews)
            {
                return "ͼ�Ļظ�";
            }
			else
			{
				throw new Exception();
			}
		}

		public static ECountType GetEnumType(string typeStr)
		{
            var retval = ECountType.UserSubscribe;

            if (Equals(ECountType.UserSubscribe, typeStr))
            {
                retval = ECountType.UserSubscribe;
            }
            else if (Equals(ECountType.UserUnsubscribe, typeStr))
            {
                retval = ECountType.UserUnsubscribe;
            }
            else if (Equals(ECountType.RequestText, typeStr))
            {
                retval = ECountType.RequestText;
            }
            else if (Equals(ECountType.RequestNews, typeStr))
            {
                retval = ECountType.RequestNews;
            }

			return retval;
		}

		public static bool Equals(ECountType type, string typeStr)
		{
			if (string.IsNullOrEmpty(typeStr)) return false;
			if (string.Equals(GetValue(type).ToLower(), typeStr.ToLower()))
			{
				return true;
			}
			return false;
		}

        public static bool Equals(string typeStr, ECountType type)
        {
            return Equals(type, typeStr);
        }

        public static ListItem GetListItem(ECountType type, bool selected)
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
                listControl.Items.Add(GetListItem(ECountType.UserSubscribe, false));
                listControl.Items.Add(GetListItem(ECountType.UserUnsubscribe, false));
                listControl.Items.Add(GetListItem(ECountType.RequestText, false));
                listControl.Items.Add(GetListItem(ECountType.RequestNews, false));
            }
        }
	}
}
