using System;
using System.Collections.Generic;
using SS.CMS.Core.Cache;
using SS.CMS.Core.Cache.Content;
using SS.CMS.Core.Common;
using SS.CMS.Core.Models;
using SS.CMS.Plugin.Data;
using SS.CMS.Utils;
using Attr = SS.CMS.Core.Models.Attributes.ContentAttribute;

namespace SS.CMS.Core.Repositories
{
    public partial class ContentDao
    {
        private void UpdateTaxis(int channelId, int contentId, int taxis)
        {
            var updateNum = _repository.Update(Q
                .Set(Attr.Taxis, taxis)
                .Where(Attr.Id, contentId)
            );

            if (updateNum <= 0) return;

            ContentManager.RemoveCache(TableName, channelId);
        }

        public void UpdateArrangeTaxis(int channelId, string attributeName, bool isDesc)
        {
            var query = Q
            .Where(Attr.ChannelId, channelId)
            .OrWhere(Attr.ChannelId, -channelId);
            //由于页面排序是按Taxis的Desc排序的，所以这里sql里面的ASC/DESC取反
            if (isDesc)
            {
                query.OrderBy(attributeName);
            }
            else
            {
                query.OrderByDesc(attributeName);
            }

            var list = _repository.GetAll<(int id, string isTop)>(query);
            var taxis = 1;
            foreach ((int id, string isTop) in list)
            {
                _repository.Update(Q.Set(Attr.Taxis, taxis++).Set(Attr.IsTop, isTop).Where(Attr.Id, id));
            }

            ContentManager.RemoveCache(TableName, channelId);
        }

        public bool SetTaxisToUp(int channelId, int contentId, bool isTop)
        {
            var taxis = GetTaxis(contentId);

            var result = _repository.Get<(int HigherId, int HigherTaxis)?>(Q
                .Select(Attr.Id, Attr.Taxis)
                .Where(Attr.ChannelId, channelId)
                .Where(Attr.Taxis, ">", taxis)
                .Where(Attr.Taxis, isTop ? ">=" : "<", TaxisIsTopStartValue)
                .OrderBy(Attr.Taxis));

            var higherId = 0;
            var higherTaxis = 0;
            if (result != null)
            {
                higherId = result.Value.HigherId;
                higherTaxis = result.Value.HigherTaxis;
            }

            if (higherId == 0) return false;

            UpdateTaxis(channelId, contentId, higherTaxis);
            UpdateTaxis(channelId, higherId, taxis);

            return true;
        }

        public bool SetTaxisToDown(int channelId, int contentId, bool isTop)
        {
            var taxis = GetTaxis(contentId);

            var result = _repository.Get<(int LowerId, int LowerTaxis)?>(Q
                .Select(Attr.Id, Attr.Taxis)
                .Where(Attr.ChannelId, channelId)
                .Where(Attr.Taxis, "<", taxis)
                .Where(Attr.Taxis, isTop ? ">=" : "<", TaxisIsTopStartValue)
                .OrderByDesc(Attr.Taxis));

            var lowerId = 0;
            var lowerTaxis = 0;
            if (result != null)
            {
                lowerId = result.Value.LowerId;
                lowerTaxis = result.Value.LowerTaxis;
            }

            if (lowerId == 0) return false;

            UpdateTaxis(channelId, contentId, lowerTaxis);
            UpdateTaxis(channelId, lowerId, taxis);

            return true;
        }

        public void Update(SiteInfo siteInfo, ChannelInfo channelInfo, ContentInfo contentInfo)
        {
            if (contentInfo == null) return;

            if (siteInfo.IsAutoPageInTextEditor && !string.IsNullOrEmpty(contentInfo.Content))
            {
                contentInfo.Content = ContentUtility.GetAutoPageContent(contentInfo.Content,
                        siteInfo.AutoPageWordNum);
            }

            //出现IsTop与Taxis不同步情况
            if (contentInfo.Top == false && contentInfo.Taxis >= TaxisIsTopStartValue)
            {
                contentInfo.Taxis = GetMaxTaxis(contentInfo.ChannelId, false) + 1;
            }
            else if (contentInfo.Top && contentInfo.Taxis < TaxisIsTopStartValue)
            {
                contentInfo.Taxis = GetMaxTaxis(contentInfo.ChannelId, true) + 1;
            }

            contentInfo.SiteId = siteInfo.Id;
            contentInfo.ChannelId = channelInfo.Id;
            contentInfo.LastEditDate = DateTime.Now;

            _repository.Update(contentInfo);

            ContentManager.UpdateCache(siteInfo, channelInfo, contentInfo);
            ContentManager.RemoveCountCache(TableName);
        }

        public void AddDownloads(int channelId, int contentId)
        {
            _repository.Increment(Attr.Downloads, Q
                .Where(Attr.Id, contentId));

            ContentManager.RemoveCache(TableName, channelId);
        }

        public void UpdateRestoreContentsByTrash(int siteId, int channelId)
        {
            var updateNum = _repository.Update(Q
                .SetRaw($"{Attr.ChannelId} = -{Attr.ChannelId}")
                .Set(Attr.LastEditDate, DateTime.Now)
                .Where(Attr.SiteId, siteId)
                .Where(Attr.ChannelId, "<", 0)
            );

            if (updateNum <= 0) return;

            ContentManager.RemoveCache(TableName, channelId);
        }

