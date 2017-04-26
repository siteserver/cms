using System;
using Atom.Core;
using BaiRong.Core;
using SiteServer.CMS.Core;
using SiteServer.CMS.Model;
using SiteServer.CMS.Model.Enumerations;

namespace SiteServer.CMS.ImportExport.Components
{
	public class ConfigurationIe
	{
		private readonly int _publishmentSystemId;
		private readonly string _filePath;

		private const string DefaultIndexTemplateName = "DefaultIndexTemplateName";
		private const string DefaultChannelTemplateName = "DefaultChannelTemplateName";
		private const string DefaultContentTemplateName = "DefaultContentTemplateName";
		private const string DefaultFileTemplateName = "DefaultFileTemplateName";

		public ConfigurationIe(int publishmentSystemId, string filePath)
		{
			_publishmentSystemId = publishmentSystemId;
			_filePath = filePath;
		}

		public void Export()
		{
			var psInfo = PublishmentSystemManager.GetPublishmentSystemInfo(_publishmentSystemId);

			var feed = AtomUtility.GetEmptyFeed();

            AtomUtility.AddDcElement(feed.AdditionalElements, PublishmentSystemAttribute.PublishmentSystemId, psInfo.PublishmentSystemId.ToString());
			AtomUtility.AddDcElement(feed.AdditionalElements, PublishmentSystemAttribute.PublishmentSystemName, psInfo.PublishmentSystemName);
			AtomUtility.AddDcElement(feed.AdditionalElements, PublishmentSystemAttribute.AuxiliaryTableForContent, psInfo.AuxiliaryTableForContent);
            AtomUtility.AddDcElement(feed.AdditionalElements, PublishmentSystemAttribute.AuxiliaryTableForGovPublic, psInfo.AuxiliaryTableForGovPublic);
            AtomUtility.AddDcElement(feed.AdditionalElements, PublishmentSystemAttribute.AuxiliaryTableForGovInteract, psInfo.AuxiliaryTableForGovInteract);
            AtomUtility.AddDcElement(feed.AdditionalElements, PublishmentSystemAttribute.AuxiliaryTableForJob, psInfo.AuxiliaryTableForJob);
            AtomUtility.AddDcElement(feed.AdditionalElements, PublishmentSystemAttribute.AuxiliaryTableForVote, psInfo.AuxiliaryTableForVote);
            AtomUtility.AddDcElement(feed.AdditionalElements, PublishmentSystemAttribute.IsCheckContentUseLevel, psInfo.IsCheckContentUseLevel.ToString());
			AtomUtility.AddDcElement(feed.AdditionalElements, PublishmentSystemAttribute.CheckContentLevel, psInfo.CheckContentLevel.ToString());
			AtomUtility.AddDcElement(feed.AdditionalElements, PublishmentSystemAttribute.PublishmentSystemDir, psInfo.PublishmentSystemDir);
			AtomUtility.AddDcElement(feed.AdditionalElements, PublishmentSystemAttribute.PublishmentSystemUrl, psInfo.PublishmentSystemUrl);
            AtomUtility.AddDcElement(feed.AdditionalElements, PublishmentSystemAttribute.IsHeadquarters, psInfo.IsHeadquarters.ToString());
            AtomUtility.AddDcElement(feed.AdditionalElements, PublishmentSystemAttribute.ParentPublishmentSystemId, psInfo.ParentPublishmentSystemId.ToString());
            AtomUtility.AddDcElement(feed.AdditionalElements, PublishmentSystemAttribute.Taxis, psInfo.Taxis.ToString());
            AtomUtility.AddDcElement(feed.AdditionalElements, PublishmentSystemAttribute.SettingsXml, psInfo.Additional.ToString());

			var indexTemplateId = TemplateManager.GetDefaultTemplateID(psInfo.PublishmentSystemId, ETemplateType.IndexPageTemplate);
			if (indexTemplateId != 0)
			{
                var indexTemplateName = TemplateManager.GetTemplateName(_publishmentSystemId, indexTemplateId);
				AtomUtility.AddDcElement(feed.AdditionalElements, DefaultIndexTemplateName, indexTemplateName);
			}

            var channelTemplateId = TemplateManager.GetDefaultTemplateID(psInfo.PublishmentSystemId, ETemplateType.ChannelTemplate);
			if (channelTemplateId != 0)
			{
                var channelTemplateName = TemplateManager.GetTemplateName(_publishmentSystemId, channelTemplateId);
				AtomUtility.AddDcElement(feed.AdditionalElements, DefaultChannelTemplateName, channelTemplateName);
			}

            var contentTemplateId = TemplateManager.GetDefaultTemplateID(psInfo.PublishmentSystemId, ETemplateType.ContentTemplate);
			if (contentTemplateId != 0)
			{
                var contentTemplateName = TemplateManager.GetTemplateName(_publishmentSystemId, contentTemplateId);
				AtomUtility.AddDcElement(feed.AdditionalElements, DefaultContentTemplateName, contentTemplateName);
			}

            var fileTemplateId = TemplateManager.GetDefaultTemplateID(psInfo.PublishmentSystemId, ETemplateType.FileTemplate);
			if (fileTemplateId != 0)
			{
                var fileTemplateName = TemplateManager.GetTemplateName(psInfo.PublishmentSystemId, fileTemplateId);
				AtomUtility.AddDcElement(feed.AdditionalElements, DefaultFileTemplateName, fileTemplateName);
			}

			var nodeGroupInfoList = DataProvider.NodeGroupDao.GetNodeGroupInfoList(psInfo.PublishmentSystemId);
            nodeGroupInfoList.Reverse();

			foreach (var nodeGroupInfo in nodeGroupInfoList)
			{
				var entry = ExportNodeGroupInfo(nodeGroupInfo);
				feed.Entries.Add(entry);
			}

			var contentGroupInfoList = DataProvider.ContentGroupDao.GetContentGroupInfoList(psInfo.PublishmentSystemId);
            contentGroupInfoList.Reverse();

			foreach (var contentGroupInfo in contentGroupInfoList)
			{
				var entry = ExportContentGroupInfo(contentGroupInfo);
				feed.Entries.Add(entry);
			}

			feed.Save(_filePath);
		}

