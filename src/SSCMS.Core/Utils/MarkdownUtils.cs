using Markdig;
using SSCMS.Utils;

namespace SSCMS.Core.Utils
{
    public static class MarkdownUtils
    {
        public static string GetYamlFrontMatter(string markdown)
        {
            if (string.IsNullOrEmpty(markdown) || !markdown.StartsWith("---") ||
                StringUtils.GetCount("---", markdown) < 2) return string.Empty;

            var content = RegexUtils.GetContent("content", "---(?<content>[\\s\\S]+?)---", markdown);
            return StringUtils.Trim(content);
        }

        public static string RemoveYamlFrontMatter(string markdown)
        {
            if (string.IsNullOrEmpty(markdown) || !markdown.StartsWith("---") ||
                StringUtils.GetCount("---", markdown) < 2) return string.Empty;

            var content = RegexUtils.Replace("---(?<content>[\\s\\S]+?)---", markdown, string.Empty);
            return StringUtils.Trim(content);
        }

        public static string ToHtml(string markdown)
        {
            if (string.IsNullOrEmpty(markdown)) return string.Empty;

            var pipeline = new MarkdownPipelineBuilder().UseAdvancedExtensions().Build();
            var html = Markdown.ToHtml(markdown, pipeline);
            if (!string.IsNullOrEmpty(html))
            {
                html = html.Replace("<a ", @"<a target=""_blank"" ");
            }

            return html;
        }
    }
}