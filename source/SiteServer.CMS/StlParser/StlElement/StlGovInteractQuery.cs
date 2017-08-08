using System.Collections.Generic;
using System.Text;
using BaiRong.Core;
using SiteServer.CMS.Core;
using SiteServer.CMS.Model;
using SiteServer.CMS.Model.Enumerations;
using SiteServer.CMS.StlParser.Cache;
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

        public static string Parse(PageInfo pageInfo, ContextInfo contextInfo)
        {
            var channelId = string.Empty;
            var channelIndex = string.Empty;
            var channelName = string.Empty;
            var interactName = string.Empty;

            string inputTemplateString;
            string loadingTemplateString;
            string successTemplateString;
            string failureTemplateString;
            StlInnerUtility.GetTemplateLoadingYesNo(pageInfo, contextInfo.InnerXml, out inputTemplateString, out loadingTemplateString, out successTemplateString, out failureTemplateString);

            foreach (var name in contextInfo.Attributes.Keys)
            {
                var value = contextInfo.Attributes[name];

                if (StringUtils.EqualsIgnoreCase(name, AttributeChannelId))
                {
                    channelId = StlEntityParser.ReplaceStlEntitiesForAttributeValue(value, pageInfo, contextInfo);
                }
                else if (StringUtils.EqualsIgnoreCase(name, AttributeChannelIndex))
                {
                    channelIndex = StlEntityParser.ReplaceStlEntitiesForAttributeValue(value, pageInfo, contextInfo);
                }
                else if (StringUtils.EqualsIgnoreCase(name, AttributeChannelName))
                {
                    channelName = StlEntityParser.ReplaceStlEntitiesForAttributeValue(value, pageInfo, contextInfo);
                }
                else if (StringUtils.EqualsIgnoreCase(name, AttributeInteractName))
                {
                    interactName = StlEntityParser.ReplaceStlEntitiesForAttributeValue(value, pageInfo, contextInfo);
                }
            }

            return ParseImpl(pageInfo, contextInfo, TranslateUtils.ToInt(channelId), channelIndex, channelName, interactName, inputTemplateString, successTemplateString, failureTemplateString);
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
                //nodeId = DataProvider.GovInteractChannelDao.GetNodeIdByInteractName(pageInfo.PublishmentSystemId, interactName);
                nodeId = GovInteractChannel.GetNodeIdByInteractName(pageInfo.PublishmentSystemId, interactName, pageInfo.Guid);
            }
            if (nodeId == 0)
            {
                nodeId = Node.GetNodeIdByChannelIdOrChannelIndexOrChannelName(pageInfo.PublishmentSystemId, pageInfo.PublishmentSystemId, channelIndex, channelName, pageInfo.Guid);
            }
            var nodeInfo = NodeManager.GetNodeInfo(pageInfo.PublishmentSystemId, nodeId);
            if (nodeInfo == null || !EContentModelTypeUtils.Equals(nodeInfo.ContentModelId, EContentModelType.GovInteract))
            {
                nodeInfo = NodeManager.GetNodeInfo(pageInfo.PublishmentSystemId, pageInfo.PublishmentSystemInfo.Additional.GovInteractNodeId);
            }
            if (nodeInfo != null)
            {
                //var queryStyleId = DataProvider.GovInteractChannelDao.GetQueryStyleId(nodeInfo.PublishmentSystemId, nodeInfo.NodeId);
                var queryStyleId = GovInteractChannel.GetQueryStyleId(nodeInfo.PublishmentSystemId, nodeInfo.NodeId, pageInfo.Guid);

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
