using System.Data;
using BaiRong.Core.Data;
using SiteServer.Plugin;
using SiteServer.Plugin.Models;

namespace SiteServer.CMS.Provider
{
    public class PluginConfigDao : DataProviderBase
    {
        private const string ParmPluginId = "@PluginId";
        private const string ParmSiteId = "@SiteId";
        private const string ParmConfigName = "@ConfigName";
        private const string ParmConfigValue = "@ConfigValue";

        public void Insert(string pluginId, int siteId, string configName, string configValue)
        {
            const string sqlString = "INSERT INTO siteserver_PluginConfig(PluginId, SiteId, ConfigName, ConfigValue) VALUES (@PluginId, @SiteId, @ConfigName, @ConfigValue)";

            var parms = new IDataParameter[]
			{
                GetParameter(ParmPluginId, DataType.NVarChar, 50, pluginId),
                GetParameter(ParmSiteId, DataType.Integer, siteId),
                GetParameter(ParmConfigName, DataType.NVarChar, 200, configName),
                GetParameter(ParmConfigValue, DataType.NText, configValue)
			};

            ExecuteNonQuery(sqlString, parms);
        }

        public void Delete(string pluginId, int siteId, string configName)
        {
            const string sqlString = "DELETE FROM siteserver_PluginConfig WHERE PluginId = @PluginId AND SiteId = @SiteId AND ConfigName = @ConfigName";

            var parms = new IDataParameter[]
            {
                GetParameter(ParmPluginId, DataType.NVarChar, 50, pluginId),
                GetParameter(ParmSiteId, DataType.Integer, siteId),
                GetParameter(ParmConfigName, DataType.NVarChar, 200, configName)
            };

            ExecuteNonQuery(sqlString, parms);
        }

        public void DeleteAll(string pluginId)
        {
            const string sqlString = "DELETE FROM siteserver_PluginConfig WHERE PluginId = @PluginId";

            var parms = new IDataParameter[]
            {
                GetParameter(ParmPluginId, DataType.NVarChar, 50, pluginId)
            };

            ExecuteNonQuery(sqlString, parms);
        }

        public void DeleteAll(int siteId)
        {
            const string sqlString = "DELETE FROM siteserver_PluginConfig WHERE SiteId = @SiteId";

            var parms = new IDataParameter[]
            {
                GetParameter(ParmSiteId, DataType.Integer, siteId)
            };

            ExecuteNonQuery(sqlString, parms);
        }

        public void Update(string pluginId, int siteId, string configName, string configValue)
        {
            const string sqlString = "UPDATE siteserver_PluginConfig SET ConfigValue = @ConfigValue WHERE PluginId = @PluginId AND SiteId = @SiteId AND ConfigName = @ConfigName";

            var parms = new IDataParameter[]
            {
                GetParameter(ParmConfigValue, DataType.NText, configValue),
                GetParameter(ParmPluginId, DataType.NVarChar, 50, pluginId),
                GetParameter(ParmSiteId, DataType.Integer, siteId),
                GetParameter(ParmConfigName, DataType.NVarChar, 200, configName)
            };
            ExecuteNonQuery(sqlString, parms);
        }

        public string GetValue(string pluginId, int siteId, string configName)
        {
            var value = string.Empty;

            const string sqlString = "SELECT ConfigValue FROM siteserver_PluginConfig WHERE PluginId = @PluginId AND SiteId = @SiteId AND ConfigName = @ConfigName";

            var parms = new IDataParameter[]
            {
                GetParameter(ParmPluginId, DataType.NVarChar, 50, pluginId),
                GetParameter(ParmSiteId, DataType.Integer, siteId),
                GetParameter(ParmConfigName, DataType.NVarChar, 200, configName)
            };

            using (var rdr = ExecuteReader(sqlString, parms))
            {
                if (rdr.Read() && !rdr.IsDBNull(0))
                {
                    value = rdr.GetString(0);
                }
                rdr.Close();
            }

            return value;
        }

        public bool IsExists(string pluginId, int siteId, string configName)
        {
            var exists = false;

            const string sqlString = "SELECT Id FROM siteserver_PluginConfig WHERE PluginId = @PluginId AND SiteId = @SiteId AND ConfigName = @ConfigName";

            var parms = new IDataParameter[]
            {
                GetParameter(ParmPluginId, DataType.NVarChar, 50, pluginId),
                GetParameter(ParmSiteId, DataType.Integer, siteId),
                GetParameter(ParmConfigName, DataType.NVarChar, 200, configName)
            };

            using (var rdr = ExecuteReader(sqlString, parms))
            {
                if (rdr.Read() && !rdr.IsDBNull(0))
                {
                    exists = true;
                }
                rdr.Close();
            }

            return exists;
        }
    }
}
