using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Datory;
using SSCMS.Core.Utils.Serialization.Atom.Atom.AdditionalElements;
using SSCMS.Core.Utils.Serialization.Atom.Atom.Core;
using SSCMS.Enums;
using SSCMS.Models;
using SSCMS.Services;
using SSCMS.Utils;

namespace SSCMS.Core.Utils.Serialization.Components
{
    internal class ChannelIe
    {
        private const string ChannelTemplateName = "ChannelTemplateName";
        private const string ContentTemplateName = "ContentTemplateName";

        private readonly IDatabaseManager _databaseManager;
        private readonly Site _site;

        public ChannelIe(IDatabaseManager databaseManager, Site site)
        {
            _databaseManager = databaseManager;
            _site = site;
        }

        public async Task ImportChannelAsync(Channel channel, ScopedElementCollection additionalElements, int parentId, IList indexNameList)
        {
            channel.LoadExtend(AtomUtility.GetDcElementContent(additionalElements, "ExtendValues"));

            channel.ChannelName = AtomUtility.GetDcElementContent(additionalElements, new List<string>{ nameof(Channel.ChannelName), "NodeName" });
            channel.SiteId = _site.Id;
            var contentModelPluginId = AtomUtility.GetDcElementContent(additionalElements, nameof(Channel.ContentModelPluginId));
            if (!string.IsNullOrEmpty(contentModelPluginId))
            {
                channel.ContentModelPluginId = contentModelPluginId;
            }
            channel.ParentId = parentId;
            var indexName = AtomUtility.GetDcElementContent(additionalElements, new List<string> { nameof(Channel.IndexName), "NodeIndexName" });
            if (!string.IsNullOrEmpty(indexName) && indexNameList.IndexOf(indexName) == -1)
            {
                channel.IndexName = indexName;
                indexNameList.Add(indexName);
            }
            channel.GroupNames = ListUtils.GetStringList(AtomUtility.GetDcElementContent(additionalElements, new List<string> { nameof(Channel.GroupNames), "NodeGroupNameCollection" }));
            channel.AddDate = DateTime.Now;
            channel.ImageUrl = AtomUtility.GetDcElementContent(additionalElements, nameof(Channel.ImageUrl));
            channel.Content = AtomUtility.Decrypt(AtomUtility.GetDcElementContent(additionalElements, nameof(Channel.Content)));
            channel.FilePath = AtomUtility.GetDcElementContent(additionalElements, nameof(Channel.FilePath));
            channel.ChannelFilePathRule = AtomUtility.GetDcElementContent(additionalElements, nameof(Channel.ChannelFilePathRule));
            channel.ContentFilePathRule = AtomUtility.GetDcElementContent(additionalElements, nameof(Channel.ContentFilePathRule));

            channel.LinkUrl = AtomUtility.GetDcElementContent(additionalElements, nameof(Channel.LinkUrl));
            channel.LinkType = TranslateUtils.ToEnum(AtomUtility.GetDcElementContent(additionalElements, nameof(Channel.LinkType)), LinkType.None);

            var channelTemplateName = AtomUtility.GetDcElementContent(additionalElements, ChannelTemplateName);
            if (!string.IsNullOrEmpty(channelTemplateName))
            {
                channel.ChannelTemplateId = await _databaseManager.TemplateRepository.GetTemplateIdByTemplateNameAsync(_site.Id, TemplateType.ChannelTemplate, channelTemplateName);
            }
            var contentTemplateName = AtomUtility.GetDcElementContent(additionalElements, ContentTemplateName);
            if (!string.IsNullOrEmpty(contentTemplateName))
            {
                channel.ContentTemplateId = await _databaseManager.TemplateRepository.GetTemplateIdByTemplateNameAsync(_site.Id, TemplateType.ContentTemplate, contentTemplateName);
            }

            channel.Keywords = AtomUtility.GetDcElementContent(additionalElements, nameof(Channel.Keywords));
            channel.Description = AtomUtility.GetDcElementContent(additionalElements, nameof(Channel.Description));
        }

        public async Task<AtomFeed> ExportChannelAsync(Channel channel)
        {
            var feed = AtomUtility.GetEmptyFeed();

            AtomUtility.AddDcElement(feed.AdditionalElements, new List<string>{ nameof(Channel.Id), "NodeId" }, channel.Id.ToString());
            AtomUtility.AddDcElement(feed.AdditionalElements, new List<string> { nameof(Channel.ChannelName), "NodeName" }, channel.ChannelName);
            AtomUtility.AddDcElement(feed.AdditionalElements, new List<string> { nameof(Channel.SiteId), "PublishmentSystemId" }, channel.SiteId.ToString());
            AtomUtility.AddDcElement(feed.AdditionalElements, nameof(Channel.ContentModelPluginId), channel.ContentModelPluginId);
            AtomUtility.AddDcElement(feed.AdditionalElements, nameof(Channel.ParentId), channel.ParentId.ToString());
            AtomUtility.AddDcElement(feed.AdditionalElements, nameof(Channel.ParentsPath), ListUtils.ToString(channel.ParentsPath));
            AtomUtility.AddDcElement(feed.AdditionalElements, nameof(Channel.ParentsCount), channel.ParentsCount.ToString());
            AtomUtility.AddDcElement(feed.AdditionalElements, nameof(Channel.ChildrenCount), channel.ChildrenCount.ToString());
            AtomUtility.AddDcElement(feed.AdditionalElements, new List<string> { nameof(Channel.IndexName), "NodeIndexName" }, channel.IndexName);
            AtomUtility.AddDcElement(feed.AdditionalElements, new List<string> { nameof(Channel.GroupNames), "NodeGroupNameCollection" }, ListUtils.ToString(channel.GroupNames));
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
            AtomUtility.AddDcElement(feed.AdditionalElements, "ExtendValues", TranslateUtils.JsonSerialize(channel.ToDictionary()));

            //var json = AtomUtility.GetDcElementContent(feed.AdditionalElements,
            //    "ExtendValues");
            //if (!string.IsNullOrEmpty(json))
            //{
            //    var dict = ListUtils.ToDictionary(json);
            //    foreach (var o in dict)
            //    {
            //        channel.Set(o.Key, o.Value);
            //    }
            //}

            if (channel.ChannelTemplateId != 0)
            {
                var channelTemplateName = await _databaseManager.TemplateRepository.GetTemplateNameAsync(channel.ChannelTemplateId);
                AtomUtility.AddDcElement(feed.AdditionalElements, ChannelTemplateName, channelTemplateName);
            }

            if (channel.ContentTemplateId != 0)
            {
                var contentTemplateName = await _databaseManager.TemplateRepository.GetTemplateNameAsync(channel.ContentTemplateId);
                AtomUtility.AddDcElement(feed.AdditionalElements, ContentTemplateName, contentTemplateName);
            }

            return feed;
        }
    }
}
