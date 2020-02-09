using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Datory;
using Datory.Utils;
using SiteServer.CMS.Context.Atom.Atom.AdditionalElements;
using SiteServer.CMS.Context.Atom.Atom.Core;
using SiteServer.CMS.DataCache;
using SiteServer.Abstractions;
using SiteServer.CMS.Repositories;


namespace SiteServer.CMS.ImportExport.Components
{
    internal class ChannelIe
    {
        private readonly Site _site;

        //保存除内容表本身字段外的属性
        private const string ChannelTemplateName = "ChannelTemplateName";
        private const string ContentTemplateName = "ContentTemplateName";

        public ChannelIe(Site site)
        {
            _site = site;
        }

        public async Task ImportNodeInfoAsync(Channel node, ScopedElementCollection additionalElements, int parentId, IList indexNameList)
        {
            node.ChannelName = AtomUtility.GetDcElementContent(additionalElements, new List<string>{ nameof(Channel.ChannelName), "NodeName" });
            node.SiteId = _site.Id;
            var contentModelPluginId = AtomUtility.GetDcElementContent(additionalElements, nameof(Channel.ContentModelPluginId));
            if (!string.IsNullOrEmpty(contentModelPluginId))
            {
                node.ContentModelPluginId = contentModelPluginId;
            }
            var contentRelatedPluginIdList = Utilities.GetStringList(AtomUtility.GetDcElementContent(additionalElements, nameof(Channel.ContentRelatedPluginIds)));
            node.ContentRelatedPluginIds = contentRelatedPluginIdList;
            node.ParentId = parentId;
            var indexName = AtomUtility.GetDcElementContent(additionalElements, new List<string> { nameof(Channel.IndexName), "NodeIndexName" });
            if (!string.IsNullOrEmpty(indexName) && indexNameList.IndexOf(indexName) == -1)
            {
                node.IndexName = indexName;
                indexNameList.Add(indexName);
            }
            node.GroupNames = Utilities.GetStringList(AtomUtility.GetDcElementContent(additionalElements, new List<string> { nameof(Channel.GroupNames), "NodeGroupNameCollection" }));
            node.AddDate = DateTime.Now;
            node.ImageUrl = AtomUtility.GetDcElementContent(additionalElements, nameof(Channel.ImageUrl));
            node.Content = AtomUtility.Decrypt(AtomUtility.GetDcElementContent(additionalElements, nameof(Channel.Content)));
            node.FilePath = AtomUtility.GetDcElementContent(additionalElements, nameof(Channel.FilePath));
            node.ChannelFilePathRule = AtomUtility.GetDcElementContent(additionalElements, nameof(Channel.ChannelFilePathRule));
            node.ContentFilePathRule = AtomUtility.GetDcElementContent(additionalElements, nameof(Channel.ContentFilePathRule));

            node.LinkUrl = AtomUtility.GetDcElementContent(additionalElements, nameof(Channel.LinkUrl));
            node.LinkType = TranslateUtils.ToEnum(AtomUtility.GetDcElementContent(additionalElements, nameof(Channel.LinkType)), LinkType.None);

            var channelTemplateName = AtomUtility.GetDcElementContent(additionalElements, ChannelTemplateName);
            if (!string.IsNullOrEmpty(channelTemplateName))
            {
                node.ChannelTemplateId = await DataProvider.TemplateRepository.GetTemplateIdByTemplateNameAsync(_site.Id, TemplateType.ChannelTemplate, channelTemplateName);
            }
            var contentTemplateName = AtomUtility.GetDcElementContent(additionalElements, ContentTemplateName);
            if (!string.IsNullOrEmpty(contentTemplateName))
            {
                node.ContentTemplateId = await DataProvider.TemplateRepository.GetTemplateIdByTemplateNameAsync(_site.Id, TemplateType.ContentTemplate, contentTemplateName);
            }

            node.Keywords = AtomUtility.GetDcElementContent(additionalElements, nameof(Channel.Keywords));
            node.Description = AtomUtility.GetDcElementContent(additionalElements, nameof(Channel.Description));

            node.LoadExtend();

            var json = AtomUtility.GetDcElementContent(additionalElements, "ExtendValues");
        }

