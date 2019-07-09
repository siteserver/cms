using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using SS.CMS.Core.Models.Attributes;
using SS.CMS.Enums;
using SS.CMS.Models;
using SS.CMS.Services;
using SS.CMS.Utils;

namespace SS.CMS.Core.Services
{
    public partial class FileManager
    {
        public async Task TranslateAsync(ICreateManager createManager, SiteInfo siteInfo, int channelId, int contentId, string translateCollection, TranslateContentType translateType)
        {
            var translateList = TranslateUtils.StringCollectionToStringList(translateCollection);
            foreach (var translate in translateList)
            {
                if (string.IsNullOrEmpty(translate)) continue;

                var translates = translate.Split('_');
                if (translates.Length != 2) continue;

                var targetSiteId = TranslateUtils.ToInt(translates[0]);
                var targetChannelId = TranslateUtils.ToInt(translates[1]);

                await TranslateAsync(createManager, siteInfo, channelId, contentId, targetSiteId, targetChannelId, translateType);
            }
        }

        public async Task TranslateAsync(ICreateManager createManager, SiteInfo siteInfo, int channelId, int contentId, int targetSiteId, int targetChannelId, TranslateContentType translateType)
        {
            if (siteInfo == null || channelId <= 0 || contentId <= 0 || targetSiteId <= 0 || targetChannelId <= 0) return;

            var targetSiteInfo = await _siteRepository.GetSiteInfoAsync(targetSiteId);
            var targetChannelInfo = await _channelRepository.GetChannelInfoAsync(targetChannelId);
            var targetTableName = await _channelRepository.GetTableNameAsync(_pluginManager, targetSiteInfo, targetChannelInfo);

            var channelInfo = await _channelRepository.GetChannelInfoAsync(channelId);
            var contentInfo = await channelInfo.ContentRepository.GetContentInfoAsync(contentId);

            if (contentInfo == null) return;

            if (translateType == TranslateContentType.Copy)
            {
                MoveFileByContentInfo(siteInfo, targetSiteInfo, contentInfo);

                contentInfo.SiteId = targetSiteId;
                contentInfo.SourceId = contentInfo.ChannelId;
                contentInfo.ChannelId = targetChannelId;
                contentInfo.Set(ContentAttribute.TranslateContentType, TranslateContentType.Copy.ToString());
                //contentInfo.Attributes.Add(ContentAttribute.TranslateContentType, TranslateContentType.Copy.ToString());
                var theContentId = await targetChannelInfo.ContentRepository.InsertAsync(targetSiteInfo, targetChannelInfo, contentInfo);

                foreach (var service in await _pluginManager.GetServicesAsync())
                {
                    try
                    {
                        service.OnContentTranslateCompleted(new ContentTranslateEventArgs(siteInfo.Id, channelInfo.Id, contentId, targetSiteId, targetChannelId, theContentId));
                    }
                    catch (Exception ex)
                    {
                        await _errorLogRepository.AddErrorLogAsync(service.PluginId, ex, nameof(service.OnContentTranslateCompleted));
                    }
                }

                await createManager.AddCreateContentTaskAsync(targetSiteInfo.Id, contentInfo.ChannelId, theContentId);
                await createManager.TriggerContentChangedEventAsync(targetSiteInfo.Id, contentInfo.ChannelId);
            }
            else if (translateType == TranslateContentType.Cut)
            {
                MoveFileByContentInfo(siteInfo, targetSiteInfo, contentInfo);

                contentInfo.SiteId = targetSiteId;
                contentInfo.SourceId = contentInfo.ChannelId;
                contentInfo.ChannelId = targetChannelId;
                contentInfo.Set(ContentAttribute.TranslateContentType, TranslateContentType.Cut.ToString());
                //contentInfo.Attributes.Add(ContentAttribute.TranslateContentType, TranslateContentType.Cut.ToString());

                var newContentId = await targetChannelInfo.ContentRepository.InsertAsync(targetSiteInfo, targetChannelInfo, contentInfo);

                foreach (var service in await _pluginManager.GetServicesAsync())
                {
                    try
                    {
                        service.OnContentTranslateCompleted(new ContentTranslateEventArgs(siteInfo.Id, channelInfo.Id, contentId, targetSiteId, targetChannelId, newContentId));
                    }
                    catch (Exception ex)
                    {
                        await _errorLogRepository.AddErrorLogAsync(service.PluginId, ex, nameof(service.OnContentTranslateCompleted));
                    }
                }

                await DeleteAsync(siteInfo, channelInfo, contentId);

                //DataProvider.ContentRepository.DeleteContents(siteInfo.Id, tableName, TranslateUtils.ToIntList(contentId), channelId);

                await createManager.AddCreateContentTaskAsync(targetSiteInfo.Id, contentInfo.ChannelId, newContentId);
                await createManager.TriggerContentChangedEventAsync(targetSiteInfo.Id, contentInfo.ChannelId);
            }
            else if (translateType == TranslateContentType.Reference)
            {
                if (contentInfo.ReferenceId != 0) return;

                contentInfo.SiteId = targetSiteId;
                contentInfo.SourceId = contentInfo.ChannelId;
                contentInfo.ChannelId = targetChannelId;
                contentInfo.ReferenceId = contentId;
                contentInfo.Set(ContentAttribute.TranslateContentType, TranslateContentType.Reference.ToString());
                //contentInfo.Attributes.Add(ContentAttribute.TranslateContentType, TranslateContentType.Reference.ToString());
                int theContentId = await targetChannelInfo.ContentRepository.InsertAsync(targetSiteInfo, targetChannelInfo, contentInfo);

                await createManager.AddCreateContentTaskAsync(targetSiteInfo.Id, contentInfo.ChannelId, theContentId);
                await createManager.TriggerContentChangedEventAsync(targetSiteInfo.Id, contentInfo.ChannelId);
            }
            else if (translateType == TranslateContentType.ReferenceContent)
            {
                if (contentInfo.ReferenceId != 0) return;

                MoveFileByContentInfo(siteInfo, targetSiteInfo, contentInfo);

                contentInfo.SiteId = targetSiteId;
                contentInfo.SourceId = contentInfo.ChannelId;
                contentInfo.ChannelId = targetChannelId;
                contentInfo.ReferenceId = contentId;
                contentInfo.Set(ContentAttribute.TranslateContentType, TranslateContentType.ReferenceContent.ToString());
                var theContentId = await targetChannelInfo.ContentRepository.InsertAsync(targetSiteInfo, targetChannelInfo, contentInfo);

                foreach (var service in await _pluginManager.GetServicesAsync())
                {
                    try
                    {
                        service.OnContentTranslateCompleted(new ContentTranslateEventArgs(siteInfo.Id, channelInfo.Id, contentId, targetSiteId, targetChannelId, theContentId));
                    }
                    catch (Exception ex)
                    {
                        await _errorLogRepository.AddErrorLogAsync(service.PluginId, ex, nameof(service.OnContentTranslateCompleted));
                    }
                }

                await createManager.AddCreateContentTaskAsync(targetSiteInfo.Id, contentInfo.ChannelId, theContentId);
                await createManager.TriggerContentChangedEventAsync(targetSiteInfo.Id, contentInfo.ChannelId);
            }
        }

