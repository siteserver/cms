using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using BaiRong.Core.AuxiliaryTable;
using BaiRong.Core.Data;
using BaiRong.Core.Model;
using BaiRong.Core.Model.Enumerations;
using SiteServer.Plugin.Models;

namespace BaiRong.Core.Provider
{
    public class TableCollectionDao : DataProviderBase
	{
		// Static constants
        private const string SqlSelectTable = "SELECT TableENName, TableCNName, AttributeNum, AuxiliaryTableType, IsCreatedInDB, IsChangedAfterCreatedInDB, IsDefault, Description FROM bairong_TableCollection WHERE TableENName = @TableENName";
		private const string SqlSelectTableType = "SELECT AuxiliaryTableType FROM bairong_TableCollection WHERE TableENName = @TableENName";
        private const string SqlSelectTableCnname = "SELECT TableCNName FROM bairong_TableCollection WHERE TableENName = @TableENName";
        private const string SqlSelectAllTableCreatedInDbByAuxiliaryType = "SELECT TableENName, TableCNName, AttributeNum, AuxiliaryTableType, IsCreatedInDB, IsChangedAfterCreatedInDB, IsDefault, Description FROM bairong_TableCollection WHERE AuxiliaryTableType = @AuxiliaryTableType AND IsCreatedInDB = @IsCreatedInDB ORDER BY IsCreatedInDB DESC, TableENName";
		private const string SqlSelectTableCount = "SELECT COUNT(*) FROM bairong_TableCollection";
		private const string SqlSelectTableEnname = "SELECT TableENName FROM bairong_TableCollection";

        private const string SqlInsertTable = "INSERT INTO bairong_TableCollection (TableENName, TableCNName, AttributeNum, AuxiliaryTableType, IsCreatedInDB, IsChangedAfterCreatedInDB, IsDefault, Description) VALUES (@TableENName, @TableCNName, @AttributeNum, @AuxiliaryTableType, @IsCreatedInDB, @IsChangedAfterCreatedInDB, @IsDefault, @Description)";
        private const string SqlUpdateTable = "UPDATE bairong_TableCollection SET TableCNName = @TableCNName, AttributeNum = @AttributeNum, AuxiliaryTableType = @AuxiliaryTableType, IsCreatedInDB = @IsCreatedInDB, IsChangedAfterCreatedInDB = @IsChangedAfterCreatedInDB, IsDefault = @IsDefault, Description = @Description WHERE  TableENName = @TableENName";
		private const string SqlUpdateTableAttributeNum = "UPDATE bairong_TableCollection SET AttributeNum = @AttributeNum WHERE  TableENName = @TableENName";
		private const string SqlUpdateTableIsCreatedInDb = "UPDATE bairong_TableCollection SET IsCreatedInDB = @IsCreatedInDB WHERE  TableENName = @TableENName";
		private const string SqlUpdateTableIsChangedAfterCreatedInDb = "UPDATE bairong_TableCollection SET IsChangedAfterCreatedInDB = @IsChangedAfterCreatedInDB WHERE  TableENName = @TableENName";
		private const string SqlDeleteTable = "DELETE FROM bairong_TableCollection WHERE TableENName = @TableENName";

		private const string ParmTableEnname = "@TableENName";
		private const string ParmTableCnname = "@TableCNName";
		private const string ParmAttributeNum = "@AttributeNum";
		private const string ParmTableType = "@AuxiliaryTableType";
		private const string ParmIsCreatedInDb = "@IsCreatedInDB";
		private const string ParmIsChangedAfterCreatedInDb = "@IsChangedAfterCreatedInDB";
        private const string ParmIsDefault = "@IsDefault";
		private const string ParmDescription = "@Description";

