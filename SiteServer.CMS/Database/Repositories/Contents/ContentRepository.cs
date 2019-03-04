using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Linq;
using System.Text;
using Dapper;
using SiteServer.CMS.Caches;
using SiteServer.CMS.Caches.Content;
using SiteServer.CMS.Core;
using SiteServer.CMS.Core.Enumerations;
using SiteServer.CMS.Core.RestRoutes.V1;
using SiteServer.CMS.Database.Attributes;
using SiteServer.CMS.Database.Core;
using SiteServer.CMS.Database.Models;
using SiteServer.CMS.Plugin.Impl;
using SiteServer.Plugin;
using SiteServer.Utils;
using SiteServer.Utils.Enumerations;
using Attr = SiteServer.CMS.Database.Attributes.ContentAttribute;

namespace SiteServer.CMS.Database.Repositories.Contents
{
    public class ContentRepository : DataProviderBase
    {

        private const int TaxisIsTopStartValue = 2000000000;

        private static readonly List<string> MinListColumns = new List<string>
        {
            Attr.Id,
            Attr.ChannelId,
            Attr.IsTop,
            Attr.AddDate,
            Attr.LastEditDate,
            Attr.Taxis,
            Attr.Hits,
            Attr.HitsByDay,
            Attr.HitsByWeek,
            Attr.HitsByMonth
        };

        public static string GetContentTableName(int siteId)
        {
            return $"siteserver_Content_{siteId}";
        }

        public override List<TableColumn> TableColumns => ReflectionUtils.GetTableColumns(typeof(ContentInfo));

        //public override List<TableColumn> TableColumns => new List<TableColumn>
        //{
        //    new TableColumn
        //    {
        //        AttributeName = Attr.Id),
        //        DataType = DataType.Integer,
        //        IsIdentity = true,
        //        IsPrimaryKey = true
        //    },
        //    new TableColumn
        //    {
        //        AttributeName = Attr.ChannelId),
        //        DataType = DataType.Integer
        //    },
        //    new TableColumn
        //    {
        //        AttributeName = Attr.SiteId),
        //        DataType = DataType.Integer
        //    },
        //    new TableColumn
        //    {
        //        AttributeName = Attr.AddUserName),
        //        DataType = DataType.VarChar
        //    },
        //    new TableColumn
        //    {
        //        AttributeName = Attr.LastEditUserName),
        //        DataType = DataType.VarChar
        //    },
        //    new TableColumn
        //    {
        //        AttributeName = Attr.LastEditDate),
        //        DataType = DataType.DateTime
        //    },
        //    new TableColumn
        //    {
        //        AttributeName = Attr.AdminId),
        //        DataType = DataType.Integer
        //    },
        //    new TableColumn
        //    {
        //        AttributeName = Attr.UserId),
        //        DataType = DataType.Integer
        //    },
        //    new TableColumn
        //    {
        //        AttributeName = Attr.Taxis),
        //        DataType = DataType.Integer
        //    },
        //    new TableColumn
        //    {
        //        AttributeName = Attr.GroupNameCollection),
        //        DataType = DataType.VarChar
        //    },
        //    new TableColumn
        //    {
        //        AttributeName = Attr.Tags),
        //        DataType = DataType.VarChar
        //    },
        //    new TableColumn
        //    {
        //        AttributeName = Attr.SourceId),
        //        DataType = DataType.Integer
        //    },
        //    new TableColumn
        //    {
        //        AttributeName = Attr.ReferenceId),
        //        DataType = DataType.Integer
        //    },
        //    new TableColumn
        //    {
        //        AttributeName = Attr.IsChecked),
        //        DataType = DataType.VarChar
        //    },
        //    new TableColumn
        //    {
        //        AttributeName = Attr.CheckedLevel),
        //        DataType = DataType.Integer
        //    },
        //    new TableColumn
        //    {
        //        AttributeName = Attr.Hits),
        //        DataType = DataType.Integer
        //    },
        //    new TableColumn
        //    {
        //        AttributeName = Attr.HitsByDay),
        //        DataType = DataType.Integer
        //    },
        //    new TableColumn
        //    {
        //        AttributeName = Attr.HitsByWeek),
        //        DataType = DataType.Integer
        //    },
        //    new TableColumn
        //    {
        //        AttributeName = Attr.HitsByMonth),
        //        DataType = DataType.Integer
        //    },
        //    new TableColumn
        //    {
        //        AttributeName = Attr.LastHitsDate),
        //        DataType = DataType.DateTime
        //    },
        //    new TableColumn
        //    {
        //        AttributeName = Attr.SettingsXml),
        //        DataType = DataType.Text
        //    },
        //    new TableColumn
        //    {
        //        AttributeName = Attr.Title),
        //        DataType = DataType.VarChar
        //    },
        //    new TableColumn
        //    {
        //        AttributeName = Attr.IsTop),
        //        DataType = DataType.VarChar
        //    },
        //    new TableColumn
        //    {
        //        AttributeName = Attr.IsRecommend),
        //        DataType = DataType.VarChar
        //    },
        //    new TableColumn
        //    {
        //        AttributeName = Attr.IsHot),
        //        DataType = DataType.VarChar
        //    },
        //    new TableColumn
        //    {
        //        AttributeName = Attr.IsColor),
        //        DataType = DataType.VarChar
        //    },
        //    new TableColumn
        //    {
        //        AttributeName = Attr.LinkUrl),
        //        DataType = DataType.VarChar
        //    },
        //    new TableColumn
        //    {
        //        AttributeName = Attr.AddDate),
        //        DataType = DataType.DateTime
        //    }
        //};

        public List<TableColumn> TableColumnsDefault
        {
            get
            {
                var tableColumns = new List<TableColumn>();
                tableColumns.AddRange(TableColumns);
                tableColumns.Add(new TableColumn
                {
                    AttributeName = ContentAttribute.SubTitle,
                    DataType = DataType.VarChar
                });
                tableColumns.Add(new TableColumn
                {
                    AttributeName = ContentAttribute.ImageUrl,
                    DataType = DataType.VarChar
                });
                tableColumns.Add(new TableColumn
                {
                    AttributeName = ContentAttribute.VideoUrl,
                    DataType = DataType.VarChar
                });
                tableColumns.Add(new TableColumn
                {
                    AttributeName = ContentAttribute.FileUrl,
                    DataType = DataType.VarChar
                });
                tableColumns.Add(new TableColumn
                {
                    AttributeName = Attr.Content,
                    DataType = DataType.Text
                });
                tableColumns.Add(new TableColumn
                {
                    AttributeName = ContentAttribute.Summary,
                    DataType = DataType.Text
                });
                tableColumns.Add(new TableColumn
                {
                    AttributeName = ContentAttribute.Author,
                    DataType = DataType.VarChar
                });
                tableColumns.Add(new TableColumn
                {
                    AttributeName = ContentAttribute.Source,
                    DataType = DataType.VarChar
                });

                return tableColumns;
            }
        }

        private void UpdateTaxis(int channelId, int id, int taxis, string tableName)
        {
            var sqlString = $"UPDATE {tableName} SET Taxis = {taxis} WHERE Id = {id}";
            DatabaseApi.ExecuteNonQuery(ConnectionString, sqlString);

            ContentManager.RemoveCache(tableName, channelId);
        }

        //public void DeleteContentsByTrash(int siteId, int channelId, string tableName)
        //{
        //    if (string.IsNullOrEmpty(tableName)) return;

        //    var contentIdList = GetContentIdListByTrash(siteId, tableName);
        //    TagUtils.RemoveTags(siteId, contentIdList);

        //    var sqlString =
        //        $"DELETE FROM {tableName} WHERE SiteId = {siteId} AND ChannelId < 0";
        //    DatabaseApi.ExecuteNonQuery(ConnectionString, sqlString);

        //    ContentManager.RemoveCache(tableName, channelId);
        //}

        //public void SetAutoPageContentToSite(SiteInfo siteInfo)
        //{
        //    if (!siteInfo.IsAutoPageInTextEditor) return;

        //    var sqlString =
        //        $"SELECT Id, {Attr.ChannelId)}, {Attr.Content)} FROM {siteInfo.TableName} WHERE SiteId = {siteInfo.Id}";

        //    using (var rdr = DatabaseApi.ExecuteReader(ConnectionString, sqlString))
        //    {
        //        while (rdr.Read())
        //        {
        //            var contentId = DatabaseApi.GetInt(rdr, 0);
        //            var channelId = DatabaseApi.GetInt(rdr, 1);
        //            var content = DatabaseApi.GetString(rdr, 2);

        //            if (!string.IsNullOrEmpty(content))
        //            {
        //                content = ContentUtility.GetAutoPageContent(content, siteInfo.AutoPageWordNum);

        //                Update(siteInfo.TableName, channelId, contentId, Attr.Content), content);
        //            }
        //        }

        //        rdr.Close();
        //    }
        //}

        //public void UpdateTrashContents(int siteId, int channelId, string tableName, List<int> contentIdList)
        //{
        //    if (string.IsNullOrEmpty(tableName)) return;

        //    var referenceIdList = GetReferenceIdList(tableName, contentIdList);
        //    if (referenceIdList.Count > 0)
        //    {
        //        DeleteContents(siteId, channelId, tableName, referenceIdList);
        //    }

        //    var updateNum = 0;

        //    if (!string.IsNullOrEmpty(tableName) && contentIdList != null && contentIdList.Count > 0)
        //    {
        //        var sqlString =
        //            $"UPDATE {tableName} SET ChannelId = -ChannelId, LastEditDate = {SqlUtils.GetComparableNow()} WHERE SiteId = {siteId} AND Id IN ({TranslateUtils.ToSqlInStringWithoutQuote(contentIdList)})";
        //        updateNum = DatabaseApi.ExecuteNonQuery(ConnectionString, sqlString);
        //    }

        //    if (updateNum <= 0) return;

        //    ContentManager.RemoveCache(tableName, channelId);
        //}

        //public void UpdateTrashContentsByChannelId(int siteId, int channelId, string tableName)
        //{
        //    if (string.IsNullOrEmpty(tableName)) return;

        //    var contentIdList = GetContentIdList(tableName, channelId);
        //    var referenceIdList = GetReferenceIdList(tableName, contentIdList);
        //    if (referenceIdList.Count > 0)
        //    {
        //        DeleteContents(siteId, channelId, tableName, referenceIdList);
        //    }
        //    var updateNum = 0;

        //    if (!string.IsNullOrEmpty(tableName))
        //    {
        //        var sqlString =
        //            $"UPDATE {tableName} SET ChannelId = -ChannelId, LastEditDate = {SqlUtils.GetComparableNow()} WHERE SiteId = {siteId} AND ChannelId = {siteId}";
        //        updateNum = DatabaseApi.ExecuteNonQuery(ConnectionString, sqlString);
        //    }

        //    if (updateNum <= 0) return;

        //    ContentManager.RemoveCache(tableName, channelId);
        //}

        //public void DeleteContent(string tableName, SiteInfo siteInfo, int channelId, int contentId)
        //{
        //    if (string.IsNullOrEmpty(tableName)) return;

        //    if (!string.IsNullOrEmpty(tableName) && contentId > 0)
        //    {
        //        TagUtils.RemoveTags(siteInfo.Id, contentId);

        //        var sqlString =
        //            $"DELETE FROM {tableName} WHERE SiteId = {siteInfo.Id} AND Id = {contentId}";
        //        DatabaseApi.ExecuteNonQuery(ConnectionString, sqlString);
        //    }

        //    if (channelId <= 0) return;

        //    ContentManager.RemoveCache(tableName, channelId);
        //}

        //public void DeleteContents(int siteId, string tableName, List<int> contentIdList, int channelId)
        //{
        //    if (string.IsNullOrEmpty(tableName)) return;

        //    var deleteNum = 0;

        //    if (!string.IsNullOrEmpty(tableName) && contentIdList != null && contentIdList.Count > 0)
        //    {
        //        TagUtils.RemoveTags(siteId, contentIdList);

        //        var sqlString =
        //            $"DELETE FROM {tableName} WHERE SiteId = {siteId} AND Id IN ({TranslateUtils.ToSqlInStringWithoutQuote(contentIdList)})";
        //        deleteNum = DatabaseApi.ExecuteNonQuery(ConnectionString, sqlString);
        //    }

        //    if (channelId <= 0 || deleteNum <= 0) return;

        //    ContentManager.RemoveCache(tableName, channelId);
        //}

        //public void DeleteContentsByChannelId(int siteId, string tableName, int channelId)
        //{
        //    if (string.IsNullOrEmpty(tableName)) return;

        //    var contentIdList = GetContentIdListChecked(tableName, channelId, string.Empty);

        //    TagUtils.RemoveTags(siteId, contentIdList);

        //    var sqlString =
        //        $"DELETE FROM {tableName} WHERE SiteId = {siteId} AND ChannelId = {channelId}";
        //    var deleteNum = DatabaseApi.ExecuteNonQuery(ConnectionString, sqlString);

