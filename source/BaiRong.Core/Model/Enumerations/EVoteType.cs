using System;
using System.Web.UI.WebControls;

namespace BaiRong.Core.Model.Enumerations
{
	
	public enum EVoteType
	{
		RadioVote,				//��ѡͶƱ
		CheckBoxVote			//��ѡͶƱ
	}

	public class EVoteTypeUtils
	{
		public static string GetValue(EVoteType type)
		{
			if (type == EVoteType.RadioVote)
			{
				return "RadioVote";
			}
			else if (type == EVoteType.CheckBoxVote)
			{
				return "CheckBoxVote";
			}
			else
			{
				throw new Exception();
			}
		}

		public static string GetText(EVoteType type)
		{
			if (type == EVoteType.CheckBoxVote)
			{
				return "��ѡ";
			}
			else if (type == EVoteType.RadioVote)
			{
				return "��ѡ";
			}
			else
			{
				throw new Exception();
			}
		}

		public static EVoteType GetEnumType(string typeStr)
		{
			var retval = EVoteType.RadioVote;

			if (Equals(EVoteType.CheckBoxVote, typeStr))
			{
				retval = EVoteType.CheckBoxVote;
			}
			else if (Equals(EVoteType.RadioVote, typeStr))
			{
				retval = EVoteType.RadioVote;
			}

			return retval;
		}

		public static bool Equals(EVoteType type, string typeStr)
		{
			if (string.IsNullOrEmpty(typeStr)) return false;
			if (string.Equals(GetValue(type).ToLower(), typeStr.ToLower()))
			{
				return true;
			}
			return false;
		}

        public static bool Equals(string typeStr, EVoteType type)
        {
            return Equals(type, typeStr);
        }

		public static ListItem GetListItem(EVoteType type, bool selected)
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
				listControl.Items.Add(GetListItem(EVoteType.RadioVote, false));
				listControl.Items.Add(GetListItem(EVoteType.CheckBoxVote, false));
			}
		}

	}
}
