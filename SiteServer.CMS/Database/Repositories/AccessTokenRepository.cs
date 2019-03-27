using System;
using System.Collections.Generic;
using SiteServer.CMS.Caches;
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

        public void Insert(AccessTokenInfo accessTokenInfo)
        {
            accessTokenInfo.Token = TranslateUtils.EncryptStringBySecretKey(StringUtils.GetGuid());
            accessTokenInfo.AddDate = DateTime.Now;

            InsertObject(accessTokenInfo);
            AccessTokenManager.ClearCache();
        }

        public bool Update(AccessTokenInfo accessTokenInfo)
        {
            var updated = UpdateObject(accessTokenInfo);
            AccessTokenManager.ClearCache();
            return updated;
        }

        public bool Delete(int id)
        {
            var deleted = DeleteById(id);
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

        public IList<AccessTokenInfo> GetAll()
        {
            return GetObjectList();
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