        //    if (channelId <= 0 || deleteNum <= 0) return;

        //    ContentManager.RemoveCache(tableName, channelId);
        //}

        //public void DeleteContentsByDeletedChannelIdList(SiteInfo siteInfo, List<int> channelIdList)
        //{
        //    foreach (var channelId in channelIdList)
        //    {
        //        var tableName = ChannelManager.GetTableName(siteInfo, channelId);
        //        if (string.IsNullOrEmpty(tableName)) continue;

        //        DatabaseApi.ExecuteNonQuery(ConnectionString, $"DELETE FROM {tableName} WHERE SiteId = {siteInfo.Id} AND {Attr.ChannelId)} = {channelId}");

        //        ContentManager.RemoveCache(tableName, channelId);
        //    }
        //}

        //public void UpdateRestoreContentsByTrash(int siteId, int channelId, string tableName)
        //{
        //    var updateNum = 0;

        //    if (!string.IsNullOrEmpty(tableName))
        //    {
        //        var sqlString =
        //            $"UPDATE {tableName} SET ChannelId = -ChannelId, LastEditDate = {SqlUtils.GetComparableNow()} WHERE SiteId = {siteId} AND ChannelId < 0";
        //        updateNum = DatabaseApi.ExecuteNonQuery(ConnectionString, sqlString);
        //    }

        //    if (updateNum <= 0) return;

        //    ContentManager.RemoveCache(tableName, channelId);
        //}

        //private void DeleteContents(int siteId, int channelId, string tableName, List<int> contentIdList)
        //{
        //    if (string.IsNullOrEmpty(tableName)) return;

        //    var deleteNum = 0;

        //    if (!string.IsNullOrEmpty(tableName) && contentIdList != null && contentIdList.Count > 0)
        //    {
        //        TagUtils.RemoveTags(siteId, contentIdList);

        //        var sqlString =
        //            $"DELETE FROM {tableName} WHERE SiteId = {siteId} AND Id IN ({TranslateUtils.ToSqlInStringWithoutQuote(contentIdList)})";
        //        deleteNum = DatabaseApi.ExecuteNonQuery(ConnectionString, sqlString);
        //    }

        //    if (deleteNum <= 0) return;

        //    ContentManager.RemoveCache(tableName, channelId);
        //}

        //public void DeletePreviewContents(int siteId, string tableName, ChannelInfo channelInfo)
        //{
        //    if (string.IsNullOrEmpty(tableName)) return;

        //    channelInfo.IsPreviewContentsExists = false;
        //    DataProvider.Channel.UpdateExtend(channelInfo);

        //    var sqlString =
        //        $"DELETE FROM {tableName} WHERE {Attr.SiteId)} = @{Attr.SiteId)} AND {Attr.ChannelId)} = @{Attr.ChannelId)} AND {Attr.SourceId)} = @{Attr.SourceId)}";

        //    using (var connection = GetConnection())
        //    {
        //        connection.Execute(sqlString, new
        //        {
        //            SiteId = siteId,
        //            ChannelId = channelInfo.Id,
        //            SourceId = SourceManager.Preview
        //        }
        //        );
        //    }
        //}

        //private List<int> GetReferenceIdList(string tableName, List<int> contentIdList)
        //{
        //    var list = new List<int>();
        //    var sqlString =
        //        $"SELECT Id FROM {tableName} WHERE ChannelId > 0 AND ReferenceId IN ({TranslateUtils.ToSqlInStringWithoutQuote(contentIdList)})";

        //    using (var rdr = DatabaseApi.ExecuteReader(ConnectionString, sqlString))
        //    {
        //        while (rdr.Read())
        //        {
        //            list.Add(DatabaseApi.GetInt(rdr, 0));
        //        }
        //        rdr.Close();
        //    }

        //    return list;
        //}

        //public List<int> GetContentIdList(string tableName, int channelId)
        //{
        //    var list = new List<int>();

        //    var sqlString = $"SELECT Id FROM {tableName} WHERE ChannelId = {channelId}";
        //    using (var rdr = DatabaseApi.ExecuteReader(ConnectionString, sqlString))
        //    {
        //        while (rdr.Read())
        //        {
        //            var contentId = DatabaseApi.GetInt(rdr, 0);
        //            list.Add(contentId);
        //        }
        //        rdr.Close();
        //    }
        //    return list;
        //}

        //public int GetTotalHits(string tableName, int siteId)
        //{
        //    return DatabaseApi.GetIntResult($"SELECT SUM(Hits) FROM {tableName} WHERE IsChecked='{true}' AND SiteId = {siteId} AND Hits > 0");
        //}

        //public int GetFirstContentId(string tableName, int channelId)
        //{
        //    var sqlString = $"SELECT Id FROM {tableName} WHERE ChannelId = {channelId} ORDER BY Taxis DESC, Id DESC";
        //    return DatabaseApi.GetIntResult(sqlString);
        //}

        //public List<ContentInfo> GetCacheContentInfoList(SiteInfo siteInfo, ChannelInfo channelInfo, int offset, int limit)
        //{
        //    var tableName = ChannelManager.GetTableName(siteInfo, channelInfo);

        //    return GetContentInfoList(tableName, GetCacheWhereString(siteInfo, channelInfo),
        //        GetOrderString(channelInfo, string.Empty), offset, limit);
        //}

        //public List<int> GetCacheContentIdList(SiteInfo siteInfo, ChannelInfo channelInfo, int offset, int limit)
        //{
        //    var tableName = ChannelManager.GetTableName(siteInfo, channelInfo);

        //    return GetCacheContentIdList(tableName, GetCacheWhereString(siteInfo, channelInfo),
        //        GetOrderString(channelInfo, string.Empty), offset, limit);
        //}

        public bool SetTaxisToUp(string tableName, int channelId, int contentId, bool isTop)
        {
            //Get Higher Taxis and Id
            var sqlString = SqlDifferences.GetSqlString(tableName, new List<string>
                {
                    Attr.Id,
                    Attr.Taxis
                },
                isTop
                    ? $"WHERE (Taxis > (SELECT Taxis FROM {tableName} WHERE Id = {contentId}) AND Taxis >= {TaxisIsTopStartValue} AND ChannelId = {channelId})"
                    : $"WHERE (Taxis > (SELECT Taxis FROM {tableName} WHERE Id = {contentId}) AND Taxis < {TaxisIsTopStartValue} AND ChannelId = {channelId})",
                "ORDER BY Taxis", 1);
            var higherId = 0;
            var higherTaxis = 0;

            using (var rdr = DatabaseApi.ExecuteReader(ConnectionString, sqlString))
            {
                if (rdr.Read())
                {
                    higherId = DatabaseApi.GetInt(rdr, 0);
                    higherTaxis = DatabaseApi.GetInt(rdr, 1);
                }
                rdr.Close();
            }

            if (higherId != 0)
            {
                //Get Taxis Of Selected Id
                var selectedTaxis = GetTaxis(contentId, tableName);

                //Set The Selected Class Taxis To Higher Level
                UpdateTaxis(channelId, contentId, higherTaxis, tableName);
                //Set The Higher Class Taxis To Lower Level
                UpdateTaxis(channelId, higherId, selectedTaxis, tableName);
                return true;
            }
            return false;
        }

        public bool SetTaxisToDown(string tableName, int channelId, int contentId, bool isTop)
        {
            //Get Lower Taxis and Id
            var sqlString = SqlDifferences.GetSqlString(tableName, new List<string>
                {
                    Attr.Id,
                    Attr.Taxis
                },
                isTop
                    ? $"WHERE (Taxis < (SELECT Taxis FROM {tableName} WHERE Id = {contentId}) AND Taxis >= {TaxisIsTopStartValue} AND ChannelId = {channelId})"
                    : $"WHERE (Taxis < (SELECT Taxis FROM {tableName} WHERE Id = {contentId}) AND Taxis < {TaxisIsTopStartValue} AND ChannelId = {channelId})",
                "ORDER BY Taxis DESC", 1);
            var lowerId = 0;
            var lowerTaxis = 0;

            using (var rdr = DatabaseApi.ExecuteReader(ConnectionString, sqlString))
            {
                if (rdr.Read())
                {
                    lowerId = DatabaseApi.GetInt(rdr, 0);
                    lowerTaxis = DatabaseApi.GetInt(rdr, 1);
                }
                rdr.Close();
            }

            if (lowerId != 0)
            {
                //Get Taxis Of Selected Class
                var selectedTaxis = GetTaxis(contentId, tableName);

                //Set The Selected Class Taxis To Lower Level
                UpdateTaxis(channelId, contentId, lowerTaxis, tableName);
                //Set The Lower Class Taxis To Higher Level
                UpdateTaxis(channelId, lowerId, selectedTaxis, tableName);
                return true;
            }
            return false;
        }

        public int GetMaxTaxis(string tableName, int channelId, bool isTop)
        {
            var maxTaxis = 0;
            if (isTop)
            {
                maxTaxis = TaxisIsTopStartValue;

                var sqlString =
                    $"SELECT MAX(Taxis) FROM {tableName} WHERE ChannelId = {channelId} AND Taxis >= {TaxisIsTopStartValue}";

                using (var conn = GetConnection())
                {
                    conn.Open();
                    using (var rdr = DatabaseApi.ExecuteReader(conn, sqlString))
                    {
                        if (rdr.Read())
                        {
                            maxTaxis = DatabaseApi.GetInt(rdr, 0);
                        }
                        rdr.Close();
                    }
                }

                if (maxTaxis < TaxisIsTopStartValue)
                {
                    maxTaxis = TaxisIsTopStartValue;
                }
            }
            else
            {
                var sqlString =
                    $"SELECT MAX(Taxis) FROM {tableName} WHERE ChannelId = {channelId} AND Taxis < {TaxisIsTopStartValue}";
                using (var conn = GetConnection())
                {
                    conn.Open();
                    using (var rdr = DatabaseApi.ExecuteReader(conn, sqlString))
                    {
                        if (rdr.Read())
                        {
                            maxTaxis = DatabaseApi.GetInt(rdr, 0);
                        }
                        rdr.Close();
                    }
                }
            }
            return maxTaxis;
        }

        public Tuple<int, string> GetValue(string tableName, int contentId, string name)
        {
            Tuple<int, string> tuple = null;

            try
            {
                var sqlString = $"SELECT {Attr.ChannelId}, {name} FROM {tableName} WHERE Id = {contentId}";

                using (var conn = GetConnection())
                {
                    conn.Open();
                    using (var rdr = DatabaseApi.ExecuteReader(conn, sqlString))
                    {
                        if (rdr.Read())
                        {
                            var channelId = DatabaseApi.GetInt(rdr, 0);
                            var value = DatabaseApi.GetString(rdr, 1);

                            tuple = new Tuple<int, string>(channelId, value);
                        }

                        rdr.Close();
                    }
                }
            }
            catch
            {
                // ignored
            }

            return tuple;
        }

        public void AddContentGroupList(string tableName, int contentId, List<string> contentGroupList)
        {
            var tuple = GetValue(tableName, contentId, Attr.GroupNameCollection);

            if (tuple != null)
            {
                var list = TranslateUtils.StringCollectionToStringList(tuple.Item2);
                foreach (var groupName in contentGroupList)
                {
                    if (!list.Contains(groupName)) list.Add(groupName);
                }
                Update(tableName, tuple.Item1, contentId, Attr.GroupNameCollection, TranslateUtils.ObjectCollectionToString(list));
            }
        }
        

        public List<int> GetContentIdList(string tableName, int channelId, bool isPeriods, string dateFrom, string dateTo, ETriState checkedState)
        {
            var list = new List<int>();

            var sqlString = $"SELECT Id FROM {tableName} WHERE ChannelId = {channelId}";
            if (isPeriods)
            {
                var dateString = string.Empty;
                if (!string.IsNullOrEmpty(dateFrom))
                {
                    dateString = $" AND AddDate >= {SqlUtils.GetComparableDate(TranslateUtils.ToDateTime(dateFrom))} ";
                }
                if (!string.IsNullOrEmpty(dateTo))
                {
                    dateString += $" AND AddDate <= {SqlUtils.GetComparableDate(TranslateUtils.ToDateTime(dateTo).AddDays(1))} ";
                }
                sqlString += dateString;
            }

            if (checkedState != ETriState.All)
            {
                sqlString += $" AND IsChecked = '{ETriStateUtils.GetValue(checkedState)}'";
            }

            sqlString += " ORDER BY Taxis DESC, Id DESC";

            using (var rdr = DatabaseApi.ExecuteReader(ConnectionString, sqlString))
            {
                while (rdr.Read())
                {
                    var contentId = DatabaseApi.GetInt(rdr, 0);
                    list.Add(contentId);
                }
                rdr.Close();
            }
            return list;
        }

