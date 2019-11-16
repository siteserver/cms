using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Datory;
using SiteServer.CMS.Data;
using SiteServer.CMS.DataCache;
using SiteServer.CMS.Model;
using SiteServer.Utils;

namespace SiteServer.CMS.Provider
{
    public class AccessTokenDao : IRepository
    {
        private readonly Repository<AccessToken> _repository;

        public AccessTokenDao()
        {
            _repository = new Repository<AccessToken>(new Database(WebConfigUtils.DatabaseType, WebConfigUtils.ConnectionString));
        }

        public IDatabase Database => _repository.Database;

        public string TableName => _repository.TableName;

        public List<TableColumn> TableColumns => _repository.TableColumns;

        //public override string TableName => "siteserver_AccessToken";

        //public override List<TableColumn> TableColumns => new List<TableColumn>
        //{
        //    new TableColumn
        //    {
        //        AttributeName = nameof(AccessTokenInfo.Id),
        //        DataType = DataType.Integer,
        //        IsIdentity = true,
        //        IsPrimaryKey = true
        //    },
        //    new TableColumn
        //    {
        //        AttributeName = nameof(AccessTokenInfo.Title),
        //        DataType = DataType.VarChar,
        //        DataLength = 200
        //    },
        //    new TableColumn
        //    {
        //        AttributeName = nameof(AccessTokenInfo.Token),
        //        DataType = DataType.VarChar,
        //        DataLength = 200
        //    },
        //    new TableColumn
        //    {
        //        AttributeName = nameof(AccessTokenInfo.AdminName),
        //        DataType = DataType.VarChar,
        //        DataLength = 200
        //    },
        //    new TableColumn
        //    {
        //        AttributeName = nameof(AccessTokenInfo.Scopes),
        //        DataType = DataType.VarChar,
        //        DataLength = 200
        //    },
        //    new TableColumn
        //    {
        //        AttributeName = nameof(AccessTokenInfo.RateLimit),
        //        DataType = DataType.Integer
        //    },
        //    new TableColumn
        //    {
        //        AttributeName = nameof(AccessTokenInfo.AddDate),
        //        DataType = DataType.DateTime
        //    },
        //    new TableColumn
        //    {
        //        AttributeName = nameof(AccessTokenInfo.UpdatedDate),
        //        DataType = DataType.DateTime
        //    }
        //};

        public async Task<int> InsertAsync(AccessToken accessTokenInfo)
        {
            var token = WebConfigUtils.EncryptStringBySecretKey(StringUtils.Guid());
            accessTokenInfo.Token = token;
            accessTokenInfo.AddDate = DateTime.Now;
            accessTokenInfo.UpdatedDate = DateTime.Now;

            accessTokenInfo.Id = await _repository.InsertAsync(accessTokenInfo);
            if (accessTokenInfo.Id > 0)
            {
                AccessTokenManager.ClearCache();
            }
            return accessTokenInfo.Id;

            //       var sqlString = $@"INSERT INTO {TableName}
            //      ({nameof(AccessTokenInfo.Title)}, 
            //       {nameof(AccessTokenInfo.Token)},
            //       {nameof(AccessTokenInfo.AdminName)},
            //       {nameof(AccessTokenInfo.Scopes)},
            //       {nameof(AccessTokenInfo.RateLimit)},
            //       {nameof(AccessTokenInfo.AddDate)},
            //       {nameof(AccessTokenInfo.UpdatedDate)})
            //VALUES
            //      (@{nameof(AccessTokenInfo.Title)}, 
            //       @{nameof(AccessTokenInfo.Token)},
            //       @{nameof(AccessTokenInfo.AdminName)},
            //       @{nameof(AccessTokenInfo.Scopes)},
            //       @{nameof(AccessTokenInfo.RateLimit)},
            //       @{nameof(AccessTokenInfo.AddDate)},
            //       @{nameof(AccessTokenInfo.UpdatedDate)})";

            //       IDataParameter[] parameters = 
            //       {
            //           GetParameter(nameof(accessTokenInfo.Title), DataType.VarChar, 200, accessTokenInfo.Title),
            //           GetParameter(nameof(accessTokenInfo.Token), DataType.VarChar, 200, token),
            //           GetParameter(nameof(accessTokenInfo.AdminName), DataType.VarChar, 200, accessTokenInfo.AdminName),
            //           GetParameter(nameof(accessTokenInfo.Scopes), DataType.VarChar, 200, accessTokenInfo.Scopes),
            //           GetParameter(nameof(accessTokenInfo.RateLimit), DataType.Integer, accessTokenInfo.RateLimit),
            //           GetParameter(nameof(accessTokenInfo.AddDate), DataType.DateTime, DateTime.Now),
            //           GetParameter(nameof(accessTokenInfo.UpdatedDate), DataType.DateTime, DateTime.Now)
            //       };

            //       ExecuteNonQuery(sqlString, parameters);

            //AccessTokenManager.ClearCache();
        }

