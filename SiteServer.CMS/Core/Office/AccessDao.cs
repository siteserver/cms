using System.Data;
using System.Data.OleDb;
using System.Collections.Generic;
using System.Text;
using SiteServer.CMS.DataCache;
using SiteServer.CMS.DataCache.Content;
using SiteServer.CMS.Model;
using SiteServer.Utils;
using SiteServer.Utils.Enumerations;

namespace SiteServer.CMS.Core.Office
{
    public class AccessDao
    {
        private readonly OleDbConnection _connection;

        public AccessDao(string filePath)
        {
            ConnectionString = $"Provider=Microsoft.Jet.OLEDB.4.0;Data Source={filePath};User Id=;Password=;";
            _connection = new OleDbConnection(ConnectionString);
        }

        public string ConnectionString { get; }

        public string GetCreateTableSqlString(string nodeName, List<TableStyleInfo> tableStyleInfoList, List<string> displayAttributes)
        {
            var createBuilder = new StringBuilder();
            createBuilder.Append($"CREATE TABLE {nodeName} ( ");

            foreach (var tableStyleInfo in tableStyleInfoList)
            {
                if (displayAttributes.Contains(tableStyleInfo.AttributeName))
                {
                    createBuilder.Append($" [{tableStyleInfo.DisplayName}] memo, ");
                }
            }

            createBuilder.Length = createBuilder.Length - 2;
            createBuilder.Append(" )");

            return createBuilder.ToString();
        }

        public List<string> GetInsertSqlStringList(string nodeName, int siteId, int channelId, string tableName, List<TableStyleInfo> styleInfoList, List<string> displayAttributes, List<int> contentIdList, bool isPeriods, string dateFrom, string dateTo, ETriState checkedState, out bool isExport)
        {
            var siteInfo = SiteManager.GetSiteInfo(siteId);
            var channelInfo = ChannelManager.GetChannelInfo(siteId, channelId);
            var insertSqlList = new List<string>();

            var preInsertBuilder = new StringBuilder();
            preInsertBuilder.Append($"INSERT INTO {nodeName} (");

            foreach (var tableStyleInfo in styleInfoList)
            {
                if (displayAttributes.Contains(tableStyleInfo.AttributeName))
                {
                    preInsertBuilder.Append($"[{tableStyleInfo.DisplayName}], ");
                }
            }

            preInsertBuilder.Length = preInsertBuilder.Length - 2;
            preInsertBuilder.Append(") VALUES (");

            if (contentIdList == null || contentIdList.Count == 0)
            {
                contentIdList = DataProvider.ContentDao.GetContentIdList(tableName, channelId, isPeriods, dateFrom, dateTo, checkedState);
            }

            isExport = contentIdList.Count > 0;

            foreach (var contentId in contentIdList)
            {
                var contentInfo = ContentManager.GetContentInfo(siteInfo, channelInfo, contentId);
                if (contentInfo == null) continue;

                var insertBuilder = new StringBuilder();
                insertBuilder.Append(preInsertBuilder);

                foreach (var tableStyleInfo in styleInfoList)
                {
                    if (displayAttributes.Contains(tableStyleInfo.AttributeName))
                    {
                        var value = contentInfo.GetString(tableStyleInfo.AttributeName);
                        insertBuilder.Append($"'{SqlUtils.ToSqlString(StringUtils.StripTags(value))}', ");
                    }
                }

                insertBuilder.Length = insertBuilder.Length - 2;
                insertBuilder.Append(") ");

                insertSqlList.Add(insertBuilder.ToString());
            }
            return insertSqlList;
        }

        public bool ExecuteSqlString(string sqlString)
        {
            bool resultState;
            OleDbTransaction myTrans = null;

            try
            {
                _connection.Open();
                myTrans = _connection.BeginTransaction();
                var command = new OleDbCommand(sqlString, _connection, myTrans);
                command.ExecuteNonQuery();
                myTrans.Commit();
                resultState = true;
            }
            catch
            {
                myTrans?.Rollback();
                resultState = false;
            }
            finally
            {
                _connection.Close();
            }

            return resultState;
        }

        public DataSet ReturnDataSet(string strSql)
        {
            var dataSet = new DataSet();
            try
            {
                _connection.Open();
                var oleDbDa = new OleDbDataAdapter(strSql, _connection);
                oleDbDa.Fill(dataSet, "objDataSet");                
            }
            finally
            {
                _connection.Close();
            }
            return dataSet;
        }

        public string[] GetTableNames()
        {
            try
            {
                _connection.Open();
                var shemaTable = _connection.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, new object[] { null, null, null, "TABLE" });
                var n = shemaTable.Rows.Count;
                var strTable = new string[n];
                var m = shemaTable.Columns.IndexOf("TABLE_NAME");
                for (var i = 0; i < n; i++)
                {
                    var m_DataRow = shemaTable.Rows[i];
                    strTable[i] = m_DataRow.ItemArray.GetValue(m).ToString();
                }
                return strTable;
            }
            finally
            {
                _connection.Close();
            }
        }
    }

}
