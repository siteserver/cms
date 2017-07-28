using System;
using System.Web.UI.WebControls;
using BaiRong.Core;
using System.Collections.Generic;

namespace Siteserver.Core.Model
{
    public enum SiteserverEThirdLoginType
    {
        QQ,
        Weibo,
        WeixinPC,
        WeixinMob,
    }

    public class SiteserverEThirdLoginTypeUtils
    {
        public static string GetValue(SiteserverEThirdLoginType type)
        {
            if (type == SiteserverEThirdLoginType.Weibo)
            {
                return "Weibo";
            }
            else if (type == SiteserverEThirdLoginType.QQ)
            {
                return "QQ";
            }
            else if (type == SiteserverEThirdLoginType.WeixinPC)
            {
                return "WeixinPC";
            }
            else if (type == SiteserverEThirdLoginType.WeixinMob)
            {
                return "WeixinMob";
            }
            else
            {
                throw new Exception();
            }
        }

        public static string GetText(SiteserverEThirdLoginType type)
        {
            if (type == SiteserverEThirdLoginType.Weibo)
            {
                return "新浪微博";
            }
            else if (type == SiteserverEThirdLoginType.QQ)
            {
                return "QQ账号";
            }
            else if (type == SiteserverEThirdLoginType.WeixinPC)
            {
                return "微信账号";
            }
            else if (type == SiteserverEThirdLoginType.WeixinMob)
            {
                return "微信账号";
            }
            else
            {
                throw new Exception();
            }
        }

        public static string GetDescription(SiteserverEThirdLoginType type)
        {
            if (type == SiteserverEThirdLoginType.Weibo)
            {
                return "新浪微博";
            }
            else if (type == SiteserverEThirdLoginType.QQ)
            {
                return "QQ账号";
            }
            else if (type == SiteserverEThirdLoginType.WeixinPC)
            {
                return "微信账号";
            }
            else if (type == SiteserverEThirdLoginType.WeixinMob)
            {
                return "微信账号";
            }
            else
            {
                throw new Exception();
            }
        }

        public static SiteserverEThirdLoginType GetEnumType(string typeStr)
        {
            SiteserverEThirdLoginType retval = SiteserverEThirdLoginType.Weibo;

            if (Equals(SiteserverEThirdLoginType.Weibo, typeStr))
            {
                retval = SiteserverEThirdLoginType.Weibo;
            }
            else if (Equals(SiteserverEThirdLoginType.QQ, typeStr))
            {
                retval = SiteserverEThirdLoginType.QQ;
            }
            else if (Equals(SiteserverEThirdLoginType.WeixinPC, typeStr))
            {
                retval = SiteserverEThirdLoginType.WeixinPC;
            }
            else if (Equals(SiteserverEThirdLoginType.WeixinMob, typeStr))
            {
                retval = SiteserverEThirdLoginType.WeixinMob;
            }
            return retval;
        }

        public static bool Equals(SiteserverEThirdLoginType type, string typeStr)
        {
            if (string.IsNullOrEmpty(typeStr)) return false;
            if (string.Equals(GetValue(type).ToLower(), typeStr.ToLower()))
            {
                return true;
            }
            return false;
        }

        public static bool Equals(string typeStr, SiteserverEThirdLoginType type)
        {
            return Equals(type, typeStr);
        }

        public static List<SiteserverEThirdLoginType> GetSiteserverEThirdLoginTypeList()
        {
            List<SiteserverEThirdLoginType> list = new List<SiteserverEThirdLoginType>();
            list.Add(SiteserverEThirdLoginType.QQ);
            list.Add(SiteserverEThirdLoginType.Weibo);
            list.Add(SiteserverEThirdLoginType.WeixinPC);
            return list;
        }

        public static ListItem GetListItem(SiteserverEThirdLoginType type, bool selected)
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
                listControl.Items.Add(GetListItem(SiteserverEThirdLoginType.Weibo, false));
                listControl.Items.Add(GetListItem(SiteserverEThirdLoginType.QQ, false));
                listControl.Items.Add(GetListItem(SiteserverEThirdLoginType.WeixinPC, false));
                listControl.Items.Add(GetListItem(SiteserverEThirdLoginType.WeixinMob, false));
            }
        }
    }
}
