using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Text;
using BaiRong.Core.AuxiliaryTable;
using BaiRong.Core.Data;
using BaiRong.Core.Model;
using BaiRong.Core.Model.Attributes;
using BaiRong.Core.Model.Enumerations;
using SiteServer.Plugin.Models;

namespace BaiRong.Core.Provider
{
    public class TableMetadataDao : DataProviderBase
    {
        private const string SqlSelectTableMetadata = "SELECT TableMetadataID, AuxiliaryTableENName, AttributeName, DataType, DataLength, Taxis, IsSystem FROM bairong_TableMetadata WHERE TableMetadataID = @TableMetadataID";

        private const string SqlSelectAllTableMetadataByEnname = "SELECT TableMetadataID, AuxiliaryTableENName, AttributeName, DataType, DataLength, Taxis, IsSystem FROM bairong_TableMetadata WHERE AuxiliaryTableENName = @AuxiliaryTableENName ORDER BY IsSystem DESC, Taxis";

        private const string SqlSelectTableMetadataCountByEnname = "SELECT COUNT(*) FROM bairong_TableMetadata WHERE AuxiliaryTableENName = @AuxiliaryTableENName";

        private const string SqlSelectTableMetadataByTableEnnameAndAttributeName = "SELECT TableMetadataID, AuxiliaryTableENName, AttributeName, DataType, DataLength, Taxis, IsSystem FROM bairong_TableMetadata WHERE AuxiliaryTableENName = @AuxiliaryTableENName AND AttributeName = @AttributeName";

        private const string SqlSelectTableMetadataIdByTableEnnameAndAttributeName = "SELECT TableMetadataID FROM bairong_TableMetadata WHERE AuxiliaryTableENName = @AuxiliaryTableENName AND AttributeName = @AttributeName";

        private const string SqlSelectTableMetadataAllAttributeName = "SELECT AttributeName FROM bairong_TableMetadata WHERE AuxiliaryTableENName = @AuxiliaryTableENName ORDER BY Taxis";

        private const string SqlUpdateTableMetadata = "UPDATE bairong_TableMetadata SET AuxiliaryTableENName = @AuxiliaryTableENName, AttributeName = @AttributeName, DataType = @DataType, DataLength = @DataLength, IsSystem = @IsSystem WHERE  TableMetadataID = @TableMetadataID";

        private const string SqlDeleteTableMetadata = "DELETE FROM bairong_TableMetadata WHERE  TableMetadataID = @TableMetadataID";

        private const string SqlDeleteTableMetadataByTableName = "DELETE FROM bairong_TableMetadata WHERE  AuxiliaryTableENName = @AuxiliaryTableENName";

        private const string SqlUpdateTableMetadataTaxis = "UPDATE bairong_TableMetadata SET Taxis = @Taxis WHERE  TableMetadataID = @TableMetadataID";

        private const string ParmTableMetadataId = "@TableMetadataID";
        private const string ParmAuxiliaryTableEnname = "@AuxiliaryTableENName";
        private const string ParmAttributeName = "@AttributeName";
        private const string ParmDataType = "@DataType";
        private const string ParmDataLength = "@DataLength";
        private const string ParmTaxis = "@Taxis";
        private const string ParmIsSystem = "@IsSystem";

        public void Insert(TableMetadataInfo info)
        {
            var sqlString = "INSERT INTO bairong_TableMetadata (AuxiliaryTableENName, AttributeName, DataType, DataLength, Taxis, IsSystem) VALUES (@AuxiliaryTableENName, @AttributeName, @DataType, @DataLength, @Taxis, @IsSystem)";

            var insertParms = new IDataParameter[]
			{
				GetParameter(ParmAuxiliaryTableEnname, DataType.VarChar, 50, info.AuxiliaryTableEnName),
				GetParameter(ParmAttributeName, DataType.VarChar, 50, info.AttributeName),
				GetParameter(ParmDataType, DataType.VarChar, 50, DataTypeUtils.GetValue(info.DataType)),
				GetParameter(ParmDataLength, DataType.Integer, info.DataLength),
				GetParameter(ParmTaxis, DataType.Integer, GetMaxTaxis(info.AuxiliaryTableEnName) + 1),
				GetParameter(ParmIsSystem, DataType.VarChar, 18, info.IsSystem.ToString())
			};

            using (var conn = GetConnection())
            {
                conn.Open();
                ExecuteNonQuery(conn, sqlString, insertParms);

                BaiRongDataProvider.TableCollectionDao.UpdateAttributeNum(info.AuxiliaryTableEnName);
                BaiRongDataProvider.TableCollectionDao.UpdateIsChangedAfterCreatedInDb(true, info.AuxiliaryTableEnName);
                TableManager.IsChanged = true;
            }
        }


