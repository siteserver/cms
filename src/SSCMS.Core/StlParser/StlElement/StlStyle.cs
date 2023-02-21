using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using SSCMS.Core.StlParser.Attributes;
using SSCMS.Services;
using SSCMS.Utils;

namespace SSCMS.Core.StlParser.StlElement
{
    [StlElement(Title = "自适应CSS样式", Description = "通过 stl:style 标签实现自适应媒体查询(Media Query)功能")]
    public static class StlStyle
    {
        public const string ElementName = "stl:style";

        [StlAttribute(Title = "类型")]
        private const string Type = nameof(Type);

        [StlAttribute(Title = "最小宽度")]
        private const string Min = nameof(Min);

        [StlAttribute(Title = "最大宽度")]
        private const string Max = nameof(Max);

        public const string TypeDesktop = "desktop";
        public const string TypeTablet = "tablet";
        public const string TypeMobile = "mobile";
        public const string TypeNotDesktop = "!desktop";
        public const string TypeNotTablet = "!tablet";
        public const string TypeNotMobile = "!mobile";

        public static SortedList<string, string> TypeList => new SortedList<string, string>
        {
            {TypeDesktop, "桌面"},
            {TypeTablet, "平板"},
            {TypeMobile, "手机"},
            {TypeNotDesktop, "非桌面"},
            {TypeNotTablet, "非平板"},
            {TypeNotMobile, "非手机"}
        };

        public static async Task<object> ParseAsync(IParseManager parseManager)
        {
            var type = string.Empty;
            var min = 0;
            var max = 0;

            foreach (var name in parseManager.ContextInfo.Attributes.AllKeys)
            {
                var value = parseManager.ContextInfo.Attributes[name];

                if (StringUtils.EqualsIgnoreCase(name, Type))
                {
                    type = value;
                }
                else if (StringUtils.EqualsIgnoreCase(name, Min))
                {
                    min = TranslateUtils.ToInt(StringUtils.ReplaceIgnoreCase(value, "px", string.Empty));
                }
                else if (StringUtils.EqualsIgnoreCase(name, Max))
                {
                    max = TranslateUtils.ToInt(StringUtils.ReplaceIgnoreCase(value, "px", string.Empty));
                }
            }

            return await ParseAsync(parseManager, type, min, max);
        }

        private static async Task<string> ParseAsync(IParseManager parseManager, string type, int min, int max)
        {
            var pageInfo = parseManager.PageInfo;
            var contextInfo = parseManager.ContextInfo;

            if (string.IsNullOrEmpty(contextInfo.InnerHtml)) return string.Empty;
            var innerHtml = string.Empty;
            var innerBuilder = new StringBuilder(contextInfo.InnerHtml);
            await parseManager.ParseInnerContentAsync(innerBuilder);
            innerHtml = innerBuilder.ToString();

            var logic = "and";
            if (min == 0 && max == 0 && !string.IsNullOrEmpty(type))
            {
                if (StringUtils.EqualsIgnoreCase(type, TypeDesktop))
                {
                    min = 1025;
                }
                else if (StringUtils.EqualsIgnoreCase(type, TypeTablet))
                {
                    min = 481;
                    max = 1024;
                }
                else if (StringUtils.EqualsIgnoreCase(type, TypeMobile))
                {
                    max = 480;
                }
                else if (StringUtils.EqualsIgnoreCase(type, TypeNotDesktop))
                {
                    max = 1024;
                }
                else if (StringUtils.EqualsIgnoreCase(type, TypeNotTablet))
                {
                    max = 480;
                    min = 1025;
                    logic = ",";
                }
                else if (StringUtils.EqualsIgnoreCase(type, TypeNotMobile))
                {
                    min = 481;
                }
            }

            var mediaStart = string.Empty;
            if (min > 0 && max > 0)
            {
                mediaStart = $"@media (min-width: {min}px) {logic} (max-width: {max}px) {{";
            }
            else if (min > 0)
            {
                mediaStart = $"@media (min-width: {min}px) {{";
            }
            else if (max > 0)
            {
                mediaStart = $"@media (max-width: {max}px) {{";
            }
            var mediaEnd = string.IsNullOrEmpty(mediaStart) ? string.Empty : "}";

            return $@"
<style>
{mediaStart}
  {innerHtml}
{mediaEnd}
</style>
";
        }
    }
}
