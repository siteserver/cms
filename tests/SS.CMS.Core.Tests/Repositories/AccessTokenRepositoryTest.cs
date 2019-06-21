using System;
using System.Collections.Generic;
using System.Linq;
using SS.CMS.Data;
using SS.CMS.Models;
using SS.CMS.Utils;
using SS.CMS.Utils.Tests;
using Xunit;

namespace SS.CMS.Core.Tests.Repositories
{
    [Collection("Database collection")]
    public class AccessTokenRepositoryTest
    {
        private readonly DatabaseFixture _fixture;

        public AccessTokenRepositoryTest(DatabaseFixture fixture)
        {
            _fixture = fixture;

            if (!TestEnv.IsTestMachine) return;
        }

        [SkippableFact]
        public void TestBasic()
        {
            Skip.IfNot(TestEnv.IsTestMachine);

            var accessTokenInfo = new AccessTokenInfo();
            _fixture.AccessTokenRepository.InsertAsync(accessTokenInfo).GetAwaiter().GetResult();
            Assert.True(accessTokenInfo.Id > 0);
            var token = accessTokenInfo.Token;
            Assert.False(string.IsNullOrWhiteSpace(token));

            accessTokenInfo = _fixture.AccessTokenRepository.GetAsync(accessTokenInfo.Id).GetAwaiter().GetResult();
            Assert.NotNull(accessTokenInfo);

            accessTokenInfo.Title = "title";
            var updated = _fixture.AccessTokenRepository.UpdateAsync(accessTokenInfo).GetAwaiter().GetResult();
            Assert.True(updated);

            _fixture.AccessTokenRepository.RegenerateAsync(accessTokenInfo).GetAwaiter().GetResult();
            Assert.NotEqual(token, accessTokenInfo.Token);

            var deleted = _fixture.AccessTokenRepository.DeleteAsync(accessTokenInfo.Id).GetAwaiter().GetResult();
            Assert.True(deleted);
        }

        [SkippableFact]
        public void TestIsTitleExists()
        {
            Skip.IfNot(TestEnv.IsTestMachine);

            const string testTitle = "IsTitleExists";

            var exists = _fixture.AccessTokenRepository.IsTitleExistsAsync(testTitle).GetAwaiter().GetResult();

            Assert.False(exists);

            var accessTokenInfo = new AccessTokenInfo
            {
                Title = testTitle
            };
            _fixture.AccessTokenRepository.InsertAsync(accessTokenInfo).GetAwaiter().GetResult();

            exists = _fixture.AccessTokenRepository.IsTitleExistsAsync(testTitle).GetAwaiter().GetResult();

            Assert.True(exists);

            var deleted = _fixture.AccessTokenRepository.DeleteAsync(accessTokenInfo.Id).GetAwaiter().GetResult();
            Assert.True(deleted);
        }

        [SkippableFact]
        public void TestGetAccessTokenInfoList()
        {
            Skip.IfNot(TestEnv.IsTestMachine);

            var accessTokenInfo = new AccessTokenInfo
            {
                Title = "title"
            };
            _fixture.AccessTokenRepository.InsertAsync(accessTokenInfo).GetAwaiter().GetResult();

            var list = _fixture.AccessTokenRepository.GetAllAsync().GetAwaiter().GetResult();

            Assert.True(list.Any());

            var deleted = _fixture.AccessTokenRepository.DeleteAsync(accessTokenInfo.Id).GetAwaiter().GetResult();
            Assert.True(deleted);
        }
    }
}
