using System;
using System.Collections.Generic;
using System.Web.UI.WebControls;

namespace BaiRong.Core.Model.Enumerations
{
	public enum EDiggType
	{
		Good,
		Bad,
        All
	}

    public class EDiggTypeUtils
	{
		public static string GetValue(EDiggType type)
		{
            if (type == EDiggType.Good)
			{
                return "Good";
			}
            else if (type == EDiggType.Bad)
			{
                return "Bad";
            }
            else if (type == EDiggType.All)
            {
                return "All";
            }
			else
			{
				throw new Exception();
			}
		}

        public static string GetText(EDiggType type)
        {
            if (type == EDiggType.Good)
            {
                return "仅显示赞同";
            }
            else if (type == EDiggType.Bad)
            {
                return "仅显示不赞同";
            }
            else if (type == EDiggType.All)
            {
                return "显示全部";
            }
            else
            {
                throw new Exception();
            }
        }

		public static EDiggType GetEnumType(string typeStr)
		{
            var retval = EDiggType.All;

            if (Equals(EDiggType.Good, typeStr))
			{
                retval = EDiggType.Good;
			}
            else if (Equals(EDiggType.Bad, typeStr))
			{
                retval = EDiggType.Bad;
			}

			return retval;
		}

		public static bool Equals(EDiggType type, string typeStr)
		{
			if (string.IsNullOrEmpty(typeStr)) return false;
			if (string.Equals(GetValue(type).ToLower(), typeStr.ToLower()))
			{
				return true;
			}
			return false;
		}

        public static bool Equals(string typeStr, EDiggType type)
        {
            return Equals(type, typeStr);
        }

        public static ListItem GetListItem(EDiggType type, bool selected)
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
                listControl.Items.Add(GetListItem(EDiggType.All, false));
                listControl.Items.Add(GetListItem(EDiggType.Good, false));
                listControl.Items.Add(GetListItem(EDiggType.Bad, false));
            }
        }

        public static SortedList<string, string> TypeList => new SortedList<string, string>
        {
            {GetValue(EDiggType.All), GetText(EDiggType.All)},
            {GetValue(EDiggType.Good), GetText(EDiggType.Good)},
            {GetValue(EDiggType.Bad), GetText(EDiggType.Bad)}
        };
    }
}
