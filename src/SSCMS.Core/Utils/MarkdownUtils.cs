using Markdig;

namespace SSCMS.Core.Utils
{
    public static class MarkdownUtils
    {
        public static string ToHtml(string markdown)
        {
            var html = Markdown.ToHtml(markdown);
            if (!string.IsNullOrEmpty(html))
            {
                html = html.Replace("<a ", @"<a target=""_blank"" ");
            }

            return html;
        }
    }
}