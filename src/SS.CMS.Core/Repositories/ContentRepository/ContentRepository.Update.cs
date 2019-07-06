using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SS.CMS.Core.Common;
using SS.CMS.Data;
using SS.CMS.Models;
using SS.CMS.Utils;
using Attr = SS.CMS.Core.Models.Attributes.ContentAttribute;

namespace SS.CMS.Core.Repositories
{
    public partial class ContentRepository
    {
        private async Task UpdateTaxisAsync(int channelId, int contentId, int taxis)
        {
            var updateNum = await _repository.UpdateAsync(Q
                .Set(Attr.Taxis, taxis)
                .Where(Attr.Id, contentId)
            );

            if (updateNum <= 0) return;

            RemoveCache(TableName, channelId);
        }

        public async Task UpdateArrangeTaxisAsync(int channelId, string attributeName, bool isDesc)
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

            var list = await _repository.GetAllAsync<(int id, string isTop)>(query);
            var taxis = 1;
            foreach ((int id, string isTop) in list)
            {
                _repository.Update(Q.Set(Attr.Taxis, taxis++).Set(Attr.IsTop, isTop).Where(Attr.Id, id));
            }

            RemoveCache(TableName, channelId);
        }

        public async Task<bool> SetTaxisToUpAsync(int channelId, int contentId, bool isTop)
        {
            var taxis = GetTaxis(contentId);

            var result = await _repository.GetAsync<(int HigherId, int HigherTaxis)?>(Q
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

            await UpdateTaxisAsync(channelId, contentId, higherTaxis);
            await UpdateTaxisAsync(channelId, higherId, taxis);

            return true;
        }

        public async Task<bool> SetTaxisToDownAsync(int channelId, int contentId, bool isTop)
        {
            var taxis = GetTaxis(contentId);

            var result = await _repository.GetAsync<(int LowerId, int LowerTaxis)?>(Q
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

            await UpdateTaxisAsync(channelId, contentId, lowerTaxis);
            await UpdateTaxisAsync(channelId, lowerId, taxis);

            return true;
        }

        public async Task UpdateAsync(SiteInfo siteInfo, ChannelInfo channelInfo, ContentInfo contentInfo)
        {
            if (contentInfo == null) return;

            if (siteInfo.IsAutoPageInTextEditor && !string.IsNullOrEmpty(contentInfo.Content))
            {
                contentInfo.Content = ContentUtility.GetAutoPageContent(contentInfo.Content,
                        siteInfo.AutoPageWordNum);
            }

            //出现IsTop与Taxis不同步情况
            if (contentInfo.IsTop == false && contentInfo.Taxis >= TaxisIsTopStartValue)
            {
                contentInfo.Taxis = GetMaxTaxis(contentInfo.ChannelId, false) + 1;
            }
            else if (contentInfo.IsTop && contentInfo.Taxis < TaxisIsTopStartValue)
            {
                contentInfo.Taxis = GetMaxTaxis(contentInfo.ChannelId, true) + 1;
            }

            contentInfo.SiteId = siteInfo.Id;
            contentInfo.ChannelId = channelInfo.Id;

            _repository.Update(contentInfo);

            await UpdateCacheAsync(siteInfo, channelInfo, contentInfo);
            RemoveCountCache(TableName);
        }

        public async Task AddDownloadsAsync(int channelId, int contentId)
        {
            await _repository.IncrementAsync(Attr.Downloads, Q
                .Where(Attr.Id, contentId));

            RemoveCache(TableName, channelId);
        }

        public async Task UpdateRestoreContentsByTrashAsync(int siteId, int channelId)
        {
            var updateNum = await _repository.UpdateAsync(Q
                .SetRaw($"{Attr.ChannelId} = -{Attr.ChannelId}")
                .Where(Attr.SiteId, siteId)
                .Where(Attr.ChannelId, "<", 0)
            );

            if (updateNum <= 0) return;

            RemoveCache(TableName, channelId);
        }

        public async Task UpdateIsCheckedAsync(int siteId, int channelId, List<int> contentIdList, int translateChannelId, string userName, bool isChecked, int checkedLevel, string reasons)
        {
            if (isChecked)
            {
                checkedLevel = 0;
            }

            var checkDate = DateTime.Now;

            foreach (var contentId in contentIdList)
            {
                var settingsXml = _repository.Get<string>(Q
                    .Select(Attr.ExtendValues)
                    .Where(Attr.Id, contentId));

                var attributes = TranslateUtils.JsonDeserialize(settingsXml, new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase));

                attributes[Attr.CheckUserName] = userName;
                attributes[Attr.CheckDate] = DateUtils.GetDateAndTimeString(checkDate);
                attributes[Attr.CheckReasons] = reasons;

                settingsXml = TranslateUtils.JsonSerialize(attributes);

                if (translateChannelId > 0)
                {
                    await _repository.UpdateAsync(Q
                        .Set(Attr.IsChecked, isChecked.ToString())
                        .Set(Attr.CheckedLevel, checkedLevel)
                        .Set(Attr.ExtendValues, settingsXml)
                        .Set(Attr.ChannelId, translateChannelId)
                        .Where(Attr.Id, contentId)
                    );
                }
                else
                {
                    await _repository.UpdateAsync(Q
                        .Set(Attr.IsChecked, isChecked.ToString())
                        .Set(Attr.CheckedLevel, checkedLevel)
                        .Set(Attr.ExtendValues, settingsXml)
                        .Where(Attr.Id, contentId)
                    );
                }

                _contentCheckRepository.Insert(new ContentCheckInfo
                {
                    TableName = TableName,
                    SiteId = siteId,
                    ChannelId = channelId,
                    ContentId = contentId,
                    UserName = userName,
                    IsChecked = isChecked,
                    CheckedLevel = checkedLevel,
                    CheckDate = checkDate,
                    Reasons = reasons
                });
            }

            RemoveCache(TableName, channelId);
        }

        public async Task SetAutoPageContentToSiteAsync(int siteId)
        {
            var siteInfo = await _siteRepository.GetSiteInfoAsync(siteId);
            if (!siteInfo.IsAutoPageInTextEditor) return;

            var resultList = _repository.GetAll<(int Id, int ChannelId, string Content)>(Q.Where(Attr.SiteId, siteId));

            foreach (var result in resultList)
            {
                if (!string.IsNullOrEmpty(result.Content))
                {
                    var content = ContentUtility.GetAutoPageContent(result.Content, siteInfo.AutoPageWordNum);

                    await UpdateAsync(result.ChannelId, result.Id, Attr.Content, content);
                }
            }
        }

        public async Task AddContentGroupListAsync(int contentId, List<string> contentGroupList)
        {
            if (!GetChanelIdAndValue<string>(contentId, Attr.GroupNameCollection, out var channelId, out var value)) return;

            var list = TranslateUtils.StringCollectionToStringList(value);
            foreach (var groupName in contentGroupList)
            {
                if (!list.Contains(groupName)) list.Add(groupName);
            }
            await UpdateAsync(channelId, contentId, Attr.GroupNameCollection, TranslateUtils.ObjectCollectionToString(list));
        }

        public async Task UpdateAsync(int channelId, int contentId, string name, string value)
        {
            var updateNum = await _repository.UpdateAsync(Q
                .Set(name, value)
                .Where(Attr.Id, contentId)
            );

            if (updateNum <= 0) return;

            RemoveCache(TableName, channelId);
        }

        public async Task UpdateTrashContentsAsync(int siteId, int channelId, IList<int> contentIdList)
        {
            var referenceIdList = GetReferenceIdList(contentIdList);
            if (referenceIdList.Count > 0)
            {
                await DeleteReferenceContentsAsync(siteId, channelId, referenceIdList);
            }

            var updateNum = 0;

            if (contentIdList != null && contentIdList.Count > 0)
            {
                updateNum = _repository.Update(Q
                    .SetRaw($"{Attr.ChannelId} = -{Attr.ChannelId}")
                    .Where(Attr.SiteId, siteId)
                    .WhereIn(Attr.Id, contentIdList)
                );
            }

            if (updateNum <= 0) return;

            RemoveCache(TableName, channelId);
        }

        public async Task UpdateTrashContentsByChannelIdAsync(int siteId, int channelId)
        {
            var contentIdList = GetContentIdList(channelId);
            var referenceIdList = GetReferenceIdList(contentIdList);
            if (referenceIdList.Count > 0)
            {
                await DeleteReferenceContentsAsync(siteId, channelId, referenceIdList);
            }

            var updateNum = _repository.Update(Q
                .SetRaw($"{Attr.ChannelId} = -{Attr.ChannelId}")
                .Where(Attr.SiteId, siteId)
                .Where(Attr.ChannelId, channelId)
            );

            if (updateNum <= 0) return;

            RemoveCache(TableName, channelId);
        }
    }
}
