using System;
using System.Collections.Generic;
using System.Text;
using SS.CMS.Core.Cache;
using SS.CMS.Core.Common;
using SS.CMS.Core.Models;
using SS.CMS.Core.Settings;
using SS.CMS.Data;
using SS.CMS.Utils;
using AppContext = SS.CMS.Core.Settings.AppContext;

namespace SS.CMS.Core.Repositories
{
    public class SiteLogDao : IDatabaseDao
    {
        private readonly Repository<SiteLogInfo> _repository;
        public SiteLogDao(IDb db)
        {
            _repository = new Repository<SiteLogInfo>(db);
        }

        public string TableName => _repository.TableName;
        public List<TableColumn> TableColumns => _repository.TableColumns;

        private static class Attr
        {
            public const string Id = nameof(SiteLogInfo.Id);
            public const string AddDate = nameof(SiteLogInfo.AddDate);
        }

        public void Insert(SiteLogInfo logInfo)
        {
            _repository.Insert(logInfo);
        }

        public void DeleteIfThreshold()
        {
            if (!ConfigManager.Instance.IsTimeThreshold) return;

            var days = ConfigManager.Instance.TimeThreshold;
            if (days <= 0) return;

            _repository.Delete(Q.Where(Attr.AddDate, "<", DateTime.Now.AddDays(-days)));
        }

        public void Delete(List<int> idList)
        {
            if (idList == null || idList.Count <= 0) return;

            _repository.Delete(Q.WhereIn(Attr.Id, idList));
        }

        public void DeleteAll()
        {
            _repository.Delete();
        }

        public string GetSelectCommend()
        {
            return "SELECT Id, SiteId, ChannelId, ContentId, UserName, IpAddress, AddDate, Action, Summary FROM siteserver_SiteLog";
        }

        public string GetSelectCommend(int siteId, string logType, string userName, string keyword, string dateFrom, string dateTo)
        {
            if (siteId == 0 && (string.IsNullOrEmpty(logType) || StringUtils.EqualsIgnoreCase(logType, "All")) && string.IsNullOrEmpty(userName) && string.IsNullOrEmpty(keyword) && string.IsNullOrEmpty(dateFrom) && string.IsNullOrEmpty(dateTo))
            {
                return GetSelectCommend();
            }

            var whereString = new StringBuilder("WHERE ");

            var isWhere = false;

            if (siteId > 0)
            {
                isWhere = true;
                whereString.AppendFormat("(SiteId = {0})", siteId);
            }

            if (!string.IsNullOrEmpty(logType) && !StringUtils.EqualsIgnoreCase(logType, "All"))
            {
                if (isWhere)
                {
                    whereString.Append(" AND ");
                }
                isWhere = true;

                if (StringUtils.EqualsIgnoreCase(logType, "Channel"))
                {
                    whereString.Append("(ChannelId > 0 AND ContentId = 0)");
                }
                else if (StringUtils.EqualsIgnoreCase(logType, "Content"))
                {
                    whereString.Append("(ChannelId > 0 AND ContentId > 0)");
                }
            }

            if (!string.IsNullOrEmpty(userName))
            {
                if (isWhere)
                {
                    whereString.Append(" AND ");
                }
                isWhere = true;
                whereString.AppendFormat("(UserName = '{0}')", userName);
            }

            if (!string.IsNullOrEmpty(keyword))
            {
                if (isWhere)
                {
                    whereString.Append(" AND ");
                }
                isWhere = true;
                whereString.AppendFormat("(Action LIKE '%{0}%' OR Summary LIKE '%{0}%')", AttackUtils.FilterSql(keyword));
            }

            if (!string.IsNullOrEmpty(dateFrom))
            {
                if (isWhere)
                {
                    whereString.Append(" AND ");
                }
                isWhere = true;
                whereString.Append($"(AddDate >= {SqlUtils.GetComparableDate(TranslateUtils.ToDateTime(dateFrom))})");
            }
            if (!string.IsNullOrEmpty(dateTo))
            {
                if (isWhere)
                {
                    whereString.Append(" AND ");
                }
                whereString.Append($"(AddDate <= {SqlUtils.GetComparableDate(TranslateUtils.ToDateTime(dateTo))})");
            }

            return "SELECT Id, SiteId, ChannelId, ContentId, UserName, IpAddress, AddDate, Action, Summary FROM siteserver_SiteLog " + whereString;
        }
    }
}