		private static AtomEntry ExportNodeGroupInfo(NodeGroupInfo nodeGroupInfo)
		{
			var entry = AtomUtility.GetEmptyEntry();

			AtomUtility.AddDcElement(entry.AdditionalElements, "IsNodeGroup", true.ToString());
			AtomUtility.AddDcElement(entry.AdditionalElements, "NodeGroupName", nodeGroupInfo.NodeGroupName);
            AtomUtility.AddDcElement(entry.AdditionalElements, "Taxis", nodeGroupInfo.Taxis.ToString());
			AtomUtility.AddDcElement(entry.AdditionalElements, "Description", nodeGroupInfo.Description);

			return entry;
		}

		private static AtomEntry ExportContentGroupInfo(ContentGroupInfo contentGroupInfo)
		{
			var entry = AtomUtility.GetEmptyEntry();

			AtomUtility.AddDcElement(entry.AdditionalElements, "IsContentGroup", true.ToString());
			AtomUtility.AddDcElement(entry.AdditionalElements, "ContentGroupName", contentGroupInfo.ContentGroupName);
            AtomUtility.AddDcElement(entry.AdditionalElements, "Taxis", contentGroupInfo.Taxis.ToString());
			AtomUtility.AddDcElement(entry.AdditionalElements, "Description", contentGroupInfo.Description);

			return entry;
		}

        public static PublishmentSystemInfo GetPublishmentSytemInfo(string filePath)
        {
            var publishmentSystemInfo = new PublishmentSystemInfo();
            if (!FileUtils.IsFileExists(filePath)) return publishmentSystemInfo;

            var feed = AtomFeed.Load(FileUtils.GetFileStreamReadOnly(filePath));

            publishmentSystemInfo.PublishmentSystemName = AtomUtility.GetDcElementContent(feed.AdditionalElements, PublishmentSystemAttribute.PublishmentSystemName);
            publishmentSystemInfo.PublishmentSystemDir = AtomUtility.GetDcElementContent(feed.AdditionalElements, PublishmentSystemAttribute.PublishmentSystemDir);
            if (publishmentSystemInfo.PublishmentSystemDir != null && publishmentSystemInfo.PublishmentSystemDir.IndexOf("\\", StringComparison.Ordinal) != -1)
            {
                publishmentSystemInfo.PublishmentSystemDir = publishmentSystemInfo.PublishmentSystemDir.Substring(publishmentSystemInfo.PublishmentSystemDir.LastIndexOf("\\", StringComparison.Ordinal) + 1);
            }
            //publishmentSystemInfo.IsCheckContentUseLevel = EBooleanUtils.GetEnumType(AtomUtility.GetDcElementContent(feed.AdditionalElements, PublishmentSystemAttribute.IsCheckContentUseLevel));
            //publishmentSystemInfo.CheckContentLevel = TranslateUtils.ToInt(AtomUtility.GetDcElementContent(feed.AdditionalElements, PublishmentSystemAttribute.CheckContentLevel));
            publishmentSystemInfo.SettingsXml = AtomUtility.GetDcElementContent(feed.AdditionalElements, PublishmentSystemAttribute.SettingsXml);
            publishmentSystemInfo.Additional.IsCreateDoubleClick = false;
            return publishmentSystemInfo;
        }

