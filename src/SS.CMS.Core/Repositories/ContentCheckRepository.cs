using System.Collections.Generic;
using System.Linq;
using SS.CMS.Data;
using SS.CMS.Models;
using SS.CMS.Repositories;
using SS.CMS.Services;

namespace SS.CMS.Core.Repositories
{
    public class ContentCheckRepository : IContentCheckRepository
    {
        private readonly Repository<ContentCheckInfo> _repository;
        public ContentCheckRepository(ISettingsManager settingsManager)
        {
            _repository = new Repository<ContentCheckInfo>(new Database(settingsManager.DatabaseType, settingsManager.DatabaseConnectionString));
        }

        public IDatabase Database => _repository.Database;
        public string TableName => _repository.TableName;
        public List<TableColumn> TableColumns => _repository.TableColumns;

        private static class Attr
        {
            public const string Id = nameof(ContentCheckInfo.Id);
            public const string TableName = nameof(ContentCheckInfo.TableName);
            public const string ChannelId = nameof(ContentCheckInfo.ChannelId);
        }

        public int Insert(ContentCheckInfo checkInfo)
        {
            checkInfo.Id = _repository.Insert(checkInfo);
            return checkInfo.Id;
        }

        public IList<ContentCheckInfo> GetCheckInfoList(string tableName, int contentId)
        {
            return _repository.GetAll(Q
                .Where(Attr.TableName, tableName)
                .Where(Attr.ChannelId, contentId)
                .OrderByDesc(Attr.Id)).ToList();
        }
    }
}

// using System.Collections.Generic;
// using System.Data;
// using Datory;
// using SiteServer.CMS.Core;
// using SiteServer.CMS.Model;
// using SiteServer.Utils;

// namespace SiteServer.CMS.Provider
// {
//     public class ContentCheckDao
//     {
//         public override string TableName => "siteserver_ContentCheck";

//         public override List<TableColumn> TableColumns => new List<TableColumn>
//         {
//             new TableColumn
//             {
//                 AttributeName = nameof(ContentCheckInfo.Id),
//                 DataType = DataType.Integer,
//                 IsIdentity = true,
//                 IsPrimaryKey = true
//             },
//             new TableColumn
//             {
//                 AttributeName = nameof(ContentCheckInfo.TableName),
//                 DataType = DataType.VarChar,
//                 DataLength = 50
//             },
//             new TableColumn
//             {
//                 AttributeName = nameof(ContentCheckInfo.SiteId),
//                 DataType = DataType.Integer
//             },
//             new TableColumn
//             {
//                 AttributeName = nameof(ContentCheckInfo.ChannelId),
//                 DataType = DataType.Integer
//             },
//             new TableColumn
//             {
//                 AttributeName = nameof(ContentCheckInfo.ContentId),
//                 DataType = DataType.Integer
//             },
//             new TableColumn
//             {
//                 AttributeName = nameof(ContentCheckInfo.UserName),
//                 DataType = DataType.VarChar,
//                 DataLength = 255
//             },
//             new TableColumn
//             {
//                 AttributeName = nameof(ContentCheckInfo.IsChecked),
//                 DataType = DataType.VarChar,
//                 DataLength = 18
//             },
//             new TableColumn
//             {
//                 AttributeName = nameof(ContentCheckInfo.CheckedLevel),
//                 DataType = DataType.Integer
//             },
//             new TableColumn
//             {
//                 AttributeName = nameof(ContentCheckInfo.CheckDate),
//                 DataType = DataType.DateTime
//             },
//             new TableColumn
//             {
//                 AttributeName = nameof(ContentCheckInfo.Reasons),
//                 DataType = DataType.VarChar,
//                 DataLength = 255
//             }
//         };

//         private const string SqlSelect = "SELECT Id, TableName, SiteId, ChannelId, ContentId, UserName, IsChecked, CheckedLevel, CheckDate, Reasons FROM siteserver_ContentCheck WHERE Id = @Id";

//         private const string SqlSelectAll = "SELECT Id, TableName, SiteId, ChannelId, ContentId, UserName, IsChecked, CheckedLevel, CheckDate, Reasons FROM siteserver_ContentCheck WHERE TableName = @TableName AND ContentId = @ContentId ORDER BY Id DESC";

//         private const string SqlDelete = "DELETE FROM siteserver_ContentCheck WHERE Id = @Id";

//         private const string ParmId = "@Id";
//         private const string ParmTableName = "@TableName";
//         private const string ParmSiteId = "@SiteId";
//         private const string ParmChannelId = "@ChannelId";
//         private const string ParmContentId = "@ContentId";
//         private const string ParmUserName = "@UserName";
//         private const string ParmIsChecked = "@IsChecked";
//         private const string ParmCheckedLevel = "@CheckedLevel";
//         private const string ParmCheckDate = "@CheckDate";
//         private const string ParmReasons = "@Reasons";