        internal void InsertWithTransaction(TableMetadataInfo info, EAuxiliaryTableType tableType, int taxis, IDbTransaction trans)
        {
            var sqlString = "INSERT INTO bairong_TableMetadata (AuxiliaryTableENName, AttributeName, DataType, DataLength, Taxis, IsSystem) VALUES (@AuxiliaryTableENName, @AttributeName, @DataType, @DataLength, @Taxis, @IsSystem)";

            var insertParms = new IDataParameter[]
		    {
			    GetParameter(ParmAuxiliaryTableEnname, DataType.VarChar, 50, info.AuxiliaryTableEnName),
			    GetParameter(ParmAttributeName, DataType.VarChar, 50, info.AttributeName),
			    GetParameter(ParmDataType, DataType.VarChar, 50, DataTypeUtils.GetValue(info.DataType)),
			    GetParameter(ParmDataLength, DataType.Integer, info.DataLength),
			    GetParameter(ParmTaxis, DataType.Integer, taxis),
			    GetParameter(ParmIsSystem, DataType.VarChar, 18, info.IsSystem.ToString())
		    };

            ExecuteNonQuery(trans, sqlString, insertParms);
            if (info.StyleInfo != null)
            {
                info.StyleInfo.TableName = info.AuxiliaryTableEnName;
                info.StyleInfo.AttributeName = info.AttributeName;
                BaiRongDataProvider.TableStyleDao.InsertWithTransaction(info.StyleInfo, EAuxiliaryTableTypeUtils.GetTableStyle(tableType), trans);
                TableStyleManager.IsChanged = true;
            }
            TableManager.IsChanged = true;
        }

        public void InsertSystemItems(string tableEnName, EAuxiliaryTableType tableType, IDbTransaction trans)
        {
            var list = GetDefaultTableMetadataInfoList(tableEnName, tableType);
            if (list != null && list.Count > 0)
            {
                var taxis = 1;
                foreach (var info in list)
                {
                    InsertWithTransaction(info, tableType, taxis++, trans);
                }
            }
        }


        public void Update(TableMetadataInfo info)
        {
            var isSqlChanged = true;
            var originalInfo = GetTableMetadataInfo(info.TableMetadataId);
            if (originalInfo != null)
            {
                if (info.AuxiliaryTableEnName == originalInfo.AuxiliaryTableEnName
                 && info.AttributeName == originalInfo.AttributeName
                 && info.DataType == originalInfo.DataType
                 && info.DataLength == originalInfo.DataLength
                 && info.Taxis == originalInfo.Taxis)
                {
                    isSqlChanged = false;
                }
            }

            var updateParms = new IDataParameter[]
			{
				GetParameter(ParmAuxiliaryTableEnname, DataType.VarChar, 50, info.AuxiliaryTableEnName),
				GetParameter(ParmAttributeName, DataType.VarChar, 50, info.AttributeName),
				GetParameter(ParmDataType, DataType.VarChar, 50, DataTypeUtils.GetValue(info.DataType)),
				GetParameter(ParmDataLength, DataType.Integer, info.DataLength),
				GetParameter(ParmIsSystem, DataType.VarChar, 18, info.IsSystem.ToString()),
				GetParameter(ParmTableMetadataId, DataType.Integer, info.TableMetadataId)
			};

            using (var conn = GetConnection())
            {
                conn.Open();
                ExecuteNonQuery(conn, SqlUpdateTableMetadata, updateParms);
                if (isSqlChanged)
                {
                    BaiRongDataProvider.TableCollectionDao.UpdateIsChangedAfterCreatedInDb(true, info.AuxiliaryTableEnName);
                }
                TableManager.IsChanged = true;
            }
        }

        public void Delete(int tableMetadataId)
        {
            var parms = new IDataParameter[]
			{
				GetParameter(ParmTableMetadataId, DataType.Integer, tableMetadataId)
			};

            var metadataInfo = GetTableMetadataInfo(tableMetadataId);

            using (var conn = GetConnection())
            {
                conn.Open();
                ExecuteNonQuery(conn, SqlDeleteTableMetadata, parms);

                BaiRongDataProvider.TableCollectionDao.UpdateAttributeNum(metadataInfo.AuxiliaryTableEnName);
                BaiRongDataProvider.TableCollectionDao.UpdateIsChangedAfterCreatedInDb(true, metadataInfo.AuxiliaryTableEnName);
                TableManager.IsChanged = true;
            }
        }

        public void Delete(string tableEnName)
        {
            Delete(tableEnName, null);
        }

        public void Delete(string tableEnName, IDbTransaction trans)
        {
            var parms = new IDataParameter[]
			{
				GetParameter(ParmAuxiliaryTableEnname, DataType.VarChar,50, tableEnName)
			};
            if (trans == null)
            {
                using (var conn = GetConnection())
                {
                    conn.Open();
                    ExecuteNonQuery(conn, SqlDeleteTableMetadataByTableName, parms);
                    TableManager.IsChanged = true;
                }
            }
            else
            {
                ExecuteNonQuery(trans, SqlDeleteTableMetadataByTableName, parms);
                TableManager.IsChanged = true;
            }
        }

