using System;
using System.Web.UI.WebControls;

namespace SiteServer.CMS.WeiXin.Model.Enumerations
{
	public enum ELotteryType
	{
        Scratch,
        BigWheel,
        GoldEgg,
        Flap,
        YaoYao
	}

    public class ELotteryTypeUtils
	{
        public static string GetValue(ELotteryType type)
		{
            if (type == ELotteryType.Scratch)
            {
                return "Scratch";
            }
            else if (type == ELotteryType.BigWheel)
            {
                return "BigWheel";
            }
            else if (type == ELotteryType.GoldEgg)
            {
                return "GoldEgg";
            }
            else if (type == ELotteryType.Flap)
            {
                return "Flap";
            }
            else if (type == ELotteryType.YaoYao)
            {
                return "YaoYao";
            }
			else
			{
				throw new Exception();
			}
		}

		public static string GetText(ELotteryType type)
		{
            if (type == ELotteryType.Scratch)
            {
                return "¹Î¹Î¿¨";
            }
            else if (type == ELotteryType.BigWheel)
            {
                return "´ó×ªÅÌ";
            }
            else if (type == ELotteryType.GoldEgg)
            {
                return "ÔÒ½ðµ°";
            }
            else if (type == ELotteryType.Flap)
            {
                return "´ó·­ÅÆ";
            }
            else if (type == ELotteryType.YaoYao)
            {
                return "Ò¡Ò¡ÀÖ";
            }
			else
			{
				throw new Exception();
			}
		}

		public static ELotteryType GetEnumType(string typeStr)
		{
            var retval = ELotteryType.Scratch;

            if (Equals(ELotteryType.Scratch, typeStr))
            {
                retval = ELotteryType.Scratch;
            }
            else if (Equals(ELotteryType.BigWheel, typeStr))
            {
                retval = ELotteryType.BigWheel;
            }
            else if (Equals(ELotteryType.GoldEgg, typeStr))
            {
                retval = ELotteryType.GoldEgg;
            }
            else if (Equals(ELotteryType.Flap, typeStr))
            {
                retval = ELotteryType.Flap;
            }
            else if (Equals(ELotteryType.YaoYao, typeStr))
            {
                retval = ELotteryType.YaoYao;
            }

			return retval;
		}

		public static bool Equals(ELotteryType type, string typeStr)
		{
			if (string.IsNullOrEmpty(typeStr)) return false;
			if (string.Equals(GetValue(type).ToLower(), typeStr.ToLower()))
			{
				return true;
			}
			return false;
		}

        public static bool Equals(string typeStr, ELotteryType type)
        {
            return Equals(type, typeStr);
        }

        public static ListItem GetListItem(ELotteryType type, bool selected)
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
                listControl.Items.Add(GetListItem(ELotteryType.Scratch, false));
                listControl.Items.Add(GetListItem(ELotteryType.BigWheel, false));
                listControl.Items.Add(GetListItem(ELotteryType.GoldEgg, false));
                listControl.Items.Add(GetListItem(ELotteryType.Flap, false));
                listControl.Items.Add(GetListItem(ELotteryType.YaoYao, false));
            }
        }
	}
}
