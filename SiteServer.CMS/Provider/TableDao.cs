using System;
using System.Collections.Generic;
using System.Data;
using SiteServer.CMS.Core;
using SiteServer.CMS.Data;
using SiteServer.CMS.Model;
using SiteServer.CMS.Model.Attributes;
using SiteServer.Plugin;
using SiteServer.Utils;

namespace SiteServer.CMS.Provider
{
    public class TableDao : DataProviderBase
	{
        public override string TableName => "siteserver_Table";

        public override List<TableColumn> TableColumns => new List<TableColumn>
        {
            new TableColumn
            {
                AttributeName = nameof(TableInfo.Id),
                DataType = DataType.Integer,
                IsPrimaryKey = true,
                IsIdentity = true
            },
            new TableColumn
            {
                AttributeName = nameof(TableInfo.TableName),
                DataType = DataType.VarChar,
                DataLength = 50
            },
            new TableColumn
            {
                AttributeName = nameof(TableInfo.DisplayName),
                DataType = DataType.VarChar,
                DataLength = 50
            },
            new TableColumn
            {
                AttributeName = nameof(TableInfo.AttributeNum),
                DataType = DataType.Integer
            },
            new TableColumn
            {
                AttributeName = nameof(TableInfo.IsCreatedInDb),
                DataType = DataType.VarChar,
                DataLength = 18
            },
            new TableColumn
            {
                AttributeName = nameof(TableInfo.IsChangedAfterCreatedInDb),
                DataType = DataType.VarChar,
                DataLength = 18
            },
            new TableColumn
            {
                AttributeName = nameof(TableInfo.IsDefault),
                DataType = DataType.VarChar,
                DataLength = 18
            },
            new TableColumn
            {
                AttributeName = nameof(TableInfo.Description),
                DataType = DataType.Text   
            }
        };

        private const string SqlSelectTable = "SELECT Id, TableName, DisplayName, AttributeNum, IsCreatedInDB, IsChangedAfterCreatedInDB, IsDefault, Description FROM siteserver_Table WHERE TableName = @TableName";
        private const string SqlSelectDisplayName = "SELECT DisplayName FROM siteserver_Table WHERE TableName = @TableName";
        private const string SqlSelectAllTableCreatedInDb = "SELECT Id, TableName, DisplayName, AttributeNum, IsCreatedInDB, IsChangedAfterCreatedInDB, IsDefault, Description FROM siteserver_Table WHERE IsCreatedInDB = @IsCreatedInDB ORDER BY IsCreatedInDB DESC, TableName";
		private const string SqlSelectTableCount = "SELECT COUNT(*) FROM siteserver_Table";
		private const string SqlSelectTableName = "SELECT TableName FROM siteserver_Table";

        private const string SqlInsertTable = "INSERT INTO siteserver_Table (TableName, DisplayName, AttributeNum, IsCreatedInDB, IsChangedAfterCreatedInDB, IsDefault, Description) VALUES (@TableName, @DisplayName, @AttributeNum, @IsCreatedInDB, @IsChangedAfterCreatedInDB, @IsDefault, @Description)";
        private const string SqlUpdateTable = "UPDATE siteserver_Table SET DisplayName = @DisplayName, AttributeNum = @AttributeNum, IsCreatedInDB = @IsCreatedInDB, IsChangedAfterCreatedInDB = @IsChangedAfterCreatedInDB, IsDefault = @IsDefault, Description = @Description WHERE  TableName = @TableName";
		private const string SqlUpdateTableAttributeNum = "UPDATE siteserver_Table SET AttributeNum = @AttributeNum WHERE  TableName = @TableName";
		private const string SqlUpdateTableIsCreatedInDb = "UPDATE siteserver_Table SET IsCreatedInDB = @IsCreatedInDB WHERE  TableName = @TableName";
		private const string SqlUpdateTableIsChangedAfterCreatedInDb = "UPDATE siteserver_Table SET IsChangedAfterCreatedInDB = @IsChangedAfterCreatedInDB WHERE  TableName = @TableName";
		private const string SqlDeleteTable = "DELETE FROM siteserver_Table WHERE TableName = @TableName";

