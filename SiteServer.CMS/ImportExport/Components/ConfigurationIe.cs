using System;
using System.Collections.Generic;
using Atom.Core;
using SiteServer.Utils;
using SiteServer.CMS.Core;
using SiteServer.CMS.Model;
using SiteServer.CMS.Model.Attributes;
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

		public void Export()
		{
			var psInfo = SiteManager.GetSiteInfo(_siteId);

			var feed = AtomUtility.GetEmptyFeed();

            AtomUtility.AddDcElement(feed.AdditionalElements, new List<string> { SiteAttribute.Id, "PublishmentSystemId" }, psInfo.Id.ToString());
			AtomUtility.AddDcElement(feed.AdditionalElements, new List<string> { SiteAttribute.SiteName, "PublishmentSystemName" }, psInfo.SiteName);
            AtomUtility.AddDcElement(feed.AdditionalElements, new List<string> { SiteAttribute.SiteDir, "PublishmentSystemDir" }, psInfo.SiteDir);
            AtomUtility.AddDcElement(feed.AdditionalElements, new List<string> { SiteAttribute.TableName, "AuxiliaryTableForContent" }, psInfo.TableName);
            AtomUtility.AddDcElement(feed.AdditionalElements, new List<string> { SiteAttribute.IsRoot, "IsHeadquarters" }, psInfo.IsRoot.ToString());
            AtomUtility.AddDcElement(feed.AdditionalElements, new List<string> { SiteAttribute.ParentId, "ParentPublishmentSystemId" }, psInfo.ParentId.ToString());
            AtomUtility.AddDcElement(feed.AdditionalElements, SiteAttribute.Taxis, psInfo.Taxis.ToString());
            AtomUtility.AddDcElement(feed.AdditionalElements, SiteAttribute.SettingsXml, psInfo.Additional.ToString());

            var indexTemplateId = TemplateManager.GetDefaultTemplateId(psInfo.Id, TemplateType.IndexPageTemplate);
			if (indexTemplateId != 0)
			{
                var indexTemplateName = TemplateManager.GetTemplateName(_siteId, indexTemplateId);
				AtomUtility.AddDcElement(feed.AdditionalElements, DefaultIndexTemplateName, indexTemplateName);
			}

            var channelTemplateId = TemplateManager.GetDefaultTemplateId(psInfo.Id, TemplateType.ChannelTemplate);
			if (channelTemplateId != 0)
			{
                var channelTemplateName = TemplateManager.GetTemplateName(_siteId, channelTemplateId);
				AtomUtility.AddDcElement(feed.AdditionalElements, DefaultChannelTemplateName, channelTemplateName);
			}

            var contentTemplateId = TemplateManager.GetDefaultTemplateId(psInfo.Id, TemplateType.ContentTemplate);
			if (contentTemplateId != 0)
			{
                var contentTemplateName = TemplateManager.GetTemplateName(_siteId, contentTemplateId);
				AtomUtility.AddDcElement(feed.AdditionalElements, DefaultContentTemplateName, contentTemplateName);
			}

            var fileTemplateId = TemplateManager.GetDefaultTemplateId(psInfo.Id, TemplateType.FileTemplate);
			if (fileTemplateId != 0)
			{
                var fileTemplateName = TemplateManager.GetTemplateName(psInfo.Id, fileTemplateId);
				AtomUtility.AddDcElement(feed.AdditionalElements, DefaultFileTemplateName, fileTemplateName);
			}

			var channelGroupInfoList = DataProvider.ChannelGroupDao.GetGroupInfoList(psInfo.Id);
            channelGroupInfoList.Reverse();

			foreach (var channelGroupInfo in channelGroupInfoList)
			{
				var entry = ChannelGroupIe.Export(channelGroupInfo);
                feed.Entries.Add(entry);
			}

			var contentGroupInfoList = DataProvider.ContentGroupDao.GetContentGroupInfoList(psInfo.Id);
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
            siteInfo.SettingsXml = AtomUtility.GetDcElementContent(feed.AdditionalElements, SiteAttribute.SettingsXml);
            siteInfo.Additional.IsCreateDoubleClick = false;
            return siteInfo;
        }

		public void Import()
		{
			if (!FileUtils.IsFileExists(_filePath)) return;

            var feed = AtomFeed.Load(FileUtils.GetFileStreamReadOnly(_filePath));

			var siteInfo = SiteManager.GetSiteInfo(_siteId);

            siteInfo.SettingsXml = AtomUtility.GetDcElementContent(feed.AdditionalElements, SiteAttribute.SettingsXml, siteInfo.SettingsXml);

            siteInfo.Additional.IsSeparatedWeb = false;
            siteInfo.Additional.IsCreateDoubleClick = false;

            DataProvider.SiteDao.Update(siteInfo);

			var indexTemplateName = AtomUtility.GetDcElementContent(feed.AdditionalElements, DefaultIndexTemplateName);
			if (!string.IsNullOrEmpty(indexTemplateName))
			{
				var indexTemplateId = TemplateManager.GetTemplateIdByTemplateName(siteInfo.Id, TemplateType.IndexPageTemplate, indexTemplateName);
				if (indexTemplateId != 0)
				{
					DataProvider.TemplateDao.SetDefault(siteInfo.Id, indexTemplateId);
				}
			}

			var channelTemplateName = AtomUtility.GetDcElementContent(feed.AdditionalElements, DefaultChannelTemplateName);
			if (!string.IsNullOrEmpty(channelTemplateName))
			{
                var channelTemplateId = TemplateManager.GetTemplateIdByTemplateName(siteInfo.Id, TemplateType.ChannelTemplate, channelTemplateName);
				if (channelTemplateId != 0)
				{
					DataProvider.TemplateDao.SetDefault(siteInfo.Id, channelTemplateId);
				}
			}

			var contentTemplateName = AtomUtility.GetDcElementContent(feed.AdditionalElements, DefaultContentTemplateName);
			if (!string.IsNullOrEmpty(contentTemplateName))
			{
                var contentTemplateId = TemplateManager.GetTemplateIdByTemplateName(siteInfo.Id, TemplateType.ContentTemplate, contentTemplateName);
				if (contentTemplateId != 0)
				{
					DataProvider.TemplateDao.SetDefault(siteInfo.Id, contentTemplateId);
				}
			}

			var fileTemplateName = AtomUtility.GetDcElementContent(feed.AdditionalElements, DefaultFileTemplateName);
			if (!string.IsNullOrEmpty(fileTemplateName))
			{
                var fileTemplateId = TemplateManager.GetTemplateIdByTemplateName(siteInfo.Id, TemplateType.FileTemplate, fileTemplateName);
				if (fileTemplateId != 0)
				{
					DataProvider.TemplateDao.SetDefault(siteInfo.Id, fileTemplateId);
				}
			}

			foreach (AtomEntry entry in feed.Entries)
			{
			    if (!ChannelGroupIe.Import(entry, siteInfo.Id))
			    {
                    ContentGroupIe.Import(entry, siteInfo.Id);
                }
			}
		}

	}
}
