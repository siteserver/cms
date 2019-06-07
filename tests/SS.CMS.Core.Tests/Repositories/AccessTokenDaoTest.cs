using System.Linq;
using System.Threading.Tasks;
using SS.CMS.Core.Common;
using SS.CMS.Core.Models;
using SS.CMS.Utils;
using Xunit;

namespace SS.CMS.Core.Tests.Repositories
{
    [TestCaseOrderer("SS.CMS.Core.Tests.PriorityOrderer", "SS.CMS.Core.Tests")]
    public class AccessTokenDaoTest : IClassFixture<EnvironmentFixture>
    {
        public EnvironmentFixture Fixture { get; }

        public AccessTokenDaoTest(EnvironmentFixture fixture)
        {
            Fixture = fixture;
        }

        [SkippableFact, TestPriority(0)]
        public async Task TestBasic()
        {
            Skip.IfNot(TestEnv.IntegrationTestMachine);

            var accessTokenInfo = new AccessTokenInfo();

            await DataProvider.AccessTokenDao.InsertAsync(accessTokenInfo);
            Assert.True(accessTokenInfo.Id > 0);
            var token = accessTokenInfo.Token;
            Assert.False(string.IsNullOrWhiteSpace(token));

            accessTokenInfo = await DataProvider.AccessTokenDao.GetAsync(accessTokenInfo.Id);
            Assert.NotNull(accessTokenInfo);

            accessTokenInfo.Title = "title";
            var updated = await DataProvider.AccessTokenDao.UpdateAsync(accessTokenInfo);
            Assert.True(updated);

            await DataProvider.AccessTokenDao.RegenerateAsync(accessTokenInfo);
            Assert.NotEqual(token, accessTokenInfo.Token);

            var deleted = await DataProvider.AccessTokenDao.DeleteAsync(accessTokenInfo.Id);
            Assert.True(deleted);
        }

        [SkippableFact, TestPriority(0)]
        public async Task TestIsTitleExists()
        {
            Skip.IfNot(TestEnv.IntegrationTestMachine);

            const string testTitle = "IsTitleExists";

            var exists = await DataProvider.AccessTokenDao.IsTitleExistsAsync(testTitle);

            Assert.False(exists);

            var accessTokenInfo = new AccessTokenInfo
            {
                Title = testTitle
            };
            await DataProvider.AccessTokenDao.InsertAsync(accessTokenInfo);

            exists = await DataProvider.AccessTokenDao.IsTitleExistsAsync(testTitle);

            Assert.True(exists);

            var deleted = await DataProvider.AccessTokenDao.DeleteAsync(accessTokenInfo.Id);
            Assert.True(deleted);
        }

        [SkippableFact, TestPriority(0)]
        public async Task TestGetAccessTokenInfoList()
        {
            Skip.IfNot(TestEnv.IntegrationTestMachine);

            var accessTokenInfo = new AccessTokenInfo
            {
                Title = "title"
            };
            await DataProvider.AccessTokenDao.InsertAsync(accessTokenInfo);

            var list = DataProvider.AccessTokenDao.GetAll();

            Assert.True(list.Any());

            var deleted = await DataProvider.AccessTokenDao.DeleteAsync(accessTokenInfo.Id);
            Assert.True(deleted);
        }
    }
}
