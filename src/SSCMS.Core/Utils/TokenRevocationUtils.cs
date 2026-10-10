using System;
using System.Threading.Tasks;
using SSCMS.Configuration;
using SSCMS.Repositories;
using SSCMS.Utils;

namespace SSCMS.Core.Utils
{
    /// <summary>
    /// JWT 吊销（#3894）：为每个账号记录"令牌生效起点"，签发时间(iat)早于该时间点的令牌一律拒绝。
    /// 键使用 SessionIdPrefix 前缀，确保"清除缓存"(ClearAllExceptAdminSessionsAsync) 时不会被清掉。
    /// </summary>
    public static class TokenRevocationUtils
    {
        public static string GetCacheKey(bool isAdmin, int id)
        {
            return $"{Constants.SessionIdPrefix}TOKEN-VALID-AFTER-{(isAdmin ? "A" : "U")}-{id}";
        }

        public static async Task RevokeAsync(IDbCacheRepository dbCacheRepository, bool isAdmin, int id)
        {
            if (dbCacheRepository == null || id <= 0) return;
            var now = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            await dbCacheRepository.RemoveAndInsertAsync(GetCacheKey(isAdmin, id), now.ToString());
        }

        public static async Task<bool> IsRevokedAsync(IDbCacheRepository dbCacheRepository, bool isAdmin, int id, DateTime issuedAtUtc)
        {
            if (dbCacheRepository == null || id <= 0) return false;
            var value = await dbCacheRepository.GetValueAsync(GetCacheKey(isAdmin, id));
            if (string.IsNullOrEmpty(value) || !long.TryParse(value, out var validAfter)) return false;
            // 没有 iat 的令牌视为已吊销
            if (issuedAtUtc == DateTime.MinValue) return true;
            var issuedAt = new DateTimeOffset(DateTime.SpecifyKind(issuedAtUtc, DateTimeKind.Utc)).ToUnixTimeSeconds();
            return issuedAt < validAfter;
        }
    }
}
