using System;
using System.Web.UI.WebControls;

namespace SiteServer.CMS.Model.Enumerations
{

	public enum EWriteBackMethod
	{
		None,				//����Ҫ�ظ�
		ByWriteBackField,	//ʹ��ǰ̨���ֶλظ�
		ByEmail,			//ʹ�õ����ʼ��ظ�
		All					//���ֻظ���ʽ����
	}

	public class EWriteBackMethodUtils
	{
		public static string GetValue(EWriteBackMethod type)
		{
			if (type == EWriteBackMethod.None)
			{
				return "None";
			}
			else if (type == EWriteBackMethod.ByWriteBackField)
			{
				return "ByWriteBackField";
			}
			else if (type == EWriteBackMethod.ByEmail)
			{
				return "ByEmail";
			}
			else if (type == EWriteBackMethod.All)
			{
				return "All";
			}
			else
			{
				throw new Exception();
			}
		}

		public static string GetText(EWriteBackMethod type)
		{
			if (type == EWriteBackMethod.None)
			{
				return "���ظ���Ϣ";
			}
			else if (type == EWriteBackMethod.ByWriteBackField)
			{
				return "ֱ�ӻظ���Ϣ";
			}
			else if (type == EWriteBackMethod.ByEmail)
			{
				return "ͨ���ʼ��ظ���Ϣ";
			}
			else if (type == EWriteBackMethod.All)
			{
				return "ͬʱʹ�����ֻظ���ʽ";
			}
			else
			{
				throw new Exception();
			}
		}

		public static EWriteBackMethod GetEnumType(string typeStr)
		{
			var retval = EWriteBackMethod.None;

			if (Equals(EWriteBackMethod.None, typeStr))
			{
				retval = EWriteBackMethod.None;
			}
			else if (Equals(EWriteBackMethod.ByWriteBackField, typeStr))
			{
				retval = EWriteBackMethod.ByWriteBackField;
			}
			else if (Equals(EWriteBackMethod.ByEmail, typeStr))
			{
				retval = EWriteBackMethod.ByEmail;
			}
			else if (Equals(EWriteBackMethod.All, typeStr))
			{
				retval = EWriteBackMethod.All;
			}

			return retval;
		}

		public static bool Equals(EWriteBackMethod type, string typeStr)
		{
			if (string.IsNullOrEmpty(typeStr)) return false;
			if (string.Equals(GetValue(type).ToLower(), typeStr.ToLower()))
			{
				return true;
			}
			return false;
		}

        public static bool Equals(string typeStr, EWriteBackMethod type)
        {
            return Equals(type, typeStr);
        }

		public static ListItem GetListItem(EWriteBackMethod type, bool selected)
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
				listControl.Items.Add(GetListItem(EWriteBackMethod.None, false));
				listControl.Items.Add(GetListItem(EWriteBackMethod.ByWriteBackField, false));
				listControl.Items.Add(GetListItem(EWriteBackMethod.ByEmail, false));
				listControl.Items.Add(GetListItem(EWriteBackMethod.All, false));
			}
		}

	}
}