        public TableMetadataInfo GetTableMetadataInfo(int tableMetadataId)
        {
            TableMetadataInfo info = null;

            var parms = new IDataParameter[]
			{
				GetParameter(ParmTableMetadataId, DataType.Integer, tableMetadataId)
			};

            using (var rdr = ExecuteReader(SqlSelectTableMetadata, parms))
            {
                if (rdr.Read())
                {
                    var i = 0;
                    info = new TableMetadataInfo(GetInt(rdr, i++), GetString(rdr, i++), GetString(rdr, i++), DataTypeUtils.GetEnumType(GetString(rdr, i++)), GetInt(rdr, i++), GetInt(rdr, i++), GetBool(rdr, i));
                }
                rdr.Close();
            }

            return info;
        }

        public TableMetadataInfo GetTableMetadataInfo(string tableEnName, string attributeName)
        {
            TableMetadataInfo info = null;

            var parms = new IDataParameter[]
			{
				GetParameter(ParmAuxiliaryTableEnname, DataType.VarChar, 50, tableEnName),
				GetParameter(ParmAttributeName, DataType.VarChar, 50, attributeName)
			};

            using (var rdr = ExecuteReader(SqlSelectTableMetadataByTableEnnameAndAttributeName, parms))
            {
                if (rdr.Read())
                {
                    var i = 0;
                    info = new TableMetadataInfo(GetInt(rdr, i++), GetString(rdr, i++), GetString(rdr, i++), DataTypeUtils.GetEnumType(GetString(rdr, i++)), GetInt(rdr, i++), GetInt(rdr, i++), GetBool(rdr, i));
                }
                rdr.Close();
            }

            return info;
        }

        public int GetTableMetadataId(string tableEnName, string attributeName)
        {
            var tableMetadataId = 0;

            var parms = new IDataParameter[]
			{
				GetParameter(ParmAuxiliaryTableEnname, DataType.VarChar, 50, tableEnName),
				GetParameter(ParmAttributeName, DataType.VarChar, 50, attributeName)
			};

            using (var conn = GetConnection())
            {
                conn.Open();
                using (var rdr = ExecuteReader(conn, SqlSelectTableMetadataIdByTableEnnameAndAttributeName, parms))
                {
                    if (rdr.Read())
                    {
                        tableMetadataId = GetInt(rdr, 0);
                    }
                    rdr.Close();
                }
            }

            return tableMetadataId;
        }

        public IEnumerable GetDataSource(string tableEnName)
        {
            var parms = new IDataParameter[]
			{
				GetParameter(ParmAuxiliaryTableEnname, DataType.VarChar, 50, tableEnName)
			};

            var enumerable = (IEnumerable)ExecuteReader(SqlSelectAllTableMetadataByEnname, parms);
            return enumerable;
        }

        public IEnumerable GetDataSorceMinusAttributes(string tableEnName, List<string> attributeNameList)
        {
            string parameterNameList;
            var parameterList = GetInParameterList(ParmAttributeName, DataType.VarChar, 50, attributeNameList, out parameterNameList);
            string sqlString =
                $"SELECT TableMetadataID, AuxiliaryTableENName, AttributeName, DataType, DataLength, DataScale, Taxis, IsSystem FROM bairong_TableMetadata WHERE AuxiliaryTableENName = @AuxiliaryTableENName AND AttributeName NOT IN ({parameterNameList}) ORDER BY Taxis";

            var paramList = new List<IDataParameter>
            {
                GetParameter(ParmAuxiliaryTableEnname, DataType.VarChar, 50, tableEnName)
            };
            paramList.AddRange(parameterList);

            var enumerable = (IEnumerable)ExecuteReader(sqlString, paramList.ToArray());
            return enumerable;
        }

        public List<TableMetadataInfo> GetTableMetadataInfoList(string tableEnName)
        {
            var list = new List<TableMetadataInfo>();

            var parms = new IDataParameter[]
			{
				GetParameter(ParmAuxiliaryTableEnname, DataType.VarChar, 50, tableEnName)
			};

            using (var rdr = ExecuteReader(SqlSelectAllTableMetadataByEnname, parms))
            {
                while (rdr.Read())
                {
                    var i = 0;
                    var info = new TableMetadataInfo(GetInt(rdr, i++), GetString(rdr, i++), GetString(rdr, i++), DataTypeUtils.GetEnumType(GetString(rdr, i++)), GetInt(rdr, i++), GetInt(rdr, i++), GetBool(rdr, i));
                    list.Add(info);
                }
                rdr.Close();
            }
            return list;
        }

        public Hashtable GetTableEnNameAndTableMetadataInfoListHashtable()
        {
            var hashtable = new Hashtable();
            var tableNameList = BaiRongDataProvider.TableCollectionDao.GetTableEnNameList();
            foreach (var tableName in tableNameList)
            {
                var list = GetTableMetadataInfoList(tableName);
                hashtable.Add(tableName, list);
            }
            return hashtable;
        }

