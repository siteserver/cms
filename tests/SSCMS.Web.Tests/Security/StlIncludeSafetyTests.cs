using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using System.Threading.Tasks;
using Moq;
using SSCMS.Core.StlParser.StlElement;
using SSCMS.Enums;
using SSCMS.Models;
using SSCMS.Parse;
using SSCMS.Services;
using Xunit;

namespace SSCMS.Web.Tests.Security
{
    public class StlIncludeSafetyTests
    {
        [Fact]
        public async Task IncludeRejectsRequestsAfterMaxDepth()
        {
            const string file = "/loop.html";
            var (parseManager, pathManager, page) = CreateParseManager(file, "recursive");
            page.IncludeDepth = 32;

            var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => StlInclude.ParseAsync(parseManager.Object));

            Assert.Contains("stl:include", ex.Message, StringComparison.OrdinalIgnoreCase);
            pathManager.Verify(x => x.GetIncludeContentAsync(It.IsAny<Site>(), It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public async Task IncludeRestoresDepthAfterSuccessfulParse()
        {
            const string file = "/header.html";
            var (parseManager, _, page) = CreateParseManager(file, "header");
            page.IncludeDepth = 2;

            await StlInclude.ParseAsync(parseManager.Object);

            Assert.Equal(2, page.IncludeDepth);
            parseManager.Verify(x => x.ParseTemplateContentAsync(It.IsAny<StringBuilder>()), Times.Once);
        }

        private static (Mock<IParseManager> ParseManager, Mock<IPathManager> PathManager, ParsePage Page) CreateParseManager(string file, string includeContent)
        {
            var pathManager = new Mock<IPathManager>();
            var site = new Site { Id = 1 };
            var template = new Template { TemplateType = TemplateType.IndexPageTemplate };
            var page = new ParsePage(pathManager.Object, EditMode.Default, new Config(), 0, 0, 0, site, template, new Dictionary<string, object>());
            var context = new ParseContext(page)
            {
                Attributes = new NameValueCollection
                {
                    { "file", file }
                }
            };

            pathManager.Setup(x => x.AddVirtualToUrl(file)).Returns(file);
            pathManager.Setup(x => x.GetIncludeContentAsync(site, file)).ReturnsAsync(includeContent);

            var parseManager = new Mock<IParseManager>();
            parseManager.SetupGet(x => x.PathManager).Returns(pathManager.Object);
            parseManager.SetupGet(x => x.PageInfo).Returns(page);
            parseManager.SetupGet(x => x.ContextInfo).Returns(context);
            parseManager
                .Setup(x => x.ReplaceStlEntitiesForAttributeValueAsync(It.IsAny<string>()))
                .ReturnsAsync((string value) => value);
            parseManager
                .Setup(x => x.ParseTemplateContentAsync(It.IsAny<StringBuilder>()))
                .Returns(Task.CompletedTask);

            return (parseManager, pathManager, page);
        }
    }
}
