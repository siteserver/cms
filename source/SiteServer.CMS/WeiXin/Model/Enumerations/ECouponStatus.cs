using System;
using System.Web.UI.WebControls;

namespace SiteServer.CMS.WeiXin.Model.Enumerations
{
	public enum ECouponStatus
	{
        Unused,
        Hold,
        Cash
	}

    public class ECouponStatusUtils
	{
        public static string GetValue(ECouponStatus type)
		{
            if (type == ECouponStatus.Unused)
            {
                return "Unused";
            }
            else if (type == ECouponStatus.Hold)
            {
                return "Hold";
            }
            else if (type == ECouponStatus.Cash)
            {
                return "Cash";
            }
			else
			{
				throw new Exception();
			}
		}

		public static string GetText(ECouponStatus type)
		{
            if (type == ECouponStatus.Unused)
            {
                return "未领取";
            }
            else if (type == ECouponStatus.Hold)
            {
                return "已领取";
            }
            else if (type == ECouponStatus.Cash)
            {
                return "已使用";
            }
			else
			{
				throw new Exception();
			}
		}

		public static ECouponStatus GetEnumType(string typeStr)
		{
            var retval = ECouponStatus.Unused;

            if (Equals(ECouponStatus.Unused, typeStr))
            {
                retval = ECouponStatus.Unused;
            }
            else if (Equals(ECouponStatus.Hold, typeStr))
            {
                retval = ECouponStatus.Hold;
            }
            else if (Equals(ECouponStatus.Cash, typeStr))
            {
                retval = ECouponStatus.Cash;
            }

			return retval;
		}

		public static bool Equals(ECouponStatus type, string typeStr)
		{
			if (string.IsNullOrEmpty(typeStr)) return false;
			if (string.Equals(GetValue(type).ToLower(), typeStr.ToLower()))
			{
				return true;
			}
			return false;
		}

        public static bool Equals(string typeStr, ECouponStatus type)
        {
            return Equals(type, typeStr);
        }

        public static ListItem GetListItem(ECouponStatus type, bool selected)
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
                listControl.Items.Add(GetListItem(ECouponStatus.Unused, false));
                listControl.Items.Add(GetListItem(ECouponStatus.Hold, false));
                listControl.Items.Add(GetListItem(ECouponStatus.Cash, false));
            }
        }
	}
}