        //public void Update(AccessTokenInfo accessTokenInfo)
        //{
        //    var sqlString = $@"UPDATE {TableName} SET
        //        {nameof(AccessTokenInfo.Title)} = @{nameof(AccessTokenInfo.Title)}, 
        //        {nameof(AccessTokenInfo.AdminName)} = @{nameof(AccessTokenInfo.AdminName)},
        //        {nameof(AccessTokenInfo.Scopes)} = @{nameof(AccessTokenInfo.Scopes)},
        //        {nameof(AccessTokenInfo.RateLimit)} = @{nameof(AccessTokenInfo.RateLimit)},
        //        {nameof(AccessTokenInfo.UpdatedDate)} = @{nameof(AccessTokenInfo.UpdatedDate)}
        //    WHERE {nameof(AccessTokenInfo.Id)} = @{nameof(AccessTokenInfo.Id)}";

        //    IDataParameter[] parameters =
        //    {
        //        GetParameter(nameof(accessTokenInfo.Title), DataType.VarChar, 200, accessTokenInfo.Title),
        //        GetParameter(nameof(accessTokenInfo.AdminName), DataType.VarChar, 200, accessTokenInfo.AdminName),
        //        GetParameter(nameof(accessTokenInfo.Scopes), DataType.VarChar, 200, accessTokenInfo.Scopes),
        //        GetParameter(nameof(accessTokenInfo.RateLimit), DataType.VarChar, 200, accessTokenInfo.RateLimit),
        //        GetParameter(nameof(accessTokenInfo.UpdatedDate), DataType.DateTime, DateTime.Now),
        //        GetParameter(nameof(accessTokenInfo.Id), DataType.Integer, accessTokenInfo.Id)
        //    };

        //    ExecuteNonQuery(sqlString, parameters);

        //    AccessTokenManager.ClearCache();
        //}

        public async Task<bool> UpdateAsync(AccessToken accessTokenInfo)
        {
            var updated = await _repository.UpdateAsync(accessTokenInfo);
            if (updated)
            {
                AccessTokenManager.ClearCache();
            }
            return updated;
        }

        //public void Delete(int id)
        //{
        //    if (id <= 0) return;

        //    var sqlString = $"DELETE FROM {TableName} WHERE {nameof(AccessTokenInfo.Id)} = {id}";
        //    ExecuteNonQuery(sqlString);

        //    AccessTokenManager.ClearCache();
        //}

        public async Task<bool> DeleteAsync(int id)
        {
            var deleted = await _repository.DeleteAsync(id);
            if (deleted)
            {
                AccessTokenManager.ClearCache();
            }
            return deleted;
        }

        //public string Regenerate(int id)
        //{
        //    var token = TranslateUtils.EncryptStringBySecretKey(StringUtils.Guid());

        //    var sqlString = $@"UPDATE {TableName} SET
        //        {nameof(AccessTokenInfo.Token)} = @{nameof(AccessTokenInfo.Token)}, 
        //        {nameof(AccessTokenInfo.UpdatedDate)} = @{nameof(AccessTokenInfo.UpdatedDate)}
        //    WHERE {nameof(AccessTokenInfo.Id)} = @{nameof(AccessTokenInfo.Id)}";

        //    IDataParameter[] parameters =
        //    {
        //        GetParameter(nameof(AccessTokenInfo.Token), DataType.VarChar, 200, token),
        //        GetParameter(nameof(AccessTokenInfo.UpdatedDate), DataType.DateTime, DateTime.Now),
        //        GetParameter(nameof(AccessTokenInfo.Id), DataType.Integer, id)
        //    };

        //    ExecuteNonQuery(sqlString, parameters);

        //    AccessTokenManager.ClearCache();

        //    return token;
        //}

        public async Task<string> RegenerateAsync(AccessToken accessTokenInfo)
        {
            accessTokenInfo.Token = WebConfigUtils.EncryptStringBySecretKey(StringUtils.Guid());

            await UpdateAsync(accessTokenInfo);

            return accessTokenInfo.Token;
        }

        public async Task<bool> IsTitleExistsAsync(string title)
        {
            return await _repository.ExistsAsync(Q.Where(nameof(AccessToken.Title), title));

            //var exists = false;

            //var sqlString = $@"SELECT {nameof(AccessTokenInfo.Id)} FROM {TableName} WHERE {nameof(AccessTokenInfo.Title)} = @{nameof(AccessTokenInfo.Title)}";

            //IDataParameter[] parameters =
            //{
            //    GetParameter(nameof(AccessTokenInfo.Title), DataType.VarChar, 200, title)
            //};

            //using (var rdr = ExecuteReader(sqlString, parameters))
            //{
            //    if (rdr.Read() && !rdr.IsDBNull(0))
            //    {
            //        exists = true;
            //    }
            //    rdr.Close();
            //}

            //return exists;
        }