        /// <summary>
        /// Get Total AuxiliaryTable Count
        /// </summary>
        public int GetTableMetadataCountByEnName(string tableEnName)
        {
            var count = 0;

            var parms = new IDataParameter[]
			{
				GetParameter(ParmAuxiliaryTableEnname, DataType.VarChar, 50, tableEnName)
			};

            using (var rdr = ExecuteReader(SqlSelectTableMetadataCountByEnname, parms))
            {
                if (rdr.Read())
                {
                    count = GetInt(rdr, 0);
                }
                rdr.Close();
            }

            return count;
        }

        public List<string> GetAttributeNameList(string tableEnName)
        {
            var parms = new IDataParameter[]
			{
				GetParameter(ParmAuxiliaryTableEnname, DataType.VarChar, 50, tableEnName)
			};

            var list = new List<string>();
            using (var rdr = ExecuteReader(SqlSelectTableMetadataAllAttributeName, parms))
            {
                while (rdr.Read())
                {
                    list.Add(GetString(rdr, 0));
                }
                rdr.Close();
            }

            return list;
        }

        /// <summary>
        /// Get max Taxis in the database
        /// </summary>
        public int GetMaxTaxis(string tableEnName)
        {
            const string sqlString = "SELECT MAX(Taxis) FROM bairong_TableMetadata WHERE AuxiliaryTableENName = @AuxiliaryTableENName";

            var parms = new IDataParameter[]
			{
				GetParameter(ParmAuxiliaryTableEnname, DataType.VarChar, 50, tableEnName)
			};

            return BaiRongDataProvider.DatabaseDao.GetIntResult(sqlString, parms);
        }


        /// <summary>
        /// Change The Texis To Higher Level
        /// </summary>
        public void TaxisUp(int selectedId, string tableEnName)
        {
            //Get Higher Taxis and ClassID
            //var sqlString = "SELECT TOP 1 TableMetadataID, Taxis FROM bairong_TableMetadata WHERE ((Taxis > (SELECT Taxis FROM bairong_TableMetadata WHERE (TableMetadataID = @TableMetadataID AND AuxiliaryTableENName = @AuxiliaryTableENName1))) AND AuxiliaryTableENName=@AuxiliaryTableENName2) ORDER BY Taxis";
            var sqlString = SqlUtils.GetTopSqlString("bairong_TableMetadata", "TableMetadataID, Taxis", "WHERE ((Taxis > (SELECT Taxis FROM bairong_TableMetadata WHERE (TableMetadataID = @TableMetadataID AND AuxiliaryTableENName = @AuxiliaryTableENName1))) AND AuxiliaryTableENName=@AuxiliaryTableENName2) ORDER BY Taxis", 1);
            var higherId = 0;
            var higherTaxis = 0;

            var parms = new IDataParameter[]
			{
				GetParameter(ParmTableMetadataId, DataType.Integer, selectedId),
				GetParameter("@AuxiliaryTableENName1", DataType.VarChar, 50, tableEnName),
				GetParameter("@AuxiliaryTableENName2", DataType.VarChar, 50, tableEnName)
			};

            using (var rdr = ExecuteReader(sqlString, parms))
            {
                if (rdr.Read())
                {
                    higherId = GetInt(rdr, 0);
                    higherTaxis = GetInt(rdr, 1);
                }
                rdr.Close();
            }

            //Get Taxis Of Selected Class
            var selectedTaxis = GetTaxis(selectedId);

            if (higherId != 0)
            {
                //Set The Selected Class Taxis To Higher Level
                SetTaxis(selectedId, higherTaxis);
                //Set The Higher Class Taxis To Lower Level
                SetTaxis(higherId, selectedTaxis);
                TableManager.IsChanged = true;
                //BaiRongDataProvider.CreateAuxiliaryTableDAO().UpdateIsChangedAfterCreatedInDB(EBoolean.True, tableENName);
            }

        }

