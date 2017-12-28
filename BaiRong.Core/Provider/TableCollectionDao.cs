using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using BaiRong.Core.Data;
using BaiRong.Core.Model;
using BaiRong.Core.Table;
using SiteServer.Plugin.Models;

namespace BaiRong.Core.Provider
{
    public class TableCollectionDao : DataProviderBase
	{
        public override string TableName => "bairong_TableCollection";

        public override List<TableColumnInfo> TableColumns => new List<TableColumnInfo>
        {
            new TableColumnInfo
            {
                ColumnName = nameof(TableCollectionInfo.TableEnName),
                DataType = DataType.VarChar,
                Length = 50,
                IsPrimaryKey = true
            },
            new TableColumnInfo
            {
                ColumnName = nameof(TableCollectionInfo.TableCnName),
                DataType = DataType.VarChar,
                Length = 50
            },
            new TableColumnInfo
            {
                ColumnName = nameof(TableCollectionInfo.AttributeNum),
                DataType = DataType.Integer
            },
            new TableColumnInfo
            {
                ColumnName = nameof(TableCollectionInfo.IsCreatedInDb),
                DataType = DataType.VarChar,
                Length = 18
            },
            new TableColumnInfo
            {
                ColumnName = nameof(TableCollectionInfo.IsChangedAfterCreatedInDb),
                DataType = DataType.VarChar,
                Length = 18
            },
            new TableColumnInfo
            {
                ColumnName = nameof(TableCollectionInfo.IsDefault),
                DataType = DataType.VarChar,
                Length = 18
            },
            new TableColumnInfo
            {
                ColumnName = nameof(TableCollectionInfo.Description),
                DataType = DataType.Text   
            }
        };

        private const string SqlSelectTable = "SELECT TableENName, TableCNName, AttributeNum, IsCreatedInDB, IsChangedAfterCreatedInDB, IsDefault, Description FROM bairong_TableCollection WHERE TableENName = @TableENName";
        private const string SqlSelectTableCnname = "SELECT TableCNName FROM bairong_TableCollection WHERE TableENName = @TableENName";
        private const string SqlSelectAllTableCreatedInDb = "SELECT TableENName, TableCNName, AttributeNum, IsCreatedInDB, IsChangedAfterCreatedInDB, IsDefault, Description FROM bairong_TableCollection WHERE IsCreatedInDB = @IsCreatedInDB ORDER BY IsCreatedInDB DESC, TableENName";
		private const string SqlSelectTableCount = "SELECT COUNT(*) FROM bairong_TableCollection";
		private const string SqlSelectTableEnname = "SELECT TableENName FROM bairong_TableCollection";

        private const string SqlInsertTable = "INSERT INTO bairong_TableCollection (TableENName, TableCNName, AttributeNum, IsCreatedInDB, IsChangedAfterCreatedInDB, IsDefault, Description) VALUES (@TableENName, @TableCNName, @AttributeNum, @IsCreatedInDB, @IsChangedAfterCreatedInDB, @IsDefault, @Description)";
        private const string SqlUpdateTable = "UPDATE bairong_TableCollection SET TableCNName = @TableCNName, AttributeNum = @AttributeNum, IsCreatedInDB = @IsCreatedInDB, IsChangedAfterCreatedInDB = @IsChangedAfterCreatedInDB, IsDefault = @IsDefault, Description = @Description WHERE  TableENName = @TableENName";
		private const string SqlUpdateTableAttributeNum = "UPDATE bairong_TableCollection SET AttributeNum = @AttributeNum WHERE  TableENName = @TableENName";
		private const string SqlUpdateTableIsCreatedInDb = "UPDATE bairong_TableCollection SET IsCreatedInDB = @IsCreatedInDB WHERE  TableENName = @TableENName";
		private const string SqlUpdateTableIsChangedAfterCreatedInDb = "UPDATE bairong_TableCollection SET IsChangedAfterCreatedInDB = @IsChangedAfterCreatedInDB WHERE  TableENName = @TableENName";
		private const string SqlDeleteTable = "DELETE FROM bairong_TableCollection WHERE TableENName = @TableENName";

		private const string ParmTableEnname = "@TableENName";
		private const string ParmTableCnname = "@TableCNName";
		private const string ParmAttributeNum = "@AttributeNum";
		private const string ParmIsCreatedInDb = "@IsCreatedInDB";
		private const string ParmIsChangedAfterCreatedInDb = "@IsChangedAfterCreatedInDB";
        private const string ParmIsDefault = "@IsDefault";
		private const string ParmDescription = "@Description";

