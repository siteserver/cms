using System;

namespace SiteServer.CMS.Model.Enumerations
{
    public enum ETranslateContentType
    {
        Copy,               //复制
        Cut,                //剪切
        Reference,           //引用地址
        ReferenceContent,   //引用内容
    }

    public class ETranslateContentTypeUtils
    {
        public static string GetValue(ETranslateContentType type)
        {
            if (type == ETranslateContentType.Copy)
            {
                return "Copy";
            }
            if (type == ETranslateContentType.Cut)
            {
                return "Cut";
            }
            if (type == ETranslateContentType.Reference)
            {
                return "Reference";
            }
            if (type == ETranslateContentType.ReferenceContent)
            {
                return "ReferenceContent";
            }
            throw new Exception();
        }

        public static string GetText(ETranslateContentType type)
        {
            if (type == ETranslateContentType.Copy)
            {
                return "复制";
            }
            if (type == ETranslateContentType.Cut)
            {
                return "剪切";
            }
            if (type == ETranslateContentType.Reference)
            {
                return "引用地址";
            }
            if (type == ETranslateContentType.ReferenceContent)
            {
                return "引用内容";
            }
            throw new Exception();
        }

        public static ETranslateContentType GetEnumType(string typeStr)
        {
            var retval = ETranslateContentType.Copy;

            if (Equals(ETranslateContentType.Copy, typeStr))
            {
                retval = ETranslateContentType.Copy;
            }
            else if (Equals(ETranslateContentType.Cut, typeStr))
            {
                retval = ETranslateContentType.Cut;
            }
            else if (Equals(ETranslateContentType.Reference, typeStr))
            {
                retval = ETranslateContentType.Reference;
            }
            else if (Equals(ETranslateContentType.ReferenceContent, typeStr))
            {
                retval = ETranslateContentType.ReferenceContent;
            }

            return retval;
        }

        public static bool Equals(ETranslateContentType type, string typeStr)
        {
            if (string.IsNullOrEmpty(typeStr)) return false;
            if (string.Equals(GetValue(type).ToLower(), typeStr.ToLower()))
            {
                return true;
            }
            return false;
        }

        public static bool Equals(string typeStr, ETranslateContentType type)
        {
            return Equals(type, typeStr);
        }
    }
}
