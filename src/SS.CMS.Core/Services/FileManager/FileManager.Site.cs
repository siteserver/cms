using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using SS.CMS.Core.Models.Attributes;
using SS.CMS.Enums;
using SS.CMS.Models;
using SS.CMS.Services.ICreateManager;
using SS.CMS.Utils;

namespace SS.CMS.Core.Services
{
    public partial class FileManager
    {
        public void Translate(ICreateManager createManager, SiteInfo siteInfo, int channelId, int contentId, string translateCollection, TranslateContentType translateType)
        {
            var translateList = TranslateUtils.StringCollectionToStringList(translateCollection);
            foreach (var translate in translateList)
            {
                if (string.IsNullOrEmpty(translate)) continue;

                var translates = translate.Split('_');
                if (translates.Length != 2) continue;

                var targetSiteId = TranslateUtils.ToInt(translates[0]);
                var targetChannelId = TranslateUtils.ToInt(translates[1]);

                Translate(createManager, siteInfo, channelId, contentId, targetSiteId, targetChannelId, translateType);
            }
        }

        public void Translate(ICreateManager createManager, SiteInfo siteInfo, int channelId, int contentId, int targetSiteId, int targetChannelId, TranslateContentType translateType)
        {
            if (siteInfo == null || channelId <= 0 || contentId <= 0 || targetSiteId <= 0 || targetChannelId <= 0) return;

            var targetSiteInfo = _siteRepository.GetSiteInfo(targetSiteId);
            var targetChannelInfo = _channelRepository.GetChannelInfo(targetSiteId, targetChannelId);
            var targetTableName = _channelRepository.GetTableName(_pluginManager, targetSiteInfo, targetChannelInfo);

            var channelInfo = _channelRepository.GetChannelInfo(siteInfo.Id, channelId);
            var contentInfo = channelInfo.ContentRepository.GetContentInfo(siteInfo, channelInfo, contentId);

            if (contentInfo == null) return;

            if (translateType == TranslateContentType.Copy)
            {
                MoveFileByContentInfo(siteInfo, targetSiteInfo, contentInfo);

                contentInfo.SiteId = targetSiteId;
                contentInfo.SourceId = contentInfo.ChannelId;
                contentInfo.ChannelId = targetChannelId;
                contentInfo.Set(ContentAttribute.TranslateContentType, TranslateContentType.Copy.ToString());
                //contentInfo.Attributes.Add(ContentAttribute.TranslateContentType, TranslateContentType.Copy.ToString());
                var theContentId = targetChannelInfo.ContentRepository.Insert(targetSiteInfo, targetChannelInfo, contentInfo);

                foreach (var service in _pluginManager.Services)
                {
                    try
                    {
                        service.OnContentTranslateCompleted(new ContentTranslateEventArgs(siteInfo.Id, channelInfo.Id, contentId, targetSiteId, targetChannelId, theContentId));
                    }
                    catch (Exception ex)
                    {
                        _errorLogRepository.AddErrorLog(service.PluginId, ex, nameof(service.OnContentTranslateCompleted));
                    }
                }

                createManager.CreateContent(targetSiteInfo.Id, contentInfo.ChannelId, theContentId);
                createManager.TriggerContentChangedEvent(targetSiteInfo.Id, contentInfo.ChannelId);
            }
            else if (translateType == TranslateContentType.Cut)
            {
                MoveFileByContentInfo(siteInfo, targetSiteInfo, contentInfo);

                contentInfo.SiteId = targetSiteId;
                contentInfo.SourceId = contentInfo.ChannelId;
                contentInfo.ChannelId = targetChannelId;
                contentInfo.Set(ContentAttribute.TranslateContentType, TranslateContentType.Cut.ToString());
                //contentInfo.Attributes.Add(ContentAttribute.TranslateContentType, TranslateContentType.Cut.ToString());

                var newContentId = targetChannelInfo.ContentRepository.Insert(targetSiteInfo, targetChannelInfo, contentInfo);

                foreach (var service in _pluginManager.Services)
                {
                    try
                    {
                        service.OnContentTranslateCompleted(new ContentTranslateEventArgs(siteInfo.Id, channelInfo.Id, contentId, targetSiteId, targetChannelId, newContentId));
                    }
                    catch (Exception ex)
                    {
                        _errorLogRepository.AddErrorLog(service.PluginId, ex, nameof(service.OnContentTranslateCompleted));
                    }
                }

                Delete(siteInfo, channelInfo, contentId);

                //DataProvider.ContentRepository.DeleteContents(siteInfo.Id, tableName, TranslateUtils.ToIntList(contentId), channelId);

                createManager.CreateContent(targetSiteInfo.Id, contentInfo.ChannelId, newContentId);
                createManager.TriggerContentChangedEvent(targetSiteInfo.Id, contentInfo.ChannelId);
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
                int theContentId = targetChannelInfo.ContentRepository.Insert(targetSiteInfo, targetChannelInfo, contentInfo);

                createManager.CreateContent(targetSiteInfo.Id, contentInfo.ChannelId, theContentId);
                createManager.TriggerContentChangedEvent(targetSiteInfo.Id, contentInfo.ChannelId);
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
                var theContentId = targetChannelInfo.ContentRepository.Insert(targetSiteInfo, targetChannelInfo, contentInfo);

                foreach (var service in _pluginManager.Services)
                {
                    try
                    {
                        service.OnContentTranslateCompleted(new ContentTranslateEventArgs(siteInfo.Id, channelInfo.Id, contentId, targetSiteId, targetChannelId, theContentId));
                    }
                    catch (Exception ex)
                    {
                        _errorLogRepository.AddErrorLog(service.PluginId, ex, nameof(service.OnContentTranslateCompleted));
                    }
                }

                createManager.CreateContent(targetSiteInfo.Id, contentInfo.ChannelId, theContentId);
                createManager.TriggerContentChangedEvent(targetSiteInfo.Id, contentInfo.ChannelId);
            }
        }

