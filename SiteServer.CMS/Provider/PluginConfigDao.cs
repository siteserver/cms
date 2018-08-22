using System.Collections.Generic;
using System.Data;
using SiteServer.CMS.Data;
using SiteServer.CMS.Model;
using SiteServer.Plugin;

namespace SiteServer.CMS.Provider
{
    public class PluginConfigDao : DataProviderBase
    {
        public override string TableName => "siteserver_PluginConfig";

        public override List<TableColumn> TableColumns => new List<TableColumn>
        {
            new TableColumn
            {
                AttributeName = nameof(PluginConfigInfo.Id),
                DataType = DataType.Integer,
                IsIdentity = true,
                IsPrimaryKey = true
            },
            new TableColumn
            {
                AttributeName = nameof(PluginConfigInfo.PluginId),
                DataType = DataType.VarChar,
                DataLength = 50
            },
            new TableColumn
            {
                AttributeName = nameof(PluginConfigInfo.SiteId),
                DataType = DataType.Integer
            },
            new TableColumn
            {
                AttributeName = nameof(PluginConfigInfo.ConfigName),
                DataType = DataType.VarChar,
                DataLength = 200
            },
            new TableColumn
            {
                AttributeName = nameof(PluginConfigInfo.ConfigValue),
                DataType = DataType.Text
            }
        };

        private const string ParmPluginId = "@PluginId";
        private const string ParmSiteId = "@SiteId";
        private const string ParmConfigName = "@ConfigName";
        private const string ParmConfigValue = "@ConfigValue";

        public void Insert(PluginConfigInfo configInfo)
        {
            const string sqlString = "INSERT INTO siteserver_PluginConfig(PluginId, SiteId, ConfigName, ConfigValue) VALUES (@PluginId, @SiteId, @ConfigName, @ConfigValue)";

            var parms = new IDataParameter[]
			{
                GetParameter(ParmPluginId, DataType.VarChar, 50, configInfo.PluginId),
                GetParameter(ParmSiteId, DataType.Integer, configInfo.SiteId),
                GetParameter(ParmConfigName, DataType.VarChar, 200, configInfo.ConfigName),
                GetParameter(ParmConfigValue, DataType.Text, configInfo.ConfigValue)
			};

            ExecuteNonQuery(sqlString, parms);
        }

        public void Delete(string pluginId, int siteId, string configName)
        {
            const string sqlString = "DELETE FROM siteserver_PluginConfig WHERE PluginId = @PluginId AND SiteId = @SiteId AND ConfigName = @ConfigName";

            var parms = new IDataParameter[]
            {
                GetParameter(ParmPluginId, DataType.VarChar, 50, pluginId),
                GetParameter(ParmSiteId, DataType.Integer, siteId),
                GetParameter(ParmConfigName, DataType.VarChar, 200, configName)
            };

            ExecuteNonQuery(sqlString, parms);
        }

        public void DeleteAll(string pluginId)
        {
            const string sqlString = "DELETE FROM siteserver_PluginConfig WHERE PluginId = @PluginId";

            var parms = new IDataParameter[]
            {
                GetParameter(ParmPluginId, DataType.VarChar, 50, pluginId)
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

        public void Update(PluginConfigInfo configInfo)
        {
            const string sqlString = "UPDATE siteserver_PluginConfig SET ConfigValue = @ConfigValue WHERE PluginId = @PluginId AND SiteId = @SiteId AND ConfigName = @ConfigName";

            var parms = new IDataParameter[]
            {
                GetParameter(ParmConfigValue, DataType.Text, configInfo.ConfigValue),
                GetParameter(ParmPluginId, DataType.VarChar, 50, configInfo.PluginId),
                GetParameter(ParmSiteId, DataType.Integer, configInfo.SiteId),
                GetParameter(ParmConfigName, DataType.VarChar, 200, configInfo.ConfigName)
            };
            ExecuteNonQuery(sqlString, parms);
        }

        public string GetValue(string pluginId, int siteId, string configName)
        {
            var value = string.Empty;

            const string sqlString = "SELECT ConfigValue FROM siteserver_PluginConfig WHERE PluginId = @PluginId AND SiteId = @SiteId AND ConfigName = @ConfigName";

            var parms = new IDataParameter[]
            {
                GetParameter(ParmPluginId, DataType.VarChar, 50, pluginId),
                GetParameter(ParmSiteId, DataType.Integer, siteId),
                GetParameter(ParmConfigName, DataType.VarChar, 200, configName)
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
                GetParameter(ParmPluginId, DataType.VarChar, 50, pluginId),
                GetParameter(ParmSiteId, DataType.Integer, siteId),
                GetParameter(ParmConfigName, DataType.VarChar, 200, configName)
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
