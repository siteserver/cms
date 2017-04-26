using System.Data;
using BaiRong.Core.Data;
using BaiRong.Core.Model;
using BaiRong.Core.Model.Enumerations;

namespace BaiRong.Core.Provider
{
	public class TableMatchDao : DataProviderBase
	{
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
			int tableMatchId;

            var sqlString = "INSERT INTO bairong_TableMatch (ConnectionString, TableName, ConnectionStringToMatch, TableNameToMatch, ColumnsMap) VALUES (@ConnectionString, @TableName, @ConnectionStringToMatch, @TableNameToMatch, @ColumnsMap)";

			var insertParms = new IDataParameter[]
			{
				GetParameter(ParmConnectionString, EDataType.VarChar, 200, tableMatchInfo.ConnectionString),
				GetParameter(ParmTableName, EDataType.VarChar, 200, tableMatchInfo.TableName),
				GetParameter(ParmConnectionStringToMatch, EDataType.VarChar, 200, tableMatchInfo.ConnectionStringToMatch),
				GetParameter(ParmTableNameToMatch, EDataType.VarChar, 200, tableMatchInfo.TableNameToMatch),
				GetParameter(ParmColumnsMap, EDataType.NText, TranslateUtils.NameValueCollectionToString(tableMatchInfo.ColumnsMap))
			};

			using (var conn = GetConnection()) 
			{
				conn.Open();
				using (var trans = conn.BeginTransaction()) 
				{
					try 
					{
                        tableMatchId = ExecuteNonQueryAndReturnId(trans, sqlString, insertParms);

						trans.Commit();
					}
					catch
					{
						trans.Rollback();
						throw;
					}
				}
			}

			return tableMatchId;
		}

		public void Update(TableMatchInfo tableMatchInfo)
		{
			var updateParms = new IDataParameter[]
			{
				GetParameter(ParmConnectionString, EDataType.VarChar, 200, tableMatchInfo.ConnectionString),
				GetParameter(ParmTableName, EDataType.VarChar, 200, tableMatchInfo.TableName),
				GetParameter(ParmConnectionStringToMatch, EDataType.VarChar, 200, tableMatchInfo.ConnectionStringToMatch),
				GetParameter(ParmTableNameToMatch, EDataType.VarChar, 200, tableMatchInfo.TableNameToMatch),
				GetParameter(ParmColumnsMap, EDataType.NText, TranslateUtils.NameValueCollectionToString(tableMatchInfo.ColumnsMap)),
				GetParameter(ParmTableMatchId, EDataType.Integer, tableMatchInfo.TableMatchId)
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
				GetParameter(ParmTableMatchId, EDataType.Integer, tableMatchId)
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
				GetParameter(ParmTableMatchId, EDataType.Integer, tableMatchId)
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