//         public void Insert(ContentCheckInfo checkInfo)
//         {
//             const string sqlString = "INSERT INTO siteserver_ContentCheck (TableName, SiteId, ChannelId, ContentId, UserName, IsChecked, CheckedLevel, CheckDate, Reasons) VALUES (@TableName, @SiteId, @ChannelId, @ContentId, @UserName, @IsChecked, @CheckedLevel, @CheckDate, @Reasons)";

//             var parms = new IDataParameter[]
//             {
//                 GetParameter(ParmTableName, DataType.VarChar, 50, checkInfo.TableName),
//                 GetParameter(ParmSiteId, DataType.Integer, checkInfo.SiteId),
//                 GetParameter(ParmChannelId, DataType.Integer, checkInfo.ChannelId),
//                 GetParameter(ParmContentId, DataType.Integer, checkInfo.ContentId),
//                 GetParameter(ParmUserName, DataType.VarChar, 255, checkInfo.UserName),
//                 GetParameter(ParmIsChecked, DataType.VarChar, 18, checkInfo.IsChecked.ToString()),
//                 GetParameter(ParmCheckedLevel, DataType.Integer, checkInfo.CheckedLevel),
//                 GetParameter(ParmCheckDate, DataType.DateTime, checkInfo.CheckDate),
//                 GetParameter(ParmReasons, DataType.VarChar, 255, checkInfo.Reasons),
//             };

//             ExecuteNonQuery(sqlString, parms);
//         }

//         public void Delete(int checkId)
//         {
//             var parms = new IDataParameter[]
//             {
//                 GetParameter(ParmId, DataType.Integer, checkId)
//             };

//             ExecuteNonQuery(SqlDelete, parms);
//         }

//         public ContentCheckInfo GetCheckInfo(int checkId)
//         {
//             ContentCheckInfo checkInfo = null;

//             var parms = new IDataParameter[]
//             {
//                 GetParameter(ParmId, DataType.Integer, checkId)
//             };

//             using (var rdr = ExecuteReader(SqlSelect, parms))
//             {
//                 if (rdr.Read())
//                 {
//                     var i = 0;
//                     checkInfo = new ContentCheckInfo(GetInt(rdr, i++), GetString(rdr, i++), GetInt(rdr, i++), GetInt(rdr, i++), GetInt(rdr, i++), GetString(rdr, i++), GetBool(rdr, i++), GetInt(rdr, i++), GetDateTime(rdr, i++), GetString(rdr, i));
//                 }
//                 rdr.Close();
//             }

//             return checkInfo;
//         }

//         public ContentCheckInfo GetCheckInfoByLastId(string tableName, int contentId)
//         {
//             ContentCheckInfo checkInfo = null;

//             //var sqlString = "SELECT TOP 1 Id, TableName, SiteId, ChannelId, ContentId, UserName, IsChecked, CheckedLevel, CheckDate, Reasons FROM siteserver_ContentCheck WHERE TableName = @TableName AND ContentId = @ContentId ORDER BY Id DESC";
//             var sqlString = SqlUtils.ToTopSqlString(TableName, "Id, TableName, SiteId, ChannelId, ContentId, UserName, IsChecked, CheckedLevel, CheckDate, Reasons", "WHERE TableName = @TableName AND ContentId = @ContentId", "ORDER BY Id DESC", 1);

//             var parms = new IDataParameter[]
//             {
//                 GetParameter(ParmTableName, DataType.VarChar, 50, tableName),
//                 GetParameter(ParmContentId, DataType.Integer, contentId)
//             };

//             using (var rdr = ExecuteReader(sqlString, parms))
//             {
//                 if (rdr.Read())
//                 {
//                     var i = 0;
//                     checkInfo = new ContentCheckInfo(GetInt(rdr, i++), GetString(rdr, i++), GetInt(rdr, i++), GetInt(rdr, i++), GetInt(rdr, i++), GetString(rdr, i++), GetBool(rdr, i++), GetInt(rdr, i++), GetDateTime(rdr, i++), GetString(rdr, i));
//                 }
//                 rdr.Close();
//             }

//             return checkInfo;
//         }

//         public List<ContentCheckInfo> GetCheckInfoList(string tableName, int contentId)
//         {
//             var list = new List<ContentCheckInfo>();

//             var parms = new IDataParameter[]
//             {
//                 GetParameter(ParmTableName, DataType.VarChar, 50, tableName),
//                 GetParameter(ParmContentId, DataType.Integer, contentId)
//             };

//             using (var rdr = ExecuteReader(SqlSelectAll, parms))
//             {
//                 while (rdr.Read())
//                 {
//                     var i = 0;
//                     list.Add(new ContentCheckInfo(GetInt(rdr, i++), GetString(rdr, i++), GetInt(rdr, i++), GetInt(rdr, i++), GetInt(rdr, i++), GetString(rdr, i++), GetBool(rdr, i++), GetInt(rdr, i++), GetDateTime(rdr, i++), GetString(rdr, i)));
//                 }
//                 rdr.Close();
//             }

//             return list;
//         }
//     }
// }