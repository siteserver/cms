using System;
using System.Collections.Generic;
using System.Web.UI.WebControls;

namespace BaiRong.Core.Model.Enumerations
{
    public enum EThirdLoginType
    {
        QQ,
        Weibo,
        WeixinPC,
        WeixinMob
    }

    public class EThirdLoginTypeUtils
    {
        public static string GetValue(EThirdLoginType type)
        {
            if (type == EThirdLoginType.Weibo)
            {
                return "Weibo";
            }
            else if (type == EThirdLoginType.QQ)
            {
                return "QQ";
            }
            else if (type == EThirdLoginType.WeixinPC)
            {
                return "WeixinPC";
            }
            else if (type == EThirdLoginType.WeixinMob)
            {
                return "WeixinMob";
            }
            else
            {
                throw new Exception();
            }
        }

        public static string GetText(EThirdLoginType type)
        {
            if (type == EThirdLoginType.Weibo)
            {
                return "新浪微博";
            }
            else if (type == EThirdLoginType.QQ)
            {
                return "QQ账号";
            }
            else if (type == EThirdLoginType.WeixinPC)
            {
                return "微信账号";
            }
            else if (type == EThirdLoginType.WeixinMob)
            {
                return "微信账号";
            }
            else
            {
                throw new Exception();
            }
        }

        public static string GetDescription(EThirdLoginType type)
        {
            if (type == EThirdLoginType.Weibo)
            {
                return "新浪微博";
            }
            else if (type == EThirdLoginType.QQ)
            {
                return "QQ账号";
            }
            else if (type == EThirdLoginType.WeixinPC)
            {
                return "微信账号";
            }
            else if (type == EThirdLoginType.WeixinMob)
            {
                return "微信账号";
            }
            else
            {
                throw new Exception();
            }
        }

        public static EThirdLoginType GetEnumType(string typeStr)
        {
            var retval = EThirdLoginType.Weibo;

            if (Equals(EThirdLoginType.Weibo, typeStr))
            {
                retval = EThirdLoginType.Weibo;
            }
            else if (Equals(EThirdLoginType.QQ, typeStr))
            {
                retval = EThirdLoginType.QQ;
            }
            else if (Equals(EThirdLoginType.WeixinPC, typeStr))
            {
                retval = EThirdLoginType.WeixinPC;
            }
            else if (Equals(EThirdLoginType.WeixinMob, typeStr))
            {
                retval = EThirdLoginType.WeixinMob;
            }
            return retval;
        }

        public static bool Equals(EThirdLoginType type, string typeStr)
        {
            if (string.IsNullOrEmpty(typeStr)) return false;
            if (string.Equals(GetValue(type).ToLower(), typeStr.ToLower()))
            {
                return true;
            }
            return false;
        }

        public static bool Equals(string typeStr, EThirdLoginType type)
        {
            return Equals(type, typeStr);
        }

        public static List<EThirdLoginType> GetEThirdLoginTypeList()
        {
            var list = new List<EThirdLoginType>();
            list.Add(EThirdLoginType.QQ);
            list.Add(EThirdLoginType.Weibo);
            list.Add(EThirdLoginType.WeixinPC);
            return list;
        }

        public static ListItem GetListItem(EThirdLoginType type, bool selected)
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
                listControl.Items.Add(GetListItem(EThirdLoginType.Weibo, false));
                listControl.Items.Add(GetListItem(EThirdLoginType.QQ, false));
                listControl.Items.Add(GetListItem(EThirdLoginType.WeixinPC, false));
                listControl.Items.Add(GetListItem(EThirdLoginType.WeixinMob, false));
            }
        }
    }
}
