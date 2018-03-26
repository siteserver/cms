using System.Collections.Generic;
using System.Data;
using SiteServer.CMS.Data;
using SiteServer.CMS.Model;
using SiteServer.Plugin;
using SiteServer.Utils;

namespace SiteServer.CMS.Provider
{
	public class TableMatchDao : DataProviderBase
	{
        public override string TableName => "siteserver_TableMatch";

        public override List<TableColumnInfo> TableColumns => new List<TableColumnInfo>
        {
            new TableColumnInfo
            {
                ColumnName = nameof(TableMatchInfo.Id),
                DataType = DataType.Integer,
                IsIdentity = true,
                IsPrimaryKey = true
            },
            new TableColumnInfo
            {
                ColumnName = nameof(TableMatchInfo.ConnectionString),
                DataType = DataType.VarChar,
                Length = 200
            },
            new TableColumnInfo
            {
                ColumnName = nameof(TableMatchInfo.TableName),
                DataType = DataType.VarChar,
                Length = 200
            },
            new TableColumnInfo
            {
                ColumnName = nameof(TableMatchInfo.ConnectionStringToMatch),
                DataType = DataType.VarChar,
                Length = 200
            },
            new TableColumnInfo
            {
                ColumnName = nameof(TableMatchInfo.TableNameToMatch),
                DataType = DataType.VarChar,
                Length = 200
            },
            new TableColumnInfo
            {
                ColumnName = nameof(TableMatchInfo.ColumnsMap),
                DataType = DataType.Text
            }
        };

        private const string SqlSelectTableMatch = "SELECT Id, ConnectionString, TableName, ConnectionStringToMatch, TableNameToMatch, ColumnsMap FROM siteserver_TableMatch WHERE Id = @Id";

		private const string SqlUpdateTableMatch = "UPDATE siteserver_TableMatch SET ConnectionString = @ConnectionString, TableName = @TableName, ConnectionStringToMatch = @ConnectionStringToMatch, TableNameToMatch = @TableNameToMatch, ColumnsMap = @ColumnsMap WHERE Id = @Id";

		private const string SqlDeleteTableMatch = "DELETE FROM siteserver_TableMatch WHERE Id = @Id";

		private const string ParmId = "@Id";
		private const string ParmConnectionString = "@ConnectionString";
		private const string ParmTableName = "@TableName";
		private const string ParmConnectionStringToMatch = "@ConnectionStringToMatch";
		private const string ParmTableNameToMatch = "@TableNameToMatch";
		private const string ParmColumnsMap = "@ColumnsMap";		

		public int Insert(TableMatchInfo tableMatchInfo)
		{
            const string sqlString = "INSERT INTO siteserver_TableMatch (ConnectionString, TableName, ConnectionStringToMatch, TableNameToMatch, ColumnsMap) VALUES (@ConnectionString, @TableName, @ConnectionStringToMatch, @TableNameToMatch, @ColumnsMap)";

			var insertParms = new IDataParameter[]
			{
				GetParameter(ParmConnectionString, DataType.VarChar, 200, tableMatchInfo.ConnectionString),
				GetParameter(ParmTableName, DataType.VarChar, 200, tableMatchInfo.TableName),
				GetParameter(ParmConnectionStringToMatch, DataType.VarChar, 200, tableMatchInfo.ConnectionStringToMatch),
				GetParameter(ParmTableNameToMatch, DataType.VarChar, 200, tableMatchInfo.TableNameToMatch),
				GetParameter(ParmColumnsMap, DataType.Text, TranslateUtils.NameValueCollectionToString(tableMatchInfo.ColumnsMap))
			};

            return ExecuteNonQueryAndReturnId(TableName, nameof(TableMatchInfo.Id), sqlString, insertParms);
		}

		public void Update(TableMatchInfo tableMatchInfo)
		{
			var updateParms = new IDataParameter[]
			{
				GetParameter(ParmConnectionString, DataType.VarChar, 200, tableMatchInfo.ConnectionString),
				GetParameter(ParmTableName, DataType.VarChar, 200, tableMatchInfo.TableName),
				GetParameter(ParmConnectionStringToMatch, DataType.VarChar, 200, tableMatchInfo.ConnectionStringToMatch),
				GetParameter(ParmTableNameToMatch, DataType.VarChar, 200, tableMatchInfo.TableNameToMatch),
				GetParameter(ParmColumnsMap, DataType.Text, TranslateUtils.NameValueCollectionToString(tableMatchInfo.ColumnsMap)),
				GetParameter(ParmId, DataType.Integer, tableMatchInfo.Id)
			};

			using (var conn = GetConnection()) 
			{
				conn.Open();
			    ExecuteNonQuery(conn, SqlUpdateTableMatch, updateParms);
			}
		}

		public void Delete(int id)
		{
			var parms = new IDataParameter[]
			{
				GetParameter(ParmId, DataType.Integer, id)
			};
							
			using (var conn = GetConnection()) 
			{
				conn.Open();
			    ExecuteNonQuery(conn, SqlDeleteTableMatch, parms);
			}
		}

		public TableMatchInfo GetTableMatchInfo(int id)
		{
			TableMatchInfo tableMatchInfo = null;

			var parms = new IDataParameter[]
			{
				GetParameter(ParmId, DataType.Integer, id)
			};

			using (var rdr = ExecuteReader(SqlSelectTableMatch, parms))
			{
				if (rdr.Read())
				{
				    var i = 0;
                    tableMatchInfo = new TableMatchInfo(GetInt(rdr, i++), GetString(rdr, i++), GetString(rdr, i++), GetString(rdr, i++), GetString(rdr, i++), TranslateUtils.ToNameValueCollection(GetString(rdr, i)));
				}
				rdr.Close();
			}

			return tableMatchInfo;
		}
	}
}
