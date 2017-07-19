using System.Collections.Generic;
using System.Text;
using BaiRong.Core.AuxiliaryTable;
using BaiRong.Core.Data;
using BaiRong.Core.Model;
using BaiRong.Core.Model.Attributes;
using BaiRong.Core.Model.Enumerations;
using SiteServer.Plugin;

namespace BaiRong.Core.Provider
{
    public class AuxiliaryTableDataDao : DataProviderBase
    {
        public string GetCreateAuxiliaryTableSqlString(string tableName)
        {
            //获取tableType
            var tableType = BaiRongDataProvider.TableCollectionDao.GetTableType(tableName);

            //获取columnSqlStringArrayList
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

            //开始生成sql语句
            var sqlBuilder = new StringBuilder();

            sqlBuilder.Append($@"CREATE TABLE {tableName} (");
            //添加默认字段
            if (WebConfigUtils.DatabaseType == EDatabaseType.MySql)
            {
                sqlBuilder.Append(@"
ID INT AUTO_INCREMENT,
NodeID INT,
PublishmentSystemID INT,
AddUserName VARCHAR(255),
LastEditUserName VARCHAR(255),
WritingUserName VARCHAR(255),
LastEditDate DATETIME,
Taxis INT,
ContentGroupNameCollection VARCHAR(255),
Tags VARCHAR(255),
SourceID INT,
ReferenceID INT,
IsChecked VARCHAR(18),
CheckedLevel INT,
Comments INT,
Photos INT,
Hits INT,
HitsByDay INT,
HitsByWeek INT,
HitsByMonth INT,
LastHitsDate DATETIME,
SettingsXML LONGTEXT,
");
            }
            else
            {
                sqlBuilder.Append(@"
ID int IDENTITY (1, 1),
NodeID int NULL,
PublishmentSystemID int NULL,
AddUserName nvarchar (255) NULL,
LastEditUserName nvarchar (255) NULL,
WritingUserName nvarchar (255) NULL,
LastEditDate datetime NULL,
Taxis int NULL,
ContentGroupNameCollection nvarchar (255) NULL,
Tags nvarchar (255) NULL,
SourceID int NULL,
ReferenceID int NULL,
IsChecked varchar (18) NULL,
CheckedLevel int NULL,
Comments int NULL,
Photos int NULL,
Hits int NULL,
HitsByDay int NULL,
HitsByWeek int NULL,
HitsByMonth int NULL,
LastHitsDate datetime NULL,
SettingsXML ntext NULL,
");
            }
            
            if (tableType == EAuxiliaryTableType.VoteContent)
            {
                if (WebConfigUtils.DatabaseType == EDatabaseType.MySql)
                {
                    sqlBuilder.Append($@"{VoteContentAttribute.IsImageVote} VARCHAR(18),").AppendLine();
                    sqlBuilder.Append($@"{VoteContentAttribute.IsSummary} VARCHAR(18),").AppendLine();
                    sqlBuilder.Append($@"{VoteContentAttribute.Participants} INT,").AppendLine();
                    sqlBuilder.Append($@"{VoteContentAttribute.IsClosed} VARCHAR(18),").AppendLine();
                    sqlBuilder.Append($@"{VoteContentAttribute.IsTop} VARCHAR(18),").AppendLine();
                }
                else
                {
                    sqlBuilder.Append($@"{VoteContentAttribute.IsImageVote} varchar (18) NULL,").AppendLine();
                    sqlBuilder.Append($@"{VoteContentAttribute.IsSummary} varchar (18) NULL,").AppendLine();
                    sqlBuilder.Append($@"{VoteContentAttribute.Participants} int NULL,").AppendLine();
                    sqlBuilder.Append($@"{VoteContentAttribute.IsClosed} varchar (18) NULL,").AppendLine();
                    sqlBuilder.Append($@"{VoteContentAttribute.IsTop} varchar (18) NULL,").AppendLine();
                }
            }
            else if (tableType == EAuxiliaryTableType.GovPublicContent)
            {
                if (WebConfigUtils.DatabaseType == EDatabaseType.MySql)
                {
                    sqlBuilder.Append($@"{GovPublicContentAttribute.DepartmentId} INT,").AppendLine();
                    sqlBuilder.Append($@"{GovPublicContentAttribute.Category1Id} INT,").AppendLine();
                    sqlBuilder.Append($@"{GovPublicContentAttribute.Category2Id} INT,").AppendLine();
                    sqlBuilder.Append($@"{GovPublicContentAttribute.Category3Id} INT,").AppendLine();
                    sqlBuilder.Append($@"{GovPublicContentAttribute.Category4Id} INT,").AppendLine();
                    sqlBuilder.Append($@"{GovPublicContentAttribute.Category5Id} INT,").AppendLine();
                    sqlBuilder.Append($@"{GovPublicContentAttribute.Category6Id} INT,").AppendLine();
                }
                else
                {
                    sqlBuilder.Append($@"{GovPublicContentAttribute.DepartmentId} int NULL,").AppendLine();
                    sqlBuilder.Append($@"{GovPublicContentAttribute.Category1Id} int NULL,").AppendLine();
                    sqlBuilder.Append($@"{GovPublicContentAttribute.Category2Id} int NULL,").AppendLine();
                    sqlBuilder.Append($@"{GovPublicContentAttribute.Category3Id} int NULL,").AppendLine();
                    sqlBuilder.Append($@"{GovPublicContentAttribute.Category4Id} int NULL,").AppendLine();
                    sqlBuilder.Append($@"{GovPublicContentAttribute.Category5Id} int NULL,").AppendLine();
                    sqlBuilder.Append($@"{GovPublicContentAttribute.Category6Id} int NULL,").AppendLine();
                }
            }
            else if (tableType == EAuxiliaryTableType.GovInteractContent)
            {
                if (WebConfigUtils.DatabaseType == EDatabaseType.MySql)
                {
                    sqlBuilder.Append($@"{GovInteractContentAttribute.DepartmentName} VARCHAR(255),").AppendLine();
                    sqlBuilder.Append($@"{GovInteractContentAttribute.QueryCode} VARCHAR(255),").AppendLine();
                    sqlBuilder.Append($@"{GovInteractContentAttribute.State} VARCHAR(50),").AppendLine();
                    sqlBuilder.Append($@"{GovInteractContentAttribute.IpAddress} VARCHAR(50),").AppendLine();
                    sqlBuilder.Append($@"{GovInteractContentAttribute.IsRecommend} VARCHAR(18),").AppendLine();
                    sqlBuilder.Append($@"{ContentAttribute.IsTop} VARCHAR(18),").AppendLine();
                    sqlBuilder.Append($@"{ContentAttribute.AddDate} DATETIME,").AppendLine();
                }
                else
                {
                    sqlBuilder.Append($@"{GovInteractContentAttribute.DepartmentName} nvarchar (255) NULL,").AppendLine();
                    sqlBuilder.Append($@"{GovInteractContentAttribute.QueryCode} nvarchar (255) NULL,").AppendLine();
                    sqlBuilder.Append($@"{GovInteractContentAttribute.State} varchar (50) NULL,").AppendLine();
                    sqlBuilder.Append($@"{GovInteractContentAttribute.IpAddress} varchar (50) NULL,").AppendLine();
                    sqlBuilder.Append($@"{GovInteractContentAttribute.IsRecommend} varchar (18) NULL,").AppendLine();
                    sqlBuilder.Append($@"{ContentAttribute.IsTop} varchar (18) NULL,").AppendLine();
                    sqlBuilder.Append($@"{ContentAttribute.AddDate} datetime NULL,").AppendLine();
                }
            }

            //添加后台定义字段
            foreach (var sqlString in columnSqlStringList)
            {
                sqlBuilder.Append(sqlString).Append(@",
");
            }
            //添加主键及索引
            if (WebConfigUtils.DatabaseType == EDatabaseType.MySql)
            {
                sqlBuilder.Append(@"PRIMARY KEY (ID)");
            }
            else
            {
                sqlBuilder.Append($@"CONSTRAINT PK_{tableName} PRIMARY KEY (ID)");
            }
sqlBuilder.Append($@"
)
go
CREATE INDEX IX_{tableName} ON {tableName}(IsTop DESC, Taxis DESC, ID DESC)
go
CREATE INDEX IX_Taxis ON {tableName}(Taxis DESC)
go");

            return sqlBuilder.ToString();
        }

        public List<TableMetadataInfo> GetPluginTableMetadataInfoList(string pluginId, List<PluginTableColumn> tableColumns)
        {
            var list = new List<TableMetadataInfo>();

            foreach (var tableColumn in tableColumns)
            {
                var metadataInfo = new TableMetadataInfo(0, pluginId, tableColumn.AttributeName, tableColumn.DataType, tableColumn.DataLength, 0, true);
                list.Add(metadataInfo);
            }

            return list;
        }

        public List<TableMetadataInfo> GetDefaultTableMetadataInfoList(string tableName, EAuxiliaryTableType tableType)
        {
            var arraylist = new List<TableMetadataInfo>();
            if (tableType == EAuxiliaryTableType.BackgroundContent)
            {
                var metadataInfo = new TableMetadataInfo(0, tableName, ContentAttribute.Title, DataType.NVarChar, 255, 0, true);
                arraylist.Add(metadataInfo);
                metadataInfo = new TableMetadataInfo(0, tableName, BackgroundContentAttribute.SubTitle, DataType.NVarChar, 255, 0, true);
                arraylist.Add(metadataInfo);
                metadataInfo = new TableMetadataInfo(0, tableName, BackgroundContentAttribute.ImageUrl, DataType.VarChar, 200, 0, true);
                arraylist.Add(metadataInfo);
                metadataInfo = new TableMetadataInfo(0, tableName, BackgroundContentAttribute.VideoUrl, DataType.VarChar, 200, 0, true);
                arraylist.Add(metadataInfo);
                metadataInfo = new TableMetadataInfo(0, tableName, BackgroundContentAttribute.FileUrl, DataType.VarChar, 200, 0, true);
                arraylist.Add(metadataInfo);
                metadataInfo = new TableMetadataInfo(0, tableName, BackgroundContentAttribute.LinkUrl, DataType.NVarChar, 200, 0, true);
                arraylist.Add(metadataInfo);
                metadataInfo = new TableMetadataInfo(0, tableName, BackgroundContentAttribute.Content, DataType.NText, 16, 0, true);
                arraylist.Add(metadataInfo);
                metadataInfo = new TableMetadataInfo(0, tableName, BackgroundContentAttribute.Summary, DataType.NText, 16, 0, true);
                arraylist.Add(metadataInfo);
                metadataInfo = new TableMetadataInfo(0, tableName, BackgroundContentAttribute.Author, DataType.NVarChar, 255, 0, true);
                arraylist.Add(metadataInfo);
                metadataInfo = new TableMetadataInfo(0, tableName, BackgroundContentAttribute.Source, DataType.NVarChar, 255, 0, true);
                arraylist.Add(metadataInfo);
                metadataInfo = new TableMetadataInfo(0, tableName, BackgroundContentAttribute.IsRecommend, DataType.VarChar, 18, 0, true);
                arraylist.Add(metadataInfo);
                metadataInfo = new TableMetadataInfo(0, tableName, BackgroundContentAttribute.IsHot, DataType.VarChar, 18, 0, true);
                arraylist.Add(metadataInfo);
                metadataInfo = new TableMetadataInfo(0, tableName, BackgroundContentAttribute.IsColor, DataType.VarChar, 18, 0, true);
                arraylist.Add(metadataInfo);
                metadataInfo = new TableMetadataInfo(0, tableName, ContentAttribute.IsTop, DataType.VarChar, 18, 0, true);
                arraylist.Add(metadataInfo);
                metadataInfo = new TableMetadataInfo(0, tableName, ContentAttribute.AddDate, DataType.DateTime, 8, 0, true);
                arraylist.Add(metadataInfo);
            }
            else if (tableType == EAuxiliaryTableType.GovPublicContent)
            {
                var metadataInfo = new TableMetadataInfo(0, tableName, ContentAttribute.Title, DataType.NVarChar, 255, 0, true);
                arraylist.Add(metadataInfo);
                metadataInfo = new TableMetadataInfo(0, tableName, GovPublicContentAttribute.Identifier, DataType.NVarChar, 255, 0, true);
                arraylist.Add(metadataInfo);
                metadataInfo = new TableMetadataInfo(0, tableName, GovPublicContentAttribute.Description, DataType.NText, 16, 0, true);
                arraylist.Add(metadataInfo);
                metadataInfo = new TableMetadataInfo(0, tableName, GovPublicContentAttribute.PublishDate, DataType.DateTime, 8, 0, true);
                arraylist.Add(metadataInfo);
                metadataInfo = new TableMetadataInfo(0, tableName, GovPublicContentAttribute.EffectDate, DataType.DateTime, 8, 0, true);
                arraylist.Add(metadataInfo);
                metadataInfo = new TableMetadataInfo(0, tableName, GovPublicContentAttribute.IsAbolition, DataType.VarChar, 18, 0, true);
                arraylist.Add(metadataInfo);
                metadataInfo = new TableMetadataInfo(0, tableName, GovPublicContentAttribute.AbolitionDate, DataType.DateTime, 8, 0, true);
                arraylist.Add(metadataInfo);
                metadataInfo = new TableMetadataInfo(0, tableName, GovPublicContentAttribute.DocumentNo, DataType.NVarChar, 255, 0, true);
                arraylist.Add(metadataInfo);
                metadataInfo = new TableMetadataInfo(0, tableName, GovPublicContentAttribute.Publisher, DataType.NVarChar, 255, 0, true);
                arraylist.Add(metadataInfo);
                metadataInfo = new TableMetadataInfo(0, tableName, GovPublicContentAttribute.Keywords, DataType.NVarChar, 255, 0, true);
                arraylist.Add(metadataInfo);
                metadataInfo = new TableMetadataInfo(0, tableName, GovPublicContentAttribute.ImageUrl, DataType.NVarChar, 255, 0, true);
                arraylist.Add(metadataInfo);
                metadataInfo = new TableMetadataInfo(0, tableName, GovPublicContentAttribute.FileUrl, DataType.NVarChar, 255, 0, true);
                arraylist.Add(metadataInfo);
                metadataInfo = new TableMetadataInfo(0, tableName, GovPublicContentAttribute.IsRecommend, DataType.VarChar, 18, 0, true);
                arraylist.Add(metadataInfo);
                metadataInfo = new TableMetadataInfo(0, tableName, GovPublicContentAttribute.IsHot, DataType.VarChar, 18, 0, true);
                arraylist.Add(metadataInfo);
                metadataInfo = new TableMetadataInfo(0, tableName, GovPublicContentAttribute.IsColor, DataType.VarChar, 18, 0, true);
                arraylist.Add(metadataInfo);
                metadataInfo = new TableMetadataInfo(0, tableName, ContentAttribute.IsTop, DataType.VarChar, 18, 0, true);
                arraylist.Add(metadataInfo);
                metadataInfo = new TableMetadataInfo(0, tableName, GovPublicContentAttribute.Content, DataType.NText, 16, 0, true);
                arraylist.Add(metadataInfo);
                metadataInfo = new TableMetadataInfo(0, tableName, ContentAttribute.AddDate, DataType.DateTime, 8, 0, true);
                arraylist.Add(metadataInfo);
            }
            else if (tableType == EAuxiliaryTableType.GovInteractContent)
            {
                var metadataInfo = new TableMetadataInfo(0, tableName, GovInteractContentAttribute.RealName, DataType.NVarChar, 255, 0, true);
                arraylist.Add(metadataInfo);
                metadataInfo = new TableMetadataInfo(0, tableName, GovInteractContentAttribute.Organization, DataType.NVarChar, 255, 0, true);
                arraylist.Add(metadataInfo);
                metadataInfo = new TableMetadataInfo(0, tableName, GovInteractContentAttribute.CardType, DataType.NVarChar, 255, 0, true);
                arraylist.Add(metadataInfo);
                metadataInfo = new TableMetadataInfo(0, tableName, GovInteractContentAttribute.CardNo, DataType.NVarChar, 255, 0, true);
                arraylist.Add(metadataInfo);
                metadataInfo = new TableMetadataInfo(0, tableName, GovInteractContentAttribute.Phone, DataType.VarChar, 50, 0, true);
                arraylist.Add(metadataInfo);
                metadataInfo = new TableMetadataInfo(0, tableName, GovInteractContentAttribute.PostCode, DataType.VarChar, 50, 0, true);
                arraylist.Add(metadataInfo);
                metadataInfo = new TableMetadataInfo(0, tableName, GovInteractContentAttribute.Address, DataType.NVarChar, 255, 0, true);
                arraylist.Add(metadataInfo);
                metadataInfo = new TableMetadataInfo(0, tableName, GovInteractContentAttribute.Email, DataType.NVarChar, 255, 0, true);
                arraylist.Add(metadataInfo);
                metadataInfo = new TableMetadataInfo(0, tableName, GovInteractContentAttribute.Fax, DataType.VarChar, 50, 0, true);
                arraylist.Add(metadataInfo);
                metadataInfo = new TableMetadataInfo(0, tableName, GovInteractContentAttribute.TypeId, DataType.Integer, 38, 0, true);
                arraylist.Add(metadataInfo);
                metadataInfo = new TableMetadataInfo(0, tableName, GovInteractContentAttribute.IsPublic, DataType.VarChar, 18, 0, true);
                arraylist.Add(metadataInfo);
                metadataInfo = new TableMetadataInfo(0, tableName, GovInteractContentAttribute.Title, DataType.NVarChar, 255, 0, true);
                arraylist.Add(metadataInfo);
                metadataInfo = new TableMetadataInfo(0, tableName, GovInteractContentAttribute.Content, DataType.NText, 16, 0, true);
                arraylist.Add(metadataInfo);
                metadataInfo = new TableMetadataInfo(0, tableName, GovInteractContentAttribute.FileUrl, DataType.NVarChar, 255, 0, true);
                arraylist.Add(metadataInfo);
                metadataInfo = new TableMetadataInfo(0, tableName, GovInteractContentAttribute.DepartmentId, DataType.Integer, 38, 0, true);
                arraylist.Add(metadataInfo);
            }
            else if (tableType == EAuxiliaryTableType.VoteContent)
            {
                var metadataInfo = new TableMetadataInfo(0, tableName, VoteContentAttribute.Title, DataType.NVarChar, 255, 0, true);
                arraylist.Add(metadataInfo);
                metadataInfo = new TableMetadataInfo(0, tableName, VoteContentAttribute.SubTitle, DataType.NVarChar, 255, 0, true);
                arraylist.Add(metadataInfo);
                metadataInfo = new TableMetadataInfo(0, tableName, VoteContentAttribute.MaxSelectNum, DataType.Integer, 38, 0, true);
                arraylist.Add(metadataInfo);
                metadataInfo = new TableMetadataInfo(0, tableName, VoteContentAttribute.ImageUrl, DataType.VarChar, 200, 0, true);
                arraylist.Add(metadataInfo);
                metadataInfo = new TableMetadataInfo(0, tableName, VoteContentAttribute.Content, DataType.NText, 16, 0, true);
                arraylist.Add(metadataInfo);
                metadataInfo = new TableMetadataInfo(0, tableName, VoteContentAttribute.Summary, DataType.NText, 16, 0, true);
                arraylist.Add(metadataInfo);
                metadataInfo = new TableMetadataInfo(0, tableName, VoteContentAttribute.AddDate, DataType.DateTime, 8, 0, true);
                arraylist.Add(metadataInfo);
                metadataInfo = new TableMetadataInfo(0, tableName, VoteContentAttribute.EndDate, DataType.DateTime, 8, 0, true);
                arraylist.Add(metadataInfo);
                metadataInfo = new TableMetadataInfo(0, tableName, VoteContentAttribute.IsVotedView, DataType.VarChar, 18, 0, true);
                arraylist.Add(metadataInfo);
                metadataInfo = new TableMetadataInfo(0, tableName, VoteContentAttribute.HiddenContent, DataType.NText, 16, 0, true);
                arraylist.Add(metadataInfo);
            }
            else if (tableType == EAuxiliaryTableType.JobContent)
            {
                var metadataInfo = new TableMetadataInfo(0, tableName, ContentAttribute.Title, DataType.NVarChar, 255, 0, true);
                arraylist.Add(metadataInfo);
                metadataInfo = new TableMetadataInfo(0, tableName, JobContentAttribute.Department, DataType.NVarChar, 255, 0, true);
                arraylist.Add(metadataInfo);
                metadataInfo = new TableMetadataInfo(0, tableName, JobContentAttribute.Location, DataType.NVarChar, 255, 0, true);
                arraylist.Add(metadataInfo);
                metadataInfo = new TableMetadataInfo(0, tableName, JobContentAttribute.NumberOfPeople, DataType.NVarChar, 50, 0, true);
                arraylist.Add(metadataInfo);
                metadataInfo = new TableMetadataInfo(0, tableName, JobContentAttribute.Responsibility, DataType.NText, 16, 0, true);
                arraylist.Add(metadataInfo);
                metadataInfo = new TableMetadataInfo(0, tableName, JobContentAttribute.Requirement, DataType.NText, 16, 0, true);
                arraylist.Add(metadataInfo);
                metadataInfo = new TableMetadataInfo(0, tableName, JobContentAttribute.IsUrgent, DataType.VarChar, 18, 0, true);
                arraylist.Add(metadataInfo);
                metadataInfo = new TableMetadataInfo(0, tableName, ContentAttribute.IsTop, DataType.VarChar, 18, 0, true);
                arraylist.Add(metadataInfo);
                metadataInfo = new TableMetadataInfo(0, tableName, ContentAttribute.AddDate, DataType.DateTime, 8, 0, true);
                arraylist.Add(metadataInfo);
            }
            else if (tableType == EAuxiliaryTableType.Custom)
            {
                var metadataInfo = new TableMetadataInfo(0, tableName, ContentAttribute.Title, DataType.NVarChar, 255, 0, true);
                arraylist.Add(metadataInfo);
                metadataInfo = new TableMetadataInfo(0, tableName, ContentAttribute.IsTop, DataType.VarChar, 18, 0, true);
                arraylist.Add(metadataInfo);
                metadataInfo = new TableMetadataInfo(0, tableName, ContentAttribute.AddDate, DataType.DateTime, 8, 0, true);
                arraylist.Add(metadataInfo);
            }
            return arraylist;
        }
    }
}
