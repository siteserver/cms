using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Datory;
using SiteServer.CMS.Core;
using SiteServer.CMS.Model;
using SiteServer.CMS.Model.Attributes;

namespace SiteServer.CMS.Provider
{
    public partial class ChannelDao
    {
        public string GetListKey(int siteId)
        {
            return Caching.GetListKey(TableName, siteId);
        }

        public string GetEntityKey(int channelId)
        {
            return Caching.GetEntityKey(TableName, channelId);
        }

        public void CacheAll(int siteId)
        {
            var cacheManager = Caching.CacheManager;

            if (!Caching.CacheManager.Exists(GetListKey(siteId)) || !Caching.CacheManager.Exists(GetEntityKey(siteId)))
            {
                var channels = new List<ChannelInfo>();
                string sqlString =
                    $@"SELECT Id, ChannelName, SiteId, ContentModelPluginId, ContentRelatedPluginIds, ParentId, ParentsPath, ParentsCount, ChildrenCount, IsLastNode, IndexName, GroupNameCollection, Taxis, AddDate, ImageUrl, Content, FilePath, ChannelFilePathRule, ContentFilePathRule, LinkUrl, LinkType, ChannelTemplateId, ContentTemplateId, Keywords, Description, ExtendValues
FROM siteserver_Channel 
WHERE (SiteId = {siteId} AND (Id = {siteId} OR ParentId > 0))
ORDER BY Taxis";

                using (var rdr = ExecuteReader(sqlString))
                {
                    while (rdr.Read())
                    {
                        var channelInfo = GetChannelInfo(rdr);
                        channels.Add(channelInfo);
                    }
                    rdr.Close();
                }

                foreach (var channel in channels)
                {
                    cacheManager.Put(GetEntityKey(channel.Id), channel);
                }

                var summaries = channels
                    .OrderBy(x => x.Taxis)
                    .ThenBy(x => x.Id)
                    .Select(x => new ChannelSummary
                    {
                        Id = x.Id,
                        ChannelName = x.ChannelName,
                        ParentId = x.ParentId,
                        ParentsPath = x.ParentsPath,
                        IndexName = x.IndexName,
                        ContentModelPluginId = x.ContentModelPluginId,
                        Taxis = x.Taxis,
                        AddDate = x.AddDate
                    }).ToList();

                cacheManager.Put(GetListKey(siteId), summaries);
            }
        }

        public List<ChannelSummary> GetAllSummary(int siteId)
        {
            var cacheManager = Caching.CacheManager;

            return cacheManager.GetOrCreate(GetListKey(siteId), () =>
            {
                var summaries = new List<ChannelSummary>();
                string sqlString =
                    $@"SELECT Id, ChannelName, ParentId, ParentsPath, IndexName, ContentModelPluginId, Taxis, AddDate
FROM siteserver_Channel 
WHERE (SiteId = {siteId} AND (Id = {siteId} OR ParentId > 0))
ORDER BY Taxis";

                using (var rdr = ExecuteReader(sqlString))
                {
                    while (rdr.Read())
                    {
                        var channelInfo = GetChannelSummary(rdr);
                        summaries.Add(channelInfo);
                    }
                    rdr.Close();
                }

                return summaries;
            });
        }

        public ChannelInfo Get(int channelId)
        {
            if (channelId == 0) return null;

            channelId = Math.Abs(channelId);
            var cacheManager = Caching.CacheManager;

            return cacheManager.GetOrCreate(GetEntityKey(channelId), () =>
            {
                ChannelInfo channelInfo = null;

                var parms = new IDataParameter[]
                {
                    GetParameter(ParmId, DataType.Integer, channelId)
                };

                using (var rdr = ExecuteReader(
                    "SELECT Id, ChannelName, SiteId, ContentModelPluginId, ContentRelatedPluginIds, ParentId, ParentsPath, ParentsCount, ChildrenCount, IsLastNode, IndexName, GroupNameCollection, Taxis, AddDate, ImageUrl, Content, FilePath, ChannelFilePathRule, ContentFilePathRule, LinkUrl, LinkType, ChannelTemplateId, ContentTemplateId, Keywords, Description, ExtendValues FROM siteserver_Channel WHERE Id = @Id",
                    parms))
                {
                    if (rdr.Read())
                    {
                        channelInfo = GetChannelInfo(rdr);
                    }

                    rdr.Close();
                }

                return channelInfo;
            });
        }
    }
}
