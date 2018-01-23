using System;
using Atom.Core;
using SiteServer.Utils;
using SiteServer.CMS.Core;
using SiteServer.CMS.Model;
using SiteServer.CMS.Model.Enumerations;

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

            AtomUtility.AddDcElement(feed.AdditionalElements, SiteAttribute.Id, psInfo.Id.ToString());
			AtomUtility.AddDcElement(feed.AdditionalElements, SiteAttribute.SiteName, psInfo.SiteName);
            AtomUtility.AddDcElement(feed.AdditionalElements, SiteAttribute.SiteDir, psInfo.SiteDir);
            AtomUtility.AddDcElement(feed.AdditionalElements, SiteAttribute.TableName, psInfo.TableName);
            AtomUtility.AddDcElement(feed.AdditionalElements, SiteAttribute.IsRoot, psInfo.IsRoot.ToString());
            AtomUtility.AddDcElement(feed.AdditionalElements, SiteAttribute.ParentId, psInfo.ParentId.ToString());
            AtomUtility.AddDcElement(feed.AdditionalElements, SiteAttribute.Taxis, psInfo.Taxis.ToString());
            AtomUtility.AddDcElement(feed.AdditionalElements, SiteAttribute.SettingsXml, psInfo.Additional.ToString());

			var indexTemplateId = TemplateManager.GetDefaultTemplateId(psInfo.Id, ETemplateType.IndexPageTemplate);
			if (indexTemplateId != 0)
			{
                var indexTemplateName = TemplateManager.GetTemplateName(_siteId, indexTemplateId);
				AtomUtility.AddDcElement(feed.AdditionalElements, DefaultIndexTemplateName, indexTemplateName);
			}

            var channelTemplateId = TemplateManager.GetDefaultTemplateId(psInfo.Id, ETemplateType.ChannelTemplate);
			if (channelTemplateId != 0)
			{
                var channelTemplateName = TemplateManager.GetTemplateName(_siteId, channelTemplateId);
				AtomUtility.AddDcElement(feed.AdditionalElements, DefaultChannelTemplateName, channelTemplateName);
			}

            var contentTemplateId = TemplateManager.GetDefaultTemplateId(psInfo.Id, ETemplateType.ContentTemplate);
			if (contentTemplateId != 0)
			{
                var contentTemplateName = TemplateManager.GetTemplateName(_siteId, contentTemplateId);
				AtomUtility.AddDcElement(feed.AdditionalElements, DefaultContentTemplateName, contentTemplateName);
			}

            var fileTemplateId = TemplateManager.GetDefaultTemplateId(psInfo.Id, ETemplateType.FileTemplate);
			if (fileTemplateId != 0)
			{
                var fileTemplateName = TemplateManager.GetTemplateName(psInfo.Id, fileTemplateId);
				AtomUtility.AddDcElement(feed.AdditionalElements, DefaultFileTemplateName, fileTemplateName);
			}

			var nodeGroupInfoList = DataProvider.ChannelGroupDao.GetGroupInfoList(psInfo.Id);
            nodeGroupInfoList.Reverse();

			foreach (var nodeGroupInfo in nodeGroupInfoList)
			{
				var entry = ExportNodeGroupInfo(nodeGroupInfo);
				feed.Entries.Add(entry);
			}

			var contentGroupInfoList = DataProvider.ContentGroupDao.GetContentGroupInfoList(psInfo.Id);
            contentGroupInfoList.Reverse();

			foreach (var contentGroupInfo in contentGroupInfoList)
			{
				var entry = ExportContentGroupInfo(contentGroupInfo);
				feed.Entries.Add(entry);
			}

			feed.Save(_filePath);
		}

		private static AtomEntry ExportNodeGroupInfo(ChannelGroupInfo nodeGroupInfo)
		{
			var entry = AtomUtility.GetEmptyEntry();

			AtomUtility.AddDcElement(entry.AdditionalElements, "IsNodeGroup", true.ToString());
			AtomUtility.AddDcElement(entry.AdditionalElements, "GroupName", nodeGroupInfo.GroupName);
            AtomUtility.AddDcElement(entry.AdditionalElements, "Taxis", nodeGroupInfo.Taxis.ToString());
			AtomUtility.AddDcElement(entry.AdditionalElements, "Description", nodeGroupInfo.Description);

			return entry;
		}

		private static AtomEntry ExportContentGroupInfo(ContentGroupInfo contentGroupInfo)
		{
			var entry = AtomUtility.GetEmptyEntry();

			AtomUtility.AddDcElement(entry.AdditionalElements, "IsContentGroup", true.ToString());
			AtomUtility.AddDcElement(entry.AdditionalElements, "GroupName", contentGroupInfo.GroupName);
            AtomUtility.AddDcElement(entry.AdditionalElements, "Taxis", contentGroupInfo.Taxis.ToString());
			AtomUtility.AddDcElement(entry.AdditionalElements, "Description", contentGroupInfo.Description);

			return entry;
		}

        public static SiteInfo GetPublishmentSytemInfo(string filePath)
        {
            var siteInfo = new SiteInfo();
            if (!FileUtils.IsFileExists(filePath)) return siteInfo;

            var feed = AtomFeed.Load(FileUtils.GetFileStreamReadOnly(filePath));

            siteInfo.SiteName = AtomUtility.GetDcElementContent(feed.AdditionalElements, SiteAttribute.SiteName);
            siteInfo.SiteDir = AtomUtility.GetDcElementContent(feed.AdditionalElements, SiteAttribute.SiteDir);
            if (siteInfo.SiteDir != null && siteInfo.SiteDir.IndexOf("\\", StringComparison.Ordinal) != -1)
            {
                siteInfo.SiteDir = siteInfo.SiteDir.Substring(siteInfo.SiteDir.LastIndexOf("\\", StringComparison.Ordinal) + 1);
            }
            //siteInfo.IsCheckContentUseLevel = EBooleanUtils.GetEnumType(AtomUtility.GetDcElementContent(feed.AdditionalElements, SiteAttribute.IsCheckContentUseLevel));
            //siteInfo.CheckContentLevel = TranslateUtils.ToInt(AtomUtility.GetDcElementContent(feed.AdditionalElements, SiteAttribute.CheckContentLevel));
            siteInfo.SettingsXml = AtomUtility.GetDcElementContent(feed.AdditionalElements, SiteAttribute.SettingsXml);
            siteInfo.Additional.IsCreateDoubleClick = false;
            return siteInfo;
        }

		public void Import()
		{
			if (!FileUtils.IsFileExists(_filePath)) return;

            var feed = AtomFeed.Load(FileUtils.GetFileStreamReadOnly(_filePath));

			var siteInfo = SiteManager.GetSiteInfo(_siteId);

            //psInfo.IsCheckContentUseLevel = EBooleanUtils.GetEnumType(AtomUtility.GetDcElementContent(feed.AdditionalElements, SiteAttribute.IsCheckContentUseLevel, EBooleanUtils.GetValue(psInfo.IsCheckContentUseLevel)));
            //psInfo.CheckContentLevel = TranslateUtils.ToInt(AtomUtility.GetDcElementContent(feed.AdditionalElements, SiteAttribute.CheckContentLevel, psInfo.CheckContentLevel.ToString()));
            siteInfo.SettingsXml = AtomUtility.GetDcElementContent(feed.AdditionalElements, SiteAttribute.SettingsXml, siteInfo.SettingsXml);

            siteInfo.Additional.IsSeparatedWeb = false;
            siteInfo.Additional.IsCreateDoubleClick = false;

            DataProvider.SiteDao.Update(siteInfo);

			var indexTemplateName = AtomUtility.GetDcElementContent(feed.AdditionalElements, DefaultIndexTemplateName);
			if (!string.IsNullOrEmpty(indexTemplateName))
			{
				var indexTemplateId = TemplateManager.GetTemplateIdByTemplateName(siteInfo.Id, ETemplateType.IndexPageTemplate, indexTemplateName);
				if (indexTemplateId != 0)
				{
					DataProvider.TemplateDao.SetDefault(siteInfo.Id, indexTemplateId);
				}
			}

			var channelTemplateName = AtomUtility.GetDcElementContent(feed.AdditionalElements, DefaultChannelTemplateName);
			if (!string.IsNullOrEmpty(channelTemplateName))
			{
                var channelTemplateId = TemplateManager.GetTemplateIdByTemplateName(siteInfo.Id, ETemplateType.ChannelTemplate, channelTemplateName);
				if (channelTemplateId != 0)
				{
					DataProvider.TemplateDao.SetDefault(siteInfo.Id, channelTemplateId);
				}
			}

			var contentTemplateName = AtomUtility.GetDcElementContent(feed.AdditionalElements, DefaultContentTemplateName);
			if (!string.IsNullOrEmpty(contentTemplateName))
			{
                var contentTemplateId = TemplateManager.GetTemplateIdByTemplateName(siteInfo.Id, ETemplateType.ContentTemplate, contentTemplateName);
				if (contentTemplateId != 0)
				{
					DataProvider.TemplateDao.SetDefault(siteInfo.Id, contentTemplateId);
				}
			}

			var fileTemplateName = AtomUtility.GetDcElementContent(feed.AdditionalElements, DefaultFileTemplateName);
			if (!string.IsNullOrEmpty(fileTemplateName))
			{
                var fileTemplateId = TemplateManager.GetTemplateIdByTemplateName(siteInfo.Id, ETemplateType.FileTemplate, fileTemplateName);
				if (fileTemplateId != 0)
				{
					DataProvider.TemplateDao.SetDefault(siteInfo.Id, fileTemplateId);
				}
			}

			foreach (AtomEntry entry in feed.Entries)
			{
				var isNodeGroup = TranslateUtils.ToBool(AtomUtility.GetDcElementContent(entry.AdditionalElements, "IsNodeGroup"));
				var isContentGroup = TranslateUtils.ToBool(AtomUtility.GetDcElementContent(entry.AdditionalElements, "IsContentGroup"));
				if (isNodeGroup)
				{
					var nodeGroupName = AtomUtility.GetDcElementContent(entry.AdditionalElements, "NodeGroupName");
				    if (DataProvider.ChannelGroupDao.IsExists(siteInfo.Id, nodeGroupName)) continue;

				    var taxis = TranslateUtils.ToInt(AtomUtility.GetDcElementContent(entry.AdditionalElements, "Taxis"));
				    var description = AtomUtility.GetDcElementContent(entry.AdditionalElements, "Description");
				    DataProvider.ChannelGroupDao.Insert(new ChannelGroupInfo(nodeGroupName, siteInfo.Id, taxis, description));
				}
				else if (isContentGroup)
				{
					var contentGroupName = AtomUtility.GetDcElementContent(entry.AdditionalElements, "ContentGroupName");
				    if (DataProvider.ContentGroupDao.IsExists(contentGroupName, siteInfo.Id)) continue;

				    var taxis = TranslateUtils.ToInt(AtomUtility.GetDcElementContent(entry.AdditionalElements, "Taxis"));
				    var description = AtomUtility.GetDcElementContent(entry.AdditionalElements, "Description");
				    DataProvider.ContentGroupDao.Insert(new ContentGroupInfo(contentGroupName, siteInfo.Id, taxis, description));
				}
			}
		}

	}
}
