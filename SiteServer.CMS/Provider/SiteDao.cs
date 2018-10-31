using System.Collections.Generic;
using System.Data;
using SiteServer.Utils;
using SiteServer.CMS.Core;
using SiteServer.CMS.Data;
using SiteServer.CMS.DataCache;
using SiteServer.CMS.Model;
using SiteServer.CMS.Plugin;
using SiteServer.CMS.Plugin.Impl;
using SiteServer.Plugin;
using SiteServer.Utils.Enumerations;

namespace SiteServer.CMS.Provider
{
    public class SiteDao : DataProviderBase
    {
        public override string TableName => "siteserver_Site";

        public override List<TableColumn> TableColumns => new List<TableColumn>
        {
            new TableColumn
            {
                AttributeName = nameof(SiteInfo.Id),
                DataType = DataType.Integer,
                IsPrimaryKey = true,
                IsIdentity = false
            },
            new TableColumn
            {
                AttributeName = nameof(SiteInfo.SiteName),
                DataType = DataType.VarChar,
                DataLength = 50
            },
            new TableColumn
            {
                AttributeName = nameof(SiteInfo.SiteDir),
                DataType = DataType.VarChar,
                DataLength = 50
            },
            new TableColumn
            {
                AttributeName = nameof(SiteInfo.TableName),
                DataType = DataType.VarChar,
                DataLength = 50
            },
            new TableColumn
            {
                AttributeName = nameof(SiteInfo.IsRoot),
                DataType = DataType.VarChar,
                DataLength = 18
            },
            new TableColumn
            {
                AttributeName = nameof(SiteInfo.ParentId),
                DataType = DataType.Integer
            },
            new TableColumn
            {
                AttributeName = nameof(SiteInfo.Taxis),
                DataType = DataType.Integer
            },
            new TableColumn
            {
                AttributeName = nameof(SiteInfo.SettingsXml),
                DataType = DataType.Text
            }
        };

        private const string ParmId = "@Id";
        private const string ParmSiteName = "@SiteName";
        private const string ParmSiteDir = "@SiteDir";
        private const string ParmTableName = "@TableName";
        private const string ParmIsRoot = "@IsRoot";
        private const string ParmParentId = "@ParentId";
        private const string ParmTaxis = "@Taxis";
        private const string ParmSettingsXml = "@SettingsXML";

        public void InsertWithTrans(SiteInfo info, IDbTransaction trans)
        {
            var sqlString = $"INSERT INTO {TableName} (Id, SiteName, SiteDir, TableName, IsRoot, ParentId, Taxis, SettingsXML) VALUES (@Id, @SiteName, @SiteDir, @TableName, @IsRoot, @ParentId, @Taxis, @SettingsXML)";

            //获取排序值
            var taxis = GetMaxTaxis() + 1;
            var insertParms = new IDataParameter[]
			{
				GetParameter(ParmId, DataType.Integer, info.Id),
				GetParameter(ParmSiteName, DataType.VarChar, 50, info.SiteName),
                GetParameter(ParmSiteDir, DataType.VarChar, 50, info.SiteDir),
                GetParameter(ParmTableName, DataType.VarChar, 50, info.TableName),
				GetParameter(ParmIsRoot, DataType.VarChar, 18, info.IsRoot.ToString()),
                GetParameter(ParmParentId, DataType.Integer, info.ParentId),
                GetParameter(ParmTaxis, DataType.Integer, taxis),
				GetParameter(ParmSettingsXml, DataType.Text, info.Additional.ToString())
			};

            ExecuteNonQuery(trans, sqlString, insertParms);
            SiteManager.ClearCache();
        }

        public void Delete(int siteId)
        {
            var siteInfo = SiteManager.GetSiteInfo(siteId);
            var list = ChannelManager.GetChannelIdList(siteId);
            DataProvider.TableStyleDao.Delete(list, siteInfo.TableName);

            DataProvider.TagDao.DeleteTags(siteId);

            DataProvider.ChannelDao.Delete(siteId, siteId);

            UpdateParentIdToZero(siteId);

            ExecuteNonQuery($"DELETE FROM siteserver_Site WHERE Id  = {siteId}");

            SiteManager.ClearCache();
            ChannelManager.RemoveCacheBySiteId(siteId);
            PermissionsImpl.ClearAllCache();
        }

