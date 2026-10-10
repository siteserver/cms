using SSCMS.Core.Utils;
using Xunit;

namespace SSCMS.Core.Tests.Utils
{
    public class AccountValidateUtilsTests
    {
        [Fact]
        public void Valid_Input_Passes()
        {
            var (isValid, _) = AccountValidateUtils.Validate("admin", "管理员", "a@b.com", "13800138000", "/avatar.png");
            Assert.True(isValid);
        }

        [Fact]
        public void Null_Fields_Are_Skipped()
        {
            var (isValid, _) = AccountValidateUtils.Validate(null, null, null, null, null);
            Assert.True(isValid);
        }

        [Fact]
        public void Overlong_UserName_Fails()
        {
            var (isValid, msg) = AccountValidateUtils.Validate(new string('a', 51), null, null, null, null);
            Assert.False(isValid);
            Assert.DoesNotContain("siteserver", msg);
        }

        [Fact]
        public void Overlong_Email_Fails()
        {
            var email = new string('a', 100) + "@b.com";
            Assert.False(AccountValidateUtils.Validate(null, null, email, null, null).IsValid);
        }

        [Fact]
        public void Invalid_Email_Format_Fails()
        {
            Assert.False(AccountValidateUtils.Validate(null, null, "not-an-email", null, null).IsValid);
        }

        [Fact]
        public void Invalid_Mobile_Fails()
        {
            Assert.False(AccountValidateUtils.Validate(null, null, null, "12345", null).IsValid);
            Assert.False(AccountValidateUtils.Validate(null, null, null, new string('1', 30), null).IsValid);
        }
    }
}
