using System.Collections.Generic;
using System.Data;
using System.Linq;
using SiteServer.CMS.Apis;
using SiteServer.CMS.Caches;
using SiteServer.CMS.Database.Core;
using SiteServer.CMS.Database.Models;
using SiteServer.CMS.Plugin;
using SiteServer.CMS.Plugin.Impl;
using SiteServer.Utils;
using SiteServer.Utils.Enumerations;

namespace SiteServer.CMS.Database.Repositories
{
    public class SiteRepository : GenericRepository<SiteInfo>
    {
        private static class Attr
        {
            public const string Id = nameof(SiteInfo.Id);
            public const string SiteDir = nameof(SiteInfo.SiteDir);
            public const string TableName = nameof(SiteInfo.TableName);
            public const string IsRoot = "IsRoot";
            public const string ParentId = nameof(SiteInfo.ParentId);
            public const string Taxis = nameof(SiteInfo.Taxis);
        }

        public void Insert(SiteInfo siteInfo)
        {
            //var sqlString = $"INSERT INTO {TableName} (Id, SiteName, SiteDir, TableName, IsRoot, ParentId, Taxis, SettingsXML) VALUES (@Id, @SiteName, @SiteDir, @TableName, @IsRoot, @ParentId, @Taxis, @SettingsXML)";

            ////获取排序值
            //siteInfo.Taxis = GetMaxTaxis() + 1;
            //IDataParameter[] parameters =
            //{
            //    GetParameter(ParamId, siteInfo.Id),
            //    GetParameter(ParamSiteName, siteInfo.SiteName),
            //    GetParameter(ParamSiteDir, siteInfo.SiteDir),
            //    GetParameter(ParamTableName, siteInfo.TableName),
            //    GetParameter(ParamIsRoot, siteInfo.IsRoot.ToString()),
            //    GetParameter(ParamParentId, siteInfo.ParentId),
            //    GetParameter(ParamTaxis, taxis),
            //    GetParameter(ParamSettingsXml,siteInfo.ToString())
            //};

            //DatabaseApi.ExecuteNonQuery(WebConfigUtils.ConnectionString, sqlString, parameters);

            siteInfo.Taxis = GetMaxTaxis() + 1;
            InsertObject(siteInfo);

            SiteManager.ClearCache();
        }

        public void Delete(int siteId)
        {
            var siteInfo = SiteManager.GetSiteInfo(siteId);
            var list = ChannelManager.GetChannelIdList(siteId);
            DataProvider.TableStyle.Delete(list, siteInfo.TableName);

            DataProvider.Tag.DeleteTags(siteId);

            DataProvider.Channel.Delete(siteId, siteId);

            UpdateParentIdToZero(siteId);

            //DatabaseApi.ExecuteNonQuery(ConnectionString, $"DELETE FROM siteserver_Site WHERE Id  = {siteId}");

            DeleteById(siteId);

            SiteManager.ClearCache();
            ChannelManager.RemoveCacheBySiteId(siteId);
            PermissionsImpl.ClearAllCache();
        }

        public void Update(SiteInfo siteInfo)
        {
            //var sqlString = $"UPDATE {TableName} SET SiteName = @SiteName, SiteDir = @SiteDir, TableName = @TableName, IsRoot = @IsRoot, ParentId = @ParentId, Taxis = @Taxis, SettingsXML = @SettingsXML WHERE  Id = @Id";

            //IDataParameter[] parameters =
            //{
            //    GetParameter(ParamSiteName, siteInfo.SiteName),
            //    GetParameter(ParamSiteDir, siteInfo.SiteDir),
            //    GetParameter(ParamTableName, siteInfo.TableName),
            //    GetParameter(ParamIsRoot, siteInfo.IsRoot.ToString()),
            //    GetParameter(ParamParentId, siteInfo.ParentId),
            //    GetParameter(ParamTaxis, siteInfo.Taxis),
            //    GetParameter(ParamSettingsXml,siteInfo.ToString()),
            //    GetParameter(ParamId, siteInfo.Id)
            //};

            if (siteInfo.Root)
            {
                UpdateAllIsRoot();
            }

            //DatabaseApi.ExecuteNonQuery(ConnectionString, sqlString, parameters);

            UpdateObject(siteInfo);

            SiteManager.ClearCache();
        }

