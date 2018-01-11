using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Text;
using BaiRong.Core;
using BaiRong.Core.Data;
using BaiRong.Core.Model;
using BaiRong.Core.Model.Attributes;
using BaiRong.Core.Model.Enumerations;
using BaiRong.Core.Table;
using SiteServer.CMS.Core;
using SiteServer.CMS.Core.Security;
using SiteServer.CMS.Model;
using SiteServer.CMS.StlParser.Cache;
using SiteServer.Plugin.Models;

namespace SiteServer.CMS.Provider
{
    public class ContentDao : DataProviderBase
    {
        public const int TaxisMaxValue = 2147483647;
        public const int TaxisIsTopStartValue = 2147480000;
        public const string SortFieldName = nameof(ContentAttribute.Taxis);

        public string StlColumns => $"{ContentAttribute.Id}, {ContentAttribute.NodeId}, {ContentAttribute.IsTop}, {ContentAttribute.AddDate}, {ContentAttribute.LastEditDate}, {ContentAttribute.Taxis}, {ContentAttribute.Hits}, {ContentAttribute.HitsByDay}, {ContentAttribute.HitsByWeek}, {ContentAttribute.HitsByMonth}";

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        public void SetValue(string tableName, int contentId, string name, string value)
        {
            string sqlString = $"UPDATE {tableName} SET {name} = '{value}' WHERE Id = {contentId}";

            ExecuteNonQuery(sqlString);

            Content.ClearCache();
        }

        public void UpdateIsChecked(string tableName, int publishmentSystemId, int nodeId, List<int> contentIdList, int translateNodeId, bool isAdmin, string userName, bool isChecked, int checkedLevel, string reasons)
        {
            if (isChecked)
            {
                checkedLevel = 0;
            }

            var checkDate = DateTime.Now;

            foreach (var contentId in contentIdList)
            {
                var settingsXml = GetValue(tableName, contentId, ContentAttribute.SettingsXml);
                var attributes = TranslateUtils.ToNameValueCollection(settingsXml);
                attributes[ContentAttribute.CheckIsAdmin] = isAdmin.ToString();
                attributes[ContentAttribute.CheckUserName] = userName;
                attributes[ContentAttribute.CheckCheckDate] = DateUtils.GetDateAndTimeString(checkDate);
                attributes[ContentAttribute.CheckReasons] = reasons;

                string sqlString =
                    $"UPDATE {tableName} SET IsChecked = '{isChecked}', CheckedLevel = {checkedLevel}, SettingsXML = '{TranslateUtils.NameValueCollectionToString(attributes)}' WHERE Id = {contentId}";
                if (translateNodeId > 0)
                {
                    sqlString =
                        $"UPDATE {tableName} SET IsChecked = '{isChecked}', CheckedLevel = {checkedLevel}, SettingsXML = '{TranslateUtils.NameValueCollectionToString(attributes)}', NodeId = {translateNodeId} WHERE Id = {contentId}";
                }
                ExecuteNonQuery(sqlString);

                var checkInfo = new ContentCheckInfo(0, tableName, publishmentSystemId, nodeId, contentId, isAdmin, userName, isChecked, checkedLevel, checkDate, reasons);
                BaiRongDataProvider.ContentCheckDao.Insert(checkInfo);
            }

            Content.ClearCache();
        }

        public void AddHits(string tableName, bool isCountHits, bool isCountHitsByDay, int contentId)
        {
            if (contentId <= 0 || !isCountHits) return;

            if (isCountHitsByDay)
            {
                var referenceId = 0;
                var hitsByDay = 0;
                var hitsByWeek = 0;
                var hitsByMonth = 0;
                var lastHitsDate = DateTime.Now;

                string sqlString =
                    $"SELECT ReferenceId, HitsByDay, HitsByWeek, HitsByMonth, LastHitsDate FROM {tableName} WHERE (Id = {contentId})";

                using (var rdr = ExecuteReader(sqlString))
                {
                    if (rdr.Read())
                    {
                        var i = 0;
                        referenceId = GetInt(rdr, i++);
                        hitsByDay = GetInt(rdr, i++);
                        hitsByWeek = GetInt(rdr, i++);
                        hitsByMonth = GetInt(rdr, i++);
                        lastHitsDate = GetDateTime(rdr, i);
                    }
                    rdr.Close();
                }

                if (referenceId > 0)
                {
                    contentId = referenceId;
                }

                var now = DateTime.Now;

                hitsByDay = now.Day != lastHitsDate.Day || now.Month != lastHitsDate.Month || now.Year != lastHitsDate.Year ? 1 : hitsByDay + 1;
                hitsByWeek = now.Month != lastHitsDate.Month || now.Year != lastHitsDate.Year || now.DayOfYear / 7 != lastHitsDate.DayOfYear / 7 ? 1 : hitsByWeek + 1;
                hitsByMonth = now.Month != lastHitsDate.Month || now.Year != lastHitsDate.Year ? 1 : hitsByMonth + 1;

                sqlString =
                    $"UPDATE {tableName} SET {SqlUtils.ToPlusSqlString("Hits")}, HitsByDay = {hitsByDay}, HitsByWeek = {hitsByWeek}, HitsByMonth = {hitsByMonth}, LastHitsDate = '{DateUtils.GetDateAndTimeString(DateTime.Now)}' WHERE Id = {contentId}  AND ReferenceId = 0";
                ExecuteNonQuery(sqlString);
            }
            else
            {
                string sqlString =
                    $"UPDATE {tableName} SET {SqlUtils.ToPlusSqlString("Hits")}, LastHitsDate = '{DateUtils.GetDateAndTimeString(DateTime.Now)}' WHERE Id = {contentId} AND ReferenceId = 0";
                var count = ExecuteNonQuery(sqlString);
                if (count < 1)
                {
                    var referenceId = 0;

                    sqlString = $"SELECT ReferenceId FROM {tableName} WHERE (Id = {contentId})";

                    using (var rdr = ExecuteReader(sqlString))
                    {
                        if (rdr.Read())
                        {
                            referenceId = GetInt(rdr, 0);
                        }
                        rdr.Close();
                    }

                    if (referenceId > 0)
                    {
                        sqlString =
                            $"UPDATE {tableName} SET {SqlUtils.ToPlusSqlString("Hits")}, LastHitsDate = '{DateUtils.GetDateAndTimeString(DateTime.Now)}' WHERE Id = {referenceId} AND ReferenceId = 0";
                        ExecuteNonQuery(sqlString);
                    }
                }
            }

            Content.ClearCache();
        }

        public void UpdateComments(string tableName, int contentId, int comments)
        {
            string sqlString = $"UPDATE {tableName} SET Comments = {comments} WHERE Id = {contentId}";
            ExecuteNonQuery(sqlString);

            Content.ClearCache();
        }

        public void DeleteContentsByTrash(int publishmentSystemId, string tableName)
        {
            if (string.IsNullOrEmpty(tableName)) return;

            var contentIdList = GetContentIdListByTrash(publishmentSystemId, tableName);
            TagUtils.RemoveTags(publishmentSystemId, contentIdList);

            string sqlString =
                $"DELETE FROM {tableName} WHERE PublishmentSystemId = {publishmentSystemId} AND NodeId < 0";
            ExecuteNonQuery(sqlString);

            Content.ClearCache();
        }

        private int Insert(string tableName, IContentInfo contentInfo)
        {
            //var contentId = 0;

            if (string.IsNullOrEmpty(tableName) || contentInfo == null) return 0;

            contentInfo.LastEditDate = DateTime.Now;

            var metadataInfoList = TableMetadataManager.GetTableMetadataInfoList(tableName);

            var names = new StringBuilder();
            var values = new StringBuilder();
            var paras = new List<IDataParameter>();
            var lowerCaseExcludeAttributesNames = new List<string>(ContentAttribute.AllAttributesLowercase);
            foreach (var metadataInfo in metadataInfoList)
            {
                lowerCaseExcludeAttributesNames.Add(metadataInfo.AttributeName.ToLower());
                names.Append($",{metadataInfo.AttributeName}").AppendLine();
                values.Append($",@{metadataInfo.AttributeName}").AppendLine();
                if (metadataInfo.DataType == DataType.Integer)
                {
                    paras.Add(GetParameter($"@{metadataInfo.AttributeName}", metadataInfo.DataType, contentInfo.GetInt(metadataInfo.AttributeName)));
                }
                else if (metadataInfo.DataType == DataType.Decimal)
                {
                    paras.Add(GetParameter($"@{metadataInfo.AttributeName}", metadataInfo.DataType, contentInfo.GetDecimal(metadataInfo.AttributeName)));
                }
                else if (metadataInfo.DataType == DataType.Boolean)
                {
                    paras.Add(GetParameter($"@{metadataInfo.AttributeName}", metadataInfo.DataType, contentInfo.GetBool(metadataInfo.AttributeName)));
                }
                else if (metadataInfo.DataType == DataType.DateTime)
                {
                    paras.Add(GetParameter($"@{metadataInfo.AttributeName}", metadataInfo.DataType, contentInfo.GetDateTime(metadataInfo.AttributeName, DateTime.Now)));
                }
                else
                {
                    paras.Add(GetParameter($"@{metadataInfo.AttributeName}", metadataInfo.DataType, contentInfo.GetString(metadataInfo.AttributeName)));
                }
            }

            var sqlString = $@"
INSERT INTO {tableName} (
    {nameof(ContentInfo.NodeId)},
    {nameof(ContentInfo.PublishmentSystemId)},
    {nameof(ContentInfo.AddUserName)},
    {nameof(ContentInfo.LastEditUserName)},
    {nameof(ContentInfo.WritingUserName)},
    {nameof(ContentInfo.LastEditDate)},
    {nameof(ContentInfo.Taxis)},
    {nameof(ContentInfo.ContentGroupNameCollection)},
    {nameof(ContentInfo.Tags)},
    {nameof(ContentInfo.SourceId)},
    {nameof(ContentInfo.ReferenceId)},
    {nameof(ContentInfo.IsChecked)},
    {nameof(ContentInfo.CheckedLevel)},
    {nameof(ContentInfo.Comments)},
    {nameof(ContentInfo.Photos)},
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
    @{nameof(ContentInfo.NodeId)},
    @{nameof(ContentInfo.PublishmentSystemId)},
    @{nameof(ContentInfo.AddUserName)},
    @{nameof(ContentInfo.LastEditUserName)},
    @{nameof(ContentInfo.WritingUserName)},
    @{nameof(ContentInfo.LastEditDate)},
    @{nameof(ContentInfo.Taxis)},
    @{nameof(ContentInfo.ContentGroupNameCollection)},
    @{nameof(ContentInfo.Tags)},
    @{nameof(ContentInfo.SourceId)},
    @{nameof(ContentInfo.ReferenceId)},
    @{nameof(ContentInfo.IsChecked)},
    @{nameof(ContentInfo.CheckedLevel)},
    @{nameof(ContentInfo.Comments)},
    @{nameof(ContentInfo.Photos)},
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
                GetParameter($"@{nameof(ContentInfo.NodeId)}", DataType.Integer, contentInfo.NodeId),
                GetParameter($"@{nameof(ContentInfo.PublishmentSystemId)}", DataType.Integer, contentInfo.PublishmentSystemId),
                GetParameter($"@{nameof(ContentInfo.AddUserName)}", DataType.VarChar, 255, contentInfo.AddUserName),
                GetParameter($"@{nameof(ContentInfo.LastEditUserName)}", DataType.VarChar, 255, contentInfo.LastEditUserName),
                GetParameter($"@{nameof(ContentInfo.WritingUserName)}", DataType.VarChar, 255, contentInfo.WritingUserName),
                GetParameter($"@{nameof(ContentInfo.LastEditDate)}", DataType.DateTime, contentInfo.LastEditDate),
                GetParameter($"@{nameof(ContentInfo.Taxis)}", DataType.Integer, contentInfo.Taxis),
                GetParameter($"@{nameof(ContentInfo.ContentGroupNameCollection)}", DataType.VarChar, 255, contentInfo.ContentGroupNameCollection),
                GetParameter($"@{nameof(ContentInfo.Tags)}", DataType.VarChar, 255, contentInfo.Tags),
                GetParameter($"@{nameof(ContentInfo.SourceId)}", DataType.Integer, contentInfo.SourceId),
                GetParameter($"@{nameof(ContentInfo.ReferenceId)}", DataType.Integer, contentInfo.ReferenceId),
                GetParameter($"@{nameof(ContentInfo.IsChecked)}", DataType.VarChar, 18, contentInfo.IsChecked.ToString()),
                GetParameter($"@{nameof(ContentInfo.CheckedLevel)}", DataType.Integer, contentInfo.CheckedLevel),
                GetParameter($"@{nameof(ContentInfo.Comments)}", DataType.Integer, contentInfo.Comments),
                GetParameter($"@{nameof(ContentInfo.Photos)}", DataType.Integer, contentInfo.Photos),
                GetParameter($"@{nameof(ContentInfo.Hits)}", DataType.Integer, contentInfo.Hits),
                GetParameter($"@{nameof(ContentInfo.HitsByDay)}", DataType.Integer, contentInfo.HitsByDay),
                GetParameter($"@{nameof(ContentInfo.HitsByWeek)}", DataType.Integer, contentInfo.HitsByWeek),
                GetParameter($"@{nameof(ContentInfo.HitsByMonth)}", DataType.Integer, contentInfo.HitsByMonth),
                GetParameter($"@{nameof(ContentInfo.LastHitsDate)}", DataType.DateTime, contentInfo.LastHitsDate),
                GetParameter($"@{nameof(ContentInfo.SettingsXml)}", DataType.Text, contentInfo.ToString(lowerCaseExcludeAttributesNames)),
                GetParameter($"@{nameof(ContentInfo.Title)}", DataType.VarChar, 255, contentInfo.Title),
                GetParameter($"@{nameof(ContentInfo.IsTop)}", DataType.VarChar, 18, contentInfo.IsTop.ToString()),
                GetParameter($"@{nameof(ContentInfo.IsRecommend)}", DataType.VarChar, 18, contentInfo.IsRecommend.ToString()),
                GetParameter($"@{nameof(ContentInfo.IsHot)}", DataType.VarChar, 18, contentInfo.IsHot.ToString()),
                GetParameter($"@{nameof(ContentInfo.IsColor)}", DataType.VarChar, 18, contentInfo.IsColor.ToString()),
                GetParameter($"@{nameof(ContentInfo.LinkUrl)}", DataType.VarChar, 200, contentInfo.LinkUrl),
                GetParameter($"@{nameof(ContentInfo.AddDate)}", DataType.DateTime, contentInfo.AddDate)
            };
            parameters.AddRange(paras);

            //IDataParameter[] parms;
            //var sqlInsert = BaiRongDataProvider.DatabaseDao.GetInsertSqlString(contentInfo.Attributes.GetExtendedAttributes(), tableName, out parms);

            var contentId = ExecuteNonQueryAndReturnId(tableName, nameof(ContentInfo.Id), sqlString, parameters.ToArray());

            Content.ClearCache();

            return contentId;

            //using (var conn = GetConnection())
            //{
            //    conn.Open();
            //    using (var trans = conn.BeginTransaction())
            //    {
            //        try
            //        {
            //            //contentId = ExecuteNonQueryAndReturnId(trans, sqlInsert, parms);
            //            contentId = ExecuteNonQueryAndReturningId(trans, sqlString, nameof(ContentInfo.Id), parameters.ToArray());

            //            trans.Commit();
            //        }
            //        catch
            //        {
            //            trans.Rollback();
            //            throw;
            //        }
            //    }
            //}

            //return contentId;
        }

