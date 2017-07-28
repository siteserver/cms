using System;
using System.Web.UI.WebControls;

namespace SiteServer.CMS.WeiXin.Model.Enumerations
{
	public enum EMatchType
	{
        Exact,
        Contains
	}

    public class EMatchTypeUtils
	{
        public static string GetValue(EMatchType type)
		{
            if (type == EMatchType.Exact)
            {
                return "Exact";
            }
            else if (type == EMatchType.Contains)
            {
                return "Contains";
            }
			else
			{
				throw new Exception();
			}
		}

		public static string GetText(EMatchType type)
		{
            if (type == EMatchType.Exact)
            {
                return "��ȷƥ��";
            }
            else if (type == EMatchType.Contains)
            {
                return "�����ؼ���";
            }
			else
			{
				throw new Exception();
			}
		}

		public static EMatchType GetEnumType(string typeStr)
		{
            var retval = EMatchType.Exact;

            if (Equals(EMatchType.Exact, typeStr))
            {
                retval = EMatchType.Exact;
            }
            else if (Equals(EMatchType.Contains, typeStr))
            {
                retval = EMatchType.Contains;
            }

			return retval;
		}

		public static bool Equals(EMatchType type, string typeStr)
		{
			if (string.IsNullOrEmpty(typeStr)) return false;
			if (string.Equals(GetValue(type).ToLower(), typeStr.ToLower()))
			{
				return true;
			}
			return false;
		}

        public static bool Equals(string typeStr, EMatchType type)
        {
            return Equals(type, typeStr);
        }

        public static ListItem GetListItem(EMatchType type, bool selected)
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
                listControl.Items.Add(GetListItem(EMatchType.Exact, false));
                listControl.Items.Add(GetListItem(EMatchType.Contains, false));
            }
        }
	}
}
