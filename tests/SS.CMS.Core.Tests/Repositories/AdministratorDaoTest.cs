using System;
using System.Threading.Tasks;
using SS.CMS.Enums;
using SS.CMS.Models;
using SS.CMS.Utils.Tests;
using Xunit;
using Xunit.Abstractions;

namespace SS.CMS.Core.Tests.Repositories
{
    [Collection("Database collection")]
    public class UserRepositoryTest
    {
        private readonly IntegrationTestsFixture _fixture;
        private readonly ITestOutputHelper _output;

        private const string TestUserName = "Tests_UserName";

        public UserRepositoryTest(IntegrationTestsFixture fixture, ITestOutputHelper output)
        {
            _fixture = fixture;
            _output = output;
        }

        [SkippableFact]
        public async Task TestInsert()
        {
            Skip.IfNot(TestEnv.IsTestMachine);

            var userInfo = new UserInfo();
            var (isSuccess, id, errorMessage) = await _fixture.UserRepository.InsertAsync(userInfo);

            Assert.True(id == 0);

            userInfo = new UserInfo
            {
                UserName = TestUserName,
                Password = "InsertTest"
            };

            (isSuccess, id, errorMessage) = await _fixture.UserRepository.InsertAsync(userInfo);
            _output.WriteLine(errorMessage);

            Assert.True(id == 0);

            userInfo = new UserInfo
            {
                UserName = TestUserName,
                Password = "InsertTest@2"
            };

            (isSuccess, id, errorMessage) = await _fixture.UserRepository.InsertAsync(userInfo);
            if (!isSuccess)
            {
                _output.WriteLine(errorMessage);
            }

            Assert.True(id > 0);
            Assert.True(!string.IsNullOrWhiteSpace(userInfo.Password));
            Assert.True(userInfo.PasswordFormat == PasswordFormat.Encrypted.Value);
            Assert.True(!string.IsNullOrWhiteSpace(userInfo.PasswordSalt));

            userInfo = await _fixture.UserRepository.GetByUserNameAsync(TestUserName);

            var password = userInfo.Password;
            var passwordFormat = userInfo.PasswordFormat;
            var passwordSalt = userInfo.PasswordSalt;

            userInfo.Password = "cccc@d";

            var (updated, _) = await _fixture.UserRepository.UpdateAsync(userInfo);
            Assert.True(updated);
            Assert.True(userInfo.Password == password);
            Assert.True(userInfo.PasswordFormat == passwordFormat);
            Assert.True(userInfo.PasswordSalt == passwordSalt);

            userInfo = await _fixture.UserRepository.GetByUserNameAsync(TestUserName);
            Assert.NotNull(userInfo);
            Assert.Equal(TestUserName, userInfo.UserName);

            var countOfFailedLogin = userInfo.CountOfFailedLogin;

            updated = await _fixture.UserRepository.UpdateLastActivityDateAndCountOfFailedLoginAsync(userInfo);
            Assert.True(updated);
            Assert.Equal(countOfFailedLogin, userInfo.CountOfFailedLogin - 1);

            userInfo = await _fixture.UserRepository.GetByUserNameAsync(TestUserName);
            Assert.NotNull(userInfo);
            Assert.Equal(TestUserName, userInfo.UserName);

            var deleted = await _fixture.UserRepository.DeleteAsync(userInfo);

            Assert.True(deleted);
        }
    }
}
