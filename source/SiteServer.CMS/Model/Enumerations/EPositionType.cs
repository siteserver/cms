using System;
using System.Web.UI.WebControls;

namespace SiteServer.CMS.Model.Enumerations
{
    public enum EPositionType
	{
		LeftTop,				//左上
		LeftBottom,				//左下
		RightTop,               //右上
        RightBottom             //右下
	}

    public class EPositionTypeUtils
	{
		public static string GetValue(EPositionType type)
		{
            if (type == EPositionType.LeftTop)
			{
                return "LeftTop";
			}
            else if (type == EPositionType.LeftBottom)
			{
                return "LeftBottom";
			}
            else if (type == EPositionType.RightTop)
			{
                return "RightTop";
            }
            else if (type == EPositionType.RightBottom)
            {
                return "RightBottom";
            }
			else
			{
				throw new Exception();
			}
		}

		public static string GetText(EPositionType type)
		{
            if (type == EPositionType.LeftTop)
            {
                return "左上";
            }
            else if (type == EPositionType.LeftBottom)
            {
                return "左下";
            }
            else if (type == EPositionType.RightTop)
            {
                return "右上";
            }
            else if (type == EPositionType.RightBottom)
            {
                return "右下";
            }
            else
            {
                throw new Exception();
            }
		}

		public static EPositionType GetEnumType(string typeStr)
		{
			var retval = EPositionType.LeftTop;

            if (Equals(EPositionType.LeftTop, typeStr))
			{
                retval = EPositionType.LeftTop;
			}
			else if (Equals(EPositionType.LeftBottom, typeStr))
			{
                retval = EPositionType.LeftBottom;
			}
			else if (Equals(EPositionType.RightTop, typeStr))
			{
                retval = EPositionType.RightTop;
            }
            else if (Equals(EPositionType.RightBottom, typeStr))
            {
                retval = EPositionType.RightBottom;
            }

			return retval;
		}

		public static bool Equals(EPositionType type, string typeStr)
		{
			if (string.IsNullOrEmpty(typeStr)) return false;
			if (string.Equals(GetValue(type).ToLower(), typeStr.ToLower()))
			{
				return true;
			}
			return false;
		}

        public static bool Equals(string typeStr, EPositionType type)
        {
            return Equals(type, typeStr);
        }

		public static ListItem GetListItem(EPositionType type, bool selected)
		{
			var item = new ListItem(GetText(type), GetValue(type));
			if (selected)
			{
				item.Selected = true;
			}
			return item;
		}

        public static void AddListItems(ListControl listControl, ERollingType rollingType)
		{
			if (listControl != null)
			{
                if (rollingType == ERollingType.Static)
                {
                    listControl.Items.Add(GetListItem(EPositionType.LeftTop, false));
                    listControl.Items.Add(GetListItem(EPositionType.LeftBottom, false));
                    listControl.Items.Add(GetListItem(EPositionType.RightTop, false));
                    listControl.Items.Add(GetListItem(EPositionType.RightBottom, false));
                }
                else if (rollingType == ERollingType.FollowingScreen)
                {
                    listControl.Items.Add(GetListItem(EPositionType.LeftBottom, false));
                    listControl.Items.Add(GetListItem(EPositionType.RightBottom, false));
                }
                else if (rollingType == ERollingType.FloatingInWindow)
                {
                    listControl.Items.Add(GetListItem(EPositionType.LeftTop, false));
                }
			}
		}

	}
}
