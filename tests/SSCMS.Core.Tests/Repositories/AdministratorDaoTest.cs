using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using SSCMS;
using SSCMS.Tests;
using Xunit;
using Xunit.Abstractions;

namespace SSCMS.Core.Tests.Repositories
{
    [Collection("Database collection")]
    public class UserRepositoryTest
    {
        private readonly IntegrationTestsFixture _fixture;
        private readonly IUserRepository _userRepository;
        private readonly ITestOutputHelper _output;

        private const string TestUserName = "Tests_UserName";

        public UserRepositoryTest(IntegrationTestsFixture fixture, ITestOutputHelper output)
        {
            _fixture = fixture;
            _userRepository = _fixture.Provider.GetService<IUserRepository>();
            _output = output;
        }

        [SkippableFact]
        public async Task TestInsert()
        {
            Skip.IfNot(TestEnv.IsTestMachine);

            var userInfo = new User();
            var (userId, errorMessage) = await _userRepository.InsertAsync(userInfo, "admin888", string.Empty);

            Assert.True(userId == 0);

            userInfo = new User
            {
                UserName = TestUserName
            };

            (userId, errorMessage) = await _userRepository.InsertAsync(userInfo, "InsertTest", string.Empty);
            _output.WriteLine(errorMessage);

            Assert.True(userId == 0);

            userInfo = new User
            {
                UserName = TestUserName
            };

            (userId, errorMessage) = await _userRepository.InsertAsync(userInfo, "InsertTest@2", string.Empty);
            if (userId == 0)
            {
                _output.WriteLine(errorMessage);
            }

            Assert.True(userId > 0);
            Assert.True(!string.IsNullOrWhiteSpace(userInfo.Password));
            Assert.True(userInfo.PasswordFormat == PasswordFormat.Encrypted);
            Assert.True(!string.IsNullOrWhiteSpace(userInfo.PasswordSalt));

            userInfo = await _userRepository.GetByUserNameAsync(TestUserName);

            var password = userInfo.Password;
            var passwordFormat = userInfo.PasswordFormat;
            var passwordSalt = userInfo.PasswordSalt;

            userInfo.Password = "cccc@d";

            await _userRepository.UpdateAsync(userInfo);
            Assert.True(userInfo.Password == password);
            Assert.True(userInfo.PasswordFormat == passwordFormat);
            Assert.True(userInfo.PasswordSalt == passwordSalt);

            userInfo = await _userRepository.GetByUserNameAsync(TestUserName);
            Assert.NotNull(userInfo);
            Assert.Equal(TestUserName, userInfo.UserName);

            var countOfFailedLogin = userInfo.CountOfFailedLogin;

            await _userRepository.UpdateLastActivityDateAndCountOfLoginAsync(userInfo);
            Assert.Equal(countOfFailedLogin, userInfo.CountOfFailedLogin - 1);

            userInfo = await _userRepository.GetByUserNameAsync(TestUserName);
            Assert.NotNull(userInfo);
            Assert.Equal(TestUserName, userInfo.UserName);

            await _userRepository.DeleteAsync(userInfo.Id);
        }
    }
}