        public async Task DeleteAsync(SiteInfo siteInfo, ChannelInfo channelInfo, int contentId)
        {
            if (siteInfo == null || contentId <= 0) return;

            await channelInfo.ContentRepository.DeleteAsync(siteInfo.Id, contentId);

            await _tagRepository.RemoveTagsAsync(siteInfo.Id, contentId);

            foreach (var service in await _pluginManager.GetServicesAsync())
            {
                try
                {
                    service.OnContentDeleteCompleted(new ContentEventArgs(siteInfo.Id, channelInfo.Id, contentId));
                }
                catch (Exception ex)
                {
                    await _errorLogRepository.AddErrorLogAsync(service.PluginId, ex, nameof(service.OnContentDeleteCompleted));
                }
            }

            channelInfo.ContentRepository.RemoveCache(channelInfo.ContentRepository.TableName, channelInfo.Id);
        }

        public string TextEditorContentEncode(SiteInfo siteInfo, string content)
        {
            if (siteInfo == null) return content;

            if (siteInfo.IsSaveImageInTextEditor && !string.IsNullOrEmpty(content))
            {
                content = SaveImage(siteInfo, content);
            }

            var builder = new StringBuilder(content);

            var url = _urlManager.GetWebUrl(siteInfo);
            if (!string.IsNullOrEmpty(url) && url != "/")
            {
                StringUtils.ReplaceHrefOrSrc(builder, url, "@");
            }
            //if (!string.IsNullOrEmpty(url))
            //{
            //    StringUtils.ReplaceHrefOrSrc(builder, url, "@");
            //}

            var relatedSiteUrl = _urlManager.ParseNavigationUrl($"~/{siteInfo.SiteDir}");
            StringUtils.ReplaceHrefOrSrc(builder, relatedSiteUrl, "@");

            builder.Replace("@'@", "'@");
            builder.Replace("@\"@", "\"@");

            return builder.ToString();
        }

