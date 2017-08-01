using System;
using System.Web.UI.WebControls;

namespace BaiRong.Core.Model.Enumerations
{
    /// <summary>
    /// 推送类型
    /// </summary>
    public enum ESubscribePushType
    {
        ManualPush,
        TimedPush 
    }

    public class ESubscribePushTypeUtils
    {
        public static string GetValue(ESubscribePushType type)
        {
            if (type == ESubscribePushType.ManualPush)
            {
                return "ManualPush";
            }
            else if (type == ESubscribePushType.TimedPush)
            {
                return "TimedPush";
            } 
            else
            {
                throw new Exception();
            }
        }

        public static string GetText(ESubscribePushType type)
        {
            if (type == ESubscribePushType.ManualPush)
            {
                return "手动推送";
            }
            else if (type == ESubscribePushType.TimedPush)
            {
                return "定时推送";
            } 
            else
            {
                throw new Exception();
            }
        }

        public static ESubscribePushType GetEnumType(string typeStr)
        {
            var retval = ESubscribePushType.ManualPush;

            if (Equals(ESubscribePushType.ManualPush, typeStr))
            {
                retval = ESubscribePushType.ManualPush;
            }
            else if (Equals(ESubscribePushType.TimedPush, typeStr))
            {
                retval = ESubscribePushType.TimedPush;
            } 

            return retval;
        }

        public static bool Equals(ESubscribePushType type, string typeStr)
        {
            if (string.IsNullOrEmpty(typeStr)) return false;
            if (string.Equals(GetValue(type).ToLower(), typeStr.ToLower()))
            {
                return true;
            }
            return false;
        }

        public static bool Equals(string typeStr, ESubscribePushType type)
        {
            return Equals(type, typeStr);
        }

        public static ListItem GetListItem(ESubscribePushType type, bool selected)
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
                listControl.Items.Add(GetListItem(ESubscribePushType.ManualPush, false));
                listControl.Items.Add(GetListItem(ESubscribePushType.TimedPush, false)); 
            }
        }
         
    }
}
