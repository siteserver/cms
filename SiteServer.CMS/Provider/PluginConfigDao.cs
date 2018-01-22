using System.Collections.Generic;
using System.Data;
using SiteServer.CMS.Data;
using SiteServer.Utils.Model;
using SiteServer.CMS.Model;
using SiteServer.Plugin;

namespace SiteServer.CMS.Provider
{
    public class PluginConfigDao : DataProviderBase
    {
        public override string TableName => "siteserver_PluginConfig";

        public override List<TableColumnInfo> TableColumns => new List<TableColumnInfo>
        {
            new TableColumnInfo
            {
                ColumnName = nameof(PluginConfigInfo.Id),
                DataType = DataType.Integer,
                IsIdentity = true,
                IsPrimaryKey = true
            },
            new TableColumnInfo
            {
                ColumnName = nameof(PluginConfigInfo.PluginId),
                DataType = DataType.VarChar,
                Length = 50
            },
            new TableColumnInfo
            {
                ColumnName = nameof(PluginConfigInfo.PublishmentSystemId),
                DataType = DataType.Integer
            },
            new TableColumnInfo
            {
                ColumnName = nameof(PluginConfigInfo.ConfigName),
                DataType = DataType.VarChar,
                Length = 200
            },
            new TableColumnInfo
            {
                ColumnName = nameof(PluginConfigInfo.ConfigValue),
                DataType = DataType.Text
            }
        };

        private const string ParmPluginId = "@PluginId";
        private const string ParmPublishmentSystemId = "@PublishmentSystemId";
        private const string ParmConfigName = "@ConfigName";
        private const string ParmConfigValue = "@ConfigValue";

        public void Insert(PluginConfigInfo configInfo)
        {
            const string sqlString = "INSERT INTO siteserver_PluginConfig(PluginId, PublishmentSystemId, ConfigName, ConfigValue) VALUES (@PluginId, @PublishmentSystemId, @ConfigName, @ConfigValue)";

            var parms = new IDataParameter[]
			{
                GetParameter(ParmPluginId, DataType.VarChar, 50, configInfo.PluginId),
                GetParameter(ParmPublishmentSystemId, DataType.Integer, configInfo.PublishmentSystemId),
                GetParameter(ParmConfigName, DataType.VarChar, 200, configInfo.ConfigName),
                GetParameter(ParmConfigValue, DataType.Text, configInfo.ConfigValue)
			};

            ExecuteNonQuery(sqlString, parms);
        }

        public void Delete(string pluginId, int publishmentSystemId, string configName)
        {
            const string sqlString = "DELETE FROM siteserver_PluginConfig WHERE PluginId = @PluginId AND PublishmentSystemId = @PublishmentSystemId AND ConfigName = @ConfigName";

            var parms = new IDataParameter[]
            {
                GetParameter(ParmPluginId, DataType.VarChar, 50, pluginId),
                GetParameter(ParmPublishmentSystemId, DataType.Integer, publishmentSystemId),
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

        public void DeleteAll(int publishmentSystemId)
        {
            const string sqlString = "DELETE FROM siteserver_PluginConfig WHERE PublishmentSystemId = @PublishmentSystemId";

            var parms = new IDataParameter[]
            {
                GetParameter(ParmPublishmentSystemId, DataType.Integer, publishmentSystemId)
            };

            ExecuteNonQuery(sqlString, parms);
        }

        public void Update(PluginConfigInfo configInfo)
        {
            const string sqlString = "UPDATE siteserver_PluginConfig SET ConfigValue = @ConfigValue WHERE PluginId = @PluginId AND PublishmentSystemId = @PublishmentSystemId AND ConfigName = @ConfigName";

            var parms = new IDataParameter[]
            {
                GetParameter(ParmConfigValue, DataType.Text, configInfo.ConfigValue),
                GetParameter(ParmPluginId, DataType.VarChar, 50, configInfo.PluginId),
                GetParameter(ParmPublishmentSystemId, DataType.Integer, configInfo.PublishmentSystemId),
                GetParameter(ParmConfigName, DataType.VarChar, 200, configInfo.ConfigName)
            };
            ExecuteNonQuery(sqlString, parms);
        }

        public string GetValue(string pluginId, int publishmentSystemId, string configName)
        {
            var value = string.Empty;

            const string sqlString = "SELECT ConfigValue FROM siteserver_PluginConfig WHERE PluginId = @PluginId AND PublishmentSystemId = @PublishmentSystemId AND ConfigName = @ConfigName";

            var parms = new IDataParameter[]
            {
                GetParameter(ParmPluginId, DataType.VarChar, 50, pluginId),
                GetParameter(ParmPublishmentSystemId, DataType.Integer, publishmentSystemId),
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

        public bool IsExists(string pluginId, int publishmentSystemId, string configName)
        {
            var exists = false;

            const string sqlString = "SELECT Id FROM siteserver_PluginConfig WHERE PluginId = @PluginId AND PublishmentSystemId = @PublishmentSystemId AND ConfigName = @ConfigName";

            var parms = new IDataParameter[]
            {
                GetParameter(ParmPluginId, DataType.VarChar, 50, pluginId),
                GetParameter(ParmPublishmentSystemId, DataType.Integer, publishmentSystemId),
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