// using System;
// using System.Collections.Generic;
// using System.Data;
// using System.Text;
// using Datory;
// using SiteServer.CMS.Core;
// using SiteServer.CMS.DataCache;
// using SiteServer.CMS.Model;
// using SiteServer.Utils;

// namespace SiteServer.CMS.Provider
// {
//     public class SiteLogDao
//     {
//         public override string TableName => "siteserver_SiteLog";

//         public override List<TableColumn> TableColumns => new List<TableColumn>
//         {
//             new TableColumn
//             {
//                 AttributeName = nameof(SiteLogInfo.Id),
//                 DataType = DataType.Integer,
//                 IsIdentity = true,
//                 IsPrimaryKey = true
//             },
//             new TableColumn
//             {
//                 AttributeName = nameof(SiteLogInfo.SiteId),
//                 DataType = DataType.Integer
//             },
//             new TableColumn
//             {
//                 AttributeName = nameof(SiteLogInfo.ChannelId),
//                 DataType = DataType.Integer
//             },
//             new TableColumn
//             {
//                 AttributeName = nameof(SiteLogInfo.ContentId),
//                 DataType = DataType.Integer
//             },
//             new TableColumn
//             {
//                 AttributeName = nameof(SiteLogInfo.UserName),
//                 DataType = DataType.VarChar,
//                 DataLength = 50
//             },
//             new TableColumn
//             {
//                 AttributeName = nameof(SiteLogInfo.IpAddress),
//                 DataType = DataType.VarChar,
//                 DataLength = 50
//             },
//             new TableColumn
//             {
//                 AttributeName = nameof(SiteLogInfo.AddDate),
//                 DataType = DataType.DateTime
//             },
//             new TableColumn
//             {
//                 AttributeName = nameof(SiteLogInfo.Action),
//                 DataType = DataType.VarChar,
//                 DataLength = 255
//             },
//             new TableColumn
//             {
//                 AttributeName = nameof(SiteLogInfo.Summary),
//                 DataType = DataType.VarChar,
//                 DataLength = 255
//             }
//         };

//         private const string ParmSiteId = "@SiteId";
//         private const string ParmChannelId = "@ChannelId";
//         private const string ParmContentId = "@ContentId";
//         private const string ParmUserName = "@UserName";
//         private const string ParmIpAddress = "@IpAddress";
//         private const string ParmAddDate = "@AddDate";
//         private const string ParmAction = "@Action";
//         private const string ParmSummary = "@Summary";

//         public void Insert(SiteLogInfo log)
//         {
//             var sqlString = "INSERT INTO siteserver_SiteLog(SiteId, ChannelId, ContentId, UserName, IpAddress, AddDate, Action, Summary) VALUES (@SiteId, @ChannelId, @ContentId, @UserName, @IpAddress, @AddDate, @Action, @Summary)";

//             var parms = new IDataParameter[]
// 			{
//                 GetParameter(ParmSiteId, DataType.Integer, log.SiteId),
//                 GetParameter(ParmChannelId, DataType.Integer, log.ChannelId),
//                 GetParameter(ParmContentId, DataType.Integer, log.ContentId),
// 				GetParameter(ParmUserName, DataType.VarChar, 50, log.UserName),
// 				GetParameter(ParmIpAddress, DataType.VarChar, 50, log.IpAddress),
//                 GetParameter(ParmAddDate, DataType.DateTime, log.AddDate),
// 				GetParameter(ParmAction, DataType.VarChar, 255, log.Action),
// 				GetParameter(ParmSummary, DataType.VarChar, 255, log.Summary)
// 			};

//             ExecuteNonQuery(sqlString, parms);
//         }

