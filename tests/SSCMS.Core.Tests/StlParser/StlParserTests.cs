using System.Threading.Tasks;
using AngleSharp;
using AngleSharp.Dom;
using AngleSharp.Html.Dom;
using AngleSharp.Html.Parser;
using Xunit;
using Xunit.Abstractions;

namespace SSCMS.Core.Tests.StlParser
{
    public class StlParserTests
    {
        private readonly ITestOutputHelper _output;

        public StlParserTests(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public async Task HtmlElementTest1()
        {
            var context = BrowsingContext.New(AngleSharp.Configuration.Default);

            //Create a new document
            var document = await context.OpenAsync(req => req.Content("<b><i>This is some <em> bold <u>and</u> italic </em> text!</i></b>"));

            var select = document.CreateElement<IHtmlSelectElement>();
            select.Id = "select1";
            _output.WriteLine(select.ToHtml());

            Assert.True(true);
        }

        [Fact]
        public void HtmlElementTest2()
        {
            var context = BrowsingContext.New(AngleSharp.Configuration.Default);
            var parser = context.GetService<IHtmlParser>();
            var document = parser.ParseDocument(string.Empty);

            var select = document.CreateElement<IHtmlSelectElement>();
            select.Id = "select2";
            _output.WriteLine(select.ToHtml());

            Assert.True(true);
        }
    }
}
