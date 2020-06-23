using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using SSCMS.Models;
using SSCMS.Repositories;
using Xunit;

namespace SSCMS.Core.Tests.Repositories
{
    [Collection("Database collection")]
    public class AccessTokenRepositoryTest
    {
        private readonly IAccessTokenRepository _accessTokenRepository;

        public AccessTokenRepositoryTest(IntegrationTestsFixture fixture)
        {
            _accessTokenRepository = fixture.Provider.GetService<IAccessTokenRepository>();
        }

        [Fact]
        public async Task TestBasic()
        {
            var accessTokenInfo = new AccessToken();
            await _accessTokenRepository.InsertAsync(accessTokenInfo);
            Assert.True(accessTokenInfo.Id > 0);
            var token = accessTokenInfo.Token;
            Assert.False(string.IsNullOrWhiteSpace(token));

            accessTokenInfo = await _accessTokenRepository.GetAsync(accessTokenInfo.Id);
            Assert.NotNull(accessTokenInfo);

            accessTokenInfo.Title = "title";
            var updated = await _accessTokenRepository.UpdateAsync(accessTokenInfo);
            Assert.True(updated);

            await _accessTokenRepository.RegenerateAsync(accessTokenInfo);
            Assert.NotEqual(token, accessTokenInfo.Token);

            var deleted = await _accessTokenRepository.DeleteAsync(accessTokenInfo.Id);
            Assert.True(deleted);
        }

        [Fact]
        public async Task TestIsTitleExists()
        {
            const string testTitle = "IsTitleExists";

            var exists = await _accessTokenRepository.IsTitleExistsAsync(testTitle);

            Assert.False(exists);

            var accessTokenInfo = new AccessToken
            {
                Title = testTitle
            };
            await _accessTokenRepository.InsertAsync(accessTokenInfo);

            exists = await _accessTokenRepository.IsTitleExistsAsync(testTitle);

            Assert.True(exists);

            var deleted = await _accessTokenRepository.DeleteAsync(accessTokenInfo.Id);
            Assert.True(deleted);
        }

        [Fact]
        public async Task TestGetAccessTokenInfoList()
        {
            var accessTokenInfo = new AccessToken
            {
                Title = "title"
            };
            await _accessTokenRepository.InsertAsync(accessTokenInfo);

            var list = await _accessTokenRepository.GetAccessTokensAsync();

            Assert.True(list.Any());

            var deleted = await _accessTokenRepository.DeleteAsync(accessTokenInfo.Id);
            Assert.True(deleted);
        }
    }
}
