using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Datory;
using SiteServer.CMS.Model;
using SiteServer.Utils;

namespace SiteServer.CMS.Provider
{
    public partial class AccessTokenDao : IRepository
    {
        private readonly Repository<AccessToken> _repository;

        public AccessTokenDao()
        {
            _repository = new Repository<AccessToken>(new Database(WebConfigUtils.DatabaseType, WebConfigUtils.ConnectionString));
        }

        public IDatabase Database => _repository.Database;

        public string TableName => _repository.TableName;

        public List<TableColumn> TableColumns => _repository.TableColumns;

        public async Task<int> InsertAsync(AccessToken accessTokenInfo)
        {
            var token = WebConfigUtils.EncryptStringBySecretKey(StringUtils.Guid());
            accessTokenInfo.Token = token;
            accessTokenInfo.AddDate = DateTime.Now;
            accessTokenInfo.UpdatedDate = DateTime.Now;

            accessTokenInfo.Id = await _repository.InsertAsync(accessTokenInfo);
            if (accessTokenInfo.Id > 0)
            {
                await RemoveCacheAsync();
            }
            return accessTokenInfo.Id;
        }

        public async Task<bool> UpdateAsync(AccessToken accessTokenInfo)
        {
            var updated = await _repository.UpdateAsync(accessTokenInfo);
            if (updated)
            {
                await RemoveCacheAsync();
            }
            return updated;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var deleted = await _repository.DeleteAsync(id);
            if (deleted)
            {
                await RemoveCacheAsync();
            }
            return deleted;
        }

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
    }
}
