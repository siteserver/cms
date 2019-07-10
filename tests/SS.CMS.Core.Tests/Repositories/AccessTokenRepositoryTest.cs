using System.Linq;
using System.Threading.Tasks;
using SS.CMS.Models;
using SS.CMS.Utils.Tests;
using Xunit;

namespace SS.CMS.Core.Tests.Repositories
{
    [Collection("Database collection")]
    public class AccessTokenRepositoryTest
    {
        private readonly IntegrationTestsFixture _fixture;

        public AccessTokenRepositoryTest(IntegrationTestsFixture fixture)
        {
            _fixture = fixture;

            if (!TestEnv.IsTestMachine) return;
        }

        [SkippableFact]
        public async Task TestBasic()
        {
            Skip.IfNot(TestEnv.IsTestMachine);

            var accessTokenInfo = new AccessToken();
            await _fixture.AccessTokenRepository.InsertAsync(accessTokenInfo);
            Assert.True(accessTokenInfo.Id > 0);
            var token = accessTokenInfo.Token;
            Assert.False(string.IsNullOrWhiteSpace(token));

            accessTokenInfo = await _fixture.AccessTokenRepository.GetAsync(accessTokenInfo.Id);
            Assert.NotNull(accessTokenInfo);

            accessTokenInfo.Title = "title";
            var updated = await _fixture.AccessTokenRepository.UpdateAsync(accessTokenInfo);
            Assert.True(updated);

            await _fixture.AccessTokenRepository.RegenerateAsync(accessTokenInfo);
            Assert.NotEqual(token, accessTokenInfo.Token);

            var deleted = await _fixture.AccessTokenRepository.DeleteAsync(accessTokenInfo.Id);
            Assert.True(deleted);
        }

        [SkippableFact]
        public async Task TestIsTitleExists()
        {
            Skip.IfNot(TestEnv.IsTestMachine);

            const string testTitle = "IsTitleExists";

            var exists = await _fixture.AccessTokenRepository.IsTitleExistsAsync(testTitle);

            Assert.False(exists);

            var accessTokenInfo = new AccessToken
            {
                Title = testTitle
            };
            await _fixture.AccessTokenRepository.InsertAsync(accessTokenInfo);

            exists = await _fixture.AccessTokenRepository.IsTitleExistsAsync(testTitle);

            Assert.True(exists);

            var deleted = await _fixture.AccessTokenRepository.DeleteAsync(accessTokenInfo.Id);
            Assert.True(deleted);
        }

        [SkippableFact]
        public async Task TestGetAccessTokenInfoList()
        {
            Skip.IfNot(TestEnv.IsTestMachine);

            var accessTokenInfo = new AccessToken
            {
                Title = "title"
            };
            await _fixture.AccessTokenRepository.InsertAsync(accessTokenInfo);

            var list = await _fixture.AccessTokenRepository.GetAllAsync();

            Assert.True(list.Any());

            var deleted = await _fixture.AccessTokenRepository.DeleteAsync(accessTokenInfo.Id);
            Assert.True(deleted);
        }
    }
}
