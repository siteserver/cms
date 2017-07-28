using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using BaiRong.Core;
using SiteServer.CMS.Core;
using SiteServer.CMS.Model;
using SiteServer.CMS.StlParser.Model;
using SiteServer.CMS.StlParser.Utility;
using SiteServer.CMS.StlTemplates;

namespace SiteServer.CMS.StlParser.StlElement
{
    [Stl(Usage = "投票", Description = "通过 stl:vote 标签在模板中实现投票功能")]
    public class StlVote
    {
        private StlVote() { }
        public const string ElementName = "stl:vote";

        public const string AttributeIsDynamic = "isDynamic";

        public static SortedList<string, string> AttributeList => new SortedList<string, string>
        {
            {AttributeIsDynamic, "是否动态显示"}
        };

        public static string Parse(string stlElement, XmlNode node, PageInfo pageInfo, ContextInfo contextInfo)
        {
            string parsedContent;
            try
            {
                var isDynamic = false;

                string inputTemplateString;
                string successTemplateString;
                string failureTemplateString;
                StlParserUtility.GetInnerTemplateStringOfInput(node, out inputTemplateString, out successTemplateString, out failureTemplateString, pageInfo, contextInfo);

                var ie = node.Attributes?.GetEnumerator();
                if (ie != null)
                {
                    while (ie.MoveNext())
                    {
                        var attr = (XmlAttribute) ie.Current;

                        if (StringUtils.EqualsIgnoreCase(attr.Name, AttributeIsDynamic))
                        {
                            isDynamic = TranslateUtils.ToBool(attr.Value);
                        }
                    }
                }

                parsedContent = isDynamic ? StlDynamic.ParseDynamicElement(stlElement, pageInfo, contextInfo) : ParseImpl(pageInfo, contextInfo, inputTemplateString);
            }
            catch (Exception ex)
            {
                parsedContent = StlParserUtility.GetStlErrorMessage(ElementName, ex);
            }

            return parsedContent;
        }

        private static string ParseImpl(PageInfo pageInfo, ContextInfo contextInfo, string inputTemplateString)
        {
            var parsedContent = string.Empty;

            pageInfo.AddPageScriptsIfNotExists(PageInfo.Components.Jquery);
            pageInfo.AddPageScriptsIfNotExists(PageInfo.JQuery.BjTemplates);
            pageInfo.AddPageScriptsIfNotExists(PageInfo.JQuery.BShowLoading);
            pageInfo.AddPageScriptsIfNotExists(PageInfo.JQuery.BValidate);

            pageInfo.AddPageScriptsIfNotExists("SiteServer.CMS.Parser.StlElement",
                $@"<link href=""{SiteFilesAssets.Vote.GetStyleUrl(pageInfo.ApiUrl)}"" type=""text/css"" rel=""stylesheet"" />
");

            var contentInfo = contextInfo.ContentInfo as VoteContentInfo;
            if (contentInfo == null && contextInfo.ContentId > 0)
            {
                contentInfo = DataProvider.VoteContentDao.GetContentInfo(pageInfo.PublishmentSystemInfo, contextInfo.ContentId);
            }

            if (contentInfo != null)
            {
                var voteTemplate = new VoteTemplate(pageInfo.PublishmentSystemInfo, contentInfo.NodeId, contentInfo);
                var contentBuilder = new StringBuilder(voteTemplate.GetTemplate(inputTemplateString));

                StlParserManager.ParseTemplateContent(contentBuilder, pageInfo, contextInfo);
                parsedContent = contentBuilder.ToString();
            }

            return parsedContent;
        }
    }
}
