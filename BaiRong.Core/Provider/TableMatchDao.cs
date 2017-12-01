using System.Collections.Generic;
using System.Data;
using BaiRong.Core.Data;
using BaiRong.Core.Model;
using SiteServer.Plugin.Models;

namespace BaiRong.Core.Provider
{
	public class TableMatchDao : DataProviderBase
	{
        public override string TableName => "bairong_TableMatch";

        public override List<TableColumnInfo> TableColumns => new List<TableColumnInfo>
        {
            new TableColumnInfo
            {
                ColumnName = nameof(TableMatchInfo.TableMatchId),
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

        private const string SqlSelectTableMatch = "SELECT TableMatchID, ConnectionString, TableName, ConnectionStringToMatch, TableNameToMatch, ColumnsMap FROM bairong_TableMatch WHERE TableMatchID = @TableMatchID";

		private const string SqlUpdateTableMatch = "UPDATE bairong_TableMatch SET ConnectionString = @ConnectionString, TableName = @TableName, ConnectionStringToMatch = @ConnectionStringToMatch, TableNameToMatch = @TableNameToMatch, ColumnsMap = @ColumnsMap WHERE TableMatchID = @TableMatchID";

		private const string SqlDeleteTableMatch = "DELETE FROM bairong_TableMatch WHERE TableMatchID = @TableMatchID";

		private const string ParmTableMatchId = "@TableMatchID";
		private const string ParmConnectionString = "@ConnectionString";
		private const string ParmTableName = "@TableName";
		private const string ParmConnectionStringToMatch = "@ConnectionStringToMatch";
		private const string ParmTableNameToMatch = "@TableNameToMatch";
		private const string ParmColumnsMap = "@ColumnsMap";		

		public int Insert(TableMatchInfo tableMatchInfo)
		{
            const string sqlString = "INSERT INTO bairong_TableMatch (ConnectionString, TableName, ConnectionStringToMatch, TableNameToMatch, ColumnsMap) VALUES (@ConnectionString, @TableName, @ConnectionStringToMatch, @TableNameToMatch, @ColumnsMap)";

			var insertParms = new IDataParameter[]
			{
				GetParameter(ParmConnectionString, DataType.VarChar, 200, tableMatchInfo.ConnectionString),
				GetParameter(ParmTableName, DataType.VarChar, 200, tableMatchInfo.TableName),
				GetParameter(ParmConnectionStringToMatch, DataType.VarChar, 200, tableMatchInfo.ConnectionStringToMatch),
				GetParameter(ParmTableNameToMatch, DataType.VarChar, 200, tableMatchInfo.TableNameToMatch),
				GetParameter(ParmColumnsMap, DataType.Text, TranslateUtils.NameValueCollectionToString(tableMatchInfo.ColumnsMap))
			};

            return ExecuteNonQueryAndReturnId(TableName, nameof(TableMatchInfo.TableMatchId), sqlString, insertParms);
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
				GetParameter(ParmTableMatchId, DataType.Integer, tableMatchInfo.TableMatchId)
			};

			using (var conn = GetConnection()) 
			{
				conn.Open();
			    ExecuteNonQuery(conn, SqlUpdateTableMatch, updateParms);
			}
		}

		public void Delete(int tableMatchId)
		{
			var parms = new IDataParameter[]
			{
				GetParameter(ParmTableMatchId, DataType.Integer, tableMatchId)
			};
							
			using (var conn = GetConnection()) 
			{
				conn.Open();
			    ExecuteNonQuery(conn, SqlDeleteTableMatch, parms);
			}
		}

		public TableMatchInfo GetTableMatchInfo(int tableMatchId)
		{
			TableMatchInfo tableMatchInfo = null;

			var parms = new IDataParameter[]
			{
				GetParameter(ParmTableMatchId, DataType.Integer, tableMatchId)
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