        public void UpdateIsChecked(int siteId, int channelId, List<int> contentIdList, int translateChannelId, string userName, bool isChecked, int checkedLevel, string reasons)
        {
            if (isChecked)
            {
                checkedLevel = 0;
            }

            var checkDate = DateTime.Now;

            foreach (var contentId in contentIdList)
            {
                var settingsXml = _repository.Get<string>(Q
                    .Select(Attr.SettingsXml)
                    .Where(Attr.Id, contentId));

                var attributes = TranslateUtils.JsonDeserialize(settingsXml, new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase));

                attributes[Attr.CheckUserName] = userName;
                attributes[Attr.CheckDate] = DateUtils.GetDateAndTimeString(checkDate);
                attributes[Attr.CheckReasons] = reasons;

                settingsXml = TranslateUtils.JsonSerialize(attributes);

                if (translateChannelId > 0)
                {
                    _repository.Update(Q
                        .Set(Attr.IsChecked, isChecked.ToString())
                        .Set(Attr.CheckedLevel, checkedLevel)
                        .Set(Attr.SettingsXml, settingsXml)
                        .Set(Attr.ChannelId, translateChannelId)
                        .Where(Attr.Id, contentId)
                    );
                }
                else
                {
                    _repository.Update(Q
                        .Set(Attr.IsChecked, isChecked.ToString())
                        .Set(Attr.CheckedLevel, checkedLevel)
                        .Set(Attr.SettingsXml, settingsXml)
                        .Where(Attr.Id, contentId)
                    );
                }

                DataProvider.ContentCheckDao.Insert(new ContentCheckInfo
                {
                    TableName = TableName,
                    SiteId = siteId,
                    ChannelId = channelId,
                    ContentId = contentId,
                    UserName = userName,
                    Checked = isChecked,
                    CheckedLevel = checkedLevel,
                    CheckDate = checkDate,
                    Reasons = reasons
                });
            }

            ContentManager.RemoveCache(TableName, channelId);
        }

        public void SetAutoPageContentToSite(int siteId)
        {
            var siteInfo = SiteManager.GetSiteInfo(siteId);
            if (!siteInfo.IsAutoPageInTextEditor) return;

            var resultList = _repository.GetAll<(int Id, int ChannelId, string Content)>(Q.Where(Attr.SiteId, siteId));

            foreach (var result in resultList)
            {
                if (!string.IsNullOrEmpty(result.Content))
                {
                    var content = ContentUtility.GetAutoPageContent(result.Content, siteInfo.AutoPageWordNum);

                    Update(result.ChannelId, result.Id, Attr.Content, content);
                }
            }
        }

        public void AddContentGroupList(int contentId, List<string> contentGroupList)
        {
            if (!GetChanelIdAndValue<string>(contentId, Attr.GroupNameCollection, out var channelId, out var value)) return;

            var list = TranslateUtils.StringCollectionToStringList(value);
            foreach (var groupName in contentGroupList)
            {
                if (!list.Contains(groupName)) list.Add(groupName);
            }
            Update(channelId, contentId, Attr.GroupNameCollection, TranslateUtils.ObjectCollectionToString(list));
        }

        public void Update(int channelId, int contentId, string name, string value)
        {
            var updateNum = _repository.Update(Q
                .Set(name, value)
                .Where(Attr.Id, contentId)
            );

            if (updateNum <= 0) return;

            ContentManager.RemoveCache(TableName, channelId);
        }

        public void UpdateTrashContents(int siteId, int channelId, IList<int> contentIdList)
        {
            var referenceIdList = GetReferenceIdList(contentIdList);
            if (referenceIdList.Count > 0)
            {
                DeleteReferenceContents(siteId, channelId, referenceIdList);
            }

            var updateNum = 0;

            if (contentIdList != null && contentIdList.Count > 0)
            {
                updateNum = _repository.Update(Q
                    .SetRaw($"{Attr.ChannelId} = -{Attr.ChannelId}")
                    .Set(Attr.LastEditDate, DateTime.Now)
                    .Where(Attr.SiteId, siteId)
                    .WhereIn(Attr.Id, contentIdList)
                );
            }

            if (updateNum <= 0) return;

            ContentManager.RemoveCache(TableName, channelId);
        }

        public void UpdateTrashContentsByChannelId(int siteId, int channelId)
        {
            var contentIdList = GetContentIdList(channelId);
            var referenceIdList = GetReferenceIdList(contentIdList);
            if (referenceIdList.Count > 0)
            {
                DeleteReferenceContents(siteId, channelId, referenceIdList);
            }

            var updateNum = _repository.Update(Q
                .SetRaw($"{Attr.ChannelId} = -{Attr.ChannelId}")
                .Set(Attr.LastEditDate, DateTime.Now)
                .Where(Attr.SiteId, siteId)
                .Where(Attr.ChannelId, channelId)
            );

            if (updateNum <= 0) return;

            ContentManager.RemoveCache(TableName, channelId);
        }
    }
}
