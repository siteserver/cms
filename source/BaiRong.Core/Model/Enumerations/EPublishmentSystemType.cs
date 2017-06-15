using System.Collections.Generic;
using System.Web.UI.WebControls;

namespace BaiRong.Core.Model.Enumerations
{
	public enum EPublishmentSystemType
	{
        Cms,
        Wcm,
        WeiXin
    }

	public class EPublishmentSystemTypeUtils
	{
		public static string GetValue(EPublishmentSystemType type)
		{
		    if (type == EPublishmentSystemType.Wcm)
			{
                return "Wcm";
            }
		    if (type == EPublishmentSystemType.WeiXin)
		    {
		        return "WeiXin";
		    }
		    return "Cms";
		}

        public static string GetText(EPublishmentSystemType type)
        {
            if (type == EPublishmentSystemType.Wcm)
            {
                return "电子政务站点";
            }
            if (type == EPublishmentSystemType.WeiXin)
            {
                return "微信站点";
            }
            return "站点";
        }

        public static string GetIconHtml(EPublishmentSystemType type)
        {
            return GetIconHtml(type, "icon-large");
        }

        public static string GetIconHtml(EPublishmentSystemType type, string iconClass)
        {
            string html = string.Empty;

            if (type == EPublishmentSystemType.Cms)
            {
                html = @"<i class=""icon-globe {0}""></i>";
            }
            else if (type == EPublishmentSystemType.Wcm)
            {
                html = @"<i class=""icon-sitemap {0}""></i>";
            }
            else if (type == EPublishmentSystemType.WeiXin)
            {
                html = @"<i class=""icon-qrcode {0}""></i>";
            }

            return string.Format(html, iconClass);
        }

        public static string GetHtml(EPublishmentSystemType type)
        {
            return $"{GetIconHtml(type)}&nbsp;{GetText(type)}";
        }

		public static EPublishmentSystemType GetEnumType(string typeStr)
		{
			var retval = EPublishmentSystemType.Cms;

            if (Equals(EPublishmentSystemType.Wcm, typeStr))
            {
                retval = EPublishmentSystemType.Wcm;
            }
            else if (Equals(EPublishmentSystemType.WeiXin, typeStr))
            {
                retval = EPublishmentSystemType.WeiXin;
            }

            return retval;
		}

		public static bool Equals(EPublishmentSystemType type, string typeStr)
		{
			if (string.IsNullOrEmpty(typeStr)) return false;
			if (string.Equals(GetValue(type).ToLower(), typeStr.ToLower()))
			{
				return true;
			}
			return false;
		}

		public static bool Equals(string typeStr, EPublishmentSystemType type)
		{
			return Equals(type, typeStr);
		}

        public static ListItem GetListItem(EPublishmentSystemType type, bool selected)
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
                listControl.Items.Add(GetListItem(EPublishmentSystemType.Cms, false));
                listControl.Items.Add(GetListItem(EPublishmentSystemType.Wcm, false));
                listControl.Items.Add(GetListItem(EPublishmentSystemType.WeiXin, false));
            }
        }

        public static List<EPublishmentSystemType> AllList()
        {
            var list = new List<EPublishmentSystemType>
            {
                EPublishmentSystemType.Cms,
                EPublishmentSystemType.Wcm,
                EPublishmentSystemType.WeiXin
            };

            return list;
        }

        public static string GetAppId(EPublishmentSystemType type)
        {
            if (type == EPublishmentSystemType.Wcm)
            {
                return AppManager.Wcm.AppId;
            }
            else if (type == EPublishmentSystemType.WeiXin)
            {
                return AppManager.WeiXin.AppId;
            }

            return AppManager.Cms.AppId;
        }
	}
}