		private const string ParmTableName = "@TableName";
		private const string ParmDisplayName = "@DisplayName";
		private const string ParmAttributeNum = "@AttributeNum";
		private const string ParmIsCreatedInDb = "@IsCreatedInDB";
		private const string ParmIsChangedAfterCreatedInDb = "@IsChangedAfterCreatedInDB";
        private const string ParmIsDefault = "@IsDefault";
		private const string ParmDescription = "@Description";

		public void Insert(TableInfo tableInfo, List<TableMetadataInfo> metadataInfoList) 
		{
			var insertParms = new IDataParameter[]
			{
				GetParameter(ParmTableName, DataType.VarChar, 50, tableInfo.TableName),
				GetParameter(ParmDisplayName, DataType.VarChar, 50, tableInfo.DisplayName),
				GetParameter(ParmAttributeNum, DataType.Integer, tableInfo.AttributeNum),
				GetParameter(ParmIsCreatedInDb, DataType.VarChar, 18, false.ToString()),
				GetParameter(ParmIsChangedAfterCreatedInDb, DataType.VarChar, 18, false.ToString()),
                GetParameter(ParmIsDefault, DataType.VarChar, 18, tableInfo.IsDefault.ToString()),
				GetParameter(ParmDescription, DataType.Text, tableInfo.Description)
			};
							
			using (var conn = GetConnection()) 
			{
				conn.Open();
				using (var trans = conn.BeginTransaction()) 
				{
					try 
					{
						ExecuteNonQuery(trans, SqlInsertTable, insertParms);
                        //DataProvider.TableMetadataDao.InsertSystemItems(tableInfo.TableName, trans);

                        if (metadataInfoList != null && metadataInfoList.Count > 0)
                        {
                            var taxis = 1;
                            foreach (var metadataInfo in metadataInfoList)
                            {
                                DataProvider.TableMetadataDao.InsertWithTransaction(metadataInfo, taxis++, trans);
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

		public void Update(TableInfo info) 
		{
			var updateParms = new IDataParameter[]
			{
				GetParameter(ParmDisplayName, DataType.VarChar, 50, info.DisplayName),
				GetParameter(ParmAttributeNum, DataType.Integer, info.AttributeNum),
				GetParameter(ParmIsCreatedInDb, DataType.VarChar, 18, info.IsCreatedInDb.ToString()),
				GetParameter(ParmIsChangedAfterCreatedInDb, DataType.VarChar, 18, info.IsChangedAfterCreatedInDb.ToString()),
                GetParameter(ParmIsDefault, DataType.VarChar, 18, info.IsDefault.ToString()),
				GetParameter(ParmDescription, DataType.Text, info.Description),
				GetParameter(ParmTableName, DataType.VarChar, 50, info.TableName)
			};

            ExecuteNonQuery(SqlUpdateTable, updateParms);
		}

		public void UpdateAttributeNum(string tableName)
		{
            var fieldNum = DataProvider.TableMetadataDao.GetTableMetadataCountByEnName(tableName);
			UpdateAttributeNum(fieldNum, tableName);
		}

		private void UpdateAttributeNum(int attributeNum, string tableName)
		{
			var updateParms = new IDataParameter[]
			{
				GetParameter(ParmAttributeNum, DataType.Integer, attributeNum),
				GetParameter(ParmTableName, DataType.VarChar, 50, tableName)
			};

            ExecuteNonQuery(SqlUpdateTableAttributeNum, updateParms);
		}

		public void UpdateIsCreatedInDb(bool isCreatedInDb, string tableName)
		{
			var updateParms = new IDataParameter[]
			{
				GetParameter(ParmIsCreatedInDb, DataType.VarChar, 18, isCreatedInDb.ToString()),
				GetParameter(ParmTableName, DataType.VarChar, 50, tableName)
			};

            ExecuteNonQuery(SqlUpdateTableIsCreatedInDb, updateParms);
		}

        public void UpdateIsChangedAfterCreatedInDbToTrue(string tableName)
		{
			var updateParms = new IDataParameter[]
			{
				GetParameter(ParmIsChangedAfterCreatedInDb, DataType.VarChar, 18, true.ToString()),
				GetParameter(ParmTableName, DataType.VarChar, 50, tableName)
			};

            ExecuteNonQuery(SqlUpdateTableIsChangedAfterCreatedInDb, updateParms);
		}

        public void UpdateIsChangedAfterCreatedInDbToFalse(string tableName)
        {
            var updateParms = new IDataParameter[]
            {
                GetParameter(ParmIsChangedAfterCreatedInDb, DataType.VarChar, 18, false.ToString()),
                GetParameter(ParmTableName, DataType.VarChar, 50, tableName)
            };

            ExecuteNonQuery(SqlUpdateTableIsChangedAfterCreatedInDb, updateParms);
        }

        public void DeleteCollectionTableInfoAndDbTable(string tableName)
		{
            var isDbExists = DataProvider.DatabaseDao.IsTableExists(tableName);
			var parms = new IDataParameter[]
			{
				GetParameter(ParmTableName, DataType.VarChar, 50, tableName),
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
							ExecuteNonQuery(trans, SqlUtils.GetDropTableSqlString(tableName));
                            TableColumnManager.ClearCache();
						}
						
						ExecuteNonQuery(trans, SqlDeleteTable, parms);
                        DataProvider.TableMetadataDao.Delete(tableName, trans);
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

        public void DeleteDbTable(string tableName)
        {
            if (!DataProvider.DatabaseDao.IsTableExists(tableName)) return;

            var dropTableSqlString = SqlUtils.GetDropTableSqlString(tableName);

            var updateParms = new IDataParameter[]
            {
                GetParameter("@IsCreatedInDB", DataType.VarChar, 18, false.ToString()),
                GetParameter("@IsChangedAfterCreatedInDB", DataType.VarChar, 18, false.ToString()),
                GetParameter("@TableName", DataType.VarChar, 50, tableName)
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

                        ExecuteNonQuery(trans, "UPDATE siteserver_Table SET IsCreatedInDB = @IsCreatedInDB, IsChangedAfterCreatedInDB = @IsChangedAfterCreatedInDB WHERE TableName = @TableName", updateParms);
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

        public void ReCreateDbTable(string tableName)
        {
            var isTableExists = DataProvider.DatabaseDao.IsTableExists(tableName);

            var updateParms = new IDataParameter[]
            {
                GetParameter("@IsCreatedInDB", DataType.VarChar, 18, true.ToString()),
                GetParameter("@IsChangedAfterCreatedInDB", DataType.VarChar, 18, false.ToString()),
                GetParameter("@TableName", DataType.VarChar, 50, tableName)
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
                            var dropTableSqlString = SqlUtils.GetDropTableSqlString(tableName);
                            ExecuteNonQuery(trans, dropTableSqlString);
                        }

                        var createTableSqlString = DataProvider.ContentDao.GetCreateTableCollectionInfoSqlString(tableName);
                        var reader = new System.IO.StringReader(createTableSqlString);
                        string sql;
                        while (null != (sql = SqlUtils.ReadNextStatementFromStream(reader)))
                        {
                            ExecuteNonQuery(trans, sql.Trim());
                        }

                        TableColumnManager.ClearCache();

                        ExecuteNonQuery(trans, "UPDATE siteserver_Table SET IsCreatedInDB = @IsCreatedInDB, IsChangedAfterCreatedInDB = @IsChangedAfterCreatedInDB WHERE  TableName = @TableName", updateParms);
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

        public void CreateDbTable(string tableName)
        {
            var isDbExists = DataProvider.DatabaseDao.IsTableExists(tableName);
            if (isDbExists) return;

            var createTableSqlString = DataProvider.ContentDao.GetCreateTableCollectionInfoSqlString(tableName);

            var updateParms = new IDataParameter[]
            {
                GetParameter("@IsCreatedInDB", DataType.VarChar, 18, true.ToString()),
                GetParameter("@IsChangedAfterCreatedInDB", DataType.VarChar, 18, false.ToString()),
                GetParameter("@TableName", DataType.VarChar, 50, tableName)
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

                        ExecuteNonQuery(trans, "UPDATE siteserver_Table SET IsCreatedInDB = @IsCreatedInDB, IsChangedAfterCreatedInDB = @IsChangedAfterCreatedInDB WHERE TableName = @TableName", updateParms);
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

        public void SyncDbTable(string tableName)
        {
            var metadataInfoList = TableMetadataManager.GetTableMetadataInfoList(tableName);
            var columnInfolist = TableColumnManager.GetTableColumnInfoList(tableName, ContentAttribute.AllAttributesLowercase);

            var sqlList = new List<string>();

            //添加新增/修改字段SQL语句
            foreach (var metadataInfo in metadataInfoList)
            {
                var columnExists = false;
                foreach (var columnInfo in columnInfolist)
                {
                    if (!StringUtils.EqualsIgnoreCase(columnInfo.AttributeName, metadataInfo.AttributeName)) continue;

                    columnExists = true;

                    if (metadataInfo.DataType != columnInfo.DataType)
                    {
                        var dropColumnsSqlList = DataProvider.DatabaseDao.GetDropColumnsSqlString(tableName, metadataInfo.AttributeName);
                        foreach (var sql in dropColumnsSqlList)
                        {
                            sqlList.Add(sql);
                        }
                        var addColumnsSqlList1 = DataProvider.TableMetadataDao.GetAddColumnsSqlString(tableName, metadataInfo);
                        foreach (var sql in addColumnsSqlList1)
                        {
                            sqlList.Add(sql);
                        }
                    }

                    break;
                }
                if (columnExists) continue;

                var addColumnsSqlList2 = DataProvider.TableMetadataDao.GetAddColumnsSqlString(tableName, metadataInfo);
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
                    if (StringUtils.EqualsIgnoreCase(columnInfo.AttributeName, metadataInfo.AttributeName))
                    {
                        isNeedDelete = false;
                        break;
                    }
                }
                if (isNeedDelete)
                {
                    var dropColumnsSqlList = DataProvider.DatabaseDao.GetDropColumnsSqlString(tableName, columnInfo.AttributeName);
                    foreach (var sql in dropColumnsSqlList)
                    {
                        sqlList.Add(sql);
                    }
                }
            }

            if (sqlList.Count <= 0) return;

            DataProvider.DatabaseDao.ExecuteSql(sqlList);
            DataProvider.TableDao.UpdateIsChangedAfterCreatedInDbToFalse(tableName);
            TableColumnManager.ClearCache();
        }

        public void CreateDbTableOfArchive(string tableName)
        {
            var createTableSqlString = DataProvider.ContentDao.GetCreateTableCollectionInfoSqlString(tableName);

            var archiveTableName = TableMetadataManager.GetTableNameOfArchive(tableName);

            createTableSqlString = createTableSqlString.Replace(tableName, archiveTableName);

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

        public TableInfo GetTableCollectionInfo(string tableName)
		{
            TableInfo info = null;
			
			var parms = new IDataParameter[]
			{
				GetParameter(ParmTableName, DataType.VarChar, 50, tableName)
			};
			
			using (var rdr = ExecuteReader(SqlSelectTable, parms)) 
			{
				if (rdr.Read())
				{
				    var i = 0;
                    info = new TableInfo(GetInt(rdr, i++), GetString(rdr, i++), GetString(rdr, i++), GetInt(rdr, i++), GetBool(rdr, i++), GetBool(rdr, i++), GetBool(rdr, i++), GetString(rdr, i));
				}
				rdr.Close();
			}

			return info;
		}

        public string GetDisplayName(string tableName)
        {
            var displayName = string.Empty;

            var parms = new IDataParameter[]
			{
				GetParameter(ParmTableName, DataType.VarChar, 50, tableName)
			};

            using (var rdr = ExecuteReader(SqlSelectDisplayName, parms))
            {
                if (rdr.Read())
                {
                    displayName = GetString(rdr, 0);
                }
                rdr.Close();
            }

            return displayName;
        }

		public IDataReader GetDataSourceCreatedInDb()
		{
			var parms = new IDataParameter[]
			{
				GetParameter(ParmIsCreatedInDb, DataType.VarChar, 18, true.ToString())
			};

			var enumerable = ExecuteReader(SqlSelectAllTableCreatedInDb, parms);
			return enumerable;
		}

		/// <summary>
		/// Get Total AuxiliaryTable Count
		/// </summary>
		public int GetAuxiliaryTableCount()
		{
            return Convert.ToInt32(ExecuteScalar(SqlSelectTableCount));
		}

		public List<string> GetTableNameList()
		{
			var list = new List<string>();
			using (var rdr = ExecuteReader(SqlSelectTableName)) 
			{
				while (rdr.Read()) 
				{
                    list.Add(GetString(rdr, 0));
				}
				rdr.Close();
			}

			return list;
		}

        public List<TableInfo> GetTableCollectionInfoListCreatedInDb()
        {
            var list = new List<TableInfo>();

            string sqlString =
                $"SELECT Id, TableName, DisplayName, AttributeNum, IsCreatedInDB, IsChangedAfterCreatedInDB, IsDefault, Description FROM siteserver_Table WHERE IsCreatedInDB = '{true}' ORDER BY IsCreatedInDB DESC, TableName";

            using (var rdr = ExecuteReader(sqlString))
            {
                while (rdr.Read())
                {
                    var i = 0;
                    var info = new TableInfo(GetInt(rdr, i++), GetString(rdr, i++), GetString(rdr, i++), GetInt(rdr, i++), GetBool(rdr, i++), GetBool(rdr, i++), GetBool(rdr, i++), GetString(rdr, i));
                    list.Add(info);
                }
                rdr.Close();
            }

            return list;
        }

        public List<TableInfo> GetTableCollectionInfoList()
        {
            var list = new List<TableInfo>();

            const string sqlString = "SELECT Id, TableName, DisplayName, AttributeNum, IsCreatedInDB, IsChangedAfterCreatedInDB, IsDefault, Description FROM siteserver_Table ORDER BY IsCreatedInDB DESC, TableName";

            using (var rdr = ExecuteReader(sqlString))
            {
                while (rdr.Read())
                {
                    var i = 0;
                    var info = new TableInfo(GetInt(rdr, i++), GetString(rdr, i++), GetString(rdr, i++), GetInt(rdr, i++), GetBool(rdr, i++), GetBool(rdr, i++), GetBool(rdr, i++), GetString(rdr, i));
                    list.Add(info);
                }
                rdr.Close();
            }

            return list;
        }

        public List<string> GetTableNameListCreatedInDb()
        {
            var list = new List<string>();

            string sqlString =
                $"SELECT TableName FROM siteserver_Table WHERE IsCreatedInDB = '{true}'";

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

            const string sqlString = "SELECT TableName FROM siteserver_Table WHERE IsCreatedInDB = 'True'";
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
                $"SELECT TableName FROM siteserver_Table WHERE TableName = '{PageUtils.FilterSql(tableName)}' AND IsCreatedInDB = 'True'";
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
                $"SELECT TableName FROM siteserver_Table WHERE TableName = '{PageUtils.FilterSql(tableName)}'";
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

            const string sqlString = "SELECT TableName FROM siteserver_Table WHERE IsCreatedInDB = 'True'";

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
                    var tableInfo = new TableInfo(0, tableName, "后台内容表", 0, false, false, true, string.Empty);
                    var metadataInfoList = DataProvider.TableMetadataDao.GetDefaultTableMetadataInfoList(tableName);
                    Insert(tableInfo, metadataInfoList);
                }
                CreateDbTable(tableName);
            }
        }

	    private const string DefaultTableName = "model_Content";
	}
}