        private void Update(string tableName, IContentInfo contentInfo)
        {
            if (string.IsNullOrEmpty(tableName) || contentInfo == null) return;

            //出现IsTop与Taxis不同步情况
            if (contentInfo.IsTop == false && contentInfo.Taxis >= TaxisIsTopStartValue)
            {
                contentInfo.Taxis = GetMaxTaxis(tableName, contentInfo.NodeId, false) + 1;
            }
            else if (contentInfo.IsTop && contentInfo.Taxis < TaxisIsTopStartValue)
            {
                contentInfo.Taxis = GetMaxTaxis(tableName, contentInfo.NodeId, true) + 1;
            }

            contentInfo.LastEditDate = DateTime.Now;

            //if (!string.IsNullOrEmpty(tableName))
            //{
            //    contentInfo.Attributes.BeforeExecuteNonQuery();
            //    sqlString = BaiRongDataProvider.DatabaseDao.GetUpdateSqlString(contentInfo.Attributes.GetExtendedAttributes(), tableName, out parms);
            //}

            var metadataInfoList = TableMetadataManager.GetTableMetadataInfoList(tableName);

            var sets = new StringBuilder();
            var paras = new List<IDataParameter>();
            var lowerCaseExcludeAttributesNames = new List<string>(ContentAttribute.AllAttributesLowercase);
            foreach (var metadataInfo in metadataInfoList)
            {
                lowerCaseExcludeAttributesNames.Add(metadataInfo.AttributeName.ToLower());
                sets.Append($",{metadataInfo.AttributeName} = @{metadataInfo.AttributeName}").AppendLine();
                if (metadataInfo.DataType == DataType.Integer)
                {
                    paras.Add(GetParameter($"@{metadataInfo.AttributeName}", metadataInfo.DataType, contentInfo.GetInt(metadataInfo.AttributeName)));
                }
                else if (metadataInfo.DataType == DataType.Decimal)
                {
                    paras.Add(GetParameter($"@{metadataInfo.AttributeName}", metadataInfo.DataType, contentInfo.GetDecimal(metadataInfo.AttributeName)));
                }
                else if (metadataInfo.DataType == DataType.Boolean)
                {
                    paras.Add(GetParameter($"@{metadataInfo.AttributeName}", metadataInfo.DataType, contentInfo.GetBool(metadataInfo.AttributeName)));
                }
                else if (metadataInfo.DataType == DataType.DateTime)
                {
                    paras.Add(GetParameter($"@{metadataInfo.AttributeName}", metadataInfo.DataType, contentInfo.GetDateTime(metadataInfo.AttributeName, DateTime.Now)));
                }
                else
                {
                    paras.Add(GetParameter($"@{metadataInfo.AttributeName}", metadataInfo.DataType, contentInfo.GetString(metadataInfo.AttributeName)));
                }
            }

            var sqlString = $@"
UPDATE {tableName} SET 
    {nameof(ContentInfo.NodeId)} = @{nameof(ContentInfo.NodeId)},
    {nameof(ContentInfo.PublishmentSystemId)} = @{nameof(ContentInfo.PublishmentSystemId)},
    {nameof(ContentInfo.AddUserName)} = @{nameof(ContentInfo.AddUserName)},
    {nameof(ContentInfo.LastEditUserName)} = @{nameof(ContentInfo.LastEditUserName)},
    {nameof(ContentInfo.WritingUserName)} = @{nameof(ContentInfo.WritingUserName)},
    {nameof(ContentInfo.LastEditDate)} = @{nameof(ContentInfo.LastEditDate)},
    {nameof(ContentInfo.Taxis)} = @{nameof(ContentInfo.Taxis)},
    {nameof(ContentInfo.ContentGroupNameCollection)} = @{nameof(ContentInfo.ContentGroupNameCollection)},
    {nameof(ContentInfo.Tags)} = @{nameof(ContentInfo.Tags)},
    {nameof(ContentInfo.SourceId)} = @{nameof(ContentInfo.SourceId)},
    {nameof(ContentInfo.ReferenceId)} = @{nameof(ContentInfo.ReferenceId)},
    {nameof(ContentInfo.IsChecked)} = @{nameof(ContentInfo.IsChecked)},
    {nameof(ContentInfo.CheckedLevel)} = @{nameof(ContentInfo.CheckedLevel)},
    {nameof(ContentInfo.Comments)} = @{nameof(ContentInfo.Comments)},
    {nameof(ContentInfo.Photos)} = @{nameof(ContentInfo.Photos)},
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
                GetParameter($"@{nameof(ContentInfo.NodeId)}", DataType.Integer, contentInfo.NodeId),
                GetParameter($"@{nameof(ContentInfo.PublishmentSystemId)}", DataType.Integer, contentInfo.PublishmentSystemId),
                GetParameter($"@{nameof(ContentInfo.AddUserName)}", DataType.VarChar, 255, contentInfo.AddUserName),
                GetParameter($"@{nameof(ContentInfo.LastEditUserName)}", DataType.VarChar, 255, contentInfo.LastEditUserName),
                GetParameter($"@{nameof(ContentInfo.WritingUserName)}", DataType.VarChar, 255, contentInfo.WritingUserName),
                GetParameter($"@{nameof(ContentInfo.LastEditDate)}", DataType.DateTime, contentInfo.LastEditDate),
                GetParameter($"@{nameof(ContentInfo.Taxis)}", DataType.Integer, contentInfo.Taxis),
                GetParameter($"@{nameof(ContentInfo.ContentGroupNameCollection)}", DataType.VarChar, 255, contentInfo.ContentGroupNameCollection),
                GetParameter($"@{nameof(ContentInfo.Tags)}", DataType.VarChar, 255, contentInfo.Tags),
                GetParameter($"@{nameof(ContentInfo.SourceId)}", DataType.Integer, contentInfo.SourceId),
                GetParameter($"@{nameof(ContentInfo.ReferenceId)}", DataType.Integer, contentInfo.ReferenceId),
                GetParameter($"@{nameof(ContentInfo.IsChecked)}", DataType.VarChar, 18, contentInfo.IsChecked.ToString()),
                GetParameter($"@{nameof(ContentInfo.CheckedLevel)}", DataType.Integer, contentInfo.CheckedLevel),
                GetParameter($"@{nameof(ContentInfo.Comments)}", DataType.Integer, contentInfo.Comments),
                GetParameter($"@{nameof(ContentInfo.Photos)}", DataType.Integer, contentInfo.Photos),
                GetParameter($"@{nameof(ContentInfo.Hits)}", DataType.Integer, contentInfo.Hits),
                GetParameter($"@{nameof(ContentInfo.HitsByDay)}", DataType.Integer, contentInfo.HitsByDay),
                GetParameter($"@{nameof(ContentInfo.HitsByWeek)}", DataType.Integer, contentInfo.HitsByWeek),
                GetParameter($"@{nameof(ContentInfo.HitsByMonth)}", DataType.Integer, contentInfo.HitsByMonth),
                GetParameter($"@{nameof(ContentInfo.LastHitsDate)}", DataType.DateTime, contentInfo.LastHitsDate),
                GetParameter($"@{nameof(ContentInfo.SettingsXml)}", DataType.Text, contentInfo.ToString(lowerCaseExcludeAttributesNames)),
                GetParameter($"@{nameof(ContentInfo.Title)}", DataType.VarChar, 255, contentInfo.Title),
                GetParameter($"@{nameof(ContentInfo.IsTop)}", DataType.VarChar, 18, contentInfo.IsTop.ToString()),
                GetParameter($"@{nameof(ContentInfo.IsRecommend)}", DataType.VarChar, 18, contentInfo.IsRecommend.ToString()),
                GetParameter($"@{nameof(ContentInfo.IsHot)}", DataType.VarChar, 18, contentInfo.IsHot.ToString()),
                GetParameter($"@{nameof(ContentInfo.IsColor)}", DataType.VarChar, 18, contentInfo.IsColor.ToString()),
                GetParameter($"@{nameof(ContentInfo.LinkUrl)}", DataType.VarChar, 200, contentInfo.LinkUrl),
                GetParameter($"@{nameof(ContentInfo.AddDate)}", DataType.DateTime, contentInfo.AddDate)
            };
            parameters.AddRange(paras);
            parameters.Add(GetParameter($"@{nameof(ContentInfo.Id)}", DataType.Integer, contentInfo.Id));

            ExecuteNonQuery(sqlString, parameters.ToArray());

            Content.ClearCache();
        }

        public void UpdateAutoPageContent(string tableName, PublishmentSystemInfo publishmentSystemInfo)
        {
            if (!publishmentSystemInfo.Additional.IsAutoPageInTextEditor) return;

            string sqlString =
                $"SELECT Id, {BackgroundContentAttribute.Content} FROM {tableName} WHERE (PublishmentSystemId = {publishmentSystemInfo.PublishmentSystemId})";

            using (var rdr = ExecuteReader(sqlString))
            {
                while (rdr.Read())
                {
                    var contentId = GetInt(rdr, 0);
                    var content = GetString(rdr, 1);
                    if (!string.IsNullOrEmpty(content))
                    {
                        content = ContentUtility.GetAutoPageContent(content, publishmentSystemInfo.Additional.AutoPageWordNum);
                        string updateString =
                            $"UPDATE {tableName} SET {BackgroundContentAttribute.Content} = '{content}' WHERE Id = {contentId}";
                        try
                        {
                            ExecuteNonQuery(updateString);
                        }
                        catch
                        {
                            // ignored
                        }
                    }
                }

                rdr.Close();
            }

            Content.ClearCache();
        }

        public void TrashContents(int publishmentSystemId, string tableName, List<int> contentIdList, int nodeId)
        {
            if (string.IsNullOrEmpty(tableName)) return;

            var referenceIdList = GetReferenceIdList(tableName, contentIdList);
            if (referenceIdList.Count > 0)
            {
                DeleteContents(publishmentSystemId, tableName, referenceIdList);
            }
            var updateNum = 0;

            if (!string.IsNullOrEmpty(tableName) && contentIdList != null && contentIdList.Count > 0)
            {
                string sqlString =
                    $"UPDATE {tableName} SET NodeId = -NodeId, LastEditDate = {SqlUtils.GetComparableNow()} WHERE PublishmentSystemId = {publishmentSystemId} AND Id IN ({TranslateUtils.ToSqlInStringWithoutQuote(contentIdList)})";
                updateNum = ExecuteNonQuery(sqlString);
            }

            if (updateNum <= 0) return;

            new Action(() =>
            {
                DataProvider.NodeDao.UpdateContentNum(PublishmentSystemManager.GetPublishmentSystemInfo(publishmentSystemId), nodeId, true);
            }).BeginInvoke(null, null);

            Content.ClearCache();
        }

