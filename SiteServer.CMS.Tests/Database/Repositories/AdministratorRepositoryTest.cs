using SiteServer.CMS.Database.Core;
using SiteServer.CMS.Database.Models;
using SiteServer.CMS.Tests.Database.Mocks;
using SiteServer.Utils;
using SiteServer.Utils.Enumerations;
using Xunit;
using Xunit.Abstractions;

namespace SiteServer.CMS.Tests.Database.Repositories
{
    [TestCaseOrderer("SiteServer.CMS.Tests.PriorityOrderer", "SiteServer.CMS.Tests")]
    public class AdministratorRepositoryTest : IClassFixture<EnvironmentFixture>
    {
        public EnvironmentFixture Fixture { get; }
        private readonly ITestOutputHelper _output;

        public AdministratorRepositoryTest(EnvironmentFixture fixture, ITestOutputHelper output)
        {
            Fixture = fixture;
            _output = output;
        }

        public const string TestUserName = "Tests_UserName";

        [SkippableFact, TestPriority(0)]
        public void TestInsert()
        {
            Skip.IfNot(TestEnv.IntegrationTestMachine);

            var adminInfo = new AdministratorInfo();

            var id = DataProvider.Administrator.Insert(adminInfo, out _);

            Assert.True(id == 0);

            adminInfo = new AdministratorInfo
            {
                UserName = TestUserName,
                Password = "InsertTest"
            };

            id = DataProvider.Administrator.Insert(adminInfo, out var errorMessage);
            _output.WriteLine(errorMessage);

            Assert.True(id == 0);

            adminInfo = new AdministratorInfo
            {
                UserName = TestUserName,
                Password = "InsertTest@2"
            };

            id = DataProvider.Administrator.Insert(adminInfo, out errorMessage);
            _output.WriteLine(errorMessage);

            Assert.True(id > 0);
            Assert.True(!string.IsNullOrWhiteSpace(adminInfo.Password));
            Assert.True(adminInfo.PasswordFormat == EPasswordFormatUtils.GetValue(EPasswordFormat.Encrypted));
            Assert.True(!string.IsNullOrWhiteSpace(adminInfo.PasswordSalt));
        }

        [SkippableFact, TestPriority(1)]
        public void TestUpdate()
        {
            Skip.IfNot(TestEnv.IntegrationTestMachine);

            var adminInfo = DataProvider.Administrator.GetByUserName(TestUserName);

            var password = adminInfo.Password;
            var passwordFormat = adminInfo.PasswordFormat;
            var passwordSalt = adminInfo.PasswordSalt;

            adminInfo.Password = "cccc@d";

            var updated = DataProvider.Administrator.Update(adminInfo, out _);
            Assert.True(updated);
            Assert.True(adminInfo.Password == password);
            Assert.True(adminInfo.PasswordFormat == passwordFormat);
            Assert.True(adminInfo.PasswordSalt == passwordSalt);
        }

        [SkippableFact, TestPriority(1)]
        public void TestUpdateLastActivityDateAndCountOfFailedLogin()
        {
            Skip.IfNot(TestEnv.IntegrationTestMachine);

            var adminInfo = DataProvider.Administrator.GetByUserName(TestUserName);
            Assert.NotNull(adminInfo);
            Assert.Equal(TestUserName, adminInfo.UserName);

            var countOfFailedLogin = adminInfo.CountOfFailedLogin;

            var updated = DataProvider.Administrator.UpdateLastActivityDateAndCountOfFailedLogin(adminInfo);
            Assert.True(updated);
            Assert.Equal(countOfFailedLogin, adminInfo.CountOfFailedLogin - 1);
        }

        [SkippableFact, TestPriority(2)]
        public void TestDelete()
        {
            Skip.IfNot(TestEnv.IntegrationTestMachine);

            var adminInfo = DataProvider.Administrator.GetByUserName(TestUserName);
            Assert.NotNull(adminInfo);
            Assert.Equal(TestUserName, adminInfo.UserName);

            var deleted = DataProvider.Administrator.Delete(adminInfo);

            Assert.True(deleted);
        }
    }
}