        /// <summary>
        /// Change The Texis To Lower Level
        /// </summary>
        public void TaxisDown(int selectedId, string tableEnName)
        {
            //Get Lower Taxis and ClassID
            //var sqlString = "SELECT TOP 1 TableMetadataID, Taxis FROM bairong_TableMetadata WHERE ((Taxis < (SELECT Taxis FROM bairong_TableMetadata WHERE (TableMetadataID = @TableMetadataID AND AuxiliaryTableENName = @AuxiliaryTableENName1))) AND AuxiliaryTableENName = @AuxiliaryTableENName2) ORDER BY Taxis DESC";
            var sqlString = SqlUtils.GetTopSqlString("bairong_TableMetadata", "TableMetadataID, Taxis", "WHERE ((Taxis < (SELECT Taxis FROM bairong_TableMetadata WHERE (TableMetadataID = @TableMetadataID AND AuxiliaryTableENName = @AuxiliaryTableENName1))) AND AuxiliaryTableENName = @AuxiliaryTableENName2) ORDER BY Taxis DESC", 1);

            var lowerId = 0;
            var lowerTaxis = 0;

            var parms = new IDataParameter[]
			{
				GetParameter(ParmTableMetadataId, DataType.Integer, selectedId),
				GetParameter("@AuxiliaryTableENName1", DataType.VarChar, 50, tableEnName),
				GetParameter("@AuxiliaryTableENName2", DataType.VarChar, 50, tableEnName)
			};

            using (var rdr = ExecuteReader(sqlString, parms))
            {
                if (rdr.Read())
                {
                    lowerId = GetInt(rdr, 0);
                    lowerTaxis = GetInt(rdr, 1);
                }
                rdr.Close();
            }

            //Get Taxis Of Selected Class
            var selectedTaxis = GetTaxis(selectedId);

            if (lowerId != 0)
            {
                //Set The Selected Class Taxis To Lower Level
                SetTaxis(selectedId, lowerTaxis);
                //Set The Lower Class Taxis To Higher Level
                SetTaxis(lowerId, selectedTaxis);
                TableManager.IsChanged = true;
                //BaiRongDataProvider.CreateAuxiliaryTableDAO().UpdateIsChangedAfterCreatedInDB(EBoolean.True, tableENName);
            }
        }

        private int GetTaxis(int selectedId)
        {
            var cmd = "SELECT Taxis FROM bairong_TableMetadata WHERE (TableMetadataID = @TableMetadataID)";
            var taxis = 0;

            var parms = new IDataParameter[]
			{
				GetParameter(ParmTableMetadataId, DataType.Integer, selectedId)
			};

            using (var rdr = ExecuteReader(cmd, parms))
            {
                if (rdr.Read())
                {
                    taxis = GetInt(rdr, 0);
                }
                rdr.Close();
            }

            return taxis;
        }

        private void SetTaxis(int id, int taxis)
        {
            var parms = new IDataParameter[]
			{
				GetParameter(ParmTaxis, DataType.Integer, taxis),
				GetParameter(ParmTableMetadataId, DataType.Integer, id)
			};

            ExecuteNonQuery(SqlUpdateTableMetadataTaxis, parms);
            TableManager.IsChanged = true;
        }

        public void CreateAuxiliaryTable(string tableEnName)
        {
            var createTableSqlString = GetCreateAuxiliaryTableSqlString(tableEnName);

            var updateParms = new IDataParameter[]
			{
				GetParameter("@IsCreatedInDB", DataType.VarChar, 18, true.ToString()),
				GetParameter("@IsChangedAfterCreatedInDB", DataType.VarChar, 18, false.ToString()),
				GetParameter("@TableENName", DataType.VarChar, 50, tableEnName)
			};

            using (var conn = GetConnection())
            {
                conn.Open();
                using (var trans = conn.BeginTransaction())
                {
                    try
                    {
                        var reader = new System.IO.StringReader(createTableSqlString);
                        string sql;
                        while (null != (sql = SqlUtils.ReadNextStatementFromStream(reader)))
                        {
                            ExecuteNonQuery(trans, sql.Trim());
                        }

                        ExecuteNonQuery(trans, "UPDATE bairong_TableCollection SET IsCreatedInDB = @IsCreatedInDB, IsChangedAfterCreatedInDB = @IsChangedAfterCreatedInDB WHERE TableENName = @TableENName", updateParms);
                        TableManager.IsChanged = true;
                        trans.Commit();
                    }
                    catch
                    {
                        trans.Rollback();
                        throw;
                    }
                }
            }
        }

        public void CreateAuxiliaryTableOfArchive(string tableEnName)
        {
            var createTableSqlString = GetCreateAuxiliaryTableSqlString(tableEnName);

            var archiveTableName = TableManager.GetTableNameOfArchive(tableEnName);

            createTableSqlString = createTableSqlString.Replace(tableEnName, archiveTableName);

            using (var conn = GetConnection())
            {
                conn.Open();
                using (var trans = conn.BeginTransaction())
                {
                    try
                    {
                        var reader = new System.IO.StringReader(createTableSqlString);
                        string sql;

                        while (null != (sql = SqlUtils.ReadNextStatementFromStream(reader)))
                        {
                            ExecuteNonQuery(trans, sql.Trim());
                        }

                        trans.Commit();
                    }
                    catch
                    {
                        trans.Rollback();
                        throw;
                    }
                }
            }
        }

        public void DeleteAuxiliaryTable(string tableEnName)
        {
            if (BaiRongDataProvider.DatabaseDao.IsTableExists(tableEnName))
            {
                string dropTableSqlString = $"DROP TABLE [{tableEnName}]";

                var updateParms = new IDataParameter[]
				{
					GetParameter("@IsCreatedInDB", DataType.VarChar, 18, false.ToString()),
					GetParameter("@IsChangedAfterCreatedInDB", DataType.VarChar, 18, false.ToString()),
					GetParameter("@TableENName", DataType.VarChar, 50, tableEnName)
				};

                using (var conn = GetConnection())
                {
                    conn.Open();
                    using (var trans = conn.BeginTransaction())
                    {
                        try
                        {
                            ExecuteNonQuery(trans, dropTableSqlString);
                            ExecuteNonQuery(trans, "UPDATE bairong_TableCollection SET IsCreatedInDB = @IsCreatedInDB, IsChangedAfterCreatedInDB = @IsChangedAfterCreatedInDB WHERE TableENName = @TableENName", updateParms);
                            TableManager.IsChanged = true;
                            trans.Commit();
                        }
                        catch
                        {
                            trans.Rollback();
                            throw;
                        }
                    }
                }
            }
        }


