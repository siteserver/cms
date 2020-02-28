using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using SS.CMS.Abstractions;
using SS.CMS.Abstractions.Parse;
using SS.CMS.StlParser.Model;
using SS.CMS.StlParser.Utility;
using Xunit;
using Xunit.Abstractions;

namespace SS.CMS.Tests.StlParser.StlElement
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
            var pageInfo = await ParsePage.GetPageInfoAsync(0, 0, siteInfo, templateInfo, pluginItems);

            var contextInfo = new ParseContext(pageInfo);

            var template = $@"<stl:a href=""https://www.siteserver.cn"">test</stl:a>";
            var builder = new StringBuilder(template);

            await StlParserManager.ParseTemplateContentAsync(builder, pageInfo, contextInfo);
            var parsedContent = builder.ToString();

            _output.WriteLine(parsedContent);

            Assert.True(true);
        }
    }
}
