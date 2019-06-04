using System;
using System.Threading.Tasks;
using AngleSharp;
using AngleSharp.Dom;
using AngleSharp.Html.Dom;
using AngleSharp.Html.Parser;
using Xunit;

namespace SS.CMS.Core.Tests.StlParser
{
    [TestCaseOrderer("SS.CMS.Core.Tests.PriorityOrderer", "SS.CMS.Core.Tests")]
    public class StlParserTests : IClassFixture<EnvironmentFixture>
    {
        private readonly EnvironmentFixture _fixture;

        public StlParserTests(EnvironmentFixture fixture)
        {
            _fixture = fixture;
        }

        [Fact, TestPriority(0)]
        public async Task HtmlElementTest1()
        {
            var context = BrowsingContext.New(Configuration.Default);

            //Create a new document
            var document = await context.OpenAsync(req => req.Content("<b><i>This is some <em> bold <u>and</u> italic </em> text!</i></b>"));

            var select = document.CreateElement<IHtmlSelectElement>();
            select.Id = "select1";
            Console.WriteLine(select.ToHtml());

            Assert.True(true);
        }

        [Fact, TestPriority(0)]
        public void HtmlElementTest2()
        {
            var context = BrowsingContext.New(Configuration.Default);
            var parser = context.GetService<IHtmlParser>();
            var document = parser.ParseDocument(string.Empty);

            var select = document.CreateElement<IHtmlSelectElement>();
            select.Id = "select2";
            Console.WriteLine(select.ToHtml());

            Assert.True(true);
        }
    }
}
