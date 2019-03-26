using System;
using SiteServer.CMS.Core;
using SiteServer.CMS.Core.Create;
using SiteServer.CMS.Core.Enumerations;
using SiteServer.CMS.Database.Attributes;
using SiteServer.CMS.Database.Core;
using SiteServer.CMS.Database.Models;
using SiteServer.CMS.Plugin;
using SiteServer.Plugin;
using SiteServer.Utils;

namespace SiteServer.CMS.Caches.Content
{
    public static partial class ContentManager
    {
        public static void Delete(SiteInfo siteInfo, ChannelInfo channelInfo, int contentId)
        {
            if (siteInfo == null || channelInfo == null || contentId <= 0) return;
            
            var tableName = ChannelManager.GetTableName(siteInfo, channelInfo);

            channelInfo.ContentRepository.Delete(siteInfo.Id, contentId);

            TagUtils.RemoveTags(siteInfo.Id, contentId);

            foreach (var service in PluginManager.Services)
            {
                try
                {
                    service.OnContentDeleteCompleted(new ContentEventArgs(siteInfo.Id, channelInfo.Id, contentId));
                }
                catch (Exception ex)
                {
                    LogUtils.AddErrorLog(service.PluginId, ex, nameof(service.OnContentDeleteCompleted));
                }
            }

            RemoveCache(tableName, channelInfo.Id);
        }

        public static void Translate(SiteInfo siteInfo, int channelId, int contentId, string translateCollection, ETranslateContentType translateType, string administratorName)
        {
            var translateList = TranslateUtils.StringCollectionToStringList(translateCollection);
            foreach (var translate in translateList)
            {
                if (string.IsNullOrEmpty(translate)) continue;

                var translates = translate.Split('_');
                if (translates.Length != 2) continue;

                var targetSiteId = TranslateUtils.ToInt(translates[0]);
                var targetChannelId = TranslateUtils.ToInt(translates[1]);

                Translate(siteInfo, channelId, contentId, targetSiteId, targetChannelId, translateType);
            }
        }