        public void Delete(SiteInfo siteInfo, ChannelInfo channelInfo, int contentId)
        {
            if (siteInfo == null || contentId <= 0) return;

            channelInfo.ContentRepository.Delete(siteInfo.Id, contentId);

            _tagRepository.RemoveTags(siteInfo.Id, contentId);

            foreach (var service in _pluginManager.Services)
            {
                try
                {
                    service.OnContentDeleteCompleted(new ContentEventArgs(siteInfo.Id, channelInfo.Id, contentId));
                }
                catch (Exception ex)
                {
                    _errorLogRepository.AddErrorLog(service.PluginId, ex, nameof(service.OnContentDeleteCompleted));
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

        public void DeleteContentsByPage(SiteInfo siteInfo, List<int> channelIdList)
        {
            foreach (var channelId in channelIdList)
            {
                var channelInfo = _channelRepository.GetChannelInfo(siteInfo.Id, channelId);
                var contentIdList = channelInfo.ContentRepository.GetContentIdList(channelId);
                if (contentIdList.Count > 0)
                {
                    foreach (var contentId in contentIdList)
                    {
                        var filePath = _pathManager.GetContentPageFilePath(siteInfo, channelId, contentId, 0);
                        FileUtils.DeleteFileIfExists(filePath);
                        DeletePagingFiles(filePath);
                        DirectoryUtils.DeleteEmptyDirectory(DirectoryUtils.GetDirectoryPath(filePath));
                    }
                }
            }
        }

        public void DeleteContents(SiteInfo siteInfo, int channelId, IList<int> contentIdList)
        {
            foreach (var contentId in contentIdList)
            {
                DeleteContent(siteInfo, channelId, contentId);
            }
        }

        public void DeleteContent(SiteInfo siteInfo, int channelId, int contentId)
        {
            var filePath = _pathManager.GetContentPageFilePath(siteInfo, channelId, contentId, 0);
            FileUtils.DeleteFileIfExists(filePath);
        }

        public void DeleteChannels(SiteInfo siteInfo, List<int> channelIdList)
        {
            foreach (var channelId in channelIdList)
            {
                var channelInfo = _channelRepository.GetChannelInfo(siteInfo.Id, channelId);
                var filePath = _pathManager.GetChannelPageFilePath(siteInfo, channelId, 0);

                FileUtils.DeleteFileIfExists(filePath);

                var contentIdList = channelInfo.ContentRepository.GetContentIdList(channelId);
                if (contentIdList.Count > 0)
                {
                    DeleteContents(siteInfo, channelId, contentIdList);
                }
            }
        }

        public void DeleteChannelsByPage(SiteInfo siteInfo, List<int> channelIdList)
        {
            foreach (var channelId in channelIdList)
            {
                if (channelId != siteInfo.Id)
                {
                    var filePath = _pathManager.GetChannelPageFilePath(siteInfo, channelId, 0);
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

        public void DeleteFiles(SiteInfo siteInfo, List<int> templateIdList)
        {
            foreach (var templateId in templateIdList)
            {
                var templateInfo = _templateRepository.GetTemplateInfo(siteInfo.Id, templateId);
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