        public List<int> GetContentIdListCheckedByChannelId(string tableName, int siteId, int channelId)
        {
            var list = new List<int>();

            var sqlString = $"SELECT Id FROM {tableName} WHERE SiteId = {siteId} AND ChannelId = {channelId} AND IsChecked = '{true}'";
            using (var rdr = DatabaseApi.ExecuteReader(ConnectionString, sqlString))
            {
                while (rdr.Read())
                {
                    list.Add(DatabaseApi.GetInt(rdr, 0));
                }
                rdr.Close();
            }
            return list;
        }

        public int GetContentId(string tableName, int channelId, int taxis, bool isNextContent)
        {
            var contentId = 0;
            var sqlString = SqlDifferences.GetSqlString(tableName, new List<string>
            {
                Attr.Id
            }, $"WHERE (ChannelId = {channelId} AND Taxis > {taxis} AND IsChecked = 'True')", "ORDER BY Taxis", 1);
            if (isNextContent)
            {
                sqlString = SqlDifferences.GetSqlString(tableName, new List<string>
                    {
                        Attr.Id
                    },
                $"WHERE (ChannelId = {channelId} AND Taxis < {taxis} AND IsChecked = 'True')", "ORDER BY Taxis DESC", 1);
            }

            using (var rdr = DatabaseApi.ExecuteReader(ConnectionString, sqlString))
            {
                if (rdr.Read())
                {
                    contentId = DatabaseApi.GetInt(rdr, 0);
                }
                rdr.Close();
            }
            return contentId;
        }

        public int GetContentId(string tableName, int channelId, string orderByString)
        {
            var contentId = 0;
            var sqlString = SqlDifferences.GetSqlString(tableName, new List<string>
            {
                Attr.Id
            }, $"WHERE (ChannelId = {channelId})", orderByString, 1);

            using (var rdr = DatabaseApi.ExecuteReader(ConnectionString, sqlString))
            {
                if (rdr.Read())
                {
                    contentId = DatabaseApi.GetInt(rdr, 0);
                }
                rdr.Close();
            }
            return contentId;
        }

        public List<string> GetValueListByStartString(string tableName, int channelId, string name, string startString, int totalNum)
        {
            var inStr = SqlUtils.GetInStr(name, startString);
            var sqlString = SqlDifferences.GetSqlString(tableName, new List<string> { name }, $"WHERE ChannelId = {channelId} AND {inStr}", string.Empty, 0, totalNum, true);
            return DatabaseApi.GetStringList(sqlString);
        }

        public int GetChannelId(string tableName, int contentId)
        {
            var channelId = 0;
            var sqlString = $"SELECT {Attr.ChannelId} FROM {tableName} WHERE (Id = {contentId})";

            using (var rdr = DatabaseApi.ExecuteReader(ConnectionString, sqlString))
            {
                if (rdr.Read())
                {
                    channelId = DatabaseApi.GetInt(rdr, 0);
                }
                rdr.Close();
            }
            return channelId;
        }

        public int GetSequence(string tableName, int channelId, int contentId)
        {
            var sqlString =
                $"SELECT COUNT(*) FROM {tableName} WHERE {Attr.ChannelId} = {channelId} AND {Attr.IsChecked} = '{true}' AND Taxis < (SELECT Taxis FROM {tableName} WHERE Id = {contentId}) AND {Attr.SourceId} != {SourceManager.Preview}";

            return DatabaseApi.GetIntResult(sqlString) + 1;
        }

        public List<int> GetChannelIdListCheckedByLastEditDateHour(string tableName, int siteId, int hour)
        {
            var list = new List<int>();

            var sqlString =
                $"SELECT DISTINCT ChannelId FROM {tableName} WHERE (SiteId = {siteId}) AND (IsChecked = '{true}') AND (LastEditDate BETWEEN {SqlUtils.GetComparableDateTime(DateTime.Now.AddHours(-hour))} AND {SqlUtils.GetComparableNow()})";

            using (var rdr = DatabaseApi.ExecuteReader(ConnectionString, sqlString))
            {
                while (rdr.Read())
                {
                    var channelId = DatabaseApi.GetInt(rdr, 0);
                    list.Add(channelId);
                }
                rdr.Close();
            }
            return list;
        }

        public DataSet GetDataSetOfAdminExcludeRecycle(string tableName, int siteId, DateTime begin, DateTime end)
        {
            var sqlString = GetSqlStringOfAdminExcludeRecycle(tableName, siteId, begin, end);

            return DatabaseApi.ExecuteDataset(ConnectionString, sqlString);
        }

        public int Insert(string tableName, SiteInfo siteInfo, ChannelInfo channelInfo, ContentInfo contentInfo)
        {
            var taxis = 0;
            if (contentInfo.SourceId == SourceManager.Preview)
            {
                channelInfo.IsPreviewContentsExists = true;
                DataProvider.Channel.UpdateExtend(channelInfo);
            }
            else
            {
                taxis = GetTaxisToInsert(tableName, contentInfo.ChannelId, contentInfo.Top);
            }
            return InsertWithTaxis(tableName, siteInfo, channelInfo, contentInfo, taxis);
        }

        public int InsertPreview(string tableName, SiteInfo siteInfo, ChannelInfo channelInfo, ContentInfo contentInfo)
        {
            channelInfo.IsPreviewContentsExists = true;
            DataProvider.Channel.UpdateExtend(channelInfo);

            contentInfo.SourceId = SourceManager.Preview;
            return InsertWithTaxis(tableName, siteInfo, channelInfo, contentInfo, 0);
        }

        public int InsertWithTaxis(string tableName, SiteInfo siteInfo, ChannelInfo channelInfo, ContentInfo contentInfo, int taxis)
        {
            if (string.IsNullOrEmpty(tableName)) return 0;

            if (siteInfo.IsAutoPageInTextEditor && contentInfo.ContainsKey(ContentAttribute.Content))
            {
                contentInfo.Content = ContentUtility.GetAutoPageContent(contentInfo.Content, siteInfo.AutoPageWordNum);
            }

            contentInfo.Taxis = taxis;

            var contentId = InsertInner(tableName, siteInfo, channelInfo, contentInfo);

            return contentId;
        }

        private int InsertInner(string tableName, SiteInfo siteInfo, ChannelInfo channelInfo, ContentInfo contentInfo)
        {
            if (string.IsNullOrEmpty(tableName) || contentInfo == null) return 0;

            contentInfo.LastEditDate = DateTime.Now;

            var columnInfoList = TableColumnManager.GetTableColumnInfoList(tableName, Attr.AllAttributes.Value);

            var names = new StringBuilder();
            var values = new StringBuilder();
            var paras = new List<IDataParameter>();
            var excludeAttributesNames = new List<string>(Attr.AllAttributes.Value);
            foreach (var columnInfo in columnInfoList)
            {
                excludeAttributesNames.Add(columnInfo.AttributeName);
                names.Append($",{columnInfo.AttributeName}").AppendLine();
                values.Append($",@{columnInfo.AttributeName}").AppendLine();
                if (columnInfo.DataType == DataType.Integer)
                {
                    paras.Add(GetParameter(columnInfo.AttributeName, contentInfo.Get<int>(columnInfo.AttributeName)));
                }
                else if (columnInfo.DataType == DataType.Decimal)
                {
                    paras.Add(GetParameter(columnInfo.AttributeName, contentInfo.Get<decimal>(columnInfo.AttributeName)));
                }
                else if (columnInfo.DataType == DataType.Boolean)
                {
                    paras.Add(GetParameter(columnInfo.AttributeName, contentInfo.Get<bool>(columnInfo.AttributeName)));
                }
                else if (columnInfo.DataType == DataType.DateTime)
                {
                    paras.Add(GetParameter(columnInfo.AttributeName, contentInfo.Get<DateTime?>(columnInfo.AttributeName)));
                }
                else
                {
                    paras.Add(GetParameter(columnInfo.AttributeName, contentInfo.Get(columnInfo.AttributeName, string.Empty)));
                }
            }

            var sqlString = $@"
INSERT INTO {tableName} (
    {Attr.ChannelId},
    {Attr.SiteId},
    {Attr.AddUserName},
    {Attr.LastEditUserName},
    {Attr.LastEditDate},
    {Attr.AdminId},
    {Attr.UserId},
    {Attr.Taxis},
    {Attr.GroupNameCollection},
    {Attr.Tags},
    {Attr.SourceId},
    {Attr.ReferenceId},
    {Attr.IsChecked},
    {Attr.CheckedLevel},
    {Attr.Hits},
    {Attr.HitsByDay},
    {Attr.HitsByWeek},
    {Attr.HitsByMonth},
    {Attr.LastHitsDate},
    {Attr.SettingsXml},
    {Attr.Title},
    {Attr.IsTop},
    {Attr.IsRecommend},
    {Attr.IsHot},
    {Attr.IsColor},
    {Attr.LinkUrl},
    {Attr.AddDate}
    {names}
) VALUES (
    @{Attr.ChannelId},
    @{Attr.SiteId},
    @{Attr.AddUserName},
    @{Attr.LastEditUserName},
    @{Attr.LastEditDate},
    @{Attr.AdminId},
    @{Attr.UserId},
    @{Attr.Taxis},
    @{Attr.GroupNameCollection},
    @{Attr.Tags},
    @{Attr.SourceId},
    @{Attr.ReferenceId},
    @{Attr.IsChecked},
    @{Attr.CheckedLevel},
    @{Attr.Hits},
    @{Attr.HitsByDay},
    @{Attr.HitsByWeek},
    @{Attr.HitsByMonth},
    @{Attr.LastHitsDate},
    @{Attr.SettingsXml},
    @{Attr.Title},
    @{Attr.IsTop},
    @{Attr.IsRecommend},
    @{Attr.IsHot},
    @{Attr.IsColor},
    @{Attr.LinkUrl},
    @{Attr.AddDate}
    {values}
)";

            var parameters = new List<IDataParameter>
            {
                GetParameter(Attr.ChannelId, contentInfo.ChannelId),
                GetParameter(Attr.SiteId, contentInfo.SiteId),
                GetParameter(Attr.AddUserName, contentInfo.AddUserName),
                GetParameter(Attr.LastEditUserName, contentInfo.LastEditUserName),
                GetParameter(Attr.LastEditDate,contentInfo.LastEditDate),
                GetParameter(Attr.AdminId, contentInfo.AdminId),
                GetParameter(Attr.UserId, contentInfo.UserId),
                GetParameter(Attr.Taxis, contentInfo.Taxis),
                GetParameter(Attr.GroupNameCollection, contentInfo.GroupNameCollection),
                GetParameter(Attr.Tags, contentInfo.Tags),
                GetParameter(Attr.SourceId, contentInfo.SourceId),
                GetParameter(Attr.ReferenceId, contentInfo.ReferenceId),
                GetParameter(Attr.IsChecked, contentInfo.Checked.ToString()),
                GetParameter(Attr.CheckedLevel, contentInfo.CheckedLevel),
                GetParameter(Attr.Hits, contentInfo.Hits),
                GetParameter(Attr.HitsByDay, contentInfo.HitsByDay),
                GetParameter(Attr.HitsByWeek, contentInfo.HitsByWeek),
                GetParameter(Attr.HitsByMonth, contentInfo.HitsByMonth),
                GetParameter(Attr.LastHitsDate,contentInfo.LastHitsDate),
                GetParameter(Attr.SettingsXml,contentInfo.SettingsXml),
                GetParameter(Attr.Title, contentInfo.Title),
                GetParameter(Attr.IsTop, contentInfo.Top.ToString()),
                GetParameter(Attr.IsRecommend, contentInfo.Recommend.ToString()),
                GetParameter(Attr.IsHot, contentInfo.Hot.ToString()),
                GetParameter(Attr.IsColor, contentInfo.Color.ToString()),
                GetParameter(Attr.LinkUrl, contentInfo.LinkUrl),
                GetParameter(Attr.AddDate,contentInfo.AddDate)
            };
            parameters.AddRange(paras);

            contentInfo.Id = DatabaseApi.ExecuteNonQueryAndReturnId(tableName, Attr.Id, ConnectionString, sqlString, parameters.ToArray());

            ContentManager.InsertCache(siteInfo, channelInfo, contentInfo);

            return contentInfo.Id;
        }

