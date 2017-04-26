using System;
using System.Web.UI.WebControls;

namespace SiteServer.CMS.WeiXin.Model.Enumerations
{
	public enum EConsumeType
	{
        Cash,             //现金消费
        CardAmount           //会员卡余额消费
    }

    public class EConsumeTypeUtils
	{
        public static string GetValue(EConsumeType type)
		{
            if (type == EConsumeType.Cash)
            {
                return "Cash";
            }
            else if (type == EConsumeType.CardAmount)
            {
                return "CardAmount";
            }
            else
			{
				throw new Exception();
			}
		}

		public static string GetText(EConsumeType type)
		{
            if (type == EConsumeType.Cash )
            {
                return "现金消费";
            }
            else if (type == EConsumeType.CardAmount)
            {
                return "会员卡余额消费";
            }
            else
			{
				throw new Exception();
			}
		}

		public static EConsumeType GetEnumType(string typeStr)
		{
            var retval = EConsumeType.Cash ;

            if (Equals(EConsumeType.Cash, typeStr))
            {
                retval = EConsumeType.Cash;
            }
            else if (Equals(EConsumeType.CardAmount, typeStr))
            {
                retval = EConsumeType.CardAmount;
            }
              
			return retval;
		}

		public static bool Equals(EConsumeType type, string typeStr)
		{
			if (string.IsNullOrEmpty(typeStr)) return false;
			if (string.Equals(GetValue(type).ToLower(), typeStr.ToLower()))
			{
				return true;
			}
			return false;
		}

        public static bool Equals(string typeStr, EConsumeType type)
        {
            return Equals(type, typeStr);
        }

        public static ListItem GetListItem(EConsumeType type, bool selected)
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
                listControl.Items.Add(GetListItem(EConsumeType.Cash , false));
                listControl.Items.Add(GetListItem(EConsumeType.CardAmount, false));
             }
        }
	}
}
