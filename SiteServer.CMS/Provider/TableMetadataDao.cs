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
    public class TableMetadataDao : DataProviderBase
    {
        public override string TableName => "siteserver_TableMetadata";

        public override List<TableColumn> TableColumns => new List<TableColumn>
        {
            new TableColumn
            {
                AttributeName = nameof(TableMetadataInfo.Id),
                DataType = DataType.Integer,
                IsIdentity = true,
                IsPrimaryKey = true
            },
            new TableColumn
            {
                AttributeName = nameof(TableMetadataInfo.TableName),
                DataType = DataType.VarChar,
                DataLength = 50
            },
            new TableColumn
            {
                AttributeName = nameof(TableMetadataInfo.AttributeName),
                DataType = DataType.VarChar,
                DataLength = 50
            },
            new TableColumn
            {
                AttributeName = nameof(TableMetadataInfo.DataType),
                DataType = DataType.VarChar,
                DataLength = 50
            },
            new TableColumn
            {
                AttributeName = nameof(TableMetadataInfo.DataLength),
                DataType = DataType.Integer
            },
            new TableColumn
            {
                AttributeName = nameof(TableMetadataInfo.Taxis),
                DataType = DataType.Integer
            },
            new TableColumn
            {
                AttributeName = nameof(TableMetadataInfo.IsSystem),
                DataType = DataType.VarChar,
                DataLength = 18
            }
        };

        private const string SqlSelectTableMetadata = "SELECT Id, TableName, AttributeName, DataType, DataLength, Taxis, IsSystem FROM siteserver_TableMetadata WHERE Id = @Id";

        private const string SqlSelectAllTableMetadataByEnname = "SELECT Id, TableName, AttributeName, DataType, DataLength, Taxis, IsSystem FROM siteserver_TableMetadata WHERE TableName = @TableName ORDER BY IsSystem DESC, Taxis";

        private const string SqlSelectTableMetadataCountByEnname = "SELECT COUNT(*) FROM siteserver_TableMetadata WHERE TableName = @TableName";

        private const string SqlSelectTableMetadataByTableNameAndAttributeName = "SELECT Id, TableName, AttributeName, DataType, DataLength, Taxis, IsSystem FROM siteserver_TableMetadata WHERE TableName = @TableName AND AttributeName = @AttributeName";

        private const string SqlSelectIdByTableNameAndAttributeName = "SELECT Id FROM siteserver_TableMetadata WHERE TableName = @TableName AND AttributeName = @AttributeName";

        private const string SqlSelectTableMetadataAllAttributeName = "SELECT AttributeName FROM siteserver_TableMetadata WHERE TableName = @TableName ORDER BY Taxis";

        private const string SqlUpdateTableMetadata = "UPDATE siteserver_TableMetadata SET TableName = @TableName, AttributeName = @AttributeName, DataType = @DataType, DataLength = @DataLength, IsSystem = @IsSystem WHERE  Id = @Id";

        private const string SqlDeleteTableMetadata = "DELETE FROM siteserver_TableMetadata WHERE  Id = @Id";

        private const string SqlDeleteTableMetadataByTableName = "DELETE FROM siteserver_TableMetadata WHERE  TableName = @TableName";

        private const string SqlUpdateTableMetadataTaxis = "UPDATE siteserver_TableMetadata SET Taxis = @Taxis WHERE  Id = @Id";

        private const string ParmId = "@Id";
        private const string ParmTableCollectionInfoEnname = "@TableName";
        private const string ParmAttributeName = "@AttributeName";
        private const string ParmDataType = "@DataType";
        private const string ParmDataLength = "@DataLength";
        private const string ParmTaxis = "@Taxis";
        private const string ParmIsSystem = "@IsSystem";

        public void Insert(TableMetadataInfo info)
        {
            if (IsExists(info.TableName, info.AttributeName)) return;

            const string sqlString = "INSERT INTO siteserver_TableMetadata (TableName, AttributeName, DataType, DataLength, Taxis, IsSystem) VALUES (@TableName, @AttributeName, @DataType, @DataLength, @Taxis, @IsSystem)";

            var parameters = new IDataParameter[]
			{
				GetParameter(ParmTableCollectionInfoEnname, DataType.VarChar, 50, info.TableName),
				GetParameter(ParmAttributeName, DataType.VarChar, 50, info.AttributeName),
				GetParameter(ParmDataType, DataType.VarChar, 50, info.DataType.Value),
				GetParameter(ParmDataLength, DataType.Integer, info.DataLength),
				GetParameter(ParmTaxis, DataType.Integer, GetMaxTaxis(info.TableName) + 1),
				GetParameter(ParmIsSystem, DataType.VarChar, 18, info.IsSystem.ToString())
			};

            ExecuteNonQuery(sqlString, parameters);

            DataProvider.TableDao.UpdateAttributeNum(info.TableName);
            DataProvider.TableDao.UpdateIsChangedAfterCreatedInDbToTrue(info.TableName);

            TableMetadataManager.ClearCache();
        }


        internal void InsertWithTransaction(TableMetadataInfo info, int taxis, IDbTransaction trans)
        {
            if (IsExistsWithTransaction(info.TableName, info.AttributeName, trans)) return;

            const string sqlString = "INSERT INTO siteserver_TableMetadata (TableName, AttributeName, DataType, DataLength, Taxis, IsSystem) VALUES (@TableName, @AttributeName, @DataType, @DataLength, @Taxis, @IsSystem)";

            var insertParms = new IDataParameter[]
		    {
			    GetParameter(ParmTableCollectionInfoEnname, DataType.VarChar, 50, info.TableName),
			    GetParameter(ParmAttributeName, DataType.VarChar, 50, info.AttributeName),
			    GetParameter(ParmDataType, DataType.VarChar, 50, info.DataType.Value),
			    GetParameter(ParmDataLength, DataType.Integer, info.DataLength),
			    GetParameter(ParmTaxis, DataType.Integer, taxis),
			    GetParameter(ParmIsSystem, DataType.VarChar, 18, info.IsSystem.ToString())
		    };

            ExecuteNonQuery(trans, sqlString, insertParms);
            if (info.StyleInfo != null)
            {
                info.StyleInfo.TableName = info.TableName;
                info.StyleInfo.AttributeName = info.AttributeName;
                DataProvider.TableStyleDao.InsertWithTransaction(info.StyleInfo, trans);
                TableStyleManager.IsChanged = true;
            }

            TableMetadataManager.ClearCache();
        }

        public void Update(TableMetadataInfo info)
        {
            var isSqlChanged = true;
            var originalInfo = GetTableMetadataInfo(info.Id);
            if (originalInfo != null)
            {
                if (StringUtils.EqualsIgnoreCase(info.TableName, originalInfo.TableName)
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
                GetParameter(ParmTableCollectionInfoEnname, DataType.VarChar, 50, info.TableName),
                GetParameter(ParmAttributeName, DataType.VarChar, 50, info.AttributeName),
                GetParameter(ParmDataType, DataType.VarChar, 50, info.DataType.Value),
                GetParameter(ParmDataLength, DataType.Integer, info.DataLength),
                GetParameter(ParmIsSystem, DataType.VarChar, 18, info.IsSystem.ToString()),
                GetParameter(ParmId, DataType.Integer, info.Id)
            };

            ExecuteNonQuery(SqlUpdateTableMetadata, updateParms);

            DataProvider.TableDao.UpdateIsChangedAfterCreatedInDbToTrue(info.TableName);
            TableMetadataManager.ClearCache();
        }

        public void Delete(int id)
        {
            var parms = new IDataParameter[]
			{
				GetParameter(ParmId, DataType.Integer, id)
			};

            var metadataInfo = GetTableMetadataInfo(id);

            ExecuteNonQuery(SqlDeleteTableMetadata, parms);

            DataProvider.TableDao.UpdateAttributeNum(metadataInfo.TableName);
            DataProvider.TableDao.UpdateIsChangedAfterCreatedInDbToTrue(metadataInfo.TableName);
            TableMetadataManager.ClearCache();
        }

        public void Delete(string tableName)
        {
            Delete(tableName, null);
        }

        public void Delete(string tableName, IDbTransaction trans)
        {
            var parms = new IDataParameter[]
			{
				GetParameter(ParmTableCollectionInfoEnname, DataType.VarChar,50, tableName)
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

        public TableMetadataInfo GetTableMetadataInfo(int id)
        {
            TableMetadataInfo info = null;

            var parms = new IDataParameter[]
			{
				GetParameter(ParmId, DataType.Integer, id)
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

        public TableMetadataInfo GetTableMetadataInfo(string tableName, string attributeName)
        {
            TableMetadataInfo info = null;

            var parms = new IDataParameter[]
			{
				GetParameter(ParmTableCollectionInfoEnname, DataType.VarChar, 50, tableName),
				GetParameter(ParmAttributeName, DataType.VarChar, 50, attributeName)
			};

            using (var rdr = ExecuteReader(SqlSelectTableMetadataByTableNameAndAttributeName, parms))
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

        public int GetId(string tableName, string attributeName)
        {
            var id = 0;

            var parms = new IDataParameter[]
			{
				GetParameter(ParmTableCollectionInfoEnname, DataType.VarChar, 50, tableName),
				GetParameter(ParmAttributeName, DataType.VarChar, 50, attributeName)
			};

            using (var conn = GetConnection())
            {
                conn.Open();
                using (var rdr = ExecuteReader(conn, SqlSelectIdByTableNameAndAttributeName, parms))
                {
                    if (rdr.Read())
                    {
                        id = GetInt(rdr, 0);
                    }
                    rdr.Close();
                }
            }

            return id;
        }

        public IDataReader GetDataSource(string tableName)
        {
            var parms = new IDataParameter[]
			{
				GetParameter(ParmTableCollectionInfoEnname, DataType.VarChar, 50, tableName)
			};

            var enumerable = ExecuteReader(SqlSelectAllTableMetadataByEnname, parms);
            return enumerable;
        }

        public List<TableMetadataInfo> GetTableMetadataInfoList(string tableName)
        {
            var list = new List<TableMetadataInfo>();

            var parms = new IDataParameter[]
			{
				GetParameter(ParmTableCollectionInfoEnname, DataType.VarChar, 50, tableName)
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
            var tableNameList = DataProvider.TableDao.GetTableNameList();
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
        public int GetTableMetadataCountByEnName(string tableName)
        {
            var count = 0;

            var parms = new IDataParameter[]
			{
				GetParameter(ParmTableCollectionInfoEnname, DataType.VarChar, 50, tableName)
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

            const string sqlString = "SELECT Id FROM siteserver_TableMetadata WHERE TableName = @TableName AND AttributeName = @AttributeName";
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

            const string sqlString = "SELECT Id FROM siteserver_TableMetadata WHERE TableName = @TableName AND AttributeName = @AttributeName";
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

        public List<string> GetAttributeNameList(string tableName)
        {
            var parms = new IDataParameter[]
			{
				GetParameter(ParmTableCollectionInfoEnname, DataType.VarChar, 50, tableName)
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
        public int GetMaxTaxis(string tableName)
        {
            const string sqlString = "SELECT MAX(Taxis) FROM siteserver_TableMetadata WHERE TableName = @TableName";

            var parms = new IDataParameter[]
			{
				GetParameter(ParmTableCollectionInfoEnname, DataType.VarChar, 50, tableName)
			};

            return DataProvider.DatabaseDao.GetIntResult(sqlString, parms);
        }


        /// <summary>
        /// Change The Texis To Higher Level
        /// </summary>
        public void TaxisUp(int selectedId, string tableName)
        {
            //Get Higher Taxis and ClassID
            //var sqlString = "SELECT TOP 1 Id, Taxis FROM siteserver_TableMetadata WHERE ((Taxis > (SELECT Taxis FROM siteserver_TableMetadata WHERE (Id = @Id AND TableName = @TableName1))) AND TableName=@TableName2) ORDER BY Taxis";
            var sqlString = SqlUtils.ToTopSqlString("siteserver_TableMetadata", "Id, Taxis",
                "WHERE ((Taxis > (SELECT Taxis FROM siteserver_TableMetadata WHERE (Id = @Id AND TableName = @TableName1))) AND TableName=@TableName2)",
                "ORDER BY Taxis",
                1);
            var higherId = 0;
            var higherTaxis = 0;

            var parms = new IDataParameter[]
			{
				GetParameter(ParmId, DataType.Integer, selectedId),
				GetParameter("@TableName1", DataType.VarChar, 50, tableName),
				GetParameter("@TableName2", DataType.VarChar, 50, tableName)
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
                //DataProvider.CreateTableCollectionInfoDAO().UpdateIsChangedAfterCreatedInDB(EBoolean.True, tableName);
            }
        }

        /// <summary>
        /// Change The Texis To Lower Level
        /// </summary>
        public void TaxisDown(int selectedId, string tableName)
        {
            //Get Lower Taxis and ClassID
            //var sqlString = "SELECT TOP 1 Id, Taxis FROM siteserver_TableMetadata WHERE ((Taxis < (SELECT Taxis FROM siteserver_TableMetadata WHERE (Id = @Id AND TableName = @TableName1))) AND TableName = @TableName2) ORDER BY Taxis DESC";
            var sqlString = SqlUtils.ToTopSqlString("siteserver_TableMetadata", "Id, Taxis",
                "WHERE ((Taxis < (SELECT Taxis FROM siteserver_TableMetadata WHERE (Id = @Id AND TableName = @TableName1))) AND TableName = @TableName2)",
                "ORDER BY Taxis DESC", 1);

            var lowerId = 0;
            var lowerTaxis = 0;

            var parms = new IDataParameter[]
			{
				GetParameter(ParmId, DataType.Integer, selectedId),
				GetParameter("@TableName1", DataType.VarChar, 50, tableName),
				GetParameter("@TableName2", DataType.VarChar, 50, tableName)
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
                //DataProvider.CreateTableCollectionInfoDAO().UpdateIsChangedAfterCreatedInDB(EBoolean.True, tableName);
            }
        }

        private int GetTaxis(int selectedId)
        {
            const string sqlString = "SELECT Taxis FROM siteserver_TableMetadata WHERE (Id = @Id)";
            var taxis = 0;

            var parms = new IDataParameter[]
			{
				GetParameter(ParmId, DataType.Integer, selectedId)
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
				GetParameter(ParmId, DataType.Integer, id)
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
