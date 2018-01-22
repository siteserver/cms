using System.Collections;
using System.Collections.Generic;
using System.Data;
using SiteServer.CMS.Core;
using SiteServer.CMS.Data;
using SiteServer.CMS.Model;
using SiteServer.Utils.Model;
using SiteServer.Plugin;
using SiteServer.Utils;

namespace SiteServer.CMS.Provider
{
    public class TableMetadataDao : DataProviderBase
    {
        public override string TableName => "bairong_TableMetadata";

        public override List<TableColumnInfo> TableColumns => new List<TableColumnInfo>
        {
            new TableColumnInfo
            {
                ColumnName = nameof(TableMetadataInfo.TableMetadataId),
                DataType = DataType.Integer,
                IsIdentity = true,
                IsPrimaryKey = true
            },
            new TableColumnInfo
            {
                ColumnName = nameof(TableMetadataInfo.AuxiliaryTableEnName),
                DataType = DataType.VarChar,
                Length = 50
            },
            new TableColumnInfo
            {
                ColumnName = nameof(TableMetadataInfo.AttributeName),
                DataType = DataType.VarChar,
                Length = 50
            },
            new TableColumnInfo
            {
                ColumnName = nameof(TableMetadataInfo.DataType),
                DataType = DataType.VarChar,
                Length = 50
            },
            new TableColumnInfo
            {
                ColumnName = nameof(TableMetadataInfo.DataLength),
                DataType = DataType.Integer
            },
            new TableColumnInfo
            {
                ColumnName = nameof(TableMetadataInfo.Taxis),
                DataType = DataType.Integer
            },
            new TableColumnInfo
            {
                ColumnName = nameof(TableMetadataInfo.IsSystem),
                DataType = DataType.VarChar,
                Length = 18
            }
        };

        private const string SqlSelectTableMetadata = "SELECT TableMetadataID, AuxiliaryTableEnName, AttributeName, DataType, DataLength, Taxis, IsSystem FROM bairong_TableMetadata WHERE TableMetadataID = @TableMetadataID";

        private const string SqlSelectAllTableMetadataByEnname = "SELECT TableMetadataID, AuxiliaryTableEnName, AttributeName, DataType, DataLength, Taxis, IsSystem FROM bairong_TableMetadata WHERE AuxiliaryTableEnName = @AuxiliaryTableEnName ORDER BY IsSystem DESC, Taxis";

        private const string SqlSelectTableMetadataCountByEnname = "SELECT COUNT(*) FROM bairong_TableMetadata WHERE AuxiliaryTableEnName = @AuxiliaryTableEnName";

        private const string SqlSelectTableMetadataByTableEnnameAndAttributeName = "SELECT TableMetadataID, AuxiliaryTableEnName, AttributeName, DataType, DataLength, Taxis, IsSystem FROM bairong_TableMetadata WHERE AuxiliaryTableEnName = @AuxiliaryTableEnName AND AttributeName = @AttributeName";

        private const string SqlSelectTableMetadataIdByTableEnnameAndAttributeName = "SELECT TableMetadataID FROM bairong_TableMetadata WHERE AuxiliaryTableEnName = @AuxiliaryTableEnName AND AttributeName = @AttributeName";

        private const string SqlSelectTableMetadataAllAttributeName = "SELECT AttributeName FROM bairong_TableMetadata WHERE AuxiliaryTableEnName = @AuxiliaryTableEnName ORDER BY Taxis";

        private const string SqlUpdateTableMetadata = "UPDATE bairong_TableMetadata SET AuxiliaryTableEnName = @AuxiliaryTableEnName, AttributeName = @AttributeName, DataType = @DataType, DataLength = @DataLength, IsSystem = @IsSystem WHERE  TableMetadataID = @TableMetadataID";

        private const string SqlDeleteTableMetadata = "DELETE FROM bairong_TableMetadata WHERE  TableMetadataID = @TableMetadataID";

        private const string SqlDeleteTableMetadataByTableName = "DELETE FROM bairong_TableMetadata WHERE  AuxiliaryTableEnName = @AuxiliaryTableEnName";

