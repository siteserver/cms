using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using BaiRong.Core;
using BaiRong.Core.Model.Enumerations;
using SiteServer.CMS.Core;
using SiteServer.CMS.Model;
using SiteServer.CMS.StlParser.Model;
using SiteServer.CMS.StlParser.Parser;
using SiteServer.CMS.StlParser.Utility;
using SiteServer.CMS.StlTemplates;

namespace SiteServer.CMS.StlParser.StlElement
{
    [Stl(Usage = "互动交流查询", Description = "通过 stl:govInteractQuery 标签在模板中实现互动交流查询功能")]
    public class StlGovInteractQuery
	{
        private StlGovInteractQuery() { }
        public const string ElementName = "stl:govInteractQuery";

        public const string AttributeChannelId = "channelId";
        public const string AttributeChannelIndex = "channelIndex";
        public const string AttributeChannelName = "channelName";
        public const string AttributeInteractName = "interactName";

	    public static SortedList<string, string> AttributeList => new SortedList<string, string>
	    {
	        {AttributeChannelId, "栏目Id"},
	        {AttributeChannelIndex, "栏目索引"},
	        {AttributeChannelName, "栏目名称"},
	        {AttributeInteractName, "互动交流名称"}
	    };

        public static string Parse(string stlElement, XmlNode node, PageInfo pageInfo, ContextInfo contextInfo)
        {
            string parsedContent;
            try
            {
                var channelId = string.Empty;
                var channelIndex = string.Empty;
                var channelName = string.Empty;
                var interactName = string.Empty;

                string inputTemplateString;
                string successTemplateString;
                string failureTemplateString;
                StlParserUtility.GetInnerTemplateStringOfInput(node, out inputTemplateString, out successTemplateString, out failureTemplateString, pageInfo, contextInfo);

                var ie = node.Attributes?.GetEnumerator();
                if (ie != null)
                {
                    while (ie.MoveNext())
                    {
                        var attr = (XmlAttribute)ie.Current;

                        if (StringUtils.EqualsIgnoreCase(attr.Name, AttributeChannelId))
                        {
                            channelId = StlEntityParser.ReplaceStlEntitiesForAttributeValue(attr.Value, pageInfo, contextInfo);
                        }
                        else if (StringUtils.EqualsIgnoreCase(attr.Name, AttributeChannelIndex))
                        {
                            channelIndex = StlEntityParser.ReplaceStlEntitiesForAttributeValue(attr.Value, pageInfo, contextInfo);
                        }
                        else if (StringUtils.EqualsIgnoreCase(attr.Name, AttributeChannelName))
                        {
                            channelName = StlEntityParser.ReplaceStlEntitiesForAttributeValue(attr.Value, pageInfo, contextInfo);
                        }
                        else if (StringUtils.EqualsIgnoreCase(attr.Name, AttributeInteractName))
                        {
                            interactName = StlEntityParser.ReplaceStlEntitiesForAttributeValue(attr.Value, pageInfo, contextInfo);
                        }
                    }
                }

                parsedContent = ParseImpl(pageInfo, contextInfo, TranslateUtils.ToInt(channelId), channelIndex, channelName, interactName, inputTemplateString, successTemplateString, failureTemplateString);
            }
            catch (Exception ex)
            {
                parsedContent = StlParserUtility.GetStlErrorMessage(ElementName, ex);
            }

            return parsedContent;
        }

        public static string ParseImpl(PageInfo pageInfo, ContextInfo contextInfo, int channelId, string channelIndex, string channelName, string interactName, string inputTemplateString, string successTemplateString, string failureTemplateString)
        {
            var parsedContent = string.Empty;

            pageInfo.AddPageScriptsIfNotExists(PageInfo.Components.Jquery);
            pageInfo.AddPageScriptsIfNotExists(PageInfo.JQuery.BShowLoading);
            pageInfo.AddPageScriptsIfNotExists(PageInfo.JQuery.BjTemplates);
            pageInfo.AddPageScriptsIfNotExists(PageInfo.JQuery.BValidate);

            var nodeId = channelId;
            if (!string.IsNullOrEmpty(interactName))
            {
                nodeId = DataProvider.GovInteractChannelDao.GetNodeIdByInteractName(pageInfo.PublishmentSystemId, interactName);
            }
            if (nodeId == 0)
            {
                nodeId = StlCacheManager.NodeId.GetNodeIdByChannelIdOrChannelIndexOrChannelName(pageInfo.PublishmentSystemId, pageInfo.PublishmentSystemId, channelIndex, channelName);
            }
            var nodeInfo = NodeManager.GetNodeInfo(pageInfo.PublishmentSystemId, nodeId);
            if (nodeInfo == null || !EContentModelTypeUtils.Equals(nodeInfo.ContentModelId, EContentModelType.GovInteract))
            {
                nodeInfo = NodeManager.GetNodeInfo(pageInfo.PublishmentSystemId, pageInfo.PublishmentSystemInfo.Additional.GovInteractNodeId);
            }
            if (nodeInfo != null)
            {
                var queryStyleId = DataProvider.GovInteractChannelDao.GetQueryStyleId(nodeInfo.PublishmentSystemId, nodeInfo.NodeId);

                var styleInfo = TagStyleManager.GetTagStyleInfo(queryStyleId) ?? new TagStyleInfo();

                var queryTemplate = new GovInteractQueryTemplate(pageInfo.PublishmentSystemInfo, nodeInfo.NodeId, styleInfo);
                var contentBuilder = new StringBuilder(queryTemplate.GetTemplate(styleInfo.IsTemplate, inputTemplateString, successTemplateString, failureTemplateString));

                StlParserManager.ParseTemplateContent(contentBuilder, pageInfo, contextInfo);
                parsedContent = contentBuilder.ToString();
            }

            return parsedContent;
        }
	}
}
