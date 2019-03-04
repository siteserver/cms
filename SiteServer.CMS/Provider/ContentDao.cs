using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Linq;
using System.Text;
using SiteServer.Utils;
using SiteServer.CMS.Core;
using SiteServer.CMS.Data;
using SiteServer.CMS.Model;
using SiteServer.CMS.Model.Attributes;
using SiteServer.Plugin;
using SiteServer.Utils.Enumerations;
using Dapper;
using SiteServer.CMS.Api.V1;
using SiteServer.CMS.DataCache;
using SiteServer.CMS.Plugin.Impl;
using SiteServer.CMS.DataCache.Content;

namespace SiteServer.CMS.Provider
{
    public class ContentDao : DataProviderBase
    {
        private const int TaxisIsTopStartValue = 2000000000;

        private static string MinListColumns { get; } = $"{ContentAttribute.Id}, {ContentAttribute.ChannelId}, {ContentAttribute.IsTop}, {ContentAttribute.AddDate}, {ContentAttribute.LastEditDate}, {ContentAttribute.Taxis}, {ContentAttribute.Hits}, {ContentAttribute.HitsByDay}, {ContentAttribute.HitsByWeek}, {ContentAttribute.HitsByMonth}";

        public static string GetContentTableName(int siteId)
        {
            return $"siteserver_Content_{siteId}";
        }

