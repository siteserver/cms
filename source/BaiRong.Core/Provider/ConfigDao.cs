using System;
using System.Collections.Specialized;
using System.Data;
using BaiRong.Core.Data;
using BaiRong.Core.Model;
using BaiRong.Core.Model.Enumerations;
using BaiRong.Core.Permissions;

namespace BaiRong.Core.Provider
{
    public class ConfigDao : DataProviderBase
	{
        public string TableName => "bairong_Config";

        private const string SqlInsertConfig = "INSERT INTO bairong_Config (IsInitialized, DatabaseVersion, RestrictionBlackList, RestrictionWhiteList, UpdateDate, UserConfig, SystemConfig) VALUES (@IsInitialized, @DatabaseVersion, @RestrictionBlackList, @RestrictionWhiteList, @UpdateDate, @UserConfig, @SystemConfig)";

        private const string SqlSelectConfig = "SELECT IsInitialized, DatabaseVersion, RestrictionBlackList, RestrictionWhiteList, UpdateDate, UserConfig, SystemConfig FROM bairong_Config";

        private const string SqlSelectIsInitialized = "SELECT IsInitialized FROM bairong_Config";

		private const string SqlSelectDatabaseVersion = "SELECT DatabaseVersion FROM bairong_Config";

        private const string SqlUpdateConfig = "UPDATE bairong_Config SET IsInitialized = @IsInitialized, DatabaseVersion = @DatabaseVersion, RestrictionBlackList = @RestrictionBlackList, RestrictionWhiteList = @RestrictionWhiteList, UpdateDate = @UpdateDate, UserConfig = @UserConfig, SystemConfig = @SystemConfig";

		private const string ParmIsInitialized = "@IsInitialized";
		private const string ParmDatabaseVersion = "@DatabaseVersion";
        private const string ParmRestrictionBlackList = "@RestrictionBlackList";
        private const string ParmRestrictionWhiteList = "@RestrictionWhiteList";
        private const string ParmUpdateDate = "@UpdateDate";
		private const string ParmUserConfig = "@UserConfig";
        private const string ParmSystemConfig = "@SystemConfig";

        public void Insert(ConfigInfo info) 
		{
			var insertParms = new IDataParameter[]
			{
				GetParameter(ParmIsInitialized, EDataType.VarChar, 18, info.IsInitialized.ToString()),
				GetParameter(ParmDatabaseVersion, EDataType.VarChar, 50, info.DatabaseVersion),
                GetParameter(ParmRestrictionBlackList, EDataType.NVarChar, 255, TranslateUtils.ObjectCollectionToString(info.RestrictionBlackList)),
                GetParameter(ParmRestrictionWhiteList, EDataType.NVarChar, 255, TranslateUtils.ObjectCollectionToString(info.RestrictionWhiteList)),
                GetParameter(ParmUpdateDate, EDataType.DateTime, info.UpdateDate),
				GetParameter(ParmUserConfig, EDataType.NText, info.UserConfigInfo.ToString()),
                GetParameter(ParmSystemConfig, EDataType.NText, info.SystemConfigInfo.ToString())
            };

            ExecuteNonQuery(SqlInsertConfig, insertParms);
            ConfigManager.IsChanged = true;
		}

		public void Update(ConfigInfo info) 
		{
			var updateParms = new IDataParameter[]
			{
				GetParameter(ParmIsInitialized, EDataType.VarChar, 18, info.IsInitialized.ToString()),
				GetParameter(ParmDatabaseVersion, EDataType.VarChar, 50, info.DatabaseVersion),
                GetParameter(ParmRestrictionBlackList, EDataType.NVarChar, 255, TranslateUtils.ObjectCollectionToString(info.RestrictionBlackList)),
                GetParameter(ParmRestrictionWhiteList, EDataType.NVarChar, 255, TranslateUtils.ObjectCollectionToString(info.RestrictionWhiteList)),
                GetParameter(ParmUpdateDate, EDataType.DateTime, info.UpdateDate),
                GetParameter(ParmUserConfig, EDataType.NText, info.UserConfigInfo.ToString()),
                GetParameter(ParmSystemConfig, EDataType.NText, info.SystemConfigInfo.ToString())
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
		    var info = new ConfigInfo
		    {
		        IsInitialized = false
		    };

		    using (var rdr = ExecuteReader(SqlSelectConfig))
            {
                if (rdr.Read())
                {
                    var i = 0;
                    info = new ConfigInfo(GetBool(rdr, i++), GetString(rdr, i++),
                        TranslateUtils.StringCollectionToStringCollection(GetString(rdr, i++)),
                        TranslateUtils.StringCollectionToStringCollection(GetString(rdr, i++)), GetDateTime(rdr, i++),
                        GetString(rdr, i++), GetString(rdr, i));
                }
                rdr.Close();
            }

			return info;
		}

        public string GetGuid(string key)
        {
            key = "guid_" + key;

            var guid = ConfigManager.SystemConfigInfo.GetExtendedAttribute(key);
            if (!string.IsNullOrEmpty(guid)) return guid;

            guid = StringUtils.GetShortGuid();
            ConfigManager.SystemConfigInfo.SetExtendedAttribute(key, guid);
            BaiRongDataProvider.ConfigDao.Update(ConfigManager.Instance);
            return guid;
        }

        public int GetSiteCount()
        {
            const string sqlString = "SELECT COUNT(*) FROM siteserver_PublishmentSystem";
            return BaiRongDataProvider.DatabaseDao.GetIntResult(sqlString);
        }

        public void InitializeConfig()
        {
            var configInfo = new ConfigInfo(true, AppManager.Version, new StringCollection(), new StringCollection(), DateTime.Now, string.Empty, string.Empty);
            Insert(configInfo);
        }

        public void InitializeUserRole(string userName, string password)
        {
            RoleManager.CreatePredefinedRoles();

            var administratorInfo = new AdministratorInfo
            {
                UserName = userName,
                Password = password
            };

            string errorMessage;
            AdminManager.CreateAdministrator(administratorInfo, out errorMessage);
            BaiRongDataProvider.RoleDao.AddUserToRole(userName, EPredefinedRoleUtils.GetValue(EPredefinedRole.ConsoleAdministrator));
        }
    }
}
