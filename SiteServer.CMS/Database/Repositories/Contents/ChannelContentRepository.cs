using System;
using System.Collections.Generic;
using SiteServer.CMS.Core;
using SiteServer.CMS.Database.Caches;
using SiteServer.CMS.Database.Core;
using SiteServer.CMS.Database.Models;
using SiteServer.CMS.Plugin.Impl;
using SiteServer.Plugin;
using SiteServer.Utils;
using Attr = SiteServer.CMS.Database.Attributes.ContentAttribute;

namespace SiteServer.CMS.Database.Repositories.Contents
{
    public class ChannelContentRepository : GenericRepositoryAbstract
    {
        public override string TableName { get; }
        public override List<TableColumn> TableColumns { get; }

        private SiteInfo SiteInfo { get; }
        private ChannelInfo ChannelInfo { get; }

        public ChannelContentRepository(SiteInfo siteInfo, ChannelInfo channelInfo)
        {
            SiteInfo = siteInfo;
            ChannelInfo = channelInfo;

            TableName = ChannelManager.GetTableName(siteInfo, channelInfo);
            TableColumns = new List<TableColumn>
            {
                new TableColumn
                {
                    AttributeName = nameof(ContentInfo.Id),
                    DataType = DataType.Integer,
                    IsIdentity = true,
                    IsPrimaryKey = true
                },
                new TableColumn
                {
                    AttributeName = nameof(ContentInfo.ChannelId),
                    DataType = DataType.Integer
                },
                new TableColumn
                {
                    AttributeName = nameof(ContentInfo.SiteId),
                    DataType = DataType.Integer
                },
                new TableColumn
                {
                    AttributeName = nameof(ContentInfo.AddUserName),
                    DataType = DataType.VarChar
                },
                new TableColumn
                {
                    AttributeName = nameof(ContentInfo.LastEditUserName),
                    DataType = DataType.VarChar
                },
                new TableColumn
                {
                    AttributeName = nameof(ContentInfo.LastEditDate),
                    DataType = DataType.DateTime
                },
                new TableColumn
                {
                    AttributeName = nameof(ContentInfo.AdminId),
                    DataType = DataType.Integer
                },
                new TableColumn
                {
                    AttributeName = nameof(ContentInfo.UserId),
                    DataType = DataType.Integer
                },
                new TableColumn
                {
                    AttributeName = nameof(ContentInfo.Taxis),
                    DataType = DataType.Integer
                },
                new TableColumn
                {
                    AttributeName = nameof(ContentInfo.GroupNameCollection),
                    DataType = DataType.VarChar
                },
                new TableColumn
                {
                    AttributeName = nameof(ContentInfo.Tags),
                    DataType = DataType.VarChar
                },
                new TableColumn
                {
                    AttributeName = nameof(ContentInfo.SourceId),
                    DataType = DataType.Integer
                },
                new TableColumn
                {
                    AttributeName = nameof(ContentInfo.ReferenceId),
                    DataType = DataType.Integer
                },
                new TableColumn
                {
                    AttributeName = nameof(ContentInfo.IsChecked),
                    DataType = DataType.VarChar
                },
                new TableColumn
                {
                    AttributeName = nameof(ContentInfo.CheckedLevel),
                    DataType = DataType.Integer
                },
                new TableColumn
                {
                    AttributeName = nameof(ContentInfo.Hits),
                    DataType = DataType.Integer
                },
                new TableColumn
                {
                    AttributeName = nameof(ContentInfo.HitsByDay),
                    DataType = DataType.Integer
                },
                new TableColumn
                {
                    AttributeName = nameof(ContentInfo.HitsByWeek),
                    DataType = DataType.Integer
                },
                new TableColumn
                {
                    AttributeName = nameof(ContentInfo.HitsByMonth),
                    DataType = DataType.Integer
                },
                new TableColumn
                {
                    AttributeName = nameof(ContentInfo.LastHitsDate),
                    DataType = DataType.DateTime
                },
                new TableColumn
                {
                    AttributeName = nameof(ContentInfo.SettingsXml),
                    DataType = DataType.Text
                },
                new TableColumn
                {
                    AttributeName = nameof(ContentInfo.Title),
                    DataType = DataType.VarChar
                },
                new TableColumn
                {
                    AttributeName = nameof(ContentInfo.IsTop),
                    DataType = DataType.VarChar
                },
                new TableColumn
                {
                    AttributeName = nameof(ContentInfo.IsRecommend),
                    DataType = DataType.VarChar
                },
                new TableColumn
                {
                    AttributeName = nameof(ContentInfo.IsHot),
                    DataType = DataType.VarChar
                },
                new TableColumn
                {
                    AttributeName = nameof(ContentInfo.IsColor),
                    DataType = DataType.VarChar
                },
                new TableColumn
                {
                    AttributeName = nameof(ContentInfo.LinkUrl),
                    DataType = DataType.VarChar
                },
                new TableColumn
                {
                    AttributeName = nameof(ContentInfo.AddDate),
                    DataType = DataType.DateTime
                }
            };
        }

