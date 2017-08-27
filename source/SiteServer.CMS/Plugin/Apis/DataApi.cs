using BaiRong.Core;
using BaiRong.Core.Data;
using BaiRong.Core.Model.Enumerations;
using SiteServer.Plugin.Apis;
using SiteServer.Plugin.Models;

namespace SiteServer.CMS.Plugin.Apis
{
    public class DataApi : IDataApi
    {
        private readonly PluginMetadata _metadata;

        public DataApi(PluginMetadata metadata)
        {
            _metadata = metadata;
        }

        private string _databaseType;
        public string DatabaseType
        {
            get
            {
                if (_databaseType != null) return _databaseType;

                _databaseType = EDatabaseTypeUtils.GetValue(WebConfigUtils.DatabaseType);
                if (string.IsNullOrEmpty(_metadata.DatabaseType)) return _databaseType;

                _databaseType = _metadata.DatabaseType;
                if (WebConfigUtils.IsProtectData)
                {
                    _databaseType = TranslateUtils.DecryptStringBySecretKey(_databaseType);
                }
                return _databaseType;
            }
        }

        private string _connectionString;
        public string ConnectionString
        {
            get
            {
                if (_connectionString != null) return _connectionString;

                _connectionString = WebConfigUtils.ConnectionString;
                if (string.IsNullOrEmpty(_metadata.ConnectionString)) return _connectionString;

                _connectionString = _metadata.ConnectionString;
                if (WebConfigUtils.IsProtectData)
                {
                    _connectionString = TranslateUtils.DecryptStringBySecretKey(_connectionString);
                }
                return _connectionString;
            }
        }

        private IDbHelper _dbHelper;
        public IDbHelper DbHelper
        {
            get
            {
                if (_dbHelper != null) return _dbHelper;

                if (EDatabaseTypeUtils.Equals(DatabaseType, EDatabaseType.MySql))
                {
                    _dbHelper = new MySql();
                }
                else
                {
                    _dbHelper = new SqlServer();
                }
                return _dbHelper;
            }
        }
    }
}
