using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Atom.Core;
using SiteServer.Utils;
using SiteServer.CMS.Core;
using SiteServer.CMS.DataCache;
using SiteServer.CMS.Model;
using SiteServer.CMS.Model.Attributes;
using SiteServer.CMS.Model.Db;
using SiteServer.Plugin;

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
			var site = await SiteManager.GetSiteAsync(_siteId);

			var feed = AtomUtility.GetEmptyFeed();

            AtomUtility.AddDcElement(feed.AdditionalElements, new List<string> { SiteAttribute.Id, "PublishmentSystemId" }, site.Id.ToString());
			AtomUtility.AddDcElement(feed.AdditionalElements, new List<string> { SiteAttribute.SiteName, "PublishmentSystemName" }, site.SiteName);
            AtomUtility.AddDcElement(feed.AdditionalElements, new List<string> { SiteAttribute.SiteDir, "PublishmentSystemDir" }, site.SiteDir);
            AtomUtility.AddDcElement(feed.AdditionalElements, new List<string> { SiteAttribute.TableName, "AuxiliaryTableForContent" }, site.TableName);
            AtomUtility.AddDcElement(feed.AdditionalElements, new List<string> { SiteAttribute.IsRoot, "IsHeadquarters" }, site.Root.ToString());
            AtomUtility.AddDcElement(feed.AdditionalElements, new List<string> { SiteAttribute.ParentId, "ParentPublishmentSystemId" }, site.ParentId.ToString());
            AtomUtility.AddDcElement(feed.AdditionalElements, SiteAttribute.Taxis, site.Taxis.ToString());
            AtomUtility.AddDcElement(feed.AdditionalElements, SiteAttribute.SettingsXml, site.Additional.ToString());

            var indexTemplateId = TemplateManager.GetDefaultTemplateId(site.Id, TemplateType.IndexPageTemplate);
			if (indexTemplateId != 0)
			{
                var indexTemplateName = TemplateManager.GetTemplateName(_siteId, indexTemplateId);
				AtomUtility.AddDcElement(feed.AdditionalElements, DefaultIndexTemplateName, indexTemplateName);
			}

            var channelTemplateId = TemplateManager.GetDefaultTemplateId(site.Id, TemplateType.ChannelTemplate);
			if (channelTemplateId != 0)
			{
                var channelTemplateName = TemplateManager.GetTemplateName(_siteId, channelTemplateId);
				AtomUtility.AddDcElement(feed.AdditionalElements, DefaultChannelTemplateName, channelTemplateName);
			}

            var contentTemplateId = TemplateManager.GetDefaultTemplateId(site.Id, TemplateType.ContentTemplate);
			if (contentTemplateId != 0)
			{
                var contentTemplateName = TemplateManager.GetTemplateName(_siteId, contentTemplateId);
				AtomUtility.AddDcElement(feed.AdditionalElements, DefaultContentTemplateName, contentTemplateName);
			}

            var fileTemplateId = TemplateManager.GetDefaultTemplateId(site.Id, TemplateType.FileTemplate);
			if (fileTemplateId != 0)
			{
                var fileTemplateName = TemplateManager.GetTemplateName(site.Id, fileTemplateId);
				AtomUtility.AddDcElement(feed.AdditionalElements, DefaultFileTemplateName, fileTemplateName);
			}

			var channelGroupInfoList = ChannelGroupManager.GetChannelGroupInfoList(site.Id);
            channelGroupInfoList.Reverse();

			foreach (var channelGroupInfo in channelGroupInfoList)
			{
				var entry = ChannelGroupIe.Export(channelGroupInfo);
                feed.Entries.Add(entry);
			}

			var contentGroupInfoList = ContentGroupManager.GetContentGroupInfoList(site.Id);
            contentGroupInfoList.Reverse();

			foreach (var contentGroupInfo in contentGroupInfoList)
			{
				var entry = ContentGroupIe.Export(contentGroupInfo);
				feed.Entries.Add(entry);
			}

			feed.Save(_filePath);
		}

        public static Site GetSite(string filePath)
        {
            var site = new Site();
            if (!FileUtils.IsFileExists(filePath)) return site;

            var feed = AtomFeed.Load(FileUtils.GetFileStreamReadOnly(filePath));

            site.SiteName = AtomUtility.GetDcElementContent(feed.AdditionalElements, new List<string> { SiteAttribute.SiteName, "PublishmentSystemName" });
            site.SiteDir = AtomUtility.GetDcElementContent(feed.AdditionalElements, new List<string> { SiteAttribute.SiteDir, "PublishmentSystemDir" });
            if (site.SiteDir != null && site.SiteDir.IndexOf("\\", StringComparison.Ordinal) != -1)
            {
                site.SiteDir = site.SiteDir.Substring(site.SiteDir.LastIndexOf("\\", StringComparison.Ordinal) + 1);
            }

            site.Additional = new SiteInfoExtend(site.SiteDir,
                AtomUtility.GetDcElementContent(feed.AdditionalElements, SiteAttribute.SettingsXml))
            {
                IsCreateDoubleClick = false
            };
            return site;
        }

		public async Task ImportAsync()
		{
			if (!FileUtils.IsFileExists(_filePath)) return;

            var feed = AtomFeed.Load(FileUtils.GetFileStreamReadOnly(_filePath));

			var site = await SiteManager.GetSiteAsync(_siteId);

            site.Additional = new SiteInfoExtend(site.SiteDir, AtomUtility.GetDcElementContent(feed.AdditionalElements, SiteAttribute.SettingsXml, site.Additional.ToString()));

            site.Additional.IsSeparatedWeb = false;
            site.Additional.IsCreateDoubleClick = false;

            await DataProvider.SiteDao.UpdateAsync(site);

			var indexTemplateName = AtomUtility.GetDcElementContent(feed.AdditionalElements, DefaultIndexTemplateName);
			if (!string.IsNullOrEmpty(indexTemplateName))
			{
				var indexTemplateId = TemplateManager.GetTemplateIdByTemplateName(site.Id, TemplateType.IndexPageTemplate, indexTemplateName);
				if (indexTemplateId != 0)
				{
					DataProvider.TemplateDao.SetDefault(site.Id, indexTemplateId);
				}
			}

			var channelTemplateName = AtomUtility.GetDcElementContent(feed.AdditionalElements, DefaultChannelTemplateName);
			if (!string.IsNullOrEmpty(channelTemplateName))
			{
                var channelTemplateId = TemplateManager.GetTemplateIdByTemplateName(site.Id, TemplateType.ChannelTemplate, channelTemplateName);
				if (channelTemplateId != 0)
				{
					DataProvider.TemplateDao.SetDefault(site.Id, channelTemplateId);
				}
			}

			var contentTemplateName = AtomUtility.GetDcElementContent(feed.AdditionalElements, DefaultContentTemplateName);
			if (!string.IsNullOrEmpty(contentTemplateName))
			{
                var contentTemplateId = TemplateManager.GetTemplateIdByTemplateName(site.Id, TemplateType.ContentTemplate, contentTemplateName);
				if (contentTemplateId != 0)
				{
					DataProvider.TemplateDao.SetDefault(site.Id, contentTemplateId);
				}
			}

			var fileTemplateName = AtomUtility.GetDcElementContent(feed.AdditionalElements, DefaultFileTemplateName);
			if (!string.IsNullOrEmpty(fileTemplateName))
			{
                var fileTemplateId = TemplateManager.GetTemplateIdByTemplateName(site.Id, TemplateType.FileTemplate, fileTemplateName);
				if (fileTemplateId != 0)
				{
					DataProvider.TemplateDao.SetDefault(site.Id, fileTemplateId);
				}
			}

			foreach (AtomEntry entry in feed.Entries)
			{
			    if (!ChannelGroupIe.Import(entry, site.Id))
			    {
                    ContentGroupIe.Import(entry, site.Id);
                }
			}
		}

	}
}
