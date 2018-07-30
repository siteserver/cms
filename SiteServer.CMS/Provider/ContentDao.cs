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
using SiteServer.CMS.StlParser.Cache;
using SiteServer.Plugin;
using SiteServer.Utils.Enumerations;
using Dapper;

namespace SiteServer.CMS.Provider
{
    public class ContentDao : DataProviderBase
    {
        private const int TaxisIsTopStartValue = 2000000000;

        private static string StlColumns { get; } = $"{ContentAttribute.Id}, {ContentAttribute.ChannelId}, {ContentAttribute.IsTop}, {ContentAttribute.AddDate}, {ContentAttribute.LastEditDate}, {ContentAttribute.Taxis}, {ContentAttribute.Hits}, {ContentAttribute.HitsByDay}, {ContentAttribute.HitsByWeek}, {ContentAttribute.HitsByMonth}";

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
                AttributeName = nameof(ContentInfo.WritingUserName),
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

        public string GetCreateTableCollectionInfoSqlString(string tableName)
        {
            var columnSqlStringList = new List<string>();

            var tableMetadataInfoList = TableMetadataManager.GetTableMetadataInfoList(tableName);
            if (tableMetadataInfoList.Count > 0)
            {
                foreach (var metadataInfo in tableMetadataInfoList)
                {
                    var columnSql = SqlUtils.GetColumnSqlString(metadataInfo.DataType, metadataInfo.AttributeName, metadataInfo.DataLength);
                    if (!string.IsNullOrEmpty(columnSql))
                    {
                        columnSqlStringList.Add(columnSql);
                    }
                }
            }

            var sqlBuilder = new StringBuilder();

            if (WebConfigUtils.DatabaseType == DatabaseType.MySql)
            {
                sqlBuilder.Append($@"CREATE TABLE `{tableName}` (");
                sqlBuilder.Append($@"
`{nameof(ContentInfo.Id)}` INT AUTO_INCREMENT,
`{nameof(ContentInfo.ChannelId)}` INT,
`{nameof(ContentInfo.SiteId)}` INT,
`{nameof(ContentInfo.AddUserName)}` VARCHAR(255),
`{nameof(ContentInfo.LastEditUserName)}` VARCHAR(255),
`{nameof(ContentInfo.WritingUserName)}` VARCHAR(255),
`{nameof(ContentInfo.LastEditDate)}` DATETIME,
`{nameof(ContentInfo.Taxis)}` INT,
`{nameof(ContentInfo.GroupNameCollection)}` VARCHAR(255),
`{nameof(ContentInfo.Tags)}` VARCHAR(255),
`{nameof(ContentInfo.SourceId)}` INT,
`{nameof(ContentInfo.ReferenceId)}` INT,
`{nameof(ContentInfo.IsChecked)}` VARCHAR(18),
`{nameof(ContentInfo.CheckedLevel)}` INT,
`{nameof(ContentInfo.Hits)}` INT,
`{nameof(ContentInfo.HitsByDay)}` INT,
`{nameof(ContentInfo.HitsByWeek)}` INT,
`{nameof(ContentInfo.HitsByMonth)}` INT,
`{nameof(ContentInfo.LastHitsDate)}` DATETIME,
`{nameof(ContentInfo.SettingsXml)}` LONGTEXT,
`{nameof(ContentInfo.Title)}` VARCHAR(255),
`{nameof(ContentInfo.IsTop)}` VARCHAR(18),
`{nameof(ContentInfo.IsRecommend)}` VARCHAR(18),
`{nameof(ContentInfo.IsHot)}` VARCHAR(18),
`{nameof(ContentInfo.IsColor)}` VARCHAR(18),
`{nameof(ContentInfo.LinkUrl)}` VARCHAR(200),
`{nameof(ContentInfo.AddDate)}` DATETIME,
");
            }
            else if (WebConfigUtils.DatabaseType == DatabaseType.SqlServer)
            {
                sqlBuilder.Append($@"CREATE TABLE {tableName} (");
                sqlBuilder.Append($@"
{nameof(ContentInfo.Id)} int IDENTITY (1, 1),
{nameof(ContentInfo.ChannelId)} int NULL,
{nameof(ContentInfo.SiteId)} int NULL,
{nameof(ContentInfo.AddUserName)} nvarchar (255) NULL,
{nameof(ContentInfo.LastEditUserName)} nvarchar (255) NULL,
{nameof(ContentInfo.WritingUserName)} nvarchar (255) NULL,
{nameof(ContentInfo.LastEditDate)} datetime NULL,
{nameof(ContentInfo.Taxis)} int NULL,
{nameof(ContentInfo.GroupNameCollection)} nvarchar (255) NULL,
{nameof(ContentInfo.Tags)} nvarchar (255) NULL,
{nameof(ContentInfo.SourceId)} int NULL,
{nameof(ContentInfo.ReferenceId)} int NULL,
{nameof(ContentInfo.IsChecked)} nvarchar (18) NULL,
{nameof(ContentInfo.CheckedLevel)} int NULL,
{nameof(ContentInfo.Hits)} int NULL,
{nameof(ContentInfo.HitsByDay)} int NULL,
{nameof(ContentInfo.HitsByWeek)} int NULL,
{nameof(ContentInfo.HitsByMonth)} int NULL,
{nameof(ContentInfo.LastHitsDate)} datetime NULL,
{nameof(ContentInfo.SettingsXml)} ntext NULL,
{nameof(ContentInfo.Title)} nvarchar (255) NULL,
{nameof(ContentInfo.IsTop)} nvarchar (18) NULL,
{nameof(ContentInfo.IsRecommend)} nvarchar (18) NULL,
{nameof(ContentInfo.IsHot)} nvarchar (18) NULL,
{nameof(ContentInfo.IsColor)} nvarchar (18) NULL,
{nameof(ContentInfo.LinkUrl)} nvarchar (200) NULL,
{nameof(ContentInfo.AddDate)} datetime NULL,
");
            }
            else if (WebConfigUtils.DatabaseType == DatabaseType.PostgreSql)
            {
                sqlBuilder.Append($@"CREATE TABLE {tableName} (");
                sqlBuilder.Append($@"
{nameof(ContentInfo.Id)} SERIAL,
{nameof(ContentInfo.ChannelId)} int4 NULL,
{nameof(ContentInfo.SiteId)} int4 NULL,
{nameof(ContentInfo.AddUserName)} varchar (255) NULL,
{nameof(ContentInfo.LastEditUserName)} varchar (255) NULL,
{nameof(ContentInfo.WritingUserName)} varchar (255) NULL,
{nameof(ContentInfo.LastEditDate)} timestamptz NULL,
{nameof(ContentInfo.Taxis)} int4 NULL,
{nameof(ContentInfo.GroupNameCollection)} varchar (255) NULL,
{nameof(ContentInfo.Tags)} varchar (255) NULL,
{nameof(ContentInfo.SourceId)} int4 NULL,
{nameof(ContentInfo.ReferenceId)} int4 NULL,
{nameof(ContentInfo.IsChecked)} varchar (18) NULL,
{nameof(ContentInfo.CheckedLevel)} int4 NULL,
{nameof(ContentInfo.Hits)} int4 NULL,
{nameof(ContentInfo.HitsByDay)} int4 NULL,
{nameof(ContentInfo.HitsByWeek)} int4 NULL,
{nameof(ContentInfo.HitsByMonth)} int4 NULL,
{nameof(ContentInfo.LastHitsDate)} timestamptz NULL,
{nameof(ContentInfo.SettingsXml)} text NULL,
{nameof(ContentInfo.Title)} varchar (255) NULL,
{nameof(ContentInfo.IsTop)} varchar (18) NULL,
{nameof(ContentInfo.IsRecommend)} varchar (18) NULL,
{nameof(ContentInfo.IsHot)} varchar (18) NULL,
{nameof(ContentInfo.IsColor)} varchar (18) NULL,
{nameof(ContentInfo.LinkUrl)} varchar (200) NULL,
{nameof(ContentInfo.AddDate)} timestamptz NULL,
");
            }
            else if (WebConfigUtils.DatabaseType == DatabaseType.Oracle)
            {
                sqlBuilder.Append($@"CREATE TABLE {tableName} (");
                sqlBuilder.Append($@"
{nameof(ContentInfo.Id)} NUMBER GENERATED BY DEFAULT ON NULL AS IDENTITY (START WITH 1 INCREMENT BY 1),
{nameof(ContentInfo.ChannelId)} number NULL,
{nameof(ContentInfo.SiteId)} number NULL,
{nameof(ContentInfo.AddUserName)} nvarchar2(255) NULL,
{nameof(ContentInfo.LastEditUserName)} nvarchar2(255) NULL,
{nameof(ContentInfo.WritingUserName)} nvarchar2(255) NULL,
{nameof(ContentInfo.LastEditDate)} timestamp(6) with time zone NULL,
{nameof(ContentInfo.Taxis)} number NULL,
{nameof(ContentInfo.GroupNameCollection)} nvarchar2(255) NULL,
{nameof(ContentInfo.Tags)} nvarchar2(255) NULL,
{nameof(ContentInfo.SourceId)} number NULL,
{nameof(ContentInfo.ReferenceId)} number NULL,
{nameof(ContentInfo.IsChecked)} nvarchar2(18) NULL,
{nameof(ContentInfo.CheckedLevel)} number NULL,
{nameof(ContentInfo.Hits)} number NULL,
{nameof(ContentInfo.HitsByDay)} number NULL,
{nameof(ContentInfo.HitsByWeek)} number NULL,
{nameof(ContentInfo.HitsByMonth)} number NULL,
{nameof(ContentInfo.LastHitsDate)} timestamp(6) with time zone NULL,
{nameof(ContentInfo.SettingsXml)} nclob NULL,
{nameof(ContentInfo.Title)} nvarchar2(255) NULL,
{nameof(ContentInfo.IsTop)} nvarchar2(18) NULL,
{nameof(ContentInfo.IsRecommend)} nvarchar2(18) NULL,
{nameof(ContentInfo.IsHot)} nvarchar2(18) NULL,
{nameof(ContentInfo.IsColor)} nvarchar2(18) NULL,
{nameof(ContentInfo.LinkUrl)} nvarchar2(200) NULL,
{nameof(ContentInfo.AddDate)} timestamp(6) with time zone NULL,
");
            }

            //添加后台定义字段
            foreach (var sqlString in columnSqlStringList)
            {
                sqlBuilder.Append(sqlString).Append(@",");
            }

            if (WebConfigUtils.DatabaseType == DatabaseType.MySql)
            {
                sqlBuilder.Append($@"
PRIMARY KEY ({nameof(ContentInfo.Id)})
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4
GO
CREATE INDEX `IX_{tableName}` ON `{tableName}`(`{nameof(ContentInfo.IsTop)}` DESC, `{nameof(ContentInfo.Taxis)}` DESC, `{nameof(ContentInfo.Id)}` DESC)
GO
CREATE INDEX `IX_{tableName}_Taxis` ON `{tableName}`(`{nameof(ContentInfo.Taxis)}` DESC)
GO");
            }
            else
            {
                sqlBuilder.Append($@"
CONSTRAINT PK_{tableName} PRIMARY KEY ({nameof(ContentInfo.Id)})
)
GO
CREATE INDEX IX_{tableName} ON {tableName}({nameof(ContentInfo.IsTop)} DESC, {nameof(ContentInfo.Taxis)} DESC, {nameof(ContentInfo.Id)} DESC)
GO
CREATE INDEX IX_{tableName}_Taxis ON {tableName}({nameof(ContentInfo.Taxis)} DESC)
GO");
            }

            return sqlBuilder.ToString();
        }

        public void SetValue(string tableName, int contentId, string name, string value)
        {
            string sqlString = $"UPDATE {tableName} SET {name} = '{value}' WHERE Id = {contentId}";

            ExecuteNonQuery(sqlString);

            Content.ClearCache();
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
                var json = GetValue(tableName, contentId, ContentAttribute.SettingsXml);
                var attributes = new ExtendedAttributes(json);
                attributes.Set(ContentAttribute.CheckUserName, userName);
                attributes.Set(ContentAttribute.CheckCheckDate, DateUtils.GetDateAndTimeString(checkDate));
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

            Content.ClearCache();
        }

        public void DeleteContentsByTrash(int siteId, string tableName)
        {
            if (string.IsNullOrEmpty(tableName)) return;

            var contentIdList = GetContentIdListByTrash(siteId, tableName);
            TagUtils.RemoveTags(siteId, contentIdList);

            string sqlString =
                $"DELETE FROM {tableName} WHERE SiteId = {siteId} AND ChannelId < 0";
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
    {nameof(ContentInfo.ChannelId)},
    {nameof(ContentInfo.SiteId)},
    {nameof(ContentInfo.AddUserName)},
    {nameof(ContentInfo.LastEditUserName)},
    {nameof(ContentInfo.WritingUserName)},
    {nameof(ContentInfo.LastEditDate)},
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
    @{nameof(ContentInfo.WritingUserName)},
    @{nameof(ContentInfo.LastEditDate)},
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
                GetParameter($"@{nameof(ContentInfo.ChannelId)}", DataType.Integer, contentInfo.ChannelId),
                GetParameter($"@{nameof(ContentInfo.SiteId)}", DataType.Integer, contentInfo.SiteId),
                GetParameter($"@{nameof(ContentInfo.AddUserName)}", DataType.VarChar, 255, contentInfo.AddUserName),
                GetParameter($"@{nameof(ContentInfo.LastEditUserName)}", DataType.VarChar, 255, contentInfo.LastEditUserName),
                GetParameter($"@{nameof(ContentInfo.WritingUserName)}", DataType.VarChar, 255, contentInfo.WritingUserName),
                GetParameter($"@{nameof(ContentInfo.LastEditDate)}", DataType.DateTime, contentInfo.LastEditDate),
                GetParameter($"@{nameof(ContentInfo.Taxis)}", DataType.Integer, contentInfo.Taxis),
                GetParameter($"@{nameof(ContentInfo.GroupNameCollection)}", DataType.VarChar, 255, contentInfo.GroupNameCollection),
                GetParameter($"@{nameof(ContentInfo.Tags)}", DataType.VarChar, 255, contentInfo.Tags),
                GetParameter($"@{nameof(ContentInfo.SourceId)}", DataType.Integer, contentInfo.SourceId),
                GetParameter($"@{nameof(ContentInfo.ReferenceId)}", DataType.Integer, contentInfo.ReferenceId),
                GetParameter($"@{nameof(ContentInfo.IsChecked)}", DataType.VarChar, 18, contentInfo.IsChecked.ToString()),
                GetParameter($"@{nameof(ContentInfo.CheckedLevel)}", DataType.Integer, contentInfo.CheckedLevel),
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
            //var sqlInsert = DataProvider.DatabaseDao.GetInsertSqlString(contentInfo.Attributes.GetExtendedAttributes(), tableName, out parms);

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
                contentInfo.Taxis = GetMaxTaxis(tableName, contentInfo.ChannelId, false) + 1;
            }
            else if (contentInfo.IsTop && contentInfo.Taxis < TaxisIsTopStartValue)
            {
                contentInfo.Taxis = GetMaxTaxis(tableName, contentInfo.ChannelId, true) + 1;
            }

            contentInfo.LastEditDate = DateTime.Now;

            //if (!string.IsNullOrEmpty(tableName))
            //{
            //    contentInfo.Attributes.BeforeExecuteNonQuery();
            //    sqlString = DataProvider.DatabaseDao.GetUpdateSqlString(contentInfo.Attributes.GetExtendedAttributes(), tableName, out parms);
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
    {nameof(ContentInfo.ChannelId)} = @{nameof(ContentInfo.ChannelId)},
    {nameof(ContentInfo.SiteId)} = @{nameof(ContentInfo.SiteId)},
    {nameof(ContentInfo.AddUserName)} = @{nameof(ContentInfo.AddUserName)},
    {nameof(ContentInfo.LastEditUserName)} = @{nameof(ContentInfo.LastEditUserName)},
    {nameof(ContentInfo.WritingUserName)} = @{nameof(ContentInfo.WritingUserName)},
    {nameof(ContentInfo.LastEditDate)} = @{nameof(ContentInfo.LastEditDate)},
    {nameof(ContentInfo.Taxis)} = @{nameof(ContentInfo.Taxis)},
    {nameof(ContentInfo.GroupNameCollection)} = @{nameof(ContentInfo.GroupNameCollection)},
    {nameof(ContentInfo.Tags)} = @{nameof(ContentInfo.Tags)},
    {nameof(ContentInfo.SourceId)} = @{nameof(ContentInfo.SourceId)},
    {nameof(ContentInfo.ReferenceId)} = @{nameof(ContentInfo.ReferenceId)},
    {nameof(ContentInfo.IsChecked)} = @{nameof(ContentInfo.IsChecked)},
    {nameof(ContentInfo.CheckedLevel)} = @{nameof(ContentInfo.CheckedLevel)},
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
                GetParameter($"@{nameof(ContentInfo.ChannelId)}", DataType.Integer, contentInfo.ChannelId),
                GetParameter($"@{nameof(ContentInfo.SiteId)}", DataType.Integer, contentInfo.SiteId),
                GetParameter($"@{nameof(ContentInfo.AddUserName)}", DataType.VarChar, 255, contentInfo.AddUserName),
                GetParameter($"@{nameof(ContentInfo.LastEditUserName)}", DataType.VarChar, 255, contentInfo.LastEditUserName),
                GetParameter($"@{nameof(ContentInfo.WritingUserName)}", DataType.VarChar, 255, contentInfo.WritingUserName),
                GetParameter($"@{nameof(ContentInfo.LastEditDate)}", DataType.DateTime, contentInfo.LastEditDate),
                GetParameter($"@{nameof(ContentInfo.Taxis)}", DataType.Integer, contentInfo.Taxis),
                GetParameter($"@{nameof(ContentInfo.GroupNameCollection)}", DataType.VarChar, 255, contentInfo.GroupNameCollection),
                GetParameter($"@{nameof(ContentInfo.Tags)}", DataType.VarChar, 255, contentInfo.Tags),
                GetParameter($"@{nameof(ContentInfo.SourceId)}", DataType.Integer, contentInfo.SourceId),
                GetParameter($"@{nameof(ContentInfo.ReferenceId)}", DataType.Integer, contentInfo.ReferenceId),
                GetParameter($"@{nameof(ContentInfo.IsChecked)}", DataType.VarChar, 18, contentInfo.IsChecked.ToString()),
                GetParameter($"@{nameof(ContentInfo.CheckedLevel)}", DataType.Integer, contentInfo.CheckedLevel),
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

        public void UpdateAutoPageContent(string tableName, SiteInfo siteInfo)
        {
            if (!siteInfo.Additional.IsAutoPageInTextEditor) return;

            string sqlString =
                $"SELECT Id, {BackgroundContentAttribute.Content} FROM {tableName} WHERE (SiteId = {siteInfo.Id})";

            using (var rdr = ExecuteReader(sqlString))
            {
                while (rdr.Read())
                {
                    var contentId = GetInt(rdr, 0);
                    var content = GetString(rdr, 1);
                    if (!string.IsNullOrEmpty(content))
                    {
                        content = ContentUtility.GetAutoPageContent(content, siteInfo.Additional.AutoPageWordNum);
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

        //public void TrashContents(int siteId, string tableName, List<int> contentIdList, int channelId)
        //{
        //    if (string.IsNullOrEmpty(tableName)) return;

        //    var referenceIdList = GetReferenceIdList(tableName, contentIdList);
        //    if (referenceIdList.Count > 0)
        //    {
        //        DeleteContents(siteId, tableName, referenceIdList);
        //    }
        //    var updateNum = 0;

        //    if (!string.IsNullOrEmpty(tableName) && contentIdList != null && contentIdList.Count > 0)
        //    {
        //        string sqlString =
        //            $"UPDATE {tableName} SET ChannelId = -ChannelId, LastEditDate = {SqlUtils.GetComparableNow()} WHERE SiteId = {siteId} AND Id IN ({TranslateUtils.ToSqlInStringWithoutQuote(contentIdList)})";
        //        updateNum = ExecuteNonQuery(sqlString);
        //    }

        //    if (updateNum <= 0) return;

        //    new Action(() =>
        //    {
        //        DataProvider.ChannelDao.UpdateContentNum(SiteManager.GetSiteInfo(siteId), channelId, true);
        //    }).BeginInvoke(null, null);

        //    Content.ClearCache();
        //}

        public void TrashContents(int siteId, string tableName, List<int> contentIdList)
        {
            if (string.IsNullOrEmpty(tableName)) return;

            var referenceIdList = GetReferenceIdList(tableName, contentIdList);
            if (referenceIdList.Count > 0)
            {
                DeleteContents(siteId, tableName, referenceIdList);
            }

            var updateNum = 0;

            if (!string.IsNullOrEmpty(tableName) && contentIdList != null && contentIdList.Count > 0)
            {
                string sqlString =
                    $"UPDATE {tableName} SET ChannelId = -ChannelId, LastEditDate = {SqlUtils.GetComparableNow()} WHERE SiteId = {siteId} AND Id IN ({TranslateUtils.ToSqlInStringWithoutQuote(contentIdList)})";
                updateNum = ExecuteNonQuery(sqlString);
            }

            if (updateNum <= 0) return;

            new Action(() =>
            {
                DataProvider.ChannelDao.UpdateContentNum(SiteManager.GetSiteInfo(siteId));
            }).BeginInvoke(null, null);

            Content.ClearCache();
        }

        public void TrashContentsByChannelId(int siteId, string tableName, int channelId)
        {
            if (string.IsNullOrEmpty(tableName)) return;

            var contentIdList = GetContentIdList(tableName, channelId);
            var referenceIdList = GetReferenceIdList(tableName, contentIdList);
            if (referenceIdList.Count > 0)
            {
                DeleteContents(siteId, tableName, referenceIdList);
            }
            var updateNum = 0;

            if (!string.IsNullOrEmpty(tableName))
            {
                string sqlString =
                    $"UPDATE {tableName} SET ChannelId = -ChannelId, LastEditDate = {SqlUtils.GetComparableNow()} WHERE SiteId = {siteId} AND ChannelId = {siteId}";
                updateNum = ExecuteNonQuery(sqlString);
            }

            if (updateNum <= 0) return;

            new Action(() =>
            {
                DataProvider.ChannelDao.UpdateContentNum(SiteManager.GetSiteInfo(siteId), channelId, true);
            }).BeginInvoke(null, null);

            Content.ClearCache();
        }

        public void DeleteContent(string tableName, SiteInfo siteInfo, int channelId, int contentId)
        {
            if (string.IsNullOrEmpty(tableName)) return;

            if (!string.IsNullOrEmpty(tableName) && contentId > 0)
            {
                TagUtils.RemoveTags(siteInfo.Id, contentId);

                string sqlString =
                    $"DELETE FROM {tableName} WHERE SiteId = {siteInfo.Id} AND Id = {contentId}";
                ExecuteNonQuery(sqlString);
            }

            if (channelId <= 0) return;

            new Action(() =>
            {
                DataProvider.ChannelDao.UpdateContentNum(siteInfo, channelId, true);
            }).BeginInvoke(null, null);

            Content.ClearCache();
        }

        public void DeleteContents(int siteId, string tableName, List<int> contentIdList, int channelId)
        {
            if (string.IsNullOrEmpty(tableName)) return;

            var deleteNum = 0;

            if (!string.IsNullOrEmpty(tableName) && contentIdList != null && contentIdList.Count > 0)
            {
                TagUtils.RemoveTags(siteId, contentIdList);

                string sqlString =
                    $"DELETE FROM {tableName} WHERE SiteId = {siteId} AND Id IN ({TranslateUtils.ToSqlInStringWithoutQuote(contentIdList)})";
                deleteNum = ExecuteNonQuery(sqlString);
            }

            if (channelId <= 0 || deleteNum <= 0) return;

            new Action(() =>
            {
                DataProvider.ChannelDao.UpdateContentNum(SiteManager.GetSiteInfo(siteId), channelId, true);
            }).BeginInvoke(null, null);

            Content.ClearCache();
        }

        public void DeleteContentsByChannelId(int siteId, string tableName, int channelId)
        {
            if (string.IsNullOrEmpty(tableName)) return;

            var contentIdList = GetContentIdListChecked(tableName, channelId, string.Empty);

            TagUtils.RemoveTags(siteId, contentIdList);

            string sqlString =
                $"DELETE FROM {tableName} WHERE SiteId = {siteId} AND ChannelId = {channelId}";
            var deleteNum = ExecuteNonQuery(sqlString);

            if (channelId <= 0 || deleteNum <= 0) return;

            new Action(() =>
            {
                DataProvider.ChannelDao.UpdateContentNum(SiteManager.GetSiteInfo(siteId), channelId, true);
            }).BeginInvoke(null, null);

            Content.ClearCache();
        }

        public void DeleteContentsByDeletedChannelIdList(IDbTransaction trans, SiteInfo siteInfo, List<int> channelIdList)
        {
            foreach (var channelId in channelIdList)
            {
                var tableName = ChannelManager.GetTableName(siteInfo, channelId);
                if (!string.IsNullOrEmpty(tableName))
                {
                    ExecuteNonQuery(trans, $"DELETE FROM {tableName} WHERE SiteId = {siteInfo.Id} AND {nameof(ContentInfo.ChannelId)} = {channelId}");
                }
            }

            Content.ClearCache();
        }

        public string GetCountSqlString(string tableName, int channelId)
        {
            return $"SELECT COUNT(*) AS ContentNum FROM {tableName} WHERE {ContentAttribute.ChannelId} = {channelId}";
        }

        public void RestoreContentsByTrash(int siteId, string tableName)
        {
            var updateNum = 0;

            if (!string.IsNullOrEmpty(tableName))
            {
                string sqlString =
                    $"UPDATE {tableName} SET ChannelId = -ChannelId, LastEditDate = {SqlUtils.GetComparableNow()} WHERE SiteId = {siteId} AND ChannelId < 0";
                updateNum = ExecuteNonQuery(sqlString);
            }

            if (updateNum <= 0) return;

            new Action(() =>
            {
                DataProvider.ChannelDao.UpdateContentNum(SiteManager.GetSiteInfo(siteId));
            }).BeginInvoke(null, null);

            Content.ClearCache();
        }

        private void SetTaxis(int id, int taxis, string tableName)
        {
            string sqlString = $"UPDATE {tableName} SET Taxis = {taxis} WHERE Id = {id}";
            ExecuteNonQuery(sqlString);

            Content.ClearCache();
        }

        private void DeleteContents(int siteId, string tableName, List<int> contentIdList)
        {
            if (string.IsNullOrEmpty(tableName)) return;

            var deleteNum = 0;

            if (!string.IsNullOrEmpty(tableName) && contentIdList != null && contentIdList.Count > 0)
            {
                TagUtils.RemoveTags(siteId, contentIdList);

                string sqlString =
                    $"DELETE FROM {tableName} WHERE SiteId = {siteId} AND Id IN ({TranslateUtils.ToSqlInStringWithoutQuote(contentIdList)})";
                deleteNum = ExecuteNonQuery(sqlString);
            }

            if (deleteNum <= 0) return;

            new Action(() =>
            {
                DataProvider.ChannelDao.UpdateContentNum(SiteManager.GetSiteInfo(siteId));
            }).BeginInvoke(null, null);

            Content.ClearCache();
        }

        public void DeletePreviewContents(int siteId, string tableName, ChannelInfo channelInfo)
        {
            if (!string.IsNullOrEmpty(tableName))
            {
                channelInfo.Additional.IsPreviewContents = false;
                DataProvider.ChannelDao.UpdateAdditional(channelInfo);

                string sqlString =
                    $"DELETE FROM {tableName} WHERE SiteId = {siteId} AND ChannelId = {channelInfo.Id} AND SourceId = {SourceManager.Preview}";
                DataProvider.DatabaseDao.ExecuteSql(sqlString);
            }
        }

        public void TidyUp(string tableName, int channelId, string attributeName, bool isDesc)
        {
            var taxisDirection = isDesc ? "ASC" : "DESC";//升序,但由于页面排序是按Taxis的Desc排序的，所以这里sql里面的ASC/DESC取反

            string sqlString =
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

            Content.ClearCache();
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        public bool UpdateTaxisToUp(string tableName, int channelId, int contentId, bool isTop)
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
                SetTaxis(contentId, higherTaxis, tableName);
                //Set The Higher Class Taxis To Lower Level
                SetTaxis(higherId, selectedTaxis, tableName);
                return true;
            }
            return false;
        }

        public bool UpdateTaxisToDown(string tableName, int channelId, int contentId, bool isTop)
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
                SetTaxis(contentId, lowerTaxis, tableName);
                //Set The Lower Class Taxis To Higher Level
                SetTaxis(lowerId, selectedTaxis, tableName);
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

        public string GetValue(string tableName, int contentId, string name)
        {
            string sqlString = $"SELECT {name} FROM {tableName} WHERE (Id = {contentId})";
            return DataProvider.DatabaseDao.GetString(sqlString);
        }

        public void AddContentGroupList(string tableName, int contentId, List<string> contentGroupList)
        {
            var list = TranslateUtils.StringCollectionToStringList(GetValue(tableName, contentId, ContentAttribute.GroupNameCollection));
            foreach (var groupName in contentGroupList)
            {
                if (!list.Contains(groupName)) list.Add(groupName);
            }
            SetValue(tableName, contentId, ContentAttribute.GroupNameCollection, TranslateUtils.ObjectCollectionToString(list));
        }       

        //public int GetReferenceId(string tableName, int contentId, out string linkUrl)
        //{
        //    var referenceId = 0;
        //    linkUrl = string.Empty;
        //    try
        //    {
        //        string sqlString = $"SELECT ReferenceId, LinkUrl FROM {tableName} WHERE Id = {contentId}";

        //        using (var rdr = ExecuteReader(sqlString))
        //        {
        //            if (rdr.Read())
        //            {
        //                referenceId = GetInt(rdr, 0);
        //                linkUrl = GetString(rdr, 1);
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

        public List<int> GetReferenceIdList(string tableName, List<int> contentIdList)
        {
            var list = new List<int>();
            string sqlString =
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

        //public string GetSqlString(string tableName, int channelId, ETriState checkedState, string userNameOnly)
        //{
        //    var orderByString = ETaxisTypeUtils.GetContentOrderByString(ETaxisType.OrderByTaxisDesc);

        //    var whereString = new StringBuilder();
        //    whereString.Append($"WHERE {nameof(ContentAttribute.ChannelId)} = {channelId} AND {nameof(ContentAttribute.SourceId)} != {SourceManager.Preview} ");

        //    if (checkedState == ETriState.True)
        //    {
        //        whereString.Append($"AND IsChecked='{true}' ");
        //    }
        //    else if (checkedState == ETriState.False)
        //    {
        //        whereString.Append($"AND IsChecked='{false}'");
        //    }

        //    if (!string.IsNullOrEmpty(userNameOnly))
        //    {
        //        whereString.Append($" AND AddUserName = '{userNameOnly}' ");
        //    }

        //    return DataProvider.DatabaseDao.GetSelectSqlString(tableName, SqlUtils.Asterisk, whereString.ToString(), orderByString);
        //}

        public string GetSelectCommandByHitsAnalysis(string tableName, int siteId)
        {
            var orderByString = ETaxisTypeUtils.GetContentOrderByString(ETaxisType.OrderByTaxisDesc);

            var whereString = new StringBuilder();
            whereString.Append($"AND IsChecked='{true}' AND SiteId = {siteId} AND Hits > 0");
            whereString.Append(orderByString);

            return DataProvider.DatabaseDao.GetSelectSqlString(tableName, SqlUtils.Asterisk, whereString.ToString());
        }

        public int GetTotalHits(string tableName, int siteId)
        {
            return DataProvider.DatabaseDao.GetIntResult($"SELECT SUM(Hits) FROM {tableName} WHERE IsChecked='{true}' AND SiteId = {siteId} AND Hits > 0");
        }

        public int GetFirstContentId(string tableName, int channelId)
        {
            string sqlString = $"SELECT Id FROM {tableName} WHERE ChannelId = {channelId} ORDER BY Taxis DESC, Id DESC";
            return DataProvider.DatabaseDao.GetIntResult(sqlString);
        }

        public List<int> GetContentIdList(string tableName, int channelId)
        {
            var list = new List<int>();

            string sqlString = $"SELECT Id FROM {tableName} WHERE ChannelId = {channelId}";
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

            string sqlString = $"SELECT Id FROM {tableName} WHERE ChannelId = {channelId}";
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

            string sqlString = $"SELECT Id FROM {tableName} WHERE SiteId = {siteId} AND ChannelId = {channelId} AND IsChecked = '{true}'";
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

        //public List<string> GetValueList(string tableName, int channelId, string name)
        //{
        //    string sqlString = $"SELECT {name} FROM {tableName} WHERE ChannelId = {channelId}";
        //    return DataProvider.DatabaseDao.GetStringList(sqlString);
        //}

        public List<string> GetValueListByStartString(string tableName, int channelId, string name, string startString, int totalNum)
        {
            var inStr = SqlUtils.GetInStr(name, startString);
            var sqlString = SqlUtils.GetDistinctTopSqlString(tableName, name, $"WHERE ChannelId = {channelId} AND {inStr}", string.Empty, totalNum);
            return DataProvider.DatabaseDao.GetStringList(sqlString);
        }

        public int GetChannelId(string tableName, int contentId)
        {
            var channelId = 0;
            string sqlString = $"SELECT {ContentAttribute.ChannelId} FROM {tableName} WHERE (Id = {contentId})";

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

        public int GetCount(string tableName, int channelId)
        {
            string sqlString = $"SELECT COUNT(*) AS TotalNum FROM {tableName} WHERE (ChannelId = {channelId})";

            return DataProvider.DatabaseDao.GetIntResult(sqlString);
        }

        public int GetSequence(string tableName, int channelId, int contentId)
        {
            string sqlString =
                $"SELECT COUNT(*) AS TotalNum FROM {tableName} WHERE ChannelId = {channelId} AND IsChecked = '{true}' AND Taxis < (SELECT Taxis FROM {tableName} WHERE (Id = {contentId}))";

            return DataProvider.DatabaseDao.GetIntResult(sqlString) + 1;
        }

        public string GetSqlStringOfAdminExcludeRecycle(string tableName, int siteId, DateTime begin, DateTime end)
        {
            string sqlString = $@"select userName,SUM(addCount) as addCount, SUM(updateCount) as updateCount from( 
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

        public List<int> GetChannelIdListCheckedByLastEditDateHour(string tableName, int siteId, int hour)
        {
            var list = new List<int>();

            string sqlString =
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

        public DataSet GetDataSetOfAdminExcludeRecycle(string tableName, int siteId, DateTime begin, DateTime end)
        {
            var sqlString = GetSqlStringOfAdminExcludeRecycle(tableName, siteId, begin, end);

            return ExecuteDataset(sqlString);
        }

        public int Insert(string tableName, SiteInfo siteInfo, IContentInfo contentInfo)
        {
            var taxis = GetTaxisToInsert(tableName, contentInfo.ChannelId, contentInfo.IsTop);
            return Insert(tableName, siteInfo, contentInfo, true, taxis);
        }

        public int InsertPreview(string tableName, SiteInfo siteInfo, ChannelInfo channelInfo, ContentInfo contentInfo)
        {
            channelInfo.Additional.IsPreviewContents = true;
            DataProvider.ChannelDao.UpdateAdditional(channelInfo);

            contentInfo.SourceId = SourceManager.Preview;
            return Insert(tableName, siteInfo, contentInfo, false, 0);
        }

        public int Insert(string tableName, SiteInfo siteInfo, IContentInfo contentInfo, bool isUpdateContentNum, int taxis)
        {
            var contentId = 0;

            if (!string.IsNullOrEmpty(tableName))
            {
                if (siteInfo.Additional.IsAutoPageInTextEditor && contentInfo.ContainsKey(BackgroundContentAttribute.Content))
                {
                    contentInfo.Set(BackgroundContentAttribute.Content, ContentUtility.GetAutoPageContent(contentInfo.GetString(BackgroundContentAttribute.Content), siteInfo.Additional.AutoPageWordNum));
                }

                contentInfo.Taxis = taxis;

                contentId = Insert(tableName, contentInfo);

                if (isUpdateContentNum)
                {
                    new Action(() =>
                    {
                        DataProvider.ChannelDao.UpdateContentNum(SiteManager.GetSiteInfo(contentInfo.SiteId), contentInfo.ChannelId, true);
                    }).BeginInvoke(null, null);
                }

                Content.ClearCache();
            }

            return contentId;
        }

        public void Update(string tableName, SiteInfo siteInfo, IContentInfo contentInfo)
        {
            if (siteInfo.Additional.IsAutoPageInTextEditor && contentInfo.ContainsKey(BackgroundContentAttribute.Content))
            {
                contentInfo.Set(BackgroundContentAttribute.Content, ContentUtility.GetAutoPageContent(contentInfo.GetString(BackgroundContentAttribute.Content), siteInfo.Additional.AutoPageWordNum));
            }

            Update(tableName, contentInfo);

            Content.ClearCache();
        }

        public ContentInfo GetContentInfo(string tableName, int contentId)
        {
            if (string.IsNullOrEmpty(tableName) || contentId <= 0) return null;

            ContentInfo info = null;

            string sqlWhere = $"WHERE Id = {contentId}";
            var sqlSelect = DataProvider.DatabaseDao.GetSelectSqlString(tableName, SqlUtils.Asterisk, sqlWhere);

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

            var channelId = DataProvider.ChannelDao.GetIdByChannelIdOrChannelIndexOrChannelName(siteId, siteId, channelIndex, channelName);
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

            var tableName = ChannelManager.GetTableName(siteInfo, channelInfo);
            //var styleInfoList = RelatedIdentities.GetTableStyleInfoList(siteInfo, channelInfo.Id);

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
                foreach (int theChannelId in channelIdList)
                {
                    if (owningChannelIdList.Contains(theChannelId))
                    {
                        list.Add(theChannelId);
                    }
                }
            }

            return GetSqlStringByCondition(tableName, siteId, list, searchType, keyword, dateFrom, dateTo, checkedState, isTrashContent);
        }

        //public string GetSqlString(string tableName, int siteId, int channelId, bool isSystemAdministrator, List<int> owningChannelIdList, string searchType, string keyword, string dateFrom, string dateTo, bool isSearchChildren, ETriState checkedState, bool isTrashContent, bool isWritingOnly, string userNameOnly)
        //{
        //    var channelInfo = ChannelManager.GetChannelInfo(siteId, channelId);
        //    var channelIdList = ChannelManager.GetChannelIdList(channelInfo, isSearchChildren ? EScopeType.All : EScopeType.Self, string.Empty, string.Empty, channelInfo.ContentModelPluginId);

        //    var list = new List<int>();
        //    if (isSystemAdministrator)
        //    {
        //        list = channelIdList;
        //    }
        //    else
        //    {
        //        foreach (int theChannelId in channelIdList)
        //        {
        //            if (owningChannelIdList.Contains(theChannelId))
        //            {
        //                list.Add(theChannelId);
        //            }
        //        }
        //    }

        //    return GetSqlStringByCondition(tableName, siteId, list, searchType, keyword, dateFrom, dateTo, checkedState, isTrashContent, isWritingOnly, userNameOnly);
        //}

        public string GetSqlStringByContentGroup(string tableName, string contentGroupName, int siteId)
        {
            contentGroupName = PageUtils.FilterSql(contentGroupName);
            string sqlString =
                $"SELECT * FROM {tableName} WHERE SiteId = {siteId} AND ChannelId > 0 AND (GroupNameCollection LIKE '{contentGroupName},%' OR GroupNameCollection LIKE '%,{contentGroupName}' OR GroupNameCollection  LIKE '%,{contentGroupName},%'  OR GroupNameCollection='{contentGroupName}')";
            return sqlString;
        }

        public DataSet GetStlDataSourceChecked(List<int> channelIdList, string tableName, int startNum, int totalNum, string orderByString, string whereString, LowerNameValueCollection others)
        {
            return GetStlDataSourceChecked(tableName, channelIdList, startNum, totalNum, orderByString, whereString, others);
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
                return DataProvider.DatabaseDao.GetSelectSqlString(tableName, startNum, totalNum, StlColumns, sqlWhereString, orderByString);
            }
            return string.Empty;
        }

        public string GetStlSqlStringCheckedBySearch(string tableName, int startNum, int totalNum, string orderByString, string whereString)
        {
            var sqlWhereString =
                    $"WHERE (ChannelId > 0 AND IsChecked = '{true}' {whereString})";

            if (!string.IsNullOrEmpty(tableName))
            {
                return DataProvider.DatabaseDao.GetSelectSqlString(tableName, startNum, totalNum, TranslateUtils.ObjectCollectionToString(ContentAttribute.AllAttributesLowercase), sqlWhereString, orderByString);
            }
            return string.Empty;
        }

        public List<int> GetIdListBySameTitle(string tableName, int channelId, string title)
        {
            var list = new List<int>();
            string sql = $"SELECT Id FROM {tableName} WHERE ChannelId = {channelId} AND Title = '{title}'";
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

        public List<IContentInfo> GetListByLimitAndOffset(string tableName, string whereString, string orderString, int limit, int offset)
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
                if (WebConfigUtils.DatabaseType == DatabaseType.MySql)
                {
                    sqlString = $"SELECT * FROM {tableName} {firstWhere} ORDER BY {order} limit {limit} offset {offset}";
                }
                else if (WebConfigUtils.DatabaseType == DatabaseType.SqlServer)
                {
                    sqlString = $@"SELECT TOP {limit} * FROM {tableName} WHERE Id NOT IN (SELECT TOP {offset} Id FROM {tableName} {firstWhere} ORDER BY {order}) {secondWhere} ORDER BY {order}";
                }
                else if (WebConfigUtils.DatabaseType == DatabaseType.PostgreSql)
                {
                    sqlString = $"SELECT * FROM {tableName} {firstWhere} ORDER BY {order} limit {limit} offset {offset}";
                }
                else if (WebConfigUtils.DatabaseType == DatabaseType.Oracle)
                {
                    sqlString = $"SELECT * FROM {tableName} {firstWhere} ORDER BY {order} OFFSET {offset} ROWS FETCH NEXT {limit} ROWS ONLY";
                }
            }
            else if (limit > 0)
            {
                if (WebConfigUtils.DatabaseType == DatabaseType.MySql)
                {
                    sqlString = $"SELECT * FROM {tableName} {firstWhere} ORDER BY {order} limit {limit}";
                }
                else if (WebConfigUtils.DatabaseType == DatabaseType.SqlServer)
                {
                    sqlString = $@"SELECT TOP {limit} * FROM {tableName} {firstWhere} ORDER BY {order}";
                }
                else if (WebConfigUtils.DatabaseType == DatabaseType.PostgreSql)
                {
                    sqlString = $"SELECT * FROM {tableName} {firstWhere} ORDER BY {order} limit {limit}";
                }
                else if (WebConfigUtils.DatabaseType == DatabaseType.Oracle)
                {
                    sqlString = $"SELECT * FROM {tableName} {firstWhere} ORDER BY {order} FETCH FIRST {limit} ROWS ONLY";
                }
            }
            else if (offset > 0)
            {
                if (WebConfigUtils.DatabaseType == DatabaseType.MySql)
                {
                    sqlString = $"SELECT * FROM {tableName} {firstWhere} ORDER BY {order} offset {offset}";
                }
                else if (WebConfigUtils.DatabaseType == DatabaseType.SqlServer)
                {
                    sqlString =
                            $@"SELECT * FROM {tableName} WHERE Id NOT IN (SELECT TOP {offset} Id FROM {tableName} {firstWhere} ORDER BY {order}) {secondWhere} ORDER BY {order}";
                }
                else if (WebConfigUtils.DatabaseType == DatabaseType.PostgreSql)
                {
                    sqlString = $"SELECT * FROM {tableName} {firstWhere} ORDER BY {order} offset {offset}";
                }
                else if (WebConfigUtils.DatabaseType == DatabaseType.Oracle)
                {
                    sqlString = $"SELECT * FROM {tableName} {firstWhere} ORDER BY {order} OFFSET {offset} ROWS";
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

        public int GetCount(string tableName, string whereString)
        {
            if (!string.IsNullOrEmpty(whereString))
            {
                whereString = whereString.Replace("WHERE ", string.Empty).Replace("where ", string.Empty);
            }
            whereString = string.IsNullOrEmpty(whereString) ? string.Empty : $"WHERE {whereString}";

            string sqlString = $"SELECT COUNT(*) FROM {tableName} {whereString}";

            return DataProvider.DatabaseDao.GetIntResult(sqlString);
        }

        public List<KeyValuePair<int, int>> GetCountListUnChecked(PermissionManager permissionManager, string tableName)
        {
            var list = new List<KeyValuePair<int, int>>();

            var siteIdList = permissionManager.SiteIdList;
            var owningChannelIdList = permissionManager.OwningChannelIdList;

            var siteIdArrayList = SiteManager.GetSiteIdList();
            foreach (var siteId in siteIdArrayList)
            {
                if (!siteIdList.Contains(siteId)) continue;

                var siteInfo = SiteManager.GetSiteInfo(siteId);
                if (!permissionManager.IsSystemAdministrator)
                {
                    //if (!owningChannelIdArrayList.Contains(psID)) continue;
                    //if (!AdminUtility.HasChannelPermissions(psID, psID, AppManager.CMS.Permission.Channel.ContentCheck)) continue;

                    var isContentCheck = false;
                    foreach (var theChannelId in owningChannelIdList)
                    {
                        if (permissionManager.HasChannelPermissions(siteId, theChannelId, ConfigManager.ChannelPermissions.ContentCheck))
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
                var isChecked = CheckManager.GetUserCheckLevel(permissionManager, siteInfo, siteInfo.Id, out checkedLevel);
                var checkLevelList = CheckManager.LevelInt.GetCheckLevelListOfNeedCheck(siteInfo, isChecked, checkedLevel);
                var sqlString = permissionManager.IsSystemAdministrator ? $"SELECT COUNT(*) AS TotalNum FROM {tableName} WHERE (SiteId = {siteId} AND ChannelId > 0 AND IsChecked = '{false}' AND CheckedLevel IN ({TranslateUtils.ToSqlInStringWithoutQuote(checkLevelList)}))" : $"SELECT COUNT(*) AS TotalNum FROM {tableName} WHERE (SiteId = {siteId} AND ChannelId IN ({TranslateUtils.ToSqlInStringWithoutQuote(owningChannelIdList)}) AND IsChecked = '{false}' AND CheckedLevel IN ({TranslateUtils.ToSqlInStringWithoutQuote(checkLevelList)}))";

                var count = DataProvider.DatabaseDao.GetIntResult(sqlString);
                if (count > 0)
                {
                    list.Add(new KeyValuePair<int, int>(siteId, count));
                }
            }
            return list;
        }

        public int GetCountCheckedImage(int siteId, int channelId)
        {
            var tableName = SiteManager.GetSiteInfo(siteId).TableName;
            string sqlString =
                $"SELECT COUNT(*) AS TotalNum FROM {tableName} WHERE (ChannelId = {channelId} AND ImageUrl <> '' AND {ContentAttribute.IsChecked} = '{true}')";

            return DataProvider.DatabaseDao.GetIntResult(sqlString);
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

        //public string GetSqlStringByDownloads(string tableName, int siteId)
        //{
        //    return DataProvider.DatabaseDao.GetSelectSqlString(tableName, SqlUtils.Asterisk, $"WHERE (SiteId = {siteId} AND IsChecked='True' AND FileUrl <> '') ");
        //}

        private int GetTaxis(int selectedId, string tableName)
        {
            var sqlString = $"SELECT Taxis FROM {tableName} WHERE (Id = {selectedId})";

            return DataProvider.DatabaseDao.GetIntResult(sqlString);
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
                var list = TableMetadataManager.GetAllLowerAttributeNameList(tableName);
                if (list.Contains(searchType.ToLower()))
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
                whereString.Append($" AND AddUserName = '{userNameOnly}' ");
            }
            if (isWritingOnly)
            {
                whereString.Append(" AND WritingUserName <> '' ");
            }

            whereString.Append(" ").Append(orderByString);

            return DataProvider.DatabaseDao.GetSelectSqlString(tableName, SqlUtils.Asterisk, whereString.ToString());
        }

        private List<int> GetContentIdListByTrash(int siteId, string tableName)
        {
            string sqlString =
                $"SELECT Id FROM {tableName} WHERE SiteId = {siteId} AND ChannelId < 0";
            return DataProvider.DatabaseDao.GetIntList(sqlString);
        }

        private DataSet GetStlDataSourceChecked(string tableName, List<int> channelIdList, int startNum, int totalNum, string orderByString, string whereString, LowerNameValueCollection others)
        {
            if (channelIdList == null || channelIdList.Count == 0)
            {
                return null;
            }
            var sqlWhereString = channelIdList.Count == 1 ? $"WHERE (ChannelId = {channelIdList[0]} AND IsChecked = '{true}' {whereString})" : $"WHERE (ChannelId IN ({TranslateUtils.ToSqlInStringWithoutQuote(channelIdList)}) AND IsChecked = '{true}' {whereString})";

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
                var sqlSelect = DataProvider.DatabaseDao.GetSelectSqlString(tableName, totalNum, StlColumns, whereString, orderByString);
                dataset = ExecuteDataset(sqlSelect);
            }
            return dataset;
        }

        private DataSet GetStlDataSourceByStartNum(string tableName, int startNum, int totalNum, string whereString, string orderByString)
        {
            DataSet dataset = null;
            if (!string.IsNullOrEmpty(tableName))
            {
                var sqlSelect = DataProvider.DatabaseDao.GetSelectSqlString(tableName, startNum, totalNum, StlColumns, whereString, orderByString);
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

        private ContentInfo GetContentInfo(IDataReader rdr)
        {
            var contentInfo = new ContentInfo();
            contentInfo.Load(rdr);
            contentInfo.Load(contentInfo.SettingsXml);

            return contentInfo;
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

        //----------------------------pager start----------------------------------------//

        public string GetSelectedCommendByCheck(string tableName, int siteId, List<int> channelIdList, List<int> checkLevelList)
        {
            var whereString = channelIdList.Count == 1
                ? $"WHERE SiteId = {siteId} AND ChannelId = {channelIdList[0]} AND IsChecked='{false}' AND CheckedLevel IN ({TranslateUtils.ToSqlInStringWithoutQuote(checkLevelList)}) "
                : $"WHERE SiteId = {siteId} AND ChannelId IN ({TranslateUtils.ToSqlInStringWithoutQuote(channelIdList)}) AND IsChecked='{false}' AND CheckedLevel IN ({TranslateUtils.ToSqlInStringWithoutQuote(checkLevelList)}) ";

            return DataProvider.DatabaseDao.GetSelectSqlString(tableName, SqlUtils.Asterisk, whereString);
        }

        public string GetPagerWhereSqlString(SiteInfo siteInfo, ChannelInfo channelInfo, string searchType, string keyword, string dateFrom, string dateTo, int checkLevel, bool isCheckOnly, bool isSelfOnly, bool isTrashOnly, bool isWritingOnly, bool isAdminOnly, PermissionManager adminPermissions, List<string> allLowerAttributeNameList)
        {
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
                    searchChannelIdList = channelIdList;
                }
                else
                {
                    foreach (var theChannelId in channelIdList)
                    {
                        if (adminPermissions.OwningChannelIdList.Contains(theChannelId))
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

            if (searchChannelIdList.Count == 0)
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
                if (allLowerAttributeNameList.Contains(searchType.ToLower()))
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

            if (isAdminOnly || adminPermissions.IsViewContentOnlySelf(siteInfo.Id, channelInfo.Id))
            {
                whereList.Add($"{nameof(ContentAttribute.AddUserName)} = '{adminPermissions.UserName}'");
            }

            if (isWritingOnly)
            {
                whereList.Add($"{nameof(ContentAttribute.WritingUserName)} <> ''");
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

        public string GetPagerOrderSqlString(ChannelInfo channelInfo)
        {
            return ETaxisTypeUtils.GetContentOrderByString(ETaxisTypeUtils.GetEnumType(channelInfo.Additional.DefaultTaxisType));
        }

        //public string GetPagerReturnColumnNames(List<string> allLowerAttributeNameList, StringCollection attributesOfDisplay)
        //{
        //    var columnLowerList = new List<string>(ContentAttribute.AllAttributesLowercase);
        //    foreach (var attribute in attributesOfDisplay)
        //    {
        //        var lowerAttribute = attribute;
        //        if (!columnLowerList.Contains(lowerAttribute) && allLowerAttributeNameList.Contains(lowerAttribute))
        //        {
        //            columnLowerList.Add(lowerAttribute);
        //        }
        //    }

        //    return TranslateUtils.ObjectCollectionToString(columnLowerList);
        //}

        //----------------------------pager end----------------------------------------//

        public bool ApiIsExists(string tableName, int id)
        {
            var sqlString = $"SELECT count(1) FROM {tableName} WHERE Id = @Id";

            using (var connection = GetConnection())
            {
                return connection.ExecuteScalar<bool>(sqlString, new { Id = id });
            }
        }

        public List<int> ApiGetContentIdList(string tableName, int siteId, int channelId, NameValueCollection queryString)
        {
            var list = new List<int>();

            var sqlString = $"SELECT Id FROM {tableName} WHERE SiteId = {siteId} AND ChannelId = {channelId}";

            if (queryString != null && queryString.Count > 0)
            {
                var lowerColumnNameList = TableMetadataManager.GetAllLowerAttributeNameList(tableName);

                foreach (string attributeName in queryString)
                {
                    if (!lowerColumnNameList.Contains(attributeName.ToLower())) continue;

                    var value = queryString[attributeName];

                    if (StringUtils.EqualsIgnoreCase(attributeName, ContentAttribute.IsChecked) || StringUtils.EqualsIgnoreCase(attributeName, ContentAttribute.IsColor) || StringUtils.EqualsIgnoreCase(attributeName, ContentAttribute.IsHot) || StringUtils.EqualsIgnoreCase(attributeName, ContentAttribute.IsRecommend) || StringUtils.EqualsIgnoreCase(attributeName, ContentAttribute.IsTop))
                    {
                        sqlString += $" AND {attributeName} = '{TranslateUtils.ToBool(value)}'";
                    }
                    else if (StringUtils.EqualsIgnoreCase(attributeName, ContentAttribute.Id) || StringUtils.EqualsIgnoreCase(attributeName, ContentAttribute.ReferenceId) || StringUtils.EqualsIgnoreCase(attributeName, ContentAttribute.SourceId) || StringUtils.EqualsIgnoreCase(attributeName, ContentAttribute.CheckedLevel))
                    {
                        sqlString += $" AND {attributeName} = {TranslateUtils.ToInt(value)}";
                    }
                    else
                    {
                        sqlString += $" AND {attributeName} = N'{PageUtils.FilterSql(value)}'";
                    }
                }
            }

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

        public Dictionary<string, object> ApiGetContentInfo(string tableName, int contentId)
        {
            if (string.IsNullOrEmpty(tableName) || contentId <= 0) return null;

            Dictionary<string, object> info = null;

            string sqlWhere = $"WHERE Id = {contentId}";
            var sqlSelect = DataProvider.DatabaseDao.GetSelectSqlString(tableName, SqlUtils.Asterisk, sqlWhere);

            using (var rdr = ExecuteReader(sqlSelect))
            {
                if (rdr.Read())
                {
                    var contentInfo = GetContentInfo(rdr);
                    info = contentInfo.ToDictionary();
                }
                rdr.Close();
            }

            return info;
        }
    }
}
