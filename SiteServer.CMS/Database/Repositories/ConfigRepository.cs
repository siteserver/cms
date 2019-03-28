using SiteServer.CMS.Caches;
using SiteServer.CMS.Database.Core;
using SiteServer.CMS.Database.Models;
using SiteServer.Utils;

namespace SiteServer.CMS.Database.Repositories
{
    public class ConfigRepository : GenericRepository<ConfigInfo>
    {
        private static class Attr
        {
            public const string Id = nameof(ConfigInfo.Id);
            public const string IsInitialized = "IsInitialized";
        }

        public void Insert(ConfigInfo configInfo)
        {
            //var sqlString =
            //    $"INSERT INTO {TableName} ({nameof(ConfigInfo.IsInitialized)}, {nameof(ConfigInfo.DatabaseVersion)}, {nameof(ConfigInfo.UpdateDate)}, {nameof(ConfigInfo.SystemConfig)}) VALUES (@{nameof(ConfigInfo.IsInitialized)}, @{nameof(ConfigInfo.DatabaseVersion)}, @{nameof(ConfigInfo.UpdateDate)}, @{nameof(ConfigInfo.SystemConfig)})";

            //IDataParameter[] parameters =
            //{
            //    GetParameter($"@{nameof(ConfigInfo.IsInitialized)}", info.IsInitialized.ToString()),
            //    GetParameter($"@{nameof(ConfigInfo.DatabaseVersion)}", info.DatabaseVersion),
            //    GetParameter($"@{nameof(ConfigInfo.UpdateDate)}",info.UpdateDate),
            //    GetParameter($"@{nameof(ConfigInfo.SystemConfig)}",info.SystemConfigInfo.ToString())
            //};

            //DatabaseApi.ExecuteNonQuery(ConnectionString, sqlString, parameters);
            InsertObject(configInfo);

            ConfigManager.IsChanged = true;
        }

        public void Update(ConfigInfo configInfo)
        {
            //var sqlString =
            //    $"UPDATE {TableName} SET {nameof(ConfigInfo.IsInitialized)} = @{nameof(ConfigInfo.IsInitialized)}, {nameof(ConfigInfo.DatabaseVersion)}= @{nameof(ConfigInfo.DatabaseVersion)}, {nameof(ConfigInfo.UpdateDate)}= @{nameof(ConfigInfo.UpdateDate)}, {nameof(ConfigInfo.SystemConfig)}= @{nameof(ConfigInfo.SystemConfig)} WHERE {nameof(ConfigInfo.Id)} = @{nameof(ConfigInfo.Id)}";

            //IDataParameter[] parameters =
            //{
            //    GetParameter($"@{nameof(ConfigInfo.IsInitialized)}", info.IsInitialized.ToString()),
            //    GetParameter($"@{nameof(ConfigInfo.DatabaseVersion)}", info.DatabaseVersion),
            //    GetParameter($"@{nameof(ConfigInfo.UpdateDate)}",info.UpdateDate),
            //    GetParameter($"@{nameof(ConfigInfo.SystemConfig)}",info.SystemConfigInfo.ToString()),
            //    GetParameter($"@{nameof(ConfigInfo.Id)}", info.Id)
            //};

            //DatabaseApi.ExecuteNonQuery(ConnectionString, sqlString, parameters);
            UpdateObject(configInfo);

            ConfigManager.IsChanged = true;
        }

        public bool IsInitialized()
        {
            try
            {
                //using (var rdr = DatabaseApi.ExecuteReader(ConnectionString, $"SELECT {nameof(ConfigInfo.IsInitialized)} FROM {TableName} ORDER BY {nameof(ConfigInfo.Id)}"))
                //{
                //    if (rdr.Read())
                //    {
                //        isInitialized = TranslateUtils.ToBool(DatabaseApi.GetString(rdr, 0));
                //    }
                //    rdr.Close();
                //}

                var isInitialized = GetValue<string>(Q
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

        //public string GetDatabaseVersion()
        //{
        //    try
        //    {
        //        //using (var rdr = DatabaseApi.ExecuteReader(ConnectionString, $"SELECT {nameof(ConfigInfo.DatabaseVersion)} FROM {TableName} ORDER BY {nameof(ConfigInfo.Id)}"))
        //        //{
        //        //    if (rdr.Read())
        //        //    {
        //        //        databaseVersion = DatabaseApi.GetString(rdr, 0);
        //        //    }
        //        //    rdr.Close();
        //        //}
        //        return GetValue<string>(Q
        //            .Select(Attr.DatabaseVersion)
        //            .OrderBy(Attr.Id));
        //    }
        //    catch
        //    {
        //        // ignored
        //    }

        //    return string.Empty;
        //}

        public ConfigInfo GetConfigInfo()
        {
            //ConfigInfo info = null;

            //using (var rdr = DatabaseApi.ExecuteReader(ConnectionString, $"SELECT {nameof(ConfigInfo.Id)}, {nameof(ConfigInfo.IsInitialized)}, {nameof(ConfigInfo.DatabaseVersion)}, {nameof(ConfigInfo.UpdateDate)}, {nameof(ConfigInfo.SystemConfig)} FROM {TableName} ORDER BY {nameof(ConfigInfo.Id)}"))
            //{
            //    if (rdr.Read())
            //    {
            //        var i = 0;
            //        info = new ConfigInfo(DatabaseApi.GetInt(rdr, i++), TranslateUtils.ToBool(DatabaseApi.GetString(rdr, i++)), DatabaseApi.GetString(rdr, i++), DatabaseApi.GetDateTime(rdr, i++), DatabaseApi.GetString(rdr, i));
            //    }
            //    rdr.Close();
            //}

            //return info;

            return GetObject(Q.OrderBy(Attr.Id));

            //return Get(new GenericQuery().OrderBy(Attr.Id));
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
//    public class ConfigDao : DataProviderBase
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
