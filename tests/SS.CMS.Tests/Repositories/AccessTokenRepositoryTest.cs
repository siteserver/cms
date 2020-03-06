﻿using System.Linq;
using System.Threading.Tasks;
using SS.CMS.Abstractions;
using SS.CMS.Abstractions.Tests;
using Xunit;
using Microsoft.Extensions.DependencyInjection;

namespace SS.CMS.Tests.Repositories
{
    [Collection("Database collection")]
    public class AccessTokenRepositoryTest
    {
        private readonly IntegrationTestsFixture _fixture;
        private readonly IAccessTokenRepository _accessTokenRepository;

        public AccessTokenRepositoryTest(IntegrationTestsFixture fixture)
        {
            _fixture = fixture;
            _accessTokenRepository = _fixture.Provider.GetService<IAccessTokenRepository>();

            if (!TestEnv.IsTestMachine) return;
        }

        [SkippableFact]
        public async Task TestBasic()
        {
            Skip.IfNot(TestEnv.IsTestMachine);

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

        [SkippableFact]
        public async Task TestIsTitleExists()
        {
            Skip.IfNot(TestEnv.IsTestMachine);

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

        [SkippableFact]
        public async Task TestGetAccessTokenInfoList()
        {
            Skip.IfNot(TestEnv.IsTestMachine);

            var accessTokenInfo = new AccessToken
            {
                Title = "title"
            };
            await _accessTokenRepository.InsertAsync(accessTokenInfo);

            var list = await _accessTokenRepository.GetAccessTokenListAsync();

            Assert.True(list.Any());

            var deleted = await _accessTokenRepository.DeleteAsync(accessTokenInfo.Id);
            Assert.True(deleted);
        }
    }
}
