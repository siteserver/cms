using System.Text;
using System.Threading.Tasks;
using SSCMS.Core.StlParser.Attributes;
using SSCMS.Core.StlParser.Utility;
using SSCMS.Enums;
using SSCMS.Parse;
using SSCMS.Services;
using SSCMS.Utils;

namespace SSCMS.Core.StlParser.StlElement
{
    [StlElement(Title = "可视化编辑", Description = "通过 stl:editable 标签在模板中插入可视化编辑元素")]
    public static class StlEditable
    {
        public const string ElementName = "stl:editable";

        [StlAttribute(Title = "可视化编辑类型")]
        private const string Type = nameof(Type);

        public static async Task<object> ParseAsync(IParseManager parseManager)
        {
            var type = EditableType.Text;

            foreach (var name in parseManager.ContextInfo.Attributes.AllKeys)
            {
                var value = parseManager.ContextInfo.Attributes[name];

                if (StringUtils.EqualsIgnoreCase(name, Type))
                {
                    type = TranslateUtils.ToEnum(value, EditableType.Text);
                }
            }

            return await ParseAsync(parseManager, type);
        }

        private static async Task<string> ParseAsync(IParseManager parseManager, EditableType type)
        {
            var pageInfo = parseManager.PageInfo;
            var contextInfo = parseManager.ContextInfo;

            var parsedContent = string.Empty;
            if (!string.IsNullOrEmpty(contextInfo.InnerHtml))
            {
                var innerBuilder = new StringBuilder(contextInfo.InnerHtml);
                await parseManager.ParseInnerContentAsync(innerBuilder);
                parsedContent = innerBuilder.ToString();
            }

            if (pageInfo.EditMode == EditMode.Visual)
            {
                var editable = VisualUtility.GetEditable(EditableType.Text, pageInfo, contextInfo);
                var editableAttributes = VisualUtility.GetEditableAttributes(editable);

                return @$"<div id=""{editable.Id}"" contenteditable=""true"" {TranslateUtils.ToAttributesString(editableAttributes)}>{parsedContent}</div>";
            }

            return @$"<div>{parsedContent}</div>";
        }
    }
}