		public void Insert(TableCollectionInfo collectionInfo, List<TableMetadataInfo> metadataInfoList) 
		{
			var insertParms = new IDataParameter[]
			{
				GetParameter(ParmTableEnname, DataType.VarChar, 50, collectionInfo.TableEnName),
				GetParameter(ParmTableCnname, DataType.VarChar, 50, collectionInfo.TableCnName),
				GetParameter(ParmAttributeNum, DataType.Integer, collectionInfo.AttributeNum),
				GetParameter(ParmIsCreatedInDb, DataType.VarChar, 18, false.ToString()),
				GetParameter(ParmIsChangedAfterCreatedInDb, DataType.VarChar, 18, false.ToString()),
                GetParameter(ParmIsDefault, DataType.VarChar, 18, collectionInfo.IsDefault.ToString()),
				GetParameter(ParmDescription, DataType.Text, collectionInfo.Description)
			};
							
			using (var conn = GetConnection()) 
			{
				conn.Open();
				using (var trans = conn.BeginTransaction()) 
				{
					try 
					{
						ExecuteNonQuery(trans, SqlInsertTable, insertParms);
                        //BaiRongDataProvider.TableMetadataDao.InsertSystemItems(collectionInfo.TableEnName, trans);

                        if (metadataInfoList != null && metadataInfoList.Count > 0)
                        {
                            var taxis = 1;
                            foreach (var metadataInfo in metadataInfoList)
                            {
                                BaiRongDataProvider.TableMetadataDao.InsertWithTransaction(metadataInfo, taxis++, trans);
                            }
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

		public void Update(TableCollectionInfo info) 
		{
			var updateParms = new IDataParameter[]
			{
				GetParameter(ParmTableCnname, DataType.VarChar, 50, info.TableCnName),
				GetParameter(ParmAttributeNum, DataType.Integer, info.AttributeNum),
				GetParameter(ParmIsCreatedInDb, DataType.VarChar, 18, info.IsCreatedInDb.ToString()),
				GetParameter(ParmIsChangedAfterCreatedInDb, DataType.VarChar, 18, info.IsChangedAfterCreatedInDb.ToString()),
                GetParameter(ParmIsDefault, DataType.VarChar, 18, info.IsDefault.ToString()),
				GetParameter(ParmDescription, DataType.Text, info.Description),
				GetParameter(ParmTableEnname, DataType.VarChar, 50, info.TableEnName)
			};

            ExecuteNonQuery(SqlUpdateTable, updateParms);
		}

		public void UpdateAttributeNum(string tableEnName)
		{
            var fieldNum = BaiRongDataProvider.TableMetadataDao.GetTableMetadataCountByEnName(tableEnName);
			UpdateAttributeNum(fieldNum, tableEnName);
		}

		private void UpdateAttributeNum(int attributeNum, string tableEnName)
		{
			var updateParms = new IDataParameter[]
			{
				GetParameter(ParmAttributeNum, DataType.Integer, attributeNum),
				GetParameter(ParmTableEnname, DataType.VarChar, 50, tableEnName)
			};

            ExecuteNonQuery(SqlUpdateTableAttributeNum, updateParms);
		}

		public void UpdateIsCreatedInDb(bool isCreatedInDb, string tableEnName)
		{
			var updateParms = new IDataParameter[]
			{
				GetParameter(ParmIsCreatedInDb, DataType.VarChar, 18, isCreatedInDb.ToString()),
				GetParameter(ParmTableEnname, DataType.VarChar, 50, tableEnName)
			};

            ExecuteNonQuery(SqlUpdateTableIsCreatedInDb, updateParms);
		}

        public void UpdateIsChangedAfterCreatedInDbToTrue(string tableName)
		{
			var updateParms = new IDataParameter[]
			{
				GetParameter(ParmIsChangedAfterCreatedInDb, DataType.VarChar, 18, true.ToString()),
				GetParameter(ParmTableEnname, DataType.VarChar, 50, tableName)
			};

            ExecuteNonQuery(SqlUpdateTableIsChangedAfterCreatedInDb, updateParms);
		}

        public void UpdateIsChangedAfterCreatedInDbToFalse(string tableName)
        {
            var updateParms = new IDataParameter[]
            {
                GetParameter(ParmIsChangedAfterCreatedInDb, DataType.VarChar, 18, false.ToString()),
                GetParameter(ParmTableEnname, DataType.VarChar, 50, tableName)
            };

            ExecuteNonQuery(SqlUpdateTableIsChangedAfterCreatedInDb, updateParms);
        }

        public void DeleteCollectionTableInfoAndDbTable(string tableEnName)
		{
            var isDbExists = BaiRongDataProvider.DatabaseDao.IsTableExists(tableEnName);
			var parms = new IDataParameter[]
			{
				GetParameter(ParmTableEnname, DataType.VarChar, 50, tableEnName),
			};
							
			using (var conn = GetConnection()) 
			{
				conn.Open();
				using (var trans = conn.BeginTransaction()) 
				{
					try 
					{
						if (isDbExists)
						{
							ExecuteNonQuery(trans, SqlUtils.GetDropTableSqlString(tableEnName));
                            TableColumnManager.ClearCache();
						}
						
						ExecuteNonQuery(trans, SqlDeleteTable, parms);
                        BaiRongDataProvider.TableMetadataDao.Delete(tableEnName, trans);
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

        public void DeleteDbTable(string tableEnName)
        {
            if (!BaiRongDataProvider.DatabaseDao.IsTableExists(tableEnName)) return;

            var dropTableSqlString = SqlUtils.GetDropTableSqlString(tableEnName);

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
                        TableColumnManager.ClearCache();

                        ExecuteNonQuery(trans, "UPDATE bairong_TableCollection SET IsCreatedInDB = @IsCreatedInDB, IsChangedAfterCreatedInDB = @IsChangedAfterCreatedInDB WHERE TableENName = @TableENName", updateParms);
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

        public void ReCreateDbTable(string tableEnName)
        {
            var isTableExists = BaiRongDataProvider.DatabaseDao.IsTableExists(tableEnName);

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
                        if (isTableExists)
                        {
                            var dropTableSqlString = SqlUtils.GetDropTableSqlString(tableEnName);
                            ExecuteNonQuery(trans, dropTableSqlString);
                        }

                        var createTableSqlString = SqlUtils.GetCreateTableCollectionInfoSqlString(tableEnName);
                        var reader = new System.IO.StringReader(createTableSqlString);
                        string sql;
                        while (null != (sql = SqlUtils.ReadNextStatementFromStream(reader)))
                        {
                            ExecuteNonQuery(trans, sql.Trim());
                        }

                        TableColumnManager.ClearCache();

                        ExecuteNonQuery(trans, "UPDATE bairong_TableCollection SET IsCreatedInDB = @IsCreatedInDB, IsChangedAfterCreatedInDB = @IsChangedAfterCreatedInDB WHERE  TableENName = @TableENName", updateParms);
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

        public void CreateDbTable(string tableEnName)
        {
            var isDbExists = BaiRongDataProvider.DatabaseDao.IsTableExists(tableEnName);
            if (isDbExists) return;

            var createTableSqlString = SqlUtils.GetCreateTableCollectionInfoSqlString(tableEnName);

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

                        TableColumnManager.ClearCache();

                        ExecuteNonQuery(trans, "UPDATE bairong_TableCollection SET IsCreatedInDB = @IsCreatedInDB, IsChangedAfterCreatedInDB = @IsChangedAfterCreatedInDB WHERE TableENName = @TableENName", updateParms);
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

        public void SyncDbTable(string tableEnName)
        {
            var metadataInfoList = TableMetadataManager.GetTableMetadataInfoList(tableEnName);
            var columnInfolist = TableColumnManager.GetTableColumnInfoListLowercase(tableEnName, ContentAttribute.AllAttributesLowercase);

            var sqlList = new List<string>();

            //添加新增/修改字段SQL语句
            foreach (var metadataInfo in metadataInfoList)
            {
                var columnExists = false;
                foreach (var columnInfo in columnInfolist)
                {
                    if (!StringUtils.EqualsIgnoreCase(columnInfo.ColumnName, metadataInfo.AttributeName)) continue;

                    columnExists = true;

                    if (metadataInfo.DataType != columnInfo.DataType || metadataInfo.DataType == DataType.VarChar && metadataInfo.DataLength != columnInfo.Length)
                    {
                        var dropColumnsSqlList = SqlUtils.GetDropColumnsSqlString(tableEnName, metadataInfo.AttributeName);
                        foreach (var sql in dropColumnsSqlList)
                        {
                            sqlList.Add(sql);
                        }
                        var addColumnsSqlList1 = SqlUtils.GetAddColumnsSqlString(tableEnName, metadataInfo);
                        foreach (var sql in addColumnsSqlList1)
                        {
                            sqlList.Add(sql);
                        }
                    }

                    break;
                }
                if (columnExists) continue;

                var addColumnsSqlList2 = SqlUtils.GetAddColumnsSqlString(tableEnName, metadataInfo);
                foreach (var sql in addColumnsSqlList2)
                {
                    sqlList.Add(sql);
                }
            }

            //添加删除字段SQL语句
            foreach (var columnInfo in columnInfolist)
            {
                var isNeedDelete = true;
                foreach (var metadataInfo in metadataInfoList)
                {
                    if (StringUtils.EqualsIgnoreCase(columnInfo.ColumnName, metadataInfo.AttributeName))
                    {
                        isNeedDelete = false;
                        break;
                    }
                }
                if (isNeedDelete)
                {
                    var dropColumnsSqlList = SqlUtils.GetDropColumnsSqlString(tableEnName, columnInfo.ColumnName);
                    foreach (var sql in dropColumnsSqlList)
                    {
                        sqlList.Add(sql);
                    }
                }
            }

            if (sqlList.Count <= 0) return;

            BaiRongDataProvider.DatabaseDao.ExecuteSql(sqlList);
            BaiRongDataProvider.TableCollectionDao.UpdateIsChangedAfterCreatedInDbToFalse(tableEnName);
            TableColumnManager.ClearCache();
        }

        public void CreateDbTableOfArchive(string tableEnName)
        {
            var createTableSqlString = SqlUtils.GetCreateTableCollectionInfoSqlString(tableEnName);

            var archiveTableName = TableMetadataManager.GetTableNameOfArchive(tableEnName);

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

                        TableColumnManager.ClearCache();

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

        public TableCollectionInfo GetTableCollectionInfo(string tableEnName)
		{
            TableCollectionInfo info = null;
			
			var parms = new IDataParameter[]
			{
				GetParameter(ParmTableEnname, DataType.VarChar, 50, tableEnName)
			};
			
			using (var rdr = ExecuteReader(SqlSelectTable, parms)) 
			{
				if (rdr.Read())
				{
				    var i = 0;
                    info = new TableCollectionInfo(GetString(rdr, i++), GetString(rdr, i++), GetInt(rdr, i++), GetBool(rdr, i++), GetBool(rdr, i++), GetBool(rdr, i++), GetString(rdr, i));
				}
				rdr.Close();
			}

			return info;
		}

        public string GetTableCnName(string tableEnName)
        {
            var tableCnName = string.Empty;

            var parms = new IDataParameter[]
			{
				GetParameter(ParmTableEnname, DataType.VarChar, 50, tableEnName)
			};

            using (var rdr = ExecuteReader(SqlSelectTableCnname, parms))
            {
                if (rdr.Read())
                {
                    tableCnName = GetString(rdr, 0);
                }
                rdr.Close();
            }

            return tableCnName;
        }

		public IEnumerable GetDataSourceCreatedInDb()
		{
			var parms = new IDataParameter[]
			{
				GetParameter(ParmIsCreatedInDb, DataType.VarChar, 18, true.ToString())
			};

			var enumerable = (IEnumerable)ExecuteReader(SqlSelectAllTableCreatedInDb, parms);
			return enumerable;
		}

		/// <summary>
		/// Get Total AuxiliaryTable Count
		/// </summary>
		public int GetAuxiliaryTableCount()
		{
            return Convert.ToInt32(ExecuteScalar(SqlSelectTableCount));
		}

		public List<string> GetTableEnNameList()
		{
			var list = new List<string>();
			using (var rdr = ExecuteReader(SqlSelectTableEnname)) 
			{
				while (rdr.Read()) 
				{
                    list.Add(GetString(rdr, 0));
				}
				rdr.Close();
			}

			return list;
		}

        public List<TableCollectionInfo> GetTableCollectionInfoListCreatedInDb()
        {
            var list = new List<TableCollectionInfo>();

            string sqlString =
                $"SELECT TableENName, TableCNName, AttributeNum, IsCreatedInDB, IsChangedAfterCreatedInDB, IsDefault, Description FROM bairong_TableCollection WHERE IsCreatedInDB = '{true}' ORDER BY IsCreatedInDB DESC, TableENName";

            using (var rdr = ExecuteReader(sqlString))
            {
                while (rdr.Read())
                {
                    var i = 0;
                    var info = new TableCollectionInfo(GetString(rdr, i++), GetString(rdr, i++), GetInt(rdr, i++), GetBool(rdr, i++), GetBool(rdr, i++), GetBool(rdr, i++), GetString(rdr, i));
                    list.Add(info);
                }
                rdr.Close();
            }

            return list;
        }

        public List<TableCollectionInfo> GetTableCollectionInfoList()
        {
            var list = new List<TableCollectionInfo>();

            const string sqlString = "SELECT TableENName, TableCNName, AttributeNum, IsCreatedInDB, IsChangedAfterCreatedInDB, IsDefault, Description FROM bairong_TableCollection ORDER BY IsCreatedInDB DESC, TableENName";

            using (var rdr = ExecuteReader(sqlString))
            {
                while (rdr.Read())
                {
                    var i = 0;
                    var info = new TableCollectionInfo(GetString(rdr, i++), GetString(rdr, i++), GetInt(rdr, i++), GetBool(rdr, i++), GetBool(rdr, i++), GetBool(rdr, i++), GetString(rdr, i));
                    list.Add(info);
                }
                rdr.Close();
            }

            return list;
        }

        public List<string> GetTableEnNameListCreatedInDb()
        {
            var list = new List<string>();

            string sqlString =
                $"SELECT TableENName FROM bairong_TableCollection WHERE IsCreatedInDB = '{true}'";

            using (var rdr = ExecuteReader(sqlString))
            {
                while (rdr.Read())
                {
                    list.Add(GetString(rdr, 0));
                }
                rdr.Close();
            }

            return list;
        }

        public bool IsTableExistsAndCreated()
        {
            var isExists = false;

            const string sqlString = "SELECT TableENName FROM bairong_TableCollection WHERE IsCreatedInDB = 'True'";
            using (var rdr = ExecuteReader(sqlString))
            {
                if (rdr.Read() && !rdr.IsDBNull(0))
                {
                    isExists = true;
                }
                rdr.Close();
            }

            return isExists;
        }

        public bool IsExistsAndCreated(string tableName)
        {
            var isExists = false;

            string sqlString =
                $"SELECT TableENName FROM bairong_TableCollection WHERE TableENName = '{PageUtils.FilterSql(tableName)}' AND IsCreatedInDB = 'True'";
            using (var rdr = ExecuteReader(sqlString))
            {
                if (rdr.Read() && !rdr.IsDBNull(0))
                {
                    isExists = true;
                }
                rdr.Close();
            }

            return isExists;
        }

        public bool IsExists(string tableName)
        {
            var isExists = false;

            string sqlString =
                $"SELECT TableENName FROM bairong_TableCollection WHERE TableENName = '{PageUtils.FilterSql(tableName)}'";
            using (var rdr = ExecuteReader(sqlString))
            {
                if (rdr.Read() && !rdr.IsDBNull(0))
                {
                    isExists = true;
                }
                rdr.Close();
            }

            return isExists;
        }

        public string GetFirstTableName()
        {
            var tableName = string.Empty;

            const string sqlString = "SELECT TableENName FROM bairong_TableCollection WHERE IsCreatedInDB = 'True'";

            using (var rdr = ExecuteReader(sqlString))
            {
                if (rdr.Read())
                {
                    tableName = GetString(rdr, 0);
                }
                rdr.Close();
            }

            return tableName;
        }

        public void CreateAllTableCollectionInfoIfNotExists()
        {
            //添加后台内容表
            if (!IsTableExistsAndCreated())
            {
                var tableName = DefaultTableName;
                if (!IsExists(tableName))
                {
                    var tableInfo = new TableCollectionInfo(tableName, "后台内容表", 0, false, false, true, string.Empty);
                    var metadataInfoList = BaiRongDataProvider.TableMetadataDao.GetDefaultTableMetadataInfoList(tableName);
                    Insert(tableInfo, metadataInfoList);
                }
                CreateDbTable(tableName);
            }
        }

	    private const string DefaultTableName = "model_Content";
	}
}