//         public void DeleteIfThreshold()
//         {
//             if (!ConfigManager.Instance.IsTimeThreshold) return;

//             var days = ConfigManager.Instance.TimeThreshold;
//             if (days <= 0) return;

//             ExecuteNonQuery($@"DELETE FROM siteserver_SiteLog WHERE AddDate < {SqlUtils.GetComparableDateTime(DateTime.Now.AddDays(-days))}");
//         }

//         public void Delete(List<int> idList)
//         {
//             if (idList != null && idList.Count > 0)
//             {
//                 string sqlString =
//                     $"DELETE FROM siteserver_SiteLog WHERE Id IN ({TranslateUtils.ToSqlInStringWithoutQuote(idList)})";

//                 ExecuteNonQuery(sqlString);
//             }
//         }

//         public void DeleteAll()
//         {
//             var sqlString = "DELETE FROM siteserver_SiteLog";

//             ExecuteNonQuery(sqlString);
//         }

//         public string GetSelectCommend()
//         {
//             return "SELECT Id, SiteId, ChannelId, ContentId, UserName, IpAddress, AddDate, Action, Summary FROM siteserver_SiteLog";
//         }

//         public string GetSelectCommend(int siteId, string logType, string userName, string keyword, string dateFrom, string dateTo)
//         {
//             if (siteId == 0 && (string.IsNullOrEmpty(logType) || StringUtils.EqualsIgnoreCase(logType, "All")) && string.IsNullOrEmpty(userName) && string.IsNullOrEmpty(keyword) && string.IsNullOrEmpty(dateFrom) && string.IsNullOrEmpty(dateTo))
//             {
//                 return GetSelectCommend();
//             }

//             var whereString = new StringBuilder("WHERE ");

//             var isWhere = false;

//             if (siteId > 0)
//             {
//                 isWhere = true;
//                 whereString.AppendFormat("(SiteId = {0})", siteId);
//             }

//             if (!string.IsNullOrEmpty(logType) && !StringUtils.EqualsIgnoreCase(logType, "All"))
//             {
//                 if (isWhere)
//                 {
//                     whereString.Append(" AND ");
//                 }
//                 isWhere = true;

//                 if (StringUtils.EqualsIgnoreCase(logType, "Channel"))
//                 {
//                     whereString.Append("(ChannelId > 0 AND ContentId = 0)");
//                 }
//                 else if (StringUtils.EqualsIgnoreCase(logType, "Content"))
//                 {
//                     whereString.Append("(ChannelId > 0 AND ContentId > 0)");
//                 }
//             }

//             if (!string.IsNullOrEmpty(userName))
//             {
//                 if (isWhere)
//                 {
//                     whereString.Append(" AND ");
//                 }
//                 isWhere = true;
//                 whereString.AppendFormat("(UserName = '{0}')", userName);
//             }

//             if (!string.IsNullOrEmpty(keyword))
//             {
//                 if (isWhere)
//                 {
//                     whereString.Append(" AND ");
//                 }
//                 isWhere = true;
//                 whereString.AppendFormat("(Action LIKE '%{0}%' OR Summary LIKE '%{0}%')", AttackUtils.FilterSql(keyword));
//             }

//             if (!string.IsNullOrEmpty(dateFrom))
//             {
//                 if (isWhere)
//                 {
//                     whereString.Append(" AND ");
//                 }
//                 isWhere = true;
//                 whereString.Append($"(AddDate >= {SqlUtils.GetComparableDate(TranslateUtils.ToDateTime(dateFrom))})");
//             }
//             if (!string.IsNullOrEmpty(dateTo))
//             {
//                 if (isWhere)
//                 {
//                     whereString.Append(" AND ");
//                 }
//                 whereString.Append($"(AddDate <= {SqlUtils.GetComparableDate(TranslateUtils.ToDateTime(dateTo))})");
//             }

//             return "SELECT Id, SiteId, ChannelId, ContentId, UserName, IpAddress, AddDate, Action, Summary FROM siteserver_SiteLog " + whereString;
//         }
//     }
// }