        public void UpdateTableName(int siteId, string tableName)
        {
            //var sqlString = $"UPDATE {TableName} SET TableName = @TableName WHERE Id = @Id";

            //IDataParameter[] parameters =
            //{
            //    GetParameter(ParamTableName, tableName),
            //    GetParameter(ParamId, siteId)
            //};

            //DatabaseApi.ExecuteNonQuery(ConnectionString, sqlString, parameters);
            
            UpdateAll(Q
                .Set(Attr.TableName, tableName)
                .Where(Attr.Id, siteId)
            );

            SiteManager.ClearCache();
        }

        public void UpdateParentIdToZero(int parentId)
        {
            //var sqlString = "UPDATE siteserver_Site SET ParentId = 0 WHERE ParentId = " + parentId;

            //DatabaseApi.ExecuteNonQuery(ConnectionString, sqlString);
            
            UpdateAll(Q
                .Set(Attr.ParentId, 0)
                .Where(Attr.ParentId, parentId)
            );

            SiteManager.ClearCache();
        }

        public IList<string> GetLowerSiteDirListThatNotIsRoot()
        {
            //var list = new List<string>();

            //var sqlString = $"SELECT SiteDir FROM {TableName} WHERE IsRoot = @IsRoot";

            //IDataParameter[] parameters =
            //{
            //    GetParameter(ParamIsRoot, false.ToString())
            //};

            //using (var rdr = DatabaseApi.ExecuteReader(ConnectionString, sqlString, parameters))
            //{
            //    while (rdr.Read())
            //    {
            //        list.Add(DatabaseApi.GetString(rdr, 0).ToLower());
            //    }
            //    rdr.Close();
            //}
            //return list;

            var list = GetValueList<string>(Q
                .Select(Attr.SiteDir)
                .WhereNot(Attr.IsRoot, true.ToString()));

            return list.Select(x => x.ToLower()).ToList();
        }

