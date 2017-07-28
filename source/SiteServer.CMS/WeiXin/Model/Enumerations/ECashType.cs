using System;
using System.Web.UI.WebControls;

namespace SiteServer.CMS.WeiXin.Model.Enumerations
{
	public enum ECashType
	{
        Consume,             //消费
        Recharge,            //充值
        Exchange             //兑换
    }

    public class ECashTypeUtils
	{
        public static string GetValue(ECashType type)
		{
            if (type == ECashType.Consume)
            {
                return "Consume";
            }
            else if (type == ECashType.Recharge)
            {
                return "Recharge";
            }
            else if (type == ECashType.Exchange)
            {
                return "Exchange";
            }
            else
			{
				throw new Exception();
			}
		}

		public static string GetText(ECashType type)
		{
            if (type == ECashType.Consume)
            {
                return "消费";
            }
            else if (type == ECashType.Recharge)
            {
                return "充值";
            }
            else if (type == ECashType.Exchange)
            {
                return "兑换";
            }
            else
            {
                throw new Exception();
            }
		}

		public static ECashType GetEnumType(string typeStr)
		{
            var retval = ECashType.Consume;

            if (Equals(ECashType.Consume, typeStr))
            {
                retval = ECashType.Consume;
            }
            else if (Equals(ECashType.Recharge, typeStr))
            {
                retval = ECashType.Recharge;
            }
            else if (Equals(ECashType.Exchange, typeStr))
            {
                retval = ECashType.Exchange;
            }
			return retval;
		}

		public static bool Equals(ECashType type, string typeStr)
		{
			if (string.IsNullOrEmpty(typeStr)) return false;
			if (string.Equals(GetValue(type).ToLower(), typeStr.ToLower()))
			{
				return true;
			}
			return false;
		}

        public static bool Equals(string typeStr, ECashType type)
        {
            return Equals(type, typeStr);
        }

        public static ListItem GetListItem(ECashType type, bool selected)
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
                listControl.Items.Add(GetListItem(ECashType.Consume, false));
                listControl.Items.Add(GetListItem(ECashType.Recharge, false));
                listControl.Items.Add(GetListItem(ECashType.Exchange,false));
             }
        }
	}
}
