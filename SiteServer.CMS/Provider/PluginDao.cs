using System.Collections.Generic;
using System.Data;
using SiteServer.CMS.Data;
using SiteServer.Utils;
using SiteServer.CMS.Model;
using SiteServer.CMS.Plugin.Model;
using SiteServer.Plugin;

namespace SiteServer.CMS.Provider
{
    public class PluginDao : DataProviderBase
    {
        public override string TableName => "siteserver_Plugin";

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
                ColumnName = nameof(PluginInfo.IsDisabled),
                DataType = DataType.VarChar,
                Length = 18
            },
            new TableColumnInfo
            {
                ColumnName = nameof(PluginInfo.Taxis),
                DataType = DataType.Integer
            }
        };

        public void Delete(string pluginId)
        {
            const string sqlString = "DELETE FROM siteserver_Plugin WHERE PluginId = @PluginId";

            var parms = new IDataParameter[]
            {
                GetParameter(nameof(PluginConfigInfo.PluginId), DataType.VarChar, 50, pluginId)
            };

            ExecuteNonQuery(sqlString, parms);
        }

        public void UpdateIsDisabled(string pluginId, bool isDisabled)
        {
            const string sqlString = "UPDATE siteserver_Plugin SET IsDisabled = @IsDisabled WHERE PluginId = @PluginId";

            var parms = new IDataParameter[]
            {
                GetParameter(nameof(PluginInfo.IsDisabled), DataType.VarChar, 18, isDisabled.ToString()),
                GetParameter(nameof(PluginConfigInfo.PluginId), DataType.VarChar, 50, pluginId)
            };

            ExecuteNonQuery(sqlString, parms);
        }

        public void UpdateTaxis(string pluginId, int taxis)
        {
            const string sqlString = "UPDATE siteserver_Plugin SET Taxis = @Taxis WHERE PluginId = @PluginId";

            var parms = new IDataParameter[]
            {
                GetParameter(nameof(PluginInfo.Taxis), DataType.Integer, taxis),
                GetParameter(nameof(PluginConfigInfo.PluginId), DataType.VarChar, 50, pluginId)
            };

            ExecuteNonQuery(sqlString, parms);
        }

        public void SetIsDisabledAndTaxis(string pluginId, out bool isDisabled, out int taxis)
        {
            isDisabled = false;
            taxis = 0;

            var exists = false;

            var sqlString = "SELECT Id FROM siteserver_Plugin WHERE PluginId = @PluginId";

            var parameters = new IDataParameter[]
            {
                GetParameter(nameof(PluginConfigInfo.PluginId), DataType.VarChar, 50, pluginId)
            };

            using (var rdr = ExecuteReader(sqlString, parameters))
            {
                if (rdr.Read() && !rdr.IsDBNull(0))
                {
                    exists = true;
                }
                rdr.Close();
            }

            if (!exists)
            {
                sqlString = "INSERT INTO siteserver_Plugin(PluginId, IsDisabled, Taxis) VALUES (@PluginId, @IsDisabled, @Taxis)";

                parameters = new IDataParameter[]
                {
                    GetParameter(nameof(PluginConfigInfo.PluginId), DataType.VarChar, 50, pluginId),
                    GetParameter(nameof(PluginInfo.IsDisabled), DataType.VarChar, 18, false.ToString()),
                    GetParameter(nameof(PluginInfo.Taxis), DataType.Integer, 0)
                };

                ExecuteNonQuery(sqlString, parameters);
            }

            sqlString = "SELECT IsDisabled, Taxis FROM siteserver_Plugin WHERE PluginId = @PluginId";

            parameters = new IDataParameter[]
            {
                GetParameter(nameof(PluginConfigInfo.PluginId), DataType.VarChar, 50, pluginId)
            };

            using (var rdr = ExecuteReader(sqlString, parameters))
            {
                if (rdr.Read() && !rdr.IsDBNull(0))
                {
                    isDisabled = TranslateUtils.ToBool(rdr.GetString(0));
                    taxis = rdr.GetInt32(1);
                }
                rdr.Close();
            }
        }
    }
}
