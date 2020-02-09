using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Datory.Utils;
using SiteServer.CMS.Context.Atom.Atom.Core;
using SiteServer.Abstractions;
using SiteServer.CMS.DataCache;
using SiteServer.CMS.Repositories;


namespace SiteServer.CMS.ImportExport.Components
{
	public class ConfigurationIe
	{
		private readonly int _siteId;
		private readonly string _filePath;

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
			var site = await DataProvider.SiteRepository.GetAsync(_siteId);

			var feed = AtomUtility.GetEmptyFeed();

            AtomUtility.AddDcElement(feed.AdditionalElements, new List<string> { nameof(Site.Id), "PublishmentSystemId" }, site.Id.ToString());
			AtomUtility.AddDcElement(feed.AdditionalElements, new List<string> { nameof(Site.SiteName), "PublishmentSystemName" }, site.SiteName);
            AtomUtility.AddDcElement(feed.AdditionalElements, new List<string> { nameof(Site.SiteDir), "PublishmentSystemDir" }, site.SiteDir);
            AtomUtility.AddDcElement(feed.AdditionalElements, new List<string> { nameof(Site.TableName), "AuxiliaryTableForContent" }, site.TableName);
            AtomUtility.AddDcElement(feed.AdditionalElements, new List<string> { nameof(Site.ParentId), "ParentPublishmentSystemId" }, site.ParentId.ToString());
            AtomUtility.AddDcElement(feed.AdditionalElements, nameof(Site.Taxis), site.Taxis.ToString());
            AtomUtility.AddDcElement(feed.AdditionalElements, "SettingsXml", site.ToString());

            var indexTemplateId = await DataProvider.TemplateRepository.GetDefaultTemplateIdAsync(site.Id, TemplateType.IndexPageTemplate);
			if (indexTemplateId != 0)
			{
                var indexTemplateName = await DataProvider.TemplateRepository.GetTemplateNameAsync(indexTemplateId);
				AtomUtility.AddDcElement(feed.AdditionalElements, DefaultIndexTemplateName, indexTemplateName);
			}

            var channelTemplateId = await DataProvider.TemplateRepository.GetDefaultTemplateIdAsync(site.Id, TemplateType.ChannelTemplate);
			if (channelTemplateId != 0)
			{
                var channelTemplateName = await DataProvider.TemplateRepository.GetTemplateNameAsync(channelTemplateId);
				AtomUtility.AddDcElement(feed.AdditionalElements, DefaultChannelTemplateName, channelTemplateName);
			}

            var contentTemplateId = await DataProvider.TemplateRepository.GetDefaultTemplateIdAsync(site.Id, TemplateType.ContentTemplate);
			if (contentTemplateId != 0)
			{
                var contentTemplateName = await DataProvider.TemplateRepository.GetTemplateNameAsync(contentTemplateId);
				AtomUtility.AddDcElement(feed.AdditionalElements, DefaultContentTemplateName, contentTemplateName);
			}

            var fileTemplateId = await DataProvider.TemplateRepository.GetDefaultTemplateIdAsync(site.Id, TemplateType.FileTemplate);
			if (fileTemplateId != 0)
			{
                var fileTemplateName = await DataProvider.TemplateRepository.GetTemplateNameAsync(fileTemplateId);
				AtomUtility.AddDcElement(feed.AdditionalElements, DefaultFileTemplateName, fileTemplateName);
			}

			var channelGroupList = await DataProvider.ChannelGroupRepository.GetChannelGroupListAsync(site.Id);

            foreach (var channelGroup in channelGroupList)
			{
				var entry = ChannelGroupIe.Export(channelGroup);
                feed.Entries.Add(entry);
			}

			var contentGroupList = await DataProvider.ContentGroupRepository.GetContentGroupsAsync(site.Id);

            foreach (var contentGroup in contentGroupList)
			{
				var entry = ContentGroupIe.Export(contentGroup);
				feed.Entries.Add(entry);
			}

			feed.Save(_filePath);
		}

        //public static Site GetSite(string filePath)
        //{
        //    var site = new Site();
        //    if (!FileUtils.IsFileExists(filePath)) return site;

        //    var feed = AtomFeed.Load(FileUtils.GetFileStreamReadOnly(filePath));