        public void Update(SiteInfo siteInfo, ChannelInfo channelInfo, ContentInfo contentInfo)
        {
            if (contentInfo == null) return;

            if (siteInfo.IsAutoPageInTextEditor &&
                contentInfo.ContainsKey(ContentAttribute.Content))
            {
                contentInfo.Content =  ContentUtility.GetAutoPageContent(contentInfo.Content,
                        siteInfo.AutoPageWordNum);
            }

            var tableName = ChannelManager.GetTableName(siteInfo, channelInfo);

            //出现IsTop与Taxis不同步情况
            if (contentInfo.Top == false && contentInfo.Taxis >= TaxisIsTopStartValue)
            {
                contentInfo.Taxis = GetMaxTaxis(tableName, contentInfo.ChannelId, false) + 1;
            }
            else if (contentInfo.Top && contentInfo.Taxis < TaxisIsTopStartValue)
            {
                contentInfo.Taxis = GetMaxTaxis(tableName, contentInfo.ChannelId, true) + 1;
            }

            contentInfo.LastEditDate = DateTime.Now;

            var columnInfoList =
                TableColumnManager.GetTableColumnInfoList(tableName, Attr.AllAttributes.Value);

            var sets = new StringBuilder();
            var paras = new List<IDataParameter>();
            var excludeAttributesNames = new List<string>(Attr.AllAttributes.Value);
            foreach (var columnInfo in columnInfoList)
            {
                excludeAttributesNames.Add(columnInfo.AttributeName);
                sets.Append($",{columnInfo.AttributeName} = @{columnInfo.AttributeName}").AppendLine();
                if (columnInfo.DataType == DataType.Integer)
                {
                    paras.Add(GetParameter(columnInfo.AttributeName,
                        contentInfo.Get<int>(columnInfo.AttributeName)));
                }
                else if (columnInfo.DataType == DataType.Decimal)
                {
                    paras.Add(GetParameter(columnInfo.AttributeName,
                        contentInfo.Get<decimal>(columnInfo.AttributeName)));
                }
                else if (columnInfo.DataType == DataType.Boolean)
                {
                    paras.Add(GetParameter(columnInfo.AttributeName,
                        contentInfo.Get<bool>(columnInfo.AttributeName)));
                }
                else if (columnInfo.DataType == DataType.DateTime)
                {
                    paras.Add(GetParameter(columnInfo.AttributeName,
                        contentInfo.Get<DateTime?>(columnInfo.AttributeName)));
                }
                else
                {
                    paras.Add(GetParameter(columnInfo.AttributeName,
                        contentInfo.Get(columnInfo.AttributeName, string.Empty)));
                }
            }

            var sqlString = $@"
UPDATE {tableName} SET 
{Attr.ChannelId} = @{Attr.ChannelId},
{Attr.SiteId} = @{Attr.SiteId},
{Attr.AddUserName} = @{Attr.AddUserName},
{Attr.LastEditUserName} = @{Attr.LastEditUserName},
{Attr.LastEditDate} = @{Attr.LastEditDate},
{Attr.AdminId} = @{Attr.AdminId},
{Attr.UserId} = @{Attr.UserId},
{Attr.Taxis} = @{Attr.Taxis},
{Attr.GroupNameCollection} = @{Attr.GroupNameCollection},
{Attr.Tags} = @{Attr.Tags},
{Attr.SourceId} = @{Attr.SourceId},
{Attr.ReferenceId} = @{Attr.ReferenceId},";

            if (contentInfo.CheckedLevel != CheckManager.LevelInt.NotChange)
            {
                sqlString += $@"
{Attr.IsChecked} = @{Attr.IsChecked},
{Attr.CheckedLevel} = @{Attr.CheckedLevel},";
            }

            sqlString += $@"
{Attr.Hits} = @{Attr.Hits},
{Attr.HitsByDay} = @{Attr.HitsByDay},
{Attr.HitsByWeek} = @{Attr.HitsByWeek},
{Attr.HitsByMonth} = @{Attr.HitsByMonth},
{Attr.LastHitsDate} = @{Attr.LastHitsDate},
{Attr.SettingsXml} = @{Attr.SettingsXml},
{Attr.Title} = @{Attr.Title},
{Attr.IsTop} = @{Attr.IsTop},
{Attr.IsRecommend} = @{Attr.IsRecommend},
{Attr.IsHot} = @{Attr.IsHot},
{Attr.IsColor} = @{Attr.IsColor},
{Attr.LinkUrl} = @{Attr.LinkUrl},
{Attr.AddDate} = @{Attr.AddDate}
{sets}
WHERE {Attr.Id} = @{Attr.Id}";

            var parameters = new List<IDataParameter>
            {
                GetParameter(Attr.ChannelId, channelInfo.Id),
                GetParameter(Attr.SiteId, siteInfo.Id),
                GetParameter(Attr.AddUserName, contentInfo.AddUserName),
                GetParameter(Attr.LastEditUserName, contentInfo.LastEditUserName),
                GetParameter(Attr.LastEditDate, contentInfo.LastEditDate),
                GetParameter(Attr.AdminId, contentInfo.AdminId),
                GetParameter(Attr.UserId, contentInfo.UserId),
                GetParameter(Attr.Taxis, contentInfo.Taxis),
                GetParameter(Attr.GroupNameCollection, contentInfo.GroupNameCollection),
                GetParameter(Attr.Tags, contentInfo.Tags),
                GetParameter(Attr.SourceId, contentInfo.SourceId),
                GetParameter(Attr.ReferenceId, contentInfo.ReferenceId),
            };
            if (contentInfo.CheckedLevel != CheckManager.LevelInt.NotChange)
            {
                parameters.Add(GetParameter(Attr.IsChecked, contentInfo.Checked.ToString()));
                parameters.Add(GetParameter(Attr.CheckedLevel, contentInfo.CheckedLevel));
            }

            parameters.Add(GetParameter(Attr.Hits, contentInfo.Hits));
            parameters.Add(GetParameter(Attr.HitsByDay, contentInfo.HitsByDay));
            parameters.Add(GetParameter(Attr.HitsByWeek, contentInfo.HitsByWeek));
            parameters.Add(GetParameter(Attr.HitsByMonth, contentInfo.HitsByMonth));
            parameters.Add(GetParameter(Attr.LastHitsDate, contentInfo.LastHitsDate));
            parameters.Add(GetParameter(Attr.SettingsXml, contentInfo.SettingsXml));
            parameters.Add(GetParameter(Attr.Title, contentInfo.Title));
            parameters.Add(GetParameter(Attr.IsTop, contentInfo.Top.ToString()));
            parameters.Add(GetParameter(Attr.IsRecommend, contentInfo.Recommend.ToString()));
            parameters.Add(GetParameter(Attr.IsHot, contentInfo.Hot.ToString()));
            parameters.Add(GetParameter(Attr.IsColor, contentInfo.Color.ToString()));
            parameters.Add(GetParameter(Attr.LinkUrl, contentInfo.LinkUrl));
            parameters.Add(GetParameter(Attr.AddDate, contentInfo.AddDate));

            parameters.AddRange(paras);
            parameters.Add(GetParameter(Attr.Id, contentInfo.Id));

            DatabaseApi.ExecuteNonQuery(ConnectionString, sqlString, parameters.ToArray());

            ContentManager.UpdateCache(siteInfo, channelInfo, contentInfo);
        }

        public void Update(string tableName, int channelId, int contentId, string name, string value)
        {
            var sqlString = $"UPDATE {tableName} SET {name} = @{name} WHERE Id = @Id";

            var parameters = new DynamicParameters();
            parameters.Add($"@{name}", value);
            parameters.Add("@Id", contentId);

            using (var connection = GetConnection())
            {
                connection.Execute(sqlString, parameters);
            }

            ContentManager.RemoveCache(tableName, channelId);
        }

        public int GetCountOfContentAdd(string tableName, int siteId, int channelId, EScopeType scope, DateTime begin, DateTime end, string userName, ETriState checkedState)
        {
            var channelInfo = ChannelManager.GetChannelInfo(siteId, channelId);
            var channelIdList = ChannelManager.GetChannelIdList(channelInfo, scope, string.Empty, string.Empty, string.Empty);
            return GetCountOfContentAdd(tableName, siteId, channelIdList, begin, end, userName, checkedState);
        }

        public List<int> GetContentIdListChecked(string tableName, int channelId, string orderByFormatString)
        {
            return GetContentIdListChecked(tableName, channelId, orderByFormatString, string.Empty);
        }

        public int GetCountOfContentUpdate(string tableName, int siteId, int channelId, EScopeType scope, DateTime begin, DateTime end, string userName)
        {
            var channelInfo = ChannelManager.GetChannelInfo(siteId, channelId);
            var channelIdList = ChannelManager.GetChannelIdList(channelInfo, scope, string.Empty, string.Empty, string.Empty);
            return GetCountOfContentUpdate(tableName, siteId, channelIdList, begin, end, userName);
        }

        private int GetCountOfContentUpdate(string tableName, int siteId, List<int> channelIdList, DateTime begin, DateTime end, string userName)
        {
            string sqlString;
            if (string.IsNullOrEmpty(userName))
            {
                sqlString = channelIdList.Count == 1
                    ? $"SELECT COUNT(Id) AS Num FROM {tableName} WHERE SiteId = {siteId} AND ChannelId = {channelIdList[0]} AND (LastEditDate BETWEEN {SqlUtils.GetComparableDate(begin)} AND {SqlUtils.GetComparableDate(end.AddDays(1))}) AND (LastEditDate <> AddDate)"
                    : $"SELECT COUNT(Id) AS Num FROM {tableName} WHERE SiteId = {siteId} AND ChannelId IN ({TranslateUtils.ToSqlInStringWithoutQuote(channelIdList)}) AND (LastEditDate BETWEEN {SqlUtils.GetComparableDate(begin)} AND {SqlUtils.GetComparableDate(end.AddDays(1))}) AND (LastEditDate <> AddDate)";
            }
            else
            {
                sqlString = channelIdList.Count == 1
                    ? $"SELECT COUNT(Id) AS Num FROM {tableName} WHERE SiteId = {siteId} AND ChannelId = {channelIdList[0]} AND (LastEditDate BETWEEN {SqlUtils.GetComparableDate(begin)} AND {SqlUtils.GetComparableDate(end.AddDays(1))}) AND (LastEditDate <> AddDate) AND (AddUserName = '{userName}')"
                    : $"SELECT COUNT(Id) AS Num FROM {tableName} WHERE SiteId = {siteId} AND ChannelId IN ({TranslateUtils.ToSqlInStringWithoutQuote(channelIdList)}) AND (LastEditDate BETWEEN {SqlUtils.GetComparableDate(begin)} AND {SqlUtils.GetComparableDate(end.AddDays(1))}) AND (LastEditDate <> AddDate) AND (AddUserName = '{userName}')";
            }

            return DatabaseApi.GetIntResult(sqlString);
        }

        public DataSet GetStlDataSourceChecked(List<int> channelIdList, string tableName, int startNum, int totalNum, string orderByString, string whereString, NameValueCollection others)
        {
            return GetStlDataSourceChecked(tableName, channelIdList, startNum, totalNum, orderByString, whereString, others);
        }

        public List<int> GetIdListBySameTitle(string tableName, int channelId, string title)
        {
            var list = new List<int>();
            var sql = $"SELECT Id FROM {tableName} WHERE ChannelId = {channelId} AND Title = '{title}'";
            using (var rdr = DatabaseApi.ExecuteReader(ConnectionString, sql))
            {
                while (rdr.Read())
                {
                    list.Add(DatabaseApi.GetInt(rdr, 0));
                }
                rdr.Close();
            }
            return list;
        }

        public int GetCount(string tableName, string whereString)
        {
            if (!string.IsNullOrEmpty(whereString))
            {
                whereString = whereString.Replace("WHERE ", string.Empty).Replace("where ", string.Empty);
            }
            whereString = string.IsNullOrEmpty(whereString) ? string.Empty : $"WHERE {whereString}";

            var sqlString = $"SELECT COUNT(*) FROM {tableName} {whereString}";

            return DatabaseApi.GetIntResult(sqlString);
        }

        public List<ContentCountInfo> GetContentCountInfoList(string tableName)
        {
            List<ContentCountInfo> list;

            var sqlString =
                $@"SELECT {Attr.SiteId}, {Attr.ChannelId}, {Attr.IsChecked}, {Attr.CheckedLevel}, {Attr.AdminId}, COUNT(*) AS {nameof(ContentCountInfo.Count)} FROM {tableName} WHERE {Attr.ChannelId} > 0 AND {Attr.SourceId} != {SourceManager.Preview} GROUP BY {Attr.SiteId}, {Attr.ChannelId}, {Attr.IsChecked}, {Attr.CheckedLevel}, {Attr.AdminId}";

            using (var connection = GetConnection())
            {
                list = connection.Query<ContentCountInfo>(sqlString).ToList();
            }

            return list;
        }

        public int GetCountCheckedImage(int siteId, int channelId)
        {
            var tableName = SiteManager.GetSiteInfo(siteId).TableName;
            var sqlString =
                $"SELECT COUNT(*) FROM {tableName} WHERE {Attr.ChannelId} = {channelId} AND {ContentAttribute.ImageUrl} != '' AND {Attr.IsChecked} = '{true}' AND {Attr.SourceId} != {SourceManager.Preview}";

            return DatabaseApi.GetIntResult(sqlString);
        }

