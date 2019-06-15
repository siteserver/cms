using System;
using System.Collections.Generic;
using Dapper;
using SS.CMS.Abstractions.Enums;
using SS.CMS.Abstractions.Models;
using SS.CMS.Core.Cache;
using SS.CMS.Core.Common;
using SS.CMS.Core.Models;
using SS.CMS.Data;
using SS.CMS.Utils;
using SS.CMS.Utils.Enumerations;
using Attr = SS.CMS.Core.Models.Attributes.ContentAttribute;

namespace SS.CMS.Core.Repositories
{
    public partial class ContentRepository
    {
        private const int TaxisIsTopStartValue = 2000000000;

        public int GetMaxTaxis(int channelId, bool isTop)
        {
            var maxTaxis = 0;

            if (isTop)
            {
                maxTaxis = TaxisIsTopStartValue;

                var max = _repository.Max(Attr.Taxis, Q
                    .Where(Attr.ChannelId, channelId)
                    .Where(Attr.Taxis, ">=", TaxisIsTopStartValue)
                );

                if (max != null)
                {
                    maxTaxis = max.Value;
                }

                if (maxTaxis < TaxisIsTopStartValue)
                {
                    maxTaxis = TaxisIsTopStartValue;
                }
            }
            else
            {
                var max = _repository.Max(Attr.Taxis, Q
                    .Where(Attr.ChannelId, channelId)
                    .Where(Attr.Taxis, "<", TaxisIsTopStartValue)
                );

                if (max != null)
                {
                    maxTaxis = max.Value;
                }
            }

            return maxTaxis;
        }

        public bool GetChanelIdAndValue<T>(int contentId, string name, out int channelId, out T value)
        {
            channelId = 0;
            value = default(T);

            var tuple = _repository.Get<(int ChannelId, T Result)?>(Q
                .Select(Attr.ChannelId, name)
                .Where(Attr.Id, contentId)
            );
            if (tuple == null) return false;

            channelId = tuple.Value.ChannelId;
            value = tuple.Value.Result;

            return true;
        }

        public T GetValue<T>(int contentId, string name)
        {
            return _repository.Get<T>(Q
                .Select(name)
                .Where(Attr.Id, contentId)
            );
        }

        public Tuple<int, T> GetValueWithChannelId<T>(int contentId, string name)
        {
            var channelId = 0;
            var value = default(T);

            var tuple = _repository.Get<(int ChannelId, T Result)?>(Q
                .Select(Attr.ChannelId, name)
                .Where(Attr.Id, contentId)
            );
            if (tuple == null) return null;

            channelId = tuple.Value.ChannelId;
            value = tuple.Value.Result;

            return new Tuple<int, T>(channelId, value);
        }

        public int GetTotalHits(int siteId)
        {
            return _repository.Sum(Attr.Hits, Q
                .Where(Attr.SiteId, siteId)
                .Where(Attr.IsChecked, true.ToString())
                .Where(Attr.Hits, ">", 0)
            );
        }

        public int GetFirstContentId(int siteId, int channelId)
        {
            return _repository.Get<int>(Q
                .Select(Attr.Id)
                .Where(Attr.SiteId, siteId)
                .Where(Attr.ChannelId, channelId)
                .OrderByDesc(Attr.Taxis, Attr.Id)
            );
        }

        public int GetContentId(int channelId, int taxis, bool isNextContent)
        {
            if (isNextContent)
            {
                return _repository.Get<int>(Q
                    .Select(Attr.Id)
                    .Where(Attr.ChannelId, channelId)
                    .Where(Attr.Taxis, "<", taxis)
                    .Where(Attr.IsChecked, true.ToString())
                    .OrderByDesc(Attr.Taxis));
            }

            return _repository.Get<int>(Q
                .Select(Attr.Id)
                .Where(Attr.ChannelId, channelId)
                .Where(Attr.Taxis, ">", taxis)
                .Where(Attr.IsChecked, true.ToString())
                .OrderBy(Attr.Taxis));
        }

