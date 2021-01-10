using System.Collections.Specialized;
using System.Text;
using System.Threading.Tasks;
using SSCMS.Core.StlParser.Attributes;
using SSCMS.Core.StlParser.Utility;
using SSCMS.Enums;
using SSCMS.Services;
using SSCMS.Utils;

namespace SSCMS.Core.StlParser.StlElement
{
    [StlElement(Title = "可视化编辑", Description = "通过 stl:editable 标签在模板中插入可视化编辑元素")]
    public static class StlEditable
    {
        public const string ElementName = "stl:editable";

        public static async Task<object> ParseAsync(IParseManager parseManager)
        {
            var attributes = new NameValueCollection();

            foreach (var name in parseManager.ContextInfo.Attributes.AllKeys)
            {
                var value = parseManager.ContextInfo.Attributes[name];

                attributes[name] = value;
            }

            return await ParseAsync(parseManager, attributes);
        }

        private static async Task<string> ParseAsync(IParseManager parseManager, NameValueCollection attributes)
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
                var editable = VisualUtility.GetEditable(pageInfo, contextInfo);
                var editableAttributes = VisualUtility.GetEditableAttributes(editable);
                foreach (var key in editableAttributes.AllKeys)
                {
                    attributes[key] = editableAttributes[key];
                }

                attributes["id"] = editable.Id;
                attributes["contenteditable"] = "true";
            }

            return @$"<div {TranslateUtils.ToAttributesString(attributes)}>{parsedContent}</div>";
        }
    }
}