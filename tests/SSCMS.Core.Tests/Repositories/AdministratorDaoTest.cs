using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using SSCMS.Enums;
using SSCMS.Models;
using SSCMS.Repositories;
using Xunit;
using Xunit.Abstractions;

namespace SSCMS.Core.Tests.Repositories
{
    [Collection("Database collection")]
    public class UserRepositoryTest
    {
        private readonly IUserRepository _userRepository;
        private readonly ITestOutputHelper _output;

        private const string TestUserName = "Tests_UserName";

        public UserRepositoryTest(IntegrationTestsFixture fixture, ITestOutputHelper output)
        {
            _userRepository = fixture.Provider.GetService<IUserRepository>();
            _output = output;
        }

        [Fact]
        public async Task TestInsert()
        {
            var user = await _userRepository.GetByUserNameAsync(TestUserName);
            if (user != null)
            {
                await _userRepository.DeleteAsync(user.Id);
            }

            var userInfo = new User();
            string errorMessage;
            var (entity, _) = await _userRepository.InsertAsync(userInfo, "admin888", string.Empty);

            Assert.Null(entity);

            userInfo = new User
            {
                UserName = TestUserName
            };

            (entity, errorMessage) = await _userRepository.InsertAsync(userInfo, "InsertTest", string.Empty);
            _output.WriteLine(errorMessage);

            Assert.Null(entity);

            userInfo = new User
            {
                UserName = TestUserName
            };

            (entity, errorMessage) = await _userRepository.InsertAsync(userInfo, "InsertTest@2", string.Empty);
            if (entity == null)
            {
                _output.WriteLine(errorMessage);
            }

            Assert.NotNull(entity);
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
            Assert.Equal(0, countOfFailedLogin);

            userInfo = await _userRepository.GetByUserNameAsync(TestUserName);
            Assert.NotNull(userInfo);
            Assert.Equal(TestUserName, userInfo.UserName);

            await _userRepository.DeleteAsync(userInfo.Id);
        }
    }
}