        private void UpdateAllIsRoot()
        {
            //var sqlString = $"UPDATE {TableName} SET IsRoot = @IsRoot";

            //IDataParameter[] parameters =
            //{
            //    GetParameter(ParamIsRoot, false.ToString())
            //};

            //DatabaseApi.ExecuteNonQuery(ConnectionString, sqlString, parameters);
            
            UpdateAll(Q
                .Set(Attr.IsRoot, false.ToString())
            );

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

        private IList<SiteInfo> GetSiteInfoList()
        {
            //var list = new List<SiteInfo>();

            //var sqlString = $"SELECT Id, SiteName, SiteDir, TableName, IsRoot, ParentId, Taxis, SettingsXML FROM {TableName} ORDER BY Taxis, Id";

            //using (var rdr = DatabaseApi.ExecuteReader(ConnectionString, sqlString))
            //{
            //    while (rdr.Read())
            //    {
            //        var i = 0;
            //        var siteInfo = new SiteInfo(DatabaseApi.GetInt(rdr, i++), DatabaseApi.GetString(rdr, i++), DatabaseApi.GetString(rdr, i++), DatabaseApi.GetString(rdr, i++), TranslateUtils.ToBool(DatabaseApi.GetString(rdr, i++)), DatabaseApi.GetInt(rdr, i++), DatabaseApi.GetInt(rdr, i++), DatabaseApi.GetString(rdr, i));
            //        list.Add(siteInfo);
            //    }
            //    rdr.Close();
            //}
            //return list;

            return GetObjectList(Q.OrderBy(Attr.Taxis, Attr.Id));
        }

        public bool IsTableUsed(string tableName)
        {
            //IDataParameter[] parameters =
            //{
            //    GetParameter(ParamTableName, tableName)
            //};

            //const string sqlString = "SELECT COUNT(*) FROM siteserver_Site WHERE TableName = @TableName";
            //var count = DatabaseApi.GetIntResult(sqlString, parameters);

            var count = Count(Q.Where(Attr.TableName, tableName));

            if (count > 0) return true;

            var contentModelPluginIdList = DataProvider.Channel.GetContentModelPluginIdList();
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
            //var siteId = 0;

            //var sqlString = $"SELECT Id FROM {TableName} WHERE IsRoot = @IsRoot";

            //IDataParameter[] parameters =
            //{
            //    GetParameter(ParamIsRoot, true.ToString())
            //};

            //using (var rdr = DatabaseApi.ExecuteReader(ConnectionString, sqlString, parameters))
            //{
            //    if (rdr.Read())
            //    {
            //        siteId = DatabaseApi.GetInt(rdr, 0);
            //    }
            //    rdr.Close();
            //}
            //return siteId;

            return GetValue<int>(Q
                .Select(Attr.Id)
                .Where(Attr.IsRoot, true.ToString()));
        }

        public int GetIdBySiteDir(string siteDir)
        {
            //var siteId = 0;

            //var sqlString = $"SELECT Id FROM {TableName} WHERE SiteDir = @SiteDir";

            //IDataParameter[] parameters =
            //{
            //    GetParameter(ParamSiteDir, siteDir)
            //};

            //using (var rdr = DatabaseApi.ExecuteReader(ConnectionString, sqlString, parameters))
            //{
            //    if (rdr.Read())
            //    {
            //        siteId = DatabaseApi.GetInt(rdr, 0);
            //    }
            //    rdr.Close();
            //}
            //return siteId;

            return GetValue<int>(Q
                .Select(Attr.Id)
                .Where(Attr.SiteDir, siteDir));
        }

        /// <summary>
        /// 得到所有系统文件夹的列表，以小写表示。
        /// </summary>
        public List<string> GetLowerSiteDirList(int parentId)
        {
            //var list = new List<string>();
            //var sqlString = "SELECT SiteDir FROM siteserver_Site WHERE ParentId = " + parentId;

            //using (var rdr = DatabaseApi.ExecuteReader(ConnectionString, sqlString))
            //{
            //    while (rdr.Read())
            //    {
            //        list.Add(DatabaseApi.GetString(rdr, 0).ToLower());
            //    }
            //    rdr.Close();
            //}

            //return list;

            return GetValueList<string>(Q
                    .Select(Attr.SiteDir)
                    .Where(Attr.ParentId, parentId))
                .Select(x => x.ToLower())
                .ToList();
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

                //var sqlSelect = DatabaseApi.Instance.GetSelectSqlString(TableName, startNum, totalNum, SqlUtils.Asterisk, sqlWhereString, orderByString);
                var sqlSelect = SqlDifferences.GetSqlString(TableName, null, sqlWhereString, orderByString, startNum - 1, totalNum);

                ie = DatabaseApi.Instance.ExecuteReader(WebConfigUtils.ConnectionString, sqlSelect);
            }

            return ie;
        }

        private int GetMaxTaxis()
        {
            //const string sqlString = "SELECT MAX(Taxis) FROM siteserver_Site";
            //return DatabaseApi.GetIntResult(sqlString);
            return Max(Attr.Taxis);
        }
    }
}


//using System.Collections.Generic;
//using System.Data;
//using SiteServer.CMS.Database.Caches;
//using SiteServer.CMS.Database.Core;
//using SiteServer.CMS.Database.Models;
//using SiteServer.CMS.Plugin;
//using SiteServer.CMS.Plugin.Impl;
//using SiteServer.Plugin;
//using SiteServer.Utils;
//using SiteServer.Utils.Enumerations;

//namespace SiteServer.CMS.Database.Repositories
//{
//    public class Site : DataProviderBase
//    {
//        public override string TableName => "siteserver_Site";

//        public override List<TableColumn> TableColumns => new List<TableColumn>
//        {
//            new TableColumn
//            {
//                AttributeName = nameof(SiteInfo.Id),
//                DataType = DataType.Integer,
//                IsPrimaryKey = true,
//                IsIdentity = false
//            },
//            new TableColumn
//            {
//                AttributeName = nameof(SiteInfo.SiteName),
//                DataType = DataType.VarChar
//            },
//            new TableColumn
//            {
//                AttributeName = nameof(SiteInfo.SiteDir),
//                DataType = DataType.VarChar
//            },
//            new TableColumn
//            {
//                AttributeName = nameof(SiteInfo.TableName),
//                DataType = DataType.VarChar
//            },
//            new TableColumn
//            {
//                AttributeName = nameof(SiteInfo.IsRoot),
//                DataType = DataType.VarChar
//            },
//            new TableColumn
//            {
//                AttributeName = nameof(SiteInfo.ParentId),
//                DataType = DataType.Integer
//            },
//            new TableColumn
//            {
//                AttributeName = nameof(SiteInfo.Taxis),
//                DataType = DataType.Integer
//            },
//            new TableColumn
//            {
//                AttributeName = nameof(SiteInfo.SettingsXml),
//                DataType = DataType.Text
//            }
//        };

