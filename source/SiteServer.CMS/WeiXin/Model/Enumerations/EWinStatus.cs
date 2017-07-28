using System;
using System.Web.UI.WebControls;

namespace SiteServer.CMS.WeiXin.Model.Enumerations
{
	public enum EWinStatus
	{
        Won,
        Applied,
        Cashed
	}

    public class EWinStatusUtils
	{
        public static string GetValue(EWinStatus type)
		{
            if (type == EWinStatus.Won)
            {
                return "Won";
            }
            else if (type == EWinStatus.Applied)
            {
                return "Applied";
            }
            else if (type == EWinStatus.Cashed)
            {
                return "Cashed";
            }
			else
			{
				throw new Exception();
			}
		}

		public static string GetText(EWinStatus type)
		{
            if (type == EWinStatus.Won)
            {
                return "已中奖";
            }
            else if (type == EWinStatus.Applied)
            {
                return "已提交信息";
            }
            else if (type == EWinStatus.Cashed)
            {
                return "已兑奖";
            }
			else
			{
				throw new Exception();
			}
		}

		public static EWinStatus GetEnumType(string typeStr)
		{
            var retval = EWinStatus.Won;

            if (Equals(EWinStatus.Won, typeStr))
            {
                retval = EWinStatus.Won;
            }
            else if (Equals(EWinStatus.Applied, typeStr))
            {
                retval = EWinStatus.Applied;
            }
            else if (Equals(EWinStatus.Cashed, typeStr))
            {
                retval = EWinStatus.Cashed;
            }

			return retval;
		}

		public static bool Equals(EWinStatus type, string typeStr)
		{
			if (string.IsNullOrEmpty(typeStr)) return false;
			if (string.Equals(GetValue(type).ToLower(), typeStr.ToLower()))
			{
				return true;
			}
			return false;
		}

        public static bool Equals(string typeStr, EWinStatus type)
        {
            return Equals(type, typeStr);
        }

        public static ListItem GetListItem(EWinStatus type, bool selected)
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
                listControl.Items.Add(GetListItem(EWinStatus.Won, false));
                listControl.Items.Add(GetListItem(EWinStatus.Applied, false));
                listControl.Items.Add(GetListItem(EWinStatus.Cashed, false));
            }
        }
	}
}
