using SS.CMS.Core.Common;
using SS.CMS.Core.Models;
using SS.CMS.Utils;
using SS.CMS.Utils.Enumerations;
using Xunit;
using Xunit.Abstractions;

namespace SS.CMS.Core.Tests.Repositories
{
    [TestCaseOrderer("SS.CMS.Core.Tests.PriorityOrderer", "SS.CMS.Core.Tests")]
    public class AdministratorDaoTest : IClassFixture<EnvironmentFixture>
    {
        public EnvironmentFixture Fixture { get; }
        private readonly ITestOutputHelper _output;

        public AdministratorDaoTest(EnvironmentFixture fixture, ITestOutputHelper output)
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

            var id = DataProvider.AdministratorDao.Insert(adminInfo, out _);

            Assert.True(id == 0);

            adminInfo = new AdministratorInfo
            {
                UserName = TestUserName,
                Password = "InsertTest"
            };

            id = DataProvider.AdministratorDao.Insert(adminInfo, out var errorMessage);
            _output.WriteLine(errorMessage);

            Assert.True(id == 0);

            adminInfo = new AdministratorInfo
            {
                UserName = TestUserName,
                Password = "InsertTest@2"
            };

            id = DataProvider.AdministratorDao.Insert(adminInfo, out errorMessage);
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

            var adminInfo = DataProvider.AdministratorDao.GetByUserName(TestUserName);

            var password = adminInfo.Password;
            var passwordFormat = adminInfo.PasswordFormat;
            var passwordSalt = adminInfo.PasswordSalt;

            adminInfo.Password = "cccc@d";

            var updated = DataProvider.AdministratorDao.Update(adminInfo, out _);
            Assert.True(updated);
            Assert.True(adminInfo.Password == password);
            Assert.True(adminInfo.PasswordFormat == passwordFormat);
            Assert.True(adminInfo.PasswordSalt == passwordSalt);
        }

        [SkippableFact, TestPriority(1)]
        public void TestUpdateLastActivityDateAndCountOfFailedLogin()
        {
            Skip.IfNot(TestEnv.IntegrationTestMachine);

            var adminInfo = DataProvider.AdministratorDao.GetByUserName(TestUserName);
            Assert.NotNull(adminInfo);
            Assert.Equal(TestUserName, adminInfo.UserName);

            var countOfFailedLogin = adminInfo.CountOfFailedLogin;

            var updated = DataProvider.AdministratorDao.UpdateLastActivityDateAndCountOfFailedLogin(adminInfo);
            Assert.True(updated);
            Assert.Equal(countOfFailedLogin, adminInfo.CountOfFailedLogin - 1);
        }

        [SkippableFact, TestPriority(2)]
        public void TestDelete()
        {
            Skip.IfNot(TestEnv.IntegrationTestMachine);

            var adminInfo = DataProvider.AdministratorDao.GetByUserName(TestUserName);
            Assert.NotNull(adminInfo);
            Assert.Equal(TestUserName, adminInfo.UserName);

            var deleted = DataProvider.AdministratorDao.Delete(adminInfo);

            Assert.True(deleted);
        }
    }
}
