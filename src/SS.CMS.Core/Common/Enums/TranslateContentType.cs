using System;
using SS.CMS.Abstractions.Enums;

namespace SS.CMS.Core.Models.Enumerations
{
    public class TranslateContentTypeUtils
    {
        public static string GetValue(TranslateContentType type)
        {
            if (type == TranslateContentType.Copy)
            {
                return "Copy";
            }
            if (type == TranslateContentType.Cut)
            {
                return "Cut";
            }
            if (type == TranslateContentType.Reference)
            {
                return "Reference";
            }
            if (type == TranslateContentType.ReferenceContent)
            {
                return "ReferenceContent";
            }
            throw new Exception();
        }

        public static string GetText(TranslateContentType type)
        {
            if (type == TranslateContentType.Copy)
            {
                return "复制";
            }
            if (type == TranslateContentType.Cut)
            {
                return "剪切";
            }
            if (type == TranslateContentType.Reference)
            {
                return "引用地址";
            }
            if (type == TranslateContentType.ReferenceContent)
            {
                return "引用内容";
            }
            throw new Exception();
        }

        public static bool Equals(TranslateContentType type, string typeStr)
        {
            if (string.IsNullOrEmpty(typeStr)) return false;
            if (string.Equals(GetValue(type).ToLower(), typeStr.ToLower()))
            {
                return true;
            }
            return false;
        }

        public static bool Equals(string typeStr, TranslateContentType type)
        {
            return Equals(type, typeStr);
        }
    }
}
