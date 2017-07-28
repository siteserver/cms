using System;
using System.Collections.Generic;
using System.Web.UI.WebControls;

namespace SiteServer.CMS.WeiXin.Model.Enumerations
{
	public enum EWebMenuType
	{
        Type1,
        Type2,
        //Type3,
        //Type4,
        //Type5
	}

    public class EWebMenuTypeUtils
	{
        public static string GetValue(EWebMenuType type)
		{
            if (type == EWebMenuType.Type1)
            {
                return "Type1";
            }
            else if (type == EWebMenuType.Type2)
            {
                return "Type2";
            }
            //else if (type == EWebMenuType.Type3)
            //{
            //    return "Type3";
            //}
            //else if (type == EWebMenuType.Type4)
            //{
            //    return "Type4";
            //}
            //else if (type == EWebMenuType.Type5)
            //{
            //    return "Type5";
            //}
			else
			{
				throw new Exception();
			}
		}

		public static string GetText(EWebMenuType type)
		{
            if (type == EWebMenuType.Type1)
            {
                return "样式一";
            }
            else if (type == EWebMenuType.Type2)
            {
                return "样式二";
            }
            //else if (type == EWebMenuType.Type3)
            //{
            //    return "样式三";
            //}
            //else if (type == EWebMenuType.Type4)
            //{
            //    return "样式四";
            //}
            //else if (type == EWebMenuType.Type5)
            //{
            //    return "样式五";
            //}
			else
			{
				throw new Exception();
			}
		}

		public static EWebMenuType GetEnumType(string typeStr)
		{
            var retval = EWebMenuType.Type1;

            if (Equals(EWebMenuType.Type1, typeStr))
            {
                retval = EWebMenuType.Type1;
            }
            else if (Equals(EWebMenuType.Type2, typeStr))
            {
                retval = EWebMenuType.Type2;
            }
            //else if (Equals(EWebMenuType.Type3, typeStr))
            //{
            //    retval = EWebMenuType.Type3;
            //}
            //else if (Equals(EWebMenuType.Type4, typeStr))
            //{
            //    retval = EWebMenuType.Type4;
            //}
            //else if (Equals(EWebMenuType.Type5, typeStr))
            //{
            //    retval = EWebMenuType.Type5;
            //}

			return retval;
		}

		public static bool Equals(EWebMenuType type, string typeStr)
		{
			if (string.IsNullOrEmpty(typeStr)) return false;
			if (string.Equals(GetValue(type).ToLower(), typeStr.ToLower()))
			{
				return true;
			}
			return false;
		}

        public static bool Equals(string typeStr, EWebMenuType type)
        {
            return Equals(type, typeStr);
        }

        public static ListItem GetListItem(EWebMenuType type, bool selected)
        {
            var item = new ListItem(GetText(type), GetValue(type));
            if (selected)
            {
                item.Selected = true;
            }
            return item;
        }

        public static List<EWebMenuType> GetList()
        {
            var list = new List<EWebMenuType>();

            list.Add(EWebMenuType.Type1);
            list.Add(EWebMenuType.Type2);
            //list.Add(EWebMenuType.Type3);
            //list.Add(EWebMenuType.Type4);
            //list.Add(EWebMenuType.Type5);

            return list;
        }
	}
}