        public static void Translate(SiteInfo siteInfo, int channelId, int contentId, int targetSiteId, int targetChannelId, ETranslateContentType translateType)
        {
            if (siteInfo == null || channelId <= 0 || contentId <= 0 || targetSiteId <= 0 || targetChannelId <= 0) return;

            var targetSiteInfo = SiteManager.GetSiteInfo(targetSiteId);
            var targetChannelInfo = ChannelManager.GetChannelInfo(targetSiteId, targetChannelId);
            var targetTableName = ChannelManager.GetTableName(targetSiteInfo, targetChannelInfo);

            var channelInfo = ChannelManager.GetChannelInfo(siteInfo.Id, channelId);

            var contentInfo = GetContentInfo(siteInfo, channelInfo, contentId);

            if (contentInfo == null) return;

            if (translateType == ETranslateContentType.Copy)
            {
                FileUtility.MoveFileByContentInfo(siteInfo, targetSiteInfo, contentInfo);

                contentInfo.SiteId = targetSiteId;
                contentInfo.SourceId = contentInfo.ChannelId;
                contentInfo.ChannelId = targetChannelId;
                contentInfo.Set(ContentAttribute.TranslateContentType, ETranslateContentType.Copy.ToString());
                //contentInfo.Attributes.Add(ContentAttribute.TranslateContentType, ETranslateContentType.Copy.ToString());
                var theContentId = DataProvider.ContentRepository.Insert(targetTableName, targetSiteInfo, targetChannelInfo, contentInfo);

                foreach (var service in PluginManager.Services)
                {
                    try
                    {
                        service.OnContentTranslateCompleted(new ContentTranslateEventArgs(siteInfo.Id, channelInfo.Id, contentId, targetSiteId, targetChannelId, theContentId));
                    }
                    catch (Exception ex)
                    {
                        LogUtils.AddErrorLog(service.PluginId, ex, nameof(service.OnContentTranslateCompleted));
                    }
                }

                CreateManager.CreateContent(targetSiteInfo.Id, contentInfo.ChannelId, theContentId);
                CreateManager.TriggerContentChangedEvent(targetSiteInfo.Id, contentInfo.ChannelId);
            }
            else if (translateType == ETranslateContentType.Cut)
            {
                FileUtility.MoveFileByContentInfo(siteInfo, targetSiteInfo, contentInfo);

                contentInfo.SiteId = targetSiteId;
                contentInfo.SourceId = contentInfo.ChannelId;
                contentInfo.ChannelId = targetChannelId;
                contentInfo.Set(ContentAttribute.TranslateContentType, ETranslateContentType.Cut.ToString());
                //contentInfo.Attributes.Add(ContentAttribute.TranslateContentType, ETranslateContentType.Cut.ToString());

                var newContentId = DataProvider.ContentRepository.Insert(targetTableName, targetSiteInfo, targetChannelInfo, contentInfo);
                //channelInfo.ContentRepository.DeleteContents(siteInfo.Id, TranslateUtils.ToIntList(contentId), channelId);

                foreach (var service in PluginManager.Services)
                {
                    try
                    {
                        service.OnContentTranslateCompleted(new ContentTranslateEventArgs(siteInfo.Id, channelInfo.Id, contentId, targetSiteId, targetChannelId, newContentId));
                    }
                    catch (Exception ex)
                    {
                        LogUtils.AddErrorLog(service.PluginId, ex, nameof(service.OnContentTranslateCompleted));
                    }

                    //try
                    //{
                    //    service.OnContentDeleteCompleted(new ContentEventArgs(siteInfo.Id, channelInfo.Id, contentId));
                    //}
                    //catch (Exception ex)
                    //{
                    //    LogUtils.AddErrorLog(service.PluginId, ex, nameof(service.OnContentDeleteCompleted));
                    //}
                }

                Delete(siteInfo, channelInfo, contentId);

                CreateManager.CreateContent(targetSiteInfo.Id, contentInfo.ChannelId, newContentId);
                CreateManager.TriggerContentChangedEvent(targetSiteInfo.Id, contentInfo.ChannelId);
            }
            else if (translateType == ETranslateContentType.Reference)
            {
                if (contentInfo.ReferenceId != 0) return;

                contentInfo.SiteId = targetSiteId;
                contentInfo.SourceId = contentInfo.ChannelId;
                contentInfo.ChannelId = targetChannelId;
                contentInfo.ReferenceId = contentId;
                contentInfo.Set(ContentAttribute.TranslateContentType, ETranslateContentType.Reference.ToString());
                //contentInfo.Attributes.Add(ContentAttribute.TranslateContentType, ETranslateContentType.Reference.ToString());
                int theContentId = DataProvider.ContentRepository.Insert(targetTableName, targetSiteInfo, targetChannelInfo, contentInfo);

                CreateManager.CreateContent(targetSiteInfo.Id, contentInfo.ChannelId, theContentId);
                CreateManager.TriggerContentChangedEvent(targetSiteInfo.Id, contentInfo.ChannelId);
            }
            else if (translateType == ETranslateContentType.ReferenceContent)
            {
                if (contentInfo.ReferenceId != 0) return;

                FileUtility.MoveFileByContentInfo(siteInfo, targetSiteInfo, contentInfo);

                contentInfo.SiteId = targetSiteId;
                contentInfo.SourceId = contentInfo.ChannelId;
                contentInfo.ChannelId = targetChannelId;
                contentInfo.ReferenceId = contentId;
                contentInfo.Set(ContentAttribute.TranslateContentType, ETranslateContentType.ReferenceContent.ToString());
                var theContentId = DataProvider.ContentRepository.Insert(targetTableName, targetSiteInfo, targetChannelInfo, contentInfo);

                foreach (var service in PluginManager.Services)
                {
                    try
                    {
                        service.OnContentTranslateCompleted(new ContentTranslateEventArgs(siteInfo.Id, channelInfo.Id, contentId, targetSiteId, targetChannelId, theContentId));
                    }
                    catch (Exception ex)
                    {
                        LogUtils.AddErrorLog(service.PluginId, ex, nameof(service.OnContentTranslateCompleted));
                    }
                }

                CreateManager.CreateContent(targetSiteInfo.Id, contentInfo.ChannelId, theContentId);
                CreateManager.TriggerContentChangedEvent(targetSiteInfo.Id, contentInfo.ChannelId);
            }
        }
    }
}