        public override List<TableColumn> TableColumns => new List<TableColumn>
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
                DataType = DataType.VarChar,
                DataLength = 255,
            },
            new TableColumn
            {
                AttributeName = nameof(ContentInfo.LastEditUserName),
                DataType = DataType.VarChar,
                DataLength = 255,
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
                DataType = DataType.VarChar,
                DataLength = 255
            },
            new TableColumn
            {
                AttributeName = nameof(ContentInfo.Tags),
                DataType = DataType.VarChar,
                DataLength = 255
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
                DataType = DataType.VarChar,
                DataLength = 18
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
                DataType = DataType.VarChar,
                DataLength = 255
            },
            new TableColumn
            {
                AttributeName = nameof(ContentInfo.IsTop),
                DataType = DataType.VarChar,
                DataLength = 18
            },
            new TableColumn
            {
                AttributeName = nameof(ContentInfo.IsRecommend),
                DataType = DataType.VarChar,
                DataLength = 18
            },
            new TableColumn
            {
                AttributeName = nameof(ContentInfo.IsHot),
                DataType = DataType.VarChar,
                DataLength = 18
            },
            new TableColumn
            {
                AttributeName = nameof(ContentInfo.IsColor),
                DataType = DataType.VarChar,
                DataLength = 18
            },
            new TableColumn
            {
                AttributeName = nameof(ContentInfo.LinkUrl),
                DataType = DataType.VarChar,
                DataLength = 200
            },
            new TableColumn
            {
                AttributeName = nameof(ContentInfo.AddDate),
                DataType = DataType.DateTime
            }
        };

        public List<TableColumn> TableColumnsDefault
        {
            get
            {
                var tableColumns = new List<TableColumn>();
                tableColumns.AddRange(TableColumns);
                tableColumns.Add(new TableColumn
                {
                    AttributeName = nameof(ContentInfo.SubTitle),
                    DataType = DataType.VarChar,
                    DataLength = 255
                });
                tableColumns.Add(new TableColumn
                {
                    AttributeName = nameof(ContentInfo.ImageUrl),
                    DataType = DataType.VarChar,
                    DataLength = 200
                });
                tableColumns.Add(new TableColumn
                {
                    AttributeName = nameof(ContentInfo.VideoUrl),
                    DataType = DataType.VarChar,
                    DataLength = 200
                });
                tableColumns.Add(new TableColumn
                {
                    AttributeName = nameof(ContentInfo.FileUrl),
                    DataType = DataType.VarChar,
                    DataLength = 200
                });
                tableColumns.Add(new TableColumn
                {
                    AttributeName = nameof(ContentInfo.Content),
                    DataType = DataType.Text
                });
                tableColumns.Add(new TableColumn
                {
                    AttributeName = nameof(ContentInfo.Summary),
                    DataType = DataType.Text
                });
                tableColumns.Add(new TableColumn
                {
                    AttributeName = nameof(ContentInfo.Author),
                    DataType = DataType.VarChar,
                    DataLength = 255
                });
                tableColumns.Add(new TableColumn
                {
                    AttributeName = nameof(ContentInfo.Source),
                    DataType = DataType.VarChar,
                    DataLength = 255
                });

                return tableColumns;
            }
        }

        public void Update(string tableName, int chananelId, int contentId, string name, string value)
        {
            var sqlString = $"UPDATE {tableName} SET {name} = @{name} WHERE Id = @Id";

            var parameters = new DynamicParameters();
            parameters.Add($"@{name}", value);
            parameters.Add("@Id", contentId);

            using (var connection = GetConnection())
            {
                connection.Execute(sqlString, parameters);
            }

            ContentManager.RemoveCache(tableName, chananelId);
        }

        public void UpdateIsChecked(string tableName, int siteId, int channelId, List<int> contentIdList, int translateChannelId, string userName, bool isChecked, int checkedLevel, string reasons)
        {
            if (isChecked)
            {
                checkedLevel = 0;
            }

            var checkDate = DateTime.Now;

            foreach (var contentId in contentIdList)
            {
                var tuple = GetValue(tableName, contentId, ContentAttribute.SettingsXml);
                if (tuple == null) continue;

                var attributes = new AttributesImpl(tuple.Item2);
                attributes.Set(ContentAttribute.CheckUserName, userName);
                attributes.Set(ContentAttribute.CheckDate, DateUtils.GetDateAndTimeString(checkDate));
                attributes.Set(ContentAttribute.CheckReasons, reasons);

                var sqlString =
                    $"UPDATE {tableName} SET {nameof(ContentInfo.IsChecked)} = '{isChecked}', {nameof(ContentInfo.CheckedLevel)} = {checkedLevel}, {nameof(ContentInfo.SettingsXml)} = '{attributes}' WHERE {nameof(ContentInfo.Id)} = {contentId}";
                if (translateChannelId > 0)
                {
                    sqlString =
                        $"UPDATE {tableName} SET {nameof(ContentInfo.IsChecked)} = '{isChecked}', {nameof(ContentInfo.CheckedLevel)} = {checkedLevel}, {nameof(ContentInfo.SettingsXml)} = '{attributes}', {nameof(ContentInfo.ChannelId)} = {translateChannelId} WHERE {nameof(ContentInfo.Id)} = {contentId}";
                }
                ExecuteNonQuery(sqlString);

                var checkInfo = new ContentCheckInfo(0, tableName, siteId, channelId, contentId, userName, isChecked, checkedLevel, checkDate, reasons);
                DataProvider.ContentCheckDao.Insert(checkInfo);
            }

            ContentManager.RemoveCache(tableName, channelId);
        }

        public void DeleteContentsByTrash(int siteId, int channelId, string tableName)
        {
            if (string.IsNullOrEmpty(tableName)) return;

            var contentIdList = GetContentIdListByTrash(siteId, tableName);
            TagUtils.RemoveTags(siteId, contentIdList);

            var sqlString =
                $"DELETE FROM {tableName} WHERE SiteId = {siteId} AND ChannelId < 0";
            ExecuteNonQuery(sqlString);

            ContentManager.RemoveCache(tableName, channelId);
        }

        public void SetAutoPageContentToSite(SiteInfo siteInfo)
        {
            if (!siteInfo.Additional.IsAutoPageInTextEditor) return;

            var sqlString =
                $"SELECT Id, {nameof(ContentInfo.ChannelId)}, {nameof(ContentInfo.Content)} FROM {siteInfo.TableName} WHERE SiteId = {siteInfo.Id}";

            using (var rdr = ExecuteReader(sqlString))
            {
                while (rdr.Read())
                {
                    var contentId = GetInt(rdr, 0);
                    var channelId = GetInt(rdr, 1);
                    var content = GetString(rdr, 2);

                    if (!string.IsNullOrEmpty(content))
                    {
                        content = ContentUtility.GetAutoPageContent(content, siteInfo.Additional.AutoPageWordNum);

                        Update(siteInfo.TableName, channelId, contentId, nameof(ContentInfo.Content), content);
                    }
                }

                rdr.Close();
            }
        }

        public void UpdateTrashContents(int siteId, int channelId, string tableName, List<int> contentIdList)
        {
            if (string.IsNullOrEmpty(tableName)) return;

            var referenceIdList = GetReferenceIdList(tableName, contentIdList);
            if (referenceIdList.Count > 0)
            {
                DeleteContents(siteId, channelId, tableName, referenceIdList);
            }

            var updateNum = 0;

            if (!string.IsNullOrEmpty(tableName) && contentIdList != null && contentIdList.Count > 0)
            {
                var sqlString =
                    $"UPDATE {tableName} SET ChannelId = -ChannelId, LastEditDate = {SqlUtils.GetComparableNow()} WHERE SiteId = {siteId} AND Id IN ({TranslateUtils.ToSqlInStringWithoutQuote(contentIdList)})";
                updateNum = ExecuteNonQuery(sqlString);
            }

            if (updateNum <= 0) return;

            ContentManager.RemoveCache(tableName, channelId);
        }

        public void UpdateTrashContentsByChannelId(int siteId, int channelId, string tableName)
        {
            if (string.IsNullOrEmpty(tableName)) return;

            var contentIdList = GetContentIdList(tableName, channelId);
            var referenceIdList = GetReferenceIdList(tableName, contentIdList);
            if (referenceIdList.Count > 0)
            {
                DeleteContents(siteId, channelId, tableName, referenceIdList);
            }
            var updateNum = 0;

            if (!string.IsNullOrEmpty(tableName))
            {
                var sqlString =
                    $"UPDATE {tableName} SET ChannelId = -ChannelId, LastEditDate = {SqlUtils.GetComparableNow()} WHERE SiteId = {siteId} AND ChannelId = {siteId}";
                updateNum = ExecuteNonQuery(sqlString);
            }

            if (updateNum <= 0) return;

            ContentManager.RemoveCache(tableName, channelId);
        }

        public void DeleteContent(string tableName, SiteInfo siteInfo, int channelId, int contentId)
        {
            if (string.IsNullOrEmpty(tableName)) return;

            if (!string.IsNullOrEmpty(tableName) && contentId > 0)
            {
                TagUtils.RemoveTags(siteInfo.Id, contentId);

                var sqlString =
                    $"DELETE FROM {tableName} WHERE SiteId = {siteInfo.Id} AND Id = {contentId}";
                ExecuteNonQuery(sqlString);
            }

            if (channelId <= 0) return;

            ContentManager.RemoveCache(tableName, channelId);
        }

        public void DeleteContents(int siteId, string tableName, List<int> contentIdList, int channelId)
        {
            if (string.IsNullOrEmpty(tableName)) return;

            var deleteNum = 0;

            if (!string.IsNullOrEmpty(tableName) && contentIdList != null && contentIdList.Count > 0)
            {
                TagUtils.RemoveTags(siteId, contentIdList);

                var sqlString =
                    $"DELETE FROM {tableName} WHERE SiteId = {siteId} AND Id IN ({TranslateUtils.ToSqlInStringWithoutQuote(contentIdList)})";
                deleteNum = ExecuteNonQuery(sqlString);
            }

            if (channelId <= 0 || deleteNum <= 0) return;

            ContentManager.RemoveCache(tableName, channelId);
        }

        public void DeleteContentsByChannelId(int siteId, string tableName, int channelId)
        {
            if (string.IsNullOrEmpty(tableName)) return;

            var contentIdList = GetContentIdListChecked(tableName, channelId, string.Empty);

            TagUtils.RemoveTags(siteId, contentIdList);

            var sqlString =
                $"DELETE FROM {tableName} WHERE SiteId = {siteId} AND ChannelId = {channelId}";
            var deleteNum = ExecuteNonQuery(sqlString);

            if (channelId <= 0 || deleteNum <= 0) return;

            ContentManager.RemoveCache(tableName, channelId);
        }

        public void DeleteContentsByDeletedChannelIdList(IDbTransaction trans, SiteInfo siteInfo, List<int> channelIdList)
        {
            foreach (var channelId in channelIdList)
            {
                var tableName = ChannelManager.GetTableName(siteInfo, channelId);
                if (string.IsNullOrEmpty(tableName)) continue;

                ExecuteNonQuery(trans, $"DELETE FROM {tableName} WHERE SiteId = {siteInfo.Id} AND {nameof(ContentInfo.ChannelId)} = {channelId}");

                ContentManager.RemoveCache(tableName, channelId);
            }
        }

        public void UpdateRestoreContentsByTrash(int siteId, int channelId, string tableName)
        {
            var updateNum = 0;

            if (!string.IsNullOrEmpty(tableName))
            {
                var sqlString =
                    $"UPDATE {tableName} SET ChannelId = -ChannelId, LastEditDate = {SqlUtils.GetComparableNow()} WHERE SiteId = {siteId} AND ChannelId < 0";
                updateNum = ExecuteNonQuery(sqlString);
            }

            if (updateNum <= 0) return;

            ContentManager.RemoveCache(tableName, channelId);
        }

        private void UpdateTaxis(int channelId, int id, int taxis, string tableName)
        {
            var sqlString = $"UPDATE {tableName} SET Taxis = {taxis} WHERE Id = {id}";
            ExecuteNonQuery(sqlString);

            ContentManager.RemoveCache(tableName, channelId);
        }

        private void DeleteContents(int siteId, int channelId, string tableName, List<int> contentIdList)
        {
            if (string.IsNullOrEmpty(tableName)) return;

            var deleteNum = 0;

            if (!string.IsNullOrEmpty(tableName) && contentIdList != null && contentIdList.Count > 0)
            {
                TagUtils.RemoveTags(siteId, contentIdList);

                var sqlString =
                    $"DELETE FROM {tableName} WHERE SiteId = {siteId} AND Id IN ({TranslateUtils.ToSqlInStringWithoutQuote(contentIdList)})";
                deleteNum = ExecuteNonQuery(sqlString);
            }

            if (deleteNum <= 0) return;

            ContentManager.RemoveCache(tableName, channelId);
        }

        public void DeletePreviewContents(int siteId, string tableName, ChannelInfo channelInfo)
        {
            if (string.IsNullOrEmpty(tableName)) return;

            channelInfo.Additional.IsPreviewContentsExists = false;
            DataProvider.ChannelDao.UpdateAdditional(channelInfo);

            var sqlString =
                $"DELETE FROM {tableName} WHERE {nameof(ContentInfo.SiteId)} = @{nameof(ContentInfo.SiteId)} AND {nameof(ContentInfo.ChannelId)} = @{nameof(ContentInfo.ChannelId)} AND {nameof(ContentInfo.SourceId)} = @{nameof(ContentInfo.SourceId)}";

            using (var connection = GetConnection())
            {
                connection.Execute(sqlString, new
                    {
                        SiteId = siteId,
                        ChannelId = channelInfo.Id,
                        SourceId = SourceManager.Preview
                    }
                );
            }
        }

        public void UpdateArrangeTaxis(string tableName, int channelId, string attributeName, bool isDesc)
        {
            var taxisDirection = isDesc ? "ASC" : "DESC";//升序,但由于页面排序是按Taxis的Desc排序的，所以这里sql里面的ASC/DESC取反

            var sqlString =
                $"SELECT Id, IsTop FROM {tableName} WHERE ChannelId = {channelId} OR ChannelId = -{channelId} ORDER BY {attributeName} {taxisDirection}";
            var sqlList = new List<string>();

            using (var rdr = ExecuteReader(sqlString))
            {
                var taxis = 1;
                while (rdr.Read())
                {
                    var id = GetInt(rdr, 0);
                    var isTop = GetBool(rdr, 1);

                    sqlList.Add(
                        $"UPDATE {tableName} SET Taxis = {taxis++}, IsTop = '{isTop}' WHERE Id = {id}");
                }
                rdr.Close();
            }

            DataProvider.DatabaseDao.ExecuteSql(sqlList);

            ContentManager.RemoveCache(tableName, channelId);
        }

        public bool SetTaxisToUp(string tableName, int channelId, int contentId, bool isTop)
        {
            //Get Higher Taxis and Id
            var sqlString = SqlUtils.ToTopSqlString(tableName, "Id, Taxis",
                isTop
                    ? $"WHERE (Taxis > (SELECT Taxis FROM {tableName} WHERE Id = {contentId}) AND Taxis >= {TaxisIsTopStartValue} AND ChannelId = {channelId})"
                    : $"WHERE (Taxis > (SELECT Taxis FROM {tableName} WHERE Id = {contentId}) AND Taxis < {TaxisIsTopStartValue} AND ChannelId = {channelId})",
                "ORDER BY Taxis", 1);
            var higherId = 0;
            var higherTaxis = 0;

            using (var rdr = ExecuteReader(sqlString))
            {
                if (rdr.Read())
                {
                    higherId = GetInt(rdr, 0);
                    higherTaxis = GetInt(rdr, 1);
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
            var sqlString = SqlUtils.ToTopSqlString(tableName, "Id, Taxis",
                isTop
                    ? $"WHERE (Taxis < (SELECT Taxis FROM {tableName} WHERE Id = {contentId}) AND Taxis >= {TaxisIsTopStartValue} AND ChannelId = {channelId})"
                    : $"WHERE (Taxis < (SELECT Taxis FROM {tableName} WHERE Id = {contentId}) AND Taxis < {TaxisIsTopStartValue} AND ChannelId = {channelId})",
                "ORDER BY Taxis DESC", 1);
            var lowerId = 0;
            var lowerTaxis = 0;

            using (var rdr = ExecuteReader(sqlString))
            {
                if (rdr.Read())
                {
                    lowerId = GetInt(rdr, 0);
                    lowerTaxis = GetInt(rdr, 1);
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
                    using (var rdr = ExecuteReader(conn, sqlString))
                    {
                        if (rdr.Read())
                        {
                            maxTaxis = GetInt(rdr, 0);
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
                    using (var rdr = ExecuteReader(conn, sqlString))
                    {
                        if (rdr.Read())
                        {
                            maxTaxis = GetInt(rdr, 0);
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
                var sqlString = $"SELECT {nameof(ContentInfo.ChannelId)}, {name} FROM {tableName} WHERE Id = {contentId}";

                using (var conn = GetConnection())
                {
                    conn.Open();
                    using (var rdr = ExecuteReader(conn, sqlString))
                    {
                        if (rdr.Read())
                        {
                            var channelId = GetInt(rdr, 0);
                            var value = GetString(rdr, 1);

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
            var tuple = GetValue(tableName, contentId, ContentAttribute.GroupNameCollection);

            if (tuple != null)
            {
                var list = TranslateUtils.StringCollectionToStringList(tuple.Item2);
                foreach (var groupName in contentGroupList)
                {
                    if (!list.Contains(groupName)) list.Add(groupName);
                }
                Update(tableName, tuple.Item1, contentId, ContentAttribute.GroupNameCollection, TranslateUtils.ObjectCollectionToString(list));
            }
        }

        private List<int> GetReferenceIdList(string tableName, List<int> contentIdList)
        {
            var list = new List<int>();
            var sqlString =
                $"SELECT Id FROM {tableName} WHERE ChannelId > 0 AND ReferenceId IN ({TranslateUtils.ToSqlInStringWithoutQuote(contentIdList)})";

            using (var rdr = ExecuteReader(sqlString))
            {
                while (rdr.Read())
                {
                    list.Add(GetInt(rdr, 0));
                }
                rdr.Close();
            }

            return list;
        }

        public int GetTotalHits(string tableName, int siteId)
        {
            return DataProvider.DatabaseDao.GetIntResult($"SELECT SUM(Hits) FROM {tableName} WHERE IsChecked='{true}' AND SiteId = {siteId} AND Hits > 0");
        }

        public int GetFirstContentId(string tableName, int channelId)
        {
            var sqlString = $"SELECT Id FROM {tableName} WHERE ChannelId = {channelId} ORDER BY Taxis DESC, Id DESC";
            return DataProvider.DatabaseDao.GetIntResult(sqlString);
        }

        public List<int> GetContentIdList(string tableName, int channelId)
        {
            var list = new List<int>();

            var sqlString = $"SELECT Id FROM {tableName} WHERE ChannelId = {channelId}";
            using (var rdr = ExecuteReader(sqlString))
            {
                while (rdr.Read())
                {
                    var contentId = GetInt(rdr, 0);
                    list.Add(contentId);
                }
                rdr.Close();
            }
            return list;
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

            using (var rdr = ExecuteReader(sqlString))
            {
                while (rdr.Read())
                {
                    var contentId = GetInt(rdr, 0);
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
            using (var rdr = ExecuteReader(sqlString))
            {
                while (rdr.Read())
                {
                    list.Add(GetInt(rdr, 0));
                }
                rdr.Close();
            }
            return list;
        }

        public int GetContentId(string tableName, int channelId, int taxis, bool isNextContent)
        {
            var contentId = 0;
            var sqlString = SqlUtils.ToTopSqlString(tableName, "Id", $"WHERE (ChannelId = {channelId} AND Taxis > {taxis} AND IsChecked = 'True')", "ORDER BY Taxis", 1);
            if (isNextContent)
            {
                sqlString = SqlUtils.ToTopSqlString(tableName, "Id",
                $"WHERE (ChannelId = {channelId} AND Taxis < {taxis} AND IsChecked = 'True')", "ORDER BY Taxis DESC", 1);
            }

            using (var rdr = ExecuteReader(sqlString))
            {
                if (rdr.Read())
                {
                    contentId = GetInt(rdr, 0);
                }
                rdr.Close();
            }
            return contentId;
        }

        public int GetContentId(string tableName, int channelId, string orderByString)
        {
            var contentId = 0;
            var sqlString = SqlUtils.ToTopSqlString(tableName, "Id", $"WHERE (ChannelId = {channelId})", orderByString, 1);

            using (var rdr = ExecuteReader(sqlString))
            {
                if (rdr.Read())
                {
                    contentId = GetInt(rdr, 0);
                }
                rdr.Close();
            }
            return contentId;
        }

        public List<string> GetValueListByStartString(string tableName, int channelId, string name, string startString, int totalNum)
        {
            var inStr = SqlUtils.GetInStr(name, startString);
            var sqlString = SqlUtils.GetDistinctTopSqlString(tableName, name, $"WHERE ChannelId = {channelId} AND {inStr}", string.Empty, totalNum);
            return DataProvider.DatabaseDao.GetStringList(sqlString);
        }

        public int GetChannelId(string tableName, int contentId)
        {
            var channelId = 0;
            var sqlString = $"SELECT {ContentAttribute.ChannelId} FROM {tableName} WHERE (Id = {contentId})";

            using (var rdr = ExecuteReader(sqlString))
            {
                if (rdr.Read())
                {
                    channelId = GetInt(rdr, 0);
                }
                rdr.Close();
            }
            return channelId;
        }

        //public int GetCount(string tableName, int channelId)
        //{
        //    var sqlString = $"SELECT COUNT(*) FROM {tableName} WHERE {nameof(ContentInfo.ChannelId)} = {channelId} AND {nameof(ContentInfo.SourceId)} != {SourceManager.Preview}";

        //    return DataProvider.DatabaseDao.GetIntResult(sqlString);
        //}

        public int GetSequence(string tableName, int channelId, int contentId)
        {
            var sqlString =
                $"SELECT COUNT(*) FROM {tableName} WHERE {nameof(ContentInfo.ChannelId)} = {channelId} AND {nameof(ContentInfo.IsChecked)} = '{true}' AND Taxis < (SELECT Taxis FROM {tableName} WHERE Id = {contentId}) AND {nameof(ContentInfo.SourceId)} != {SourceManager.Preview}";

            return DataProvider.DatabaseDao.GetIntResult(sqlString) + 1;
        }

        public List<int> GetChannelIdListCheckedByLastEditDateHour(string tableName, int siteId, int hour)
        {
            var list = new List<int>();

            var sqlString =
                $"SELECT DISTINCT ChannelId FROM {tableName} WHERE (SiteId = {siteId}) AND (IsChecked = '{true}') AND (LastEditDate BETWEEN {SqlUtils.GetComparableDateTime(DateTime.Now.AddHours(-hour))} AND {SqlUtils.GetComparableNow()})";

            using (var rdr = ExecuteReader(sqlString))
            {
                while (rdr.Read())
                {
                    var channelId = GetInt(rdr, 0);
                    list.Add(channelId);
                }
                rdr.Close();
            }
            return list;
        }

        public DataSet GetDataSetOfAdminExcludeRecycle(string tableName, int siteId, DateTime begin, DateTime end)
        {
            var sqlString = GetSqlStringOfAdminExcludeRecycle(tableName, siteId, begin, end);

            return ExecuteDataset(sqlString);
        }

        public int Insert(string tableName, SiteInfo siteInfo, ChannelInfo channelInfo, ContentInfo contentInfo)
        {
            var taxis = 0;
            if (contentInfo.SourceId == SourceManager.Preview)
            {
                channelInfo.Additional.IsPreviewContentsExists = true;
                DataProvider.ChannelDao.UpdateAdditional(channelInfo);
            }
            else
            {
                taxis = GetTaxisToInsert(tableName, contentInfo.ChannelId, contentInfo.IsTop);
            }
            return InsertWithTaxis(tableName, siteInfo, channelInfo, contentInfo, taxis);
        }

        public int InsertPreview(string tableName, SiteInfo siteInfo, ChannelInfo channelInfo, ContentInfo contentInfo)
        {
            channelInfo.Additional.IsPreviewContentsExists = true;
            DataProvider.ChannelDao.UpdateAdditional(channelInfo);

            contentInfo.SourceId = SourceManager.Preview;
            return InsertWithTaxis(tableName, siteInfo, channelInfo, contentInfo, 0);
        }

        public int InsertWithTaxis(string tableName, SiteInfo siteInfo, ChannelInfo channelInfo, ContentInfo contentInfo, int taxis)
        {
            if (string.IsNullOrEmpty(tableName)) return 0;

            if (siteInfo.Additional.IsAutoPageInTextEditor && contentInfo.ContainsKey(BackgroundContentAttribute.Content))
            {
                contentInfo.Set(BackgroundContentAttribute.Content, ContentUtility.GetAutoPageContent(contentInfo.GetString(BackgroundContentAttribute.Content), siteInfo.Additional.AutoPageWordNum));
            }

            contentInfo.Taxis = taxis;

            var contentId = InsertInner(tableName, siteInfo, channelInfo, contentInfo);

            return contentId;
        }

        private int InsertInner(string tableName, SiteInfo siteInfo, ChannelInfo channelInfo, ContentInfo contentInfo)
        {
            if (string.IsNullOrEmpty(tableName) || contentInfo == null) return 0;

            contentInfo.LastEditDate = DateTime.Now;

            var columnInfoList = TableColumnManager.GetTableColumnInfoList(tableName, ContentAttribute.AllAttributes.Value);

            var names = new StringBuilder();
            var values = new StringBuilder();
            var paras = new List<IDataParameter>();
            var excludeAttributesNames = new List<string>(ContentAttribute.AllAttributes.Value);
            foreach (var columnInfo in columnInfoList)
            {
                excludeAttributesNames.Add(columnInfo.AttributeName);
                names.Append($",{columnInfo.AttributeName}").AppendLine();
                values.Append($",@{columnInfo.AttributeName}").AppendLine();
                if (columnInfo.DataType == DataType.Integer)
                {
                    paras.Add(GetParameter(columnInfo.AttributeName, columnInfo.DataType, contentInfo.GetInt(columnInfo.AttributeName)));
                }
                else if (columnInfo.DataType == DataType.Decimal)
                {
                    paras.Add(GetParameter(columnInfo.AttributeName, columnInfo.DataType, contentInfo.GetDecimal(columnInfo.AttributeName)));
                }
                else if (columnInfo.DataType == DataType.Boolean)
                {
                    paras.Add(GetParameter(columnInfo.AttributeName, columnInfo.DataType, contentInfo.GetBool(columnInfo.AttributeName)));
                }
                else if (columnInfo.DataType == DataType.DateTime)
                {
                    paras.Add(GetParameter(columnInfo.AttributeName, columnInfo.DataType, contentInfo.GetDateTime(columnInfo.AttributeName, DateTime.Now)));
                }
                else
                {
                    paras.Add(GetParameter(columnInfo.AttributeName, columnInfo.DataType, contentInfo.GetString(columnInfo.AttributeName)));
                }
            }

            var sqlString = $@"
INSERT INTO {tableName} (
    {nameof(ContentInfo.ChannelId)},
    {nameof(ContentInfo.SiteId)},
    {nameof(ContentInfo.AddUserName)},
    {nameof(ContentInfo.LastEditUserName)},
    {nameof(ContentInfo.LastEditDate)},
    {nameof(ContentInfo.AdminId)},
    {nameof(ContentInfo.UserId)},
    {nameof(ContentInfo.Taxis)},
    {nameof(ContentInfo.GroupNameCollection)},
    {nameof(ContentInfo.Tags)},
    {nameof(ContentInfo.SourceId)},
    {nameof(ContentInfo.ReferenceId)},
    {nameof(ContentInfo.IsChecked)},
    {nameof(ContentInfo.CheckedLevel)},
    {nameof(ContentInfo.Hits)},
    {nameof(ContentInfo.HitsByDay)},
    {nameof(ContentInfo.HitsByWeek)},
    {nameof(ContentInfo.HitsByMonth)},
    {nameof(ContentInfo.LastHitsDate)},
    {nameof(ContentInfo.SettingsXml)},
    {nameof(ContentInfo.Title)},
    {nameof(ContentInfo.IsTop)},
    {nameof(ContentInfo.IsRecommend)},
    {nameof(ContentInfo.IsHot)},
    {nameof(ContentInfo.IsColor)},
    {nameof(ContentInfo.LinkUrl)},
    {nameof(ContentInfo.AddDate)}
    {names}
) VALUES (
    @{nameof(ContentInfo.ChannelId)},
    @{nameof(ContentInfo.SiteId)},
    @{nameof(ContentInfo.AddUserName)},
    @{nameof(ContentInfo.LastEditUserName)},
    @{nameof(ContentInfo.LastEditDate)},
    @{nameof(ContentInfo.AdminId)},
    @{nameof(ContentInfo.UserId)},
    @{nameof(ContentInfo.Taxis)},
    @{nameof(ContentInfo.GroupNameCollection)},
    @{nameof(ContentInfo.Tags)},
    @{nameof(ContentInfo.SourceId)},
    @{nameof(ContentInfo.ReferenceId)},
    @{nameof(ContentInfo.IsChecked)},
    @{nameof(ContentInfo.CheckedLevel)},
    @{nameof(ContentInfo.Hits)},
    @{nameof(ContentInfo.HitsByDay)},
    @{nameof(ContentInfo.HitsByWeek)},
    @{nameof(ContentInfo.HitsByMonth)},
    @{nameof(ContentInfo.LastHitsDate)},
    @{nameof(ContentInfo.SettingsXml)},
    @{nameof(ContentInfo.Title)},
    @{nameof(ContentInfo.IsTop)},
    @{nameof(ContentInfo.IsRecommend)},
    @{nameof(ContentInfo.IsHot)},
    @{nameof(ContentInfo.IsColor)},
    @{nameof(ContentInfo.LinkUrl)},
    @{nameof(ContentInfo.AddDate)}
    {values}
)";

            var parameters = new List<IDataParameter>
            {
                GetParameter(nameof(ContentInfo.ChannelId), DataType.Integer, contentInfo.ChannelId),
                GetParameter(nameof(ContentInfo.SiteId), DataType.Integer, contentInfo.SiteId),
                GetParameter(nameof(ContentInfo.AddUserName), DataType.VarChar, 255, contentInfo.AddUserName),
                GetParameter(nameof(ContentInfo.LastEditUserName), DataType.VarChar, 255, contentInfo.LastEditUserName),
                GetParameter(nameof(ContentInfo.LastEditDate), DataType.DateTime, contentInfo.LastEditDate),
                GetParameter(nameof(ContentInfo.AdminId), DataType.Integer, contentInfo.AdminId),
                GetParameter(nameof(ContentInfo.UserId), DataType.Integer, contentInfo.UserId),
                GetParameter(nameof(ContentInfo.Taxis), DataType.Integer, contentInfo.Taxis),
                GetParameter(nameof(ContentInfo.GroupNameCollection), DataType.VarChar, 255, contentInfo.GroupNameCollection),
                GetParameter(nameof(ContentInfo.Tags), DataType.VarChar, 255, contentInfo.Tags),
                GetParameter(nameof(ContentInfo.SourceId), DataType.Integer, contentInfo.SourceId),
                GetParameter(nameof(ContentInfo.ReferenceId), DataType.Integer, contentInfo.ReferenceId),
                GetParameter(nameof(ContentInfo.IsChecked), DataType.VarChar, 18, contentInfo.IsChecked.ToString()),
                GetParameter(nameof(ContentInfo.CheckedLevel), DataType.Integer, contentInfo.CheckedLevel),
                GetParameter(nameof(ContentInfo.Hits), DataType.Integer, contentInfo.Hits),
                GetParameter(nameof(ContentInfo.HitsByDay), DataType.Integer, contentInfo.HitsByDay),
                GetParameter(nameof(ContentInfo.HitsByWeek), DataType.Integer, contentInfo.HitsByWeek),
                GetParameter(nameof(ContentInfo.HitsByMonth), DataType.Integer, contentInfo.HitsByMonth),
                GetParameter(nameof(ContentInfo.LastHitsDate), DataType.DateTime, contentInfo.LastHitsDate),
                GetParameter(nameof(ContentInfo.SettingsXml), DataType.Text, contentInfo.ToString(excludeAttributesNames)),
                GetParameter(nameof(ContentInfo.Title), DataType.VarChar, 255, contentInfo.Title),
                GetParameter(nameof(ContentInfo.IsTop), DataType.VarChar, 18, contentInfo.IsTop.ToString()),
                GetParameter(nameof(ContentInfo.IsRecommend), DataType.VarChar, 18, contentInfo.IsRecommend.ToString()),
                GetParameter(nameof(ContentInfo.IsHot), DataType.VarChar, 18, contentInfo.IsHot.ToString()),
                GetParameter(nameof(ContentInfo.IsColor), DataType.VarChar, 18, contentInfo.IsColor.ToString()),
                GetParameter(nameof(ContentInfo.LinkUrl), DataType.VarChar, 200, contentInfo.LinkUrl),
                GetParameter(nameof(ContentInfo.AddDate), DataType.DateTime, contentInfo.AddDate)
            };
            parameters.AddRange(paras);

            contentInfo.Id = ExecuteNonQueryAndReturnId(tableName, nameof(ContentInfo.Id), sqlString, parameters.ToArray());

            ContentManager.InsertCache(siteInfo, channelInfo, contentInfo);

            return contentInfo.Id;
        }

        public void Update(SiteInfo siteInfo, ChannelInfo channelInfo, ContentInfo contentInfo)
        {
            if (contentInfo == null) return;

            if (siteInfo.Additional.IsAutoPageInTextEditor &&
                contentInfo.ContainsKey(BackgroundContentAttribute.Content))
            {
                contentInfo.Set(BackgroundContentAttribute.Content,
                    ContentUtility.GetAutoPageContent(contentInfo.GetString(BackgroundContentAttribute.Content),
                        siteInfo.Additional.AutoPageWordNum));
            }

            var tableName = ChannelManager.GetTableName(siteInfo, channelInfo);

            //出现IsTop与Taxis不同步情况
            if (contentInfo.IsTop == false && contentInfo.Taxis >= TaxisIsTopStartValue)
            {
                contentInfo.Taxis = GetMaxTaxis(tableName, contentInfo.ChannelId, false) + 1;
            }
            else if (contentInfo.IsTop && contentInfo.Taxis < TaxisIsTopStartValue)
            {
                contentInfo.Taxis = GetMaxTaxis(tableName, contentInfo.ChannelId, true) + 1;
            }

            contentInfo.LastEditDate = DateTime.Now;

            var columnInfoList =
                TableColumnManager.GetTableColumnInfoList(tableName, ContentAttribute.AllAttributes.Value);

            var sets = new StringBuilder();
            var paras = new List<IDataParameter>();
            var excludeAttributesNames = new List<string>(ContentAttribute.AllAttributes.Value);
            foreach (var columnInfo in columnInfoList)
            {
                excludeAttributesNames.Add(columnInfo.AttributeName);
                sets.Append($",{columnInfo.AttributeName} = @{columnInfo.AttributeName}").AppendLine();
                if (columnInfo.DataType == DataType.Integer)
                {
                    paras.Add(GetParameter(columnInfo.AttributeName, columnInfo.DataType,
                        contentInfo.GetInt(columnInfo.AttributeName)));
                }
                else if (columnInfo.DataType == DataType.Decimal)
                {
                    paras.Add(GetParameter(columnInfo.AttributeName, columnInfo.DataType,
                        contentInfo.GetDecimal(columnInfo.AttributeName)));
                }
                else if (columnInfo.DataType == DataType.Boolean)
                {
                    paras.Add(GetParameter(columnInfo.AttributeName, columnInfo.DataType,
                        contentInfo.GetBool(columnInfo.AttributeName)));
                }
                else if (columnInfo.DataType == DataType.DateTime)
                {
                    paras.Add(GetParameter(columnInfo.AttributeName, columnInfo.DataType,
                        contentInfo.GetDateTime(columnInfo.AttributeName, DateTime.Now)));
                }
                else
                {
                    paras.Add(GetParameter(columnInfo.AttributeName, columnInfo.DataType,
                        contentInfo.GetString(columnInfo.AttributeName)));
                }
            }

            var sqlString = $@"
UPDATE {tableName} SET 
    {nameof(ContentInfo.ChannelId)} = @{nameof(ContentInfo.ChannelId)},
    {nameof(ContentInfo.SiteId)} = @{nameof(ContentInfo.SiteId)},
    {nameof(ContentInfo.AddUserName)} = @{nameof(ContentInfo.AddUserName)},
    {nameof(ContentInfo.LastEditUserName)} = @{nameof(ContentInfo.LastEditUserName)},
    {nameof(ContentInfo.LastEditDate)} = @{nameof(ContentInfo.LastEditDate)},
    {nameof(ContentInfo.AdminId)} = @{nameof(ContentInfo.AdminId)},
    {nameof(ContentInfo.UserId)} = @{nameof(ContentInfo.UserId)},
    {nameof(ContentInfo.Taxis)} = @{nameof(ContentInfo.Taxis)},
    {nameof(ContentInfo.GroupNameCollection)} = @{nameof(ContentInfo.GroupNameCollection)},
    {nameof(ContentInfo.Tags)} = @{nameof(ContentInfo.Tags)},
    {nameof(ContentInfo.SourceId)} = @{nameof(ContentInfo.SourceId)},
    {nameof(ContentInfo.ReferenceId)} = @{nameof(ContentInfo.ReferenceId)},";

            if (contentInfo.CheckedLevel != CheckManager.LevelInt.NotChange)
            {
                sqlString += $@"
    {nameof(ContentInfo.IsChecked)} = @{nameof(ContentInfo.IsChecked)},
    {nameof(ContentInfo.CheckedLevel)} = @{nameof(ContentInfo.CheckedLevel)},";
            }

            sqlString += $@"
    {nameof(ContentInfo.Hits)} = @{nameof(ContentInfo.Hits)},
    {nameof(ContentInfo.HitsByDay)} = @{nameof(ContentInfo.HitsByDay)},
    {nameof(ContentInfo.HitsByWeek)} = @{nameof(ContentInfo.HitsByWeek)},
    {nameof(ContentInfo.HitsByMonth)} = @{nameof(ContentInfo.HitsByMonth)},
    {nameof(ContentInfo.LastHitsDate)} = @{nameof(ContentInfo.LastHitsDate)},
    {nameof(ContentInfo.SettingsXml)} = @{nameof(ContentInfo.SettingsXml)},
    {nameof(ContentInfo.Title)} = @{nameof(ContentInfo.Title)},
    {nameof(ContentInfo.IsTop)} = @{nameof(ContentInfo.IsTop)},
    {nameof(ContentInfo.IsRecommend)} = @{nameof(ContentInfo.IsRecommend)},
    {nameof(ContentInfo.IsHot)} = @{nameof(ContentInfo.IsHot)},
    {nameof(ContentInfo.IsColor)} = @{nameof(ContentInfo.IsColor)},
    {nameof(ContentInfo.LinkUrl)} = @{nameof(ContentInfo.LinkUrl)},
    {nameof(ContentInfo.AddDate)} = @{nameof(ContentInfo.AddDate)}
    {sets}
WHERE {nameof(ContentInfo.Id)} = @{nameof(ContentInfo.Id)}";

            var parameters = new List<IDataParameter>
            {
                GetParameter(nameof(ContentInfo.ChannelId), DataType.Integer, channelInfo.Id),
                GetParameter(nameof(ContentInfo.SiteId), DataType.Integer, siteInfo.Id),
                GetParameter(nameof(ContentInfo.AddUserName), DataType.VarChar, 255, contentInfo.AddUserName),
                GetParameter(nameof(ContentInfo.LastEditUserName), DataType.VarChar, 255,
                    contentInfo.LastEditUserName),
                GetParameter(nameof(ContentInfo.LastEditDate), DataType.DateTime, contentInfo.LastEditDate),
                GetParameter(nameof(ContentInfo.AdminId), DataType.Integer,
                    contentInfo.AdminId),
                GetParameter(nameof(ContentInfo.UserId), DataType.Integer,
                    contentInfo.UserId),
                GetParameter(nameof(ContentInfo.Taxis), DataType.Integer, contentInfo.Taxis),
                GetParameter(nameof(ContentInfo.GroupNameCollection), DataType.VarChar, 255,
                    contentInfo.GroupNameCollection),
                GetParameter(nameof(ContentInfo.Tags), DataType.VarChar, 255, contentInfo.Tags),
                GetParameter(nameof(ContentInfo.SourceId), DataType.Integer, contentInfo.SourceId),
                GetParameter(nameof(ContentInfo.ReferenceId), DataType.Integer, contentInfo.ReferenceId),
            };
            if (contentInfo.CheckedLevel != CheckManager.LevelInt.NotChange)
            {
                parameters.Add(GetParameter(nameof(ContentInfo.IsChecked), DataType.VarChar, 18,
                    contentInfo.IsChecked.ToString()));
                parameters.Add(GetParameter(nameof(ContentInfo.CheckedLevel), DataType.Integer,
                    contentInfo.CheckedLevel));
            }
            parameters.Add(GetParameter(nameof(ContentInfo.Hits), DataType.Integer, contentInfo.Hits));
            parameters.Add(
                GetParameter(nameof(ContentInfo.HitsByDay), DataType.Integer, contentInfo.HitsByDay));
            parameters.Add(
                GetParameter(nameof(ContentInfo.HitsByWeek), DataType.Integer, contentInfo.HitsByWeek));
            parameters.Add(
                GetParameter(nameof(ContentInfo.HitsByMonth), DataType.Integer,
                    contentInfo.HitsByMonth));
            parameters.Add(
                GetParameter(nameof(ContentInfo.LastHitsDate), DataType.DateTime,
                    contentInfo.LastHitsDate));
            parameters.Add(
                GetParameter(nameof(ContentInfo.SettingsXml), DataType.Text,
                    contentInfo.ToString(excludeAttributesNames)));
            parameters.Add(
                GetParameter(nameof(ContentInfo.Title), DataType.VarChar, 255,
                    contentInfo.Title));
            parameters.Add(
                GetParameter(nameof(ContentInfo.IsTop), DataType.VarChar, 18,
                    contentInfo.IsTop.ToString()));
            parameters.Add(
                GetParameter(nameof(ContentInfo.IsRecommend), DataType.VarChar,
                    18, contentInfo.IsRecommend.ToString()));
            parameters.Add(
                GetParameter(nameof(ContentInfo.IsHot), DataType.VarChar, 18,
                    contentInfo.IsHot.ToString()));
            parameters.Add(
                GetParameter(nameof(ContentInfo.IsColor),
                    DataType.VarChar, 18, contentInfo.IsColor.ToString()));
            parameters.Add(
                GetParameter(nameof(ContentInfo.LinkUrl),
                    DataType.VarChar, 200, contentInfo.LinkUrl));
            parameters.Add(GetParameter(
                $"@{nameof(ContentInfo.AddDate)}", DataType.DateTime,
                contentInfo.AddDate));

            parameters.AddRange(paras);
            parameters.Add(GetParameter(nameof(ContentInfo.Id), DataType.Integer, contentInfo.Id));

            ExecuteNonQuery(sqlString, parameters.ToArray());

            ContentManager.UpdateCache(siteInfo, channelInfo, contentInfo);
            ContentManager.RemoveCountCache(tableName);
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

            return DataProvider.DatabaseDao.GetIntResult(sqlString);
        }

        public DataSet GetStlDataSourceChecked(List<int> channelIdList, string tableName, int startNum, int totalNum, string orderByString, string whereString, NameValueCollection others)
        {
            return GetStlDataSourceChecked(tableName, channelIdList, startNum, totalNum, orderByString, whereString, others);
        }

        public List<int> GetIdListBySameTitle(string tableName, int channelId, string title)
        {
            var list = new List<int>();
            var sql = $"SELECT Id FROM {tableName} WHERE ChannelId = {channelId} AND Title = '{title}'";
            using (var rdr = ExecuteReader(sql))
            {
                while (rdr.Read())
                {
                    list.Add(GetInt(rdr, 0));
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

            return DataProvider.DatabaseDao.GetIntResult(sqlString);
        }

        public List<ContentCountInfo> GetContentCountInfoList(string tableName)
        {
            List<ContentCountInfo> list;

            var sqlString =
                $@"SELECT {nameof(ContentInfo.SiteId)}, {nameof(ContentInfo.ChannelId)}, {nameof(ContentInfo.IsChecked)}, {nameof(ContentInfo.CheckedLevel)}, {nameof(ContentInfo.AdminId)}, COUNT(*) AS {nameof(ContentCountInfo.Count)} FROM {tableName} WHERE {nameof(ContentInfo.ChannelId)} > 0 AND {nameof(ContentInfo.SourceId)} != {SourceManager.Preview} GROUP BY {nameof(ContentInfo.SiteId)}, {nameof(ContentInfo.ChannelId)}, {nameof(ContentInfo.IsChecked)}, {nameof(ContentInfo.CheckedLevel)}, {nameof(ContentInfo.AdminId)}";

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
                $"SELECT COUNT(*) FROM {tableName} WHERE {nameof(ContentInfo.ChannelId)} = {channelId} AND {nameof(ContentInfo.ImageUrl)} != '' AND {ContentAttribute.IsChecked} = '{true}' AND {nameof(ContentInfo.SourceId)} != {SourceManager.Preview}";

            return DataProvider.DatabaseDao.GetIntResult(sqlString);
        }

        private int GetTaxis(int selectedId, string tableName)
        {
            var sqlString = $"SELECT Taxis FROM {tableName} WHERE (Id = {selectedId})";

            return DataProvider.DatabaseDao.GetIntResult(sqlString);
        }

        private List<int> GetContentIdListByTrash(int siteId, string tableName)
        {
            var sqlString =
                $"SELECT Id FROM {tableName} WHERE SiteId = {siteId} AND ChannelId < 0";
            return DataProvider.DatabaseDao.GetIntList(sqlString);
        }

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
            DataSet dataset = null;
            if (!string.IsNullOrEmpty(tableName))
            {
                var sqlSelect = DataProvider.DatabaseDao.GetSelectSqlString(tableName, totalNum, MinListColumns, whereString, orderByString);
                dataset = ExecuteDataset(sqlSelect);
            }
            return dataset;
        }

        private DataSet GetStlDataSourceByStartNum(string tableName, int startNum, int totalNum, string whereString, string orderByString)
        {
            DataSet dataset = null;
            if (!string.IsNullOrEmpty(tableName))
            {
                //var sqlSelect = DataProvider.DatabaseDao.GetSelectSqlString(tableName, startNum, totalNum, MinListColumns, whereString, orderByString);
                var sqlSelect = DataProvider.DatabaseDao.GetPageSqlString(tableName, MinListColumns, whereString, orderByString, startNum - 1, totalNum);
                dataset = ExecuteDataset(sqlSelect);
            }
            return dataset;
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
                sqlString += $" AND {ContentAttribute.IsChecked} = '{ETriStateUtils.GetValue(checkedState)}'";
            }

            return DataProvider.DatabaseDao.GetIntResult(sqlString);
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
                sqlString = SqlUtils.ToTopSqlString(tableName, "Id",
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

            using (var rdr = ExecuteReader(sqlString))
            {
                while (rdr.Read())
                {
                    var contentId = GetInt(rdr, 0);
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

        public bool ApiIsExists(string tableName, int id)
        {
            var sqlString = $"SELECT count(1) FROM {tableName} WHERE Id = @Id";

            using (var connection = GetConnection())
            {
                return connection.ExecuteScalar<bool>(sqlString, new { Id = id });
            }
        }

        public List<Tuple<int, int>> ApiGetContentIdListBySiteId(string tableName, int siteId, ApiContentsParameters parameters, out int totalCount)
        {
            totalCount = 0;
            var list = new List<ContentInfo>();

            var whereString = $"WHERE {ContentAttribute.SiteId} = {siteId} AND {ContentAttribute.ChannelId} > 0 AND {ContentAttribute.IsChecked} = '{true}'";
            if (parameters.ChannelIds.Count > 0)
            {
                whereString += $" AND {ContentAttribute.ChannelId} IN ({TranslateUtils.ObjectCollectionToString(parameters.ChannelIds)})";
            }
            if (!string.IsNullOrEmpty(parameters.ChannelGroup))
            {
                var channelIdList = ChannelManager.GetChannelIdList(siteId, parameters.ChannelGroup);
                if (channelIdList.Count > 0)
                {
                    whereString += $" AND {ContentAttribute.ChannelId} IN ({TranslateUtils.ObjectCollectionToString(channelIdList)})";
                }
            }
            if (!string.IsNullOrEmpty(parameters.ContentGroup))
            {
                whereString += $" AND ({ContentAttribute.GroupNameCollection} = '{parameters.ContentGroup}' OR {SqlUtils.GetInStr(ContentAttribute.GroupNameCollection, parameters.ContentGroup + ",")} OR {SqlUtils.GetInStr(ContentAttribute.GroupNameCollection, "," + parameters.ContentGroup + ",")} OR {SqlUtils.GetInStr(ContentAttribute.GroupNameCollection, "," + parameters.ContentGroup)})";
            }
            if (!string.IsNullOrEmpty(parameters.Tag))
            {
                whereString += $" AND ({ContentAttribute.Tags} = '{parameters.Tag}' OR {SqlUtils.GetInStr(ContentAttribute.Tags, parameters.Tag + ",")} OR {SqlUtils.GetInStr(ContentAttribute.Tags, "," + parameters.Tag + ",")} OR {SqlUtils.GetInStr(ContentAttribute.Tags, "," + parameters.Tag)})";
            }

            var channelInfo = ChannelManager.GetChannelInfo(siteId, siteId);
            var orderString = GetOrderString(channelInfo, AttackUtils.FilterSql(parameters.OrderBy));
            var dbArgs = new Dictionary<string, object>();

            if (parameters.QueryString != null && parameters.QueryString.Count > 0)
            {
                var columnNameList = TableColumnManager.GetTableColumnNameList(tableName);

                foreach (string attributeName in parameters.QueryString)
                {
                    if (!StringUtils.ContainsIgnoreCase(columnNameList, attributeName)) continue;

                    var value = parameters.QueryString[attributeName];

                    if (StringUtils.EqualsIgnoreCase(attributeName, ContentAttribute.IsChecked) || StringUtils.EqualsIgnoreCase(attributeName, ContentAttribute.IsColor) || StringUtils.EqualsIgnoreCase(attributeName, ContentAttribute.IsHot) || StringUtils.EqualsIgnoreCase(attributeName, ContentAttribute.IsRecommend) || StringUtils.EqualsIgnoreCase(attributeName, ContentAttribute.IsTop))
                    {
                        whereString += $" AND {attributeName} = '{TranslateUtils.ToBool(value)}'";
                    }
                    else if (StringUtils.EqualsIgnoreCase(attributeName, ContentAttribute.Id) || StringUtils.EqualsIgnoreCase(attributeName, ContentAttribute.ReferenceId) || StringUtils.EqualsIgnoreCase(attributeName, ContentAttribute.SourceId) || StringUtils.EqualsIgnoreCase(attributeName, ContentAttribute.CheckedLevel))
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

            totalCount = DataProvider.DatabaseDao.GetPageTotalCount(tableName, whereString, dbArgs);
            if (totalCount > 0 && parameters.Skip < totalCount)
            {
                var sqlString = DataProvider.DatabaseDao.GetPageSqlString(tableName, MinListColumns, whereString, orderString, parameters.Skip, parameters.Top);

                using (var connection = GetConnection())
                {
                    list = connection.Query<ContentInfo>(sqlString, dbArgs).ToList();
                }
            }

            var tupleList = new List<Tuple<int, int>>();
            foreach (var contentInfo in list)
            {
                tupleList.Add(new Tuple<int, int>(contentInfo.ChannelId, contentInfo.Id));
            }

            return tupleList;
        }

        public List<int> ApiGetContentIdListByChannelId(string tableName, int siteId, int channelId, int top, int skip, string like, string orderby, NameValueCollection queryString, out int totalCount)
        {
            var list = new List<int>();

            var channelInfo = ChannelManager.GetChannelInfo(siteId, channelId);
            var channelIdList = ChannelManager.GetChannelIdList(channelInfo, EScopeType.All, string.Empty, string.Empty, string.Empty);
            var whereString = $"WHERE {ContentAttribute.SiteId} = {siteId} AND {ContentAttribute.ChannelId} IN ({TranslateUtils.ToSqlInStringWithoutQuote(channelIdList)}) AND {ContentAttribute.IsChecked} = '{true}'";

            var likeList = TranslateUtils.StringCollectionToStringList(StringUtils.TrimAndToLower(like));
            var orderString = GetOrderString(channelInfo, AttackUtils.FilterSql(orderby));
            var dbArgs = new Dictionary<string, object>();

            if (queryString != null && queryString.Count > 0)
            {
                var columnNameList = TableColumnManager.GetTableColumnNameList(tableName);

                foreach (string attributeName in queryString)
                {
                    if (!StringUtils.ContainsIgnoreCase(columnNameList, attributeName)) continue;

                    var value = queryString[attributeName];

                    if (StringUtils.EqualsIgnoreCase(attributeName, ContentAttribute.IsChecked) || StringUtils.EqualsIgnoreCase(attributeName, ContentAttribute.IsColor) || StringUtils.EqualsIgnoreCase(attributeName, ContentAttribute.IsHot) || StringUtils.EqualsIgnoreCase(attributeName, ContentAttribute.IsRecommend) || StringUtils.EqualsIgnoreCase(attributeName, ContentAttribute.IsTop))
                    {
                        whereString += $" AND {attributeName} = '{TranslateUtils.ToBool(value)}'";
                    }
                    else if (StringUtils.EqualsIgnoreCase(attributeName, ContentAttribute.Id) || StringUtils.EqualsIgnoreCase(attributeName, ContentAttribute.ReferenceId) || StringUtils.EqualsIgnoreCase(attributeName, ContentAttribute.SourceId) || StringUtils.EqualsIgnoreCase(attributeName, ContentAttribute.CheckedLevel))
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

            totalCount = DataProvider.DatabaseDao.GetPageTotalCount(tableName, whereString, dbArgs);
            if (totalCount > 0 && skip < totalCount)
            {
                var sqlString = DataProvider.DatabaseDao.GetPageSqlString(tableName, MinListColumns, whereString, orderString, skip, top);

                using (var connection = GetConnection())
                {
                    list = connection.Query<int>(sqlString, dbArgs).ToList();
                }
            }

            return list;
        }

        #region Table

        public string GetSelectCommandByHitsAnalysis(string tableName, int siteId)
        {
            var orderByString = ETaxisTypeUtils.GetContentOrderByString(ETaxisType.OrderByTaxisDesc);

            var whereString = new StringBuilder();
            whereString.Append($"AND IsChecked = '{true}' AND SiteId = {siteId} AND Hits > 0");
            whereString.Append(orderByString);

            return DataProvider.DatabaseDao.GetSelectSqlString(tableName, SqlUtils.Asterisk, whereString.ToString());
        }

        public string GetSqlStringOfAdminExcludeRecycle(string tableName, int siteId, DateTime begin, DateTime end)
        {
            var sqlString = $@"select userName,SUM(addCount) as addCount, SUM(updateCount) as updateCount from( 
SELECT AddUserName as userName, Count(AddUserName) as addCount, 0 as updateCount FROM {tableName} 
INNER JOIN {DataProvider.AdministratorDao.TableName} ON AddUserName = {DataProvider.AdministratorDao.TableName}.UserName 
WHERE {tableName}.SiteId = {siteId} AND (({tableName}.ChannelId > 0)) 
AND LastEditDate BETWEEN {SqlUtils.GetComparableDate(begin)} AND {SqlUtils.GetComparableDate(end.AddDays(1))}
GROUP BY AddUserName
Union
SELECT LastEditUserName as userName,0 as addCount, Count(LastEditUserName) as updateCount FROM {tableName} 
INNER JOIN {DataProvider.AdministratorDao.TableName} ON LastEditUserName = {DataProvider.AdministratorDao.TableName}.UserName 
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
                        //whereStringBuilder.Append(
                        //    $" ({ContentAttribute.GroupNameCollection} = '{theGroup.Trim()}' OR CHARINDEX('{theGroup.Trim()},',{ContentAttribute.GroupNameCollection}) > 0 OR CHARINDEX(',{theGroup.Trim()},',{ContentAttribute.GroupNameCollection}) > 0 OR CHARINDEX(',{theGroup.Trim()}',{ContentAttribute.GroupNameCollection}) > 0) OR ");

                        whereStringBuilder.Append(
                                $" ({ContentAttribute.GroupNameCollection} = '{theGroup.Trim()}' OR {SqlUtils.GetInStr(ContentAttribute.GroupNameCollection, theGroup.Trim() + ",")} OR {SqlUtils.GetInStr(ContentAttribute.GroupNameCollection, "," + theGroup.Trim() + ",")} OR {SqlUtils.GetInStr(ContentAttribute.GroupNameCollection, "," + theGroup.Trim())}) OR ");
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
                        //whereStringBuilder.Append(
                        //    $" ({ContentAttribute.GroupNameCollection} <> '{theGroupNot.Trim()}' AND CHARINDEX('{theGroupNot.Trim()},',{ContentAttribute.GroupNameCollection}) = 0 AND CHARINDEX(',{theGroupNot.Trim()},',{ContentAttribute.GroupNameCollection}) = 0 AND CHARINDEX(',{theGroupNot.Trim()}',{ContentAttribute.GroupNameCollection}) = 0) AND ");

                        whereStringBuilder.Append(
                                $" ({ContentAttribute.GroupNameCollection} <> '{theGroupNot.Trim()}' AND {SqlUtils.GetNotInStr(ContentAttribute.GroupNameCollection, theGroupNot.Trim() + ",")} AND {SqlUtils.GetNotInStr(ContentAttribute.GroupNameCollection, "," + theGroupNot.Trim() + ",")} AND {SqlUtils.GetNotInStr(ContentAttribute.GroupNameCollection, "," + theGroupNot.Trim())}) AND ");
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
                var contentIdList = DataProvider.TagDao.GetContentIdListByTagCollection(tagCollection, siteId);
                if (contentIdList.Count > 0)
                {
                    var inString = TranslateUtils.ToSqlInStringWithoutQuote(contentIdList);
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
                    var theSiteId = DataProvider.ChannelDao.GetSiteId(theChannelId);
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

                var theSiteId = DataProvider.ChannelDao.GetSiteId(channelId);
                var channelIdList = ChannelManager.GetChannelIdList(ChannelManager.GetChannelInfo(theSiteId, channelId),
                            EScopeType.All, string.Empty, string.Empty, string.Empty);

                whereBuilder.Append(channelIdList.Count == 1
                    ? $"(ChannelId = {channelIdList[0]}) "
                    : $"(ChannelId IN ({TranslateUtils.ToSqlInStringWithoutQuote(channelIdList)})) ");
            }

            var typeList = new List<string>();
            if (string.IsNullOrEmpty(type))
            {
                typeList.Add(ContentAttribute.Title);
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
                dateAttribute = ContentAttribute.AddDate;
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
                //            whereBuilder.Append($"({ContentAttribute.SettingsXml} LIKE '%{key}={value}%')");
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
                //return DataProvider.DatabaseDao.GetSelectSqlString(tableName, startNum, totalNum, MinListColumns, sqlWhereString, orderByString);
                return DataProvider.DatabaseDao.GetPageSqlString(tableName, MinListColumns, sqlWhereString, orderByString, startNum - 1, totalNum);
            }
            return string.Empty;
        }

        public string GetStlSqlStringCheckedBySearch(string tableName, int startNum, int totalNum, string orderByString, string whereString)
        {
            var sqlWhereString =
                    $"WHERE (ChannelId > 0 AND IsChecked = '{true}' {whereString})";

            if (!string.IsNullOrEmpty(tableName))
            {
                //return DataProvider.DatabaseDao.GetSelectSqlString(tableName, startNum, totalNum, TranslateUtils.ObjectCollectionToString(ContentAttribute.AllAttributes.Value), sqlWhereString, orderByString);
                return DataProvider.DatabaseDao.GetPageSqlString(tableName, TranslateUtils.ObjectCollectionToString(ContentAttribute.AllAttributes.Value), sqlWhereString, orderByString, startNum - 1, totalNum);
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
                    ? $" AND {BackgroundContentAttribute.ImageUrl} <> '' "
                    : $" AND {BackgroundContentAttribute.ImageUrl} = '' ");
            }

            if (isVideoExists)
            {
                whereBuilder.Append(isVideo
                    ? $" AND {BackgroundContentAttribute.VideoUrl} <> '' "
                    : $" AND {BackgroundContentAttribute.VideoUrl} = '' ");
            }

            if (isFileExists)
            {
                whereBuilder.Append(isFile
                    ? $" AND {BackgroundContentAttribute.FileUrl} <> '' "
                    : $" AND {BackgroundContentAttribute.FileUrl} = '' ");
            }

            if (isTopExists)
            {
                whereBuilder.Append($" AND {ContentAttribute.IsTop} = '{isTop}' ");
            }

            if (isRecommendExists)
            {
                whereBuilder.Append($" AND {ContentAttribute.IsRecommend} = '{isRecommend}' ");
            }

            if (isHotExists)
            {
                whereBuilder.Append($" AND {ContentAttribute.IsHot} = '{isHot}' ");
            }

            if (isColorExists)
            {
                whereBuilder.Append($" AND {ContentAttribute.IsColor} = '{isColor}' ");
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
                        //whereBuilder.Append(
                        //    $" ({ContentAttribute.GroupNameCollection} = '{trimGroup}' OR CHARINDEX('{trimGroup},',{ContentAttribute.GroupNameCollection}) > 0 OR CHARINDEX(',{trimGroup},',{ContentAttribute.GroupNameCollection}) > 0 OR CHARINDEX(',{trimGroup}',{ContentAttribute.GroupNameCollection}) > 0) OR ");

                        whereBuilder.Append(
                                $" ({ContentAttribute.GroupNameCollection} = '{trimGroup}' OR {SqlUtils.GetInStr(ContentAttribute.GroupNameCollection, trimGroup + ",")} OR {SqlUtils.GetInStr(ContentAttribute.GroupNameCollection, "," + trimGroup + ",")} OR {SqlUtils.GetInStr(ContentAttribute.GroupNameCollection, "," + trimGroup)}) OR ");
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
                        //whereBuilder.Append(
                        //    $" ({ContentAttribute.GroupNameCollection} <> '{trimGroup}' AND CHARINDEX('{trimGroup},',{ContentAttribute.GroupNameCollection}) = 0 AND CHARINDEX(',{trimGroup},',{ContentAttribute.GroupNameCollection}) = 0 AND CHARINDEX(',{trimGroup}',{ContentAttribute.GroupNameCollection}) = 0) AND ");

                        whereBuilder.Append(
                                $" ({ContentAttribute.GroupNameCollection} <> '{trimGroup}' AND {SqlUtils.GetNotInStr(ContentAttribute.GroupNameCollection, trimGroup + ",")} AND {SqlUtils.GetNotInStr(ContentAttribute.GroupNameCollection, "," + trimGroup + ",")} AND {SqlUtils.GetNotInStr(ContentAttribute.GroupNameCollection, "," + trimGroup)}) AND ");
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
                var contentIdArrayList = DataProvider.TagDao.GetContentIdListByTagCollection(tagCollection, siteId);
                if (contentIdArrayList.Count > 0)
                {
                    whereBuilder.Append(
                        $" AND (ID IN ({TranslateUtils.ToSqlInStringWithoutQuote(contentIdArrayList)}))");
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
                    ? $" AND {BackgroundContentAttribute.ImageUrl} <> '' "
                    : $" AND {BackgroundContentAttribute.ImageUrl} = '' ");
            }

            if (isVideoExists)
            {
                whereBuilder.Append(isVideo
                    ? $" AND {BackgroundContentAttribute.VideoUrl} <> '' "
                    : $" AND {BackgroundContentAttribute.VideoUrl} = '' ");
            }

            if (isFileExists)
            {
                whereBuilder.Append(isFile
                    ? $" AND {BackgroundContentAttribute.FileUrl} <> '' "
                    : $" AND {BackgroundContentAttribute.FileUrl} = '' ");
            }

            if (isTopExists)
            {
                whereBuilder.Append($" AND {ContentAttribute.IsTop} = '{isTop}' ");
            }

            if (isRecommendExists)
            {
                whereBuilder.Append($" AND {ContentAttribute.IsRecommend} = '{isRecommend}' ");
            }

            if (isHotExists)
            {
                whereBuilder.Append($" AND {ContentAttribute.IsHot} = '{isHot}' ");
            }

            if (isColorExists)
            {
                whereBuilder.Append($" AND {ContentAttribute.IsColor} = '{isColor}' ");
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
                        //whereBuilder.Append(
                        //    $" ({ContentAttribute.GroupNameCollection} = '{trimGroup}' OR CHARINDEX('{trimGroup},',{ContentAttribute.GroupNameCollection}) > 0 OR CHARINDEX(',{trimGroup},',{ContentAttribute.GroupNameCollection}) > 0 OR CHARINDEX(',{trimGroup}',{ContentAttribute.GroupNameCollection}) > 0) OR ");

                        whereBuilder.Append(
                                $" ({ContentAttribute.GroupNameCollection} = '{trimGroup}' OR {SqlUtils.GetInStr(ContentAttribute.GroupNameCollection, trimGroup + ",")} OR {SqlUtils.GetInStr(ContentAttribute.GroupNameCollection, "," + trimGroup + ",")} OR {SqlUtils.GetInStr(ContentAttribute.GroupNameCollection, "," + trimGroup)}) OR ");
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
                        //whereBuilder.Append(
                        //    $" ({ContentAttribute.GroupNameCollection} <> '{trimGroup}' AND CHARINDEX('{trimGroup},',{ContentAttribute.GroupNameCollection}) = 0 AND CHARINDEX(',{trimGroup},',{ContentAttribute.GroupNameCollection}) = 0 AND CHARINDEX(',{trimGroup}',{ContentAttribute.GroupNameCollection}) = 0) AND ");

                        whereBuilder.Append(
                                $" ({ContentAttribute.GroupNameCollection} <> '{trimGroup}' AND {SqlUtils.GetNotInStr(ContentAttribute.GroupNameCollection, trimGroup + ",")} AND {SqlUtils.GetNotInStr(ContentAttribute.GroupNameCollection, "," + trimGroup + ",")} AND {SqlUtils.GetNotInStr(ContentAttribute.GroupNameCollection, "," + trimGroup)}) AND ");
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
            var whereString = new StringBuilder($"WHERE {nameof(ContentAttribute.SourceId)} != {SourceManager.Preview} AND ");

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

            if (StringUtils.EqualsIgnoreCase(searchType, ContentAttribute.IsTop) || StringUtils.EqualsIgnoreCase(searchType, ContentAttribute.IsRecommend) || StringUtils.EqualsIgnoreCase(searchType, ContentAttribute.IsColor) || StringUtils.EqualsIgnoreCase(searchType, ContentAttribute.IsHot))
            {
                if (!string.IsNullOrEmpty(keyword))
                {
                    whereString.Append($"AND ({ContentAttribute.Title} LIKE '%{keyword}%') ");
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
                whereString.Append($" AND {ContentAttribute.AddUserName} = '{userNameOnly}' ");
            }
            if (isWritingOnly)
            {
                whereString.Append($" AND {ContentAttribute.UserId} > 0 ");
            }

            whereString.Append(" ").Append(orderByString);

            return DataProvider.DatabaseDao.GetSelectSqlString(tableName, SqlUtils.Asterisk, whereString.ToString());
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
                $"{nameof(ContentAttribute.SiteId)} = {siteInfo.Id}",
                $"{nameof(ContentAttribute.SourceId)} != {SourceManager.Preview}"
            };

            if (!string.IsNullOrEmpty(dateFrom))
            {
                whereList.Add($"{nameof(ContentAttribute.AddDate)} >= {SqlUtils.GetComparableDate(TranslateUtils.ToDateTime(dateFrom))}");
            }
            if (!string.IsNullOrEmpty(dateTo))
            {
                whereList.Add($"{nameof(ContentAttribute.AddDate)} <= {SqlUtils.GetComparableDate(TranslateUtils.ToDateTime(dateTo).AddDays(1))}");
            }

            if (isAllChannels)
            {
                whereList.Add(isTrashOnly
                    ? $"{nameof(ContentAttribute.ChannelId)} < 0"
                    : $"{nameof(ContentAttribute.ChannelId)} > 0");
            }
            else if (searchChannelIdList.Count == 0)
            {
                whereList.Add($"{nameof(ContentAttribute.ChannelId)} = 0");
            }
            else if (searchChannelIdList.Count == 1)
            {
                whereList.Add($"{nameof(ContentAttribute.ChannelId)} = {channelInfo.Id}");
            }
            else
            {
                whereList.Add($"{nameof(ContentAttribute.ChannelId)} IN ({TranslateUtils.ToSqlInStringWithoutQuote(searchChannelIdList)})");
            }

            if (StringUtils.EqualsIgnoreCase(searchType, ContentAttribute.IsTop) || StringUtils.EqualsIgnoreCase(searchType, ContentAttribute.IsRecommend) || StringUtils.EqualsIgnoreCase(searchType, ContentAttribute.IsColor) || StringUtils.EqualsIgnoreCase(searchType, ContentAttribute.IsHot))
            {
                if (!string.IsNullOrEmpty(keyword))
                {
                    whereList.Add($"{ContentAttribute.Title} LIKE '%{keyword}%'");
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
                //    : $"{nameof(ContentAttribute.SettingsXml)} LIKE '%{searchType}={keyword}%'");
            }

            if (isCheckOnly)
            {
                whereList.Add(checkLevel == CheckManager.LevelInt.All
                    ? $"{nameof(ContentAttribute.IsChecked)} = '{false}'"
                    : $"{nameof(ContentAttribute.IsChecked)} = '{false}' AND {nameof(ContentAttribute.CheckedLevel)} = {checkLevel}");
            }
            else
            {
                if (checkLevel != CheckManager.LevelInt.All)
                {
                    whereList.Add(checkLevel == siteInfo.Additional.CheckContentLevel
                        ? $"{nameof(ContentAttribute.IsChecked)} = '{true}'"
                        : $"{nameof(ContentAttribute.IsChecked)} = '{false}' AND {nameof(ContentAttribute.CheckedLevel)} = {checkLevel}");
                }
            }

            if (onlyAdminId.HasValue)
            {
                whereList.Add($"{nameof(ContentAttribute.AdminId)} = {onlyAdminId.Value}");
            }

            if (isWritingOnly)
            {
                whereList.Add($"{nameof(ContentAttribute.UserId)} > 0");
            }

            return $"WHERE {string.Join(" AND ", whereList)}";
        }

        public string GetPagerWhereSqlString(int channelId, ETriState checkedState, string userNameOnly)
        {
            var whereString = new StringBuilder();
            whereString.Append($"WHERE {nameof(ContentAttribute.ChannelId)} = {channelId} AND {nameof(ContentAttribute.SourceId)} != {SourceManager.Preview} ");

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

        private string GetCreateContentTableSqlString(string tableName, List<TableColumn> tableColumns)
        {
            var sqlBuilder = new StringBuilder(DataProvider.DatabaseDao.GetCreateTableSqlString(tableName, tableColumns));
            sqlBuilder.AppendLine().Append("GO");

            if (WebConfigUtils.DatabaseType == DatabaseType.MySql)
            {
                sqlBuilder.Append($@"
CREATE INDEX `IX_{tableName}` ON `{tableName}`(`{nameof(ContentInfo.IsTop)}` DESC, `{nameof(ContentInfo.Taxis)}` DESC, `{nameof(ContentInfo.Id)}` DESC)
GO
CREATE INDEX `IX_{tableName}_Taxis` ON `{tableName}`(`{nameof(ContentInfo.Taxis)}` DESC)
GO");
            }
            else
            {
                sqlBuilder.Append($@"
CREATE INDEX IX_{tableName} ON {tableName}({nameof(ContentInfo.IsTop)} DESC, {nameof(ContentInfo.Taxis)} DESC, {nameof(ContentInfo.Id)} DESC)
GO
CREATE INDEX IX_{tableName}_Taxis ON {tableName}({nameof(ContentInfo.Taxis)} DESC)
GO");
            }

            return sqlBuilder.ToString();
        }

        public void CreateContentTable(string tableName, List<TableColumn> columnInfoList)
        {
            var isDbExists = DataProvider.DatabaseDao.IsTableExists(tableName);
            if (isDbExists) return;

            var createTableSqlString = GetCreateContentTableSqlString(tableName, columnInfoList);

            using (var conn = GetConnection())
            {
                conn.Open();
                using (var trans = conn.BeginTransaction())
                {
                    try
                    {
                        var reader = new System.IO.StringReader(createTableSqlString);
                        string sql;
                        while (null != (sql = SqlUtils.ReadNextStatementFromStream(reader)))
                        {
                            ExecuteNonQuery(trans, sql.Trim());
                        }

                        TableColumnManager.ClearCache();

                        trans.Commit();
                    }
                    catch
                    {
                        trans.Rollback();
                        throw;
                    }
                }
            }
        }

        #endregion

        #region Cache

        //private string GetCacheWhereString(SiteInfo siteInfo, ChannelInfo channelInfo)
        //{
        //    return $"WHERE {nameof(ContentInfo.SiteId)} = {siteInfo.Id} AND {nameof(ContentInfo.ChannelId)} = {channelInfo.Id} AND {nameof(ContentAttribute.SourceId)} != {SourceManager.Preview}";
        //}

        public string GetCacheWhereString(SiteInfo siteInfo, ChannelInfo channelInfo, int? onlyAdminId)
        {
            var whereString = $"WHERE {nameof(ContentInfo.SiteId)} = {siteInfo.Id} AND {nameof(ContentInfo.ChannelId)} = {channelInfo.Id} AND {nameof(ContentAttribute.SourceId)} != {SourceManager.Preview}";
            if (onlyAdminId.HasValue)
            {
                whereString += $" AND {nameof(ContentAttribute.AdminId)} = {onlyAdminId.Value}";
            }

            return whereString;
        }

        public string GetOrderString(ChannelInfo channelInfo, string orderBy)
        {
            return ETaxisTypeUtils.GetContentOrderByString(ETaxisTypeUtils.GetEnumType(channelInfo.Additional.DefaultTaxisType), orderBy);
        }

        public List<ContentInfo> GetCacheContentInfoList(SiteInfo siteInfo, ChannelInfo channelInfo, int? onlyAdminId, int offset, int limit)
        {
            var tableName = ChannelManager.GetTableName(siteInfo, channelInfo);

            return GetContentInfoList(tableName, GetCacheWhereString(siteInfo, channelInfo, onlyAdminId),
                GetOrderString(channelInfo, string.Empty), offset, limit);
        }

        public List<ContentInfo> GetContentInfoList(string tableName, string whereString, string orderString, int offset, int limit)
        {
            var list = new List<ContentInfo>();

            var sqlString = DataProvider.DatabaseDao.GetPageSqlString(tableName, SqlUtils.Asterisk, whereString, orderString, offset, limit);

            using (var rdr = ExecuteReader(sqlString))
            {
                while (rdr.Read())
                {
                    var contentInfo = new ContentInfo(rdr);

                    list.Add(contentInfo);
                }
                rdr.Close();
            }

            return list;
        }

        public List<int> GetCacheContentIdList(SiteInfo siteInfo, ChannelInfo channelInfo, int? onlyAdminId, int offset, int limit)
        {
            var tableName = ChannelManager.GetTableName(siteInfo, channelInfo);

            return GetCacheContentIdList(tableName, GetCacheWhereString(siteInfo, channelInfo, onlyAdminId),
                GetOrderString(channelInfo, string.Empty), offset, limit);
        }

        public List<int> GetCacheContentIdList(string tableName, string whereString, string orderString, int offset, int limit)
        {
            var list = new List<int>();

            var sqlString = DataProvider.DatabaseDao.GetPageSqlString(tableName, MinListColumns, whereString, orderString, offset, limit);

            using (var rdr = ExecuteReader(sqlString))
            {
                while (rdr.Read())
                {
                    var contentId = GetInt(rdr, 0);

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

            var sqlWhere = $"WHERE {ContentAttribute.ChannelId} = {channelId} AND {ContentAttribute.Id} = {contentId}";
            var sqlSelect = DataProvider.DatabaseDao.GetSelectSqlString(tableName, SqlUtils.Asterisk, sqlWhere);

            using (var rdr = ExecuteReader(sqlSelect))
            {
                if (rdr.Read())
                {
                    contentInfo = new ContentInfo(rdr);
                }
                rdr.Close();
            }

            return contentInfo;
        }

        #endregion
    }
}