        public int GetChannelId(int contentId)
        {
            return _repository.Get<int>(Q.Select(Attr.ChannelId).Where(Attr.Id, contentId));
        }

        public int GetContentId(int channelId, TaxisType taxisType)
        {
            var query = Q
                .Select(Attr.Id)
                .Where(Attr.ChannelId, channelId);

            QueryOrder(query, taxisType);

            return _repository.Get<int>(query);
        }

        public int GetTaxisToInsert(int channelId, bool isTop)
        {
            int taxis;

            if (isTop)
            {
                taxis = GetMaxTaxis(channelId, true) + 1;
            }
            else
            {
                taxis = GetMaxTaxis(channelId, false) + 1;
            }

            return taxis;
        }

        private int GetTaxis(int contentId)
        {
            return _repository.Get<int>(Q
                .Select(Attr.Taxis)
                .Where(Attr.Id, contentId)
            );
        }

        public int GetSequence(int channelId, int contentId)
        {
            var taxis = GetTaxis(contentId);

            return _repository.Count(Q
                .Where(Attr.ChannelId, channelId)
                .Where(Attr.IsChecked, true.ToString())
                .Where(Attr.Taxis, "<", taxis)
                .WhereNot(Attr.SourceId, SourceManager.Preview)
            ) + 1;
        }

        public int GetCountCheckedImage(int siteId, int channelId)
        {
            return _repository.Count(Q
                .Where(Attr.ChannelId, channelId)
                .WhereNotNull(Attr.ImageUrl)
                .WhereNot(Attr.ImageUrl, string.Empty)
                .Where(Attr.IsChecked, true.ToString())
                .WhereNot(Attr.SourceId, SourceManager.Preview)
            );
        }

        public int GetCountOfContentAdd(int siteId, int channelId, ScopeType scope, DateTime begin, DateTime end, string userName, bool? checkedState)
        {
            var channelInfo = ChannelManager.GetChannelInfo(siteId, channelId);
            var channelIdList = ChannelManager.GetChannelIdList(channelInfo, scope, string.Empty, string.Empty, string.Empty);
            return GetCountOfContentAdd(siteId, channelIdList, begin, end, userName, checkedState);
        }

        private int GetCountOfContentAdd(int siteId, List<int> channelIdList, DateTime begin, DateTime end, string userName, bool? checkedState)
        {
            var query = Q.Where(Attr.SiteId, siteId);
            query.WhereIn(Attr.ChannelId, channelIdList);
            query.WhereBetween(Attr.AddDate, begin, end.AddDays(1));
            if (!string.IsNullOrEmpty(userName))
            {
                query.Where(Attr.AddUserName, userName);
            }

            if (checkedState.HasValue)
            {
                query.Where(Attr.IsChecked, checkedState.ToString());
            }

            return _repository.Count(query);
        }

        public int GetCountOfContentUpdate(int siteId, int channelId, ScopeType scope, DateTime begin, DateTime end, string userName)
        {
            var channelInfo = ChannelManager.GetChannelInfo(siteId, channelId);
            var channelIdList = ChannelManager.GetChannelIdList(channelInfo, scope, string.Empty, string.Empty, string.Empty);
            return GetCountOfContentUpdate(siteId, channelIdList, begin, end, userName);
        }

        private int GetCountOfContentUpdate(int siteId, List<int> channelIdList, DateTime begin, DateTime end, string userName)
        {
            var query = Q.Where(Attr.SiteId, siteId);
            query.WhereIn(Attr.ChannelId, channelIdList);
            query.WhereBetween(Attr.LastEditDate, begin, end.AddDays(1));
            query.WhereRaw($"{Attr.LastEditDate} != {Attr.AddDate}");
            if (!string.IsNullOrEmpty(userName))
            {
                query.Where(Attr.AddUserName, userName);
            }

            return _repository.Count(query);
        }

        public ContentInfo GetCacheContentInfo(int contentId)
        {
            if (contentId <= 0) return null;
            return _repository.Get(contentId);
        }
    }
}
