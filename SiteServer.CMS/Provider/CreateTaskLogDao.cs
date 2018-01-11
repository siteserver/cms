using System.Collections.Generic;
using System.Data;
using System.Text;
using BaiRong.Core;
using BaiRong.Core.Data;
using BaiRong.Core.Model;
using BaiRong.Core.Model.Enumerations;
using SiteServer.CMS.Model;
using SiteServer.CMS.Model.Enumerations;
using SiteServer.Plugin.Models;

namespace SiteServer.CMS.Provider
{
    public class CreateTaskLogDao : DataProviderBase
    {
        public override string TableName => "siteserver_CreateTaskLog";

        public override List<TableColumnInfo> TableColumns => new List<TableColumnInfo>
        {
            new TableColumnInfo
            {
                ColumnName = nameof(CreateTaskLogInfo.Id),
                DataType = DataType.Integer,
                IsIdentity = true,
                IsPrimaryKey = true
            },
            new TableColumnInfo
            {
                ColumnName = nameof(CreateTaskLogInfo.CreateType),
                DataType = DataType.VarChar,
                Length = 50
            },
            new TableColumnInfo
            {
                ColumnName = nameof(CreateTaskLogInfo.PublishmentSystemId),
                DataType = DataType.Integer
            },
            new TableColumnInfo
            {
                ColumnName = nameof(CreateTaskLogInfo.ChannelId),
                DataType = DataType.Integer
            },
            new TableColumnInfo
            {
                ColumnName = nameof(CreateTaskLogInfo.ContentId),
                DataType = DataType.Integer
            },
            new TableColumnInfo
            {
                ColumnName = nameof(CreateTaskLogInfo.TemplateId),
                DataType = DataType.Integer
            },
            new TableColumnInfo
            {
                ColumnName = nameof(CreateTaskLogInfo.TaskName),
                DataType = DataType.VarChar,
                Length = 50
            },
            new TableColumnInfo
            {
                ColumnName = nameof(CreateTaskLogInfo.TimeSpan),
                DataType = DataType.VarChar,
                Length = 50
            },
            new TableColumnInfo
            {
                ColumnName = nameof(CreateTaskLogInfo.IsSuccess),
                DataType = DataType.VarChar,
                Length = 18
            },
            new TableColumnInfo
            {
                ColumnName = nameof(CreateTaskLogInfo.ErrorMessage),
                DataType = DataType.VarChar,
                Length = 255
            },
            new TableColumnInfo
            {
                ColumnName = nameof(CreateTaskLogInfo.AddDate),
                DataType = DataType.DateTime
            }
        };

        private const string ParmCreateType = "@CreateType";
        private const string ParmPublishmentSystemId = "@PublishmentSystemId";
        private const string ParmChannelId = "@ChannelId";
        private const string ParmContentId = "@ContentId";
        private const string ParmTemplateId = "@TemplateId";
        private const string ParmTaskName = "@TaskName";
        private const string ParmTimeSpan = "@TimeSpan";
        private const string ParmIsSuccess = "@IsSuccess";
        private const string ParmErrorMessage = "@ErrorMessage";
        private const string ParmAddDate = "@AddDate";

        public void Insert(CreateTaskLogInfo log)
        {
            DeleteExcess90Days();

            const string sqlString = "INSERT INTO siteserver_CreateTaskLog(CreateType, PublishmentSystemId, ChannelId, ContentId, TemplateId, TaskName, TimeSpan, IsSuccess, ErrorMessage, AddDate) VALUES (@CreateType, @PublishmentSystemId, @ChannelId, @ContentId, @TemplateId, @TaskName, @TimeSpan, @IsSuccess, @ErrorMessage, @AddDate)";

            var parms = new IDataParameter[]
            {
                GetParameter(ParmCreateType, DataType.VarChar, 50, ECreateTypeUtils.GetValue(log.CreateType)),
                GetParameter(ParmPublishmentSystemId, DataType.Integer, log.PublishmentSystemId),
                GetParameter(ParmChannelId, DataType.Integer, log.ChannelId),
                GetParameter(ParmContentId, DataType.Integer, log.ContentId),
                GetParameter(ParmTemplateId, DataType.Integer, log.TemplateId),
                GetParameter(ParmTaskName, DataType.VarChar, 50, log.TaskName),
                GetParameter(ParmTimeSpan, DataType.VarChar, 50, log.TimeSpan),
                GetParameter(ParmIsSuccess, DataType.VarChar, 18, log.IsSuccess.ToString()),
                GetParameter(ParmErrorMessage, DataType.VarChar, 255, log.ErrorMessage),
                GetParameter(ParmAddDate, DataType.DateTime, log.AddDate)
            };

            ExecuteNonQuery(sqlString, parms);
        }

