using System.Collections.Generic;
using SS.CMS.Core.Cache;
using SS.CMS.Core.Common;
using SS.CMS.Core.Models;
using SS.CMS.Plugin.Data;
using SS.CMS.Utils;

namespace SS.CMS.Core.Repositories
{
    public class ConfigDao : IDatabaseDao
    {
        private readonly Repository<ConfigInfo> _repository;
        public ConfigDao()
        {
            _repository = new Repository<ConfigInfo>(AppSettings.DatabaseType, AppSettings.ConnectionString);
        }

        public string TableName => _repository.TableName;
        public List<TableColumn> TableColumns => _repository.TableColumns;

        private static class Attr
        {
            public const string Id = nameof(ConfigInfo.Id);
            public const string IsInitialized = "IsInitialized";
            public const string DatabaseVersion = nameof(ConfigInfo.DatabaseVersion);
            public const string UpdateDate = nameof(ConfigInfo.UpdateDate);
            public const string SystemConfig = "SystemConfig";
        }

        public int Insert(ConfigInfo configInfo)
        {
            configInfo.Id = _repository.Insert(configInfo);
            if (configInfo.Id > 0)
            {
                ConfigManager.IsChanged = true;
            }

            return configInfo.Id;
        }

        public bool Update(ConfigInfo configInfo)
        {
            var updated = _repository.Update(configInfo);
            if (updated)
            {
                ConfigManager.IsChanged = true;
            }

            return updated;
        }

        public bool IsInitialized()
        {
            try
            {
                var isInitialized = _repository.Get<string>(Q
                    .Select(Attr.IsInitialized)
                    .OrderBy(Attr.Id));

                return TranslateUtils.ToBool(isInitialized);
            }
            catch
            {
                // ignored
            }

            return false;
        }

        public ConfigInfo GetConfigInfo()
        {
            ConfigInfo info = null;

            try
            {
                info = _repository.Get(Q.OrderBy(Attr.Id));
            }
            catch
            {
                SystemManager.SyncSystemTables();

                info = _repository.Get(Q.OrderBy(Attr.Id));
            }

            return info;
        }
    }
}


//using System.Collections.Generic;
//using System.Data;
//using SiteServer.CMS.Database.Caches;
//using SiteServer.CMS.Database.Core;
//using SiteServer.CMS.Database.Models;
//using SiteServer.Plugin;
//using SiteServer.Utils;

//namespace SiteServer.CMS.Database.Repositories
//{
//    public class ConfigDao
//	{
//        public override string TableName => "siteserver_Config";

//        public override List<TableColumn> TableColumns => new List<TableColumn>
//        {
//            new TableColumn
//            {
//                AttributeName = nameof(ConfigInfo.Id),
//                DataType = DataType.Integer,
//                IsIdentity = true,
//                IsPrimaryKey = true
//            },
//            new TableColumn
//            {
//                AttributeName = nameof(ConfigInfo.IsInitialized),
//                DataType = DataType.VarChar
//            },
//            new TableColumn
//            {
//                AttributeName = nameof(ConfigInfo.DatabaseVersion),
//                DataType = DataType.VarChar
//            },
//            new TableColumn
//            {
//                AttributeName = nameof(ConfigInfo.UpdateDate),
//                DataType = DataType.DateTime
//            },
//            new TableColumn
//            {
//                AttributeName = nameof(ConfigInfo.SystemConfig),
//                DataType = DataType.Text
//            }
//        };

//        public void InsertObject(ConfigInfo info)
//        {
//            var sqlString =
//                $"INSERT INTO {TableName} ({nameof(ConfigInfo.IsInitialized)}, {nameof(ConfigInfo.DatabaseVersion)}, {nameof(ConfigInfo.UpdateDate)}, {nameof(ConfigInfo.SystemConfig)}) VALUES (@{nameof(ConfigInfo.IsInitialized)}, @{nameof(ConfigInfo.DatabaseVersion)}, @{nameof(ConfigInfo.UpdateDate)}, @{nameof(ConfigInfo.SystemConfig)})";

