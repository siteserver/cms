using System;
using System.Collections.Generic;
using System.Threading.Tasks;
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

        public async Task ExportAsync()
        {
            var siteInfo = await _siteRepository.GetSiteInfoAsync(_siteId);

            var feed = AtomUtility.GetEmptyFeed();

            AtomUtility.AddDcElement(feed.AdditionalElements, new List<string> { SiteAttribute.Id, "PublishmentSystemId" }, siteInfo.Id.ToString());
            AtomUtility.AddDcElement(feed.AdditionalElements, new List<string> { SiteAttribute.SiteName, "PublishmentSystemName" }, siteInfo.SiteName);
            AtomUtility.AddDcElement(feed.AdditionalElements, new List<string> { SiteAttribute.SiteDir, "PublishmentSystemDir" }, siteInfo.SiteDir);
            AtomUtility.AddDcElement(feed.AdditionalElements, new List<string> { SiteAttribute.TableName, "AuxiliaryTableForContent" }, siteInfo.TableName);
            AtomUtility.AddDcElement(feed.AdditionalElements, new List<string> { SiteAttribute.ParentId, "ParentPublishmentSystemId" }, siteInfo.ParentId.ToString());
            AtomUtility.AddDcElement(feed.AdditionalElements, SiteAttribute.Taxis, siteInfo.Taxis.ToString());
            AtomUtility.AddDcElement(feed.AdditionalElements, SiteAttribute.ExtendValues, siteInfo.ExtendValues);

            var indexTemplateId = await _templateRepository.GetDefaultTemplateIdAsync(siteInfo.Id, TemplateType.IndexPageTemplate);
            if (indexTemplateId != 0)
            {
                var indexTemplateName = await _templateRepository.GetTemplateNameAsync(indexTemplateId);
                AtomUtility.AddDcElement(feed.AdditionalElements, DefaultIndexTemplateName, indexTemplateName);
            }

            var channelTemplateId = await _templateRepository.GetDefaultTemplateIdAsync(siteInfo.Id, TemplateType.ChannelTemplate);
            if (channelTemplateId != 0)
            {
                var channelTemplateName = await _templateRepository.GetTemplateNameAsync(channelTemplateId);
                AtomUtility.AddDcElement(feed.AdditionalElements, DefaultChannelTemplateName, channelTemplateName);
            }

            var contentTemplateId = await _templateRepository.GetDefaultTemplateIdAsync(siteInfo.Id, TemplateType.ContentTemplate);
            if (contentTemplateId != 0)
            {
                var contentTemplateName = await _templateRepository.GetTemplateNameAsync(contentTemplateId);
                AtomUtility.AddDcElement(feed.AdditionalElements, DefaultContentTemplateName, contentTemplateName);
            }

            var fileTemplateId = await _templateRepository.GetDefaultTemplateIdAsync(siteInfo.Id, TemplateType.FileTemplate);
            if (fileTemplateId != 0)
            {
                var fileTemplateName = await _templateRepository.GetTemplateNameAsync(fileTemplateId);
                AtomUtility.AddDcElement(feed.AdditionalElements, DefaultFileTemplateName, fileTemplateName);
            }

            var channelGroupInfoList = await _channelGroupRepository.GetChannelGroupInfoListAsync(siteInfo.Id);
            channelGroupInfoList.Reverse();

            foreach (var channelGroupInfo in channelGroupInfoList)
            {
                var entry = ChannelGroupIe.Export(channelGroupInfo);
                feed.Entries.Add(entry);
            }

            var contentGroupInfoList = await _contentGroupRepository.GetContentGroupInfoListAsync(siteInfo.Id);
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

        public async Task ImportAsync()
        {
            if (!FileUtils.IsFileExists(_filePath)) return;

            var feed = AtomFeed.Load(FileUtils.GetFileStreamReadOnly(_filePath));

            var siteInfo = await _siteRepository.GetSiteInfoAsync(_siteId);

            siteInfo.ExtendValues = AtomUtility.GetDcElementContent(feed.AdditionalElements, SiteAttribute.ExtendValues, siteInfo.ExtendValues);

            siteInfo.IsSeparatedWeb = false;
            siteInfo.IsCreateDoubleClick = false;

            await _siteRepository.UpdateAsync(siteInfo);

            var indexTemplateName = AtomUtility.GetDcElementContent(feed.AdditionalElements, DefaultIndexTemplateName);
            if (!string.IsNullOrEmpty(indexTemplateName))
            {
                var indexTemplateId = await _templateRepository.GetTemplateIdByTemplateNameAsync(siteInfo.Id, TemplateType.IndexPageTemplate, indexTemplateName);
                if (indexTemplateId != 0)
                {
                    await _templateRepository.SetDefaultAsync(siteInfo.Id, indexTemplateId);
                }
            }

            var channelTemplateName = AtomUtility.GetDcElementContent(feed.AdditionalElements, DefaultChannelTemplateName);
            if (!string.IsNullOrEmpty(channelTemplateName))
            {
                var channelTemplateId = await _templateRepository.GetTemplateIdByTemplateNameAsync(siteInfo.Id, TemplateType.ChannelTemplate, channelTemplateName);
                if (channelTemplateId != 0)
                {
                    await _templateRepository.SetDefaultAsync(siteInfo.Id, channelTemplateId);
                }
            }

            var contentTemplateName = AtomUtility.GetDcElementContent(feed.AdditionalElements, DefaultContentTemplateName);
            if (!string.IsNullOrEmpty(contentTemplateName))
            {
                var contentTemplateId = await _templateRepository.GetTemplateIdByTemplateNameAsync(siteInfo.Id, TemplateType.ContentTemplate, contentTemplateName);
                if (contentTemplateId != 0)
                {
                    await _templateRepository.SetDefaultAsync(siteInfo.Id, contentTemplateId);
                }
            }

            var fileTemplateName = AtomUtility.GetDcElementContent(feed.AdditionalElements, DefaultFileTemplateName);
            if (!string.IsNullOrEmpty(fileTemplateName))
            {
                var fileTemplateId = await _templateRepository.GetTemplateIdByTemplateNameAsync(siteInfo.Id, TemplateType.FileTemplate, fileTemplateName);
                if (fileTemplateId != 0)
                {
                    await _templateRepository.SetDefaultAsync(siteInfo.Id, fileTemplateId);
                }
            }

            foreach (AtomEntry entry in feed.Entries)
            {
                if (!await ChannelGroupIe.ImportAsync(entry, siteInfo.Id, _channelGroupRepository))
                {
                    await ContentGroupIe.ImportAsync(entry, siteInfo.Id, _contentGroupRepository);
                }
            }
        }

    }
}