        public async Task<AtomFeed> ExportNodeInfoAsync(Channel channel)
        {
            var feed = AtomUtility.GetEmptyFeed();

            AtomUtility.AddDcElement(feed.AdditionalElements, new List<string>{ nameof(Channel.Id), "NodeId" }, channel.Id.ToString());
            AtomUtility.AddDcElement(feed.AdditionalElements, new List<string> { nameof(Channel.ChannelName), "NodeName" }, channel.ChannelName);
            AtomUtility.AddDcElement(feed.AdditionalElements, new List<string> { nameof(Channel.SiteId), "PublishmentSystemId" }, channel.SiteId.ToString());
            AtomUtility.AddDcElement(feed.AdditionalElements, nameof(Channel.ContentModelPluginId), channel.ContentModelPluginId);
            AtomUtility.AddDcElement(feed.AdditionalElements, nameof(Channel.ContentRelatedPluginIds), Utilities.ToString(channel.ContentRelatedPluginIds));
            AtomUtility.AddDcElement(feed.AdditionalElements, nameof(Channel.ParentId), channel.ParentId.ToString());
            AtomUtility.AddDcElement(feed.AdditionalElements, nameof(Channel.ParentsPath), channel.ParentsPath);
            AtomUtility.AddDcElement(feed.AdditionalElements, nameof(Channel.ParentsCount), channel.ParentsCount.ToString());
            AtomUtility.AddDcElement(feed.AdditionalElements, nameof(Channel.ChildrenCount), channel.ChildrenCount.ToString());
            AtomUtility.AddDcElement(feed.AdditionalElements, new List<string> { nameof(Channel.IndexName), "NodeIndexName" }, channel.IndexName);
            AtomUtility.AddDcElement(feed.AdditionalElements, new List<string> { nameof(Channel.GroupNames), "NodeGroupNameCollection" }, Utilities.ToString(channel.GroupNames));
            AtomUtility.AddDcElement(feed.AdditionalElements, nameof(Channel.Taxis), channel.Taxis.ToString());
            if (channel.AddDate.HasValue)
            {
                AtomUtility.AddDcElement(feed.AdditionalElements, nameof(Channel.AddDate), channel.AddDate.Value.ToLongDateString());
            }
            AtomUtility.AddDcElement(feed.AdditionalElements, nameof(Channel.ImageUrl), channel.ImageUrl);
            AtomUtility.AddDcElement(feed.AdditionalElements, nameof(Channel.Content), AtomUtility.Encrypt(channel.Content));
            AtomUtility.AddDcElement(feed.AdditionalElements, nameof(Channel.FilePath), channel.FilePath);
            AtomUtility.AddDcElement(feed.AdditionalElements, nameof(Channel.ChannelFilePathRule), channel.ChannelFilePathRule);
            AtomUtility.AddDcElement(feed.AdditionalElements, nameof(Channel.ContentFilePathRule), channel.ContentFilePathRule);
            AtomUtility.AddDcElement(feed.AdditionalElements, nameof(Channel.LinkUrl), channel.LinkUrl);
            AtomUtility.AddDcElement(feed.AdditionalElements, nameof(Channel.LinkType), channel.LinkType.GetValue());
            AtomUtility.AddDcElement(feed.AdditionalElements, nameof(Channel.ChannelTemplateId), channel.ChannelTemplateId.ToString());
            AtomUtility.AddDcElement(feed.AdditionalElements, nameof(Channel.ContentTemplateId), channel.ContentTemplateId.ToString());
            AtomUtility.AddDcElement(feed.AdditionalElements, nameof(Channel.Keywords), channel.Keywords);
            AtomUtility.AddDcElement(feed.AdditionalElements, nameof(Channel.Description), channel.Description);

            var json = AtomUtility.GetDcElementContent(feed.AdditionalElements,
                "ExtendValues");
            if (!string.IsNullOrEmpty(json))
            {
                var dict = Utilities.ToDictionary(json);
                foreach (var o in dict)
                {
                    channel.Set(o.Key, o.Value);
                }
            }

            if (channel.ChannelTemplateId != 0)
            {
                var channelTemplateName = await DataProvider.TemplateRepository.GetTemplateNameAsync(channel.ChannelTemplateId);
                AtomUtility.AddDcElement(feed.AdditionalElements, ChannelTemplateName, channelTemplateName);
            }

            if (channel.ContentTemplateId != 0)
            {
                var contentTemplateName = await DataProvider.TemplateRepository.GetTemplateNameAsync(channel.ContentTemplateId);
                AtomUtility.AddDcElement(feed.AdditionalElements, ContentTemplateName, contentTemplateName);
            }

            return feed;
        }
    }
}
