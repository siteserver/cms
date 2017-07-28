using System;
using System.Web.UI.WebControls;

namespace BaiRong.Core.Model.Enumerations
{
	
	public enum EVoteRestrictType
	{
		NoRestrict,				//�����ظ�ͶƱ
		RestrictOneDay,			//һ���ڽ�ֹͬһIP�ظ�ͶƱ
		RestrictOnlyOnce,		//ÿ̨��ֻ��ͶһƱ
        RestrictUser    		//ÿ�û�ֻ��ͶһƱ
	}

	public class EVoteRestrictTypeUtils
	{
		public static string GetValue(EVoteRestrictType type)
		{
			if (type == EVoteRestrictType.NoRestrict)
			{
				return "NoRestrict";
			}
			else if (type == EVoteRestrictType.RestrictOneDay)
			{
				return "RestrictOneDay";
			}
			else if (type == EVoteRestrictType.RestrictOnlyOnce)
			{
				return "RestrictOnlyOnce";
            }
            else if (type == EVoteRestrictType.RestrictUser)
            {
                return "RestrictUser";
            }
			else
			{
				throw new Exception();
			}
		}

		public static string GetText(EVoteRestrictType type)
		{
			if (type == EVoteRestrictType.NoRestrict)
			{
				return "�����ظ�ͶƱ";
			}
			else if (type == EVoteRestrictType.RestrictOneDay)
			{
				return "һ���ڽ�ֹ�ظ�ͶƱ";
			}
			else if (type == EVoteRestrictType.RestrictOnlyOnce)
			{
				return "ÿ̨��ֻ��ͶһƱ";
            }
            else if (type == EVoteRestrictType.RestrictUser)
            {
                return "ÿ�û�ֻ��ͶһƱ";
            }
			else
			{
				throw new Exception();
			}
		}

		public static EVoteRestrictType GetEnumType(string typeStr)
		{
			var retval = EVoteRestrictType.NoRestrict;

			if (Equals(EVoteRestrictType.NoRestrict, typeStr))
			{
				retval = EVoteRestrictType.NoRestrict;
			}
			else if (Equals(EVoteRestrictType.RestrictOneDay, typeStr))
			{
				retval = EVoteRestrictType.RestrictOneDay;
			}
			else if (Equals(EVoteRestrictType.RestrictOnlyOnce, typeStr))
			{
				retval = EVoteRestrictType.RestrictOnlyOnce;
            }
            else if (Equals(EVoteRestrictType.RestrictUser, typeStr))
            {
                retval = EVoteRestrictType.RestrictUser;
            }

			return retval;
		}

		public static bool Equals(EVoteRestrictType type, string typeStr)
		{
			if (string.IsNullOrEmpty(typeStr)) return false;
			if (string.Equals(GetValue(type).ToLower(), typeStr.ToLower()))
			{
				return true;
			}
			return false;
		}

        public static bool Equals(string typeStr, EVoteRestrictType type)
        {
            return Equals(type, typeStr);
        }

		public static ListItem GetListItem(EVoteRestrictType type, bool selected)
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
				listControl.Items.Add(GetListItem(EVoteRestrictType.NoRestrict, false));
				listControl.Items.Add(GetListItem(EVoteRestrictType.RestrictOneDay, false));
				listControl.Items.Add(GetListItem(EVoteRestrictType.RestrictOnlyOnce, false));
                listControl.Items.Add(GetListItem(EVoteRestrictType.RestrictUser, false));
			}
		}

	}
}
