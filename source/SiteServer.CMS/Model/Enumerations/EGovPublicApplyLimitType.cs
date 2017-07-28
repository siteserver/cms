using System;
using System.Web.UI.WebControls;

namespace SiteServer.CMS.Model.Enumerations
{
    public enum EGovPublicApplyLimitType
	{
        Normal,                 //正常
        Alert,                  //预警
        Yellow,                 //黄牌
        Red,                    //红牌
	}

    public class EGovPublicApplyLimitTypeUtils
	{
		public static string GetValue(EGovPublicApplyLimitType type)
		{
            if (type == EGovPublicApplyLimitType.Normal)
			{
                return "Normal";
			}
            else if (type == EGovPublicApplyLimitType.Alert)
			{
                return "Alert";
            }
            else if (type == EGovPublicApplyLimitType.Yellow)
            {
                return "Yellow";
            }
            else if (type == EGovPublicApplyLimitType.Red)
            {
                return "Red";
            }
			else
			{
				throw new Exception();
			}
		}

		public static string GetText(EGovPublicApplyLimitType type)
		{
            if (type == EGovPublicApplyLimitType.Normal)
			{
                return "未超期";
			}
            else if (type == EGovPublicApplyLimitType.Alert)
			{
                return "预警";
            }
            else if (type == EGovPublicApplyLimitType.Yellow)
            {
                return "黄牌";
            }
            else if (type == EGovPublicApplyLimitType.Red)
            {
                return "红牌";
            }
			else
			{
				throw new Exception();
			}
		}

		public static EGovPublicApplyLimitType GetEnumType(string typeStr)
		{
            var retval = EGovPublicApplyLimitType.Normal;

            if (Equals(EGovPublicApplyLimitType.Normal, typeStr))
			{
                retval = EGovPublicApplyLimitType.Normal;
			}
            else if (Equals(EGovPublicApplyLimitType.Alert, typeStr))
			{
                retval = EGovPublicApplyLimitType.Alert;
            }
            else if (Equals(EGovPublicApplyLimitType.Yellow, typeStr))
            {
                retval = EGovPublicApplyLimitType.Yellow;
            }
            else if (Equals(EGovPublicApplyLimitType.Red, typeStr))
            {
                retval = EGovPublicApplyLimitType.Red;
            }
			return retval;
		}

		public static bool Equals(EGovPublicApplyLimitType type, string typeStr)
		{
			if (string.IsNullOrEmpty(typeStr)) return false;
			if (string.Equals(GetValue(type).ToLower(), typeStr.ToLower()))
			{
				return true;
			}
			return false;
		}

        public static bool Equals(string typeStr, EGovPublicApplyLimitType type)
        {
            return Equals(type, typeStr);
        }

        public static ListItem GetListItem(EGovPublicApplyLimitType type, bool selected)
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
                listControl.Items.Add(GetListItem(EGovPublicApplyLimitType.Normal, false));
                listControl.Items.Add(GetListItem(EGovPublicApplyLimitType.Alert, false));
                listControl.Items.Add(GetListItem(EGovPublicApplyLimitType.Yellow, false));
                listControl.Items.Add(GetListItem(EGovPublicApplyLimitType.Red, false));
            }
        }
	}
}
