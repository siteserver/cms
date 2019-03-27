using System;
using System.Collections.Generic;
using System.Text;
using SiteServer.CMS.Caches;
using SiteServer.CMS.Database.Core;
using SiteServer.CMS.Database.Models;
using SiteServer.Utils;

namespace SiteServer.CMS.Database.Repositories
{
    public class SiteLogRepository : GenericRepository<SiteLogInfo>
    {
        private static class Attr
        {
            public const string Id = nameof(SiteLogInfo.Id);
            public const string AddDate = nameof(SiteLogInfo.AddDate);
        }

        public void Insert(SiteLogInfo logInfo)
        {
            //const string sqlString = "INSERT INTO siteserver_SiteLog(SiteId, ChannelId, ContentId, UserName, IpAddress, AddDate, Action, Summary) VALUES (@SiteId, @ChannelId, @ContentId, @UserName, @IpAddress, @AddDate, @Action, @Summary)";

            //IDataParameter[] parameters =
            //{
            //    GetParameter(ParamSiteId, logInfo.SiteId),
            //    GetParameter(ParamChannelId, logInfo.ChannelId),
            //    GetParameter(ParamContentId, logInfo.ContentId),
            //    GetParameter(ParamUserName, logInfo.UserName),
            //    GetParameter(ParamIpAddress, logInfo.IpAddress),
            //    GetParameter(ParamAddDate,logInfo.AddDate),
            //    GetParameter(ParamAction, logInfo.Action),
            //    GetParameter(ParamSummary, logInfo.Summary)
            //};

            //DatabaseApi.ExecuteNonQuery(ConnectionString, sqlString, parameters);

            InsertObject(logInfo);
        }

        public void DeleteIfThreshold()
        {
            if (!ConfigManager.Instance.IsTimeThreshold) return;

            var days = ConfigManager.Instance.TimeThreshold;
            if (days <= 0) return;

            //DatabaseApi.ExecuteNonQuery(ConnectionString, $@"DELETE FROM siteserver_SiteLog WHERE AddDate < {SqlUtils.GetComparableDateTime(DateTime.Now.AddDays(-days))}");

            DeleteAll(Q.Where(Attr.AddDate, "<", DateTime.Now.AddDays(-days)));
        }

        public void Delete(List<int> idList)
        {
            if (idList == null || idList.Count <= 0) return;

            //var sqlString =
            //    $"DELETE FROM siteserver_SiteLog WHERE Id IN ({TranslateUtils.ToSqlInStringWithoutQuote(idList)})";

            //DatabaseApi.ExecuteNonQuery(ConnectionString, sqlString);

            DeleteAll(Q.WhereIn(Attr.Id, idList));
        }

