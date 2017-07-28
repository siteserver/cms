using System;

namespace SiteServer.CMS.Model.Enumerations
{
	public enum EGovPublicCategoryClassType
	{
        Channel,
        Form,
        Department,
        Service,
        UserDefined
	}

    public class EGovPublicCategoryClassTypeUtils
	{
		public static string GetValue(EGovPublicCategoryClassType type)
		{
            if (type == EGovPublicCategoryClassType.Channel)
			{
                return "Channel";
			}
            else if (type == EGovPublicCategoryClassType.Form)
			{
                return "Form";
            }
            else if (type == EGovPublicCategoryClassType.Department)
            {
                return "Department";
            }
            else if (type == EGovPublicCategoryClassType.Service)
            {
                return "Service";
            }
            else if (type == EGovPublicCategoryClassType.UserDefined)
            {
                return "UserDefined";
            }
			else
			{
				throw new Exception();
			}
		}

		public static string GetText(EGovPublicCategoryClassType type)
		{
            if (type == EGovPublicCategoryClassType.Channel)
			{
                return "主题";
			}
            else if (type == EGovPublicCategoryClassType.Form)
			{
                return "体裁";
            }
            else if (type == EGovPublicCategoryClassType.Department)
            {
                return "机构";
            }
            else if (type == EGovPublicCategoryClassType.Service)
            {
                return "服务对象";
            }
            else if (type == EGovPublicCategoryClassType.UserDefined)
            {
                return "自定义";
            }
			else
			{
				throw new Exception();
			}
		}

		public static EGovPublicCategoryClassType GetEnumType(string typeStr)
		{
            var retval = EGovPublicCategoryClassType.Form;

            if (Equals(EGovPublicCategoryClassType.Channel, typeStr))
			{
                retval = EGovPublicCategoryClassType.Channel;
			}
            else if (Equals(EGovPublicCategoryClassType.Form, typeStr))
			{
                retval = EGovPublicCategoryClassType.Form;
            }
            else if (Equals(EGovPublicCategoryClassType.Department, typeStr))
            {
                retval = EGovPublicCategoryClassType.Department;
            }
            else if (Equals(EGovPublicCategoryClassType.Service, typeStr))
            {
                retval = EGovPublicCategoryClassType.Service;
            }
            else if (Equals(EGovPublicCategoryClassType.UserDefined, typeStr))
            {
                retval = EGovPublicCategoryClassType.UserDefined;
            }
			return retval;
		}

		public static bool Equals(EGovPublicCategoryClassType type, string typeStr)
		{
			if (string.IsNullOrEmpty(typeStr)) return false;
			if (string.Equals(GetValue(type).ToLower(), typeStr.ToLower()))
			{
				return true;
			}
			return false;
		}

        public static bool Equals(string typeStr, EGovPublicCategoryClassType type)
        {
            return Equals(type, typeStr);
        }
	}
}
