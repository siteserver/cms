using System;
using System.Collections.Generic;
using Datory;
using SiteServer.CMS.Core;
using SiteServer.CMS.DataCache;
using SiteServer.CMS.Model;
using SiteServer.Utils;

namespace SiteServer.CMS.Provider
{
    public class AccessTokenDao : IDatabaseDao
    {
        private readonly Repository<AccessTokenInfo> _repository;
        public AccessTokenDao()
        {
            _repository = new Repository<AccessTokenInfo>(AppSettings.DatabaseType, AppSettings.ConnectionString);
        }

        public string TableName => _repository.TableName;
        public List<TableColumn> TableColumns => _repository.TableColumns;

        private static class Attr
        {
            public const string Title = nameof(AccessTokenInfo.Title);
        }

        public int Insert(AccessTokenInfo accessTokenInfo)
        {
            accessTokenInfo.Token = TranslateUtils.EncryptStringBySecretKey(StringUtils.GetGuid());
            accessTokenInfo.AddDate = DateTime.Now;

            accessTokenInfo.Id = _repository.Insert(accessTokenInfo);
            if (accessTokenInfo.Id > 0)
            {
                AccessTokenManager.ClearCache();
            }
            return accessTokenInfo.Id;
        }

        public bool Update(AccessTokenInfo accessTokenInfo)
        {
            var updated = _repository.Update(accessTokenInfo);
            if (updated)
            {
                AccessTokenManager.ClearCache();
            }
            return updated;
        }

        public bool Delete(int id)
        {
            var deleted = _repository.Delete(id);
            if (deleted)
            {
                AccessTokenManager.ClearCache();
            }
            return deleted;
        }

        public string Regenerate(AccessTokenInfo accessTokenInfo)
        {
            accessTokenInfo.Token = TranslateUtils.EncryptStringBySecretKey(StringUtils.GetGuid());

            Update(accessTokenInfo);

            return accessTokenInfo.Token;
        }

        public bool IsTitleExists(string title)
        {
            return _repository.Exists(Q.Where(Attr.Title, title));
        }

        public IList<AccessTokenInfo> GetAll()
        {
            return _repository.GetAll();
        }

        public AccessTokenInfo Get(int id)
        {
            return _repository.Get(id);
        }
    }
}
