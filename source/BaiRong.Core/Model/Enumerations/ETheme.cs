using System;
using System.Collections;
using System.Web.UI.WebControls;

namespace BaiRong.Core.Model.Enumerations
{
	public enum ETheme
	{
		Default,
		Google
	}

    public class EThemeUtils
	{
		public static string GetValue(ETheme type)
		{
            if (type == ETheme.Default)
			{
                return "Default";
			}
            else if (type == ETheme.Google)
			{
                return "Google";
			}
			else
			{
				throw new Exception();
			}
		}

        public static string GetText(ETheme type)
		{
            if (type == ETheme.Default)
			{
                return "Ĭ�Ϸ��";
			}
            else if (type == ETheme.Google)
			{
                return "Google���";
			}
			else
			{
				throw new Exception();
			}
		}

		public static ETheme GetEnumType(string typeStr)
		{
            var retval = ETheme.Default;

            if (Equals(ETheme.Default, typeStr))
			{
                retval = ETheme.Default;
			}
            else if (Equals(ETheme.Google, typeStr))
			{
                retval = ETheme.Google;
			}

			return retval;
		}

		public static bool Equals(ETheme type, string typeStr)
		{
			if (string.IsNullOrEmpty(typeStr)) return false;
			if (string.Equals(GetValue(type).ToLower(), typeStr.ToLower()))
			{
				return true;
			}
			return false;
		}

		public static bool Equals(string typeStr, ETheme type)
		{
			return Equals(type, typeStr);
		}

		public static ListItem GetListItem(ETheme type, bool selected)
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
                listControl.Items.Add(GetListItem(ETheme.Default, false));
				listControl.Items.Add(GetListItem(ETheme.Google, false));
			}
		}

        public static ArrayList GetArrayList()
        {
            var arraylist = new ArrayList();
            arraylist.Add(ETheme.Default);
            arraylist.Add(ETheme.Google);
            return arraylist;
        }

		public static void AddListItems(ListControl listControl, string trueText, string falseText)
		{
			if (listControl != null)
			{
                var item = new ListItem(trueText, GetValue(ETheme.Default));
				listControl.Items.Add(item);
                item = new ListItem(falseText, GetValue(ETheme.Google));
				listControl.Items.Add(item);
			}
		}

	}
}
