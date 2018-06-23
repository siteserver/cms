using System;
using System.Collections.Generic;
using System.Data;
using SiteServer.Utils;
using SiteServer.CMS.Core;
using SiteServer.CMS.Data;
using SiteServer.CMS.Model;
using SiteServer.CMS.Plugin;
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
            ChannelManager.RemoveCache(siteId);
            PermissionManager.ClearAllCache();
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

        public List<int> GetIdListByParent(int parentId)
        {
            var list = new List<int>();

            var sqlString = $"SELECT Id FROM {TableName} WHERE ParentId = @ParentId";

            var parms = new IDataParameter[]
			{
				GetParameter(ParmParentId, DataType.Integer, parentId)
			};

            using (var rdr = ExecuteReader(sqlString, parms))
            {
                while (rdr.Read())
                {
                    list.Add(GetInt(rdr, 0));
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

        protected List<int> GetIdList(DateTime sinceDate)
        {
            var list = new List<int>();

            string sqlString =
                $"SELECT p.Id FROM {TableName} p INNER JOIN {DataProvider.ChannelDao.TableName} n ON (p.Id = n.{nameof(ChannelInfo.Id)} AND (n.AddDate BETWEEN {SqlUtils.GetComparableDate(sinceDate)} AND {SqlUtils.GetComparableNow()})) ORDER BY p.IsRoot DESC, p.ParentId, p.Taxis DESC, n.{nameof(ChannelInfo.Id)}";

            using (var rdr = ExecuteReader(sqlString))
            {
                while (rdr.Read())
                {
                    list.Add(GetInt(rdr, 0));
                }
                rdr.Close();
            }
            return list;
        }

        private List<SiteInfo> GetSiteInfoList()
        {
            var list = new List<SiteInfo>();

            var sqlString = $"SELECT Id, SiteName, SiteDir, TableName, IsRoot, ParentId, Taxis, SettingsXML FROM {TableName} ORDER BY Taxis";

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

        public int GetSiteCount()
        {
            var count = 0;

            var sqlString = $"SELECT Count(*) FROM {TableName}";

            using (var rdr = ExecuteReader(sqlString))
            {
                if (rdr.Read())
                {
                    count = GetInt(rdr, 0);
                }
                rdr.Close();
            }
            return count;
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

        public IDataReader GetStlDataSource(string siteName, string siteDir, int startNum, int totalNum, string whereString, EScopeType scopeType, string orderByString, string since)
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

                var sqlSelect = DataProvider.DatabaseDao.GetSelectSqlString(TableName, startNum, totalNum, SqlUtils.Asterisk, sqlWhereString, orderByString);

                ie = ExecuteReader(sqlSelect);
            }

            return ie;
        }

        public bool UpdateTaxisToDown(int siteId)
        {
            SetTaxisNotZero();
            //var sbSql = new StringBuilder();
            //sbSql.AppendFormat("SELECT TOP 1 Id, Taxis FROM siteserver_Site ");
            //sbSql.AppendFormat(" WHERE Taxis > (SELECT Taxis FROM siteserver_Site WHERE Id = {0}) ", siteId);
            //sbSql.AppendFormat(" ORDER BY Taxis ");

            var sqlString = SqlUtils.ToTopSqlString("siteserver_Site", "Id, Taxis", $"WHERE Taxis > (SELECT Taxis FROM siteserver_Site WHERE Id = {siteId})", "ORDER BY Taxis", 1);

            var lowerId = 0;
            var lowerTaxis = 0;

            using (var reader = ExecuteReader(sqlString))
            {
                if (reader.Read())
                {
                    lowerId = Convert.ToInt32(reader[0]);
                    lowerTaxis = Convert.ToInt32(reader[1]);
                }
                reader.Close();
            }

            var selectedTaxis = GetTaxis(siteId);
            if (lowerId == 0) return false;

            SetTaxis(siteId, lowerTaxis);
            SetTaxis(lowerId, selectedTaxis);

            SiteManager.ClearCache();

            return true;
        }

        public bool UpdateTaxisToUp(int siteId)
        {
            SetTaxisNotZero();
            //var sbSql = new StringBuilder();
            //sbSql.AppendFormat("SELECT TOP 1 Id, Taxis FROM siteserver_Site ");
            //sbSql.AppendFormat(" WHERE Taxis < (SELECT Taxis FROM siteserver_Site WHERE Id = {0}) ", siteId);
            //sbSql.AppendFormat(" ORDER BY Taxis DESC");

            var sqlString = SqlUtils.ToTopSqlString("siteserver_Site", "Id, Taxis", $"WHERE Taxis < (SELECT Taxis FROM siteserver_Site WHERE Id = {siteId})", "ORDER BY Taxis DESC", 1);

            var higherId = 0;
            var higherTaxis = 0;

            using (var reader = ExecuteReader(sqlString))
            {
                if (reader.Read())
                {
                    higherId = Convert.ToInt32(reader[0]);
                    higherTaxis = Convert.ToInt32(reader[1]);
                }
                reader.Close();
            }

            var selectedTaxis = GetTaxis(siteId);
            if (higherId == 0) return false;

            SetTaxis(siteId, higherTaxis);
            SetTaxis(higherId, selectedTaxis);

            SiteManager.ClearCache();

            return true;
        }

        private void SetTaxis(int siteId, int taxis)
        {
            ExecuteNonQuery($"UPDATE siteserver_Site SET Taxis = {taxis} WHERE Id = {siteId}");
        }

        private int GetTaxis(int siteId)
        {
            var taxis = 0;
            using (var reader = ExecuteReader($"SELECT Taxis FROM siteserver_Site WHERE Id = {siteId}"))
            {
                if (reader.Read())
                {
                    taxis = Convert.ToInt32(reader[0]);
                }
                reader.Close();
            }
            return taxis;
        }

        private static int GetMaxTaxis()
        {
            const string sqlString = "SELECT MAX(Taxis) FROM siteserver_Site";
            return DataProvider.DatabaseDao.GetIntResult(sqlString);
        }

        private void SetTaxisNotZero()
        {
            const string sqlString = @"UPDATE siteserver_Site SET Taxis = Id where Taxis = 0";
            ExecuteNonQuery(sqlString);
        }
    }
}
