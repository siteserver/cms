using System;
using System.Collections;
using System.Collections.Generic;
using Atom.AdditionalElements;
using Atom.Core;
using SiteServer.CMS.Core;
using SiteServer.CMS.Model;
using SiteServer.CMS.Model.Attributes;
using SiteServer.Plugin;

namespace SiteServer.CMS.ImportExport.Components
{
    internal class ChannelIe
    {
        private readonly SiteInfo _siteInfo;

        //保存除内容表本身字段外的属性
        private const string ChannelTemplateName = "ChannelTemplateName";
        private const string ContentTemplateName = "ContentTemplateName";

        public ChannelIe(SiteInfo siteInfo)
        {
            _siteInfo = siteInfo;
        }

        public void ImportNodeInfo(ChannelInfo nodeInfo, ScopedElementCollection additionalElements, int parentId, IList indexNameList)
        {
            nodeInfo.ChannelName = AtomUtility.GetDcElementContent(additionalElements, new List<string>{ ChannelAttribute.ChannelName, "NodeName" });
            nodeInfo.SiteId = _siteInfo.Id;
            var contentModelPluginId = AtomUtility.GetDcElementContent(additionalElements, ChannelAttribute.ContentModelPluginId);
            if (!string.IsNullOrEmpty(contentModelPluginId))
            {
                nodeInfo.ContentModelPluginId = contentModelPluginId;
            }
            var contentRelatedPluginIds = AtomUtility.GetDcElementContent(additionalElements, ChannelAttribute.ContentRelatedPluginIds);
            if (!string.IsNullOrEmpty(contentRelatedPluginIds))
            {
                nodeInfo.ContentRelatedPluginIds = contentRelatedPluginIds;
            }
            nodeInfo.ParentId = parentId;
            var indexName = AtomUtility.GetDcElementContent(additionalElements, new List<string> { ChannelAttribute.IndexName, "NodeIndexName" });
            if (!string.IsNullOrEmpty(indexName) && indexNameList.IndexOf(indexName) == -1)
            {
                nodeInfo.IndexName = indexName;
                indexNameList.Add(indexName);
            }
            nodeInfo.GroupNameCollection = AtomUtility.GetDcElementContent(additionalElements, new List<string> { ChannelAttribute.GroupNameCollection, "NodeGroupNameCollection" });
            nodeInfo.AddDate = DateTime.Now;
            nodeInfo.ImageUrl = AtomUtility.GetDcElementContent(additionalElements, ChannelAttribute.ImageUrl);
            nodeInfo.Content = AtomUtility.Decrypt(AtomUtility.GetDcElementContent(additionalElements, ChannelAttribute.Content));
            nodeInfo.FilePath = AtomUtility.GetDcElementContent(additionalElements, ChannelAttribute.FilePath);
            nodeInfo.ChannelFilePathRule = AtomUtility.GetDcElementContent(additionalElements, ChannelAttribute.ChannelFilePathRule);
            nodeInfo.ContentFilePathRule = AtomUtility.GetDcElementContent(additionalElements, ChannelAttribute.ContentFilePathRule);

            nodeInfo.LinkUrl = AtomUtility.GetDcElementContent(additionalElements, ChannelAttribute.LinkUrl);
            nodeInfo.LinkType = AtomUtility.GetDcElementContent(additionalElements, ChannelAttribute.LinkType);

            var channelTemplateName = AtomUtility.GetDcElementContent(additionalElements, ChannelTemplateName);
            if (!string.IsNullOrEmpty(channelTemplateName))
            {
                nodeInfo.ChannelTemplateId = TemplateManager.GetTemplateIdByTemplateName(_siteInfo.Id, TemplateType.ChannelTemplate, channelTemplateName);
            }
            var contentTemplateName = AtomUtility.GetDcElementContent(additionalElements, ContentTemplateName);
            if (!string.IsNullOrEmpty(contentTemplateName))
            {
                nodeInfo.ContentTemplateId = TemplateManager.GetTemplateIdByTemplateName(_siteInfo.Id, TemplateType.ContentTemplate, contentTemplateName);
            }

            nodeInfo.Keywords = AtomUtility.GetDcElementContent(additionalElements, ChannelAttribute.Keywords);
            nodeInfo.Description = AtomUtility.GetDcElementContent(additionalElements, ChannelAttribute.Description);

            nodeInfo.SetExtendValues(AtomUtility.GetDcElementContent(additionalElements, ChannelAttribute.ExtendValues));
        }

