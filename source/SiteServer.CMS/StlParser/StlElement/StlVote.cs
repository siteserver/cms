using System;
using System.Collections.Generic;
using System.Collections.Specialized;
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
    public class StlVote
    {
        private StlVote() { }
        public const string ElementName = "stl:vote";

        public const string Attribute_Theme = "theme";                      //主题样式

        public const string Attribute_IsDynamic = "isdynamic";              //是否动态显示

        public const string Theme_Style1 = "Style1";
        public const string Theme_Style2 = "Style2";

        public static List<string> AttributeValuesTheme
        {
            get
            {
                var list = new List<string>();
                list.Add(Theme_Style1);
                list.Add(Theme_Style2);
                return list;
            }
        }

        public static ListDictionary AttributeList
        {
            get
            {
                var attributes = new ListDictionary();
                attributes.Add(Attribute_Theme, "主题样式");
                attributes.Add(Attribute_IsDynamic, "是否动态显示");
                return attributes;
            }
        }

        public sealed class InputTemplate
        {
            public const string ElementName = "stl:inputtemplate";
        }

        public static string Parse(string stlElement, XmlNode node, PageInfo pageInfo, ContextInfo contextInfo)
        {
            var parsedContent = string.Empty;
            try
            {
                var theme = string.Empty;
                var isDynamic = false;

                var inputTemplateString = string.Empty;
                var successTemplateString = string.Empty;
                var failureTemplateString = string.Empty;
                StlParserUtility.GetInnerTemplateStringOfInput(node, out inputTemplateString, out successTemplateString, out failureTemplateString, pageInfo, contextInfo);

                var ie = node.Attributes.GetEnumerator();

                while (ie.MoveNext())
                {
                    var attr = (XmlAttribute)ie.Current;
                    var attributeName = attr.Name.ToLower();
                    if (attributeName.Equals(Attribute_Theme))
                    {
                        theme = attr.Value;
                    }
                    else if (attributeName.Equals(Attribute_IsDynamic))
                    {
                        isDynamic = TranslateUtils.ToBool(attr.Value);
                    }
                }

                pageInfo.AddPageScriptsIfNotExists(PageInfo.Components.Jquery);
                pageInfo.AddPageScriptsIfNotExists(PageInfo.JQuery.BjTemplates);
                pageInfo.AddPageScriptsIfNotExists(PageInfo.JQuery.BShowLoading);
                pageInfo.AddPageScriptsIfNotExists(PageInfo.JQuery.BValidate);

                pageInfo.AddPageScriptsIfNotExists("SiteServer.CMS.Parser.StlElement",
                    $@"<link href=""{SiteFilesAssets.Vote.GetStyleUrl(pageInfo.ApiUrl)}"" type=""text/css"" rel=""stylesheet"" />
");

                if (isDynamic)
                {
                    parsedContent = StlDynamic.ParseDynamicElement(stlElement, pageInfo, contextInfo);
                }
                else
                {
                    parsedContent = ParseImpl(pageInfo, contextInfo, theme, inputTemplateString);
                }
            }
            catch (Exception ex)
            {
                parsedContent = StlParserUtility.GetStlErrorMessage(ElementName, ex);
            }

            return parsedContent;
        }

        private static string ParseImpl(PageInfo pageInfo, ContextInfo contextInfo, string theme, string inputTemplateString)
        {
            var parsedContent = string.Empty;

            var contentInfo = contextInfo.ContentInfo as VoteContentInfo;
            if (contentInfo == null && contextInfo.ContentID > 0)
            {
                contentInfo = DataProvider.VoteContentDao.GetContentInfo(pageInfo.PublishmentSystemInfo, contextInfo.ContentID);
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