//            IDataParameter[] parameters =
//			{
//				GetParameter($"@{nameof(ConfigInfo.IsInitialized)}", info.IsInitialized.ToString()),
//				GetParameter($"@{nameof(ConfigInfo.DatabaseVersion)}", info.DatabaseVersion),
//                GetParameter($"@{nameof(ConfigInfo.UpdateDate)}",info.UpdateDate),
//                GetParameter($"@{nameof(ConfigInfo.SystemConfig)}",info.SystemConfigInfo.ToString())
//            };

//            DatabaseApi.ExecuteNonQuery(ConnectionString, sqlString, parameters);
//            ConfigManager.IsChanged = true;
//		}

//		public void UpdateObject(ConfigInfo info)
//		{
//		    var sqlString =
//                $"UPDATE {TableName} SET {nameof(ConfigInfo.IsInitialized)} = @{nameof(ConfigInfo.IsInitialized)}, {nameof(ConfigInfo.DatabaseVersion)}= @{nameof(ConfigInfo.DatabaseVersion)}, {nameof(ConfigInfo.UpdateDate)}= @{nameof(ConfigInfo.UpdateDate)}, {nameof(ConfigInfo.SystemConfig)}= @{nameof(ConfigInfo.SystemConfig)} WHERE {nameof(ConfigInfo.Id)} = @{nameof(ConfigInfo.Id)}";

//            IDataParameter[] parameters =
//			{
//				GetParameter($"@{nameof(ConfigInfo.IsInitialized)}", info.IsInitialized.ToString()),
//				GetParameter($"@{nameof(ConfigInfo.DatabaseVersion)}", info.DatabaseVersion),
//                GetParameter($"@{nameof(ConfigInfo.UpdateDate)}",info.UpdateDate),
//                GetParameter($"@{nameof(ConfigInfo.SystemConfig)}",info.SystemConfigInfo.ToString()),
//			    GetParameter($"@{nameof(ConfigInfo.Id)}", info.Id)
//            };

//		    DatabaseApi.ExecuteNonQuery(ConnectionString, sqlString, parameters);
//            ConfigManager.IsChanged = true;
//		}

//		public bool IsInitialized()
//		{
//            var isInitialized = false;

//			try
//			{
//                using (var rdr = DatabaseApi.ExecuteReader(ConnectionString, $"SELECT {nameof(ConfigInfo.IsInitialized)} FROM {TableName} ORDER BY {nameof(ConfigInfo.Id)}")) 
//				{
//					if (rdr.Read()) 
//					{
//                        isInitialized = TranslateUtils.ToBool(DatabaseApi.GetString(rdr, 0));
//					}
//					rdr.Close();
//				}
//			}
//		    catch
//		    {
//		        // ignored
//		    }

//		    return isInitialized;
//		}

//		public string GetDatabaseVersion()
//		{
//			var databaseVersion = string.Empty;

//			try
//			{
//				using (var rdr = DatabaseApi.ExecuteReader(ConnectionString, $"SELECT {nameof(ConfigInfo.DatabaseVersion)} FROM {TableName} ORDER BY {nameof(ConfigInfo.Id)}")) 
//				{
//					if (rdr.Read()) 
//					{
//                        databaseVersion = DatabaseApi.GetString(rdr, 0);
//					}
//					rdr.Close();
//				}
//			}
//		    catch
//		    {
//		        // ignored
//		    }

//		    return databaseVersion;
//		}

//		public ConfigInfo GetConfigInfo()
//		{
//            ConfigInfo info = null;

//		    using (var rdr = DatabaseApi.ExecuteReader(ConnectionString, $"SELECT {nameof(ConfigInfo.Id)}, {nameof(ConfigInfo.IsInitialized)}, {nameof(ConfigInfo.DatabaseVersion)}, {nameof(ConfigInfo.UpdateDate)}, {nameof(ConfigInfo.SystemConfig)} FROM {TableName} ORDER BY {nameof(ConfigInfo.Id)}"))
//            {
//                if (rdr.Read())
//                {
//                    var i = 0;
//                    info = new ConfigInfo(DatabaseApi.GetInt(rdr, i++), TranslateUtils.ToBool(DatabaseApi.GetString(rdr, i++)), DatabaseApi.GetString(rdr, i++), DatabaseApi.GetDateTime(rdr, i++), DatabaseApi.GetString(rdr, i));
//                }
//                rdr.Close();
//            }

//			return info;
//		}
//    }
//}
