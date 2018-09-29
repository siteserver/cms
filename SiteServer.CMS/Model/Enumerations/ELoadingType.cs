using System;

namespace SiteServer.CMS.Model.Enumerations
{
    public enum ELoadingType
    {
        ContentTree,
        Channel,
        ChannelClickSelect,
        SiteAnalysis,
        TemplateFilePathRule,
        ConfigurationCreateDetails,
        ConfigurationCrossSiteTrans
    }

    public static class ELoadingTypeUtils
    {
        public static string GetValue(ELoadingType type)
        {
            if (type == ELoadingType.ContentTree)
            {
                return "ContentTree";
            }
            if (type == ELoadingType.Channel)
            {
                return "Channel";
            }
            if (type == ELoadingType.ChannelClickSelect)
            {
                return "ChannelClickSelect";
            }
            if (type == ELoadingType.SiteAnalysis)
            {
                return "SiteAnalysis";
            }
            if (type == ELoadingType.TemplateFilePathRule)
            {
                return "TemplateFilePathRule";
            }
            if (type == ELoadingType.ConfigurationCreateDetails)
            {
                return "ConfigurationCreateDetails";
            }
            if (type == ELoadingType.ConfigurationCrossSiteTrans)
            {
                return "ConfigurationCrossSiteTrans";
            }
            throw new Exception();
        }

        public static ELoadingType GetEnumType(string typeStr)
        {
            var retval = ELoadingType.Channel;

            if (Equals(ELoadingType.ContentTree, typeStr))
            {
                retval = ELoadingType.ContentTree;
            }
            else if (Equals(ELoadingType.Channel, typeStr))
            {
                retval = ELoadingType.Channel;
            }
            else if (Equals(ELoadingType.ChannelClickSelect, typeStr))
            {
                retval = ELoadingType.ChannelClickSelect;
            }
            else if (Equals(ELoadingType.SiteAnalysis, typeStr))
            {
                retval = ELoadingType.SiteAnalysis;
            }
            else if (Equals(ELoadingType.TemplateFilePathRule, typeStr))
            {
                retval = ELoadingType.TemplateFilePathRule;
            }
            else if (Equals(ELoadingType.ConfigurationCreateDetails, typeStr))
            {
                retval = ELoadingType.ConfigurationCreateDetails;
            }
            else if (Equals(ELoadingType.ConfigurationCrossSiteTrans, typeStr))
            {
                retval = ELoadingType.ConfigurationCrossSiteTrans;
            }

            return retval;
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
