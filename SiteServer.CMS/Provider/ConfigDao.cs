using System.Collections.Generic;
using System.Data;
using SiteServer.CMS.Core;
using SiteServer.CMS.Data;
using SiteServer.CMS.DataCache;
using SiteServer.CMS.Model;
using SiteServer.Plugin;

namespace SiteServer.CMS.Provider
{
    public class ConfigDao : DataProviderBase
	{
        public override string TableName => "siteserver_Config";

        public override List<TableColumn> TableColumns => new List<TableColumn>
        {
            new TableColumn
            {
                AttributeName = nameof(ConfigInfo.Id),
                DataType = DataType.Integer,
                IsIdentity = true,
                IsPrimaryKey = true
            },
            new TableColumn
            {
                AttributeName = nameof(ConfigInfo.IsInitialized),
                DataType = DataType.VarChar,
                DataLength = 18
            },
            new TableColumn
            {
                AttributeName = nameof(ConfigInfo.DatabaseVersion),
                DataType = DataType.VarChar,
                DataLength = 50
            },
            new TableColumn
            {
                AttributeName = nameof(ConfigInfo.UpdateDate),
                DataType = DataType.DateTime
            },
            new TableColumn
            {
                AttributeName = nameof(ConfigInfo.SystemConfig),
                DataType = DataType.Text
            }
        };

        public void Insert(ConfigInfo info)
        {
            var sqlString =
                $"INSERT INTO {TableName} ({nameof(ConfigInfo.IsInitialized)}, {nameof(ConfigInfo.DatabaseVersion)}, {nameof(ConfigInfo.UpdateDate)}, {nameof(ConfigInfo.SystemConfig)}) VALUES (@{nameof(ConfigInfo.IsInitialized)}, @{nameof(ConfigInfo.DatabaseVersion)}, @{nameof(ConfigInfo.UpdateDate)}, @{nameof(ConfigInfo.SystemConfig)})";

            var insertParms = new IDataParameter[]
			{
				GetParameter($"@{nameof(ConfigInfo.IsInitialized)}", DataType.VarChar, 18, info.IsInitialized.ToString()),
				GetParameter($"@{nameof(ConfigInfo.DatabaseVersion)}", DataType.VarChar, 50, info.DatabaseVersion),
                GetParameter($"@{nameof(ConfigInfo.UpdateDate)}", DataType.DateTime, info.UpdateDate),
                GetParameter($"@{nameof(ConfigInfo.SystemConfig)}", DataType.Text, info.SystemConfigInfo.ToString())
            };

            ExecuteNonQuery(sqlString, insertParms);
            ConfigManager.IsChanged = true;
		}

		public void Update(ConfigInfo info)
		{
		    var sqlString =
                $"UPDATE {TableName} SET {nameof(ConfigInfo.IsInitialized)} = @{nameof(ConfigInfo.IsInitialized)}, {nameof(ConfigInfo.DatabaseVersion)}= @{nameof(ConfigInfo.DatabaseVersion)}, {nameof(ConfigInfo.UpdateDate)}= @{nameof(ConfigInfo.UpdateDate)}, {nameof(ConfigInfo.SystemConfig)}= @{nameof(ConfigInfo.SystemConfig)} WHERE {nameof(ConfigInfo.Id)} = @{nameof(ConfigInfo.Id)}";

            var updateParms = new IDataParameter[]
			{
				GetParameter($"@{nameof(ConfigInfo.IsInitialized)}", DataType.VarChar, 18, info.IsInitialized.ToString()),
				GetParameter($"@{nameof(ConfigInfo.DatabaseVersion)}", DataType.VarChar, 50, info.DatabaseVersion),
                GetParameter($"@{nameof(ConfigInfo.UpdateDate)}", DataType.DateTime, info.UpdateDate),
                GetParameter($"@{nameof(ConfigInfo.SystemConfig)}", DataType.Text, info.SystemConfigInfo.ToString()),
			    GetParameter($"@{nameof(ConfigInfo.Id)}", DataType.Integer, info.Id)
            };

            ExecuteNonQuery(sqlString, updateParms);
            ConfigManager.IsChanged = true;
		}

		public bool IsInitialized()
		{
            var isInitialized = false;

			try
			{
                using (var rdr = ExecuteReader($"SELECT {nameof(ConfigInfo.IsInitialized)} FROM {TableName} ORDER BY {nameof(ConfigInfo.Id)}")) 
				{
					if (rdr.Read()) 
					{
                        isInitialized = GetBool(rdr, 0);
					}
					rdr.Close();
				}
			}
		    catch
		    {
		        // ignored
		    }

		    return isInitialized;
		}

		public string GetDatabaseVersion()
		{
			var databaseVersion = string.Empty;

			try
			{
				using (var rdr = ExecuteReader($"SELECT {nameof(ConfigInfo.DatabaseVersion)} FROM {TableName} ORDER BY {nameof(ConfigInfo.Id)}")) 
				{
					if (rdr.Read()) 
					{
                        databaseVersion = GetString(rdr, 0);
					}
					rdr.Close();
				}
			}
		    catch
		    {
		        // ignored
		    }

		    return databaseVersion;
		}

		public ConfigInfo GetConfigInfo()
		{
            ConfigInfo info = null;

		    using (var rdr = ExecuteReader($"SELECT {nameof(ConfigInfo.Id)}, {nameof(ConfigInfo.IsInitialized)}, {nameof(ConfigInfo.DatabaseVersion)}, {nameof(ConfigInfo.UpdateDate)}, {nameof(ConfigInfo.SystemConfig)} FROM {TableName} ORDER BY {nameof(ConfigInfo.Id)}"))
            {
                if (rdr.Read())
                {
                    var i = 0;
                    info = new ConfigInfo(GetInt(rdr, i++), GetBool(rdr, i++), GetString(rdr, i++), GetDateTime(rdr, i++), GetString(rdr, i));
                }
                rdr.Close();
            }

			return info;
		}
    }
}
