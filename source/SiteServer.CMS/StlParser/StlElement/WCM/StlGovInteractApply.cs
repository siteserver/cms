using System;
using System.Collections.Specialized;
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

namespace SiteServer.CMS.StlParser.StlElement.WCM
{
	public class StlGovInteractApply
	{
        private StlGovInteractApply() { }
        public const string ElementName = "stl:govinteractapply";     //互动交流提交

        public const string Attribute_ChannelID = "channelid";			        //栏目ID
        public const string Attribute_ChannelIndex = "channelindex";			//栏目索引
        public const string Attribute_ChannelName = "channelname";				//栏目名称
        public const string Attribute_InteractName = "interactname";				//互动交流名称

		public static ListDictionary AttributeList
		{
			get
			{
				var attributes = new ListDictionary();
                attributes.Add(Attribute_ChannelID, "栏目ID");
                attributes.Add(Attribute_ChannelIndex, "栏目索引");
                attributes.Add(Attribute_ChannelName, "栏目名称");
                attributes.Add(Attribute_InteractName, "互动交流名称");

				return attributes;
			}
		}

        public static string Parse(string stlElement, XmlNode node, PageInfo pageInfo, ContextInfo contextInfo)
        {
            var parsedContent = string.Empty;
            try
            {
                var channelID = string.Empty;
                var channelIndex = string.Empty;
                var channelName = string.Empty;
                var interactName = string.Empty;
                var applyInfo = new TagStyleGovInteractApplyInfo(string.Empty);

                var inputTemplateString = string.Empty;
                var successTemplateString = string.Empty;
                var failureTemplateString = string.Empty;
                StlParserUtility.GetInnerTemplateStringOfInput(node, out inputTemplateString, out successTemplateString, out failureTemplateString, pageInfo, contextInfo);

                var ie = node.Attributes.GetEnumerator();

                while (ie.MoveNext())
                {
                    var attr = (XmlAttribute)ie.Current;
                    var attributeName = attr.Name.ToLower();
                    if (attributeName.Equals(Attribute_ChannelID))
                    {
                        channelID = StlEntityParser.ReplaceStlEntitiesForAttributeValue(attr.Value, pageInfo, contextInfo);
                    }
                    else if (attributeName.Equals(Attribute_ChannelIndex))
                    {
                        channelIndex = StlEntityParser.ReplaceStlEntitiesForAttributeValue(attr.Value, pageInfo, contextInfo);
                    }
                    else if (attributeName.Equals(Attribute_ChannelName))
                    {
                        channelName = StlEntityParser.ReplaceStlEntitiesForAttributeValue(attr.Value, pageInfo, contextInfo);
                    }
                    else if (attributeName.Equals(Attribute_InteractName))
                    {
                        interactName = StlEntityParser.ReplaceStlEntitiesForAttributeValue(attr.Value, pageInfo, contextInfo);
                    }
                }

                parsedContent = ParseImpl(pageInfo, contextInfo, TranslateUtils.ToInt(channelID), channelIndex, channelName, interactName, inputTemplateString, successTemplateString, failureTemplateString);
            }
            catch (Exception ex)
            {
                parsedContent = StlParserUtility.GetStlErrorMessage(ElementName, ex);
            }

            return parsedContent;
        }

        public static string ParseImpl(PageInfo pageInfo, ContextInfo contextInfo, int channelID, string channelIndex, string channelName, string interactName, string inputTemplateString, string successTemplateString, string failureTemplateString)
        {
            var parsedContent = string.Empty;

            pageInfo.AddPageScriptsIfNotExists(PageInfo.Components.Jquery);
            pageInfo.AddPageScriptsIfNotExists(PageInfo.JQuery.BShowLoading);
            pageInfo.AddPageScriptsIfNotExists(PageInfo.JQuery.BValidate);

            var nodeID = channelID;
            if (!string.IsNullOrEmpty(interactName))
            {
                nodeID = DataProvider.GovInteractChannelDao.GetNodeIdByInteractName(pageInfo.PublishmentSystemId, interactName);
            }
            if (nodeID == 0)
            {
                nodeID = StlCacheManager.NodeId.GetNodeIdByChannelIdOrChannelIndexOrChannelName(pageInfo.PublishmentSystemId, pageInfo.PublishmentSystemId, channelIndex, channelName);
            }
            var nodeInfo = NodeManager.GetNodeInfo(pageInfo.PublishmentSystemId, nodeID);
            if (nodeInfo == null || !EContentModelTypeUtils.Equals(nodeInfo.ContentModelId, EContentModelType.GovInteract))
            {
                nodeInfo = NodeManager.GetNodeInfo(pageInfo.PublishmentSystemId, pageInfo.PublishmentSystemInfo.Additional.GovInteractNodeId);
            }
            if (nodeInfo != null)
            {
                var applyStyleID = DataProvider.GovInteractChannelDao.GetApplyStyleId(nodeInfo.PublishmentSystemId, nodeInfo.NodeId);

                var styleInfo = TagStyleManager.GetTagStyleInfo(applyStyleID);
                if (styleInfo == null)
                {
                    styleInfo = new TagStyleInfo();
                }
                var applyInfo = new TagStyleGovInteractApplyInfo(styleInfo.SettingsXML);

                var applyTemplate = new GovInteractApplyTemplate(pageInfo.PublishmentSystemInfo, nodeInfo.NodeId, styleInfo, applyInfo);
                var contentBuilder = new StringBuilder(applyTemplate.GetTemplate(styleInfo.IsTemplate, inputTemplateString, successTemplateString, failureTemplateString));
                contentBuilder.Replace("{ChannelID}", nodeInfo.NodeId.ToString());

                StlParserManager.ParseTemplateContent(contentBuilder, pageInfo, contextInfo);
                parsedContent = contentBuilder.ToString();
            }

            return parsedContent;
        }
	}
}
