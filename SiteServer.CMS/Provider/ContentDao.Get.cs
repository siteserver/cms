using System;
using System.Collections.Generic;
using Dapper;
using Datory;
using SiteServer.CMS.Core;
using SiteServer.CMS.DataCache;
using SiteServer.CMS.DataCache.Content;
using SiteServer.CMS.Model;
using SiteServer.Utils;
using SiteServer.Utils.Enumerations;
using Attr = SiteServer.CMS.Model.Attributes.ContentAttribute;

namespace SiteServer.CMS.Provider
{
    public partial class ContentDao
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

        public int GetContentId(int channelId, string orderByString)
        {
            var contentId = 0;
            var sqlString = SqlUtils.ToTopSqlString(TableName, "Id", $"WHERE (ChannelId = {channelId})", orderByString, 1);

            using (var connection = new Connection(WebConfigUtils.DatabaseType, WebConfigUtils.ConnectionString))
            {
                using (var rdr = connection.ExecuteReader(sqlString))
                {
                    if (rdr.Read())
                    {
                        contentId = DatabaseUtils.GetInt(rdr, 0);
                    }
                    rdr.Close();
                }
            }
            return contentId;
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

        public int GetCount(string whereString)
        {
            if (!string.IsNullOrEmpty(whereString))
            {
                whereString = whereString.Replace("WHERE ", string.Empty).Replace("where ", string.Empty);
            }
            whereString = string.IsNullOrEmpty(whereString) ? string.Empty : $"WHERE {whereString}";

            var sqlString = $"SELECT COUNT(*) FROM {TableName} {whereString}";

            return DatabaseUtils.GetIntResult(sqlString);
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

        public int GetCountOfContentAdd(int siteId, int channelId, EScopeType scope, DateTime begin, DateTime end, string userName, ETriState checkedState)
        {
            var channelInfo = ChannelManager.GetChannelInfo(siteId, channelId);
            var channelIdList = ChannelManager.GetChannelIdList(channelInfo, scope, string.Empty, string.Empty, string.Empty);
            return GetCountOfContentAdd(siteId, channelIdList, begin, end, userName, checkedState);
        }

        private int GetCountOfContentAdd(int siteId, List<int> channelIdList, DateTime begin, DateTime end, string userName, ETriState checkedState)
        {
            var query = Q.Where(Attr.SiteId, siteId);
            query.WhereIn(Attr.ChannelId, channelIdList);
            query.WhereBetween(Attr.AddDate, begin, end.AddDays(1));
            if (!string.IsNullOrEmpty(userName))
            {
                query.Where(Attr.AddUserName, userName);
            }

            if (checkedState != ETriState.All)
            {
                query.Where(Attr.IsChecked, ETriStateUtils.GetValue(checkedState));
            }

            return _repository.Count(query);
        }

        public int GetCountOfContentUpdate(int siteId, int channelId, EScopeType scope, DateTime begin, DateTime end, string userName)
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
