using System;
using SS.CMS.Enums;

namespace SS.CMS.Core.Common
{
    public static class TemplateTypeUtils
    {
        public static bool Equals(TemplateType type, string typeStr)
        {
            if (string.IsNullOrEmpty(typeStr)) return false;
            if (string.Equals(type.Value.ToLower(), typeStr.ToLower()))
            {
                return true;
            }
            return false;
        }

        public static bool Equals(string typeStr, TemplateType type)
        {
            return Equals(type, typeStr);
        }

        public static string GetText(TemplateType templateType)
        {
            if (templateType == TemplateType.IndexPageTemplate)
            {
                return "首页模板";
            }
            if (templateType == TemplateType.ChannelTemplate)
            {
                return "栏目模板";
            }
            if (templateType == TemplateType.ContentTemplate)
            {
                return "内容模板";
            }
            if (templateType == TemplateType.FileTemplate)
            {
                return "单页模板";
            }

            throw new Exception();
        }
    }
}
