using Datory;
using Moq;
using SSCMS.Core.Repositories;
using SSCMS.Repositories;
using SSCMS.Services;
using Xunit;

namespace SSCMS.Web.Tests.Security
{
    public class ChannelRepositorySqlSafetyTests
    {
        [Fact]
        public void GetWhereStringSanitizesGroupChannelNames()
        {
            var repository = CreateRepository();

            var where = repository.GetWhereString("news' OR 1=1--", null, false, false);

            Assert.DoesNotContain("news' OR", where);
            Assert.Contains("_sqlquote_", where);
        }

        private static ChannelRepository CreateRepository()
        {
            var settingsManager = new Mock<ISettingsManager>();
            settingsManager
                .Setup(x => x.DatabaseType)
                .Returns(DatabaseType.SQLite);
            settingsManager
                .Setup(x => x.Database)
                .Returns(new Database(DatabaseType.SQLite, "Data Source=:memory:"));

            return new ChannelRepository(settingsManager.Object, Mock.Of<ITemplateRepository>());
        }
    }
}