        public void TrashContents(int publishmentSystemId, string tableName, List<int> contentIdList)
        {
            if (string.IsNullOrEmpty(tableName)) return;

            var referenceIdList = GetReferenceIdList(tableName, contentIdList);
            if (referenceIdList.Count > 0)
            {
                DeleteContents(publishmentSystemId, tableName, referenceIdList);
            }

            var updateNum = 0;

            if (!string.IsNullOrEmpty(tableName) && contentIdList != null && contentIdList.Count > 0)
            {
                string sqlString =
                    $"UPDATE {tableName} SET NodeId = -NodeId, LastEditDate = {SqlUtils.GetComparableNow()} WHERE PublishmentSystemId = {publishmentSystemId} AND Id IN ({TranslateUtils.ToSqlInStringWithoutQuote(contentIdList)})";
                updateNum = ExecuteNonQuery(sqlString);
            }

            if (updateNum <= 0) return;

            new Action(() =>
            {
                DataProvider.NodeDao.UpdateContentNum(PublishmentSystemManager.GetPublishmentSystemInfo(publishmentSystemId));
            }).BeginInvoke(null, null);

            Content.ClearCache();
        }

        public void TrashContentsByNodeId(int publishmentSystemId, string tableName, int nodeId)
        {
            if (string.IsNullOrEmpty(tableName)) return;

            var contentIdList = GetContentIdList(tableName, nodeId);
            var referenceIdList = GetReferenceIdList(tableName, contentIdList);
            if (referenceIdList.Count > 0)
            {
                DeleteContents(publishmentSystemId, tableName, referenceIdList);
            }
            var updateNum = 0;

            if (!string.IsNullOrEmpty(tableName))
            {
                string sqlString =
                    $"UPDATE {tableName} SET NodeId = -NodeId, LastEditDate = {SqlUtils.GetComparableNow()} WHERE PublishmentSystemId = {publishmentSystemId} AND NodeId = {publishmentSystemId}";
                updateNum = ExecuteNonQuery(sqlString);
            }

            if (updateNum <= 0) return;

            new Action(() =>
            {
                DataProvider.NodeDao.UpdateContentNum(PublishmentSystemManager.GetPublishmentSystemInfo(publishmentSystemId), nodeId, true);
            }).BeginInvoke(null, null);

            Content.ClearCache();
        }

        public void DeleteContents(int publishmentSystemId, string tableName, List<int> contentIdList, int nodeId)
        {
            if (string.IsNullOrEmpty(tableName)) return;

            var deleteNum = 0;

            if (!string.IsNullOrEmpty(tableName) && contentIdList != null && contentIdList.Count > 0)
            {
                TagUtils.RemoveTags(publishmentSystemId, contentIdList);

                string sqlString =
                    $"DELETE FROM {tableName} WHERE PublishmentSystemId = {publishmentSystemId} AND Id IN ({TranslateUtils.ToSqlInStringWithoutQuote(contentIdList)})";
                deleteNum = ExecuteNonQuery(sqlString);
            }

            if (nodeId <= 0 || deleteNum <= 0) return;

            new Action(() =>
            {
                DataProvider.NodeDao.UpdateContentNum(PublishmentSystemManager.GetPublishmentSystemInfo(publishmentSystemId), nodeId, true);
            }).BeginInvoke(null, null);

            Content.ClearCache();
        }

        public void DeleteContentsByNodeId(int publishmentSystemId, string tableName, int nodeId)
        {
            if (string.IsNullOrEmpty(tableName)) return;

            var contentIdList = GetContentIdListChecked(tableName, nodeId, string.Empty);

            TagUtils.RemoveTags(publishmentSystemId, contentIdList);

            string sqlString =
                $"DELETE FROM {tableName} WHERE PublishmentSystemId = {publishmentSystemId} AND NodeId = {nodeId}";
            var deleteNum = ExecuteNonQuery(sqlString);

            if (nodeId <= 0 || deleteNum <= 0) return;

            new Action(() =>
            {
                DataProvider.NodeDao.UpdateContentNum(PublishmentSystemManager.GetPublishmentSystemInfo(publishmentSystemId), nodeId, true);
            }).BeginInvoke(null, null);

            Content.ClearCache();
        }

        public void RestoreContentsByTrash(int publishmentSystemId, string tableName)
        {
            var updateNum = 0;

            if (!string.IsNullOrEmpty(tableName))
            {
                string sqlString =
                    $"UPDATE {tableName} SET NodeId = -NodeId, LastEditDate = {SqlUtils.GetComparableNow()} WHERE PublishmentSystemId = {publishmentSystemId} AND NodeId < 0";
                updateNum = ExecuteNonQuery(sqlString);
            }

            if (updateNum <= 0) return;

            new Action(() =>
            {
                DataProvider.NodeDao.UpdateContentNum(PublishmentSystemManager.GetPublishmentSystemInfo(publishmentSystemId));
            }).BeginInvoke(null, null);

            Content.ClearCache();
        }

        private void SetTaxis(int id, int taxis, string tableName)
        {
            string sqlString = $"UPDATE {tableName} SET Taxis = {taxis} WHERE Id = {id}";
            ExecuteNonQuery(sqlString);

            Content.ClearCache();
        }

        private void DeleteContents(int publishmentSystemId, string tableName, List<int> contentIdList)
        {
            if (string.IsNullOrEmpty(tableName)) return;

            var deleteNum = 0;

            if (!string.IsNullOrEmpty(tableName) && contentIdList != null && contentIdList.Count > 0)
            {
                TagUtils.RemoveTags(publishmentSystemId, contentIdList);

                string sqlString =
                    $"DELETE FROM {tableName} WHERE PublishmentSystemId = {publishmentSystemId} AND Id IN ({TranslateUtils.ToSqlInStringWithoutQuote(contentIdList)})";
                deleteNum = ExecuteNonQuery(sqlString);
            }

            if (deleteNum <= 0) return;

            new Action(() =>
            {
                DataProvider.NodeDao.UpdateContentNum(PublishmentSystemManager.GetPublishmentSystemInfo(publishmentSystemId));
            }).BeginInvoke(null, null);

            Content.ClearCache();
        }

        public void DeletePreviewContents(int publishmentSystemId, string tableName, NodeInfo nodeInfo)
        {
            if (!string.IsNullOrEmpty(tableName))
            {
                nodeInfo.Additional.IsPreviewContents = false;
                DataProvider.NodeDao.UpdateAdditional(nodeInfo);

                string sqlString =
                    $"DELETE FROM {tableName} WHERE PublishmentSystemId = {publishmentSystemId} AND NodeId = {nodeInfo.NodeId} AND SourceId = {SourceManager.Preview}";
                BaiRongDataProvider.DatabaseDao.ExecuteSql(sqlString);
            }
        }

