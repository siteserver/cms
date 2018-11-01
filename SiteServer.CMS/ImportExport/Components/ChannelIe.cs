using System;
using System.Collections;
using System.Collections.Generic;
using Atom.AdditionalElements;
using Atom.Core;
using SiteServer.CMS.DataCache;
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

        public AtomFeed ExportNodeInfo(ChannelInfo channelInfo)
        {
            var feed = AtomUtility.GetEmptyFeed();

            AtomUtility.AddDcElement(feed.AdditionalElements, new List<string>{ ChannelAttribute.Id, "NodeId" }, channelInfo.Id.ToString());
            AtomUtility.AddDcElement(feed.AdditionalElements, new List<string> { ChannelAttribute.ChannelName, "NodeName" }, channelInfo.ChannelName);
            AtomUtility.AddDcElement(feed.AdditionalElements, new List<string> { ChannelAttribute.SiteId, "PublishmentSystemId" }, channelInfo.SiteId.ToString());
            AtomUtility.AddDcElement(feed.AdditionalElements, ChannelAttribute.ContentModelPluginId, channelInfo.ContentModelPluginId);
            AtomUtility.AddDcElement(feed.AdditionalElements, ChannelAttribute.ContentRelatedPluginIds, channelInfo.ContentRelatedPluginIds);
            AtomUtility.AddDcElement(feed.AdditionalElements, ChannelAttribute.ParentId, channelInfo.ParentId.ToString());
            AtomUtility.AddDcElement(feed.AdditionalElements, ChannelAttribute.ParentsPath, channelInfo.ParentsPath);
            AtomUtility.AddDcElement(feed.AdditionalElements, ChannelAttribute.ParentsCount, channelInfo.ParentsCount.ToString());
            AtomUtility.AddDcElement(feed.AdditionalElements, ChannelAttribute.ChildrenCount, channelInfo.ChildrenCount.ToString());
            AtomUtility.AddDcElement(feed.AdditionalElements, ChannelAttribute.IsLastNode, channelInfo.IsLastNode.ToString());
            AtomUtility.AddDcElement(feed.AdditionalElements, new List<string> { ChannelAttribute.IndexName, "NodeIndexName" }, channelInfo.IndexName);
            AtomUtility.AddDcElement(feed.AdditionalElements, new List<string> { ChannelAttribute.GroupNameCollection, "NodeGroupNameCollection" }, channelInfo.GroupNameCollection);
            AtomUtility.AddDcElement(feed.AdditionalElements, ChannelAttribute.Taxis, channelInfo.Taxis.ToString());
            AtomUtility.AddDcElement(feed.AdditionalElements, ChannelAttribute.AddDate, channelInfo.AddDate.ToLongDateString());
            AtomUtility.AddDcElement(feed.AdditionalElements, ChannelAttribute.ImageUrl, channelInfo.ImageUrl);
            AtomUtility.AddDcElement(feed.AdditionalElements, ChannelAttribute.Content, AtomUtility.Encrypt(channelInfo.Content));
            AtomUtility.AddDcElement(feed.AdditionalElements, ChannelAttribute.FilePath, channelInfo.FilePath);
            AtomUtility.AddDcElement(feed.AdditionalElements, ChannelAttribute.ChannelFilePathRule, channelInfo.ChannelFilePathRule);
            AtomUtility.AddDcElement(feed.AdditionalElements, ChannelAttribute.ContentFilePathRule, channelInfo.ContentFilePathRule);
            AtomUtility.AddDcElement(feed.AdditionalElements, ChannelAttribute.LinkUrl, channelInfo.LinkUrl);
            AtomUtility.AddDcElement(feed.AdditionalElements, ChannelAttribute.LinkType, channelInfo.LinkType);
            AtomUtility.AddDcElement(feed.AdditionalElements, ChannelAttribute.ChannelTemplateId, channelInfo.ChannelTemplateId.ToString());
            AtomUtility.AddDcElement(feed.AdditionalElements, ChannelAttribute.ContentTemplateId, channelInfo.ContentTemplateId.ToString());
            AtomUtility.AddDcElement(feed.AdditionalElements, ChannelAttribute.Keywords, channelInfo.Keywords);
            AtomUtility.AddDcElement(feed.AdditionalElements, ChannelAttribute.Description, channelInfo.Description);
            AtomUtility.AddDcElement(feed.AdditionalElements, ChannelAttribute.ExtendValues, channelInfo.Additional.ToString());

            if (channelInfo.ChannelTemplateId != 0)
            {
                var channelTemplateName = TemplateManager.GetTemplateName(channelInfo.SiteId, channelInfo.ChannelTemplateId);
                AtomUtility.AddDcElement(feed.AdditionalElements, ChannelTemplateName, channelTemplateName);
            }

            if (channelInfo.ContentTemplateId != 0)
            {
                var contentTemplateName = TemplateManager.GetTemplateName(channelInfo.SiteId, channelInfo.ContentTemplateId);
                AtomUtility.AddDcElement(feed.AdditionalElements, ContentTemplateName, contentTemplateName);
            }

            return feed;
        }
    }
}