        public void Update(SiteInfo info)
        {
            var sqlString = $"UPDATE {TableName} SET SiteName = @SiteName, SiteDir = @SiteDir, TableName = @TableName, IsRoot = @IsRoot, ParentId = @ParentId, Taxis = @Taxis, SettingsXML = @SettingsXML WHERE  Id = @Id";

            var updateParms = new IDataParameter[]
			{
				GetParameter(ParmSiteName, DataType.VarChar, 50, info.SiteName),
                GetParameter(ParmSiteDir, DataType.VarChar, 50, info.SiteDir),
                GetParameter(ParmTableName, DataType.VarChar, 50, info.TableName),
				GetParameter(ParmIsRoot, DataType.VarChar, 18, info.IsRoot.ToString()),
                GetParameter(ParmParentId, DataType.Integer, info.ParentId),
                GetParameter(ParmTaxis, DataType.Integer, info.Taxis),
				GetParameter(ParmSettingsXml, DataType.Text, info.Additional.ToString()),
				GetParameter(ParmId, DataType.Integer, info.Id)
			};

            if (info.IsRoot)
            {
                UpdateAllIsRoot();
            }

            ExecuteNonQuery(sqlString, updateParms);
            SiteManager.ClearCache();
        }

        public void UpdateTableName(int siteId, string tableName)
        {
            var sqlString = $"UPDATE {TableName} SET TableName = @TableName WHERE Id = @Id";

            var updateParms = new IDataParameter[]
            {
                GetParameter(ParmTableName, DataType.VarChar, 50, tableName),
                GetParameter(ParmId, DataType.Integer, siteId)
            };

            ExecuteNonQuery(sqlString, updateParms);
            SiteManager.ClearCache();
        }

        public void UpdateParentIdToZero(int parentId)
        {
            var sqlString = "UPDATE siteserver_Site SET ParentId = 0 WHERE ParentId = " + parentId;

            ExecuteNonQuery(sqlString);
            SiteManager.ClearCache();
        }

        public List<string> GetLowerSiteDirListThatNotIsRoot()
        {
            var list = new List<string>();

            var sqlString = $"SELECT SiteDir FROM {TableName} WHERE IsRoot = @IsRoot";

            var parms = new IDataParameter[]
			{
				GetParameter(ParmIsRoot, DataType.VarChar, 18, false.ToString())
			};

            using (var rdr = ExecuteReader(sqlString, parms))
            {
                while (rdr.Read())
                {
                    list.Add(GetString(rdr, 0).ToLower());
                }
                rdr.Close();
            }
            return list;
        }

        private void UpdateAllIsRoot()
        {
            var sqlString = $"UPDATE {TableName} SET IsRoot = @IsRoot";

            var updateParms = new IDataParameter[]
			{
				GetParameter(ParmIsRoot, DataType.VarChar, 18, false.ToString())
			};

            ExecuteNonQuery(sqlString, updateParms);
            SiteManager.ClearCache();
        }

        public List<KeyValuePair<int, SiteInfo>> GetSiteInfoKeyValuePairList()
        {
            var list = new List<KeyValuePair<int, SiteInfo>>();

            var siteInfoList = GetSiteInfoList();
            foreach (var siteInfo in siteInfoList)
            {
                var entry = new KeyValuePair<int, SiteInfo>(siteInfo.Id, siteInfo);
                list.Add(entry);
            }

            return list;
        }

        private List<SiteInfo> GetSiteInfoList()
        {
            var list = new List<SiteInfo>();

            var sqlString = $"SELECT Id, SiteName, SiteDir, TableName, IsRoot, ParentId, Taxis, SettingsXML FROM {TableName} ORDER BY Taxis, Id";

            using (var rdr = ExecuteReader(sqlString))
            {
                while (rdr.Read())
                {
                    var i = 0;
                    var siteInfo = new SiteInfo(GetInt(rdr, i++), GetString(rdr, i++), GetString(rdr, i++), GetString(rdr, i++), GetBool(rdr, i++), GetInt(rdr, i++), GetInt(rdr, i++), GetString(rdr, i));
                    list.Add(siteInfo);
                }
                rdr.Close();
            }
            return list;
        }