//        private const string ParamId = "@Id";
//        private const string ParamSiteName = "@SiteName";
//        private const string ParamSiteDir = "@SiteDir";
//        private const string ParamTableName = "@TableName";
//        private const string ParamIsRoot = "@IsRoot";
//        private const string ParamParentId = "@ParentId";
//        private const string ParamTaxis = "@Taxis";
//        private const string ParamSettingsXml = "@SettingsXML";

//        public void InsertWithTrans(SiteInfo siteInfo, IDbTransaction trans)
//        {
//            var sqlString = $"INSERT INTO {TableName} (Id, SiteName, SiteDir, TableName, IsRoot, ParentId, Taxis, SettingsXML) VALUES (@Id, @SiteName, @SiteDir, @TableName, @IsRoot, @ParentId, @Taxis, @SettingsXML)";

//            //获取排序值
//            var taxis = GetMaxTaxis() + 1;
//            IDataParameter[] parameters =
//			{
//				GetParameter(ParamId, siteInfo.Id),
//				GetParameter(ParamSiteName, siteInfo.SiteName),
//                GetParameter(ParamSiteDir, siteInfo.SiteDir),
//                GetParameter(ParamTableName, siteInfo.TableName),
//				GetParameter(ParamIsRoot, siteInfo.IsRoot.ToString()),
//                GetParameter(ParamParentId, siteInfo.ParentId),
//                GetParameter(ParamTaxis, taxis),
//				GetParameter(ParamSettingsXml,siteInfo.ToString())
//			};

//            DatabaseApi.ExecuteNonQuery(trans, sqlString, parameters);
//            SiteManager.ClearCache();
//        }

//        public void DeleteById(int siteId)
//        {
//            var siteInfo = SiteManager.GetSiteInfo(siteId);
//            var list = ChannelManager.GetChannelIdList(siteId);
//            DataProvider.TableStyle.DeleteById(list, siteInfo.TableName);

//            DataProvider.Tag.DeleteTags(siteId);

//            DataProvider.Channel.DeleteById(siteId, siteId);

//            UpdateParentIdToZero(siteId);

//            DatabaseApi.ExecuteNonQuery(ConnectionString, $"DELETE FROM siteserver_Site WHERE Id  = {siteId}");

//            SiteManager.ClearCache();
//            ChannelManager.RemoveCacheBySiteId(siteId);
//            PermissionsImpl.ClearAllCache();
//        }

//        public void UpdateObject(SiteInfo siteInfo)
//        {
//            var sqlString = $"UPDATE {TableName} SET SiteName = @SiteName, SiteDir = @SiteDir, TableName = @TableName, IsRoot = @IsRoot, ParentId = @ParentId, Taxis = @Taxis, SettingsXML = @SettingsXML WHERE  Id = @Id";

//            IDataParameter[] parameters =
//			{
//				GetParameter(ParamSiteName, siteInfo.SiteName),
//                GetParameter(ParamSiteDir, siteInfo.SiteDir),
//                GetParameter(ParamTableName, siteInfo.TableName),
//				GetParameter(ParamIsRoot, siteInfo.IsRoot.ToString()),
//                GetParameter(ParamParentId, siteInfo.ParentId),
//                GetParameter(ParamTaxis, siteInfo.Taxis),
//				GetParameter(ParamSettingsXml,siteInfo.ToString()),
//				GetParameter(ParamId, siteInfo.Id)
//			};

//            if (siteInfo.IsRoot)
//            {
//                UpdateAllIsRoot();
//            }