        public async Task<IEnumerable<AccessToken>> GetAccessTokenInfoListAsync()
        {
            return await _repository.GetAllAsync(Q.OrderBy(nameof(AccessToken.Id)));

            //var list = new List<AccessTokenInfo>();

            //var sqlString = $@"SELECT {nameof(AccessTokenInfo.Id)}, 
            //    {nameof(AccessTokenInfo.Title)}, 
            //    {nameof(AccessTokenInfo.Token)},
            //    {nameof(AccessTokenInfo.AdminName)},
            //    {nameof(AccessTokenInfo.Scopes)},
            //    {nameof(AccessTokenInfo.RateLimit)},
            //    {nameof(AccessTokenInfo.AddDate)},
            //    {nameof(AccessTokenInfo.UpdatedDate)}
            //FROM {TableName} ORDER BY {nameof(AccessTokenInfo.Id)}";

            //using (var rdr = ExecuteReader(sqlString))
            //{
            //    while (rdr.Read())
            //    {
            //        list.Add(GetAccessTokenInfo(rdr));
            //    }
            //    rdr.Close();
            //}

            //return list;
        }

        public async Task<AccessToken> GetAsync(int id)
        {
            return await _repository.GetAsync(id);

            //AccessTokenInfo accessTokenInfo = null;

            //var sqlString = $@"SELECT {nameof(AccessTokenInfo.Id)}, 
            //    {nameof(AccessTokenInfo.Title)}, 
            //    {nameof(AccessTokenInfo.Token)},
            //    {nameof(AccessTokenInfo.AdminName)},
            //    {nameof(AccessTokenInfo.Scopes)},
            //    {nameof(AccessTokenInfo.RateLimit)},
            //    {nameof(AccessTokenInfo.AddDate)},
            //    {nameof(AccessTokenInfo.UpdatedDate)}
            //FROM {TableName} WHERE {nameof(AccessTokenInfo.Id)} = {id}";

            //using (var rdr = ExecuteReader(sqlString))
            //{
            //    if (rdr.Read())
            //    {
            //        accessTokenInfo = GetAccessTokenInfo(rdr);
            //    }
            //    rdr.Close();
            //}

            //return accessTokenInfo;
        }

        public async Task<Dictionary<string, AccessToken>> GetAccessTokenInfoDictionaryAsync()
        {
            var dictionary = new Dictionary<string, AccessToken>();

            foreach (var accessTokenInfo in await _repository.GetAllAsync())
            {
                var token = WebConfigUtils.DecryptStringBySecretKey(accessTokenInfo.Token);
                if (!string.IsNullOrEmpty(token))
                {
                    dictionary[token] = accessTokenInfo;
                }
            }

            //var sqlString = $@"SELECT {nameof(AccessTokenInfo.Id)}, 
            //{nameof(AccessTokenInfo.Title)}, 
            //{nameof(AccessTokenInfo.Token)},
            //{nameof(AccessTokenInfo.AdminName)},
            //{nameof(AccessTokenInfo.Scopes)},
            //{nameof(AccessTokenInfo.RateLimit)},
            //{nameof(AccessTokenInfo.AddDate)},
            //{nameof(AccessTokenInfo.UpdatedDate)}
            //FROM {TableName}";

            //using (var rdr = ExecuteReader(sqlString))
            //{
            //    while (rdr.Read())
            //    {
            //        var accessTokenInfo = GetAccessTokenInfo(rdr);
            //        var token = TranslateUtils.DecryptStringBySecretKey(accessTokenInfo.Token);
            //        if (!string.IsNullOrEmpty(token))
            //        {
            //            dictionary[token] = accessTokenInfo;
            //        }
            //    }
            //    rdr.Close();
            //}

            return dictionary;
        }

        //private static AccessTokenInfo GetAccessTokenInfo(IDataRecord rdr)
        //{
        //    if (rdr == null) return null;

        //    var accessTokenInfo = new AccessTokenInfo();

        //    var i = 0;
        //    accessTokenInfo.Id = rdr.IsDBNull(i) ? 0 : rdr.GetInt32(i);
        //    i++;
        //    accessTokenInfo.Title = rdr.IsDBNull(i) ? string.Empty : rdr.GetString(i);
        //    i++;
        //    accessTokenInfo.Token = rdr.IsDBNull(i) ? string.Empty : rdr.GetString(i);
        //    i++;
        //    accessTokenInfo.AdminName = rdr.IsDBNull(i) ? string.Empty : rdr.GetString(i);
        //    i++;
        //    accessTokenInfo.Scopes = rdr.IsDBNull(i) ? string.Empty : rdr.GetString(i);
        //    i++;
        //    accessTokenInfo.RateLimit = rdr.IsDBNull(i) ? 0 : rdr.GetInt32(i);
        //    i++;
        //    accessTokenInfo.AddDate = rdr.IsDBNull(i) ? DateTime.Now : rdr.GetDateTime(i);
        //    i++;
        //    accessTokenInfo.UpdatedDate = rdr.IsDBNull(i) ? DateTime.Now : rdr.GetDateTime(i);

        //    return accessTokenInfo;
        //}

    }
}
