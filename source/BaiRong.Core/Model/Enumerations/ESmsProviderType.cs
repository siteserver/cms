using System;
using System.Collections.Generic;

namespace BaiRong.Core.Model.Enumerations
{
    public enum ESmsProviderType
    {
        AliDaYu,
        YunPian,
    }

    public class ESmsProviderTypeUtils
    {
        public static string GetValue(ESmsProviderType type)
        {
            if (type == ESmsProviderType.AliDaYu)
            {
                return "AliDaYu";
            }
            else if (type == ESmsProviderType.YunPian)
            {
                return "YunPian";
            }
            else
            {
                throw new Exception();
            }
        }

        public static string GetText(ESmsProviderType type)
        {
            if (type == ESmsProviderType.AliDaYu)
            {
                return "阿里大于";
            }
            else if (type == ESmsProviderType.YunPian)
            {
                return "云片";
            }
            else
            {
                throw new Exception();
            }
        }

        public static string GetUrl(ESmsProviderType type)
        {
            if (type == ESmsProviderType.AliDaYu)
            {
                return "http://www.alidayu.com/";
            }
            else if (type == ESmsProviderType.YunPian)
            {
                return "http://www.yunpian.com/";
            }
            else
            {
                throw new Exception();
            }
        }

        public static ESmsProviderType GetEnumType(string typeStr)
        {
            var retval = ESmsProviderType.AliDaYu;
            if (Equals(typeStr, ESmsProviderType.YunPian))
            {
                retval = ESmsProviderType.YunPian;
            }
            return retval;
        }

        public static bool Equals(ESmsProviderType type, string typeStr)
        {
            return !string.IsNullOrEmpty(typeStr) && string.Equals(GetValue(type).ToLower(), typeStr.ToLower());
        }

        public static bool Equals(string typeStr, ESmsProviderType type)
        {
            return Equals(type, typeStr);
        }

        public static List<ESmsProviderType> GetList()
        {
            return new List<ESmsProviderType>
            {
                //ESmsProviderType.AliDaYu,
                ESmsProviderType.YunPian
            };
        }
    }
}
