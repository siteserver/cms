using SiteServer.CMS.Database.Core;
using SiteServer.CMS.Database.Models;

namespace SiteServer.CMS.Database.Repositories
{
    public class PluginConfigRepository : GenericRepository<PluginConfigInfo>
    {
        private static class Attr
        {
            public const string PluginId = nameof(PluginConfigInfo.PluginId);
            public const string SiteId = nameof(PluginConfigInfo.SiteId);
            public const string ConfigName = nameof(PluginConfigInfo.ConfigName);
            public const string ConfigValue = nameof(PluginConfigInfo.ConfigValue);
        }

        public void Insert(PluginConfigInfo configInfo)
        {
            //const string sqlString = "INSERT INTO siteserver_PluginConfig(PluginId, SiteId, ConfigName, ConfigValue) VALUES (@PluginId, @SiteId, @ConfigName, @ConfigValue)";

            //IDataParameter[] parameters =
            //{
            //    GetParameter(ParamPluginId, configInfo.PluginId),
            //    GetParameter(ParamSiteId, configInfo.SiteId),
            //    GetParameter(ParamConfigName, configInfo.ConfigName),
            //    GetParameter(ParamConfigValue,configInfo.ConfigValue)
            //};

            //DatabaseApi.ExecuteNonQuery(ConnectionString, sqlString, parameters);

            InsertObject(configInfo);
        }

        public void Delete(string pluginId, int siteId, string configName)
        {
            //const string sqlString = "DELETE FROM siteserver_PluginConfig WHERE PluginId = @PluginId AND SiteId = @SiteId AND ConfigName = @ConfigName";

            //IDataParameter[] parameters =
            //{
            //    GetParameter(ParamPluginId, pluginId),
            //    GetParameter(ParamSiteId, siteId),
            //    GetParameter(ParamConfigName, configName)
            //};

            //DatabaseApi.ExecuteNonQuery(ConnectionString, sqlString, parameters);

            DeleteAll(Q
                .Where(Attr.SiteId, siteId)
                .Where(Attr.PluginId, pluginId)
                .Where(Attr.ConfigName, configName));
        }

        public void Update(PluginConfigInfo configInfo)
        {
            //const string sqlString = "UPDATE siteserver_PluginConfig SET ConfigValue = @ConfigValue WHERE PluginId = @PluginId AND SiteId = @SiteId AND ConfigName = @ConfigName";

            //IDataParameter[] parameters =
            //{
            //    GetParameter(ParamConfigValue,configInfo.ConfigValue),
            //    GetParameter(ParamPluginId, configInfo.PluginId),
            //    GetParameter(ParamSiteId, configInfo.SiteId),
            //    GetParameter(ParamConfigName, configInfo.ConfigName)
            //};
            //DatabaseApi.ExecuteNonQuery(ConnectionString, sqlString, parameters);

            UpdateObject(configInfo);
        }

        public string GetValue(string pluginId, int siteId, string configName)
        {
            //var value = string.Empty;

            //const string sqlString = "SELECT ConfigValue FROM siteserver_PluginConfig WHERE PluginId = @PluginId AND SiteId = @SiteId AND ConfigName = @ConfigName";

            //IDataParameter[] parameters =
            //{
            //    GetParameter(ParamPluginId, pluginId),
            //    GetParameter(ParamSiteId, siteId),
            //    GetParameter(ParamConfigName, configName)
            //};

            //using (var rdr = DatabaseApi.ExecuteReader(ConnectionString, sqlString, parameters))
            //{
            //    if (rdr.Read() && !rdr.IsDBNull(0))
            //    {
            //        value = rdr.GetString(0);
            //    }
            //    rdr.Close();
            //}

            //return value;

            return GetValue<string>(Q
                .Select(Attr.ConfigValue)
                .Where(Attr.SiteId, siteId)
                .Where(Attr.PluginId, pluginId)
                .Where(Attr.ConfigName, configName));
        }

        public bool IsExists(string pluginId, int siteId, string configName)
        {
            //var exists = false;

            //const string sqlString = "SELECT Id FROM siteserver_PluginConfig WHERE PluginId = @PluginId AND SiteId = @SiteId AND ConfigName = @ConfigName";

            //IDataParameter[] parameters =
            //{
            //    GetParameter(ParamPluginId, pluginId),
            //    GetParameter(ParamSiteId, siteId),
            //    GetParameter(ParamConfigName, configName)
            //};

            //using (var rdr = DatabaseApi.ExecuteReader(ConnectionString, sqlString, parameters))
            //{
            //    if (rdr.Read() && !rdr.IsDBNull(0))
            //    {
            //        exists = true;
            //    }
            //    rdr.Close();
            //}

            //return exists;

            return Exists(Q
                .Where(Attr.SiteId, siteId)
                .Where(Attr.PluginId, pluginId)
                .Where(Attr.ConfigName, configName));
        }
    }
}


//using System.Collections.Generic;
//using System.Data;
//using SiteServer.CMS.Database.Core;
//using SiteServer.CMS.Database.Models;
//using SiteServer.Plugin;

