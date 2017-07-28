using System;
using System.Web.UI.WebControls;

namespace SiteServer.CMS.Model.Enumerations
{

    public enum EAdvertisementType
    {
        FloatImage,				//漂浮广告
        ScreenDown,             //全屏下推
        OpenWindow,             //弹出窗口
    }

    public class EAdvertisementTypeUtils
    {
        public static string GetValue(EAdvertisementType type)
        {
            if (type == EAdvertisementType.FloatImage)
            {
                return "FloatImage";
            }
            else if (type == EAdvertisementType.ScreenDown)
            {
                return "ScreenDown";
            }
            else if (type == EAdvertisementType.OpenWindow)
            {
                return "OpenWindow";
            }
            else
            {
                throw new Exception();
            }
        }

        public static string GetText(EAdvertisementType type)
        {
            if (type == EAdvertisementType.FloatImage)
            {
                return "漂浮广告";
            }
            else if (type == EAdvertisementType.ScreenDown)
            {
                return "全屏下推";
            }
            else if (type == EAdvertisementType.OpenWindow)
            {
                return "弹出窗口";
            }
            else
            {
                throw new Exception();
            }
        }

        public static EAdvertisementType GetEnumType(string typeStr)
        {
            var retval = EAdvertisementType.FloatImage;

            if (Equals(EAdvertisementType.FloatImage, typeStr))
            {
                retval = EAdvertisementType.FloatImage;
            }
            else if (Equals(EAdvertisementType.ScreenDown, typeStr))
            {
                retval = EAdvertisementType.ScreenDown;
            }
            else if (Equals(EAdvertisementType.OpenWindow, typeStr))
            {
                retval = EAdvertisementType.OpenWindow;
            }

            return retval;
        }

        public static bool Equals(EAdvertisementType type, string typeStr)
        {
            if (string.IsNullOrEmpty(typeStr)) return false;
            if (string.Equals(GetValue(type).ToLower(), typeStr.ToLower()))
            {
                return true;
            }
            return false;
        }

        public static bool Equals(string typeStr, EAdvertisementType type)
        {
            return Equals(type, typeStr);
        }

        public static ListItem GetListItem(EAdvertisementType type, bool selected)
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
                listControl.Items.Add(GetListItem(EAdvertisementType.FloatImage, false));
                listControl.Items.Add(GetListItem(EAdvertisementType.ScreenDown, false));
                listControl.Items.Add(GetListItem(EAdvertisementType.OpenWindow, false));
            }
        }
    }
}
