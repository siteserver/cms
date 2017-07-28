using System;
using System.Collections.Generic;

namespace BaiRong.Core.Model.Enumerations
{
    public enum ESmsTemplateType
    {
        Code,
    }

    public class ESmsTemplateTypeUtils
    {
        public static string GetValue(ESmsTemplateType type)
        {
            if (type == ESmsTemplateType.Code)
            {
                return "Code";
            }
            else
            {
                throw new Exception();
            }
        }

        public static string GetText(ESmsTemplateType type)
        {
            if (type == ESmsTemplateType.Code)
            {
                return "验证码";
            }
            else
            {
                throw new Exception();
            }
        }

        public static ESmsTemplateType GetEnumType(string typeStr)
        {
            var retval = ESmsTemplateType.Code;
            if (Equals(typeStr, ESmsTemplateType.Code))
            {
                retval = ESmsTemplateType.Code;
            }
            return retval;
        }

        public static bool Equals(ESmsTemplateType type, string typeStr)
        {
            return !string.IsNullOrEmpty(typeStr) && string.Equals(GetValue(type).ToLower(), typeStr.ToLower());
        }

        public static bool Equals(string typeStr, ESmsTemplateType type)
        {
            return Equals(type, typeStr);
        }

        public static List<ESmsTemplateType> GetList()
        {
            return new List<ESmsTemplateType>
            {
                ESmsTemplateType.Code
            };
        }
    }
}
