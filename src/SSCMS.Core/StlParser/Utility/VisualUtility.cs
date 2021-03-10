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
        public static void AddEditableToPage(ParsePage page, string elementId, ParseContext context, string parsedContent)
        {
            var editable = new Editable
            {
                Id = elementId,
                ElementName = context.ElementName,
                StlElement = StringUtils.Base64Encode(context.OuterHtml),
                EditedContent = parsedContent,
                ParsedContent = parsedContent,
                File = string.IsNullOrEmpty(page.IncludeFile)
                ? string.Empty
                : StringUtils.Base64Encode(page.IncludeFile),
                Index = context.StartIndex,
            };
            page.Editables.Add(editable);
        }

        public static void AddEditableToAttributes(Dictionary<string, string> attributes, string elementId, string elementName)
        {
            attributes["data-element"] = "true";
            attributes["data-element-id"] = elementId;
            attributes["data-element-name"] = elementName;
        }

        public static void AddEditableToAttributes(NameValueCollection attributes, string elementId, string elementName)
        {
            attributes["data-element"] = "true";
            attributes["data-element-id"] = elementId;
            attributes["data-element-name"] = elementName;
            if (elementName == StlEditable.ElementName)
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