        private const string SqlUpdateTableMetadataTaxis = "UPDATE bairong_TableMetadata SET Taxis = @Taxis WHERE  TableMetadataID = @TableMetadataID";

        private const string ParmTableMetadataId = "@TableMetadataID";
        private const string ParmTableCollectionInfoEnname = "@AuxiliaryTableEnName";
        private const string ParmAttributeName = "@AttributeName";
        private const string ParmDataType = "@DataType";
        private const string ParmDataLength = "@DataLength";
        private const string ParmTaxis = "@Taxis";
        private const string ParmIsSystem = "@IsSystem";

        public void Insert(TableMetadataInfo info)
        {
            if (IsExists(info.AuxiliaryTableEnName, info.AttributeName)) return;

            const string sqlString = "INSERT INTO bairong_TableMetadata (AuxiliaryTableEnName, AttributeName, DataType, DataLength, Taxis, IsSystem) VALUES (@AuxiliaryTableEnName, @AttributeName, @DataType, @DataLength, @Taxis, @IsSystem)";

            var parameters = new IDataParameter[]
			{
				GetParameter(ParmTableCollectionInfoEnname, DataType.VarChar, 50, info.AuxiliaryTableEnName),
				GetParameter(ParmAttributeName, DataType.VarChar, 50, info.AttributeName),
				GetParameter(ParmDataType, DataType.VarChar, 50, info.DataType.Value),
				GetParameter(ParmDataLength, DataType.Integer, info.DataLength),
				GetParameter(ParmTaxis, DataType.Integer, GetMaxTaxis(info.AuxiliaryTableEnName) + 1),
				GetParameter(ParmIsSystem, DataType.VarChar, 18, info.IsSystem.ToString())
			};

            ExecuteNonQuery(sqlString, parameters);

            DataProvider.TableCollectionDao.UpdateAttributeNum(info.AuxiliaryTableEnName);
            DataProvider.TableCollectionDao.UpdateIsChangedAfterCreatedInDbToTrue(info.AuxiliaryTableEnName);

            TableMetadataManager.ClearCache();
        }


        internal void InsertWithTransaction(TableMetadataInfo info, int taxis, IDbTransaction trans)
        {
            if (IsExistsWithTransaction(info.AuxiliaryTableEnName, info.AttributeName, trans)) return;

            const string sqlString = "INSERT INTO bairong_TableMetadata (AuxiliaryTableEnName, AttributeName, DataType, DataLength, Taxis, IsSystem) VALUES (@AuxiliaryTableEnName, @AttributeName, @DataType, @DataLength, @Taxis, @IsSystem)";

            var insertParms = new IDataParameter[]
		    {
			    GetParameter(ParmTableCollectionInfoEnname, DataType.VarChar, 50, info.AuxiliaryTableEnName),
			    GetParameter(ParmAttributeName, DataType.VarChar, 50, info.AttributeName),
			    GetParameter(ParmDataType, DataType.VarChar, 50, info.DataType.Value),
			    GetParameter(ParmDataLength, DataType.Integer, info.DataLength),
			    GetParameter(ParmTaxis, DataType.Integer, taxis),
			    GetParameter(ParmIsSystem, DataType.VarChar, 18, info.IsSystem.ToString())
		    };

            ExecuteNonQuery(trans, sqlString, insertParms);
            if (info.StyleInfo != null)
            {
                info.StyleInfo.TableName = info.AuxiliaryTableEnName;
                info.StyleInfo.AttributeName = info.AttributeName;
                DataProvider.TableStyleDao.InsertWithTransaction(info.StyleInfo, trans);
                TableStyleManager.IsChanged = true;
            }

            TableMetadataManager.ClearCache();
        }