        private int GetTaxis(int selectedId, string tableName)
        {
            var sqlString = $"SELECT Taxis FROM {tableName} WHERE (Id = {selectedId})";

            return DatabaseApi.GetIntResult(sqlString);
        }

        //private List<int> GetContentIdListByTrash(int siteId, string tableName)
        //{
        //    var sqlString =
        //        $"SELECT Id FROM {tableName} WHERE SiteId = {siteId} AND ChannelId < 0";
        //    return DatabaseApi.GetIntList(sqlString);
        //}

        private DataSet GetStlDataSourceChecked(string tableName, List<int> channelIdList, int startNum, int totalNum, string orderByString, string whereString, NameValueCollection others)
        {
            if (channelIdList == null || channelIdList.Count == 0)
            {
                return null;
            }
            var sqlWhereString = channelIdList.Count == 1 ? $"WHERE (ChannelId = {channelIdList[0]} AND IsChecked = '{true}' {whereString})" : $"WHERE (ChannelId IN ({TranslateUtils.ToSqlInStringWithoutQuote(channelIdList)}) AND IsChecked = '{true}' {whereString})";

            if (others != null && others.Count > 0)
            {
                var columnNameList = TableColumnManager.GetTableColumnNameList(tableName);

                foreach (var attributeName in others.AllKeys)
                {
                    if (StringUtils.ContainsIgnoreCase(columnNameList, attributeName))
                    {
                        var value = others.Get(attributeName);
                        if (!string.IsNullOrEmpty(value))
                        {
                            value = value.Trim();
                            if (StringUtils.StartsWithIgnoreCase(value, "not:"))
                            {
                                value = value.Substring("not:".Length);
                                if (value.IndexOf(',') == -1)
                                {
                                    sqlWhereString += $" AND ({attributeName} <> '{value}')";
                                }
                                else
                                {
                                    var collection = TranslateUtils.StringCollectionToStringList(value);
                                    foreach (var val in collection)
                                    {
                                        sqlWhereString += $" AND ({attributeName} <> '{val}')";
                                    }
                                }
                            }
                            else if (StringUtils.StartsWithIgnoreCase(value, "contains:"))
                            {
                                value = value.Substring("contains:".Length);
                                if (value.IndexOf(',') == -1)
                                {
                                    sqlWhereString += $" AND ({attributeName} LIKE '%{value}%')";
                                }
                                else
                                {
                                    var builder = new StringBuilder(" AND (");
                                    var collection = TranslateUtils.StringCollectionToStringList(value);
                                    foreach (var val in collection)
                                    {
                                        builder.Append($" {attributeName} LIKE '%{val}%' OR ");
                                    }
                                    builder.Length -= 3;

                                    builder.Append(")");

                                    sqlWhereString += builder.ToString();
                                }
                            }
                            else if (StringUtils.StartsWithIgnoreCase(value, "start:"))
                            {
                                value = value.Substring("start:".Length);
                                if (value.IndexOf(',') == -1)
                                {
                                    sqlWhereString += $" AND ({attributeName} LIKE '{value}%')";
                                }
                                else
                                {
                                    var builder = new StringBuilder(" AND (");
                                    var collection = TranslateUtils.StringCollectionToStringList(value);
                                    foreach (var val in collection)
                                    {
                                        builder.Append($" {attributeName} LIKE '{val}%' OR ");
                                    }
                                    builder.Length -= 3;

                                    builder.Append(")");

                                    sqlWhereString += builder.ToString();
                                }
                            }
                            else if (StringUtils.StartsWithIgnoreCase(value, "end:"))
                            {
                                value = value.Substring("end:".Length);
                                if (value.IndexOf(',') == -1)
                                {
                                    sqlWhereString += $" AND ({attributeName} LIKE '%{value}')";
                                }
                                else
                                {
                                    var builder = new StringBuilder(" AND (");
                                    var collection = TranslateUtils.StringCollectionToStringList(value);
                                    foreach (var val in collection)
                                    {
                                        builder.Append($" {attributeName} LIKE '%{val}' OR ");
                                    }
                                    builder.Length -= 3;

                                    builder.Append(")");

                                    sqlWhereString += builder.ToString();
                                }
                            }
                            else
                            {
                                if (value.IndexOf(',') == -1)
                                {
                                    sqlWhereString += $" AND ({attributeName} = '{value}')";
                                }
                                else
                                {
                                    var builder = new StringBuilder(" AND (");
                                    var collection = TranslateUtils.StringCollectionToStringList(value);
                                    foreach (var val in collection)
                                    {
                                        builder.Append($" {attributeName} = '{val}' OR ");
                                    }
                                    builder.Length -= 3;

                                    builder.Append(")");

                                    sqlWhereString += builder.ToString();
                                }
                            }
                        }
                    }
                }
            }

            return startNum <= 1 ? GetStlDataSourceByContentNumAndWhereString(tableName, totalNum, sqlWhereString, orderByString) : GetStlDataSourceByStartNum(tableName, startNum, totalNum, sqlWhereString, orderByString);
        }

        private DataSet GetStlDataSourceByContentNumAndWhereString(string tableName, int totalNum, string whereString, string orderByString)
        {
            DataSet dataSet = null;
            if (!string.IsNullOrEmpty(tableName))
            {
                var sqlSelect = DatabaseApi.GetSelectSqlString(tableName, totalNum, MinListColumns, whereString, orderByString);
                dataSet = DatabaseApi.ExecuteDataset(ConnectionString, sqlSelect);
            }
            return dataSet;
        }

        private DataSet GetStlDataSourceByStartNum(string tableName, int startNum, int totalNum, string whereString, string orderByString)
        {
            DataSet dataSet = null;
            if (!string.IsNullOrEmpty(tableName))
            {
                var sqlSelect = SqlDifferences.GetSqlString(tableName, MinListColumns, whereString, orderByString, startNum - 1, totalNum);
                dataSet = DatabaseApi.ExecuteDataset(ConnectionString, sqlSelect);
            }
            return dataSet;
        }

        private int GetTaxisToInsert(string tableName, int channelId, bool isTop)
        {
            int taxis;

            if (isTop)
            {
                taxis = GetMaxTaxis(tableName, channelId, true) + 1;
            }
            else
            {
                taxis = GetMaxTaxis(tableName, channelId, false) + 1;
            }

            return taxis;
        }

        private int GetCountOfContentAdd(string tableName, int siteId, List<int> channelIdList, DateTime begin, DateTime end, string userName, ETriState checkedState)
        {
            string sqlString;
            if (string.IsNullOrEmpty(userName))
            {
                sqlString = channelIdList.Count == 1
                    ? $"SELECT COUNT(Id) AS Num FROM {tableName} WHERE SiteId = {siteId} AND ChannelId = {channelIdList[0]} AND (AddDate BETWEEN {SqlUtils.GetComparableDate(begin)} AND {SqlUtils.GetComparableDate(end.AddDays(1))})"
                    : $"SELECT COUNT(Id) AS Num FROM {tableName} WHERE SiteId = {siteId} AND ChannelId IN ({TranslateUtils.ToSqlInStringWithoutQuote(channelIdList)}) AND (AddDate BETWEEN {SqlUtils.GetComparableDate(begin)} AND {SqlUtils.GetComparableDate(end.AddDays(1))})";
            }
            else
            {
                sqlString = channelIdList.Count == 1
                    ? $"SELECT COUNT(Id) AS Num FROM {tableName} WHERE SiteId = {siteId} AND ChannelId = {channelIdList[0]} AND (AddDate BETWEEN {SqlUtils.GetComparableDate(begin)} AND {SqlUtils.GetComparableDate(end.AddDays(1))}) AND (AddUserName = '{userName}')"
                    : $"SELECT COUNT(Id) AS Num FROM {tableName} WHERE SiteId = {siteId} AND ChannelId IN ({TranslateUtils.ToSqlInStringWithoutQuote(channelIdList)}) AND (AddDate BETWEEN {SqlUtils.GetComparableDate(begin)} AND {SqlUtils.GetComparableDate(end.AddDays(1))}) AND (AddUserName = '{userName}')";
            }

            if (checkedState != ETriState.All)
            {
                sqlString += $" AND {Attr.IsChecked} = '{ETriStateUtils.GetValue(checkedState)}'";
            }

            return DatabaseApi.GetIntResult(sqlString);
        }

        private List<int> GetContentIdListChecked(string tableName, int channelId, int totalNum, string orderByFormatString, string whereString)
        {
            var channelIdList = new List<int>
            {
                channelId
            };
            return GetContentIdListChecked(tableName, channelIdList, totalNum, orderByFormatString, whereString);
        }

        private List<int> GetContentIdListChecked(string tableName, List<int> channelIdList, int totalNum, string orderString, string whereString)
        {
            var list = new List<int>();

            if (channelIdList == null || channelIdList.Count == 0)
            {
                return list;
            }

            string sqlString;

            if (totalNum > 0)
            {
                sqlString = SqlDifferences.GetSqlString(tableName, new List<string>
                    {
                        Attr.Id
                    },
                    channelIdList.Count == 1
                        ? $"WHERE (ChannelId = {channelIdList[0]} AND IsChecked = '{true}' {whereString})"
                        : $"WHERE (ChannelId IN ({TranslateUtils.ToSqlInStringWithoutQuote(channelIdList)}) AND IsChecked = '{true}' {whereString})", orderString,
                    totalNum);
            }
            else
            {
                sqlString = channelIdList.Count == 1
                    ? $"SELECT Id FROM {tableName} WHERE (ChannelId = {channelIdList[0]} AND IsChecked = '{true}' {whereString}) {orderString}"
                    : $"SELECT Id FROM {tableName} WHERE (ChannelId IN ({TranslateUtils.ToSqlInStringWithoutQuote(channelIdList)}) AND IsChecked = '{true}' {whereString}) {orderString}";
            }

            using (var rdr = DatabaseApi.ExecuteReader(ConnectionString, sqlString))
            {
                while (rdr.Read())
                {
                    var contentId = DatabaseApi.GetInt(rdr, 0);
                    list.Add(contentId);
                }
                rdr.Close();
            }
            return list;
        }

        private List<int> GetContentIdListChecked(string tableName, int channelId, string orderByFormatString, string whereString)
        {
            return GetContentIdListChecked(tableName, channelId, 0, orderByFormatString, whereString);
        }

        public IList<(int, int)> ApiGetContentIdListBySiteId(string tableName, int siteId, ApiContentsParameters parameters, out int totalCount)
        {
            totalCount = 0;
            var retVal = new List<(int, int)>();

            var whereString = $"WHERE {Attr.SiteId} = {siteId} AND {Attr.ChannelId} > 0 AND {Attr.IsChecked} = '{true}'";
            if (parameters.ChannelIds.Count > 0)
            {
                whereString += $" AND {Attr.ChannelId} IN ({TranslateUtils.ObjectCollectionToString(parameters.ChannelIds)})";
            }
            if (!string.IsNullOrEmpty(parameters.ChannelGroup))
            {
                var channelIdList = ChannelManager.GetChannelIdList(siteId, parameters.ChannelGroup);
                if (channelIdList.Count > 0)
                {
                    whereString += $" AND {Attr.ChannelId} IN ({TranslateUtils.ObjectCollectionToString(channelIdList)})";
                }
            }
            if (!string.IsNullOrEmpty(parameters.ContentGroup))
            {
                whereString += $" AND ({Attr.GroupNameCollection} = '{parameters.ContentGroup}' OR {SqlUtils.GetInStr(Attr.GroupNameCollection, parameters.ContentGroup + ",")} OR {SqlUtils.GetInStr(Attr.GroupNameCollection, "," + parameters.ContentGroup + ",")} OR {SqlUtils.GetInStr(Attr.GroupNameCollection, "," + parameters.ContentGroup)})";
            }
            if (!string.IsNullOrEmpty(parameters.Tag))
            {
                whereString += $" AND ({Attr.Tags} = '{parameters.Tag}' OR {SqlUtils.GetInStr(Attr.Tags, parameters.Tag + ",")} OR {SqlUtils.GetInStr(Attr.Tags, "," + parameters.Tag + ",")} OR {SqlUtils.GetInStr(Attr.Tags, "," + parameters.Tag)})";
            }

            var channelInfo = ChannelManager.GetChannelInfo(siteId, siteId);
            var orderString = GetOrderString(channelInfo, AttackUtils.FilterSql(parameters.OrderBy));
            var dbArgs = new Dictionary<string, object>();

            if (parameters.QueryString != null && parameters.QueryString.Count > 0)
            {
                var columnNameList = TableColumnManager.GetTableColumnNameList(tableName);

                foreach (var attributeName in parameters.QueryString.AllKeys)
                {
                    if (!StringUtils.ContainsIgnoreCase(columnNameList, attributeName)) continue;

                    var value = parameters.QueryString[attributeName];

                    if (StringUtils.EqualsIgnoreCase(attributeName, Attr.IsChecked) || StringUtils.EqualsIgnoreCase(attributeName, Attr.IsColor) || StringUtils.EqualsIgnoreCase(attributeName, Attr.IsHot) || StringUtils.EqualsIgnoreCase(attributeName, Attr.IsRecommend) || StringUtils.EqualsIgnoreCase(attributeName, Attr.IsTop))
                    {
                        whereString += $" AND {attributeName} = '{TranslateUtils.ToBool(value)}'";
                    }
                    else if (StringUtils.EqualsIgnoreCase(attributeName, Attr.Id) || StringUtils.EqualsIgnoreCase(attributeName, Attr.ReferenceId) || StringUtils.EqualsIgnoreCase(attributeName, Attr.SourceId) || StringUtils.EqualsIgnoreCase(attributeName, Attr.CheckedLevel))
                    {
                        whereString += $" AND {attributeName} = {TranslateUtils.ToInt(value)}";
                    }
                    else if (parameters.Likes.Contains(attributeName))
                    {
                        whereString += $" AND {attributeName} LIKE '%{AttackUtils.FilterSql(value)}%'";
                    }
                    else
                    {
                        whereString += $" AND {attributeName} = @{attributeName}";
                        dbArgs.Add(attributeName, value);
                    }
                }
            }

            totalCount = DatabaseApi.GetPageTotalCount(tableName, whereString, dbArgs);
            if (totalCount > 0 && parameters.Skip < totalCount)
            {
                var sqlString = SqlDifferences.GetSqlString(tableName, MinListColumns, whereString, orderString, parameters.Skip, parameters.Top);

                using (var connection = GetConnection())
                {
                    retVal = connection.Query<ContentInfo>(sqlString, dbArgs).Select(o => (o.ChannelId, o.Id)).ToList();
                }
            }

            return retVal;
        }