        public bool IsTableUsed(string tableName)
        {
            var parameters = new IDataParameter[]
            {
                GetParameter(ParmTableName, DataType.VarChar, 50, tableName)
            };

            const string sqlString = "SELECT COUNT(*) FROM siteserver_Site WHERE TableName = @TableName";
            var count = DataProvider.DatabaseDao.GetIntResult(sqlString, parameters);

            if (count > 0) return true;

            var contentModelPluginIdList = DataProvider.ChannelDao.GetContentModelPluginIdList();
            foreach (var pluginId in contentModelPluginIdList)
            {
                var service = PluginManager.GetService(pluginId);
                if (service != null && PluginContentTableManager.IsContentTable(service) && service.ContentTableName == tableName)
                {
                    return true;
                }
            }

            return false;
        }

        public int GetIdByIsRoot()
        {
            var siteId = 0;

            var sqlString = $"SELECT Id FROM {TableName} WHERE IsRoot = @IsRoot";

            var parms = new IDataParameter[]
			{
				GetParameter(ParmIsRoot, DataType.VarChar, 18, true.ToString())
			};

            using (var rdr = ExecuteReader(sqlString, parms))
            {
                if (rdr.Read())
                {
                    siteId = GetInt(rdr, 0);
                }
                rdr.Close();
            }
            return siteId;
        }

        public int GetIdBySiteDir(string siteDir)
        {
            var siteId = 0;

            var sqlString = $"SELECT Id FROM {TableName} WHERE SiteDir = @SiteDir";

            var parms = new IDataParameter[]
			{
				GetParameter(ParmSiteDir, DataType.VarChar, 50, siteDir)
			};

            using (var rdr = ExecuteReader(sqlString, parms))
            {
                if (rdr.Read())
                {
                    siteId = GetInt(rdr, 0);
                }
                rdr.Close();
            }
            return siteId;
        }

        /// <summary>
        /// 得到所有系统文件夹的列表，以小写表示。
        /// </summary>
        public List<string> GetLowerSiteDirList(int parentId)
        {
            var list = new List<string>();
            var sqlString = "SELECT SiteDir FROM siteserver_Site WHERE ParentId = " + parentId;

            using (var rdr = ExecuteReader(sqlString))
            {
                while (rdr.Read())
                {
                    list.Add(GetString(rdr, 0).ToLower());
                }
                rdr.Close();
            }

            return list;
        }

        public IDataReader GetStlDataSource(string siteName, string siteDir, int startNum, int totalNum, string whereString, EScopeType scopeType, string orderByString)
        {
            IDataReader ie = null;

            var sqlWhereString = string.Empty;

            SiteInfo siteInfo = null;
            if (!string.IsNullOrEmpty(siteName))
            {
                siteInfo = SiteManager.GetSiteInfoBySiteName(siteName);
            }
            else if (!string.IsNullOrEmpty(siteDir))
            {
                siteInfo = SiteManager.GetSiteInfoByDirectory(siteDir);
            }

            if (siteInfo != null)
            {
                sqlWhereString = $"WHERE (ParentId = {siteInfo.Id})";
            }
            else
            {
                if (scopeType == EScopeType.Children)
                {
                    sqlWhereString = "WHERE (ParentId = 0 AND IsRoot = 'False')";
                }
                else if (scopeType == EScopeType.Descendant)
                {
                    sqlWhereString = "WHERE (IsRoot = 'False')";
                }
            }

            if (!string.IsNullOrEmpty(whereString))
            {
                sqlWhereString = string.IsNullOrEmpty(sqlWhereString) ? $"WHERE ({whereString})" : $"{sqlWhereString} AND ({whereString})";
            }

            if (string.IsNullOrEmpty(orderByString) || StringUtils.EqualsIgnoreCase(orderByString, "default"))
            {
                orderByString = "ORDER BY IsRoot DESC, ParentId, Taxis DESC, Id";

                //var sqlSelect = DataProvider.DatabaseDao.GetSelectSqlString(TableName, startNum, totalNum, SqlUtils.Asterisk, sqlWhereString, orderByString);
                var sqlSelect = DataProvider.DatabaseDao.GetPageSqlString(TableName, SqlUtils.Asterisk, sqlWhereString, orderByString, startNum - 1, totalNum);

                ie = ExecuteReader(sqlSelect);
            }

            return ie;
        }

        private static int GetMaxTaxis()
        {
            const string sqlString = "SELECT MAX(Taxis) FROM siteserver_Site";
            return DataProvider.DatabaseDao.GetIntResult(sqlString);
        }
    }
}
