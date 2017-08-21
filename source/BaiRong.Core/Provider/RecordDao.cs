using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using BaiRong.Core.Data;
using SiteServer.Plugin;
using SiteServer.Plugin.Models;

namespace BaiRong.Core.Provider
{
    public class RecordDao : DataProviderBase
    {
        private const string TableName = "bairong_Record";

        private const string ParmText = "@Text";
        private const string ParmSummary = "@Summary";
        private const string ParmSource = "@Source";
        private const string ParmAddDate = "@AddDate";

        /**
sqlserver

CREATE TABLE bairong_Record(
    Id            int              IDENTITY(1,1),
    Text    nvarchar(2000)            NULL,
    Summary    nvarchar(2000)            NULL,
    Source       nvarchar(200)    NULL,
    AddDate       datetime         NULL,
    CONSTRAINT PK_bairong_Record PRIMARY KEY CLUSTERED (Id)
)

mysql

CREATE TABLE bairong_ErrorLog(
    Id            INT                      AUTO_INCREMENT,
    Text    NATIONAL VARCHAR(2000),
    Summary       NATIONAL VARCHAR(2000),
    Source        NATIONAL VARCHAR(200),
    AddDate       DATETIME,
    PRIMARY KEY (Id)
)ENGINE=INNODB
    **/

        private void Insert(string text, string summary, string source)
        {
            var sqlString = $"INSERT INTO {TableName} (Text, Summary, Source, AddDate) VALUES (@Text, @Summary, @Source, @AddDate)";

            var parms = new IDataParameter[]
            {
                GetParameter(ParmText, DataType.NVarChar, 2000, text),
                GetParameter(ParmSummary, DataType.NVarChar, 2000, summary),
                GetParameter(ParmSource, DataType.NVarChar, 200, source),
                GetParameter(ParmAddDate, DataType.DateTime, DateTime.Now)
            };

            ExecuteNonQuery(sqlString, parms);
        }

        public void Delete(List<int> idList)
        {
            if (idList != null && idList.Count > 0)
            {
                var sqlString =
                    $"DELETE FROM {TableName} WHERE ID IN ({TranslateUtils.ToSqlInStringWithoutQuote(idList)})";

                ExecuteNonQuery(sqlString);
            }
        }

        public void DeleteAll()
        {
            var sqlString = $"DELETE FROM {TableName}";

            ExecuteNonQuery(sqlString);
        }

        public string GetSelectCommend()
        {
            return $"SELECT Id, Text, Summary, Source, AddDate FROM {TableName}";
        }

        public string GetSelectCommend(string keyword, string dateFrom, string dateTo)
        {
            if (string.IsNullOrEmpty(keyword) && string.IsNullOrEmpty(dateFrom) && string.IsNullOrEmpty(dateTo))
            {
                return GetSelectCommend();
            }

            var whereString = new StringBuilder("WHERE ");

            var isWhere = false;

            if (!string.IsNullOrEmpty(keyword))
            {
                isWhere = true;
                var filterKeyword = PageUtils.FilterSql(keyword);
                whereString.Append(
                    $"(Text LIKE '%{filterKeyword}%' OR Summary LIKE '%{filterKeyword}%' OR Source LIKE '%{filterKeyword}%')");
            }

            if (!string.IsNullOrEmpty(dateFrom))
            {
                if (isWhere)
                {
                    whereString.Append(" AND ");
                }
                isWhere = true;
                whereString.Append($"(AddDate >= '{dateFrom}')");
            }
            if (!string.IsNullOrEmpty(dateTo))
            {
                if (isWhere)
                {
                    whereString.Append(" AND ");
                }
                whereString.Append($"(AddDate <= '{dateTo}')");
            }

            return $"SELECT Id, Text, Summary, Source, AddDate FROM {TableName} {whereString}";
        }

        private static readonly object LockObject = new object();

        public bool IsRecord()
        {
#if (DEBUG)
            const string cacheKey = "BaiRong.Core.Provider.RecordDao.IsRecord";
            var retval = CacheUtils.GetInt(cacheKey, -1);
            if (retval != -1) return retval == 1;

            lock (LockObject)
            {
                retval = CacheUtils.GetInt(cacheKey, -1);
                if (retval == -1)
                {
                    retval = FileUtils.IsFileExists(PathUtils.GetTemporaryFilesPath("record.txt")) ? 1 : 0;
                    CacheUtils.Insert(cacheKey, retval);
                }
            }

            return retval == 1;
#else
            return false;
#endif
        }

        public void RecordCommandExecute(IDbCommand command, string source)
        {
            if (!IsRecord()) return;
            if (command.CommandText.Contains(TableName)) return;

            var builder = new StringBuilder();
            foreach (var parameter in command.Parameters)
            {
                var commandParameter = parameter as IDataParameter;
                if (commandParameter != null)
                {
                    builder.Append(commandParameter.ParameterName + "=" + commandParameter.Value + "<br />").AppendLine();
                }
            }

            Insert(command.CommandText, builder.ToString(), source);
        }
    }
}