        public IList<(int, int)> ApiGetContentIdListByChannelId(string tableName, int siteId, int channelId, int top, int skip, string like, string orderBy, NameValueCollection queryString, out int totalCount)
        {
            var retVal = new List<(int, int)>();

            var channelInfo = ChannelManager.GetChannelInfo(siteId, channelId);
            var channelIdList = ChannelManager.GetChannelIdList(channelInfo, EScopeType.All, string.Empty, string.Empty, string.Empty);
            var whereString = $"WHERE {Attr.SiteId} = {siteId} AND {Attr.ChannelId} IN ({TranslateUtils.ToSqlInStringWithoutQuote(channelIdList)}) AND {Attr.IsChecked} = '{true}'";

            var likeList = TranslateUtils.StringCollectionToStringList(StringUtils.TrimAndToLower(like));
            var orderString = GetOrderString(channelInfo, AttackUtils.FilterSql(orderBy));
            var dbArgs = new Dictionary<string, object>();

            if (queryString != null && queryString.Count > 0)
            {
                var columnNameList = TableColumnManager.GetTableColumnNameList(tableName);

                foreach (string attributeName in queryString)
                {
                    if (!StringUtils.ContainsIgnoreCase(columnNameList, attributeName)) continue;

                    var value = queryString[attributeName];

                    if (StringUtils.EqualsIgnoreCase(attributeName, Attr.IsChecked) || StringUtils.EqualsIgnoreCase(attributeName, Attr.IsColor) || StringUtils.EqualsIgnoreCase(attributeName, Attr.IsHot) || StringUtils.EqualsIgnoreCase(attributeName, Attr.IsRecommend) || StringUtils.EqualsIgnoreCase(attributeName, Attr.IsTop))
                    {
                        whereString += $" AND {attributeName} = '{TranslateUtils.ToBool(value)}'";
                    }
                    else if (StringUtils.EqualsIgnoreCase(attributeName, Attr.Id) || StringUtils.EqualsIgnoreCase(attributeName, Attr.ReferenceId) || StringUtils.EqualsIgnoreCase(attributeName, Attr.SourceId) || StringUtils.EqualsIgnoreCase(attributeName, Attr.CheckedLevel))
                    {
                        whereString += $" AND {attributeName} = {TranslateUtils.ToInt(value)}";
                    }
                    else if (likeList.Contains(attributeName))
                    {
                        whereString += $" AND {attributeName} LIKE '%{AttackUtils.FilterSql(value)}%'";
                    }
                    else
                    {
                        whereString += $" AND {attributeName} = @{attributeName}";
                        dbArgs.Add(attributeName, value);
                    }
                }
            }

            totalCount = DatabaseApi.GetPageTotalCount(tableName, whereString, dbArgs);
            if (totalCount > 0 && skip < totalCount)
            {
                var sqlString = SqlDifferences.GetSqlString(tableName, MinListColumns, whereString, orderString, skip, top);

                using (var connection = GetConnection())
                {
                    retVal = connection.Query<ContentInfo>(sqlString, dbArgs).Select(o => (o.ChannelId, o.Id)).ToList();
                }
            }

            return retVal;
        }

        #region Table

        public string GetSelectCommandByHitsAnalysis(string tableName, int siteId)
        {
            var orderByString = ETaxisTypeUtils.GetContentOrderByString(ETaxisType.OrderByTaxisDesc);

            var whereString = new StringBuilder();
            whereString.Append($"AND IsChecked = '{true}' AND SiteId = {siteId} AND Hits > 0");
            whereString.Append(orderByString);

            return DatabaseApi.GetSelectSqlString(tableName, whereString.ToString());
        }

        public string GetSqlStringOfAdminExcludeRecycle(string tableName, int siteId, DateTime begin, DateTime end)
        {
            var sqlString = $@"select userName,SUM(addCount) as addCount, SUM(updateCount) as updateCount from( 
SELECT AddUserName as userName, Count(AddUserName) as addCount, 0 as updateCount FROM {tableName} 
INNER JOIN {DataProvider.Administrator.TableName} ON AddUserName = {DataProvider.Administrator.TableName}.UserName 
WHERE {tableName}.SiteId = {siteId} AND (({tableName}.ChannelId > 0)) 
AND LastEditDate BETWEEN {SqlUtils.GetComparableDate(begin)} AND {SqlUtils.GetComparableDate(end.AddDays(1))}
GROUP BY AddUserName
Union
SELECT LastEditUserName as userName,0 as addCount, Count(LastEditUserName) as updateCount FROM {tableName} 
INNER JOIN {DataProvider.Administrator.TableName} ON LastEditUserName = {DataProvider.Administrator.TableName}.UserName 
WHERE {tableName}.SiteId = {siteId} AND (({tableName}.ChannelId > 0)) 
AND LastEditDate BETWEEN {SqlUtils.GetComparableDate(begin)} AND {SqlUtils.GetComparableDate(end.AddDays(1))}
AND LastEditDate != AddDate
GROUP BY LastEditUserName
) as tmp
group by tmp.userName";


            return sqlString;
        }

        public string GetStlWhereString(int siteId, string group, string groupNot, string tags, bool isTopExists, bool isTop, string where)
        {
            var whereStringBuilder = new StringBuilder();

            if (isTopExists)
            {
                whereStringBuilder.Append($" AND IsTop = '{isTop}' ");
            }

            if (!string.IsNullOrEmpty(group))
            {
                group = group.Trim().Trim(',');
                var groupArr = group.Split(',');
                if (groupArr.Length > 0)
                {
                    whereStringBuilder.Append(" AND (");
                    foreach (var theGroup in groupArr)
                    {
                        whereStringBuilder.Append(
                                $" ({Attr.GroupNameCollection} = '{theGroup.Trim()}' OR {SqlUtils.GetInStr(Attr.GroupNameCollection, theGroup.Trim() + ",")} OR {SqlUtils.GetInStr(Attr.GroupNameCollection, "," + theGroup.Trim() + ",")} OR {SqlUtils.GetInStr(Attr.GroupNameCollection, "," + theGroup.Trim())}) OR ");
                    }
                    if (groupArr.Length > 0)
                    {
                        whereStringBuilder.Length = whereStringBuilder.Length - 3;
                    }
                    whereStringBuilder.Append(") ");
                }
            }

            if (!string.IsNullOrEmpty(groupNot))
            {
                groupNot = groupNot.Trim().Trim(',');
                var groupNotArr = groupNot.Split(',');
                if (groupNotArr.Length > 0)
                {
                    whereStringBuilder.Append(" AND (");
                    foreach (var theGroupNot in groupNotArr)
                    {
                        whereStringBuilder.Append(
                                $" ({Attr.GroupNameCollection} <> '{theGroupNot.Trim()}' AND {SqlUtils.GetNotInStr(Attr.GroupNameCollection, theGroupNot.Trim() + ",")} AND {SqlUtils.GetNotInStr(Attr.GroupNameCollection, "," + theGroupNot.Trim() + ",")} AND {SqlUtils.GetNotInStr(Attr.GroupNameCollection, "," + theGroupNot.Trim())}) AND ");
                    }
                    if (groupNotArr.Length > 0)
                    {
                        whereStringBuilder.Length = whereStringBuilder.Length - 4;
                    }
                    whereStringBuilder.Append(") ");
                }
            }

            if (!string.IsNullOrEmpty(tags))
            {
                var tagCollection = TagUtils.ParseTagsString(tags);
                var contentIdList = DataProvider.Tag.GetContentIdListByTagCollection(tagCollection, siteId);
                if (contentIdList.Count > 0)
                {
                    var inString = TranslateUtils.ToSqlInStringWithoutQuote(contentIdList.ToList());
                    whereStringBuilder.Append($" AND (Id IN ({inString}))");
                }
            }

            if (!string.IsNullOrEmpty(where))
            {
                whereStringBuilder.Append($" AND ({where}) ");
            }

            return whereStringBuilder.ToString();
        }

        public string GetWhereStringByStlSearch(bool isAllSites, string siteName, string siteDir, string siteIds, string channelIndex, string channelName, string channelIds, string type, string word, string dateAttribute, string dateFrom, string dateTo, string since, int siteId, List<string> excludeAttributes, NameValueCollection form)
        {
            var whereBuilder = new StringBuilder();

            SiteInfo siteInfo = null;
            if (!string.IsNullOrEmpty(siteName))
            {
                siteInfo = SiteManager.GetSiteInfoBySiteName(siteName);
            }
            else if (!string.IsNullOrEmpty(siteDir))
            {
                siteInfo = SiteManager.GetSiteInfoByDirectory(siteDir);
            }
            if (siteInfo == null)
            {
                siteInfo = SiteManager.GetSiteInfo(siteId);
            }

            var channelId = ChannelManager.GetChannelId(siteId, siteId, channelIndex, channelName);
            var channelInfo = ChannelManager.GetChannelInfo(siteId, channelId);

            if (isAllSites)
            {
                whereBuilder.Append("(SiteId > 0) ");
            }
            else if (!string.IsNullOrEmpty(siteIds))
            {
                whereBuilder.Append($"(SiteId IN ({TranslateUtils.ToSqlInStringWithoutQuote(TranslateUtils.StringCollectionToIntList(siteIds))})) ");
            }
            else
            {
                whereBuilder.Append($"(SiteId = {siteInfo.Id}) ");
            }

            if (!string.IsNullOrEmpty(channelIds))
            {
                whereBuilder.Append(" AND ");
                var channelIdList = new List<int>();
                foreach (var theChannelId in TranslateUtils.StringCollectionToIntList(channelIds))
                {
                    var theSiteId = DataProvider.Channel.GetSiteId(theChannelId);
                    channelIdList.AddRange(
                        ChannelManager.GetChannelIdList(ChannelManager.GetChannelInfo(theSiteId, theChannelId),
                            EScopeType.All, string.Empty, string.Empty, string.Empty));
                }
                whereBuilder.Append(channelIdList.Count == 1
                    ? $"(ChannelId = {channelIdList[0]}) "
                    : $"(ChannelId IN ({TranslateUtils.ToSqlInStringWithoutQuote(channelIdList)})) ");
            }
            else if (channelId != siteId)
            {
                whereBuilder.Append(" AND ");

                var theSiteId = DataProvider.Channel.GetSiteId(channelId);
                var channelIdList = ChannelManager.GetChannelIdList(ChannelManager.GetChannelInfo(theSiteId, channelId),
                            EScopeType.All, string.Empty, string.Empty, string.Empty);

                whereBuilder.Append(channelIdList.Count == 1
                    ? $"(ChannelId = {channelIdList[0]}) "
                    : $"(ChannelId IN ({TranslateUtils.ToSqlInStringWithoutQuote(channelIdList)})) ");
            }

            var typeList = new List<string>();
            if (string.IsNullOrEmpty(type))
            {
                typeList.Add(Attr.Title);
            }
            else
            {
                typeList = TranslateUtils.StringCollectionToStringList(type);
            }

            if (!string.IsNullOrEmpty(word))
            {
                whereBuilder.Append(" AND (");
                foreach (var attributeName in typeList)
                {
                    whereBuilder.Append($"[{attributeName}] LIKE '%{AttackUtils.FilterSql(word)}%' OR ");
                }
                whereBuilder.Length = whereBuilder.Length - 3;
                whereBuilder.Append(")");
            }

            if (string.IsNullOrEmpty(dateAttribute))
            {
                dateAttribute = Attr.AddDate;
            }

            if (!string.IsNullOrEmpty(dateFrom))
            {
                whereBuilder.Append(" AND ");
                whereBuilder.Append($" {dateAttribute} >= {SqlUtils.GetComparableDate(TranslateUtils.ToDateTime(dateFrom))} ");
            }
            if (!string.IsNullOrEmpty(dateTo))
            {
                whereBuilder.Append(" AND ");
                whereBuilder.Append($" {dateAttribute} <= {SqlUtils.GetComparableDate(TranslateUtils.ToDateTime(dateTo))} ");
            }
            if (!string.IsNullOrEmpty(since))
            {
                var sinceDate = DateTime.Now.AddHours(-DateUtils.GetSinceHours(since));
                whereBuilder.Append($" AND {dateAttribute} BETWEEN {SqlUtils.GetComparableDateTime(sinceDate)} AND {SqlUtils.GetComparableNow()} ");
            }

            var tableName = ChannelManager.GetTableName(siteInfo, channelInfo);
            //var styleInfoList = RelatedIdentities.GetTableStyleInfoList(siteInfo, channelInfo.Id);

            foreach (string key in form.Keys)
            {
                if (excludeAttributes.Contains(key.ToLower())) continue;
                if (string.IsNullOrEmpty(form[key])) continue;

                var value = StringUtils.Trim(form[key]);
                if (string.IsNullOrEmpty(value)) continue;

                var columnInfo = TableColumnManager.GetTableColumnInfo(tableName, key);

                if (columnInfo != null && (columnInfo.DataType == DataType.VarChar || columnInfo.DataType == DataType.Text))
                {
                    whereBuilder.Append(" AND ");
                    whereBuilder.Append($"({key} LIKE '%{value}%')");
                }
                //else
                //{
                //    foreach (var tableStyleInfo in styleInfoList)
                //    {
                //        if (StringUtils.EqualsIgnoreCase(tableStyleInfo.AttributeName, key))
                //        {
                //            whereBuilder.Append(" AND ");
                //            whereBuilder.Append($"({Attr.SettingsXml} LIKE '%{key}={value}%')");
                //            break;
                //        }
                //    }
                //}
            }

            return whereBuilder.ToString();
        }