        public void ReCreateAuxiliaryTable(string tableEnName, EAuxiliaryTableType tableType)
        {
            var defaultTableMetadataInfoList = GetDefaultTableMetadataInfoList(tableEnName, tableType);

            if (BaiRongDataProvider.DatabaseDao.IsTableExists(tableEnName))
            {
                var updateParms = new IDataParameter[]
				{
					GetParameter("@IsCreatedInDB", DataType.VarChar, 18, true.ToString()),
					GetParameter("@IsChangedAfterCreatedInDB", DataType.VarChar, 18, false.ToString()),
					GetParameter("@TableENName", DataType.VarChar, 50, tableEnName)
				};

                var taxis = GetMaxTaxis(tableEnName) + 1;

                using (var conn = GetConnection())
                {
                    conn.Open();
                    using (var trans = conn.BeginTransaction())
                    {
                        try
                        {
                            foreach (var tableMetadataInfo in defaultTableMetadataInfoList)
                            {
                                if (GetTableMetadataId(tableEnName, tableMetadataInfo.AttributeName) == 0)
                                {
                                    InsertWithTransaction(tableMetadataInfo, tableType, taxis++, trans);
                                }
                            }

                            string dropTableSqlString = $"DROP TABLE [{tableEnName}]";
                            var createTableSqlString = GetCreateAuxiliaryTableSqlString(tableEnName);

                            ExecuteNonQuery(trans, dropTableSqlString);

                            var reader = new System.IO.StringReader(createTableSqlString);
                            string sql;
                            while (null != (sql = SqlUtils.ReadNextStatementFromStream(reader)))
                            {
                                ExecuteNonQuery(trans, sql.Trim());
                            }

                            ExecuteNonQuery(trans, "UPDATE bairong_TableCollection SET IsCreatedInDB = @IsCreatedInDB, IsChangedAfterCreatedInDB = @IsChangedAfterCreatedInDB WHERE  TableENName = @TableENName", updateParms);
                            TableManager.IsChanged = true;
                            SqlUtils.Cache_RemoveTableColumnInfoListCache();
                            trans.Commit();
                        }
                        catch
                        {
                            trans.Rollback();
                            throw;
                        }
                    }
                }
            }
        }

        protected const string ErrorCommandMessage = "此辅助表无字段，无法在数据库中生成表！";

        protected List<string> GetAlterDropColumnSqls(string tableEnName, string attributeName)
        {
            var sqlList = new List<string>();
            if (WebConfigUtils.DatabaseType != EDatabaseType.MySql)
            {
                var defaultConstraintName = BaiRongDataProvider.DatabaseDao.GetDefaultConstraintName(tableEnName, attributeName);
                if (!string.IsNullOrEmpty(defaultConstraintName))
                {
                    sqlList.Add($"ALTER TABLE [{tableEnName}] DROP CONSTRAINT [{defaultConstraintName}]");
                }
            }
            sqlList.Add($"ALTER TABLE [{tableEnName}] DROP COLUMN [{attributeName}]");
            return sqlList;
        }

        protected List<string> GetAlterAddColumnSqls(string tableEnName, TableMetadataInfo metadataInfo)
        {
            var sqlList = new List<string>();
            var columnSqlString = SqlUtils.GetColumnSqlString(metadataInfo.DataType, metadataInfo.AttributeName, metadataInfo.DataLength);
            var alterSqlString = SqlUtils.GetAddColumnsSqlString(tableEnName, columnSqlString);
            sqlList.Add(alterSqlString);
            return sqlList;
        }

