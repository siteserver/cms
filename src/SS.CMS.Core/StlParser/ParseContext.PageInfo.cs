using System;
using System.Collections.Generic;
using SS.CMS.Core.Common;
using SS.CMS.Core.StlParser.Models;
using SS.CMS.Models;
using SS.CMS.Utils;

namespace SS.CMS.Core.StlParser
{
    public partial class ParseContext
    {
        public int SiteId => PageInfo.SiteId;

        public SiteInfo SiteInfo => PageInfo.SiteInfo;

        public int PageChannelId => PageInfo.PageChannelId;

        public int PageContentId => PageInfo.PageContentId;

        public TemplateInfo TemplateInfo => PageInfo.TemplateInfo;

        public SortedDictionary<string, string> HeadCodes => PageInfo.HeadCodes;

        public SortedDictionary<string, string> BodyCodes => PageInfo.BodyCodes;

        public SortedDictionary<string, string> FootCodes => PageInfo.FootCodes;

        public bool IsLocal => PageInfo.IsLocal;

        public int UniqueId => PageInfo.UniqueId;

        public PageInfo PageInfo { get; set; }

        public string GetErrorMessage(string elementName, string stlElement, Exception ex)
        {
            var summary = string.Empty;
            summary = $@"站点名称：{SiteInfo.SiteName}，
模板类型：{TemplateTypeUtils.GetText(TemplateInfo.Type)}，
模板名称：{TemplateInfo.TemplateName}
<br />
STL标签：{StringUtils.HtmlEncode(stlElement)}";

            summary += $@"";

            ErrorLogRepository.AddStlErrorLog(summary, elementName, stlElement, ex);

            return $@"
<!--
{elementName}
error: {ex.Message}
stl: {stlElement}
-->";
        }
    }
}
