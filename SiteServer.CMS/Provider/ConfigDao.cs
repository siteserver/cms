using System.Collections.Generic;
using System.Data;
using SiteServer.CMS.Core;
using SiteServer.CMS.Data;
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

        private const string SqlInsertConfig = "INSERT INTO siteserver_Config (IsInitialized, DatabaseVersion, UpdateDate, SystemConfig) VALUES (@IsInitialized, @DatabaseVersion, @UpdateDate, @SystemConfig)";

        private const string SqlSelectConfig = "SELECT IsInitialized, DatabaseVersion, UpdateDate, SystemConfig FROM siteserver_Config";

        private const string SqlSelectIsInitialized = "SELECT IsInitialized FROM siteserver_Config";

		private const string SqlSelectDatabaseVersion = "SELECT DatabaseVersion FROM siteserver_Config";

        private const string SqlUpdateConfig = "UPDATE siteserver_Config SET IsInitialized = @IsInitialized, DatabaseVersion = @DatabaseVersion, UpdateDate = @UpdateDate, SystemConfig = @SystemConfig";

		private const string ParmIsInitialized = "@IsInitialized";
		private const string ParmDatabaseVersion = "@DatabaseVersion";
        private const string ParmUpdateDate = "@UpdateDate";
        private const string ParmSystemConfig = "@SystemConfig";

        public void Insert(ConfigInfo info) 
		{
			var insertParms = new IDataParameter[]
			{
				GetParameter(ParmIsInitialized, DataType.VarChar, 18, info.IsInitialized.ToString()),
				GetParameter(ParmDatabaseVersion, DataType.VarChar, 50, info.DatabaseVersion),
                GetParameter(ParmUpdateDate, DataType.DateTime, info.UpdateDate),
                GetParameter(ParmSystemConfig, DataType.Text, info.SystemConfigInfo.ToString())
            };

            ExecuteNonQuery(SqlInsertConfig, insertParms);
            ConfigManager.IsChanged = true;
		}

		public void Update(ConfigInfo info) 
		{
			var updateParms = new IDataParameter[]
			{
				GetParameter(ParmIsInitialized, DataType.VarChar, 18, info.IsInitialized.ToString()),
				GetParameter(ParmDatabaseVersion, DataType.VarChar, 50, info.DatabaseVersion),
                GetParameter(ParmUpdateDate, DataType.DateTime, info.UpdateDate),
                GetParameter(ParmSystemConfig, DataType.Text, info.SystemConfigInfo.ToString())
            };

            ExecuteNonQuery(SqlUpdateConfig, updateParms);
            ConfigManager.IsChanged = true;
		}

		public bool IsInitialized()
		{
            var isInitialized = false;

			try
			{
                using (var rdr = ExecuteReader(SqlSelectIsInitialized)) 
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
				using (var rdr = ExecuteReader(SqlSelectDatabaseVersion)) 
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

		    using (var rdr = ExecuteReader(SqlSelectConfig))
            {
                if (rdr.Read())
                {
                    var i = 0;
                    info = new ConfigInfo(GetBool(rdr, i++), GetString(rdr, i++), GetDateTime(rdr, i++), GetString(rdr, i));
                }
                rdr.Close();
            }

			return info;
		}
    }
}
