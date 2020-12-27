using System.Collections.Generic;
using System.Threading.Tasks;
using SSCMS.Core.Utils.Serialization.Atom.Atom.Core;
using SSCMS.Enums;
using SSCMS.Models;
using SSCMS.Services;
using SSCMS.Utils;

namespace SSCMS.Core.Utils.Serialization.Components
{
	public class ConfigurationIe
    {
        private readonly IDatabaseManager _databaseManager;
        private readonly CacheUtils _caching;
		private readonly Site _site;
		private readonly string _filePath;

		private const string DefaultIndexTemplateName = "DefaultIndexTemplateName";
		private const string DefaultChannelTemplateName = "DefaultChannelTemplateName";
		private const string DefaultContentTemplateName = "DefaultContentTemplateName";
		private const string DefaultFileTemplateName = "DefaultFileTemplateName";

		public ConfigurationIe(IDatabaseManager databaseManager, CacheUtils caching, Site site, string filePath)
        {
			_databaseManager = databaseManager;
            _caching = caching;
			_site = site;
			_filePath = filePath;
		}

		public async Task ExportAsync()
		{
			var feed = AtomUtility.GetEmptyFeed();

            AtomUtility.AddDcElement(feed.AdditionalElements, new List<string> { nameof(Site.Id), "PublishmentSystemId" }, _site.Id.ToString());
            AtomUtility.AddDcElement(feed.AdditionalElements, new List<string> { nameof(Site.SiteDir), "PublishmentSystemDir" }, _site.SiteDir);
			AtomUtility.AddDcElement(feed.AdditionalElements, new List<string> { nameof(Site.SiteName), "PublishmentSystemName" }, _site.SiteName);
            AtomUtility.AddDcElement(feed.AdditionalElements, nameof(Site.SiteType), _site.SiteType);
			AtomUtility.AddDcElement(feed.AdditionalElements, nameof(Site.ImageUrl), _site.ImageUrl);
            AtomUtility.AddDcElement(feed.AdditionalElements, nameof(Site.Keywords), _site.Keywords);
            AtomUtility.AddDcElement(feed.AdditionalElements, nameof(Site.Description), _site.Description);
            AtomUtility.AddDcElement(feed.AdditionalElements, new List<string> { nameof(Site.TableName), "AuxiliaryTableForContent" }, _site.TableName);
            AtomUtility.AddDcElement(feed.AdditionalElements, nameof(Site.Taxis), _site.Taxis.ToString());
            AtomUtility.AddDcElement(feed.AdditionalElements, "SettingsXml", TranslateUtils.JsonSerialize(_site.ToDictionary(null)));
			AtomUtility.AddDcElement(feed.AdditionalElements, "ExtendValues", AtomUtility.Encrypt(TranslateUtils.JsonSerialize(_site)));

            var indexTemplateId = await _databaseManager.TemplateRepository.GetDefaultTemplateIdAsync(_site.Id, TemplateType.IndexPageTemplate);
			if (indexTemplateId != 0)
			{
                var indexTemplateName = await _databaseManager.TemplateRepository.GetTemplateNameAsync(indexTemplateId);
				AtomUtility.AddDcElement(feed.AdditionalElements, DefaultIndexTemplateName, indexTemplateName);
			}

            var channelTemplateId = await _databaseManager.TemplateRepository.GetDefaultTemplateIdAsync(_site.Id, TemplateType.ChannelTemplate);
			if (channelTemplateId != 0)
			{
                var channelTemplateName = await _databaseManager.TemplateRepository.GetTemplateNameAsync(channelTemplateId);
				AtomUtility.AddDcElement(feed.AdditionalElements, DefaultChannelTemplateName, channelTemplateName);
			}

            var contentTemplateId = await _databaseManager.TemplateRepository.GetDefaultTemplateIdAsync(_site.Id, TemplateType.ContentTemplate);
			if (contentTemplateId != 0)
			{
                var contentTemplateName = await _databaseManager.TemplateRepository.GetTemplateNameAsync(contentTemplateId);
				AtomUtility.AddDcElement(feed.AdditionalElements, DefaultContentTemplateName, contentTemplateName);
			}

            var fileTemplateId = await _databaseManager.TemplateRepository.GetDefaultTemplateIdAsync(_site.Id, TemplateType.FileTemplate);
			if (fileTemplateId != 0)
			{
                var fileTemplateName = await _databaseManager.TemplateRepository.GetTemplateNameAsync(fileTemplateId);
				AtomUtility.AddDcElement(feed.AdditionalElements, DefaultFileTemplateName, fileTemplateName);
			}

			var channelGroupList = await _databaseManager.ChannelGroupRepository.GetChannelGroupsAsync(_site.Id);
			var channelGroupIe = new ChannelGroupIe(_databaseManager, _caching);
            foreach (var channelGroup in channelGroupList)
			{
				var entry = channelGroupIe.Export(channelGroup);
                feed.Entries.Add(entry);
			}

			var contentGroupList = await _databaseManager.ContentGroupRepository.GetContentGroupsAsync(_site.Id);
            var contentGroupIe = new ContentGroupIe(_databaseManager, _caching);
            foreach (var contentGroup in contentGroupList)
			{
				var entry = contentGroupIe.Export(contentGroup);
				feed.Entries.Add(entry);
			}

            var contentTagList = await _databaseManager.ContentTagRepository.GetTagsAsync(_site.Id);
            var contentTagIe = new ContentTagIe(_databaseManager, _caching);
            foreach (var contentTag in contentTagList)
            {
                var entry = contentTagIe.Export(contentTag);
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

		public async Task ImportAsync(string guid)
		{
			if (!FileUtils.IsFileExists(_filePath)) return;

            var feed = AtomFeed.Load(FileUtils.GetFileStreamReadOnly(_filePath));

            var extendValues = AtomUtility.GetDcElementContent(feed.AdditionalElements,
                "ExtendValues");
            if (!string.IsNullOrEmpty(extendValues))
            {
				_site.LoadExtend(AtomUtility.Decrypt(extendValues));
            }
            else
            {
                var json = AtomUtility.GetDcElementContent(feed.AdditionalElements,
                    "SettingsXml");
                if (!string.IsNullOrEmpty(json))
                {
                    var dict = ListUtils.ToDictionary(json);
                    foreach (var o in dict)
                    {
                        _site.Set(o.Key, o.Value);
                    }
                }
			}

            _site.IsSeparatedWeb = false;
            _site.IsCreateDoubleClick = false;

			//_site.SiteType = AtomUtility.GetDcElementContent(feed.AdditionalElements,
   //             nameof(Site.SiteType));
            _site.ImageUrl = AtomUtility.GetDcElementContent(feed.AdditionalElements,
                nameof(Site.ImageUrl));
            _site.Keywords = AtomUtility.GetDcElementContent(feed.AdditionalElements,
                nameof(Site.Keywords));
            _site.Description = AtomUtility.GetDcElementContent(feed.AdditionalElements,
                nameof(Site.Description));

            _caching.SetProcess(guid, "更新站点配置...");
			await _databaseManager.SiteRepository.UpdateAsync(_site);

			var indexTemplateName = AtomUtility.GetDcElementContent(feed.AdditionalElements, DefaultIndexTemplateName);
			if (!string.IsNullOrEmpty(indexTemplateName))
			{
				var indexTemplateId = await _databaseManager.TemplateRepository.GetTemplateIdByTemplateNameAsync(_site.Id, TemplateType.IndexPageTemplate, indexTemplateName);
				if (indexTemplateId != 0)
				{
					await _databaseManager.TemplateRepository.SetDefaultAsync(indexTemplateId);
				}
			}

			var channelTemplateName = AtomUtility.GetDcElementContent(feed.AdditionalElements, DefaultChannelTemplateName);
			if (!string.IsNullOrEmpty(channelTemplateName))
			{
                var channelTemplateId = await _databaseManager.TemplateRepository.GetTemplateIdByTemplateNameAsync(_site.Id, TemplateType.ChannelTemplate, channelTemplateName);
				if (channelTemplateId != 0)
				{
					await _databaseManager.TemplateRepository.SetDefaultAsync(channelTemplateId);
				}
			}

			var contentTemplateName = AtomUtility.GetDcElementContent(feed.AdditionalElements, DefaultContentTemplateName);
			if (!string.IsNullOrEmpty(contentTemplateName))
			{
                var contentTemplateId = await _databaseManager.TemplateRepository.GetTemplateIdByTemplateNameAsync(_site.Id, TemplateType.ContentTemplate, contentTemplateName);
				if (contentTemplateId != 0)
				{
					await _databaseManager.TemplateRepository.SetDefaultAsync(contentTemplateId);
				}
			}

			var fileTemplateName = AtomUtility.GetDcElementContent(feed.AdditionalElements, DefaultFileTemplateName);
			if (!string.IsNullOrEmpty(fileTemplateName))
			{
                var fileTemplateId = await _databaseManager.TemplateRepository.GetTemplateIdByTemplateNameAsync(_site.Id, TemplateType.FileTemplate, fileTemplateName);
				if (fileTemplateId != 0)
				{
					await _databaseManager.TemplateRepository.SetDefaultAsync(fileTemplateId);
				}
			}

            var channelGroupIe = new ChannelGroupIe(_databaseManager, _caching);
            await channelGroupIe.ImportAsync(feed, _site.Id, guid);

            var contentGroupIe = new ContentGroupIe(_databaseManager, _caching);
            await contentGroupIe.ImportAsync(feed, _site.Id, guid);

            var contentTagIe = new ContentTagIe(_databaseManager, _caching);
            await contentTagIe.ImportAsync(feed, _site.Id, guid);
		}

	}
}
