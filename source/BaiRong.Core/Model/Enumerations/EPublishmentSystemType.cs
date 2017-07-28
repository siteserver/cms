using System.Collections.Generic;
using System.Web.UI.WebControls;

namespace BaiRong.Core.Model.Enumerations
{
	public enum EPublishmentSystemType
	{
        CMS,
        WCM
    }

	public class EPublishmentSystemTypeUtils
	{
		public static string GetValue(EPublishmentSystemType type)
		{
            if (type == EPublishmentSystemType.WCM)
			{
                return "WCM";
            }
            else
			{
                return "CMS";
            }
		}

        public static string GetText(EPublishmentSystemType type)
        {
            return type == EPublishmentSystemType.WCM ? "电子政务站点" : "通用站点";
        }

        public static string GetAppName(EPublishmentSystemType type)
        {
            return type == EPublishmentSystemType.WCM ? "SiteServer WCM" : "SiteServer CMS";
        }

        public static string GetIconHtml(EPublishmentSystemType type)
        {
            return GetIconHtml(type, "icon-large");
        }

        public static string GetIconHtml(EPublishmentSystemType type, string iconClass)
        {
            return string.Format(type == EPublishmentSystemType.WCM ? @"<i class=""icon-sitemap {0}""></i>" : @"<i class=""icon-globe {0}""></i>", iconClass);
        }

        public static string GetHtml(EPublishmentSystemType type)
        {
            return $"{GetIconHtml(type)}&nbsp;{GetText(type)}";
        }

		public static EPublishmentSystemType GetEnumType(string typeStr)
		{
			var retval = EPublishmentSystemType.CMS;

            if (Equals(EPublishmentSystemType.CMS, typeStr))
			{
                retval = EPublishmentSystemType.CMS;
            }
            else if (Equals(EPublishmentSystemType.WCM, typeStr))
            {
                retval = EPublishmentSystemType.WCM;
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

        public static bool IsNodeRelated(EPublishmentSystemType type)
        {
            if (type == EPublishmentSystemType.CMS || type == EPublishmentSystemType.WCM)
            {
                return true;
            }
            return false;
        }

        public static void AddListItems(ListControl listControl)
        {
            if (listControl != null)
            {
                listControl.Items.Add(GetListItem(EPublishmentSystemType.CMS, false));
                listControl.Items.Add(GetListItem(EPublishmentSystemType.WCM, false));
            }
        }

        public static List<EPublishmentSystemType> AllList()
        {
            var list = new List<EPublishmentSystemType>();

            list.Add(EPublishmentSystemType.CMS);
            list.Add(EPublishmentSystemType.WCM);

            return list;
        }

        public static string GetAppID(EPublishmentSystemType type)
        {
            if (type == EPublishmentSystemType.CMS)
            {
                return AppManager.Cms.AppId;
            }
            else if (type == EPublishmentSystemType.WCM)
            {
                return AppManager.Wcm.AppId;
            }

            return string.Empty;
        }
	}
}