        public void Update(TableMetadataInfo info)
        {
            var isSqlChanged = true;
            var originalInfo = GetTableMetadataInfo(info.TableMetadataId);
            if (originalInfo != null)
            {
                if (StringUtils.EqualsIgnoreCase(info.AuxiliaryTableEnName, originalInfo.AuxiliaryTableEnName)
                 && StringUtils.EqualsIgnoreCase(info.AttributeName, originalInfo.AttributeName)
                 && info.DataType == originalInfo.DataType
                 && info.DataLength == originalInfo.DataLength
                 && info.IsSystem == originalInfo.IsSystem)
                {
                    isSqlChanged = false;
                }
            }

            if (!isSqlChanged) return;

            var updateParms = new IDataParameter[]
            {
                GetParameter(ParmTableCollectionInfoEnname, DataType.VarChar, 50, info.AuxiliaryTableEnName),
                GetParameter(ParmAttributeName, DataType.VarChar, 50, info.AttributeName),
                GetParameter(ParmDataType, DataType.VarChar, 50, info.DataType.Value),
                GetParameter(ParmDataLength, DataType.Integer, info.DataLength),
                GetParameter(ParmIsSystem, DataType.VarChar, 18, info.IsSystem.ToString()),
                GetParameter(ParmTableMetadataId, DataType.Integer, info.TableMetadataId)
            };

            ExecuteNonQuery(SqlUpdateTableMetadata, updateParms);

            DataProvider.TableCollectionDao.UpdateIsChangedAfterCreatedInDbToTrue(info.AuxiliaryTableEnName);
            TableMetadataManager.ClearCache();
        }

        public void Delete(int tableMetadataId)
        {
            var parms = new IDataParameter[]
			{
				GetParameter(ParmTableMetadataId, DataType.Integer, tableMetadataId)
			};

            var metadataInfo = GetTableMetadataInfo(tableMetadataId);

            ExecuteNonQuery(SqlDeleteTableMetadata, parms);

            DataProvider.TableCollectionDao.UpdateAttributeNum(metadataInfo.AuxiliaryTableEnName);
            DataProvider.TableCollectionDao.UpdateIsChangedAfterCreatedInDbToTrue(metadataInfo.AuxiliaryTableEnName);
            TableMetadataManager.ClearCache();
        }

        public void Delete(string tableEnName)
        {
            Delete(tableEnName, null);
        }

        public void Delete(string tableEnName, IDbTransaction trans)
        {
            var parms = new IDataParameter[]
			{
				GetParameter(ParmTableCollectionInfoEnname, DataType.VarChar,50, tableEnName)
			};
            if (trans == null)
            {
                ExecuteNonQuery(SqlDeleteTableMetadataByTableName, parms);
                TableMetadataManager.ClearCache();
            }
            else
            {
                ExecuteNonQuery(trans, SqlDeleteTableMetadataByTableName, parms);
                TableMetadataManager.ClearCache();
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
				GetParameter(ParmTableCollectionInfoEnname, DataType.VarChar, 50, tableEnName),
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
				GetParameter(ParmTableCollectionInfoEnname, DataType.VarChar, 50, tableEnName),
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
				GetParameter(ParmTableCollectionInfoEnname, DataType.VarChar, 50, tableEnName)
			};

            var enumerable = (IEnumerable)ExecuteReader(SqlSelectAllTableMetadataByEnname, parms);
            return enumerable;
        }

