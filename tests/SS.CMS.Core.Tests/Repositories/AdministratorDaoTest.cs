using SS.CMS.Core.Models;
using SS.CMS.Core.Repositories;
using SS.CMS.Enums;
using SS.CMS.Models;
using SS.CMS.Utils;
using SS.CMS.Utils.Enumerations;
using Xunit;
using Xunit.Abstractions;

namespace SS.CMS.Core.Tests.Repositories
{
    [TestCaseOrderer("SS.CMS.Core.Tests.PriorityOrderer", "SS.CMS.Core.Tests")]
    public class UserRepositoryTest : IClassFixture<EnvironmentFixture>
    {
        private readonly EnvironmentFixture _fixture;
        private readonly ITestOutputHelper _output;

        public UserRepositoryTest(EnvironmentFixture fixture, ITestOutputHelper output)
        {
            _fixture = fixture;
            _output = output;
        }

        public const string TestUserName = "Tests_UserName";

        [SkippableFact, TestPriority(0)]
        public void TestInsert()
        {
            Skip.IfNot(TestEnv.IntegrationTestMachine);

            var userInfo = new UserInfo();
            var id = _fixture.UserRepository.Insert(userInfo, out _);

            Assert.True(id == 0);

            userInfo = new UserInfo
            {
                UserName = TestUserName,
                Password = "InsertTest"
            };

            id = _fixture.UserRepository.Insert(userInfo, out var errorMessage);
            _output.WriteLine(errorMessage);

            Assert.True(id == 0);

            userInfo = new UserInfo
            {
                UserName = TestUserName,
                Password = "InsertTest@2"
            };

            id = _fixture.UserRepository.Insert(userInfo, out errorMessage);
            _output.WriteLine(errorMessage);

            Assert.True(id > 0);
            Assert.True(!string.IsNullOrWhiteSpace(userInfo.Password));
            Assert.True(userInfo.PasswordFormat == PasswordFormat.Encrypted.Value);
            Assert.True(!string.IsNullOrWhiteSpace(userInfo.PasswordSalt));
        }

        [SkippableFact, TestPriority(1)]
        public void TestUpdate()
        {
            Skip.IfNot(TestEnv.IntegrationTestMachine);

            var userInfo = _fixture.UserRepository.GetByUserName(TestUserName);

            var password = userInfo.Password;
            var passwordFormat = userInfo.PasswordFormat;
            var passwordSalt = userInfo.PasswordSalt;

            userInfo.Password = "cccc@d";

            var updated = _fixture.UserRepository.Update(userInfo, out _);
            Assert.True(updated);
            Assert.True(userInfo.Password == password);
            Assert.True(userInfo.PasswordFormat == passwordFormat);
            Assert.True(userInfo.PasswordSalt == passwordSalt);
        }

        [SkippableFact, TestPriority(1)]
        public void TestUpdateLastActivityDateAndCountOfFailedLogin()
        {
            Skip.IfNot(TestEnv.IntegrationTestMachine);

            var userInfo = _fixture.UserRepository.GetByUserName(TestUserName);
            Assert.NotNull(userInfo);
            Assert.Equal(TestUserName, userInfo.UserName);

            var countOfFailedLogin = userInfo.CountOfFailedLogin;

            var updated = _fixture.UserRepository.UpdateLastActivityDateAndCountOfFailedLogin(userInfo);
            Assert.True(updated);
            Assert.Equal(countOfFailedLogin, userInfo.CountOfFailedLogin - 1);
        }

        [SkippableFact, TestPriority(2)]
        public void TestDelete()
        {
            Skip.IfNot(TestEnv.IntegrationTestMachine);

            var userInfo = _fixture.UserRepository.GetByUserName(TestUserName);
            Assert.NotNull(userInfo);
            Assert.Equal(TestUserName, userInfo.UserName);

            var deleted = _fixture.UserRepository.Delete(userInfo);

            Assert.True(deleted);
        }
    }
}