		public void Insert(AuxiliaryTableInfo info) 
		{
			var insertParms = new IDataParameter[]
			{
				GetParameter(ParmTableEnname, DataType.VarChar, 50, info.TableEnName),
				GetParameter(ParmTableCnname, DataType.NVarChar, 50, info.TableCnName),
				GetParameter(ParmAttributeNum, DataType.Integer, info.AttributeNum),
				GetParameter(ParmTableType, DataType.VarChar, 50, EAuxiliaryTableTypeUtils.GetValue(info.AuxiliaryTableType)),
				GetParameter(ParmIsCreatedInDb, DataType.VarChar, 18, false.ToString()),
				GetParameter(ParmIsChangedAfterCreatedInDb, DataType.VarChar, 18, false.ToString()),
                GetParameter(ParmIsDefault, DataType.VarChar, 18, info.IsDefault.ToString()),
				GetParameter(ParmDescription, DataType.NText, info.Description)
			};
							
			using (var conn = GetConnection()) 
			{
				conn.Open();
				using (var trans = conn.BeginTransaction()) 
				{
					try 
					{
						ExecuteNonQuery(trans, SqlInsertTable, insertParms);
                        BaiRongDataProvider.TableMetadataDao.InsertSystemItems(info.TableEnName, info.AuxiliaryTableType, trans);
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

		public void Update(AuxiliaryTableInfo info) 
		{
			var updateParms = new IDataParameter[]
			{
				GetParameter(ParmTableCnname, DataType.NVarChar, 50, info.TableCnName),
				GetParameter(ParmAttributeNum, DataType.Integer, info.AttributeNum),
				GetParameter(ParmTableType, DataType.VarChar, 50, EAuxiliaryTableTypeUtils.GetValue(info.AuxiliaryTableType)),
				GetParameter(ParmIsCreatedInDb, DataType.VarChar, 18, info.IsCreatedInDb.ToString()),
				GetParameter(ParmIsChangedAfterCreatedInDb, DataType.VarChar, 18, info.IsChangedAfterCreatedInDb.ToString()),
                GetParameter(ParmIsDefault, DataType.VarChar, 18, info.IsDefault.ToString()),
				GetParameter(ParmDescription, DataType.NText, info.Description),
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
            TableManager.IsChanged = true;
		}

        public void UpdateIsChangedAfterCreatedInDb(bool isChangedAfterCreatedInDb, string tableEnName)
		{
			var updateParms = new IDataParameter[]
			{
				GetParameter(ParmIsChangedAfterCreatedInDb, DataType.VarChar, 18, isChangedAfterCreatedInDb.ToString()),
				GetParameter(ParmTableEnname, DataType.VarChar, 50, tableEnName)
			};

            ExecuteNonQuery(SqlUpdateTableIsChangedAfterCreatedInDb, updateParms);
            TableManager.IsChanged = true;
		}

		public void Delete(string tableEnName)
		{
            var isTableExists = BaiRongDataProvider.DatabaseDao.IsTableExists(tableEnName);
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
						if (isTableExists)
						{
							ExecuteNonQuery(trans, $"DROP TABLE [{tableEnName}]");
						}
						
						ExecuteNonQuery(trans, SqlDeleteTable, parms);
                        BaiRongDataProvider.TableMetadataDao.Delete(tableEnName, trans);
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

		public AuxiliaryTableInfo GetAuxiliaryTableInfo(string tableEnName)
		{
			AuxiliaryTableInfo info = null;
			
			var parms = new IDataParameter[]
			{
				GetParameter(ParmTableEnname, DataType.VarChar, 50, tableEnName)
			};
			
			using (var rdr = ExecuteReader(SqlSelectTable, parms)) 
			{
				if (rdr.Read())
				{
				    var i = 0;
                    info = new AuxiliaryTableInfo(GetString(rdr, i++), GetString(rdr, i++), GetInt(rdr, i++), EAuxiliaryTableTypeUtils.GetEnumType(GetString(rdr, i++)), GetBool(rdr, i++), GetBool(rdr, i++), GetBool(rdr, i++), GetString(rdr, i));
				}
				rdr.Close();
			}

			return info;
		}

		public EAuxiliaryTableType GetTableType(string tableEnName)
		{
			var type = EAuxiliaryTableType.BackgroundContent;
			
			var parms = new IDataParameter[]
			{
				GetParameter(ParmTableEnname, DataType.VarChar, 50, tableEnName),
			};
			
			using (var rdr = ExecuteReader(SqlSelectTableType, parms)) 
			{
				if (rdr.Read()) 
				{
                    type = EAuxiliaryTableTypeUtils.GetEnumType(GetString(rdr, 0));
				}
				rdr.Close();
			}

			return type;
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

		public IEnumerable GetDataSourceByAuxiliaryTableType()
		{
            var sqlSelect = "SELECT TableENName, TableCNName, AttributeNum, AuxiliaryTableType, IsCreatedInDB, IsChangedAfterCreatedInDB, IsDefault, Description FROM bairong_TableCollection ORDER BY IsCreatedInDB DESC, TableENName";

			var enumerable = (IEnumerable)ExecuteReader(sqlSelect);
			return enumerable;
		}

		public IEnumerable GetDataSourceCreatedInDbByAuxiliaryTableType(EAuxiliaryTableType type)
		{
			var parms = new IDataParameter[]
			{
				GetParameter(ParmTableType, DataType.VarChar, 50, EAuxiliaryTableTypeUtils.GetValue(type)),
				GetParameter(ParmIsCreatedInDb, DataType.VarChar, 18, true.ToString())
			};

			var enumerable = (IEnumerable)ExecuteReader(SqlSelectAllTableCreatedInDbByAuxiliaryType, parms);
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

        public List<AuxiliaryTableInfo> GetAuxiliaryTableListCreatedInDbByAuxiliaryTableType(params EAuxiliaryTableType[] eAuxiliaryTableTypeArray)
		{
			var auxiliaryTableTypeList = new List<string>();
            foreach (var eAuxiliaryTableType in eAuxiliaryTableTypeArray)
            {
                auxiliaryTableTypeList.Add(EAuxiliaryTableTypeUtils.GetValue(eAuxiliaryTableType));
            }

            string sqlString =
                $"SELECT TableENName, TableCNName, AttributeNum, AuxiliaryTableType, IsCreatedInDB, IsChangedAfterCreatedInDB, IsDefault, Description FROM bairong_TableCollection WHERE AuxiliaryTableType IN ({TranslateUtils.ToSqlInStringWithQuote(auxiliaryTableTypeList)}) AND IsCreatedInDB = '{true}' ORDER BY IsCreatedInDB DESC, TableENName";

		    var list = new List<AuxiliaryTableInfo>();

            using (var rdr = ExecuteReader(sqlString)) 
			{
				while (rdr.Read())
				{
				    var i = 0;
                    var info = new AuxiliaryTableInfo(GetString(rdr, i++), GetString(rdr, i++), GetInt(rdr, i++), EAuxiliaryTableTypeUtils.GetEnumType(GetString(rdr, i++)), GetBool(rdr, i++), GetBool(rdr, i++), GetBool(rdr, i++), GetString(rdr, i));
                    list.Add(info);
				}
				rdr.Close();
			}

			return list;
		}

        public List<string> GetTableEnNameListCreatedInDbByAuxiliaryTableType(params EAuxiliaryTableType[] eAuxiliaryTableTypeArray)
        {
            var auxiliaryTableTypeList = new List<string>();
            foreach (var eAuxiliaryTableType in eAuxiliaryTableTypeArray)
            {
                auxiliaryTableTypeList.Add(EAuxiliaryTableTypeUtils.GetValue(eAuxiliaryTableType));
            }

            string sqlString =
                $"SELECT TableENName FROM bairong_TableCollection WHERE AuxiliaryTableType IN ({TranslateUtils.ToSqlInStringWithQuote(auxiliaryTableTypeList)}) AND IsCreatedInDB = '{true}' ORDER BY IsCreatedInDB DESC, TableENName";

            var list = new List<string>();

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

        public List<AuxiliaryTableInfo> GetAuxiliaryTableListCreatedInDb()
        {
            var list = new List<AuxiliaryTableInfo>();

            string sqlString =
                $"SELECT TableENName, TableCNName, AttributeNum, AuxiliaryTableType, IsCreatedInDB, IsChangedAfterCreatedInDB, IsDefault, Description FROM bairong_TableCollection WHERE IsCreatedInDB = '{true}' ORDER BY IsCreatedInDB DESC, TableENName";

            using (var rdr = ExecuteReader(sqlString))
            {
                while (rdr.Read())
                {
                    var i = 0;
                    var info = new AuxiliaryTableInfo(GetString(rdr, i++), GetString(rdr, i++), GetInt(rdr, i++), EAuxiliaryTableTypeUtils.GetEnumType(GetString(rdr, i++)), GetBool(rdr, i++), GetBool(rdr, i++), GetBool(rdr, i++), GetString(rdr, i));
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


		/// <summary>
		/// 获取表的使用次数，不抛出错误
		/// </summary>
		/// <param name="tableEnName"></param>
		/// <param name="tableType"></param>
		/// <returns></returns>
		public int GetTableUsedNum(string tableEnName, EAuxiliaryTableType tableType)
		{
			var count = 0;

            if (tableType == EAuxiliaryTableType.BackgroundContent)
            {
                string sqlString =
                    $"SELECT COUNT(*) FROM siteserver_PublishmentSystem WHERE (AuxiliaryTableForContent = '{PageUtils.FilterSql(tableEnName)}')";
                try
                {
                    count += BaiRongDataProvider.DatabaseDao.GetIntResult(sqlString);
                }
                catch
                {
                    // ignored
                }
            }

            string sqlString2 =
                $"SELECT COUNT(*) FROM bairong_ContentModel WHERE (TableName = '{PageUtils.FilterSql(tableEnName)}')";
            try
            {
                count += BaiRongDataProvider.DatabaseDao.GetIntResult(sqlString2);
            }
		    catch
		    {
		        // ignored
		    }

		    return count;
		}

        public bool IsTableExists(EAuxiliaryTableType tableType)
        {
            var isExists = false;

            string sqlString =
                $"SELECT TableENName FROM bairong_TableCollection WHERE AuxiliaryTableType = '{EAuxiliaryTableTypeUtils.GetValue(tableType)}' AND IsCreatedInDB = 'True'";
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

        public bool IsTableExists(string tableName)
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

        public string GetFirstTableNameByTableType(EAuxiliaryTableType tableType)
        {
            var tableName = string.Empty;

            string sqlString =
                $"SELECT TableENName FROM bairong_TableCollection WHERE AuxiliaryTableType = '{EAuxiliaryTableTypeUtils.GetValue(tableType)}' AND IsCreatedInDB = 'True'";

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

        public void CreateAllAuxiliaryTableIfNotExists()
        {
            //添加后台内容表
            if (!IsTableExists(EAuxiliaryTableType.BackgroundContent))
            {
                var tableName = EAuxiliaryTableTypeUtils.GetDefaultTableName(EAuxiliaryTableType.BackgroundContent);
                var tableInfo = new AuxiliaryTableInfo(tableName, "后台内容表", 0, EAuxiliaryTableType.BackgroundContent, false, false, true, string.Empty);
                BaiRongDataProvider.TableCollectionDao.Insert(tableInfo);
                BaiRongDataProvider.TableMetadataDao.CreateAuxiliaryTable(tableInfo.TableEnName);
            }
        }
	}
}
