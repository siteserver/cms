using System;
using System.Collections.Generic;
using SS.CMS.Core.Models.Attributes;
using SS.CMS.Enums;
using SS.CMS.Models;
using SS.CMS.Repositories;
using SS.CMS.Utils;
using SS.CMS.Utils.Atom.Atom.Core;

namespace SS.CMS.Core.Serialization.Components
{
    public class ConfigurationIe
    {
        private readonly int _siteId;
        private readonly string _filePath;
        private readonly ISiteRepository _siteRepository;
        private readonly IChannelGroupRepository _channelGroupRepository;
        private readonly IContentGroupRepository _contentGroupRepository;
        private readonly ITemplateRepository _templateRepository;

        private const string DefaultIndexTemplateName = "DefaultIndexTemplateName";
        private const string DefaultChannelTemplateName = "DefaultChannelTemplateName";
        private const string DefaultContentTemplateName = "DefaultContentTemplateName";
        private const string DefaultFileTemplateName = "DefaultFileTemplateName";

        public ConfigurationIe(int siteId, string filePath)
        {
            _siteId = siteId;
            _filePath = filePath;
        }

        public void Export()
        {
            var siteInfo = _siteRepository.GetSiteInfo(_siteId);

            var feed = AtomUtility.GetEmptyFeed();

            AtomUtility.AddDcElement(feed.AdditionalElements, new List<string> { SiteAttribute.Id, "PublishmentSystemId" }, siteInfo.Id.ToString());
            AtomUtility.AddDcElement(feed.AdditionalElements, new List<string> { SiteAttribute.SiteName, "PublishmentSystemName" }, siteInfo.SiteName);
            AtomUtility.AddDcElement(feed.AdditionalElements, new List<string> { SiteAttribute.SiteDir, "PublishmentSystemDir" }, siteInfo.SiteDir);
            AtomUtility.AddDcElement(feed.AdditionalElements, new List<string> { SiteAttribute.TableName, "AuxiliaryTableForContent" }, siteInfo.TableName);
            AtomUtility.AddDcElement(feed.AdditionalElements, new List<string> { SiteAttribute.ParentId, "ParentPublishmentSystemId" }, siteInfo.ParentId.ToString());
            AtomUtility.AddDcElement(feed.AdditionalElements, SiteAttribute.Taxis, siteInfo.Taxis.ToString());
            AtomUtility.AddDcElement(feed.AdditionalElements, SiteAttribute.ExtendValues, siteInfo.ExtendValues);

            var indexTemplateId = _templateRepository.GetDefaultTemplateId(siteInfo.Id, TemplateType.IndexPageTemplate);
            if (indexTemplateId != 0)
            {
                var indexTemplateName = _templateRepository.GetTemplateName(_siteId, indexTemplateId);
                AtomUtility.AddDcElement(feed.AdditionalElements, DefaultIndexTemplateName, indexTemplateName);
            }

            var channelTemplateId = _templateRepository.GetDefaultTemplateId(siteInfo.Id, TemplateType.ChannelTemplate);
            if (channelTemplateId != 0)
            {
                var channelTemplateName = _templateRepository.GetTemplateName(_siteId, channelTemplateId);
                AtomUtility.AddDcElement(feed.AdditionalElements, DefaultChannelTemplateName, channelTemplateName);
            }

            var contentTemplateId = _templateRepository.GetDefaultTemplateId(siteInfo.Id, TemplateType.ContentTemplate);
            if (contentTemplateId != 0)
            {
                var contentTemplateName = _templateRepository.GetTemplateName(_siteId, contentTemplateId);
                AtomUtility.AddDcElement(feed.AdditionalElements, DefaultContentTemplateName, contentTemplateName);
            }

            var fileTemplateId = _templateRepository.GetDefaultTemplateId(siteInfo.Id, TemplateType.FileTemplate);
            if (fileTemplateId != 0)
            {
                var fileTemplateName = _templateRepository.GetTemplateName(siteInfo.Id, fileTemplateId);
                AtomUtility.AddDcElement(feed.AdditionalElements, DefaultFileTemplateName, fileTemplateName);
            }

            var channelGroupInfoList = _channelGroupRepository.GetChannelGroupInfoList(siteInfo.Id);
            channelGroupInfoList.Reverse();

            foreach (var channelGroupInfo in channelGroupInfoList)
            {
                var entry = ChannelGroupIe.Export(channelGroupInfo);
                feed.Entries.Add(entry);
            }

            var contentGroupInfoList = _contentGroupRepository.GetContentGroupInfoList(siteInfo.Id);
            contentGroupInfoList.Reverse();

            foreach (var contentGroupInfo in contentGroupInfoList)
            {
                var entry = ContentGroupIe.Export(contentGroupInfo);
                feed.Entries.Add(entry);
            }

            feed.Save(_filePath);
        }