//            DatabaseApi.ExecuteNonQuery(ConnectionString, sqlString, parameters);
//            SiteManager.ClearCache();
//        }

//        public void UpdateTableName(int siteId, string tableName)
//        {
//            var sqlString = $"UPDATE {TableName} SET TableName = @TableName WHERE Id = @Id";

//            IDataParameter[] parameters =
//            {
//                GetParameter(ParamTableName, tableName),
//                GetParameter(ParamId, siteId)
//            };

//            DatabaseApi.ExecuteNonQuery(ConnectionString, sqlString, parameters);
//            SiteManager.ClearCache();
//        }

//        public void UpdateParentIdToZero(int parentId)
//        {
//            var sqlString = "UPDATE siteserver_Site SET ParentId = 0 WHERE ParentId = " + parentId;

//            DatabaseApi.ExecuteNonQuery(ConnectionString, sqlString);
//            SiteManager.ClearCache();
//        }

//        public List<string> GetLowerSiteDirListThatNotIsRoot()
//        {
//            var list = new List<string>();

//            var sqlString = $"SELECT SiteDir FROM {TableName} WHERE IsRoot = @IsRoot";

//            IDataParameter[] parameters =
//			{
//				GetParameter(ParamIsRoot, false.ToString())
//			};

//            using (var rdr = DatabaseApi.ExecuteReader(ConnectionString, sqlString, parameters))
//            {
//                while (rdr.Read())
//                {
//                    list.Add(DatabaseApi.GetString(rdr, 0).ToLower());
//                }
//                rdr.Close();
//            }
//            return list;
//        }

//        private void UpdateAllIsRoot()
//        {
//            var sqlString = $"UPDATE {TableName} SET IsRoot = @IsRoot";

//            IDataParameter[] parameters =
//			{
//				GetParameter(ParamIsRoot, false.ToString())
//			};

//            DatabaseApi.ExecuteNonQuery(ConnectionString, sqlString, parameters);
//            SiteManager.ClearCache();
//        }

//        public List<KeyValuePair<int, SiteInfo>> GetSiteInfoKeyValuePairList()
//        {
//            var list = new List<KeyValuePair<int, SiteInfo>>();

//            var siteInfoList = GetSiteInfoList();
//            foreach (var siteInfo in siteInfoList)
//            {
//                var entry = new KeyValuePair<int, SiteInfo>(siteInfo.Id, siteInfo);
//                list.Add(entry);
//            }

//            return list;
//        }

//        private List<SiteInfo> GetSiteInfoList()
//        {
//            var list = new List<SiteInfo>();

//            var sqlString = $"SELECT Id, SiteName, SiteDir, TableName, IsRoot, ParentId, Taxis, SettingsXML FROM {TableName} ORDER BY Taxis, Id";

//            using (var rdr = DatabaseApi.ExecuteReader(ConnectionString, sqlString))
//            {
//                while (rdr.Read())
//                {
//                    var i = 0;
//                    var siteInfo = new SiteInfo(DatabaseApi.GetInt(rdr, i++), DatabaseApi.GetString(rdr, i++), DatabaseApi.GetString(rdr, i++), DatabaseApi.GetString(rdr, i++), TranslateUtils.ToBool(DatabaseApi.GetString(rdr, i++)), DatabaseApi.GetInt(rdr, i++), DatabaseApi.GetInt(rdr, i++), DatabaseApi.GetString(rdr, i));
//                    list.Add(siteInfo);
//                }
//                rdr.Close();
//            }
//            return list;
//        }

//        public bool IsTableUsed(string tableName)
//        {
//            IDataParameter[] parameters =
//            {
//                GetParameter(ParamTableName, tableName)
//            };

//            const string sqlString = "SELECT COUNT(*) FROM siteserver_Site WHERE TableName = @TableName";
//            var count = DatabaseApi.Instance.GetIntResult(sqlString, parameters);

//            if (count > 0) return true;

//            var contentModelPluginIdList = DataProvider.Channel.GetContentModelPluginIdList();
//            foreach (var pluginId in contentModelPluginIdList)
//            {
//                var service = PluginManager.GetService(pluginId);
//                if (service != null && PluginContentTableManager.IsContentTable(service) && service.ContentTableName == tableName)
//                {
//                    return true;
//                }
//            }