        public string GetSqlString(string tableName, int siteId, int channelId, bool isSystemAdministrator, List<int> owningChannelIdList, string searchType, string keyword, string dateFrom, string dateTo, bool isSearchChildren, ETriState checkedState, bool isTrashContent)
        {
            var channelInfo = ChannelManager.GetChannelInfo(siteId, channelId);
            var channelIdList = ChannelManager.GetChannelIdList(channelInfo,
                isSearchChildren ? EScopeType.All : EScopeType.Self, string.Empty, string.Empty, channelInfo.ContentModelPluginId);

            var list = new List<int>();
            if (isSystemAdministrator)
            {
                list = channelIdList;
            }
            else
            {
                foreach (var theChannelId in channelIdList)
                {
                    if (owningChannelIdList.Contains(theChannelId))
                    {
                        list.Add(theChannelId);
                    }
                }
            }

            return GetSqlStringByCondition(tableName, siteId, list, searchType, keyword, dateFrom, dateTo, checkedState, isTrashContent);
        }

        public string GetSqlStringByContentGroup(string tableName, string contentGroupName, int siteId)
        {
            contentGroupName = AttackUtils.FilterSql(contentGroupName);
            var sqlString =
                $"SELECT * FROM {tableName} WHERE SiteId = {siteId} AND ChannelId > 0 AND (GroupNameCollection LIKE '{contentGroupName},%' OR GroupNameCollection LIKE '%,{contentGroupName}' OR GroupNameCollection  LIKE '%,{contentGroupName},%'  OR GroupNameCollection='{contentGroupName}')";
            return sqlString;
        }

        public string GetStlSqlStringChecked(List<int> channelIdList, string tableName, int siteId, int channelId, int startNum, int totalNum, string orderByString, string whereString, EScopeType scopeType, string groupChannel, string groupChannelNot)
        {
            string sqlWhereString;

            if (siteId == channelId && scopeType == EScopeType.All && string.IsNullOrEmpty(groupChannel) && string.IsNullOrEmpty(groupChannelNot))
            {
                sqlWhereString =
                    $"WHERE (SiteId = {siteId} AND ChannelId > 0 AND IsChecked = '{true}' {whereString})";
            }
            else
            {
                if (channelIdList == null || channelIdList.Count == 0)
                {
                    return string.Empty;
                }
                sqlWhereString = channelIdList.Count == 1 ? $"WHERE (ChannelId = {channelIdList[0]} AND IsChecked = '{true}' {whereString})" : $"WHERE (ChannelId IN ({TranslateUtils.ToSqlInStringWithoutQuote(channelIdList)}) AND IsChecked = '{true}' {whereString})";
            }

            if (!string.IsNullOrEmpty(tableName))
            {
                //return DatabaseApi.GetSelectSqlString(tableName, startNum, totalNum, MinListColumns, sqlWhereString, orderByString);
                return SqlDifferences.GetSqlString(tableName, MinListColumns, sqlWhereString, orderByString, startNum - 1, totalNum);
            }
            return string.Empty;
        }

        public string GetStlSqlStringCheckedBySearch(string tableName, int startNum, int totalNum, string orderByString, string whereString)
        {
            var sqlWhereString =
                    $"WHERE (ChannelId > 0 AND IsChecked = '{true}' {whereString})";

            if (!string.IsNullOrEmpty(tableName))
            {
                //return DatabaseApi.GetSelectSqlString(tableName, startNum, totalNum, TranslateUtils.ObjectCollectionToString(Attr.AllAttributes.Value), sqlWhereString, orderByString);
                return SqlDifferences.GetSqlString(tableName, Attr.AllAttributes.Value, sqlWhereString, orderByString, startNum - 1, totalNum);
            }
            return string.Empty;
        }

        public string GetStlWhereString(int siteId, string group, string groupNot, string tags, bool isImageExists, bool isImage, bool isVideoExists, bool isVideo, bool isFileExists, bool isFile, bool isTopExists, bool isTop, bool isRecommendExists, bool isRecommend, bool isHotExists, bool isHot, bool isColorExists, bool isColor, string where)
        {
            var whereBuilder = new StringBuilder();
            whereBuilder.Append($" AND SiteId = {siteId} ");

            if (isImageExists)
            {
                whereBuilder.Append(isImage
                    ? $" AND {ContentAttribute.ImageUrl} <> '' "
                    : $" AND {ContentAttribute.ImageUrl} = '' ");
            }

            if (isVideoExists)
            {
                whereBuilder.Append(isVideo
                    ? $" AND {ContentAttribute.VideoUrl} <> '' "
                    : $" AND {ContentAttribute.VideoUrl} = '' ");
            }

            if (isFileExists)
            {
                whereBuilder.Append(isFile
                    ? $" AND {ContentAttribute.FileUrl} <> '' "
                    : $" AND {ContentAttribute.FileUrl} = '' ");
            }

            if (isTopExists)
            {
                whereBuilder.Append($" AND {Attr.IsTop} = '{isTop}' ");
            }

            if (isRecommendExists)
            {
                whereBuilder.Append($" AND {Attr.IsRecommend} = '{isRecommend}' ");
            }

            if (isHotExists)
            {
                whereBuilder.Append($" AND {Attr.IsHot} = '{isHot}' ");
            }

            if (isColorExists)
            {
                whereBuilder.Append($" AND {Attr.IsColor} = '{isColor}' ");
            }

            if (!string.IsNullOrEmpty(group))
            {
                group = group.Trim().Trim(',');
                var groupArr = group.Split(',');
                if (groupArr != null && groupArr.Length > 0)
                {
                    whereBuilder.Append(" AND (");
                    foreach (var theGroup in groupArr)
                    {
                        var trimGroup = theGroup.Trim();

                        whereBuilder.Append(
                                $" ({Attr.GroupNameCollection} = '{trimGroup}' OR {SqlUtils.GetInStr(Attr.GroupNameCollection, trimGroup + ",")} OR {SqlUtils.GetInStr(Attr.GroupNameCollection, "," + trimGroup + ",")} OR {SqlUtils.GetInStr(Attr.GroupNameCollection, "," + trimGroup)}) OR ");
                    }
                    if (groupArr.Length > 0)
                    {
                        whereBuilder.Length = whereBuilder.Length - 3;
                    }
                    whereBuilder.Append(") ");
                }
            }

            if (!string.IsNullOrEmpty(groupNot))
            {
                groupNot = groupNot.Trim().Trim(',');
                var groupNotArr = groupNot.Split(',');
                if (groupNotArr != null && groupNotArr.Length > 0)
                {
                    whereBuilder.Append(" AND (");
                    foreach (var theGroupNot in groupNotArr)
                    {
                        var trimGroup = theGroupNot.Trim();

                        whereBuilder.Append(
                                $" ({Attr.GroupNameCollection} <> '{trimGroup}' AND {SqlUtils.GetNotInStr(Attr.GroupNameCollection, trimGroup + ",")} AND {SqlUtils.GetNotInStr(Attr.GroupNameCollection, "," + trimGroup + ",")} AND {SqlUtils.GetNotInStr(Attr.GroupNameCollection, "," + trimGroup)}) AND ");
                    }
                    if (groupNotArr.Length > 0)
                    {
                        whereBuilder.Length = whereBuilder.Length - 4;
                    }
                    whereBuilder.Append(") ");
                }
            }

            if (!string.IsNullOrEmpty(tags))
            {
                var tagCollection = TagUtils.ParseTagsString(tags);
                var contentIdArrayList = DataProvider.Tag.GetContentIdListByTagCollection(tagCollection, siteId);
                if (contentIdArrayList.Count > 0)
                {
                    whereBuilder.Append(
                        $" AND (ID IN ({TranslateUtils.ToSqlInStringWithoutQuote(contentIdArrayList.ToList())}))");
                }
            }

            if (!string.IsNullOrEmpty(where))
            {
                whereBuilder.Append($" AND ({where}) ");
            }

            return whereBuilder.ToString();
        }

        public string GetStlWhereStringBySearch(string group, string groupNot, bool isImageExists, bool isImage, bool isVideoExists, bool isVideo, bool isFileExists, bool isFile, bool isTopExists, bool isTop, bool isRecommendExists, bool isRecommend, bool isHotExists, bool isHot, bool isColorExists, bool isColor, string where)
        {
            var whereBuilder = new StringBuilder();

            if (isImageExists)
            {
                whereBuilder.Append(isImage
                    ? $" AND {ContentAttribute.ImageUrl} <> '' "
                    : $" AND {ContentAttribute.ImageUrl} = '' ");
            }

            if (isVideoExists)
            {
                whereBuilder.Append(isVideo
                    ? $" AND {ContentAttribute.VideoUrl} <> '' "
                    : $" AND {ContentAttribute.VideoUrl} = '' ");
            }

            if (isFileExists)
            {
                whereBuilder.Append(isFile
                    ? $" AND {ContentAttribute.FileUrl} <> '' "
                    : $" AND {ContentAttribute.FileUrl} = '' ");
            }

            if (isTopExists)
            {
                whereBuilder.Append($" AND {Attr.IsTop} = '{isTop}' ");
            }

            if (isRecommendExists)
            {
                whereBuilder.Append($" AND {Attr.IsRecommend} = '{isRecommend}' ");
            }

            if (isHotExists)
            {
                whereBuilder.Append($" AND {Attr.IsHot} = '{isHot}' ");
            }

            if (isColorExists)
            {
                whereBuilder.Append($" AND {Attr.IsColor} = '{isColor}' ");
            }

            if (!string.IsNullOrEmpty(group))
            {
                group = group.Trim().Trim(',');
                var groupArr = group.Split(',');
                if (groupArr != null && groupArr.Length > 0)
                {
                    whereBuilder.Append(" AND (");
                    foreach (var theGroup in groupArr)
                    {
                        var trimGroup = theGroup.Trim();

                        whereBuilder.Append(
                                $" ({Attr.GroupNameCollection} = '{trimGroup}' OR {SqlUtils.GetInStr(Attr.GroupNameCollection, trimGroup + ",")} OR {SqlUtils.GetInStr(Attr.GroupNameCollection, "," + trimGroup + ",")} OR {SqlUtils.GetInStr(Attr.GroupNameCollection, "," + trimGroup)}) OR ");
                    }
                    if (groupArr.Length > 0)
                    {
                        whereBuilder.Length = whereBuilder.Length - 3;
                    }
                    whereBuilder.Append(") ");
                }
            }

            if (!string.IsNullOrEmpty(groupNot))
            {
                groupNot = groupNot.Trim().Trim(',');
                var groupNotArr = groupNot.Split(',');
                if (groupNotArr != null && groupNotArr.Length > 0)
                {
                    whereBuilder.Append(" AND (");
                    foreach (var theGroupNot in groupNotArr)
                    {
                        var trimGroup = theGroupNot.Trim();

                        whereBuilder.Append(
                                $" ({Attr.GroupNameCollection} <> '{trimGroup}' AND {SqlUtils.GetNotInStr(Attr.GroupNameCollection, trimGroup + ",")} AND {SqlUtils.GetNotInStr(Attr.GroupNameCollection, "," + trimGroup + ",")} AND {SqlUtils.GetNotInStr(Attr.GroupNameCollection, "," + trimGroup)}) AND ");
                    }
                    if (groupNotArr.Length > 0)
                    {
                        whereBuilder.Length = whereBuilder.Length - 4;
                    }
                    whereBuilder.Append(") ");
                }
            }

            if (!string.IsNullOrEmpty(where))
            {
                whereBuilder.Append($" AND ({where}) ");
            }

            return whereBuilder.ToString();
        }

