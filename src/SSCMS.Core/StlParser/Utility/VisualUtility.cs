using System;
using System.Collections.Specialized;
using SSCMS.Parse;
using SSCMS.Utils;

namespace SSCMS.Core.StlParser.Utility
{
    public static class VisualUtility
    {
        public static Editable GetEditable(ParsePage page, ParseContext context)
        {
            var editable = new Editable
            {
                Id = StringUtils.GetElementId(),
                ElementName = context.ElementName,
                StlElement = StringUtils.Base64Encode(context.OuterHtml),

                File = string.IsNullOrEmpty(page.IncludeFile)
                ? string.Empty
                : StringUtils.Base64Encode(page.IncludeFile),
                Index = context.StartIndex,
            };
            page.Editables.Add(editable);
            return editable;
        }

        public static NameValueCollection GetEditableAttributes(Editable editable)
        {
            var attributes = new NameValueCollection(StringComparer.OrdinalIgnoreCase)
            {
                ["data-element"] = "true",
                ["data-element-id"] = editable.Id,
                ["data-element-name"] = editable.ElementName
            };
            return attributes;
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