        public void TidyUp(string tableName, int nodeId, string attributeName, bool isDesc)
        {
            var taxisDirection = isDesc ? "ASC" : "DESC";//升序,但由于页面排序是按Taxis的Desc排序的，所以这里sql里面的ASC/DESC取反

            string sqlString =
                $"SELECT Id, IsTop FROM {tableName} WHERE NodeId = {nodeId} OR NodeId = -{nodeId} ORDER BY {attributeName} {taxisDirection}";
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

            BaiRongDataProvider.DatabaseDao.ExecuteSql(sqlList);

            Content.ClearCache();
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        public bool UpdateTaxisToUp(string tableName, int nodeId, int contentId, bool isTop)
        {
            //Get Higher Taxis and Id
            var sqlString = SqlUtils.ToTopSqlString(tableName, "Id, Taxis",
                isTop
                    ? $"WHERE (Taxis > (SELECT Taxis FROM {tableName} WHERE Id = {contentId}) AND Taxis >= {TaxisIsTopStartValue} AND NodeId = {nodeId})"
                    : $"WHERE (Taxis > (SELECT Taxis FROM {tableName} WHERE Id = {contentId}) AND Taxis < {TaxisIsTopStartValue} AND NodeId = {nodeId})",
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
                SetTaxis(contentId, higherTaxis, tableName);
                //Set The Higher Class Taxis To Lower Level
                SetTaxis(higherId, selectedTaxis, tableName);
                return true;
            }
            return false;
        }

        public bool UpdateTaxisToDown(string tableName, int nodeId, int contentId, bool isTop)
        {
            //Get Lower Taxis and Id
            var sqlString = SqlUtils.ToTopSqlString(tableName, "Id, Taxis",
                isTop
                    ? $"WHERE (Taxis < (SELECT Taxis FROM {tableName} WHERE Id = {contentId}) AND Taxis >= {TaxisIsTopStartValue} AND NodeId = {nodeId})"
                    : $"WHERE (Taxis < (SELECT Taxis FROM {tableName} WHERE Id = {contentId}) AND Taxis < {TaxisIsTopStartValue} AND NodeId = {nodeId})",
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
                SetTaxis(contentId, lowerTaxis, tableName);
                //Set The Lower Class Taxis To Higher Level
                SetTaxis(lowerId, selectedTaxis, tableName);
                return true;
            }
            return false;
        }

        public int GetMaxTaxis(string tableName, int nodeId, bool isTop)
        {
            var maxTaxis = 0;
            if (isTop)
            {
                maxTaxis = TaxisIsTopStartValue;

                string sqlString =
                    $"SELECT MAX(Taxis) FROM {tableName} WHERE NodeId = {nodeId} AND Taxis >= {TaxisIsTopStartValue}";

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
                if (maxTaxis == TaxisMaxValue)
                {
                    maxTaxis = TaxisMaxValue - 1;
                }
            }
            else
            {
                string sqlString =
                    $"SELECT MAX(Taxis) FROM {tableName} WHERE NodeId = {nodeId} AND Taxis < {TaxisIsTopStartValue}";
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

        public string GetValue(string tableName, int contentId, string name)
        {
            string sqlString = $"SELECT {name} FROM {tableName} WHERE (Id = {contentId})";
            return BaiRongDataProvider.DatabaseDao.GetString(sqlString);
        }

        public void AddContentGroupList(string tableName, int contentId, List<string> contentGroupList)
        {
            var list = TranslateUtils.StringCollectionToStringList(GetValue(tableName, contentId, ContentAttribute.ContentGroupNameCollection));
            foreach (var groupName in contentGroupList)
            {
                if (!list.Contains(groupName)) list.Add(groupName);
            }
            SetValue(tableName, contentId, ContentAttribute.ContentGroupNameCollection, TranslateUtils.ObjectCollectionToString(list));
        }       

        public int GetReferenceId(string tableName, int contentId, out string linkUrl)
        {
            var referenceId = 0;
            linkUrl = string.Empty;
            try
            {
                string sqlString = $"SELECT ReferenceId, LinkUrl FROM {tableName} WHERE Id = {contentId}";

                using (var rdr = ExecuteReader(sqlString))
                {
                    if (rdr.Read())
                    {
                        referenceId = GetInt(rdr, 0);
                        linkUrl = GetString(rdr, 1);
                    }
                    rdr.Close();
                }
            }
            catch
            {
                // ignored
            }
            return referenceId;
        }

        public List<int> GetReferenceIdList(string tableName, List<int> contentIdList)
        {
            var list = new List<int>();
            string sqlString =
                $"SELECT Id FROM {tableName} WHERE NodeId > 0 AND ReferenceId IN ({TranslateUtils.ToSqlInStringWithoutQuote(contentIdList)})";

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

        public string GetSelectCommend(string tableName, int nodeId, ETriState checkedState, string userNameOnly)
        {
            var orderByString = ETaxisTypeUtils.GetContentOrderByString(ETaxisType.OrderByTaxisDesc);

            var whereString = new StringBuilder();
            whereString.Append($"WHERE (NodeId = {nodeId}) ");

            if (checkedState == ETriState.True)
            {
                whereString.Append("AND IsChecked='True' ");
            }
            else if (checkedState == ETriState.False)
            {
                whereString.Append("AND IsChecked='False'");
            }

            if (!string.IsNullOrEmpty(userNameOnly))
            {
                whereString.Append($" AND AddUserName = '{userNameOnly}' ");
            }

            //whereString.Append(orderByString);

            return BaiRongDataProvider.DatabaseDao.GetSelectSqlString(tableName, SqlUtils.Asterisk, whereString.ToString(), orderByString);
        }

        public string GetSelectCommandByHitsAnalysis(string tableName, int publishmentSystemId)
        {
            var orderByString = ETaxisTypeUtils.GetContentOrderByString(ETaxisType.OrderByTaxisDesc);

            var whereString = new StringBuilder();
            whereString.Append($"AND IsChecked='{true}' AND PublishmentSystemId = {publishmentSystemId} AND Hits > 0");
            whereString.Append(orderByString);

            return BaiRongDataProvider.DatabaseDao.GetSelectSqlString(tableName, SqlUtils.Asterisk, whereString.ToString());
        }

        public int GetTotalHits(string tableName, int publishmentSystemId)
        {
            return BaiRongDataProvider.DatabaseDao.GetIntResult($"SELECT SUM(Hits) FROM {tableName} WHERE IsChecked='{true}' AND PublishmentSystemId = {publishmentSystemId} AND Hits > 0");
        }

        public int GetFirstContentId(string tableName, int nodeId)
        {
            string sqlString = $"SELECT Id FROM {tableName} WHERE NodeId = {nodeId} ORDER BY Taxis DESC, Id DESC";
            return BaiRongDataProvider.DatabaseDao.GetIntResult(sqlString);
        }

        public List<int> GetContentIdList(string tableName, int nodeId)
        {
            var list = new List<int>();

            string sqlString = $"SELECT Id FROM {tableName} WHERE NodeId = {nodeId}";
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

        public List<int> GetContentIdList(string tableName, int nodeId, bool isPeriods, string dateFrom, string dateTo, ETriState checkedState)
        {
            var list = new List<int>();

            string sqlString = $"SELECT Id FROM {tableName} WHERE NodeId = {nodeId}";
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

        public List<int> GetContentIdListCheckedByNodeId(string tableName, int publishmentSystemId, int nodeId)
        {
            var list = new List<int>();

            string sqlString = $"SELECT Id FROM {tableName} WHERE PublishmentSystemId = {publishmentSystemId} AND NodeId = {nodeId} AND IsChecked = '{true}'";
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

        public int GetContentId(string tableName, int nodeId, int taxis, bool isNextContent)
        {
            var contentId = 0;
            var sqlString = SqlUtils.ToTopSqlString(tableName, "Id", $"WHERE (NodeId = {nodeId} AND Taxis > {taxis} AND IsChecked = 'True')", "ORDER BY Taxis", 1);
            if (isNextContent)
            {
                sqlString = SqlUtils.ToTopSqlString(tableName, "Id",
                $"WHERE (NodeId = {nodeId} AND Taxis < {taxis} AND IsChecked = 'True')", "ORDER BY Taxis DESC", 1);
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

        public int GetContentId(string tableName, int nodeId, string orderByString)
        {
            var contentId = 0;
            var sqlString = SqlUtils.ToTopSqlString(tableName, "Id", $"WHERE (NodeId = {nodeId})", orderByString, 1);

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

        public List<string> GetValueList(string tableName, int nodeId, string name)
        {
            string sqlString = $"SELECT {name} FROM {tableName} WHERE NodeId = {nodeId}";
            return BaiRongDataProvider.DatabaseDao.GetStringList(sqlString);
        }

        public List<string> GetValueListByStartString(string tableName, int nodeId, string name, string startString, int totalNum)
        {
            var inStr = SqlUtils.GetInStr(name, startString);
            var sqlString = SqlUtils.GetDistinctTopSqlString(tableName, name, $"WHERE NodeId = {nodeId} AND {inStr}", string.Empty, totalNum);
            return BaiRongDataProvider.DatabaseDao.GetStringList(sqlString);
        }

        public int GetNodeId(string tableName, int contentId)
        {
            var nodeId = 0;
            string sqlString = $"SELECT {ContentAttribute.NodeId} FROM {tableName} WHERE (Id = {contentId})";

            using (var rdr = ExecuteReader(sqlString))
            {
                if (rdr.Read())
                {
                    nodeId = GetInt(rdr, 0);
                }
                rdr.Close();
            }
            return nodeId;
        }

        public DateTime GetAddDate(string tableName, int contentId)
        {
            var addDate = DateTime.Now;
            string sqlString = $"SELECT {ContentAttribute.AddDate} FROM {tableName} WHERE (Id = {contentId})";

            using (var rdr = ExecuteReader(sqlString))
            {
                if (rdr.Read())
                {
                    addDate = GetDateTime(rdr, 0);
                }
                rdr.Close();
            }
            return addDate;
        }

        public DateTime GetLastEditDate(string tableName, int contentId)
        {
            var lastEditDate = DateTime.Now;
            string sqlString = $"SELECT {ContentAttribute.LastEditDate} FROM {tableName} WHERE (Id = {contentId})";

            using (var rdr = ExecuteReader(sqlString))
            {
                if (rdr.Read())
                {
                    lastEditDate = GetDateTime(rdr, 0);
                }
                rdr.Close();
            }
            return lastEditDate;
        }

        public int GetCount(string tableName, int nodeId)
        {
            string sqlString = $"SELECT COUNT(*) AS TotalNum FROM {tableName} WHERE (NodeId = {nodeId})";

            return BaiRongDataProvider.DatabaseDao.GetIntResult(sqlString);
        }

        public int GetSequence(string tableName, int nodeId, int contentId)
        {
            string sqlString =
                $"SELECT COUNT(*) AS TotalNum FROM {tableName} WHERE NodeId = {nodeId} AND IsChecked = '{true}' AND Taxis < (SELECT Taxis FROM {tableName} WHERE (Id = {contentId}))";

            return BaiRongDataProvider.DatabaseDao.GetIntResult(sqlString) + 1;
        }

        public string GetSelectCommendOfAdminExcludeRecycle(string tableName, int publishmentSystemId, DateTime begin, DateTime end)
        {
            string sqlString = $@"select userName,SUM(addCount) as addCount, SUM(updateCount) as updateCount from( 
SELECT AddUserName as userName, Count(AddUserName) as addCount, 0 as updateCount FROM {tableName} 
INNER JOIN bairong_Administrator ON AddUserName = bairong_Administrator.UserName 
WHERE {tableName}.PublishmentSystemId = {publishmentSystemId} AND (({tableName}.NodeId > 0)) 
AND LastEditDate BETWEEN {SqlUtils.GetComparableDate(begin)} AND {SqlUtils.GetComparableDate(end.AddDays(1))}
GROUP BY AddUserName
Union
SELECT LastEditUserName as userName,0 as addCount, Count(LastEditUserName) as updateCount FROM {tableName} 
INNER JOIN bairong_Administrator ON LastEditUserName = bairong_Administrator.UserName 
WHERE {tableName}.PublishmentSystemId = {publishmentSystemId} AND (({tableName}.NodeId > 0)) 
AND LastEditDate BETWEEN {SqlUtils.GetComparableDate(begin)} AND {SqlUtils.GetComparableDate(end.AddDays(1))}
AND LastEditDate != AddDate
GROUP BY LastEditUserName
) as tmp
group by tmp.userName";


            return sqlString;
        }

        public List<int> GetNodeIdListCheckedByLastEditDateHour(string tableName, int publishmentSystemId, int hour)
        {
            var list = new List<int>();

            string sqlString =
                $"SELECT DISTINCT NodeId FROM {tableName} WHERE (PublishmentSystemId = {publishmentSystemId}) AND (IsChecked = '{true}') AND (LastEditDate BETWEEN {SqlUtils.GetComparableDateTime(DateTime.Now.AddHours(-hour))} AND {SqlUtils.GetComparableNow()})";

            using (var rdr = ExecuteReader(sqlString))
            {
                while (rdr.Read())
                {
                    var nodeId = GetInt(rdr, 0);
                    list.Add(nodeId);
                }
                rdr.Close();
            }
            return list;
        }

        public string GetSelectedCommendByCheck(string tableName, int publishmentSystemId, List<int> nodeIdList, List<int> checkLevelList)
        {
            var whereString = nodeIdList.Count == 1
                ? $"WHERE PublishmentSystemId = {publishmentSystemId} AND NodeId = {nodeIdList[0]} AND IsChecked='{false}' AND CheckedLevel IN ({TranslateUtils.ToSqlInStringWithoutQuote(checkLevelList)}) "
                : $"WHERE PublishmentSystemId = {publishmentSystemId} AND NodeId IN ({TranslateUtils.ToSqlInStringWithoutQuote(nodeIdList)}) AND IsChecked='{false}' AND CheckedLevel IN ({TranslateUtils.ToSqlInStringWithoutQuote(checkLevelList)}) ";

            return BaiRongDataProvider.DatabaseDao.GetSelectSqlString(tableName, SqlUtils.Asterisk, whereString);
        }

        public string GetStlWhereString(int publishmentSystemId, string group, string groupNot, string tags, bool isTopExists, bool isTop, string where)
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
                        //    $" ({ContentAttribute.ContentGroupNameCollection} = '{theGroup.Trim()}' OR CHARINDEX('{theGroup.Trim()},',{ContentAttribute.ContentGroupNameCollection}) > 0 OR CHARINDEX(',{theGroup.Trim()},',{ContentAttribute.ContentGroupNameCollection}) > 0 OR CHARINDEX(',{theGroup.Trim()}',{ContentAttribute.ContentGroupNameCollection}) > 0) OR ");

                        whereStringBuilder.Append(
                                $" ({ContentAttribute.ContentGroupNameCollection} = '{theGroup.Trim()}' OR {SqlUtils.GetInStr(ContentAttribute.ContentGroupNameCollection, theGroup.Trim() + ",")} OR {SqlUtils.GetInStr(ContentAttribute.ContentGroupNameCollection, "," + theGroup.Trim() + ",")} OR {SqlUtils.GetInStr(ContentAttribute.ContentGroupNameCollection, "," + theGroup.Trim())}) OR ");
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
                        //    $" ({ContentAttribute.ContentGroupNameCollection} <> '{theGroupNot.Trim()}' AND CHARINDEX('{theGroupNot.Trim()},',{ContentAttribute.ContentGroupNameCollection}) = 0 AND CHARINDEX(',{theGroupNot.Trim()},',{ContentAttribute.ContentGroupNameCollection}) = 0 AND CHARINDEX(',{theGroupNot.Trim()}',{ContentAttribute.ContentGroupNameCollection}) = 0) AND ");

                        whereStringBuilder.Append(
                                $" ({ContentAttribute.ContentGroupNameCollection} <> '{theGroupNot.Trim()}' AND {SqlUtils.GetNotInStr(ContentAttribute.ContentGroupNameCollection, theGroupNot.Trim() + ",")} AND {SqlUtils.GetNotInStr(ContentAttribute.ContentGroupNameCollection, "," + theGroupNot.Trim() + ",")} AND {SqlUtils.GetNotInStr(ContentAttribute.ContentGroupNameCollection, "," + theGroupNot.Trim())}) AND ");
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
                var contentIdList = BaiRongDataProvider.TagDao.GetContentIdListByTagCollection(tagCollection, publishmentSystemId);
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

        public DataSet GetDataSetOfAdminExcludeRecycle(string tableName, int publishmentSystemId, DateTime begin, DateTime end)
        {
            var sqlString = GetSelectCommendOfAdminExcludeRecycle(tableName, publishmentSystemId, begin, end);

            return ExecuteDataset(sqlString);
        }

        public int Insert(string tableName, PublishmentSystemInfo publishmentSystemInfo, IContentInfo contentInfo)
        {
            var taxis = GetTaxisToInsert(tableName, contentInfo.NodeId, contentInfo.IsTop);
            return Insert(tableName, publishmentSystemInfo, contentInfo, true, taxis);
        }

        public int InsertPreview(string tableName, PublishmentSystemInfo publishmentSystemInfo, NodeInfo nodeInfo, ContentInfo contentInfo)
        {
            nodeInfo.Additional.IsPreviewContents = true;
            DataProvider.NodeDao.UpdateAdditional(nodeInfo);

            contentInfo.SourceId = SourceManager.Preview;
            return Insert(tableName, publishmentSystemInfo, contentInfo, false, 0);
        }

        public int Insert(string tableName, PublishmentSystemInfo publishmentSystemInfo, IContentInfo contentInfo, bool isUpdateContentNum, int taxis)
        {
            var contentId = 0;

            if (!string.IsNullOrEmpty(tableName))
            {
                if (publishmentSystemInfo.Additional.IsAutoPageInTextEditor && contentInfo.ContainsKey(BackgroundContentAttribute.Content))
                {
                    contentInfo.Set(BackgroundContentAttribute.Content, ContentUtility.GetAutoPageContent(contentInfo.GetString(BackgroundContentAttribute.Content), publishmentSystemInfo.Additional.AutoPageWordNum));
                }

                contentInfo.Taxis = taxis;

                contentId = Insert(tableName, contentInfo);

                if (isUpdateContentNum)
                {
                    new Action(() =>
                    {
                        DataProvider.NodeDao.UpdateContentNum(PublishmentSystemManager.GetPublishmentSystemInfo(contentInfo.PublishmentSystemId), contentInfo.NodeId, true);
                    }).BeginInvoke(null, null);
                }

                Content.ClearCache();
            }

            return contentId;
        }

        public void Update(string tableName, PublishmentSystemInfo publishmentSystemInfo, IContentInfo contentInfo)
        {
            if (publishmentSystemInfo.Additional.IsAutoPageInTextEditor && contentInfo.ContainsKey(BackgroundContentAttribute.Content))
            {
                contentInfo.Set(BackgroundContentAttribute.Content, ContentUtility.GetAutoPageContent(contentInfo.GetString(BackgroundContentAttribute.Content), publishmentSystemInfo.Additional.AutoPageWordNum));
            }

            Update(tableName, contentInfo);

            Content.ClearCache();
        }

        public ContentInfo GetContentInfo(string tableName, int contentId)
        {
            if (string.IsNullOrEmpty(tableName) || contentId <= 0) return null;

            ContentInfo info = null;

            string sqlWhere = $"WHERE Id = {contentId}";
            var sqlSelect = BaiRongDataProvider.DatabaseDao.GetSelectSqlString(tableName, SqlUtils.Asterisk, sqlWhere);

            using (var rdr = ExecuteReader(sqlSelect))
            {
                if (rdr.Read())
                {
                    info = GetContentInfo(rdr);
                }
                rdr.Close();
            }

            return info;
        }

        public int GetCountOfContentAdd(string tableName, int publishmentSystemId, int nodeId, EScopeType scope, DateTime begin, DateTime end, string userName)
        {
            var nodeIdList = DataProvider.NodeDao.GetNodeIdListByScopeType(nodeId, scope, string.Empty, string.Empty);
            return GetCountOfContentAdd(tableName, publishmentSystemId, nodeIdList, begin, end, userName);
        }

        public List<int> GetContentIdListChecked(string tableName, int nodeId, string orderByFormatString)
        {
            return GetContentIdListChecked(tableName, nodeId, orderByFormatString, string.Empty);
        }

        public int GetCountOfContentUpdate(string tableName, int publishmentSystemId, int nodeId, EScopeType scope, DateTime begin, DateTime end, string userName)
        {
            var nodeIdList = DataProvider.NodeDao.GetNodeIdListByScopeType(nodeId, scope, string.Empty, string.Empty);
            return GetCountOfContentUpdate(tableName, publishmentSystemId, nodeIdList, begin, end, userName);
        }

        public int GetCountOfContentUpdate(string tableName, int publishmentSystemId, List<int> nodeIdList, DateTime begin, DateTime end, string userName)
        {
            string sqlString;
            if (string.IsNullOrEmpty(userName))
            {
                sqlString = nodeIdList.Count == 1
                    ? $"SELECT COUNT(Id) AS Num FROM {tableName} WHERE PublishmentSystemId = {publishmentSystemId} AND NodeId = {nodeIdList[0]} AND (LastEditDate BETWEEN {SqlUtils.GetComparableDate(begin)} AND {SqlUtils.GetComparableDate(end.AddDays(1))}) AND (LastEditDate <> AddDate)"
                    : $"SELECT COUNT(Id) AS Num FROM {tableName} WHERE PublishmentSystemId = {publishmentSystemId} AND NodeId IN ({TranslateUtils.ToSqlInStringWithoutQuote(nodeIdList)}) AND (LastEditDate BETWEEN {SqlUtils.GetComparableDate(begin)} AND {SqlUtils.GetComparableDate(end.AddDays(1))}) AND (LastEditDate <> AddDate)";
            }
            else
            {
                sqlString = nodeIdList.Count == 1
                    ? $"SELECT COUNT(Id) AS Num FROM {tableName} WHERE PublishmentSystemId = {publishmentSystemId} AND NodeId = {nodeIdList[0]} AND (LastEditDate BETWEEN {SqlUtils.GetComparableDate(begin)} AND {SqlUtils.GetComparableDate(end.AddDays(1))}) AND (LastEditDate <> AddDate) AND (AddUserName = '{userName}')"
                    : $"SELECT COUNT(Id) AS Num FROM {tableName} WHERE PublishmentSystemId = {publishmentSystemId} AND NodeId IN ({TranslateUtils.ToSqlInStringWithoutQuote(nodeIdList)}) AND (LastEditDate BETWEEN {SqlUtils.GetComparableDate(begin)} AND {SqlUtils.GetComparableDate(end.AddDays(1))}) AND (LastEditDate <> AddDate) AND (AddUserName = '{userName}')";
            }

            return BaiRongDataProvider.DatabaseDao.GetIntResult(sqlString);
        }
        
        public string GetWhereStringByStlSearch(bool isAllSites, string siteName, string siteDir, string siteIds, string channelIndex, string channelName, string channelIds, string type, string word, string dateAttribute, string dateFrom, string dateTo, string since, int publishmentSystemId, List<string> excludeAttributes, NameValueCollection form, out bool isDefaultCondition)
        {
            isDefaultCondition = true;
            var whereBuilder = new StringBuilder();

            PublishmentSystemInfo publishmentSystemInfo = null;
            if (!string.IsNullOrEmpty(siteName))
            {
                publishmentSystemInfo = PublishmentSystemManager.GetPublishmentSystemInfoBySiteName(siteName);
            }
            else if (!string.IsNullOrEmpty(siteDir))
            {
                publishmentSystemInfo = PublishmentSystemManager.GetPublishmentSystemInfoByDirectory(siteDir);
            }
            if (publishmentSystemInfo == null)
            {
                publishmentSystemInfo = PublishmentSystemManager.GetPublishmentSystemInfo(publishmentSystemId);
            }

            var channelId = DataProvider.NodeDao.GetNodeIdByChannelIdOrChannelIndexOrChannelName(publishmentSystemId, publishmentSystemId, channelIndex, channelName);
            var nodeInfo = NodeManager.GetNodeInfo(publishmentSystemId, channelId);

            if (isAllSites)
            {
                whereBuilder.Append("(PublishmentSystemId > 0) ");
            }
            else if (!string.IsNullOrEmpty(siteIds))
            {
                whereBuilder.Append($"(PublishmentSystemId IN ({TranslateUtils.ToSqlInStringWithoutQuote(TranslateUtils.StringCollectionToIntList(siteIds))})) ");
            }
            else
            {
                whereBuilder.Append($"(PublishmentSystemId = {publishmentSystemInfo.PublishmentSystemId}) ");
            }

            if (!string.IsNullOrEmpty(channelIds))
            {
                whereBuilder.Append(" AND ");
                var nodeIdList = new List<int>();
                foreach (var nodeId in TranslateUtils.StringCollectionToIntList(channelIds))
                {
                    nodeIdList.Add(nodeId);
                    nodeIdList.AddRange(DataProvider.NodeDao.GetNodeIdListForDescendant(nodeId));
                }
                whereBuilder.Append(nodeIdList.Count == 1
                    ? $"(NodeId = {nodeIdList[0]}) "
                    : $"(NodeId IN ({TranslateUtils.ToSqlInStringWithoutQuote(nodeIdList)})) ");
            }
            else if (channelId != publishmentSystemId)
            {
                whereBuilder.Append(" AND ");
                var nodeIdList = DataProvider.NodeDao.GetNodeIdListForDescendant(channelId);
                nodeIdList.Add(channelId);
                whereBuilder.Append(nodeIdList.Count == 1
                    ? $"(NodeId = {nodeIdList[0]}) "
                    : $"(NodeId IN ({TranslateUtils.ToSqlInStringWithoutQuote(nodeIdList)})) ");
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
                    whereBuilder.Append($"[{attributeName}] LIKE '%{PageUtils.FilterSql(word)}%' OR ");
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

            var tableName = NodeManager.GetTableName(publishmentSystemInfo, nodeInfo);
            var styleInfoList = RelatedIdentities.GetTableStyleInfoList(publishmentSystemInfo, nodeInfo.NodeId);

            foreach (string key in form.Keys)
            {
                if (excludeAttributes.Contains(key.ToLower())) continue;
                if (string.IsNullOrEmpty(form[key])) continue;

                var value = StringUtils.Trim(form[key]);
                if (string.IsNullOrEmpty(value)) continue;

                if (TableMetadataManager.IsAttributeNameExists(tableName, key))
                {
                    whereBuilder.Append(" AND ");
                    whereBuilder.Append($"({key} LIKE '%{value}%')");
                }
                else
                {
                    foreach (var tableStyleInfo in styleInfoList)
                    {
                        if (StringUtils.EqualsIgnoreCase(tableStyleInfo.AttributeName, key))
                        {
                            whereBuilder.Append(" AND ");
                            whereBuilder.Append($"({ContentAttribute.SettingsXml} LIKE '%{key}={value}%')");
                            break;
                        }
                    }
                }
            }

            if (whereBuilder.ToString().Contains(" AND "))
            {
                isDefaultCondition = false;
            }

            return whereBuilder.ToString();
        }

        public string GetSelectCommend(string tableName, int publishmentSystemId, int nodeId, bool isSystemAdministrator, List<int> owningNodeIdList, string searchType, string keyword, string dateFrom, string dateTo, bool isSearchChildren, ETriState checkedState, bool isNoDup, bool isTrashContent)
        {
            var nodeInfo = NodeManager.GetNodeInfo(publishmentSystemId, nodeId);
            var nodeIdList = DataProvider.NodeDao.GetNodeIdListByScopeType(nodeInfo.NodeId, nodeInfo.ChildrenCount,
                isSearchChildren ? EScopeType.All : EScopeType.Self, string.Empty, string.Empty, nodeInfo.ContentModelPluginId);

            var list = new List<int>();
            if (isSystemAdministrator)
            {
                list = nodeIdList;
            }
            else
            {
                foreach (int theNodeId in nodeIdList)
                {
                    if (owningNodeIdList.Contains(theNodeId))
                    {
                        list.Add(theNodeId);
                    }
                }
            }

            return GetSelectCommendByCondition(tableName, publishmentSystemId, list, searchType, keyword, dateFrom, dateTo, checkedState, isNoDup, isTrashContent);
        }

        public string GetSelectCommend(string tableName, int publishmentSystemId, int nodeId, bool isSystemAdministrator, List<int> owningNodeIdList, string searchType, string keyword, string dateFrom, string dateTo, bool isSearchChildren, ETriState checkedState, bool isNoDup, bool isTrashContent, bool isWritingOnly, string userNameOnly)
        {
            var nodeInfo = NodeManager.GetNodeInfo(publishmentSystemId, nodeId);
            var nodeIdList = DataProvider.NodeDao.GetNodeIdListByScopeType(nodeInfo.NodeId, nodeInfo.ChildrenCount, isSearchChildren ? EScopeType.All : EScopeType.Self, string.Empty, string.Empty, nodeInfo.ContentModelPluginId);

            var list = new List<int>();
            if (isSystemAdministrator)
            {
                list = nodeIdList;
            }
            else
            {
                foreach (int theNodeId in nodeIdList)
                {
                    if (owningNodeIdList.Contains(theNodeId))
                    {
                        list.Add(theNodeId);
                    }
                }
            }

            return GetSelectCommendByCondition(tableName, publishmentSystemId, list, searchType, keyword, dateFrom, dateTo, checkedState, isNoDup, isTrashContent, isWritingOnly, userNameOnly);
        }

        public string GetSelectCommendByContentGroup(string tableName, string contentGroupName, int publishmentSystemId)
        {
            contentGroupName = PageUtils.FilterSql(contentGroupName);
            string sqlString =
                $"SELECT * FROM {tableName} WHERE PublishmentSystemId = {publishmentSystemId} AND NodeId > 0 AND (ContentGroupNameCollection LIKE '{contentGroupName},%' OR ContentGroupNameCollection LIKE '%,{contentGroupName}' OR ContentGroupNameCollection  LIKE '%,{contentGroupName},%'  OR ContentGroupNameCollection='{contentGroupName}')";
            return sqlString;
        }

        public DataSet GetStlDataSourceChecked(List<int> nodeIdList, string tableName, int startNum, int totalNum, string orderByString, string whereString, bool isNoDup, LowerNameValueCollection others)
        {
            return GetStlDataSourceChecked(tableName, nodeIdList, startNum, totalNum, orderByString, whereString, isNoDup, others);
        }

        public string GetStlSqlStringChecked(List<int> nodeIdList, string tableName, int publishmentSystemId, int nodeId, int startNum, int totalNum, string orderByString, string whereString, EScopeType scopeType, string groupChannel, string groupChannelNot, bool isNoDup)
        {
            string sqlWhereString;

            if (publishmentSystemId == nodeId && scopeType == EScopeType.All && string.IsNullOrEmpty(groupChannel) && string.IsNullOrEmpty(groupChannelNot))
            {
                sqlWhereString =
                    $"WHERE (PublishmentSystemId = {publishmentSystemId} AND NodeId > 0 AND IsChecked = '{true}' {whereString})";
            }
            else
            {
                if (nodeIdList == null || nodeIdList.Count == 0)
                {
                    return string.Empty;
                }
                sqlWhereString = nodeIdList.Count == 1 ? $"WHERE (NodeId = {nodeIdList[0]} AND IsChecked = '{true}' {whereString})" : $"WHERE (NodeId IN ({TranslateUtils.ToSqlInStringWithoutQuote(nodeIdList)}) AND IsChecked = '{true}' {whereString})";
            }

            if (isNoDup)
            {
                var sqlString = BaiRongDataProvider.DatabaseDao.GetSelectSqlString(tableName, "MIN(Id)", sqlWhereString + " GROUP BY Title");
                sqlWhereString += $" AND Id IN ({sqlString})";
            }

            if (!string.IsNullOrEmpty(tableName))
            {
                return BaiRongDataProvider.DatabaseDao.GetSelectSqlString(tableName, startNum, totalNum, StlColumns, sqlWhereString, orderByString);
            }
            return string.Empty;
        }

        public string GetStlSqlStringCheckedBySearch(string tableName, int startNum, int totalNum, string orderByString, string whereString, bool isNoDup)
        {
            string sqlWhereString =
                    $"WHERE (NodeId > 0 AND IsChecked = '{true}' {whereString})";
            if (isNoDup)
            {
                var sqlString = BaiRongDataProvider.DatabaseDao.GetSelectSqlString(tableName, "MIN(Id)", sqlWhereString + " GROUP BY Title");
                sqlWhereString += $" AND Id IN ({sqlString})";
            }

            if (!string.IsNullOrEmpty(tableName))
            {
                return BaiRongDataProvider.DatabaseDao.GetSelectSqlString(tableName, startNum, totalNum, $"{ContentAttribute.Id}, {ContentAttribute.NodeId}, {ContentAttribute.IsTop}, {ContentAttribute.AddDate}", sqlWhereString, orderByString);
            }
            return string.Empty;
        }

        public List<int> GetIdListBySameTitleInOneNode(string tableName, int nodeId, string title)
        {
            var list = new List<int>();
            string sql = $"SELECT Id FROM {tableName} WHERE NodeId = {nodeId} AND Title = '{title}'";
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

        public List<IContentInfo> GetListByLimitAndOffset(string tableName, int nodeId, string whereString, string orderString, int limit, int offset)
        {
            var list = new List<IContentInfo>();
            if (!string.IsNullOrEmpty(whereString))
            {
                whereString = whereString.Replace("WHERE ", string.Empty).Replace("where ", string.Empty);
            }
            if (!string.IsNullOrEmpty(orderString))
            {
                orderString = orderString.Replace("ORDER BY ", string.Empty).Replace("order by ", string.Empty);
            }
            var firstWhere = string.IsNullOrEmpty(whereString) ? string.Empty : $"WHERE {whereString}";
            var secondWhere = string.IsNullOrEmpty(whereString) ? string.Empty : $"AND {whereString}";
            var order = string.IsNullOrEmpty(orderString) ? "IsTop DESC, Id DESC" : orderString;

            var sqlString = $"SELECT * FROM {tableName} {firstWhere} ORDER BY {order}";
            if (limit > 0 && offset > 0)
            {
                switch (WebConfigUtils.DatabaseType)
                {
                    case EDatabaseType.MySql:
                        sqlString = $"SELECT * FROM {tableName} {firstWhere} ORDER BY {order} limit {limit} offset {offset}";
                        break;
                    case EDatabaseType.SqlServer:
                        sqlString = $@"SELECT TOP {limit} * FROM {tableName} WHERE Id NOT IN (SELECT TOP {offset} Id FROM {tableName} {firstWhere} ORDER BY {order}) {secondWhere} ORDER BY {order}";
                        break;
                    case EDatabaseType.PostgreSql:
                        sqlString = $"SELECT * FROM {tableName} {firstWhere} ORDER BY {order} limit {limit} offset {offset}";
                        break;
                    case EDatabaseType.Oracle:
                        sqlString = $"SELECT * FROM {tableName} {firstWhere} ORDER BY {order} OFFSET {offset} ROWS FETCH NEXT {limit} ROWS ONLY";
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            else if (limit > 0)
            {
                switch (WebConfigUtils.DatabaseType)
                {
                    case EDatabaseType.MySql:
                        sqlString = $"SELECT * FROM {tableName} {firstWhere} ORDER BY {order} limit {limit}";
                        break;
                    case EDatabaseType.SqlServer:
                        sqlString = $@"SELECT TOP {limit} * FROM {tableName} {firstWhere} ORDER BY {order}";
                        break;
                    case EDatabaseType.PostgreSql:
                        sqlString = $"SELECT * FROM {tableName} {firstWhere} ORDER BY {order} limit {limit}";
                        break;
                    case EDatabaseType.Oracle:
                        sqlString = $"SELECT * FROM {tableName} {firstWhere} ORDER BY {order} FETCH FIRST {limit} ROWS ONLY";
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            else if (offset > 0)
            {
                switch (WebConfigUtils.DatabaseType)
                {
                    case EDatabaseType.MySql:
                        sqlString = $"SELECT * FROM {tableName} {firstWhere} ORDER BY {order} offset {offset}";
                        break;
                    case EDatabaseType.SqlServer:
                        sqlString =
                            $@"SELECT * FROM {tableName} WHERE Id NOT IN (SELECT TOP {offset} Id FROM {tableName} {firstWhere} ORDER BY {order}) {secondWhere} ORDER BY {order}";
                        break;
                    case EDatabaseType.PostgreSql:
                        sqlString = $"SELECT * FROM {tableName} {firstWhere} ORDER BY {order} offset {offset}";
                        break;
                    case EDatabaseType.Oracle:
                        sqlString = $"SELECT * FROM {tableName} {firstWhere} ORDER BY {order} OFFSET {offset} ROWS";
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            using (var rdr = ExecuteReader(sqlString))
            {
                while (rdr.Read())
                {
                    var info = GetContentInfo(rdr);
                    list.Add(info);
                }
                rdr.Close();
            }

            return list;
        }

        public int GetCount(string tableName, int nodeId, string whereString)
        {
            if (!string.IsNullOrEmpty(whereString))
            {
                whereString = whereString.Replace("WHERE ", string.Empty).Replace("where ", string.Empty);
            }
            whereString = string.IsNullOrEmpty(whereString) ? string.Empty : $"WHERE {whereString}";

            string sqlString = $"SELECT COUNT(*) FROM {tableName} {whereString}";

            return BaiRongDataProvider.DatabaseDao.GetIntResult(sqlString);
        }

        public List<KeyValuePair<int, int>> GetCountListUnChecked(bool isSystemAdministrator, string administratorName, List<int> publishmentSystemIdList, List<int> owningNodeIdList, string tableName)
        {
            var list = new List<KeyValuePair<int, int>>();

            var publishmentSystemIdArrayList = PublishmentSystemManager.GetPublishmentSystemIdList();
            foreach (var publishmentSystemId in publishmentSystemIdArrayList)
            {
                if (!publishmentSystemIdList.Contains(publishmentSystemId)) continue;

                var publishmentSystemInfo = PublishmentSystemManager.GetPublishmentSystemInfo(publishmentSystemId);
                if (!isSystemAdministrator)
                {
                    //if (!owningNodeIDArrayList.Contains(psID)) continue;
                    //if (!AdminUtility.HasChannelPermissions(psID, psID, AppManager.CMS.Permission.Channel.ContentCheck)) continue;

                    var isContentCheck = false;
                    foreach (var theNodeId in owningNodeIdList)
                    {
                        if (AdminUtility.HasChannelPermissions(administratorName, publishmentSystemId, theNodeId, AppManager.Permissions.Channel.ContentCheck))
                        {
                            isContentCheck = true;
                        }
                    }
                    if (!isContentCheck)
                    {
                        continue;
                    }
                }

                int checkedLevel;
                var isChecked = CheckManager.GetUserCheckLevel(administratorName, publishmentSystemInfo, publishmentSystemInfo.PublishmentSystemId, out checkedLevel);
                var checkLevelList = CheckManager.LevelInt.GetCheckLevelListOfNeedCheck(publishmentSystemInfo, isChecked, checkedLevel);
                var sqlString = isSystemAdministrator ? $"SELECT COUNT(*) AS TotalNum FROM {tableName} WHERE (PublishmentSystemID = {publishmentSystemId} AND NodeID > 0 AND IsChecked = '{false}' AND CheckedLevel IN ({TranslateUtils.ToSqlInStringWithoutQuote(checkLevelList)}))" : $"SELECT COUNT(*) AS TotalNum FROM {tableName} WHERE (PublishmentSystemID = {publishmentSystemId} AND NodeID IN ({TranslateUtils.ToSqlInStringWithoutQuote(owningNodeIdList)}) AND IsChecked = '{false}' AND CheckedLevel IN ({TranslateUtils.ToSqlInStringWithoutQuote(checkLevelList)}))";

                var count = BaiRongDataProvider.DatabaseDao.GetIntResult(sqlString);
                if (count > 0)
                {
                    list.Add(new KeyValuePair<int, int>(publishmentSystemId, count));
                }
            }
            return list;
        }

        public int GetCountCheckedImage(int publishmentSystemId, int nodeId)
        {
            var tableName = PublishmentSystemManager.GetPublishmentSystemInfo(publishmentSystemId).AuxiliaryTableForContent;
            string sqlString =
                $"SELECT COUNT(*) AS TotalNum FROM {tableName} WHERE (NodeID = {nodeId} AND ImageUrl <> '' AND {ContentAttribute.IsChecked} = '{true}')";

            return BaiRongDataProvider.DatabaseDao.GetIntResult(sqlString);
        }

        public string GetStlWhereString(int publishmentSystemId, string tableName, string group, string groupNot, string tags, bool isImageExists, bool isImage, bool isVideoExists, bool isVideo, bool isFileExists, bool isFile, bool isTopExists, bool isTop, bool isRecommendExists, bool isRecommend, bool isHotExists, bool isHot, bool isColorExists, bool isColor, string where, bool isCreateSearchDuplicate)
        {
            var whereBuilder = new StringBuilder();
            whereBuilder.Append($" AND PublishmentSystemID = {publishmentSystemId} ");

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
                        //    $" ({ContentAttribute.ContentGroupNameCollection} = '{trimGroup}' OR CHARINDEX('{trimGroup},',{ContentAttribute.ContentGroupNameCollection}) > 0 OR CHARINDEX(',{trimGroup},',{ContentAttribute.ContentGroupNameCollection}) > 0 OR CHARINDEX(',{trimGroup}',{ContentAttribute.ContentGroupNameCollection}) > 0) OR ");

                        whereBuilder.Append(
                                $" ({ContentAttribute.ContentGroupNameCollection} = '{trimGroup}' OR {SqlUtils.GetInStr(ContentAttribute.ContentGroupNameCollection, trimGroup + ",")} OR {SqlUtils.GetInStr(ContentAttribute.ContentGroupNameCollection, "," + trimGroup + ",")} OR {SqlUtils.GetInStr(ContentAttribute.ContentGroupNameCollection, "," + trimGroup)}) OR ");
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
                        //    $" ({ContentAttribute.ContentGroupNameCollection} <> '{trimGroup}' AND CHARINDEX('{trimGroup},',{ContentAttribute.ContentGroupNameCollection}) = 0 AND CHARINDEX(',{trimGroup},',{ContentAttribute.ContentGroupNameCollection}) = 0 AND CHARINDEX(',{trimGroup}',{ContentAttribute.ContentGroupNameCollection}) = 0) AND ");

                        whereBuilder.Append(
                                $" ({ContentAttribute.ContentGroupNameCollection} <> '{trimGroup}' AND {SqlUtils.GetNotInStr(ContentAttribute.ContentGroupNameCollection, trimGroup + ",")} AND {SqlUtils.GetNotInStr(ContentAttribute.ContentGroupNameCollection, "," + trimGroup + ",")} AND {SqlUtils.GetNotInStr(ContentAttribute.ContentGroupNameCollection, "," + trimGroup)}) AND ");
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
                var contentIdArrayList = BaiRongDataProvider.TagDao.GetContentIdListByTagCollection(tagCollection, publishmentSystemId);
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

            if (!isCreateSearchDuplicate)
            {
                var sqlString = BaiRongDataProvider.DatabaseDao.GetSelectSqlString(tableName, "MIN(ID)", whereBuilder + " GROUP BY Title");
                whereBuilder.Append($" AND ID IN ({sqlString}) ");
            }

            return whereBuilder.ToString();
        }

        public string GetStlWhereStringBySearch(string tableName, string group, string groupNot, string tags, bool isImageExists, bool isImage, bool isVideoExists, bool isVideo, bool isFileExists, bool isFile, bool isTopExists, bool isTop, bool isRecommendExists, bool isRecommend, bool isHotExists, bool isHot, bool isColorExists, bool isColor, string where)
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
                        //    $" ({ContentAttribute.ContentGroupNameCollection} = '{trimGroup}' OR CHARINDEX('{trimGroup},',{ContentAttribute.ContentGroupNameCollection}) > 0 OR CHARINDEX(',{trimGroup},',{ContentAttribute.ContentGroupNameCollection}) > 0 OR CHARINDEX(',{trimGroup}',{ContentAttribute.ContentGroupNameCollection}) > 0) OR ");

                        whereBuilder.Append(
                                $" ({ContentAttribute.ContentGroupNameCollection} = '{trimGroup}' OR {SqlUtils.GetInStr(ContentAttribute.ContentGroupNameCollection, trimGroup + ",")} OR {SqlUtils.GetInStr(ContentAttribute.ContentGroupNameCollection, "," + trimGroup + ",")} OR {SqlUtils.GetInStr(ContentAttribute.ContentGroupNameCollection, "," + trimGroup)}) OR ");
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
                        //    $" ({ContentAttribute.ContentGroupNameCollection} <> '{trimGroup}' AND CHARINDEX('{trimGroup},',{ContentAttribute.ContentGroupNameCollection}) = 0 AND CHARINDEX(',{trimGroup},',{ContentAttribute.ContentGroupNameCollection}) = 0 AND CHARINDEX(',{trimGroup}',{ContentAttribute.ContentGroupNameCollection}) = 0) AND ");

                        whereBuilder.Append(
                                $" ({ContentAttribute.ContentGroupNameCollection} <> '{trimGroup}' AND {SqlUtils.GetNotInStr(ContentAttribute.ContentGroupNameCollection, trimGroup + ",")} AND {SqlUtils.GetNotInStr(ContentAttribute.ContentGroupNameCollection, "," + trimGroup + ",")} AND {SqlUtils.GetNotInStr(ContentAttribute.ContentGroupNameCollection, "," + trimGroup)}) AND ");
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

        public string GetSelectCommendByDownloads(string tableName, int publishmentSystemId)
        {
            var whereString = new StringBuilder();
            whereString.Append(
                $"WHERE (PublishmentSystemID = {publishmentSystemId} AND IsChecked='True' AND FileUrl <> '') ");

            return BaiRongDataProvider.DatabaseDao.GetSelectSqlString(tableName, SqlUtils.Asterisk, whereString.ToString());
        }

        private int GetTaxis(int selectedId, string tableName)
        {
            string sqlString = $"SELECT Taxis FROM {tableName} WHERE (Id = {selectedId})";

            return BaiRongDataProvider.DatabaseDao.GetIntResult(sqlString);
        }

        private string GetSelectCommendByCondition(string tableName, int publishmentSystemId, List<int> nodeIdList, string searchType, string keyword, string dateFrom, string dateTo, ETriState checkedState, bool isNoDup, bool isTrashContent)
        {
            return GetSelectCommendByCondition(tableName, publishmentSystemId, nodeIdList, searchType, keyword, dateFrom, dateTo, checkedState, isNoDup, isTrashContent, false, string.Empty);
        }

        private string GetSelectCommendByCondition(string tableName, int publishmentSystemId, List<int> nodeIdList, string searchType, string keyword, string dateFrom, string dateTo, ETriState checkedState, bool isNoDup, bool isTrashContent, bool isWritingOnly, string userNameOnly)
        {
            if (nodeIdList == null || nodeIdList.Count == 0)
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
            var whereString = new StringBuilder("WHERE ");

            if (isTrashContent)
            {
                for (var i = 0; i < nodeIdList.Count; i++)
                {
                    var theNodeId = nodeIdList[i];
                    nodeIdList[i] = -theNodeId;
                }
            }

            whereString.Append(nodeIdList.Count == 1
                ? $"PublishmentSystemId = {publishmentSystemId} AND (NodeId = {nodeIdList[0]}) "
                : $"PublishmentSystemId = {publishmentSystemId} AND (NodeId IN ({TranslateUtils.ToSqlInStringWithoutQuote(nodeIdList)})) ");

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
                var list = TableMetadataManager.GetAllLowerAttributeNameList(tableName);
                whereString.Append(list.Contains(searchType.ToLower())
                    ? $"AND ({searchType} LIKE '%{keyword}%') "
                    : $"AND (SettingsXML LIKE '%{searchType}={keyword}%') ");
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

            if (isNoDup)
            {
                var sqlString = BaiRongDataProvider.DatabaseDao.GetSelectSqlString(tableName, "MIN(Id)", whereString + " GROUP BY Title");
                whereString.Append($"AND Id IN ({sqlString})");
            }

            if (!string.IsNullOrEmpty(userNameOnly))
            {
                whereString.Append($" AND AddUserName = '{userNameOnly}' ");
            }
            if (isWritingOnly)
            {
                whereString.Append(" AND WritingUserName <> '' ");
            }

            whereString.Append(" ").Append(orderByString);

            return BaiRongDataProvider.DatabaseDao.GetSelectSqlString(tableName, SqlUtils.Asterisk, whereString.ToString());
        }

        private List<int> GetContentIdListByTrash(int publishmentSystemId, string tableName)
        {
            string sqlString =
                $"SELECT Id FROM {tableName} WHERE PublishmentSystemId = {publishmentSystemId} AND NodeId < 0";
            return BaiRongDataProvider.DatabaseDao.GetIntList(sqlString);
        }

        private DataSet GetStlDataSourceChecked(string tableName, List<int> nodeIdList, int startNum, int totalNum, string orderByString, string whereString, bool isNoDup, LowerNameValueCollection others)
        {
            if (nodeIdList == null || nodeIdList.Count == 0)
            {
                return null;
            }
            var sqlWhereString = nodeIdList.Count == 1 ? $"WHERE (NodeId = {nodeIdList[0]} AND IsChecked = '{true}' {whereString})" : $"WHERE (NodeId IN ({TranslateUtils.ToSqlInStringWithoutQuote(nodeIdList)}) AND IsChecked = '{true}' {whereString})";

            if (isNoDup)
            {
                var sqlString = BaiRongDataProvider.DatabaseDao.GetSelectSqlString(tableName, "MIN(Id)", sqlWhereString + " GROUP BY Title");
                sqlWhereString += $" AND Id IN ({sqlString})";
            }

            if (others != null && others.Count > 0)
            {
                var lowerColumnNameList = TableMetadataManager.GetAllLowerAttributeNameList(tableName);
                foreach (var attributeName in others.Keys)
                {
                    if (lowerColumnNameList.Contains(attributeName.ToLower()))
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
                var sqlSelect = BaiRongDataProvider.DatabaseDao.GetSelectSqlString(tableName, totalNum, StlColumns, whereString, orderByString);
                dataset = ExecuteDataset(sqlSelect);
            }
            return dataset;
        }

        private DataSet GetStlDataSourceByStartNum(string tableName, int startNum, int totalNum, string whereString, string orderByString)
        {
            DataSet dataset = null;
            if (!string.IsNullOrEmpty(tableName))
            {
                var sqlSelect = BaiRongDataProvider.DatabaseDao.GetSelectSqlString(tableName, startNum, totalNum, StlColumns, whereString, orderByString);
                dataset = ExecuteDataset(sqlSelect);
            }
            return dataset;
        }

        private int GetTaxisToInsert(string tableName, int nodeId, bool isTop)
        {
            int taxis;

            if (isTop)
            {
                taxis = GetMaxTaxis(tableName, nodeId, true) + 1;
            }
            else
            {
                taxis = GetMaxTaxis(tableName, nodeId, false) + 1;
            }

            return taxis;
        }

        private ContentInfo GetContentInfo(IDataReader rdr)
        {
            var contentInfo = new ContentInfo();
            contentInfo.Load(rdr);
            contentInfo.Load(contentInfo.SettingsXml);

            return contentInfo;
        }

        private int GetCountOfContentAdd(string tableName, int publishmentSystemId, List<int> nodeIdList, DateTime begin, DateTime end, string userName)
        {
            string sqlString;
            if (string.IsNullOrEmpty(userName))
            {
                sqlString = nodeIdList.Count == 1
                    ? $"SELECT COUNT(Id) AS Num FROM {tableName} WHERE PublishmentSystemId = {publishmentSystemId} AND NodeId = {nodeIdList[0]} AND (AddDate BETWEEN {SqlUtils.GetComparableDate(begin)} AND {SqlUtils.GetComparableDate(end.AddDays(1))})"
                    : $"SELECT COUNT(Id) AS Num FROM {tableName} WHERE PublishmentSystemId = {publishmentSystemId} AND NodeId IN ({TranslateUtils.ToSqlInStringWithoutQuote(nodeIdList)}) AND (AddDate BETWEEN {SqlUtils.GetComparableDate(begin)} AND {SqlUtils.GetComparableDate(end.AddDays(1))})";
            }
            else
            {
                sqlString = nodeIdList.Count == 1
                    ? $"SELECT COUNT(Id) AS Num FROM {tableName} WHERE PublishmentSystemId = {publishmentSystemId} AND NodeId = {nodeIdList[0]} AND (AddDate BETWEEN {SqlUtils.GetComparableDate(begin)} AND {SqlUtils.GetComparableDate(end.AddDays(1))}) AND (AddUserName = '{userName}')"
                    : $"SELECT COUNT(Id) AS Num FROM {tableName} WHERE PublishmentSystemId = {publishmentSystemId} AND NodeId IN ({TranslateUtils.ToSqlInStringWithoutQuote(nodeIdList)}) AND (AddDate BETWEEN {SqlUtils.GetComparableDate(begin)} AND {SqlUtils.GetComparableDate(end.AddDays(1))}) AND (AddUserName = '{userName}')";
            }

            return BaiRongDataProvider.DatabaseDao.GetIntResult(sqlString);
        }

        private List<int> GetContentIdListChecked(string tableName, int nodeId, int totalNum, string orderByFormatString, string whereString)
        {
            var nodeIdList = new List<int>
            {
                nodeId
            };
            return GetContentIdListChecked(tableName, nodeIdList, totalNum, orderByFormatString, whereString);
        }

        private List<int> GetContentIdListChecked(string tableName, List<int> nodeIdList, int totalNum, string orderString, string whereString)
        {
            var list = new List<int>();

            if (nodeIdList == null || nodeIdList.Count == 0)
            {
                return list;
            }

            string sqlString;

            if (totalNum > 0)
            {
                sqlString = SqlUtils.ToTopSqlString(tableName, "Id",
                    nodeIdList.Count == 1
                        ? $"WHERE (NodeId = {nodeIdList[0]} AND IsChecked = '{true}' {whereString})"
                        : $"WHERE (NodeId IN ({TranslateUtils.ToSqlInStringWithoutQuote(nodeIdList)}) AND IsChecked = '{true}' {whereString})", orderString,
                    totalNum);
            }
            else
            {
                sqlString = nodeIdList.Count == 1
                    ? $"SELECT Id FROM {tableName} WHERE (NodeId = {nodeIdList[0]} AND IsChecked = '{true}' {whereString}) {orderString}"
                    : $"SELECT Id FROM {tableName} WHERE (NodeId IN ({TranslateUtils.ToSqlInStringWithoutQuote(nodeIdList)}) AND IsChecked = '{true}' {whereString}) {orderString}";
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

        private List<int> GetContentIdListChecked(string tableName, int nodeId, string orderByFormatString, string whereString)
        {
            return GetContentIdListChecked(tableName, nodeId, 0, orderByFormatString, whereString);
        }
        //private void UpdateIsChecked(string tableName, int publishmentSystemId, int nodeId, List<int> contentIdList, int translateNodeId, bool isAdmin, string userName, bool isChecked, int checkedLevel, string reasons, bool isCheck)
        //{
        //    if (isChecked)
        //    {
        //        checkedLevel = 0;
        //    }

        //    var checkDate = DateTime.Now;

        //    foreach (var contentId in contentIdList)
        //    {
        //        var settingsXml = GetValue(tableName, contentId, ContentAttribute.SettingsXml);
        //        var attributes = TranslateUtils.ToNameValueCollection(settingsXml);
        //        attributes[ContentAttribute.CheckIsAdmin] = isAdmin.ToString();
        //        attributes[ContentAttribute.CheckUserName] = userName;
        //        attributes[ContentAttribute.CheckCheckDate] = DateUtils.GetDateAndTimeString(checkDate);
        //        attributes[ContentAttribute.CheckReasons] = reasons;

        //        string sqlString =
        //            $"UPDATE {tableName} SET IsChecked = '{isChecked}', CheckedLevel = {checkedLevel}, SettingsXML = '{TranslateUtils.NameValueCollectionToString(attributes)}' WHERE Id = {contentId}";
        //        if (translateNodeId > 0)
        //        {
        //            sqlString =
        //                $"UPDATE {tableName} SET IsChecked = '{isChecked}', CheckedLevel = {checkedLevel}, SettingsXML = '{TranslateUtils.NameValueCollectionToString(attributes)}', NodeId = {translateNodeId} WHERE Id = {contentId}";
        //        }
        //        ExecuteNonQuery(sqlString);

        //        var checkInfo = new ContentCheckInfo(0, tableName, publishmentSystemId, nodeId, contentId, isAdmin, userName, isChecked, checkedLevel, checkDate, reasons);
        //        BaiRongDataProvider.ContentCheckDao.Insert(checkInfo);
        //    }
        //}

        //private void UpdatePhotos(string tableName, int contentId, int photos)
        //{
        //    string sqlString = $"UPDATE {tableName} SET Photos = {photos} WHERE Id = {contentId}";
        //    ExecuteNonQuery(sqlString);
        //}

        //private int GetReferenceId(string tableName, int contentId, out string linkUrl, out int nodeId)
        //{
        //    var referenceId = 0;
        //    nodeId = 0;
        //    linkUrl = string.Empty;
        //    try
        //    {
        //        string sqlString = $"SELECT ReferenceId, NodeId, LinkUrl FROM {tableName} WHERE Id = {contentId}";

        //        using (var rdr = ExecuteReader(sqlString))
        //        {
        //            if (rdr.Read())
        //            {
        //                referenceId = GetInt(rdr, 0);
        //                nodeId = GetInt(rdr, 1);
        //                linkUrl = GetString(rdr, 2);
        //            }
        //            rdr.Close();
        //        }
        //    }
        //    catch
        //    {
        //        // ignored
        //    }
        //    return referenceId;
        //}

        //private string GetSelectCommendByWhere(string tableName, int publishmentSystemId, List<int> nodeIdList, string where, ETriState checkedState)
        //{
        //    if (nodeIdList == null || nodeIdList.Count == 0)
        //    {
        //        return null;
        //    }

        //    var orderByString = ETaxisTypeUtils.GetContentOrderByString(ETaxisType.OrderByTaxisDesc);

        //    var whereString = new StringBuilder("WHERE ");

        //    whereString.Append(
        //        nodeIdList.Count == 1
        //            ? $"PublishmentSystemId = {publishmentSystemId} AND (NodeId = {nodeIdList[0]}) AND ({where}) "
        //            : $"PublishmentSystemId = {publishmentSystemId} AND (NodeId IN ({TranslateUtils.ToSqlInStringWithoutQuote(nodeIdList)})) AND ({where}) ");

        //    if (checkedState == ETriState.True)
        //    {
        //        whereString.Append("AND IsChecked='True' ");
        //    }
        //    else if (checkedState == ETriState.False)
        //    {
        //        whereString.Append("AND IsChecked='False' ");
        //    }

        //    whereString.Append(orderByString);

        //    return BaiRongDataProvider.DatabaseDao.GetSelectSqlString(tableName, SqlUtils.Asterisk, whereString.ToString());
        //}

        //private string GetSelectCommend(string tableName, int nodeId, ETriState checkedState)
        //{
        //    var orderByString = ETaxisTypeUtils.GetContentOrderByString(ETaxisType.OrderByTaxisDesc);

        //    var whereString = new StringBuilder();
        //    whereString.Append($"WHERE (NodeId = {nodeId}) ");

        //    if (checkedState == ETriState.True)
        //    {
        //        whereString.Append("AND IsChecked='True' ");
        //    }
        //    else if (checkedState == ETriState.False)
        //    {
        //        whereString.Append("AND IsChecked='False'");
        //    }

        //    //whereString.Append(orderByString);

        //    return BaiRongDataProvider.DatabaseDao.GetSelectSqlString(tableName, SqlUtils.Asterisk, whereString.ToString(), orderByString);
        //}

        //private string GetSelectCommend(string tableName, List<int> nodeIdList, ETriState checkedState)
        //{
        //    var orderByString = ETaxisTypeUtils.GetContentOrderByString(ETaxisType.OrderByTaxisDesc);

        //    var whereString = new StringBuilder();

        //    whereString.Append(nodeIdList.Count == 1
        //        ? $"WHERE (NodeId = {nodeIdList[0]}) "
        //        : $"WHERE (NodeId IN ({TranslateUtils.ToSqlInStringWithoutQuote(nodeIdList)})) ");

        //    if (checkedState == ETriState.True)
        //    {
        //        whereString.Append("AND IsChecked='True' ");
        //    }
        //    else if (checkedState == ETriState.False)
        //    {
        //        whereString.Append("AND IsChecked='False'");
        //    }

        //    whereString.Append(orderByString);

        //    return BaiRongDataProvider.DatabaseDao.GetSelectSqlString(tableName, SqlUtils.Asterisk, whereString.ToString());
        //}

        //private List<int> GetContentIdListByPublishmentSystemId(string tableName, int publishmentSystemId)
        //{
        //    var list = new List<int>();

        //    string sqlString = $"SELECT Id FROM {tableName} WHERE PublishmentSystemId = {publishmentSystemId}";
        //    using (var rdr = ExecuteReader(sqlString))
        //    {
        //        while (rdr.Read())
        //        {
        //            var contentId = GetInt(rdr, 0);
        //            list.Add(contentId);
        //        }
        //        rdr.Close();
        //    }
        //    return list;
        //}

        //private void DeleteContentsArchive(int publishmentSystemId, string tableName, List<int> contentIdList)
        //{
        //    if (!string.IsNullOrEmpty(tableName) && contentIdList != null && contentIdList.Count > 0)
        //    {
        //        string sqlString =
        //            $"DELETE FROM {tableName} WHERE PublishmentSystemId = {publishmentSystemId} AND Id IN ({TranslateUtils.ToSqlInStringWithoutQuote(contentIdList)})";
        //        ExecuteNonQuery(sqlString);
        //    }
        //}

        //private int GetContentId(string tableName, int nodeId, string attributeName, string value)
        //{
        //    var contentId = 0;
        //    string sqlString = $"SELECT Id FROM {tableName} WHERE (NodeId = {nodeId} AND {attributeName} = '{value}')";

        //    using (var rdr = ExecuteReader(sqlString))
        //    {
        //        if (rdr.Read())
        //        {
        //            contentId = GetInt(rdr, 0);
        //        }
        //        rdr.Close();
        //    }
        //    return contentId;
        //}

        //private int GetCountChecked(string tableName, int nodeId, int days)
        //{
        //    var whereString = string.Empty;
        //    if (days > 0)
        //    {
        //        whereString = "AND " + SqlUtils.GetDateDiffLessThanDays("AddDate", days.ToString());
        //    }
        //    return GetCountChecked(tableName, nodeId, whereString);
        //}

        //private int GetCountChecked(string tableName, int nodeId, string whereString)
        //{
        //    string sqlString =
        //        $"SELECT COUNT(*) AS TotalNum FROM {tableName} WHERE (NodeId = {nodeId} AND IsChecked = '{true}' {whereString})";

        //    return BaiRongDataProvider.DatabaseDao.GetIntResult(sqlString);
        //}

        //private int GetStlCountChecked(string tableName, List<int> nodeIdList, string whereString)
        //{
        //    if (nodeIdList == null || nodeIdList.Count == 0)
        //    {
        //        return 0;
        //    }
        //    var sqlWhereString = nodeIdList.Count == 1 ? $"WHERE (NodeId ={nodeIdList[0]} AND IsChecked = '{true}' {whereString})" : $"WHERE (NodeId IN ({TranslateUtils.ToSqlInStringWithoutQuote(nodeIdList)}) AND IsChecked = '{true}' {whereString})";

        //    string sqlString = $"SELECT COUNT(*) FROM {tableName} {sqlWhereString}";

        //    return BaiRongDataProvider.DatabaseDao.GetIntResult(sqlString);
        //}

        //private List<int> GetContentIdListCheck(int publishmentSystemId, int nodeId, string tableName)
        //{
        //    var list = new List<int>();

        //    string sqlString =
        //        $"SELECT Id FROM {tableName} WHERE PublishmentSystemId = {publishmentSystemId} AND NodeId = {nodeId} AND IsChecked = '{false}'";
        //    using (var rdr = ExecuteReader(sqlString))
        //    {
        //        while (rdr.Read())
        //        {
        //            var contentId = GetInt(rdr, 0);
        //            list.Add(contentId);
        //        }
        //        rdr.Close();
        //    }
        //    return list;
        //}

        //private List<int> GetContentIdListUnCheck(int publishmentSystemId, int nodeId, string tableName)
        //{
        //    var list = new List<int>();

        //    string sqlString =
        //        $"SELECT Id FROM {tableName} WHERE PublishmentSystemId = {publishmentSystemId} AND NodeId = {nodeId} AND IsChecked = '{true}'";
        //    using (var rdr = ExecuteReader(sqlString))
        //    {
        //        while (rdr.Read())
        //        {
        //            var contentId = GetInt(rdr, 0);
        //            list.Add(contentId);
        //        }
        //        rdr.Close();
        //    }
        //    return list;
        //}

        //private ContentInfo GetContentInfoNotTrash(string tableName, int contentId)
        //{
        //    ContentInfo info = null;
        //    if (contentId > 0)
        //    {
        //        if (!string.IsNullOrEmpty(tableName))
        //        {
        //            string sqlWhere = $"WHERE NodeId > 0 AND Id = {contentId}";
        //            var sqlSelect = BaiRongDataProvider.DatabaseDao.GetSelectSqlString(tableName, SqlUtils.Asterisk, sqlWhere);

        //            using (var rdr = ExecuteReader(sqlSelect))
        //            {
        //                if (rdr.Read())
        //                {
        //                    info = GetContentInfo(rdr);
        //                }
        //                rdr.Close();
        //            }
        //        }
        //    }

        //    return info;
        //}
        //private List<int> GetContentIdListChecked(string tableName, int nodeId, int totalNum, string orderByFormatString)
        //{
        //    return GetContentIdListChecked(tableName, nodeId, totalNum, orderByFormatString, string.Empty);
        //}

        //public int TrashContents(int publishmentSystemId, string tableName, List<int> contentIdList)
        //{
        //    if (!string.IsNullOrEmpty(tableName) && contentIdList != null && contentIdList.Count > 0)
        //    {
        //        string sqlString =
        //            $"UPDATE {tableName} SET NodeId = -NodeId, LastEditDate = {SqlUtils.GetComparableNow()} WHERE PublishmentSystemId = {publishmentSystemId} AND Id IN ({TranslateUtils.ToSqlInStringWithoutQuote(contentIdList)})";
        //        return ExecuteNonQuery(sqlString);
        //    }
        //    return 0;
        //}

        //public int TrashContentsByNodeId(int publishmentSystemId, string tableName, int nodeId)
        //{
        //    if (!string.IsNullOrEmpty(tableName))
        //    {
        //        string sqlString =
        //            $"UPDATE {tableName} SET NodeId = -NodeId, LastEditDate = {SqlUtils.GetComparableNow()} WHERE PublishmentSystemId = {publishmentSystemId} AND NodeId = {publishmentSystemId}";
        //        return ExecuteNonQuery(sqlString);
        //    }
        //    return 0;
        //}

        //public int DeleteContents(int publishmentSystemId, string tableName, List<int> contentIdList)
        //{
        //    if (!string.IsNullOrEmpty(tableName) && contentIdList != null && contentIdList.Count > 0)
        //    {
        //        TagUtils.RemoveTags(publishmentSystemId, contentIdList);

        //        string sqlString =
        //            $"DELETE FROM {tableName} WHERE PublishmentSystemId = {publishmentSystemId} AND Id IN ({TranslateUtils.ToSqlInStringWithoutQuote(contentIdList)})";
        //        return ExecuteNonQuery(sqlString);
        //    }
        //    return 0;
        //}

        //public int DeleteContentsByNodeId(int publishmentSystemId, string tableName, int nodeId, List<int> contentIdList)
        //{
        //    if (!string.IsNullOrEmpty(tableName))
        //    {
        //        TagUtils.RemoveTags(publishmentSystemId, contentIdList);

        //        string sqlString =
        //            $"DELETE FROM {tableName} WHERE PublishmentSystemId = {publishmentSystemId} AND NodeId = {nodeId}";
        //        return ExecuteNonQuery(sqlString);
        //    }
        //    return 0;
        //}

        //public int RestoreContentsByTrash(int publishmentSystemId, string tableName)
        //{
        //    if (!string.IsNullOrEmpty(tableName))
        //    {
        //        string sqlString =
        //            $"UPDATE {tableName} SET NodeId = -NodeId, LastEditDate = {SqlUtils.GetComparableNow()} WHERE PublishmentSystemId = {publishmentSystemId} AND NodeId < 0";
        //        return ExecuteNonQuery(sqlString);
        //    }
        //    return 0;
        //}

        //public string GetSelectCommend(string tableName, int publishmentSystemId, int nodeId, bool isSystemAdministrator, List<int> owningNodeIdList, string searchType, string keyword, string dateFrom, string dateTo, bool isSearchChildren, ETriState checkedState)
        //{
        //    return GetSelectCommend(tableName, publishmentSystemId, nodeId, isSystemAdministrator, owningNodeIdList, searchType, keyword, dateFrom, dateTo, isSearchChildren, checkedState, false, false);
        //}
        //public string GetWritingSelectCommend(string writingUserName, string tableName, int publishmentSystemId, List<int> nodeIdList, string searchType, string keyword, string dateFrom, string dateTo)
        //{
        //    if (nodeIdList == null || nodeIdList.Count == 0)
        //    {
        //        return null;
        //    }

        //    var whereString = new StringBuilder($"WHERE WritingUserName = '{writingUserName}' ");

        //    if (nodeIdList.Count == 1)
        //    {
        //        whereString.AppendFormat("AND PublishmentSystemId = {0} AND NodeId = {1} ", publishmentSystemId, nodeIdList[0]);
        //    }
        //    else
        //    {
        //        whereString.AppendFormat("AND PublishmentSystemId = {0} AND NodeId IN ({1}) ", publishmentSystemId, TranslateUtils.ToSqlInStringWithoutQuote(nodeIdList));
        //    }

        //    var dateString = string.Empty;
        //    if (!string.IsNullOrEmpty(dateFrom))
        //    {
        //        dateString = $" AND AddDate >= {SqlUtils.GetComparableDate(TranslateUtils.ToDateTime(dateFrom))} ";
        //    }
        //    if (!string.IsNullOrEmpty(dateTo))
        //    {
        //        dateString += $" AND AddDate <= {SqlUtils.GetComparableDate(TranslateUtils.ToDateTime(dateTo).AddDays(1))} ";
        //    }

        //    if (string.IsNullOrEmpty(keyword))
        //    {
        //        whereString.Append(dateString);
        //    }
        //    else
        //    {
        //        var list = TableMetadataManager.GetAllLowerAttributeNameList(tableName);
        //        if (list.Contains(searchType.ToLower()))
        //        {
        //            whereString.AppendFormat("AND ([{0}] LIKE '%{1}%') {2} ", searchType, keyword, dateString);
        //        }
        //    }

        //    return BaiRongDataProvider.DatabaseDao.GetSelectSqlString(tableName, SqlUtils.Asterisk, whereString.ToString());
        //}
    }
}
