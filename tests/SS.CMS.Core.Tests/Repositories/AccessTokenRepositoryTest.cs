using System.Linq;
using SS.CMS.Abstractions.Models;
using SS.CMS.Abstractions.Repositories;
using SS.CMS.Core.Models;
using SS.CMS.Core.Repositories;
using SS.CMS.Utils;
using Xunit;

namespace SS.CMS.Core.Tests.Repositories
{
    [TestCaseOrderer("SS.CMS.Core.Tests.PriorityOrderer", "SS.CMS.Core.Tests")]
    public class AccessTokenRepositoryTest : IClassFixture<EnvironmentFixture>
    {
        private readonly EnvironmentFixture _fixture;
        private readonly IAccessTokenRepository _accessTokenRepository;

        public AccessTokenRepositoryTest(EnvironmentFixture fixture)
        {
            _fixture = fixture;
            _accessTokenRepository = new AccessTokenRepository(fixture.Db, fixture.AppSettings, fixture.MemoryCache);
        }

        [SkippableFact, TestPriority(0)]
        public void TestCreateTable()
        {
            Skip.IfNot(TestEnv.IntegrationTestMachine);

            _fixture.Db.CreateTableAsync(_accessTokenRepository.TableName, _accessTokenRepository.TableColumns).GetAwaiter().GetResult();

            Assert.True(_fixture.Db.IsTableExistsAsync(_accessTokenRepository.TableName).GetAwaiter().GetResult());
        }

        [SkippableFact, TestPriority(1)]
        public void TestBasic()
        {
            Skip.IfNot(TestEnv.IntegrationTestMachine);

            var accessTokenInfo = new AccessTokenInfo();
            _accessTokenRepository.InsertAsync(accessTokenInfo).GetAwaiter().GetResult();
            Assert.True(accessTokenInfo.Id > 0);
            var token = accessTokenInfo.Token;
            Assert.False(string.IsNullOrWhiteSpace(token));

            accessTokenInfo = _accessTokenRepository.GetAsync(accessTokenInfo.Id).GetAwaiter().GetResult();
            Assert.NotNull(accessTokenInfo);

            accessTokenInfo.Title = "title";
            var updated = _accessTokenRepository.UpdateAsync(accessTokenInfo).GetAwaiter().GetResult();
            Assert.True(updated);

            _accessTokenRepository.RegenerateAsync(accessTokenInfo).GetAwaiter().GetResult();
            Assert.NotEqual(token, accessTokenInfo.Token);

            var deleted = _accessTokenRepository.DeleteAsync(accessTokenInfo.Id).GetAwaiter().GetResult();
            Assert.True(deleted);
        }

        [SkippableFact, TestPriority(1)]
        public void TestIsTitleExists()
        {
            Skip.IfNot(TestEnv.IntegrationTestMachine);

            const string testTitle = "IsTitleExists";

            var exists = _accessTokenRepository.IsTitleExistsAsync(testTitle).GetAwaiter().GetResult();

            Assert.False(exists);

            var accessTokenInfo = new AccessTokenInfo
            {
                Title = testTitle
            };
            _accessTokenRepository.InsertAsync(accessTokenInfo).GetAwaiter().GetResult();

            exists = _accessTokenRepository.IsTitleExistsAsync(testTitle).GetAwaiter().GetResult();

            Assert.True(exists);

            var deleted = _accessTokenRepository.DeleteAsync(accessTokenInfo.Id).GetAwaiter().GetResult();
            Assert.True(deleted);
        }

        [SkippableFact, TestPriority(1)]
        public void TestGetAccessTokenInfoList()
        {
            Skip.IfNot(TestEnv.IntegrationTestMachine);

            var accessTokenInfo = new AccessTokenInfo
            {
                Title = "title"
            };
            _accessTokenRepository.InsertAsync(accessTokenInfo).GetAwaiter().GetResult();

            var list = _accessTokenRepository.GetAllAsync().GetAwaiter().GetResult();

            Assert.True(list.Any());

            var deleted = _accessTokenRepository.DeleteAsync(accessTokenInfo.Id).GetAwaiter().GetResult();
            Assert.True(deleted);
        }

        [SkippableFact, TestPriority(2)]
        public void TestDropTable()
        {
            Skip.IfNot(TestEnv.IntegrationTestMachine);

            _fixture.Db.DropTableAsync(_accessTokenRepository.TableName).GetAwaiter().GetResult();

            Assert.False(_fixture.Db.IsTableExistsAsync(_accessTokenRepository.TableName).GetAwaiter().GetResult());
        }
    }
}
