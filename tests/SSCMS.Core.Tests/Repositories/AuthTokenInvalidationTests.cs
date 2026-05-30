using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using SSCMS.Models;
using SSCMS.Repositories;
using SSCMS.Services;
using Xunit;

namespace SSCMS.Core.Tests.Repositories
{
    [Collection("Database collection")]
    public class AuthTokenInvalidationTests
    {
        private readonly IAdministratorRepository _administratorRepository;
        private readonly IUserRepository _userRepository;
        private readonly ICacheManager _cacheManager;

        public AuthTokenInvalidationTests(IntegrationTestsFixture fixture)
        {
            _administratorRepository = fixture.Provider.GetService<IAdministratorRepository>();
            _userRepository = fixture.Provider.GetService<IUserRepository>();
            _cacheManager = fixture.Provider.GetService<ICacheManager>();
        }

        [Fact]
        public async Task UserChangePasswordInvalidatesCachedToken()
        {
            const string userName = "Tests_Token_User";
            var existing = await _userRepository.GetByUserNameAsync(userName);
            if (existing != null)
            {
                await _userRepository.DeleteAsync(existing.Id);
            }

            var (user, errorMessage) = await _userRepository.InsertAsync(new User
            {
                UserName = userName
            }, "Password1", true, "127.0.0.1");
            Assert.True(user != null, errorMessage);

            var cacheKey = $"user:{user.Id}:token";
            _cacheManager.AddOrUpdate(cacheKey, "old-token");

            var (success, changeErrorMessage) = await _userRepository.ChangePasswordAsync(user.Id, "Password2");

            Assert.True(success, changeErrorMessage);
            Assert.False(_cacheManager.Exists(cacheKey));

            await _userRepository.DeleteAsync(user.Id);
        }

        [Fact]
        public async Task AdministratorChangePasswordInvalidatesCachedToken()
        {
            const string userName = "Tests_Token_Admin";
            var existing = await _administratorRepository.GetByUserNameAsync(userName);
            if (existing != null)
            {
                await _administratorRepository.DeleteAsync(existing.Id);
            }

            var administrator = new Administrator
            {
                UserName = userName
            };
            var (isValid, errorMessage) = await _administratorRepository.InsertAsync(administrator, "Password1");
            Assert.True(isValid, errorMessage);

            administrator = await _administratorRepository.GetByUserNameAsync(userName);
            var cacheKey = $"admin:{administrator.Id}:token";
            _cacheManager.AddOrUpdate(cacheKey, "old-token");

            var (changeIsValid, changeErrorMessage) = await _administratorRepository.ChangePasswordAsync(administrator, "Password2");

            Assert.True(changeIsValid, changeErrorMessage);
            Assert.False(_cacheManager.Exists(cacheKey));

            await _administratorRepository.DeleteAsync(administrator.Id);
        }
    }
}
