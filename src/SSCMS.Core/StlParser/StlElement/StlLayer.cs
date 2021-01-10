using System.Text;
using System.Threading.Tasks;
using SSCMS.Core.StlParser.Attributes;
using SSCMS.Parse;
using SSCMS.Services;
using SSCMS.Utils;

namespace SSCMS.Core.StlParser.StlElement
{
    [StlElement(Title = "页面弹层", Description = "通过 stl:layer 标签在模板中显示弹层组件")]
    public static class StlLayer
    {
        public const string ElementName = "stl:layer";

        [StlAttribute(Title = "触发函数名称")]
        private const string FuncName = nameof(FuncName);

        [StlAttribute(Title = "标题")]
        private const string Title = nameof(Title);

        [StlAttribute(Title = "Url地址")]
        private const string Url = nameof(Url);

        [StlAttribute(Title = "宽度")]
        private const string Width = nameof(Width);

        [StlAttribute(Title = "高度")]
        private const string Height = nameof(Height);

        [StlAttribute(Title = "开启遮罩关闭")]
        private const string ShadeClose = nameof(ShadeClose);

        [StlAttribute(Title = "坐标")]
        private const string Offset = nameof(Offset);

        public static async Task<object> ParseAsync(IParseManager parseManager)
        {
            var funcName = string.Empty;
            var title = string.Empty;
            var url = string.Empty;
            var width = string.Empty;
            var height = string.Empty;
            var shadeClose = true;
            var offset = "auto";

            foreach (var name in parseManager.ContextInfo.Attributes.AllKeys)
            {
                var value = parseManager.ContextInfo.Attributes[name];

                if (StringUtils.EqualsIgnoreCase(name, FuncName))
                {
                    funcName = await parseManager.ReplaceStlEntitiesForAttributeValueAsync(value);
                }
                else if (StringUtils.EqualsIgnoreCase(name, Title))
                {
                    title = await parseManager.ReplaceStlEntitiesForAttributeValueAsync(value);
                }
                else if (StringUtils.EqualsIgnoreCase(name, Url))
                {
                    url = await parseManager.ReplaceStlEntitiesForAttributeValueAsync(value);
                }
                else if (StringUtils.EqualsIgnoreCase(name, Width))
                {
                    width = value;
                    if (!string.IsNullOrEmpty(width) && char.IsDigit(width[^1]))
                    {
                        width += "px";
                    }
                }
                else if (StringUtils.EqualsIgnoreCase(name, Height))
                {
                    height = value;
                    if (!string.IsNullOrEmpty(height) && char.IsDigit(height[^1]))
                    {
                        height += "px";
                    }
                }
                else if (StringUtils.EqualsIgnoreCase(name, ShadeClose))
                {
                    shadeClose = TranslateUtils.ToBool(value);
                }
                else if (StringUtils.EqualsIgnoreCase(name, Offset))
                {
                    offset = value;
                    if (!string.IsNullOrEmpty(offset) && char.IsDigit(offset[^1]))
                    {
                        offset += "px";
                    }
                }
            }

            return await ParseAsync(parseManager, funcName, title, url, width, height, shadeClose, offset);
        }

        private static async Task<string> ParseAsync(IParseManager parseManager, string funcName, string title,
            string url, string width, string height, bool shadeClose, string offset)
        {
            var pageInfo = parseManager.PageInfo;
            var contextInfo = parseManager.ContextInfo;

            await pageInfo.AddPageHeadCodeIfNotExistsAsync(ParsePage.Const.Jquery);
            await pageInfo.AddPageHeadCodeIfNotExistsAsync(ParsePage.Const.Layer);

            var type = 1;
            var content = string.Empty;
            if (!string.IsNullOrEmpty(url))
            {
                type = 2;
                content = $"'{url}'";
            }
            else if (!string.IsNullOrEmpty(contextInfo.InnerHtml))
            {
                var innerBuilder = new StringBuilder(contextInfo.InnerHtml);
                await parseManager.ParseInnerContentAsync(innerBuilder);
                var elementId = StringUtils.GetElementId();
                pageInfo.BodyCodes.Add(elementId,
                    $@"<div id=""{elementId}"" style=""display: none"">{innerBuilder}</div>");
                content = $"$('#{elementId}')";
            }

            var area = string.Empty;
            if (!string.IsNullOrEmpty(width) || !string.IsNullOrEmpty(height))
            {
                area = string.IsNullOrEmpty(height)
                    ? $@"
area: '{width}',"
                    : $@"
area: ['{width}', '{height}'],";
            }

            var offsetStr = StringUtils.StartsWith(offset, "[") ? offset : $"'{offset}'";

            var script =
                $@"layer.open({{type: {type},{area}shadeClose: {shadeClose.ToString().ToLower()},offset:{offsetStr},title: '{title}',content: {content}}});";

            return !string.IsNullOrEmpty(funcName)
                ? $@"<script>function {funcName}(){{{script}}}</script>"
                : $@"<script>$(document).ready(function() {{{script}}});</script>";
        }
    }
}
