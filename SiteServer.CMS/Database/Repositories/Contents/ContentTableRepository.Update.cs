using System;
using System.Collections.Generic;
using SiteServer.CMS.Caches;
using SiteServer.CMS.Caches.Content;
using SiteServer.CMS.Core;
using SiteServer.CMS.Database.Core;
using SiteServer.CMS.Database.Models;
using SiteServer.CMS.Database.Wrapper;
using SiteServer.CMS.Plugin.Impl;
using SiteServer.Plugin;
using SiteServer.Utils;
using Attr = SiteServer.CMS.Database.Attributes.ContentAttribute;

namespace SiteServer.CMS.Database.Repositories.Contents
{
    public partial class ContentTableRepository
    {
        public void UpdateIsChecked(int channelId, List<int> contentIdList, int translateChannelId, string userName, bool isChecked, int checkedLevel, string reasons)
        {
            //if (isChecked)
            //{
            //    checkedLevel = 0;
            //}

            //var checkDate = DateTime.Now;

            //foreach (var contentId in contentIdList)
            //{
            //    var tuple = GetValue(tableName, contentId, Attr.SettingsXml);
            //    if (tuple == null) continue;

            //    var attributes = new AttributesImpl(tuple.Item2);
            //    attributes.Set(Attr.CheckUserName, userName);
            //    attributes.Set(Attr.CheckDate, DateUtils.GetDateAndTimeString(checkDate));
            //    attributes.Set(Attr.CheckReasons, reasons);

            //    var sqlString =
            //        $"UPDATE {tableName} SET {ContentAttribute.IsChecked)} = '{isChecked}', {ContentAttribute.CheckedLevel)} = {checkedLevel}, {ContentAttribute.SettingsXml)} = '{attributes}' WHERE {ContentAttribute.Id)} = {contentId}";
            //    if (translateChannelId > 0)
            //    {
            //        sqlString =
            //            $"UPDATE {tableName} SET {ContentAttribute.IsChecked)} = '{isChecked}', {ContentAttribute.CheckedLevel)} = {checkedLevel}, {ContentAttribute.SettingsXml)} = '{attributes}', {ContentAttribute.ChannelId)} = {translateChannelId} WHERE {ContentAttribute.Id)} = {contentId}";
            //    }
            //    DatabaseApi.ExecuteNonQuery(ConnectionString, sqlString);

            //    var checkInfo = new ContentCheckInfo
            //    {
            //        TableName = tableName,
            //        SiteId = siteId,
            //        ChannelId = channelId,
            //        ContentId = contentId,
            //        UserName = userName,
            //        Checked = isChecked,
            //        CheckedLevel = checkedLevel,
            //        CheckDate = checkDate,
            //        Reasons = reasons
            //    };
            //    DataProvider.ContentCheck.Insert(checkInfo);
            //}

            //ContentManager.RemoveCache(tableName, channelId);

            if (isChecked)
            {
                checkedLevel = 0;
            }

            var checkDate = DateTime.Now;

            foreach (var contentId in contentIdList)
            {
                var settingsXml = GetValue<string>(Q
                    .Select(Attr.SettingsXml)
                    .Where(Attr.Id, contentId));

                var attributes = TranslateUtils.JsonDeserialize(settingsXml, new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase));

                attributes[Attr.CheckUserName] = userName;
                attributes[Attr.CheckDate] = DateUtils.GetDateAndTimeString(checkDate);
                attributes[Attr.CheckReasons] = reasons;

                settingsXml = TranslateUtils.JsonSerialize(attributes);

                if (translateChannelId > 0)
                {
                    UpdateAll(Q
                        .Set(Attr.IsChecked, isChecked.ToString())
                        .Set(Attr.CheckedLevel, checkedLevel)
                        .Set(Attr.SettingsXml, settingsXml)
                        .Set(Attr.ChannelId, translateChannelId)
                        .Where(Attr.Id, contentId)
                    );
                }
                else
                {
                    UpdateAll(Q
                        .Set(Attr.IsChecked, isChecked.ToString())
                        .Set(Attr.CheckedLevel, checkedLevel)
                        .Set(Attr.SettingsXml, settingsXml)
                        .Where(Attr.Id, contentId)
                    );
                }

                DataProvider.ContentCheck.Insert(new ContentCheckInfo
                {
                    TableName = TableName,
                    SiteId = SiteId,
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

        public void UpdateArrangeTaxis(int channelId, string attributeName, bool isDesc)
        {
            //var taxisDirection = isDesc ? "ASC" : "DESC";//升序,但由于页面排序是按Taxis的Desc排序的，所以这里sql里面的ASC/DESC取反

            //var sqlString =
            //    $"SELECT Id, IsTop FROM {tableName} WHERE ChannelId = {channelId} OR ChannelId = -{channelId} ORDER BY {attributeName} {taxisDirection}";
            //var sqlList = new List<string>();

            //using (var rdr = DatabaseApi.ExecuteReader(ConnectionString, sqlString))
            //{
            //    var taxis = 1;
            //    while (rdr.Read())
            //    {
            //        var id = DatabaseApi.GetInt(rdr, 0);
            //        var isTop = TranslateUtils.ToBool(DatabaseApi.GetString(rdr, 1));

            //        sqlList.Add(
            //            $"UPDATE {tableName} SET Taxis = {taxis++}, IsTop = '{isTop}' WHERE Id = {id}");
            //    }
            //    rdr.Close();
            //}

            //DatabaseApi.ExecuteSql(sqlList);

            //ContentManager.RemoveCache(tableName, channelId);

            var query = Q
                .Select(Attr.Id)
                .Where(Attr.ChannelId, channelId)
                .OrWhere(Attr.ChannelId, -channelId);
            if (isDesc)
            {
                query.OrderBy(attributeName);
            }
            else
            {
                query.OrderByDesc(attributeName);
            }

            var idList = GetValueList<int>(query);

            var taxis = 1;
            foreach (var contentId in idList)
            {
                UpdateAll(Q
                    .Set(Attr.Taxis, taxis++)
                    .Where(Attr.Id, contentId)
                );
            }
        }

        public void SetAutoPageContentToSite()
        {
            var siteInfo = SiteManager.GetSiteInfo(SiteId);
            if (!siteInfo.IsAutoPageInTextEditor) return;

            //var sqlString =
            //    $"SELECT Id, {ContentAttribute.ChannelId)}, {ContentAttribute.Content)} FROM {siteInfo.TableName} WHERE SiteId = {siteInfo.Id}";

            //using (var rdr = DatabaseApi.ExecuteReader(ConnectionString, sqlString))
            //{
            //    while (rdr.Read())
            //    {
            //        var contentId = DatabaseApi.GetInt(rdr, 0);
            //        var channelId = DatabaseApi.GetInt(rdr, 1);
            //        var content = DatabaseApi.GetString(rdr, 2);

            //        if (!string.IsNullOrEmpty(content))
            //        {
            //            content = ContentUtility.GetAutoPageContent(content, siteInfo.AutoPageWordNum);

            //            Update(siteInfo.TableName, channelId, contentId, ContentAttribute.Content), content);
            //        }
            //    }

            //    rdr.Close();
            //}

            var resultList = GetValueList<(int Id, int ChannelId, string Content)>(Q.Where(Attr.SiteId, SiteId));

            foreach (var result in resultList)
            {

                if (!string.IsNullOrEmpty(result.Content))
                {
                    var content = ContentUtility.GetAutoPageContent(result.Content, siteInfo.AutoPageWordNum);

                    Update(result.ChannelId, result.Id, Attr.Content, content);
                }
            }
        }

        private void Update(int channelId, int contentId, string name, string value)
        {
            //var sqlString = $"UPDATE {tableName} SET {name} = @{name} WHERE Id = @Id";

            //var parameters = new DynamicParameters();
            //parameters.Add($"@{name}", value);
            //parameters.Add("@Id", contentId);

            //using (var connection = GetConnection())
            //{
            //    connection.Execute(sqlString, parameters);
            //}

            UpdateAll(Q
                .Set(name, value)
                .Where(Attr.Id, contentId)
            );

            ContentManager.RemoveCache(TableName, channelId);
        }

        public void UpdateTrashContents(int siteId, int channelId, IList<int> contentIdList)
        {
            var referenceIdList = GetReferenceIdList(contentIdList);
            if (referenceIdList.Count > 0)
            {
                DeleteContents(siteId, channelId, referenceIdList);
            }

            var updateNum = 0;

            if (contentIdList != null && contentIdList.Count > 0)
            {
                //var sqlString =
                //    $"UPDATE {tableName} SET ChannelId = -ChannelId, LastEditDate = {SqlUtils.GetComparableNow()} WHERE SiteId = {siteId} AND Id IN ({TranslateUtils.ToSqlInStringWithoutQuote(contentIdList)})";
                //updateNum = DatabaseApi.ExecuteNonQuery(ConnectionString, sqlString);

                updateNum = UpdateAll(Q
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
                DeleteContents(siteId, channelId, referenceIdList);
            }

            //var sqlString =
            //    $"UPDATE {tableName} SET ChannelId = -ChannelId, LastEditDate = {SqlUtils.GetComparableNow()} WHERE SiteId = {siteId} AND ChannelId = {siteId}";
            //updateNum = DatabaseApi.ExecuteNonQuery(ConnectionString, sqlString);

            var updateNum = UpdateAll(Q
                .SetRaw($"{Attr.ChannelId} = -{Attr.ChannelId}")
                .Set(Attr.LastEditDate, DateTime.Now)
                .Where(Attr.SiteId, siteId)
                .Where(Attr.ChannelId, channelId)
            );

            if (updateNum <= 0) return;

            ContentManager.RemoveCache(TableName, channelId);
        }

        public void UpdateRestoreContentsByTrash(int siteId, int channelId)
        {
            //var sqlString =
            //    $"UPDATE {tableName} SET ChannelId = -ChannelId, LastEditDate = {SqlUtils.GetComparableNow()} WHERE SiteId = {siteId} AND ChannelId < 0";
            //var updateNum = DatabaseApi.ExecuteNonQuery(ConnectionString, sqlString);

            var updateNum = UpdateAll(Q
                .SetRaw($"{Attr.ChannelId} = -{Attr.ChannelId}")
                .Set(Attr.LastEditDate, DateTime.Now)
                .Where(Attr.SiteId, siteId)
                .Where(Attr.ChannelId, "<", 0)
            );

            if (updateNum <= 0) return;

            ContentManager.RemoveCache(TableName, channelId);
        }

        public void DeletePreviewContents(int siteId, ChannelInfo channelInfo)
        {
            channelInfo.IsPreviewContentsExists = false;
            DataProvider.Channel.UpdateExtend(channelInfo);

            //var sqlString =
            //    $"DELETE FROM {tableName} WHERE {ContentAttribute.SiteId)} = @{ContentAttribute.SiteId)} AND {ContentAttribute.ChannelId)} = @{ContentAttribute.ChannelId)} AND {ContentAttribute.SourceId)} = @{ContentAttribute.SourceId)}";

            //using (var connection = GetConnection())
            //{
            //    connection.Execute(sqlString, new
            //        {
            //            SiteId = siteId,
            //            ChannelId = channelInfo.Id,
            //            SourceId = SourceManager.Preview
            //        }
            //    );
            //}

            base.DeleteAll(Q
                .Where(Attr.SiteId, siteId)
                .Where(Attr.ChannelId, channelInfo.Id)
                .Where(Attr.SourceId, SourceManager.Preview)
            );
        }
    }
}
