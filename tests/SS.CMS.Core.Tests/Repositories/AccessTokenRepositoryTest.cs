using System.Linq;
using SS.CMS.Abstractions.Models;
using SS.CMS.Data;
using SS.CMS.Utils;
using Xunit;

namespace SS.CMS.Core.Tests.Repositories
{
    [TestCaseOrderer("SS.CMS.Core.Tests.PriorityOrderer", "SS.CMS.Core.Tests")]
    public class AccessTokenRepositoryTest : IClassFixture<EnvironmentFixture>
    {
        private readonly EnvironmentFixture _fixture;

        public AccessTokenRepositoryTest(EnvironmentFixture fixture)
        {
            _fixture = fixture;
        }

        [SkippableFact, TestPriority(0)]
        public void TestCreateTable()
        {
            Skip.IfNot(TestEnv.IntegrationTestMachine);

            var db = new Db(_fixture.SettingsManager.DatabaseType, _fixture.SettingsManager.DatabaseConnectionString);

            db.CreateTableAsync(_fixture.AccessTokenRepository.TableName, _fixture.AccessTokenRepository.TableColumns).GetAwaiter().GetResult();

            Assert.True(db.IsTableExistsAsync(_fixture.AccessTokenRepository.TableName).GetAwaiter().GetResult());
        }

        [SkippableFact, TestPriority(1)]
        public void TestBasic()
        {
            Skip.IfNot(TestEnv.IntegrationTestMachine);

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

        [SkippableFact, TestPriority(1)]
        public void TestIsTitleExists()
        {
            Skip.IfNot(TestEnv.IntegrationTestMachine);

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

        [SkippableFact, TestPriority(1)]
        public void TestGetAccessTokenInfoList()
        {
            Skip.IfNot(TestEnv.IntegrationTestMachine);

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

        [SkippableFact, TestPriority(2)]
        public void TestDropTable()
        {
            Skip.IfNot(TestEnv.IntegrationTestMachine);

            var db = new Db(_fixture.SettingsManager.DatabaseType, _fixture.SettingsManager.DatabaseConnectionString);

            db.DropTableAsync(_fixture.AccessTokenRepository.TableName).GetAwaiter().GetResult();

            Assert.False(db.IsTableExistsAsync(_fixture.AccessTokenRepository.TableName).GetAwaiter().GetResult());
        }
    }
}