        public string TextEditorContentDecode(SiteInfo siteInfo, string content, bool isLocal)
        {
            if (siteInfo == null) return content;

            var builder = new StringBuilder(content);

            var virtualAssetsUrl = $"@/{siteInfo.AssetsDir}";
            var webUrl = _urlManager.GetWebUrl(siteInfo);
            string assetsUrl;
            if (isLocal)
            {
                assetsUrl = _urlManager.GetSiteUrl(siteInfo,
                    siteInfo.AssetsDir, true);
            }
            else
            {
                assetsUrl = _urlManager.GetAssetsUrl(siteInfo);
            }
            StringUtils.ReplaceHrefOrSrc(builder, virtualAssetsUrl, assetsUrl);
            StringUtils.ReplaceHrefOrSrc(builder, "@/", PageUtils.AddEndSlashToUrl(webUrl));
            StringUtils.ReplaceHrefOrSrc(builder, "@", PageUtils.AddEndSlashToUrl(webUrl));

            builder.Replace("&#xa0;", "&nbsp;");

            return builder.ToString();
        }

        public async Task DeleteContentsByPageAsync(SiteInfo siteInfo, List<int> channelIdList)
        {
            foreach (var channelId in channelIdList)
            {
                var channelInfo = await _channelRepository.GetChannelInfoAsync(channelId);
                var contentIdList = await channelInfo.ContentRepository.GetContentIdListAsync(channelId);
                foreach (var contentId in contentIdList)
                {
                    var filePath = await _pathManager.GetContentPageFilePathAsync(siteInfo, channelId, contentId, 0);
                    FileUtils.DeleteFileIfExists(filePath);
                    DeletePagingFiles(filePath);
                    DirectoryUtils.DeleteEmptyDirectory(DirectoryUtils.GetDirectoryPath(filePath));
                }
            }
        }

        public async Task DeleteContentsAsync(SiteInfo siteInfo, int channelId, IEnumerable<int> contentIdList)
        {
            foreach (var contentId in contentIdList)
            {
                await DeleteContentAsync(siteInfo, channelId, contentId);
            }
        }

        public async Task DeleteContentAsync(SiteInfo siteInfo, int channelId, int contentId)
        {
            var filePath = await _pathManager.GetContentPageFilePathAsync(siteInfo, channelId, contentId, 0);
            FileUtils.DeleteFileIfExists(filePath);
        }

        public async Task DeleteChannelsAsync(SiteInfo siteInfo, List<int> channelIdList)
        {
            foreach (var channelId in channelIdList)
            {
                var channelInfo = await _channelRepository.GetChannelInfoAsync(channelId);
                var filePath = await _pathManager.GetChannelPageFilePathAsync(siteInfo, channelId, 0);

                FileUtils.DeleteFileIfExists(filePath);

                var contentIdList = await channelInfo.ContentRepository.GetContentIdListAsync(channelId);
                await DeleteContentsAsync(siteInfo, channelId, contentIdList);
            }
        }

        public async Task DeleteChannelsByPageAsync(SiteInfo siteInfo, List<int> channelIdList)
        {
            foreach (var channelId in channelIdList)
            {
                if (channelId != siteInfo.Id)
                {
                    var filePath = await _pathManager.GetChannelPageFilePathAsync(siteInfo, channelId, 0);
                    FileUtils.DeleteFileIfExists(filePath);
                    DeletePagingFiles(filePath);
                    DirectoryUtils.DeleteEmptyDirectory(DirectoryUtils.GetDirectoryPath(filePath));
                }
            }
        }

        public void DeletePagingFiles(string filePath)
        {
            var fileName = (new FileInfo(filePath)).Name;
            fileName = fileName.Substring(0, fileName.IndexOf('.'));
            var filesPath = DirectoryUtils.GetFilePaths(DirectoryUtils.GetDirectoryPath(filePath));
            foreach (var otherFilePath in filesPath)
            {
                var otherFileName = (new FileInfo(otherFilePath)).Name;
                otherFileName = otherFileName.Substring(0, otherFileName.IndexOf('.'));
                if (otherFileName.Contains(fileName + "_"))
                {
                    var isNum = otherFileName.Replace(fileName + "_", string.Empty);
                    if (ConvertHelper.GetInteger(isNum) > 0)
                    {
                        FileUtils.DeleteFileIfExists(otherFilePath);
                    }
                }
            }
        }

        public async Task DeleteFilesAsync(SiteInfo siteInfo, List<int> templateIdList)
        {
            foreach (var templateId in templateIdList)
            {
                var templateInfo = await _templateRepository.GetTemplateInfoAsync(templateId);
                if (templateInfo == null || templateInfo.Type != TemplateType.FileTemplate)
                {
                    return;
                }

                var filePath = _pathManager.MapPath(siteInfo, templateInfo.CreatedFileFullName);

                FileUtils.DeleteFileIfExists(filePath);
            }
        }
    }
}