		public void Import()
		{
			if (!FileUtils.IsFileExists(_filePath)) return;

            var feed = AtomFeed.Load(FileUtils.GetFileStreamReadOnly(_filePath));

			var publishmentSystemInfo = PublishmentSystemManager.GetPublishmentSystemInfo(_publishmentSystemId);

            //psInfo.IsCheckContentUseLevel = EBooleanUtils.GetEnumType(AtomUtility.GetDcElementContent(feed.AdditionalElements, PublishmentSystemAttribute.IsCheckContentUseLevel, EBooleanUtils.GetValue(psInfo.IsCheckContentUseLevel)));
            //psInfo.CheckContentLevel = TranslateUtils.ToInt(AtomUtility.GetDcElementContent(feed.AdditionalElements, PublishmentSystemAttribute.CheckContentLevel, psInfo.CheckContentLevel.ToString()));
            publishmentSystemInfo.SettingsXml = AtomUtility.GetDcElementContent(feed.AdditionalElements, PublishmentSystemAttribute.SettingsXml, publishmentSystemInfo.SettingsXml);

            publishmentSystemInfo.Additional.ApiUrl = PublishmentSystemInfoExtend.DefaultApiUrl;
            publishmentSystemInfo.Additional.HomeUrl = PublishmentSystemInfoExtend.DefaultHomeUrl;
            publishmentSystemInfo.Additional.IsMultiDeployment = false;
            publishmentSystemInfo.Additional.IsCreateDoubleClick = false;

            DataProvider.PublishmentSystemDao.Update(publishmentSystemInfo);

			var indexTemplateName = AtomUtility.GetDcElementContent(feed.AdditionalElements, DefaultIndexTemplateName);
			if (!string.IsNullOrEmpty(indexTemplateName))
			{
				var indexTemplateId = TemplateManager.GetTemplateIDByTemplateName(publishmentSystemInfo.PublishmentSystemId, ETemplateType.IndexPageTemplate, indexTemplateName);
				if (indexTemplateId != 0)
				{
					DataProvider.TemplateDao.SetDefault(publishmentSystemInfo.PublishmentSystemId, indexTemplateId);
				}
			}

			var channelTemplateName = AtomUtility.GetDcElementContent(feed.AdditionalElements, DefaultChannelTemplateName);
			if (!string.IsNullOrEmpty(channelTemplateName))
			{
                var channelTemplateId = TemplateManager.GetTemplateIDByTemplateName(publishmentSystemInfo.PublishmentSystemId, ETemplateType.ChannelTemplate, channelTemplateName);
				if (channelTemplateId != 0)
				{
					DataProvider.TemplateDao.SetDefault(publishmentSystemInfo.PublishmentSystemId, channelTemplateId);
				}
			}

			var contentTemplateName = AtomUtility.GetDcElementContent(feed.AdditionalElements, DefaultContentTemplateName);
			if (!string.IsNullOrEmpty(contentTemplateName))
			{
                var contentTemplateId = TemplateManager.GetTemplateIDByTemplateName(publishmentSystemInfo.PublishmentSystemId, ETemplateType.ContentTemplate, contentTemplateName);
				if (contentTemplateId != 0)
				{
					DataProvider.TemplateDao.SetDefault(publishmentSystemInfo.PublishmentSystemId, contentTemplateId);
				}
			}

			var fileTemplateName = AtomUtility.GetDcElementContent(feed.AdditionalElements, DefaultFileTemplateName);
			if (!string.IsNullOrEmpty(fileTemplateName))
			{
                var fileTemplateId = TemplateManager.GetTemplateIDByTemplateName(publishmentSystemInfo.PublishmentSystemId, ETemplateType.FileTemplate, fileTemplateName);
				if (fileTemplateId != 0)
				{
					DataProvider.TemplateDao.SetDefault(publishmentSystemInfo.PublishmentSystemId, fileTemplateId);
				}
			}

			foreach (AtomEntry entry in feed.Entries)
			{
				var isNodeGroup = TranslateUtils.ToBool(AtomUtility.GetDcElementContent(entry.AdditionalElements, "IsNodeGroup"));
				var isContentGroup = TranslateUtils.ToBool(AtomUtility.GetDcElementContent(entry.AdditionalElements, "IsContentGroup"));
				if (isNodeGroup)
				{
					var nodeGroupName = AtomUtility.GetDcElementContent(entry.AdditionalElements, "NodeGroupName");
				    if (DataProvider.NodeGroupDao.IsExists(publishmentSystemInfo.PublishmentSystemId, nodeGroupName)) continue;

				    var taxis = TranslateUtils.ToInt(AtomUtility.GetDcElementContent(entry.AdditionalElements, "Taxis"));
				    var description = AtomUtility.GetDcElementContent(entry.AdditionalElements, "Description");
				    DataProvider.NodeGroupDao.Insert(new NodeGroupInfo(nodeGroupName, publishmentSystemInfo.PublishmentSystemId, taxis, description));
				}
				else if (isContentGroup)
				{
					var contentGroupName = AtomUtility.GetDcElementContent(entry.AdditionalElements, "ContentGroupName");
				    if (DataProvider.ContentGroupDao.IsExists(contentGroupName, publishmentSystemInfo.PublishmentSystemId)) continue;

				    var taxis = TranslateUtils.ToInt(AtomUtility.GetDcElementContent(entry.AdditionalElements, "Taxis"));
				    var description = AtomUtility.GetDcElementContent(entry.AdditionalElements, "Description");
				    DataProvider.ContentGroupDao.Insert(new ContentGroupInfo(contentGroupName, publishmentSystemInfo.PublishmentSystemId, taxis, description));
				}
			}
		}

	}
}
