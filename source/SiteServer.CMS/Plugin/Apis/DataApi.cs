using System;
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

                var databaseType = EDatabaseTypeUtils.GetEnumType(DatabaseType);

                switch (databaseType)
                {
                    case EDatabaseType.MySql:
                        _dbHelper = new MySql();
                        break;
                    case EDatabaseType.SqlServer:
                        _dbHelper = new SqlServer();
                        break;
                    case EDatabaseType.PostgreSql:
                        _dbHelper = new PostgreSql();
                        break;
                    case EDatabaseType.Oracle:
                        _dbHelper = new Oracle();
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                return _dbHelper;
            }
        }

        public string Encrypt(string inputString)
        {
            return TranslateUtils.EncryptStringBySecretKey(inputString);
        }

        public string Decrypt(string inputString)
        {
            return TranslateUtils.DecryptStringBySecretKey(inputString);
        }

        public string FilterXss(string html)
        {
            return PageUtils.FilterXss(html);
        }

        public string FilterSql(string sql)
        {
            return PageUtils.FilterSql(sql);
        }
    }
}