        public void UpdateIsChecked(List<int> contentIdList, int translateChannelId, string userName, bool isChecked, int checkedLevel, string reasons)
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
            //        $"UPDATE {tableName} SET {nameof(ContentInfo.IsChecked)} = '{isChecked}', {nameof(ContentInfo.CheckedLevel)} = {checkedLevel}, {nameof(ContentInfo.SettingsXml)} = '{attributes}' WHERE {nameof(ContentInfo.Id)} = {contentId}";
            //    if (translateChannelId > 0)
            //    {
            //        sqlString =
            //            $"UPDATE {tableName} SET {nameof(ContentInfo.IsChecked)} = '{isChecked}', {nameof(ContentInfo.CheckedLevel)} = {checkedLevel}, {nameof(ContentInfo.SettingsXml)} = '{attributes}', {nameof(ContentInfo.ChannelId)} = {translateChannelId} WHERE {nameof(ContentInfo.Id)} = {contentId}";
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

                var attributes = new AttributesImpl(settingsXml);
                attributes.Set(Attr.CheckUserName, userName);
                attributes.Set(Attr.CheckDate, DateUtils.GetDateAndTimeString(checkDate));
                attributes.Set(Attr.CheckReasons, reasons);

                settingsXml = attributes.ToString(Attr.ExcludedAttributes.Value);

                if (translateChannelId > 0)
                {
                    UpdateValue(new Dictionary<string, object>
                    {
                        {Attr.IsChecked, isChecked.ToString()},
                        {Attr.CheckedLevel, checkedLevel},
                        {Attr.SettingsXml, settingsXml},
                        {Attr.ChannelId, translateChannelId }
                    }, Q.Where(Attr.Id, contentId));
                }
                else
                {
                    UpdateValue(new Dictionary<string, object>
                    {
                        {Attr.IsChecked, isChecked.ToString()},
                        {Attr.CheckedLevel, checkedLevel},
                        {Attr.SettingsXml, settingsXml}
                    }, Q.Where(Attr.Id, contentId));
                }

                DataProvider.ContentCheck.Insert(new ContentCheckInfo
                {
                    TableName = TableName,
                    SiteId = SiteInfo.Id,
                    ChannelId = ChannelInfo.Id,
                    ContentId = contentId,
                    UserName = userName,
                    Checked = isChecked,
                    CheckedLevel = checkedLevel,
                    CheckDate = checkDate,
                    Reasons = reasons
                });
            }

            ContentManager.RemoveCache(TableName, ChannelInfo.Id);
        }

        public void UpdateArrangeTaxis(string attributeName, bool isDesc)
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
                .Where(Attr.ChannelId, ChannelInfo.Id)
                .OrWhere(Attr.ChannelId, -ChannelInfo.Id);
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
                UpdateValue(new Dictionary<string, object>
                {
                    {Attr.Taxis, taxis++}
                }, Q.Where(Attr.Id, contentId));
            }
        }

        private IList<int> GetContentIdListByTrash()
        {
            //var sqlString =
            //    $"SELECT Id FROM {tableName} WHERE SiteId = {siteId} AND ChannelId < 0";
            //return DatabaseApi.GetIntList(sqlString);

            return base.GetValueList<int>(Q
                .Select(Attr.Id)
                .Where(Attr.SiteId, SiteInfo.Id)
                .Where(Attr.ChannelId, "<", 0)
            );
        }

        public void DeleteContentsByTrash()
        {
            var contentIdList = GetContentIdListByTrash();
            TagUtils.RemoveTags(SiteInfo.Id, contentIdList);

            var sqlString =
                $"DELETE FROM {tableName} WHERE SiteId = {siteId} AND ChannelId < 0";
            DatabaseApi.ExecuteNonQuery(ConnectionString, sqlString);

            ContentManager.RemoveCache(tableName, channelId);
        }
    }
}
