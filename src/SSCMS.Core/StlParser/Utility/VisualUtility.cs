using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using SSCMS.Core.StlParser.StlElement;
using SSCMS.Parse;
using SSCMS.Utils;

namespace SSCMS.Core.StlParser.Utility
{
    public static class VisualUtility
    {
        public static void AddEditableToPage(ParsePage page, ParseContext context, NameValueCollection attributes, string innerHtml)
        {
            var elementId = StringUtils.GetElementId();
            var editable = new Editable
            {
                ElementId = elementId,
                ElementName = context.ElementName,
                Attributes = TranslateUtils.ToDictionary(attributes),
                InnerHtml = innerHtml,
                StlElement = StringUtils.Base64Encode(context.OuterHtml),
                IncludeFile = string.IsNullOrEmpty(page.IncludeFile)
                ? string.Empty
                : StringUtils.Base64Encode(page.IncludeFile),
                StartIndex = context.StartIndex,
                IsChanged = false
            };
            page.Editables.Add(editable);

            attributes["data-element"] = "true";
            attributes["data-element-id"] = elementId;
            attributes["data-element-name"] = context.ElementName;
            if (StringUtils.EqualsIgnoreCase(context.ElementName, StlEditable.ElementName))
            {
                attributes["id"] = elementId;
                attributes["contenteditable"] = "true";
            }
        }

        //public static string Parse(ParsePage page, ParseContext context, EditableType type, string parsedContent)
        //{
        //    var editable = GetEditable(type, page, context, parsedContent);

        //    if (editable.Type == EditableType.Text)
        //    {
        //        return @$"<div class=""s-element"" id=""{editable.Id}"" contenteditable=""true"">{editable.ParsedContent}</div>";
        //    }
        //    if (editable.Type == EditableType.Media)
        //    {
        //        return @$"<div class=""s-element"" id=""{editable.Id}"">{editable.ParsedContent}</div>";
        //    }

        //    return string.Empty;

        //}
    }
}