        //    site.SiteName = AtomUtility.GetDcElementContent(feed.AdditionalElements, new List<string> { nameof(Site.SiteName), "PublishmentSystemName" });
        //    site.SiteDir = AtomUtility.GetDcElementContent(feed.AdditionalElements, new List<string> { nameof(Site.SiteDir), "PublishmentSystemDir" });
        //    if (site.SiteDir != null && site.SiteDir.IndexOf("\\", StringComparison.Ordinal) != -1)
        //    {
        //        site.SiteDir = site.SiteDir.Substring(site.SiteDir.LastIndexOf("\\", StringComparison.Ordinal) + 1);
        //    }

        //    site.SettingsXml = AtomUtility.GetDcElementContent(feed.AdditionalElements, "SettingsXml");

        //    site.IsCreateDoubleClick = false;

        //    return site;
        //}

		public async Task ImportAsync()
		{
			if (!FileUtils.IsFileExists(_filePath)) return;

            var feed = AtomFeed.Load(FileUtils.GetFileStreamReadOnly(_filePath));

			var site = await DataProvider.SiteRepository.GetAsync(_siteId);

            var json = AtomUtility.GetDcElementContent(feed.AdditionalElements,
                "SettingsXml");
            if (!string.IsNullOrEmpty(json))
            {
                var dict = Utilities.ToDictionary(json);
                foreach (var o in dict)
                {
                    site.Set(o.Key, o.Value);
                }
			}
            json = AtomUtility.GetDcElementContent(feed.AdditionalElements,
                "ExtendValues");
            if (!string.IsNullOrEmpty(json))
            {
                var dict = Utilities.ToDictionary(json);
                foreach (var o in dict)
                {
                    site.Set(o.Key, o.Value);
                }
            }

			site.IsSeparatedWeb = false;
            site.IsCreateDoubleClick = false;

            await DataProvider.SiteRepository.UpdateAsync(site);

			var indexTemplateName = AtomUtility.GetDcElementContent(feed.AdditionalElements, DefaultIndexTemplateName);
			if (!string.IsNullOrEmpty(indexTemplateName))
			{
				var indexTemplateId = await DataProvider.TemplateRepository.GetTemplateIdByTemplateNameAsync(site.Id, TemplateType.IndexPageTemplate, indexTemplateName);
				if (indexTemplateId != 0)
				{
					await DataProvider.TemplateRepository.SetDefaultAsync(indexTemplateId);
				}
			}

			var channelTemplateName = AtomUtility.GetDcElementContent(feed.AdditionalElements, DefaultChannelTemplateName);
			if (!string.IsNullOrEmpty(channelTemplateName))
			{
                var channelTemplateId = await DataProvider.TemplateRepository.GetTemplateIdByTemplateNameAsync(site.Id, TemplateType.ChannelTemplate, channelTemplateName);
				if (channelTemplateId != 0)
				{
					await DataProvider.TemplateRepository.SetDefaultAsync(channelTemplateId);
				}
			}

			var contentTemplateName = AtomUtility.GetDcElementContent(feed.AdditionalElements, DefaultContentTemplateName);
			if (!string.IsNullOrEmpty(contentTemplateName))
			{
                var contentTemplateId = await DataProvider.TemplateRepository.GetTemplateIdByTemplateNameAsync(site.Id, TemplateType.ContentTemplate, contentTemplateName);
				if (contentTemplateId != 0)
				{
					await DataProvider.TemplateRepository.SetDefaultAsync(contentTemplateId);
				}
			}

			var fileTemplateName = AtomUtility.GetDcElementContent(feed.AdditionalElements, DefaultFileTemplateName);
			if (!string.IsNullOrEmpty(fileTemplateName))
			{
                var fileTemplateId = await DataProvider.TemplateRepository.GetTemplateIdByTemplateNameAsync(site.Id, TemplateType.FileTemplate, fileTemplateName);
				if (fileTemplateId != 0)
				{
					await DataProvider.TemplateRepository.SetDefaultAsync(fileTemplateId);
				}
			}

			foreach (AtomEntry entry in feed.Entries)
			{
			    if (!await ChannelGroupIe.ImportAsync(entry, site.Id))
			    {
                    await ContentGroupIe.ImportAsync(entry, site.Id);
                }
			}
		}

	}
}
