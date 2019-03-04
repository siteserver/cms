using System.Collections.Generic;
using SiteServer.CMS.Caches;
using SiteServer.CMS.Caches.Content;
using SiteServer.CMS.Core;
using Attr = SiteServer.CMS.Database.Attributes.ContentAttribute;

namespace SiteServer.CMS.Database.Repositories.Contents
{
    public partial class ContentTableRepository
    {
        public void DeleteContentsByTrash()
        {
            //var sqlString =
            //    $"SELECT Id FROM {tableName} WHERE SiteId = {siteId} AND ChannelId < 0";
            //return DatabaseApi.GetIntList(sqlString);

            var contentIdList = GetValueList<int>(Q
                .Select(Attr.Id)
                .Where(Attr.SiteId, SiteId)
                .Where(Attr.ChannelId, "<", 0)
            );
            TagUtils.RemoveTags(SiteId, contentIdList);

            //var sqlString =
            //    $"DELETE FROM {tableName} WHERE SiteId = {siteId} AND ChannelId < 0";
            //DatabaseApi.ExecuteNonQuery(ConnectionString, sqlString);

            //ContentManager.RemoveCache(tableName, channelId);

            DeleteAll(Q
                .Where(Attr.SiteId, SiteId)
                .Where(Attr.ChannelId, "<", 0)
            );

            ContentManager.RemoveCacheBySiteId(TableName, SiteId);
        }

        private void DeleteContents(int siteId, int channelId, IList<int> contentIdList)
        {
            var deleteNum = 0;

            if (contentIdList != null && contentIdList.Count > 0)
            {
                TagUtils.RemoveTags(siteId, contentIdList);

                //var sqlString =
                //    $"DELETE FROM {tableName} WHERE SiteId = {siteId} AND Id IN ({TranslateUtils.ToSqlInStringWithoutQuote(contentIdList)})";
                //deleteNum = DatabaseApi.ExecuteNonQuery(ConnectionString, sqlString);

                deleteNum = DeleteAll(Q
                    .Where(Attr.SiteId, siteId)
                    .WhereIn(Attr.Id, contentIdList)
                );
            }

            if (deleteNum <= 0) return;

            ContentManager.RemoveCache(TableName, channelId);
        }

        public void DeleteContent(int siteId, int channelId, int contentId)
        {
            if (contentId > 0)
            {
                TagUtils.RemoveTags(siteId, contentId);

                //var sqlString =
                //    $"DELETE FROM {tableName} WHERE SiteId = {siteInfo.Id} AND Id = {contentId}";
                //DatabaseApi.ExecuteNonQuery(ConnectionString, sqlString);

                DeleteAll(Q
                    .Where(Attr.SiteId, siteId)
                    .Where(Attr.Id, contentId)
                );
            }

            if (channelId <= 0) return;

            ContentManager.RemoveCache(TableName, channelId);
        }

        public void DeleteContents(int siteId, IList<int> contentIdList, int channelId)
        {
            var deleteNum = 0;

            if (contentIdList != null && contentIdList.Count > 0)
            {
                TagUtils.RemoveTags(siteId, contentIdList);

                //var sqlString =
                //    $"DELETE FROM {tableName} WHERE SiteId = {siteId} AND Id IN ({TranslateUtils.ToSqlInStringWithoutQuote(contentIdList)})";
                //deleteNum = DatabaseApi.ExecuteNonQuery(ConnectionString, sqlString);

                deleteNum = DeleteAll(Q
                    .Where(Attr.SiteId, siteId)
                    .WhereIn(Attr.Id, contentIdList)
                );
            }

            if (channelId <= 0 || deleteNum <= 0) return;

            ContentManager.RemoveCache(TableName, channelId);
        }

        public void DeleteContentsByChannelId(int siteId, int channelId)
        {
            var contentIdList = GetContentIdListChecked(channelId);

            TagUtils.RemoveTags(siteId, contentIdList);

            //var sqlString =
            //    $"DELETE FROM {tableName} WHERE SiteId = {siteId} AND ChannelId = {channelId}";
            //var deleteNum = DatabaseApi.ExecuteNonQuery(ConnectionString, sqlString);

            var deleteNum = base.DeleteAll(Q
                .Where(Attr.SiteId, siteId)
                .Where(Attr.ChannelId, channelId)
            );

            if (channelId <= 0 || deleteNum <= 0) return;

            ContentManager.RemoveCache(TableName, channelId);
        }
    }
}
