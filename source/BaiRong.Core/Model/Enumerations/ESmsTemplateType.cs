using System;
using System.Collections.Generic;

namespace BaiRong.Core.Model.Enumerations
{
    public enum ESmsTemplateType
    {
        Verify,
        Notify
    }

    public class ESmsTemplateTypeUtils
    {
        public static string GetValue(ESmsTemplateType type)
        {
            if (type == ESmsTemplateType.Verify)
            {
                return "Verify";
            }
            if (type == ESmsTemplateType.Notify)
            {
                return "Notify";
            }
            throw new Exception();
        }

        public static string GetText(ESmsTemplateType type)
        {
            if (type == ESmsTemplateType.Verify)
            {
                return "验证码类";
            }
            if (type == ESmsTemplateType.Notify)
            {
                return "通知类";
            }
            throw new Exception();
        }

        public static ESmsTemplateType GetEnumType(string typeStr)
        {
            var retval = ESmsTemplateType.Verify;
            if (Equals(typeStr, ESmsTemplateType.Verify))
            {
                retval = ESmsTemplateType.Verify;
            }
            else if (Equals(typeStr, ESmsTemplateType.Notify))
            {
                retval = ESmsTemplateType.Notify;
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
                ESmsTemplateType.Verify,
                ESmsTemplateType.Notify
            };
        }
    }
}
