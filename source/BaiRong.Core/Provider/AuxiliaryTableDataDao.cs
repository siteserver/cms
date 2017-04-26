using System.Collections.Generic;
using System.Text;
using BaiRong.Core.AuxiliaryTable;
using BaiRong.Core.Data;
using BaiRong.Core.Model;
using BaiRong.Core.Model.Attributes;
using BaiRong.Core.Model.Enumerations;

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
            if (WebConfigUtils.IsMySql)
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
                if (WebConfigUtils.IsMySql)
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
                if (WebConfigUtils.IsMySql)
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
                if (WebConfigUtils.IsMySql)
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
            if (WebConfigUtils.IsMySql)
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

        public List<TableMetadataInfo> GetDefaultTableMetadataInfoList(string tableName, EAuxiliaryTableType tableType)
        {
            var arraylist = new List<TableMetadataInfo>();
            if (tableType == EAuxiliaryTableType.BackgroundContent)
            {
                var metadataInfo = new TableMetadataInfo(0, tableName, ContentAttribute.Title, EDataType.NVarChar, 255, 0, true);
                arraylist.Add(metadataInfo);
                metadataInfo = new TableMetadataInfo(0, tableName, BackgroundContentAttribute.SubTitle, EDataType.NVarChar, 255, 0, true);
                arraylist.Add(metadataInfo);
                metadataInfo = new TableMetadataInfo(0, tableName, BackgroundContentAttribute.ImageUrl, EDataType.VarChar, 200, 0, true);
                arraylist.Add(metadataInfo);
                metadataInfo = new TableMetadataInfo(0, tableName, BackgroundContentAttribute.VideoUrl, EDataType.VarChar, 200, 0, true);
                arraylist.Add(metadataInfo);
                metadataInfo = new TableMetadataInfo(0, tableName, BackgroundContentAttribute.FileUrl, EDataType.VarChar, 200, 0, true);
                arraylist.Add(metadataInfo);
                metadataInfo = new TableMetadataInfo(0, tableName, BackgroundContentAttribute.LinkUrl, EDataType.NVarChar, 200, 0, true);
                arraylist.Add(metadataInfo);
                metadataInfo = new TableMetadataInfo(0, tableName, BackgroundContentAttribute.Content, EDataType.NText, 16, 0, true);
                arraylist.Add(metadataInfo);
                metadataInfo = new TableMetadataInfo(0, tableName, BackgroundContentAttribute.Summary, EDataType.NText, 16, 0, true);
                arraylist.Add(metadataInfo);
                metadataInfo = new TableMetadataInfo(0, tableName, BackgroundContentAttribute.Author, EDataType.NVarChar, 255, 0, true);
                arraylist.Add(metadataInfo);
                metadataInfo = new TableMetadataInfo(0, tableName, BackgroundContentAttribute.Source, EDataType.NVarChar, 255, 0, true);
                arraylist.Add(metadataInfo);
                metadataInfo = new TableMetadataInfo(0, tableName, BackgroundContentAttribute.IsRecommend, EDataType.VarChar, 18, 0, true);
                arraylist.Add(metadataInfo);
                metadataInfo = new TableMetadataInfo(0, tableName, BackgroundContentAttribute.IsHot, EDataType.VarChar, 18, 0, true);
                arraylist.Add(metadataInfo);
                metadataInfo = new TableMetadataInfo(0, tableName, BackgroundContentAttribute.IsColor, EDataType.VarChar, 18, 0, true);
                arraylist.Add(metadataInfo);
                metadataInfo = new TableMetadataInfo(0, tableName, ContentAttribute.IsTop, EDataType.VarChar, 18, 0, true);
                arraylist.Add(metadataInfo);
                metadataInfo = new TableMetadataInfo(0, tableName, ContentAttribute.AddDate, EDataType.DateTime, 8, 0, true);
                arraylist.Add(metadataInfo);
            }
            else if (tableType == EAuxiliaryTableType.GovPublicContent)
            {
                var metadataInfo = new TableMetadataInfo(0, tableName, ContentAttribute.Title, EDataType.NVarChar, 255, 0, true);
                arraylist.Add(metadataInfo);
                metadataInfo = new TableMetadataInfo(0, tableName, GovPublicContentAttribute.Identifier, EDataType.NVarChar, 255, 0, true);
                arraylist.Add(metadataInfo);
                metadataInfo = new TableMetadataInfo(0, tableName, GovPublicContentAttribute.Description, EDataType.NText, 16, 0, true);
                arraylist.Add(metadataInfo);
                metadataInfo = new TableMetadataInfo(0, tableName, GovPublicContentAttribute.PublishDate, EDataType.DateTime, 8, 0, true);
                arraylist.Add(metadataInfo);
                metadataInfo = new TableMetadataInfo(0, tableName, GovPublicContentAttribute.EffectDate, EDataType.DateTime, 8, 0, true);
                arraylist.Add(metadataInfo);
                metadataInfo = new TableMetadataInfo(0, tableName, GovPublicContentAttribute.IsAbolition, EDataType.VarChar, 18, 0, true);
                arraylist.Add(metadataInfo);
                metadataInfo = new TableMetadataInfo(0, tableName, GovPublicContentAttribute.AbolitionDate, EDataType.DateTime, 8, 0, true);
                arraylist.Add(metadataInfo);
                metadataInfo = new TableMetadataInfo(0, tableName, GovPublicContentAttribute.DocumentNo, EDataType.NVarChar, 255, 0, true);
                arraylist.Add(metadataInfo);
                metadataInfo = new TableMetadataInfo(0, tableName, GovPublicContentAttribute.Publisher, EDataType.NVarChar, 255, 0, true);
                arraylist.Add(metadataInfo);
                metadataInfo = new TableMetadataInfo(0, tableName, GovPublicContentAttribute.Keywords, EDataType.NVarChar, 255, 0, true);
                arraylist.Add(metadataInfo);
                metadataInfo = new TableMetadataInfo(0, tableName, GovPublicContentAttribute.ImageUrl, EDataType.NVarChar, 255, 0, true);
                arraylist.Add(metadataInfo);
                metadataInfo = new TableMetadataInfo(0, tableName, GovPublicContentAttribute.FileUrl, EDataType.NVarChar, 255, 0, true);
                arraylist.Add(metadataInfo);
                metadataInfo = new TableMetadataInfo(0, tableName, GovPublicContentAttribute.IsRecommend, EDataType.VarChar, 18, 0, true);
                arraylist.Add(metadataInfo);
                metadataInfo = new TableMetadataInfo(0, tableName, GovPublicContentAttribute.IsHot, EDataType.VarChar, 18, 0, true);
                arraylist.Add(metadataInfo);
                metadataInfo = new TableMetadataInfo(0, tableName, GovPublicContentAttribute.IsColor, EDataType.VarChar, 18, 0, true);
                arraylist.Add(metadataInfo);
                metadataInfo = new TableMetadataInfo(0, tableName, ContentAttribute.IsTop, EDataType.VarChar, 18, 0, true);
                arraylist.Add(metadataInfo);
                metadataInfo = new TableMetadataInfo(0, tableName, GovPublicContentAttribute.Content, EDataType.NText, 16, 0, true);
                arraylist.Add(metadataInfo);
                metadataInfo = new TableMetadataInfo(0, tableName, ContentAttribute.AddDate, EDataType.DateTime, 8, 0, true);
                arraylist.Add(metadataInfo);
            }
            else if (tableType == EAuxiliaryTableType.GovInteractContent)
            {
                var metadataInfo = new TableMetadataInfo(0, tableName, GovInteractContentAttribute.RealName, EDataType.NVarChar, 255, 0, true);
                arraylist.Add(metadataInfo);
                metadataInfo = new TableMetadataInfo(0, tableName, GovInteractContentAttribute.Organization, EDataType.NVarChar, 255, 0, true);
                arraylist.Add(metadataInfo);
                metadataInfo = new TableMetadataInfo(0, tableName, GovInteractContentAttribute.CardType, EDataType.NVarChar, 255, 0, true);
                arraylist.Add(metadataInfo);
                metadataInfo = new TableMetadataInfo(0, tableName, GovInteractContentAttribute.CardNo, EDataType.NVarChar, 255, 0, true);
                arraylist.Add(metadataInfo);
                metadataInfo = new TableMetadataInfo(0, tableName, GovInteractContentAttribute.Phone, EDataType.VarChar, 50, 0, true);
                arraylist.Add(metadataInfo);
                metadataInfo = new TableMetadataInfo(0, tableName, GovInteractContentAttribute.PostCode, EDataType.VarChar, 50, 0, true);
                arraylist.Add(metadataInfo);
                metadataInfo = new TableMetadataInfo(0, tableName, GovInteractContentAttribute.Address, EDataType.NVarChar, 255, 0, true);
                arraylist.Add(metadataInfo);
                metadataInfo = new TableMetadataInfo(0, tableName, GovInteractContentAttribute.Email, EDataType.NVarChar, 255, 0, true);
                arraylist.Add(metadataInfo);
                metadataInfo = new TableMetadataInfo(0, tableName, GovInteractContentAttribute.Fax, EDataType.VarChar, 50, 0, true);
                arraylist.Add(metadataInfo);
                metadataInfo = new TableMetadataInfo(0, tableName, GovInteractContentAttribute.TypeId, EDataType.Integer, 38, 0, true);
                arraylist.Add(metadataInfo);
                metadataInfo = new TableMetadataInfo(0, tableName, GovInteractContentAttribute.IsPublic, EDataType.VarChar, 18, 0, true);
                arraylist.Add(metadataInfo);
                metadataInfo = new TableMetadataInfo(0, tableName, GovInteractContentAttribute.Title, EDataType.NVarChar, 255, 0, true);
                arraylist.Add(metadataInfo);
                metadataInfo = new TableMetadataInfo(0, tableName, GovInteractContentAttribute.Content, EDataType.NText, 16, 0, true);
                arraylist.Add(metadataInfo);
                metadataInfo = new TableMetadataInfo(0, tableName, GovInteractContentAttribute.FileUrl, EDataType.NVarChar, 255, 0, true);
                arraylist.Add(metadataInfo);
                metadataInfo = new TableMetadataInfo(0, tableName, GovInteractContentAttribute.DepartmentId, EDataType.Integer, 38, 0, true);
                arraylist.Add(metadataInfo);
            }
            else if (tableType == EAuxiliaryTableType.VoteContent)
            {
                var metadataInfo = new TableMetadataInfo(0, tableName, VoteContentAttribute.Title, EDataType.NVarChar, 255, 0, true);
                arraylist.Add(metadataInfo);
                metadataInfo = new TableMetadataInfo(0, tableName, VoteContentAttribute.SubTitle, EDataType.NVarChar, 255, 0, true);
                arraylist.Add(metadataInfo);
                metadataInfo = new TableMetadataInfo(0, tableName, VoteContentAttribute.MaxSelectNum, EDataType.Integer, 38, 0, true);
                arraylist.Add(metadataInfo);
                metadataInfo = new TableMetadataInfo(0, tableName, VoteContentAttribute.ImageUrl, EDataType.VarChar, 200, 0, true);
                arraylist.Add(metadataInfo);
                metadataInfo = new TableMetadataInfo(0, tableName, VoteContentAttribute.Content, EDataType.NText, 16, 0, true);
                arraylist.Add(metadataInfo);
                metadataInfo = new TableMetadataInfo(0, tableName, VoteContentAttribute.Summary, EDataType.NText, 16, 0, true);
                arraylist.Add(metadataInfo);
                metadataInfo = new TableMetadataInfo(0, tableName, VoteContentAttribute.AddDate, EDataType.DateTime, 8, 0, true);
                arraylist.Add(metadataInfo);
                metadataInfo = new TableMetadataInfo(0, tableName, VoteContentAttribute.EndDate, EDataType.DateTime, 8, 0, true);
                arraylist.Add(metadataInfo);
                metadataInfo = new TableMetadataInfo(0, tableName, VoteContentAttribute.IsVotedView, EDataType.VarChar, 18, 0, true);
                arraylist.Add(metadataInfo);
                metadataInfo = new TableMetadataInfo(0, tableName, VoteContentAttribute.HiddenContent, EDataType.NText, 16, 0, true);
                arraylist.Add(metadataInfo);
            }
            else if (tableType == EAuxiliaryTableType.JobContent)
            {
                var metadataInfo = new TableMetadataInfo(0, tableName, ContentAttribute.Title, EDataType.NVarChar, 255, 0, true);
                arraylist.Add(metadataInfo);
                metadataInfo = new TableMetadataInfo(0, tableName, JobContentAttribute.Department, EDataType.NVarChar, 255, 0, true);
                arraylist.Add(metadataInfo);
                metadataInfo = new TableMetadataInfo(0, tableName, JobContentAttribute.Location, EDataType.NVarChar, 255, 0, true);
                arraylist.Add(metadataInfo);
                metadataInfo = new TableMetadataInfo(0, tableName, JobContentAttribute.NumberOfPeople, EDataType.NVarChar, 50, 0, true);
                arraylist.Add(metadataInfo);
                metadataInfo = new TableMetadataInfo(0, tableName, JobContentAttribute.Responsibility, EDataType.NText, 16, 0, true);
                arraylist.Add(metadataInfo);
                metadataInfo = new TableMetadataInfo(0, tableName, JobContentAttribute.Requirement, EDataType.NText, 16, 0, true);
                arraylist.Add(metadataInfo);
                metadataInfo = new TableMetadataInfo(0, tableName, JobContentAttribute.IsUrgent, EDataType.VarChar, 18, 0, true);
                arraylist.Add(metadataInfo);
                metadataInfo = new TableMetadataInfo(0, tableName, ContentAttribute.IsTop, EDataType.VarChar, 18, 0, true);
                arraylist.Add(metadataInfo);
                metadataInfo = new TableMetadataInfo(0, tableName, ContentAttribute.AddDate, EDataType.DateTime, 8, 0, true);
                arraylist.Add(metadataInfo);
            }
            else if (tableType == EAuxiliaryTableType.UserDefined)
            {
                var metadataInfo = new TableMetadataInfo(0, tableName, ContentAttribute.Title, EDataType.NVarChar, 255, 0, true);
                arraylist.Add(metadataInfo);
                metadataInfo = new TableMetadataInfo(0, tableName, ContentAttribute.IsTop, EDataType.VarChar, 18, 0, true);
                arraylist.Add(metadataInfo);
                metadataInfo = new TableMetadataInfo(0, tableName, ContentAttribute.AddDate, EDataType.DateTime, 8, 0, true);
                arraylist.Add(metadataInfo);
            }
            return arraylist;
        }
    }
}
