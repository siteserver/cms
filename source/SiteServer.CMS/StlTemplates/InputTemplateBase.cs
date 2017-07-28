using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using BaiRong.Core.Model;
using BaiRong.Core.Model.Enumerations;
using SiteServer.CMS.Core;
using SiteServer.CMS.Model;

namespace SiteServer.CMS.StlTemplates
{
    public class InputTemplateBase
    {
        protected InputTemplateBase() { }

        public string GetInnerAdditionalAttributes(TableStyleInfo styleInfo)
        {
            var additionalAttributes = string.Empty;

            var inputType = EInputTypeUtils.GetEnumType(styleInfo.InputType);

            if (inputType == EInputType.Text || inputType == EInputType.Date || inputType == EInputType.DateTime)
            {
                additionalAttributes = @"class=""is_text""";
            }
            else if (inputType == EInputType.Image || inputType == EInputType.Video || inputType == EInputType.File)
            {
                additionalAttributes = @"class=""is_upload""";
            }
            else if (inputType == EInputType.TextArea)
            {
                additionalAttributes = @"class=""is_textarea""";
            }

            return additionalAttributes;
        }

        public string GetStyle(ETableStyle tableStyle)
        {
            var width = 260;
            if (tableStyle == ETableStyle.BackgroundContent)
            {
                width = 320;
            }

            return $@"
.is_text, .input_text {{ border:#cfd8e1 1px solid;background-color:#fff;width:{width}px; height:18px; line-height:18px; vertical-align:middle }}
.is_upload {{ border:#cfd8e1 1px solid; width:{(width + 68)}px; }}
.is_textarea {{ border:#cfd8e1 1px solid;background-color:#fff;width:320px; height:90px }}
.is_btn {{ line-height: 16px; padding: 5px 10px; }}
.is_success{{ margin: 0 auto; font: 14px Arial, Helvetica, sans-serif; color: #090 !important; padding: 10px 10px 10px 45px; width: 90%; text-align: left; line-height: 160%; font-weight: bold; }}
.is_failure{{ margin: 0 auto; font: 14px Arial, Helvetica, sans-serif; color:#CC0000 !important; padding: 10px 10px 10px 45px; width: 90%; text-align: left; line-height: 160%; font-weight: bold; }}
";
        }

        protected string GetAttributesHtml(NameValueCollection pageScripts, PublishmentSystemInfo publishmentSystemInfo, List<TableStyleInfo> styleInfoList)
        {
            if (styleInfoList == null) return string.Empty;

            var output = new StringBuilder();

            foreach (var styleInfo in styleInfoList)
            {
                if (styleInfo.IsVisible == false) continue;

                var helpHtml = styleInfo.DisplayName + ":";
                var formCollection = new NameValueCollection
                {
                    [styleInfo.AttributeName] = string.Empty
                };
                var inputHtml = InputTypeParser.Parse(publishmentSystemInfo, 0, styleInfo, ETableStyle.InputContent, styleInfo.AttributeName, formCollection, false, false, GetInnerAdditionalAttributes(styleInfo), pageScripts, true);
                output.Append($@"
<tr>
    <td style=""padding: 5px; width: 80px;"">{helpHtml}</td>
    <td style=""padding: 5px;"">{inputHtml}</td>
</tr>
");
            }

            return output.ToString();
        }
    }
}

