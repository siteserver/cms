using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using SS.CMS.Core.StlParser;
using SS.CMS.Core.StlParser.Models;
using SS.CMS.Models;
using SS.CMS.Utils.Tests;
using Xunit;
using Xunit.Abstractions;

namespace SS.CMS.Core.Tests.StlParser.StlElement
{
    [Collection("Database collection")]
    public class StlATests
    {
        private readonly IntegrationTestsFixture _fixture;
        private readonly ITestOutputHelper _output;

        public StlATests(IntegrationTestsFixture fixture, ITestOutputHelper output)
        {
            _fixture = fixture;
            _output = output;
        }

        [Fact]
        public async Task ParseTest()
        {
            var siteInfo = new Site();
            var templateInfo = new Template();
            var pluginItems = new Dictionary<string, object>();
            var pageInfo = new PageInfo(0, 0, siteInfo, templateInfo, pluginItems);

            var contextInfo = new ParseContext(pageInfo, _fixture.Configuration, _fixture.Cache, _fixture.SettingsManager, _fixture.PluginManager, _fixture.PathManager, _fixture.UrlManager, _fixture.FileManager, _fixture.SiteRepository, _fixture.ChannelRepository, _fixture.UserRepository, _fixture.TableStyleRepository, _fixture.TemplateRepository, _fixture.TagRepository, _fixture.ErrorLogRepository);

            var template = $@"<stl:a href=""https://www.siteserver.cn"">test</stl:a>";
            var builder = new StringBuilder(template);

            await contextInfo.ParseTemplateContentAsync(builder);
            var parsedContent = builder.ToString();

            _output.WriteLine(parsedContent);

            Assert.True(true);
        }
    }
}