        public void DeleteAll()
        {
            //const string sqlString = "DELETE FROM siteserver_SiteLog";

            //DatabaseApi.ExecuteNonQuery(ConnectionString, sqlString);

            base.DeleteAll();
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


//using System;
//using System.Collections.Generic;
//using System.Data;
//using System.Text;
//using SiteServer.CMS.Database.Caches;
//using SiteServer.CMS.Database.Core;
//using SiteServer.CMS.Database.Models;
//using SiteServer.Plugin;
//using SiteServer.Utils;

//namespace SiteServer.CMS.Database.Repositories
//{
//    public class SiteLog : DataProviderBase
//    {
//        public override string TableName => "siteserver_SiteLog";

//        public override List<TableColumn> TableColumns => new List<TableColumn>
//        {
//            new TableColumn
//            {
//                AttributeName = nameof(SiteLogInfo.Id),
//                DataType = DataType.Integer,
//                IsIdentity = true,
//                IsPrimaryKey = true
//            },
//            new TableColumn
//            {
//                AttributeName = nameof(SiteLogInfo.SiteId),
//                DataType = DataType.Integer
//            },
//            new TableColumn
//            {
//                AttributeName = nameof(SiteLogInfo.ChannelId),
//                DataType = DataType.Integer
//            },
//            new TableColumn
//            {
//                AttributeName = nameof(SiteLogInfo.ContentId),
//                DataType = DataType.Integer
//            },
//            new TableColumn
//            {
//                AttributeName = nameof(SiteLogInfo.UserName),
//                DataType = DataType.VarChar
//            },
//            new TableColumn
//            {
//                AttributeName = nameof(SiteLogInfo.IpAddress),
//                DataType = DataType.VarChar
//            },
//            new TableColumn
//            {
//                AttributeName = nameof(SiteLogInfo.AddDate),
//                DataType = DataType.DateTime
//            },
//            new TableColumn
//            {
//                AttributeName = nameof(SiteLogInfo.Action),
//                DataType = DataType.VarChar
//            },
//            new TableColumn
//            {
//                AttributeName = nameof(SiteLogInfo.Summary),
//                DataType = DataType.VarChar
//            }
//        };

//        private const string ParamSiteId = "@SiteId";
//        private const string ParamChannelId = "@ChannelId";
//        private const string ParamContentId = "@ContentId";
//        private const string ParamUserName = "@UserName";
//        private const string ParamIpAddress = "@IpAddress";
//        private const string ParamAddDate = "@AddDate";
//        private const string ParamAction = "@Action";
//        private const string ParamSummary = "@Summary";

//        public void InsertObject(SiteLogInfo logInfo)
//        {
//            const string sqlString = "INSERT INTO siteserver_SiteLog(SiteId, ChannelId, ContentId, UserName, IpAddress, AddDate, Action, Summary) VALUES (@SiteId, @ChannelId, @ContentId, @UserName, @IpAddress, @AddDate, @Action, @Summary)";

//            IDataParameter[] parameters =
//			{
//                GetParameter(ParamSiteId, logInfo.SiteId),
//                GetParameter(ParamChannelId, logInfo.ChannelId),
//                GetParameter(ParamContentId, logInfo.ContentId),
//				GetParameter(ParamUserName, logInfo.UserName),
//				GetParameter(ParamIpAddress, logInfo.IpAddress),
//                GetParameter(ParamAddDate,logInfo.AddDate),
//				GetParameter(ParamAction, logInfo.Action),
//				GetParameter(ParamSummary, logInfo.Summary)
//			};

//            DatabaseApi.ExecuteNonQuery(ConnectionString, sqlString, parameters);
//        }

//        public void DeleteIfThreshold()
//        {
//            if (!ConfigManager.Instance.IsTimeThreshold) return;

//            var days = ConfigManager.Instance.TimeThreshold;
//            if (days <= 0) return;

//            DatabaseApi.ExecuteNonQuery(ConnectionString, $@"DELETE FROM siteserver_SiteLog WHERE AddDate < {SqlUtils.GetComparableDateTime(DateTime.Now.AddDays(-days))}");
//        }

//        public void DeleteById(List<int> idList)
//        {
//            if (idList == null || idList.Count <= 0) return;

//            var sqlString =
//                $"DELETE FROM siteserver_SiteLog WHERE Id IN ({TranslateUtils.ToSqlInStringWithoutQuote(idList)})";

//            DatabaseApi.ExecuteNonQuery(ConnectionString, sqlString);
//        }

//        public void DeleteAll()
//        {
//            const string sqlString = "DELETE FROM siteserver_SiteLog";

//            DatabaseApi.ExecuteNonQuery(ConnectionString, sqlString);
//        }

//        public string GetSelectCommend()
//        {
//            return "SELECT Id, SiteId, ChannelId, ContentId, UserName, IpAddress, AddDate, Action, Summary FROM siteserver_SiteLog";
//        }

//        public string GetSelectCommend(int siteId, string logType, string userName, string keyword, string dateFrom, string dateTo)
//        {
//            if (siteId == 0 && (string.IsNullOrEmpty(logType) || StringUtils.EqualsIgnoreCase(logType, "All")) && string.IsNullOrEmpty(userName) && string.IsNullOrEmpty(keyword) && string.IsNullOrEmpty(dateFrom) && string.IsNullOrEmpty(dateTo))
//            {
//                return GetSelectCommend();
//            }

//            var whereString = new StringBuilder("WHERE ");

//            var isWhere = false;

//            if (siteId > 0)
//            {
//                isWhere = true;
//                whereString.AppendFormat("(SiteId = {0})", siteId);
//            }

//            if (!string.IsNullOrEmpty(logType) && !StringUtils.EqualsIgnoreCase(logType, "All"))
//            {
//                if (isWhere)
//                {
//                    whereString.Append(" AND ");
//                }
//                isWhere = true;

//                if (StringUtils.EqualsIgnoreCase(logType, "Channel"))
//                {
//                    whereString.Append("(ChannelId > 0 AND ContentId = 0)");
//                }
//                else if (StringUtils.EqualsIgnoreCase(logType, "Content"))
//                {
//                    whereString.Append("(ChannelId > 0 AND ContentId > 0)");
//                }
//            }

//            if (!string.IsNullOrEmpty(userName))
//            {
//                if (isWhere)
//                {
//                    whereString.Append(" AND ");
//                }
//                isWhere = true;
//                whereString.AppendFormat("(UserName = '{0}')", userName);
//            }

//            if (!string.IsNullOrEmpty(keyword))
//            {
//                if (isWhere)
//                {
//                    whereString.Append(" AND ");
//                }
//                isWhere = true;
//                whereString.AppendFormat("(Action LIKE '%{0}%' OR Summary LIKE '%{0}%')", AttackUtils.FilterSql(keyword));
//            }

//            if (!string.IsNullOrEmpty(dateFrom))
//            {
//                if (isWhere)
//                {
//                    whereString.Append(" AND ");
//                }
//                isWhere = true;
//                whereString.Append($"(AddDate >= {SqlUtils.GetComparableDate(TranslateUtils.ToDateTime(dateFrom))})");
//            }
//            if (!string.IsNullOrEmpty(dateTo))
//            {
//                if (isWhere)
//                {
//                    whereString.Append(" AND ");
//                }
//                whereString.Append($"(AddDate <= {SqlUtils.GetComparableDate(TranslateUtils.ToDateTime(dateTo))})");
//            }

//            return "SELECT Id, SiteId, ChannelId, ContentId, UserName, IpAddress, AddDate, Action, Summary FROM siteserver_SiteLog " + whereString;
//        }
//    }
//}