        private string GetSqlStringByCondition(string tableName, int siteId, List<int> channelIdList, string searchType, string keyword, string dateFrom, string dateTo, ETriState checkedState, bool isTrashContent)
        {
            return GetSqlStringByCondition(tableName, siteId, channelIdList, searchType, keyword, dateFrom, dateTo, checkedState, isTrashContent, false, string.Empty);
        }

        private string GetSqlStringByCondition(string tableName, int siteId, List<int> channelIdList, string searchType, string keyword, string dateFrom, string dateTo, ETriState checkedState, bool isTrashContent, bool isWritingOnly, string userNameOnly)
        {
            if (channelIdList == null || channelIdList.Count == 0)
            {
                return null;
            }

            var orderByString = ETaxisTypeUtils.GetContentOrderByString(ETaxisType.OrderByTaxisDesc);

            var dateString = string.Empty;
            if (!string.IsNullOrEmpty(dateFrom))
            {
                dateString = $" AND AddDate >= {SqlUtils.GetComparableDate(TranslateUtils.ToDateTime(dateFrom))} ";
            }
            if (!string.IsNullOrEmpty(dateTo))
            {
                dateString += $" AND AddDate <= {SqlUtils.GetComparableDate(TranslateUtils.ToDateTime(dateTo).AddDays(1))} ";
            }
            var whereString = new StringBuilder($"WHERE {nameof(Attr.SourceId)} != {SourceManager.Preview} AND ");

            if (isTrashContent)
            {
                for (var i = 0; i < channelIdList.Count; i++)
                {
                    var theChannelId = channelIdList[i];
                    channelIdList[i] = -theChannelId;
                }
            }

            whereString.Append(channelIdList.Count == 1
                ? $"SiteId = {siteId} AND (ChannelId = {channelIdList[0]}) "
                : $"SiteId = {siteId} AND (ChannelId IN ({TranslateUtils.ToSqlInStringWithoutQuote(channelIdList)})) ");

            if (StringUtils.EqualsIgnoreCase(searchType, Attr.IsTop) || StringUtils.EqualsIgnoreCase(searchType, Attr.IsRecommend) || StringUtils.EqualsIgnoreCase(searchType, Attr.IsColor) || StringUtils.EqualsIgnoreCase(searchType, Attr.IsHot))
            {
                if (!string.IsNullOrEmpty(keyword))
                {
                    whereString.Append($"AND ({Attr.Title} LIKE '%{keyword}%') ");
                }
                whereString.Append($" AND {searchType} = '{true}'");
            }
            else if (!string.IsNullOrEmpty(keyword))
            {
                var columnNameList = TableColumnManager.GetTableColumnNameList(tableName);

                if (StringUtils.ContainsIgnoreCase(columnNameList, searchType))
                {
                    whereString.Append($"AND ({searchType} LIKE '%{keyword}%') ");
                }
            }

            whereString.Append(dateString);

            if (checkedState == ETriState.True)
            {
                whereString.Append("AND IsChecked='True' ");
            }
            else if (checkedState == ETriState.False)
            {
                whereString.Append("AND IsChecked='False' ");
            }

            if (!string.IsNullOrEmpty(userNameOnly))
            {
                whereString.Append($" AND {Attr.AddUserName} = '{userNameOnly}' ");
            }
            if (isWritingOnly)
            {
                whereString.Append($" AND {Attr.UserId} > 0 ");
            }

            whereString.Append(" ").Append(orderByString);

            return DatabaseApi.GetSelectSqlString(tableName, whereString.ToString());
        }

        public string GetPagerWhereSqlString(SiteInfo siteInfo, ChannelInfo channelInfo, string searchType, string keyword, string dateFrom, string dateTo, int checkLevel, bool isCheckOnly, bool isSelfOnly, bool isTrashOnly, bool isWritingOnly, int? onlyAdminId, PermissionsImpl adminPermissions, List<string> allAttributeNameList)
        {
            var isAllChannels = false;
            var searchChannelIdList = new List<int>();

            if (isSelfOnly)
            {
                searchChannelIdList = new List<int>
                {
                    channelInfo.Id
                };
            }
            else
            {
                var channelIdList = ChannelManager.GetChannelIdList(channelInfo, EScopeType.All, string.Empty, string.Empty, channelInfo.ContentModelPluginId);

                if (adminPermissions.IsSystemAdministrator)
                {
                    if (channelInfo.Id == siteInfo.Id)
                    {
                        isAllChannels = true;
                    }

                    searchChannelIdList = channelIdList;
                }
                else
                {
                    foreach (var theChannelId in channelIdList)
                    {
                        if (adminPermissions.ChannelIdList.Contains(theChannelId))
                        {
                            searchChannelIdList.Add(theChannelId);
                        }
                    }
                }
            }
            if (isTrashOnly)
            {
                searchChannelIdList = searchChannelIdList.Select(i => -i).ToList();
            }

            var whereList = new List<string>
            {
                $"{nameof(Attr.SiteId)} = {siteInfo.Id}",
                $"{nameof(Attr.SourceId)} != {SourceManager.Preview}"
            };

            if (!string.IsNullOrEmpty(dateFrom))
            {
                whereList.Add($"{nameof(Attr.AddDate)} >= {SqlUtils.GetComparableDate(TranslateUtils.ToDateTime(dateFrom))}");
            }
            if (!string.IsNullOrEmpty(dateTo))
            {
                whereList.Add($"{nameof(Attr.AddDate)} <= {SqlUtils.GetComparableDate(TranslateUtils.ToDateTime(dateTo).AddDays(1))}");
            }

            if (isAllChannels)
            {
                whereList.Add(isTrashOnly
                    ? $"{nameof(Attr.ChannelId)} < 0"
                    : $"{nameof(Attr.ChannelId)} > 0");
            }
            else if (searchChannelIdList.Count == 0)
            {
                whereList.Add($"{nameof(Attr.ChannelId)} = 0");
            }
            else if (searchChannelIdList.Count == 1)
            {
                whereList.Add($"{nameof(Attr.ChannelId)} = {channelInfo.Id}");
            }
            else
            {
                whereList.Add($"{nameof(Attr.ChannelId)} IN ({TranslateUtils.ToSqlInStringWithoutQuote(searchChannelIdList)})");
            }

            if (StringUtils.EqualsIgnoreCase(searchType, Attr.IsTop) || StringUtils.EqualsIgnoreCase(searchType, Attr.IsRecommend) || StringUtils.EqualsIgnoreCase(searchType, Attr.IsColor) || StringUtils.EqualsIgnoreCase(searchType, Attr.IsHot))
            {
                if (!string.IsNullOrEmpty(keyword))
                {
                    whereList.Add($"{Attr.Title} LIKE '%{keyword}%'");
                }
                whereList.Add($"{searchType} = '{true}'");
            }
            else if (!string.IsNullOrEmpty(keyword))
            {
                if (StringUtils.ContainsIgnoreCase(allAttributeNameList, searchType))
                {
                    whereList.Add($"{searchType} LIKE '%{keyword}%'");
                }
                //whereList.Add(allLowerAttributeNameList.Contains(searchType.ToLower())
                //    ? $"{searchType} LIKE '%{keyword}%'"
                //    : $"{nameof(Attr.SettingsXml)} LIKE '%{searchType}={keyword}%'");
            }

            if (isCheckOnly)
            {
                whereList.Add(checkLevel == CheckManager.LevelInt.All
                    ? $"{nameof(Attr.IsChecked)} = '{false}'"
                    : $"{nameof(Attr.IsChecked)} = '{false}' AND {nameof(Attr.CheckedLevel)} = {checkLevel}");
            }
            else
            {
                if (checkLevel != CheckManager.LevelInt.All)
                {
                    whereList.Add(checkLevel == siteInfo.CheckContentLevel
                        ? $"{nameof(Attr.IsChecked)} = '{true}'"
                        : $"{nameof(Attr.IsChecked)} = '{false}' AND {nameof(Attr.CheckedLevel)} = {checkLevel}");
                }
            }

            if (onlyAdminId.HasValue)
            {
                whereList.Add($"{nameof(Attr.AdminId)} = {onlyAdminId.Value}");
            }

            if (isWritingOnly)
            {
                whereList.Add($"{nameof(Attr.UserId)} > 0");
            }

            return $"WHERE {string.Join(" AND ", whereList)}";
        }

        public string GetPagerWhereSqlString(int channelId, ETriState checkedState, string userNameOnly)
        {
            var whereString = new StringBuilder();
            whereString.Append($"WHERE {nameof(Attr.ChannelId)} = {channelId} AND {nameof(Attr.SourceId)} != {SourceManager.Preview} ");

            if (checkedState == ETriState.True)
            {
                whereString.Append($"AND IsChecked='{true}' ");
            }
            else if (checkedState == ETriState.False)
            {
                whereString.Append($"AND IsChecked='{false}'");
            }

            if (!string.IsNullOrEmpty(userNameOnly))
            {
                whereString.Append($" AND AddUserName = '{userNameOnly}' ");
            }

            return whereString.ToString();
        }

        #endregion

        #region Cache

        public string GetCacheWhereString(SiteInfo siteInfo, ChannelInfo channelInfo, int? onlyAdminId)
        {
            var whereString = $"WHERE {Attr.SiteId} = {siteInfo.Id} AND {Attr.ChannelId} = {channelInfo.Id} AND {nameof(Attr.SourceId)} != {SourceManager.Preview}";
            if (onlyAdminId.HasValue)
            {
                whereString += $" AND {nameof(Attr.AdminId)} = {onlyAdminId.Value}";
            }

            return whereString;
        }

        public string GetOrderString(ChannelInfo channelInfo, string orderBy)
        {
            return ETaxisTypeUtils.GetContentOrderByString(ETaxisTypeUtils.GetEnumType(channelInfo.DefaultTaxisType), orderBy);
        }

        public List<ContentInfo> GetContentInfoList(string tableName, string whereString, string orderString, int offset, int limit)
        {
            var list = new List<ContentInfo>();

            var sqlString = SqlDifferences.GetSqlString(tableName, null, whereString, orderString, offset, limit);

            using (var rdr = DatabaseApi.ExecuteReader(ConnectionString, sqlString))
            {
                while (rdr.Read())
                {
                    var dict = TranslateUtils.ToDictionary(rdr);
                    if (dict != null)
                    {
                        var contentInfo = new ContentInfo(dict);
                        list.Add(contentInfo);
                    }
                }
                rdr.Close();
            }

            return list;
        }

        

        public List<int> GetCacheContentIdList(string tableName, string whereString, string orderString, int offset, int limit)
        {
            var list = new List<int>();

            var sqlString = SqlDifferences.GetSqlString(tableName, MinListColumns, whereString, orderString, offset, limit);

            using (var rdr = DatabaseApi.ExecuteReader(ConnectionString, sqlString))
            {
                while (rdr.Read())
                {
                    var contentId = DatabaseApi.GetInt(rdr, 0);

                    list.Add(contentId);
                }
                rdr.Close();
            }

            return list;
        }

        public ContentInfo GetCacheContentInfo(string tableName, int channelId, int contentId)
        {
            if (string.IsNullOrEmpty(tableName) || contentId <= 0) return null;

            ContentInfo contentInfo = null;

            var sqlWhere = $"WHERE {Attr.ChannelId} = {channelId} AND {Attr.Id} = {contentId}";
            var sqlSelect = DatabaseApi.GetSelectSqlString(tableName, sqlWhere);

            using (var rdr = DatabaseApi.ExecuteReader(ConnectionString, sqlSelect))
            {
                if (rdr.Read())
                {
                    var dict = TranslateUtils.ToDictionary(rdr);
                    if (dict != null)
                    {
                        contentInfo = new ContentInfo(dict);
                    }
                }
                rdr.Close();
            }

            return contentInfo;
        }

        #endregion
    }
}
