using System.Collections.Generic;
using System.Text;
using BaiRong.Core.AuxiliaryTable;
using BaiRong.Core.Data;
using BaiRong.Core.Model;
using BaiRong.Core.Model.Attributes;
using BaiRong.Core.Model.Enumerations;
using SiteServer.Plugin.Models;

namespace BaiRong.Core.Provider
{
    public class AuxiliaryTableDataDao : DataProviderBase
    {
        public string GetCreateAuxiliaryTableSqlString(string tableName)
        {
            var columnSqlStringList = new List<string>();

            var tableMetadataInfoList = TableManager.GetTableMetadataInfoList(tableName);
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
            
            //添加默认字段
            if (WebConfigUtils.DatabaseType == EDatabaseType.MySql)
            {
                sqlBuilder.Append($@"CREATE TABLE `{tableName}` (");
                sqlBuilder.Append($@"
`{nameof(ContentInfo.Id)}` INT AUTO_INCREMENT,
`{nameof(ContentInfo.NodeId)}` INT,
`{nameof(ContentInfo.PublishmentSystemId)}` INT,
`{nameof(ContentInfo.AddUserName)}` VARCHAR(255),
`{nameof(ContentInfo.LastEditUserName)}` VARCHAR(255),
`{nameof(ContentInfo.WritingUserName)}` VARCHAR(255),
`{nameof(ContentInfo.LastEditDate)}` DATETIME,
`{nameof(ContentInfo.Taxis)}` INT,
`{nameof(ContentInfo.ContentGroupNameCollection)}` VARCHAR(255),
`{nameof(ContentInfo.Tags)}` VARCHAR(255),
`{nameof(ContentInfo.SourceId)}` INT,
`{nameof(ContentInfo.ReferenceId)}` INT,
`{nameof(ContentInfo.IsChecked)}` VARCHAR(18),
`{nameof(ContentInfo.CheckedLevel)}` INT,
`{nameof(ContentInfo.Comments)}` INT,
`{nameof(ContentInfo.Photos)}` INT,
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
`{nameof(ContentInfo.AddDate)}` DATETIME,
");
            }
            else
            {
                sqlBuilder.Append($@"CREATE TABLE {tableName} (");
                sqlBuilder.Append($@"
{nameof(ContentInfo.Id)} int IDENTITY (1, 1),
{nameof(ContentInfo.NodeId)} int NULL,
{nameof(ContentInfo.PublishmentSystemId)} int NULL,
{nameof(ContentInfo.AddUserName)} nvarchar (255) NULL,
{nameof(ContentInfo.LastEditUserName)} nvarchar (255) NULL,
{nameof(ContentInfo.WritingUserName)} nvarchar (255) NULL,
{nameof(ContentInfo.LastEditDate)} datetime NULL,
{nameof(ContentInfo.Taxis)} int NULL,
{nameof(ContentInfo.ContentGroupNameCollection)} nvarchar (255) NULL,
{nameof(ContentInfo.Tags)} nvarchar (255) NULL,
{nameof(ContentInfo.SourceId)} int NULL,
{nameof(ContentInfo.ReferenceId)} int NULL,
{nameof(ContentInfo.IsChecked)} varchar (18) NULL,
{nameof(ContentInfo.CheckedLevel)} int NULL,
{nameof(ContentInfo.Comments)} int NULL,
{nameof(ContentInfo.Photos)} int NULL,
{nameof(ContentInfo.Hits)} int NULL,
{nameof(ContentInfo.HitsByDay)} int NULL,
{nameof(ContentInfo.HitsByWeek)} int NULL,
{nameof(ContentInfo.HitsByMonth)} int NULL,
{nameof(ContentInfo.LastHitsDate)} datetime NULL,
{nameof(ContentInfo.SettingsXml)} ntext NULL,
{nameof(ContentInfo.Title)} nvarchar (255) NULL,
{nameof(ContentInfo.IsTop)} varchar (18) NULL,
{nameof(ContentInfo.IsRecommend)} varchar (18) NULL,
{nameof(ContentInfo.IsHot)} varchar (18) NULL,
{nameof(ContentInfo.IsColor)} varchar (18) NULL,
{nameof(ContentInfo.AddDate)} datetime NULL,
");
            }

            //添加后台定义字段
            foreach (var sqlString in columnSqlStringList)
            {
                sqlBuilder.Append(sqlString).Append(@",");
            }

            if (WebConfigUtils.DatabaseType == EDatabaseType.MySql)
            {
                sqlBuilder.Append($@"
PRIMARY KEY ({nameof(ContentInfo.Id)})
)
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

        public List<TableMetadataInfo> GetDefaultTableMetadataInfoList(string tableName, EAuxiliaryTableType tableType)
        {
            var list = new List<TableMetadataInfo>();
            if (tableType != EAuxiliaryTableType.BackgroundContent) return list;

            var metadataInfo = new TableMetadataInfo(0, tableName, BackgroundContentAttribute.SubTitle, DataType.NVarChar, 255, 0, true);
            list.Add(metadataInfo);
            metadataInfo = new TableMetadataInfo(0, tableName, BackgroundContentAttribute.ImageUrl, DataType.VarChar, 200, 0, true);
            list.Add(metadataInfo);
            metadataInfo = new TableMetadataInfo(0, tableName, BackgroundContentAttribute.VideoUrl, DataType.VarChar, 200, 0, true);
            list.Add(metadataInfo);
            metadataInfo = new TableMetadataInfo(0, tableName, BackgroundContentAttribute.FileUrl, DataType.VarChar, 200, 0, true);
            list.Add(metadataInfo);
            metadataInfo = new TableMetadataInfo(0, tableName, BackgroundContentAttribute.LinkUrl, DataType.NVarChar, 200, 0, true);
            list.Add(metadataInfo);
            metadataInfo = new TableMetadataInfo(0, tableName, BackgroundContentAttribute.Content, DataType.NText, 16, 0, true);
            list.Add(metadataInfo);
            metadataInfo = new TableMetadataInfo(0, tableName, BackgroundContentAttribute.Summary, DataType.NText, 16, 0, true);
            list.Add(metadataInfo);
            metadataInfo = new TableMetadataInfo(0, tableName, BackgroundContentAttribute.Author, DataType.NVarChar, 255, 0, true);
            list.Add(metadataInfo);
            metadataInfo = new TableMetadataInfo(0, tableName, BackgroundContentAttribute.Source, DataType.NVarChar, 255, 0, true);
            list.Add(metadataInfo);
            return list;
        }
    }
}