        public void SyncTable(string tableEnName)
        {
            var list = GetTableMetadataInfoList(tableEnName);
            var databaseName = SqlUtils.GetDatabaseNameFormConnectionString(WebConfigUtils.ConnectionString);
            var tableId = BaiRongDataProvider.DatabaseDao.GetTableId(WebConfigUtils.ConnectionString, databaseName, tableEnName);
            var columnlist = BaiRongDataProvider.DatabaseDao.GetTableColumnInfoList(WebConfigUtils.ConnectionString, databaseName, tableEnName, tableId);

            var sqlList = new List<string>();

            //添加新增/修改字段SQL语句
            foreach (var metadataInfo in list)
            {
                if (metadataInfo.IsSystem) continue;
                var columnExists = false;
                foreach (var columnInfo in columnlist)
                {
                    if (StringUtils.EqualsIgnoreCase(columnInfo.ColumnName, metadataInfo.AttributeName))
                    {
                        columnExists = true;
                        if (!BaiRongDataProvider.DatabaseDao.IsColumnEquals(metadataInfo, columnInfo))
                        {
                            var alterSqllist = GetAlterDropColumnSqls(tableEnName, columnInfo.ColumnName);
                            foreach (var sql in alterSqllist)
                            {
                                sqlList.Add(sql);
                            }
                            alterSqllist = GetAlterAddColumnSqls(tableEnName, metadataInfo);
                            foreach (var sql in alterSqllist)
                            {
                                sqlList.Add(sql);
                            }
                        }
                        break;
                    }
                }
                if (!columnExists)
                {
                    var alterSqlList = GetAlterAddColumnSqls(tableEnName, metadataInfo);
                    foreach (var sql in alterSqlList)
                    {
                        sqlList.Add(sql);
                    }
                }
            }

            //添加删除字段SQL语句
            var tableType = BaiRongDataProvider.TableCollectionDao.GetTableType(tableEnName);
            var hiddenAttributeNameList = TableManager.GetHiddenAttributeNameList(EAuxiliaryTableTypeUtils.GetTableStyle(tableType));
            foreach (var columnInfo in columnlist)
            {
                if (hiddenAttributeNameList.Contains(columnInfo.ColumnName.ToLower())) continue;
                var isNeedDelete = true;
                foreach (var metadataInfo in list)
                {
                    if (StringUtils.EqualsIgnoreCase(columnInfo.ColumnName, metadataInfo.AttributeName))
                    {
                        isNeedDelete = false;
                        break;
                    }
                }
                if (isNeedDelete)
                {
                    var alterSqlList = GetAlterDropColumnSqls(tableEnName, columnInfo.ColumnName);
                    foreach (var sql in alterSqlList)
                    {
                        sqlList.Add(sql);
                    }
                }
            }
            BaiRongDataProvider.DatabaseDao.ExecuteSql(sqlList);
            BaiRongDataProvider.TableCollectionDao.UpdateIsChangedAfterCreatedInDb(false, tableEnName);
        }

        public List<TableMetadataInfo> GetDefaultTableMetadataInfoList(string tableName, EAuxiliaryTableType tableType)
        {
            var list = new List<TableMetadataInfo>();
            if (tableType != EAuxiliaryTableType.BackgroundContent) return list;

            var metadataInfo = new TableMetadataInfo(0, tableName, BackgroundContentAttribute.SubTitle, DataType.NVarChar, 255, 0, true);
            list.Add(metadataInfo);
            metadataInfo = new TableMetadataInfo(0, tableName, BackgroundContentAttribute.ImageUrl, DataType.VarChar, 200, 0, true);
            list.Add(metadataInfo);
            metadataInfo = new TableMetadataInfo(0, tableName, BackgroundContentAttribute.VideoUrl, DataType.VarChar, 200, 0, true);
            list.Add(metadataInfo);
            metadataInfo = new TableMetadataInfo(0, tableName, BackgroundContentAttribute.FileUrl, DataType.VarChar, 200, 0, true);
            list.Add(metadataInfo);
            metadataInfo = new TableMetadataInfo(0, tableName, BackgroundContentAttribute.LinkUrl, DataType.NVarChar, 200, 0, true);
            list.Add(metadataInfo);
            metadataInfo = new TableMetadataInfo(0, tableName, BackgroundContentAttribute.Content, DataType.NText, 16, 0, true);
            list.Add(metadataInfo);
            metadataInfo = new TableMetadataInfo(0, tableName, BackgroundContentAttribute.Summary, DataType.NText, 16, 0, true);
            list.Add(metadataInfo);
            metadataInfo = new TableMetadataInfo(0, tableName, BackgroundContentAttribute.Author, DataType.NVarChar, 255, 0, true);
            list.Add(metadataInfo);
            metadataInfo = new TableMetadataInfo(0, tableName, BackgroundContentAttribute.Source, DataType.NVarChar, 255, 0, true);
            list.Add(metadataInfo);
            return list;
        }

