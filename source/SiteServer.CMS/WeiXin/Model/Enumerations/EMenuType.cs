using System;
using System.Web.UI.WebControls;

namespace SiteServer.CMS.WeiXin.Model.Enumerations
{
	public enum EMenuType
	{
        None,
        Site,
        Keyword,
        Url
	}

    public class EMenuTypeUtils
	{
        public static string GetValue(EMenuType type)
		{
            if (type == EMenuType.None)
            {
                return "None";
            }
            else if (type == EMenuType.Site)
            {
                return "Site";
            }
            else if (type == EMenuType.Keyword)
            {
                return "Keyword";
            }
            else if (type == EMenuType.Url)
            {
                return "Url";
            }
			else
			{
				throw new Exception();
			}
		}

		public static string GetText(EMenuType type)
		{
            if (type == EMenuType.None)
            {
                return "无触发";
            }
            else if (type == EMenuType.Site)
            {
                return "微网站页面";
            }
            else if (type == EMenuType.Keyword)
            {
                return "关键词";
            }
            else if (type == EMenuType.Url)
            {
                return "指定网址";
            }
			else
			{
				throw new Exception();
			}
		}

		public static EMenuType GetEnumType(string typeStr)
		{
            var retval = EMenuType.None;

            if (Equals(EMenuType.None, typeStr))
            {
                retval = EMenuType.None;
            }
            else if (Equals(EMenuType.Site, typeStr))
            {
                retval = EMenuType.Site;
            }
            else if (Equals(EMenuType.Keyword, typeStr))
            {
                retval = EMenuType.Keyword;
            }
            else if (Equals(EMenuType.Url, typeStr))
            {
                retval = EMenuType.Url;
            }

			return retval;
		}

		public static bool Equals(EMenuType type, string typeStr)
		{
			if (string.IsNullOrEmpty(typeStr)) return false;
			if (string.Equals(GetValue(type).ToLower(), typeStr.ToLower()))
			{
				return true;
			}
			return false;
		}

        public static bool Equals(string typeStr, EMenuType type)
        {
            return Equals(type, typeStr);
        }

        public static ListItem GetListItem(EMenuType type, bool selected)
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
                listControl.Items.Add(GetListItem(EMenuType.None, false));
                listControl.Items.Add(GetListItem(EMenuType.Site, false));
                listControl.Items.Add(GetListItem(EMenuType.Keyword, false));
                listControl.Items.Add(GetListItem(EMenuType.Url, false));
            }
        }
	}
}
