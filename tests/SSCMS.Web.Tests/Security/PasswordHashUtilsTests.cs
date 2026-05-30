using SSCMS.Utils;
using Xunit;

namespace SSCMS.Web.Tests.Security
{
    public class PasswordHashUtilsTests
    {
        [Fact]
        public void HashPasswordStoresOneWayVerifierCompatibleWithMd5Submissions()
        {
            const string password = "Password1";
            var hash = PasswordHashUtils.HashPassword(password, out var salt);

            Assert.NotEqual(password, hash);
            Assert.DoesNotContain(password, hash);
            Assert.True(PasswordHashUtils.VerifyPassword(password, false, hash, salt));
            Assert.True(PasswordHashUtils.VerifyPassword(AuthUtils.Md5ByString(password), true, hash, salt));
            Assert.False(PasswordHashUtils.VerifyPassword("wrong-password", false, hash, salt));
        }
    }
}