        public static SiteInfo GetSiteInfo(string filePath)
        {
            var siteInfo = new SiteInfo();
            if (!FileUtils.IsFileExists(filePath)) return siteInfo;

            var feed = AtomFeed.Load(FileUtils.GetFileStreamReadOnly(filePath));

            siteInfo.SiteName = AtomUtility.GetDcElementContent(feed.AdditionalElements, new List<string> { SiteAttribute.SiteName, "PublishmentSystemName" });
            siteInfo.SiteDir = AtomUtility.GetDcElementContent(feed.AdditionalElements, new List<string> { SiteAttribute.SiteDir, "PublishmentSystemDir" });
            if (siteInfo.SiteDir != null && siteInfo.SiteDir.IndexOf("\\", StringComparison.Ordinal) != -1)
            {
                siteInfo.SiteDir = siteInfo.SiteDir.Substring(siteInfo.SiteDir.LastIndexOf("\\", StringComparison.Ordinal) + 1);
            }
            siteInfo.ExtendValues = AtomUtility.GetDcElementContent(feed.AdditionalElements, SiteAttribute.ExtendValues);
            siteInfo.IsCreateDoubleClick = false;
            return siteInfo;
        }

        public void Import()
        {
            if (!FileUtils.IsFileExists(_filePath)) return;

            var feed = AtomFeed.Load(FileUtils.GetFileStreamReadOnly(_filePath));

            var siteInfo = _siteRepository.GetSiteInfo(_siteId);

            siteInfo.ExtendValues = AtomUtility.GetDcElementContent(feed.AdditionalElements, SiteAttribute.ExtendValues, siteInfo.ExtendValues);

            siteInfo.IsSeparatedWeb = false;
            siteInfo.IsCreateDoubleClick = false;

            _siteRepository.Update(siteInfo);

            var indexTemplateName = AtomUtility.GetDcElementContent(feed.AdditionalElements, DefaultIndexTemplateName);
            if (!string.IsNullOrEmpty(indexTemplateName))
            {
                var indexTemplateId = _templateRepository.GetTemplateIdByTemplateName(siteInfo.Id, TemplateType.IndexPageTemplate, indexTemplateName);
                if (indexTemplateId != 0)
                {
                    _templateRepository.SetDefault(siteInfo.Id, indexTemplateId);
                }
            }

            var channelTemplateName = AtomUtility.GetDcElementContent(feed.AdditionalElements, DefaultChannelTemplateName);
            if (!string.IsNullOrEmpty(channelTemplateName))
            {
                var channelTemplateId = _templateRepository.GetTemplateIdByTemplateName(siteInfo.Id, TemplateType.ChannelTemplate, channelTemplateName);
                if (channelTemplateId != 0)
                {
                    _templateRepository.SetDefault(siteInfo.Id, channelTemplateId);
                }
            }

            var contentTemplateName = AtomUtility.GetDcElementContent(feed.AdditionalElements, DefaultContentTemplateName);
            if (!string.IsNullOrEmpty(contentTemplateName))
            {
                var contentTemplateId = _templateRepository.GetTemplateIdByTemplateName(siteInfo.Id, TemplateType.ContentTemplate, contentTemplateName);
                if (contentTemplateId != 0)
                {
                    _templateRepository.SetDefault(siteInfo.Id, contentTemplateId);
                }
            }

            var fileTemplateName = AtomUtility.GetDcElementContent(feed.AdditionalElements, DefaultFileTemplateName);
            if (!string.IsNullOrEmpty(fileTemplateName))
            {
                var fileTemplateId = _templateRepository.GetTemplateIdByTemplateName(siteInfo.Id, TemplateType.FileTemplate, fileTemplateName);
                if (fileTemplateId != 0)
                {
                    _templateRepository.SetDefault(siteInfo.Id, fileTemplateId);
                }
            }

            foreach (AtomEntry entry in feed.Entries)
            {
                if (!ChannelGroupIe.Import(entry, siteInfo.Id, _channelGroupRepository))
                {
                    ContentGroupIe.Import(entry, siteInfo.Id, _contentGroupRepository);
                }
            }
        }

    }
}