        public void Delete(List<int> idList)
        {
            if (idList == null || idList.Count <= 0) return;

            string sqlString =
                $"DELETE FROM siteserver_CreateTaskLog WHERE Id IN ({TranslateUtils.ToSqlInStringWithoutQuote(idList)})";

            ExecuteNonQuery(sqlString);
        }

        public void DeleteExcess90Days()
        {
            ExecuteNonQuery("DELETE FROM siteserver_CreateTaskLog WHERE " + SqlUtils.GetDateDiffGreatThanDays("AddDate", 90.ToString()));
        }

        public void DeleteAll()
        {
            const string sqlString = "DELETE FROM siteserver_CreateTaskLog";

            ExecuteNonQuery(sqlString);
        }

        public List<CreateTaskLogInfo> GetList(int publishmentSystemId, int totalNum)
        {
            var list = new List<CreateTaskLogInfo>();

            if (publishmentSystemId > 0)
            {
                var sqlString = SqlUtils.ToTopSqlString("siteserver_CreateTaskLog", "Id, CreateType, PublishmentSystemId, ChannelId, ContentId, TemplateId, TaskName, TimeSpan, IsSuccess, ErrorMessage, AddDate", "WHERE PublishmentSystemId = @PublishmentSystemId", string.Empty, totalNum);

                var parms = new IDataParameter[]
                {
                    GetParameter(ParmPublishmentSystemId, DataType.Integer, publishmentSystemId)
                };

                using (var rdr = ExecuteReader(sqlString, parms))
                {
                    while (rdr.Read())
                    {
                        var i = 0;
                        var info = new CreateTaskLogInfo(GetInt(rdr, i++), ECreateTypeUtils.GetEnumType(GetString(rdr, i++)), GetInt(rdr, i++), GetInt(rdr, i++), GetInt(rdr, i++), GetInt(rdr, i++), GetString(rdr, i++), GetString(rdr, i++), GetBool(rdr, i++), GetString(rdr, i++), GetDateTime(rdr, i));
                        list.Add(info);
                    }
                    rdr.Close();
                }
            }
            else
            {
                var sqlString = SqlUtils.ToTopSqlString("siteserver_CreateTaskLog", "Id, CreateType, PublishmentSystemId, ChannelId, ContentId, TemplateId, TaskName, TimeSpan, IsSuccess, ErrorMessage, AddDate", string.Empty, string.Empty, totalNum);

                using (var rdr = ExecuteReader(sqlString))
                {
                    while (rdr.Read())
                    {
                        var i = 0;
                        var info = new CreateTaskLogInfo(GetInt(rdr, i++), ECreateTypeUtils.GetEnumType(GetString(rdr, i++)), GetInt(rdr, i++), GetInt(rdr, i++), GetInt(rdr, i++), GetInt(rdr, i++), GetString(rdr, i++), GetString(rdr, i++), GetBool(rdr, i++), GetString(rdr, i++), GetDateTime(rdr, i));
                        list.Add(info);
                    }
                    rdr.Close();
                }
            }

            return list;
        }

        public string GetSelectCommend()
        {
            return "SELECT Id, CreateType, PublishmentSystemId, ChannelId, ContentId, TemplateId, TaskName, TimeSpan, IsSuccess, ErrorMessage, AddDate FROM siteserver_CreateTaskLog";
        }

        public string GetSelectCommend(ETriState successState, string keyword, string dateFrom, string dateTo)
        {
            var whereString = new StringBuilder("WHERE ");

            if (successState != ETriState.All)
            {
                whereString.AppendFormat("IsSuccess = '{0}'", ETriStateUtils.GetValue(successState));
            }
            else
            {
                whereString.Append("1 = 1");
            }            

            if (!string.IsNullOrEmpty(keyword))
            {
                whereString.Append(" AND ");
                whereString.AppendFormat("(ErrorMessage LIKE '%{0}%')", PageUtils.FilterSql(keyword));
            }

            if (!string.IsNullOrEmpty(dateFrom))
            {
                whereString.Append(" AND ");
                whereString.Append($"(AddDate >= {SqlUtils.GetComparableDate(TranslateUtils.ToDateTime(dateFrom))})");
            }
            if (!string.IsNullOrEmpty(dateTo))
            {
                whereString.Append(" AND ");
                whereString.Append($"(AddDate <= {SqlUtils.GetComparableDate(TranslateUtils.ToDateTime(dateTo))})");
            }

            return "SELECT Id, CreateType, PublishmentSystemId, ChannelId, ContentId, TemplateId, TaskName, TimeSpan, IsSuccess, ErrorMessage, AddDate FROM siteserver_CreateTaskLog " + whereString;
        }
    }
}
