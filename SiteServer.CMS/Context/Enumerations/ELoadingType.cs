using System;

namespace SiteServer.CMS.Context.Enumerations
{
    public enum ELoadingType
    {
        ContentTree,
        ChannelClickSelect,
        SiteAnalysis
    }

    public static class ELoadingTypeUtils
    {
        public static string GetValue(ELoadingType type)
        {
            if (type == ELoadingType.ContentTree)
            {
                return "ContentTree";
            }
            if (type == ELoadingType.ChannelClickSelect)
            {
                return "ChannelClickSelect";
            }
            if (type == ELoadingType.SiteAnalysis)
            {
                return "SiteAnalysis";
            }
            throw new Exception();
        }

        public static ELoadingType GetEnumType(string typeStr)
        {
            var retVal = ELoadingType.ContentTree;

            if (Equals(ELoadingType.ContentTree, typeStr))
            {
                retVal = ELoadingType.ContentTree;
            }
            else if (Equals(ELoadingType.ChannelClickSelect, typeStr))
            {
                retVal = ELoadingType.ChannelClickSelect;
            }
            else if (Equals(ELoadingType.SiteAnalysis, typeStr))
            {
                retVal = ELoadingType.SiteAnalysis;
            }

            return retVal;
        }

        public static bool Equals(ELoadingType type, string typeStr)
        {
            if (string.IsNullOrEmpty(typeStr)) return false;
            if (string.Equals(GetValue(type).ToLower(), typeStr.ToLower()))
            {
                return true;
            }
            return false;
        }

        public static bool Equals(string typeStr, ELoadingType type)
        {
            return Equals(type, typeStr);
        }
    }
}
