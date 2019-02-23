using System;
using System.Collections.Generic;
using SiteServer.CMS.Database.Caches;
using SiteServer.CMS.Database.Core;
using SiteServer.CMS.Database.Models;
using SiteServer.Utils;

namespace SiteServer.CMS.Database.Repositories
{
    public class AccessTokenRepository : GenericRepository<AccessTokenInfo>
    {
        private static class Attr
        {
            public const string Title = nameof(AccessTokenInfo.Title);
        }

        public new int Insert(AccessTokenInfo accessTokenInfo)
        {
            accessTokenInfo.Token = TranslateUtils.EncryptStringBySecretKey(StringUtils.GetGuid());
            accessTokenInfo.AddDate = DateTime.Now;

            var identity = base.InsertObject(accessTokenInfo);
            AccessTokenManager.ClearCache();
            return identity;
        }

        public new bool Update(AccessTokenInfo accessTokenInfo)
        {
            var updated = base.UpdateObject(accessTokenInfo);
            AccessTokenManager.ClearCache();
            return updated;
        }

        public new bool Delete(int id)
        {
            var deleted = base.DeleteById(id);
            AccessTokenManager.ClearCache();
            return deleted;
        }

        public string Regenerate(AccessTokenInfo accessTokenInfo)
        {
            accessTokenInfo.Token = TranslateUtils.EncryptStringBySecretKey(StringUtils.GetGuid());

            Update(accessTokenInfo);

            return accessTokenInfo.Token;
        }

        public AccessTokenInfo Get(int id)
        {
            return GetObjectById(id);
        }

        public AccessTokenInfo Get()
        {
            return GetObject();
        }

        public IList<AccessTokenInfo> GetAll()
        {
            return base.GetObjectList();
        }

        public bool IsTitleExists(string title)
        {
            return Exists(Q.Where(Attr.Title, title));
            //bool exists;

            //using (var connection = GetConnection())
            //{
            //    exists = connection.ExecuteScalar<bool>($"SELECT COUNT(1) FROM {TableName} WHERE {nameof(AccessTokenInfo.Title)} = @{nameof(AccessTokenInfo.Title)}", new
            //    {
            //        Title = title
            //    });
            //}

            //return exists;
        }
    }

}