        public List<TableMetadataInfo> GetTableMetadataInfoList(string tableEnName)
        {
            var list = new List<TableMetadataInfo>();

            var parms = new IDataParameter[]
			{
				GetParameter(ParmTableCollectionInfoEnname, DataType.VarChar, 50, tableEnName)
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

        public Dictionary<string, List<TableMetadataInfo>> GetTableNameWithTableMetadataInfoList()
        {
            var dict = new Dictionary<string, List<TableMetadataInfo>>();
            var tableNameList = DataProvider.TableCollectionDao.GetTableEnNameList();
            foreach (var tableName in tableNameList)
            {
                var list = GetTableMetadataInfoList(tableName);
                if (list != null && list.Count > 0)
                {
                    dict.Add(tableName, list);
                }
            }
            return dict;
        }

        /// <summary>
        /// Get Total TableCollectionInfo Count
        /// </summary>
        public int GetTableMetadataCountByEnName(string tableEnName)
        {
            var count = 0;

            var parms = new IDataParameter[]
			{
				GetParameter(ParmTableCollectionInfoEnname, DataType.VarChar, 50, tableEnName)
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

        public bool IsExists(string tableName, string attributeName)
        {
            var exists = false;

            const string sqlString = "SELECT TableMetadataID FROM bairong_TableMetadata WHERE AuxiliaryTableEnName = @AuxiliaryTableEnName AND AttributeName = @AttributeName";
            var parms = new IDataParameter[]
            {
                GetParameter(ParmTableCollectionInfoEnname, DataType.VarChar, 50, tableName),
                GetParameter(ParmAttributeName, DataType.VarChar, 50, attributeName)
            };

            using (var rdr = ExecuteReader(sqlString, parms))
            {
                if (rdr.Read())
                {
                    if (!rdr.IsDBNull(0))
                    {
                        exists = true;
                    }
                }
                rdr.Close();
            }
            return exists;
        }

        public bool IsExistsWithTransaction(string tableName, string attributeName, IDbTransaction trans)
        {
            var exists = false;

            const string sqlString = "SELECT TableMetadataID FROM bairong_TableMetadata WHERE AuxiliaryTableEnName = @AuxiliaryTableEnName AND AttributeName = @AttributeName";
            var parms = new IDataParameter[]
            {
                GetParameter(ParmTableCollectionInfoEnname, DataType.VarChar, 50, tableName),
                GetParameter(ParmAttributeName, DataType.VarChar, 50, attributeName)
            };

            using (var rdr = ExecuteReader(trans, sqlString, parms))
            {
                if (rdr.Read())
                {
                    if (!rdr.IsDBNull(0))
                    {
                        exists = true;
                    }
                }
                rdr.Close();
            }
            return exists;
        }

        public List<string> GetAttributeNameList(string tableEnName)
        {
            var parms = new IDataParameter[]
			{
				GetParameter(ParmTableCollectionInfoEnname, DataType.VarChar, 50, tableEnName)
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
            const string sqlString = "SELECT MAX(Taxis) FROM bairong_TableMetadata WHERE AuxiliaryTableEnName = @AuxiliaryTableEnName";

            var parms = new IDataParameter[]
			{
				GetParameter(ParmTableCollectionInfoEnname, DataType.VarChar, 50, tableEnName)
			};

            return DataProvider.DatabaseDao.GetIntResult(sqlString, parms);
        }


        /// <summary>
        /// Change The Texis To Higher Level
        /// </summary>
        public void TaxisUp(int selectedId, string tableEnName)
        {
            //Get Higher Taxis and ClassID
            //var sqlString = "SELECT TOP 1 TableMetadataID, Taxis FROM bairong_TableMetadata WHERE ((Taxis > (SELECT Taxis FROM bairong_TableMetadata WHERE (TableMetadataID = @TableMetadataID AND AuxiliaryTableEnName = @AuxiliaryTableEnName1))) AND AuxiliaryTableEnName=@AuxiliaryTableEnName2) ORDER BY Taxis";
            var sqlString = SqlUtils.ToTopSqlString("bairong_TableMetadata", "TableMetadataID, Taxis",
                "WHERE ((Taxis > (SELECT Taxis FROM bairong_TableMetadata WHERE (TableMetadataID = @TableMetadataID AND AuxiliaryTableEnName = @AuxiliaryTableEnName1))) AND AuxiliaryTableEnName=@AuxiliaryTableEnName2)",
                "ORDER BY Taxis",
                1);
            var higherId = 0;
            var higherTaxis = 0;

            var parms = new IDataParameter[]
			{
				GetParameter(ParmTableMetadataId, DataType.Integer, selectedId),
				GetParameter("@AuxiliaryTableEnName1", DataType.VarChar, 50, tableEnName),
				GetParameter("@AuxiliaryTableEnName2", DataType.VarChar, 50, tableEnName)
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
                //DataProvider.CreateTableCollectionInfoDAO().UpdateIsChangedAfterCreatedInDB(EBoolean.True, tableENName);
            }
        }

        /// <summary>
        /// Change The Texis To Lower Level
        /// </summary>
        public void TaxisDown(int selectedId, string tableEnName)
        {
            //Get Lower Taxis and ClassID
            //var sqlString = "SELECT TOP 1 TableMetadataID, Taxis FROM bairong_TableMetadata WHERE ((Taxis < (SELECT Taxis FROM bairong_TableMetadata WHERE (TableMetadataID = @TableMetadataID AND AuxiliaryTableEnName = @AuxiliaryTableEnName1))) AND AuxiliaryTableEnName = @AuxiliaryTableEnName2) ORDER BY Taxis DESC";
            var sqlString = SqlUtils.ToTopSqlString("bairong_TableMetadata", "TableMetadataID, Taxis",
                "WHERE ((Taxis < (SELECT Taxis FROM bairong_TableMetadata WHERE (TableMetadataID = @TableMetadataID AND AuxiliaryTableEnName = @AuxiliaryTableEnName1))) AND AuxiliaryTableEnName = @AuxiliaryTableEnName2)",
                "ORDER BY Taxis DESC", 1);

            var lowerId = 0;
            var lowerTaxis = 0;

            var parms = new IDataParameter[]
			{
				GetParameter(ParmTableMetadataId, DataType.Integer, selectedId),
				GetParameter("@AuxiliaryTableEnName1", DataType.VarChar, 50, tableEnName),
				GetParameter("@AuxiliaryTableEnName2", DataType.VarChar, 50, tableEnName)
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
                //DataProvider.CreateTableCollectionInfoDAO().UpdateIsChangedAfterCreatedInDB(EBoolean.True, tableENName);
            }
        }

        private int GetTaxis(int selectedId)
        {
            const string sqlString = "SELECT Taxis FROM bairong_TableMetadata WHERE (TableMetadataID = @TableMetadataID)";
            var taxis = 0;

            var parms = new IDataParameter[]
			{
				GetParameter(ParmTableMetadataId, DataType.Integer, selectedId)
			};

            using (var rdr = ExecuteReader(sqlString, parms))
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
            TableMetadataManager.ClearCache();
        }

        public List<string> GetAddColumnsSqlString(string tableName, TableMetadataInfo metadataInfo)
        {
            var sqlList = new List<string>();
            var columnSqlString = SqlUtils.GetColumnSqlString(metadataInfo.DataType, metadataInfo.AttributeName, metadataInfo.DataLength);
            var alterSqlString = SqlUtils.GetAddColumnsSqlString(tableName, columnSqlString);
            sqlList.Add(alterSqlString);
            return sqlList;
        }

        public List<TableMetadataInfo> GetDefaultTableMetadataInfoList(string tableName)
        {
            var list = new List<TableMetadataInfo>();

            var metadataInfo = new TableMetadataInfo(0, tableName, BackgroundContentAttribute.SubTitle, DataType.VarChar, 255, 0, true);
            list.Add(metadataInfo);
            metadataInfo = new TableMetadataInfo(0, tableName, BackgroundContentAttribute.ImageUrl, DataType.VarChar, 200, 0, true);
            list.Add(metadataInfo);
            metadataInfo = new TableMetadataInfo(0, tableName, BackgroundContentAttribute.VideoUrl, DataType.VarChar, 200, 0, true);
            list.Add(metadataInfo);
            metadataInfo = new TableMetadataInfo(0, tableName, BackgroundContentAttribute.FileUrl, DataType.VarChar, 200, 0, true);
            list.Add(metadataInfo);
            metadataInfo = new TableMetadataInfo(0, tableName, BackgroundContentAttribute.Content, DataType.Text, 16, 0, true);
            list.Add(metadataInfo);
            metadataInfo = new TableMetadataInfo(0, tableName, BackgroundContentAttribute.Summary, DataType.Text, 16, 0, true);
            list.Add(metadataInfo);
            metadataInfo = new TableMetadataInfo(0, tableName, BackgroundContentAttribute.Author, DataType.VarChar, 255, 0, true);
            list.Add(metadataInfo);
            metadataInfo = new TableMetadataInfo(0, tableName, BackgroundContentAttribute.Source, DataType.VarChar, 255, 0, true);
            list.Add(metadataInfo);
            return list;
        }
    }
}
