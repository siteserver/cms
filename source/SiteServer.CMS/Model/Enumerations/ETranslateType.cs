using System;
using System.Web.UI.WebControls;

namespace SiteServer.CMS.Model.Enumerations
{
	/// <summary>
	/// 批量转移类型
	/// </summary>
	public enum ETranslateType
	{
		Content,				//仅转移内容
		Channel,				//仅转移栏目
		All						//转移栏目及内容
	}

	public class ETranslateTypeUtils
	{
		public static string GetValue(ETranslateType type)
		{
			if (type == ETranslateType.Content)
			{
				return "Content";
			}
			else if (type == ETranslateType.Channel)
			{
				return "Channel";
			}
			else if (type == ETranslateType.All)
			{
				return "All";
			}
			else
			{
				throw new Exception();
			}
		}

		public static string GetText(ETranslateType type)
		{
			if (type == ETranslateType.Content)
			{
				return "仅转移内容";
			}
			else if (type == ETranslateType.Channel)
			{
				return "仅转移栏目";
			}
			else if (type == ETranslateType.All)
			{
				return "转移栏目及内容";
			}
			else
			{
				throw new Exception();
			}
		}

		public static ETranslateType GetEnumType(string typeStr)
		{
			var retval = ETranslateType.Content;

			if (Equals(ETranslateType.Content, typeStr))
			{
				retval = ETranslateType.Content;
			}
			else if (Equals(ETranslateType.Channel, typeStr))
			{
				retval = ETranslateType.Channel;
			}
			else if (Equals(ETranslateType.All, typeStr))
			{
				retval = ETranslateType.All;
			}

			return retval;
		}

		public static bool Equals(ETranslateType type, string typeStr)
		{
			if (string.IsNullOrEmpty(typeStr)) return false;
			if (string.Equals(GetValue(type).ToLower(), typeStr.ToLower()))
			{
				return true;
			}
			return false;
		}

        public static bool Equals(string typeStr, ETranslateType type)
        {
            return Equals(type, typeStr);
        }

		public static ListItem GetListItem(ETranslateType type, bool selected)
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
				listControl.Items.Add(GetListItem(ETranslateType.Content, false));
				listControl.Items.Add(GetListItem(ETranslateType.Channel, false));
				listControl.Items.Add(GetListItem(ETranslateType.All, false));
			}
		}

	}
}