        public AtomFeed ExportNodeInfo(ChannelInfo nodeInfo)
        {
            var feed = AtomUtility.GetEmptyFeed();

            AtomUtility.AddDcElement(feed.AdditionalElements, new List<string>{ ChannelAttribute.Id, "NodeId" }, nodeInfo.Id.ToString());
            AtomUtility.AddDcElement(feed.AdditionalElements, new List<string> { ChannelAttribute.ChannelName, "NodeName" }, nodeInfo.ChannelName);
            AtomUtility.AddDcElement(feed.AdditionalElements, new List<string> { ChannelAttribute.SiteId, "PublishmentSystemId" }, nodeInfo.SiteId.ToString());
            AtomUtility.AddDcElement(feed.AdditionalElements, ChannelAttribute.ContentModelPluginId, nodeInfo.ContentModelPluginId);
            AtomUtility.AddDcElement(feed.AdditionalElements, ChannelAttribute.ContentRelatedPluginIds, nodeInfo.ContentRelatedPluginIds);
            AtomUtility.AddDcElement(feed.AdditionalElements, ChannelAttribute.ParentId, nodeInfo.ParentId.ToString());
            AtomUtility.AddDcElement(feed.AdditionalElements, ChannelAttribute.ParentsPath, nodeInfo.ParentsPath);
            AtomUtility.AddDcElement(feed.AdditionalElements, ChannelAttribute.ParentsCount, nodeInfo.ParentsCount.ToString());
            AtomUtility.AddDcElement(feed.AdditionalElements, ChannelAttribute.ChildrenCount, nodeInfo.ChildrenCount.ToString());
            AtomUtility.AddDcElement(feed.AdditionalElements, ChannelAttribute.IsLastNode, nodeInfo.IsLastNode.ToString());
            AtomUtility.AddDcElement(feed.AdditionalElements, new List<string> { ChannelAttribute.IndexName, "NodeIndexName" }, nodeInfo.IndexName);
            AtomUtility.AddDcElement(feed.AdditionalElements, new List<string> { ChannelAttribute.GroupNameCollection, "NodeGroupNameCollection" }, nodeInfo.GroupNameCollection);
            AtomUtility.AddDcElement(feed.AdditionalElements, ChannelAttribute.Taxis, nodeInfo.Taxis.ToString());
            AtomUtility.AddDcElement(feed.AdditionalElements, ChannelAttribute.AddDate, nodeInfo.AddDate.ToLongDateString());
            AtomUtility.AddDcElement(feed.AdditionalElements, ChannelAttribute.ImageUrl, nodeInfo.ImageUrl);
            AtomUtility.AddDcElement(feed.AdditionalElements, ChannelAttribute.Content, AtomUtility.Encrypt(nodeInfo.Content));
            AtomUtility.AddDcElement(feed.AdditionalElements, ChannelAttribute.ContentNum, nodeInfo.ContentNum.ToString());
            AtomUtility.AddDcElement(feed.AdditionalElements, ChannelAttribute.FilePath, nodeInfo.FilePath);
            AtomUtility.AddDcElement(feed.AdditionalElements, ChannelAttribute.ChannelFilePathRule, nodeInfo.ChannelFilePathRule);
            AtomUtility.AddDcElement(feed.AdditionalElements, ChannelAttribute.ContentFilePathRule, nodeInfo.ContentFilePathRule);
            AtomUtility.AddDcElement(feed.AdditionalElements, ChannelAttribute.LinkUrl, nodeInfo.LinkUrl);
            AtomUtility.AddDcElement(feed.AdditionalElements, ChannelAttribute.LinkType, nodeInfo.LinkType);
            AtomUtility.AddDcElement(feed.AdditionalElements, ChannelAttribute.ChannelTemplateId, nodeInfo.ChannelTemplateId.ToString());
            AtomUtility.AddDcElement(feed.AdditionalElements, ChannelAttribute.ContentTemplateId, nodeInfo.ContentTemplateId.ToString());
            AtomUtility.AddDcElement(feed.AdditionalElements, ChannelAttribute.Keywords, nodeInfo.Keywords);
            AtomUtility.AddDcElement(feed.AdditionalElements, ChannelAttribute.Description, nodeInfo.Description);
            AtomUtility.AddDcElement(feed.AdditionalElements, ChannelAttribute.ExtendValues, nodeInfo.Additional.ToString());

            if (nodeInfo.ChannelTemplateId != 0)
            {
                var channelTemplateName = TemplateManager.GetTemplateName(nodeInfo.SiteId, nodeInfo.ChannelTemplateId);
                AtomUtility.AddDcElement(feed.AdditionalElements, ChannelTemplateName, channelTemplateName);
            }

            if (nodeInfo.ContentTemplateId != 0)
            {
                var contentTemplateName = TemplateManager.GetTemplateName(nodeInfo.SiteId, nodeInfo.ContentTemplateId);
                AtomUtility.AddDcElement(feed.AdditionalElements, ContentTemplateName, contentTemplateName);
            }

            return feed;
        }
    }
}
