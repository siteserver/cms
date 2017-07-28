using System;
using System.Web.UI.WebControls;

namespace SiteServer.CMS.Model.Enumerations
{
    public enum EGovInteractLimitType
	{
        Normal,                 //正常
        Alert,                  //预警
        Yellow,                 //黄牌
        Red,                    //红牌
	}

    public class EGovInteractLimitTypeUtils
	{
		public static string GetValue(EGovInteractLimitType type)
		{
            if (type == EGovInteractLimitType.Normal)
			{
                return "Normal";
			}
            else if (type == EGovInteractLimitType.Alert)
			{
                return "Alert";
            }
            else if (type == EGovInteractLimitType.Yellow)
            {
                return "Yellow";
            }
            else if (type == EGovInteractLimitType.Red)
            {
                return "Red";
            }
			else
			{
				throw new Exception();
			}
		}

		public static string GetText(EGovInteractLimitType type)
		{
            if (type == EGovInteractLimitType.Normal)
			{
                return "未超期";
			}
            else if (type == EGovInteractLimitType.Alert)
			{
                return "预警";
            }
            else if (type == EGovInteractLimitType.Yellow)
            {
                return "黄牌";
            }
            else if (type == EGovInteractLimitType.Red)
            {
                return "红牌";
            }
			else
			{
				throw new Exception();
			}
		}

		public static EGovInteractLimitType GetEnumType(string typeStr)
		{
            var retval = EGovInteractLimitType.Normal;

            if (Equals(EGovInteractLimitType.Normal, typeStr))
			{
                retval = EGovInteractLimitType.Normal;
			}
            else if (Equals(EGovInteractLimitType.Alert, typeStr))
			{
                retval = EGovInteractLimitType.Alert;
            }
            else if (Equals(EGovInteractLimitType.Yellow, typeStr))
            {
                retval = EGovInteractLimitType.Yellow;
            }
            else if (Equals(EGovInteractLimitType.Red, typeStr))
            {
                retval = EGovInteractLimitType.Red;
            }
			return retval;
		}

		public static bool Equals(EGovInteractLimitType type, string typeStr)
		{
			if (string.IsNullOrEmpty(typeStr)) return false;
			if (string.Equals(GetValue(type).ToLower(), typeStr.ToLower()))
			{
				return true;
			}
			return false;
		}

        public static bool Equals(string typeStr, EGovInteractLimitType type)
        {
            return Equals(type, typeStr);
        }

        public static ListItem GetListItem(EGovInteractLimitType type, bool selected)
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
                listControl.Items.Add(GetListItem(EGovInteractLimitType.Normal, false));
                listControl.Items.Add(GetListItem(EGovInteractLimitType.Alert, false));
                listControl.Items.Add(GetListItem(EGovInteractLimitType.Yellow, false));
                listControl.Items.Add(GetListItem(EGovInteractLimitType.Red, false));
            }
        }
	}
}