        public string GetCreateAuxiliaryTableSqlString(string tableName)
        {
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

            var sqlBuilder = new StringBuilder();

            //添加默认字段
            if (WebConfigUtils.DatabaseType == EDatabaseType.MySql)
            {
                sqlBuilder.Append($@"CREATE TABLE `{tableName}` (");
                sqlBuilder.Append($@"
`{nameof(ContentInfo.Id)}` INT AUTO_INCREMENT,
`{nameof(ContentInfo.NodeId)}` INT,
`{nameof(ContentInfo.PublishmentSystemId)}` INT,
`{nameof(ContentInfo.AddUserName)}` VARCHAR(255),
`{nameof(ContentInfo.LastEditUserName)}` VARCHAR(255),
`{nameof(ContentInfo.WritingUserName)}` VARCHAR(255),
`{nameof(ContentInfo.LastEditDate)}` DATETIME,
`{nameof(ContentInfo.Taxis)}` INT,
`{nameof(ContentInfo.ContentGroupNameCollection)}` VARCHAR(255),
`{nameof(ContentInfo.Tags)}` VARCHAR(255),
`{nameof(ContentInfo.SourceId)}` INT,
`{nameof(ContentInfo.ReferenceId)}` INT,
`{nameof(ContentInfo.IsChecked)}` VARCHAR(18),
`{nameof(ContentInfo.CheckedLevel)}` INT,
`{nameof(ContentInfo.Comments)}` INT,
`{nameof(ContentInfo.Photos)}` INT,
`{nameof(ContentInfo.Hits)}` INT,
`{nameof(ContentInfo.HitsByDay)}` INT,
`{nameof(ContentInfo.HitsByWeek)}` INT,
`{nameof(ContentInfo.HitsByMonth)}` INT,
`{nameof(ContentInfo.LastHitsDate)}` DATETIME,
`{nameof(ContentInfo.SettingsXml)}` LONGTEXT,
`{nameof(ContentInfo.Title)}` VARCHAR(255),
`{nameof(ContentInfo.IsTop)}` VARCHAR(18),
`{nameof(ContentInfo.IsRecommend)}` VARCHAR(18),
`{nameof(ContentInfo.IsHot)}` VARCHAR(18),
`{nameof(ContentInfo.IsColor)}` VARCHAR(18),
`{nameof(ContentInfo.AddDate)}` DATETIME,
");
            }
            else
            {
                sqlBuilder.Append($@"CREATE TABLE {tableName} (");
                sqlBuilder.Append($@"
{nameof(ContentInfo.Id)} int IDENTITY (1, 1),
{nameof(ContentInfo.NodeId)} int NULL,
{nameof(ContentInfo.PublishmentSystemId)} int NULL,
{nameof(ContentInfo.AddUserName)} nvarchar (255) NULL,
{nameof(ContentInfo.LastEditUserName)} nvarchar (255) NULL,
{nameof(ContentInfo.WritingUserName)} nvarchar (255) NULL,
{nameof(ContentInfo.LastEditDate)} datetime NULL,
{nameof(ContentInfo.Taxis)} int NULL,
{nameof(ContentInfo.ContentGroupNameCollection)} nvarchar (255) NULL,
{nameof(ContentInfo.Tags)} nvarchar (255) NULL,
{nameof(ContentInfo.SourceId)} int NULL,
{nameof(ContentInfo.ReferenceId)} int NULL,
{nameof(ContentInfo.IsChecked)} varchar (18) NULL,
{nameof(ContentInfo.CheckedLevel)} int NULL,
{nameof(ContentInfo.Comments)} int NULL,
{nameof(ContentInfo.Photos)} int NULL,
{nameof(ContentInfo.Hits)} int NULL,
{nameof(ContentInfo.HitsByDay)} int NULL,
{nameof(ContentInfo.HitsByWeek)} int NULL,
{nameof(ContentInfo.HitsByMonth)} int NULL,
{nameof(ContentInfo.LastHitsDate)} datetime NULL,
{nameof(ContentInfo.SettingsXml)} ntext NULL,
{nameof(ContentInfo.Title)} nvarchar (255) NULL,
{nameof(ContentInfo.IsTop)} varchar (18) NULL,
{nameof(ContentInfo.IsRecommend)} varchar (18) NULL,
{nameof(ContentInfo.IsHot)} varchar (18) NULL,
{nameof(ContentInfo.IsColor)} varchar (18) NULL,
{nameof(ContentInfo.AddDate)} datetime NULL,
");
            }

            //添加后台定义字段
            foreach (var sqlString in columnSqlStringList)
            {
                sqlBuilder.Append(sqlString).Append(@",");
            }

            if (WebConfigUtils.DatabaseType == EDatabaseType.MySql)
            {
                sqlBuilder.Append($@"
PRIMARY KEY ({nameof(ContentInfo.Id)})
)
GO
CREATE INDEX `IX_{tableName}` ON `{tableName}`(`{nameof(ContentInfo.IsTop)}` DESC, `{nameof(ContentInfo.Taxis)}` DESC, `{nameof(ContentInfo.Id)}` DESC)
GO
CREATE INDEX `IX_{tableName}_Taxis` ON `{tableName}`(`{nameof(ContentInfo.Taxis)}` DESC)
GO");
            }
            else
            {
                sqlBuilder.Append($@"
CONSTRAINT PK_{tableName} PRIMARY KEY ({nameof(ContentInfo.Id)})
)
GO
CREATE INDEX IX_{tableName} ON {tableName}({nameof(ContentInfo.IsTop)} DESC, {nameof(ContentInfo.Taxis)} DESC, {nameof(ContentInfo.Id)} DESC)
GO
CREATE INDEX IX_{tableName}_Taxis ON {tableName}({nameof(ContentInfo.Taxis)} DESC)
GO");
            }

            return sqlBuilder.ToString();
        }
    }
}
