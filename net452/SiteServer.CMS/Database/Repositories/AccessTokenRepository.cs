using System;
using Datory;
using SiteServer.CMS.Caches;
using SiteServer.CMS.Database.Models;
using SiteServer.Utils;

namespace SiteServer.CMS.Database.Repositories
{
    public class AccessTokenRepository : Repository<AccessTokenInfo>
    {
        public AccessTokenRepository() : base(WebConfigUtils.DatabaseType, WebConfigUtils.ConnectionString)
        {
        }

        private static class Attr
        {
            public const string Title = nameof(AccessTokenInfo.Title);
        }

        public override int Insert(AccessTokenInfo accessTokenInfo)
        {
            accessTokenInfo.Token = TranslateUtils.EncryptStringBySecretKey(StringUtils.GetGuid());
            accessTokenInfo.AddDate = DateTime.Now;

            accessTokenInfo.Id = base.Insert(accessTokenInfo);
            if (accessTokenInfo.Id > 0)
            {
                AccessTokenManager.ClearCache();
            }
            return accessTokenInfo.Id;
        }

        public override bool Update(AccessTokenInfo accessTokenInfo)
        {
            var updated = base.Update(accessTokenInfo);
            if (updated)
            {
                AccessTokenManager.ClearCache();
            }
            return updated;
        }

        public override bool Delete(int id)
        {
            var deleted = base.Delete(id);
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