//namespace SiteServer.CMS.Database.Repositories
//{
//    public class PluginConfig : DataProviderBase
//    {
//        public override string TableName => "siteserver_PluginConfig";

//        public override List<TableColumn> TableColumns => new List<TableColumn>
//        {
//            new TableColumn
//            {
//                AttributeName = nameof(PluginConfigInfo.Id),
//                DataType = DataType.Integer,
//                IsIdentity = true,
//                IsPrimaryKey = true
//            },
//            new TableColumn
//            {
//                AttributeName = nameof(PluginConfigInfo.PluginId),
//                DataType = DataType.VarChar
//            },
//            new TableColumn
//            {
//                AttributeName = nameof(PluginConfigInfo.SiteId),
//                DataType = DataType.Integer
//            },
//            new TableColumn
//            {
//                AttributeName = nameof(PluginConfigInfo.ConfigName),
//                DataType = DataType.VarChar
//            },
//            new TableColumn
//            {
//                AttributeName = nameof(PluginConfigInfo.ConfigValue),
//                DataType = DataType.Text
//            }
//        };

//        private const string ParamPluginId = "@PluginId";
//        private const string ParamSiteId = "@SiteId";
//        private const string ParamConfigName = "@ConfigName";
//        private const string ParamConfigValue = "@ConfigValue";

//        public void InsertObject(PluginConfigInfo configInfo)
//        {
//            const string sqlString = "INSERT INTO siteserver_PluginConfig(PluginId, SiteId, ConfigName, ConfigValue) VALUES (@PluginId, @SiteId, @ConfigName, @ConfigValue)";

//            IDataParameter[] parameters =
//			{
//                GetParameter(ParamPluginId, configInfo.PluginId),
//                GetParameter(ParamSiteId, configInfo.SiteId),
//                GetParameter(ParamConfigName, configInfo.ConfigName),
//                GetParameter(ParamConfigValue,configInfo.ConfigValue)
//			};

//            DatabaseApi.ExecuteNonQuery(ConnectionString, sqlString, parameters);
//        }

//        public void DeleteById(string pluginId, int siteId, string configName)
//        {
//            const string sqlString = "DELETE FROM siteserver_PluginConfig WHERE PluginId = @PluginId AND SiteId = @SiteId AND ConfigName = @ConfigName";

//            IDataParameter[] parameters =
//            {
//                GetParameter(ParamPluginId, pluginId),
//                GetParameter(ParamSiteId, siteId),
//                GetParameter(ParamConfigName, configName)
//            };

//            DatabaseApi.ExecuteNonQuery(ConnectionString, sqlString, parameters);
//        }

//        public void UpdateObject(PluginConfigInfo configInfo)
//        {
//            const string sqlString = "UPDATE siteserver_PluginConfig SET ConfigValue = @ConfigValue WHERE PluginId = @PluginId AND SiteId = @SiteId AND ConfigName = @ConfigName";

//            IDataParameter[] parameters =
//            {
//                GetParameter(ParamConfigValue,configInfo.ConfigValue),
//                GetParameter(ParamPluginId, configInfo.PluginId),
//                GetParameter(ParamSiteId, configInfo.SiteId),
//                GetParameter(ParamConfigName, configInfo.ConfigName)
//            };
//            DatabaseApi.ExecuteNonQuery(ConnectionString, sqlString, parameters);
//        }

//        public string GetValueById(string pluginId, int siteId, string configName)
//        {
//            var value = string.Empty;

//            const string sqlString = "SELECT ConfigValue FROM siteserver_PluginConfig WHERE PluginId = @PluginId AND SiteId = @SiteId AND ConfigName = @ConfigName";

//            IDataParameter[] parameters =
//            {
//                GetParameter(ParamPluginId, pluginId),
//                GetParameter(ParamSiteId, siteId),
//                GetParameter(ParamConfigName, configName)
//            };

//            using (var rdr = DatabaseApi.ExecuteReader(ConnectionString, sqlString, parameters))
//            {
//                if (rdr.Read() && !rdr.IsDBNull(0))
//                {
//                    value = rdr.GetString(0);
//                }
//                rdr.Close();
//            }

//            return value;
//        }

//        public bool IsExists(string pluginId, int siteId, string configName)
//        {
//            var exists = false;

//            const string sqlString = "SELECT Id FROM siteserver_PluginConfig WHERE PluginId = @PluginId AND SiteId = @SiteId AND ConfigName = @ConfigName";

//            IDataParameter[] parameters =
//            {
//                GetParameter(ParamPluginId, pluginId),
//                GetParameter(ParamSiteId, siteId),
//                GetParameter(ParamConfigName, configName)
//            };

//            using (var rdr = DatabaseApi.ExecuteReader(ConnectionString, sqlString, parameters))
//            {
//                if (rdr.Read() && !rdr.IsDBNull(0))
//                {
//                    exists = true;
//                }
//                rdr.Close();
//            }

//            return exists;
//        }
//    }
//}