//            return false;
//        }

//        public int GetIdByIsRoot()
//        {
//            var siteId = 0;

//            var sqlString = $"SELECT Id FROM {TableName} WHERE IsRoot = @IsRoot";

//            IDataParameter[] parameters =
//			{
//				GetParameter(ParamIsRoot, true.ToString())
//			};

//            using (var rdr = DatabaseApi.ExecuteReader(ConnectionString, sqlString, parameters))
//            {
//                if (rdr.Read())
//                {
//                    siteId = DatabaseApi.GetInt(rdr, 0);
//                }
//                rdr.Close();
//            }
//            return siteId;
//        }

//        public int GetIdBySiteDir(string siteDir)
//        {
//            var siteId = 0;

//            var sqlString = $"SELECT Id FROM {TableName} WHERE SiteDir = @SiteDir";

//            IDataParameter[] parameters =
//			{
//				GetParameter(ParamSiteDir, siteDir)
//			};

//            using (var rdr = DatabaseApi.ExecuteReader(ConnectionString, sqlString, parameters))
//            {
//                if (rdr.Read())
//                {
//                    siteId = DatabaseApi.GetInt(rdr, 0);
//                }
//                rdr.Close();
//            }
//            return siteId;
//        }

//        /// <summary>
//        /// 得到所有系统文件夹的列表，以小写表示。
//        /// </summary>
//        public List<string> GetLowerSiteDirList(int parentId)
//        {
//            var list = new List<string>();
//            var sqlString = "SELECT SiteDir FROM siteserver_Site WHERE ParentId = " + parentId;

//            using (var rdr = DatabaseApi.ExecuteReader(ConnectionString, sqlString))
//            {
//                while (rdr.Read())
//                {
//                    list.Add(DatabaseApi.GetString(rdr, 0).ToLower());
//                }
//                rdr.Close();
//            }

//            return list;
//        }

//        public IDataReader GetStlDataSource(string siteName, string siteDir, int startNum, int totalNum, string whereString, EScopeType scopeType, string orderByString)
//        {
//            IDataReader ie = null;

//            var sqlWhereString = string.Empty;

//            SiteInfo siteInfo = null;
//            if (!string.IsNullOrEmpty(siteName))
//            {
//                siteInfo = SiteManager.GetSiteInfoBySiteName(siteName);
//            }
//            else if (!string.IsNullOrEmpty(siteDir))
//            {
//                siteInfo = SiteManager.GetSiteInfoByDirectory(siteDir);
//            }

//            if (siteInfo != null)
//            {
//                sqlWhereString = $"WHERE (ParentId = {siteInfo.Id})";
//            }
//            else
//            {
//                if (scopeType == EScopeType.Children)
//                {
//                    sqlWhereString = "WHERE (ParentId = 0 AND IsRoot = 'False')";
//                }
//                else if (scopeType == EScopeType.Descendant)
//                {
//                    sqlWhereString = "WHERE (IsRoot = 'False')";
//                }
//            }

//            if (!string.IsNullOrEmpty(whereString))
//            {
//                sqlWhereString = string.IsNullOrEmpty(sqlWhereString) ? $"WHERE ({whereString})" : $"{sqlWhereString} AND ({whereString})";
//            }

//            if (string.IsNullOrEmpty(orderByString) || StringUtils.EqualsIgnoreCase(orderByString, "default"))
//            {
//                orderByString = "ORDER BY IsRoot DESC, ParentId, Taxis DESC, Id";

//                //var sqlSelect = DatabaseApi.Instance.GetSelectSqlString(TableName, startNum, totalNum, SqlUtils.Asterisk, sqlWhereString, orderByString);
//                var sqlSelect = SqlDifferences.GetSqlString(TableName, null, sqlWhereString, orderByString, startNum - 1, totalNum);

//                ie = DatabaseApi.ExecuteReader(ConnectionString, sqlSelect);
//            }

//            return ie;
//        }

//        private static int GetMaxTaxis()
//        {
//            const string sqlString = "SELECT MAX(Taxis) FROM siteserver_Site";
//            return DatabaseApi.Instance.GetIntResult(sqlString);
//        }
//    }
//}
