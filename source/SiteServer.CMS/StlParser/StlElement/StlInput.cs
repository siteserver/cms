using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using BaiRong.Core;
using SiteServer.CMS.Core;
using SiteServer.CMS.StlParser.Model;
using SiteServer.CMS.StlParser.Parser;
using SiteServer.CMS.StlParser.Utility;
using SiteServer.CMS.StlTemplates;

namespace SiteServer.CMS.StlParser.StlElement
{
    [Stl(Usage = "提交表单", Description = "通过 stl:input 标签在模板中实现提交表单功能")]
    public class StlInput
    {
        private StlInput() { }
        public const string ElementName = "stl:input";

        public const string AttributeInputName = "inputName";
        public const string AttributeIsLoadValues = "isLoadValues";

        public static SortedList<string, string> AttributeList => new SortedList<string, string>
        {
            {AttributeInputName, "提交表单名称"},
            {AttributeIsLoadValues, "是否载入URL参数"}
        };

        public static string Parse(string stlElement, XmlNode node, PageInfo pageInfo, ContextInfo contextInfo)
        {
            string parsedContent;
            try
            {
                var inputName = string.Empty;
                var isLoadValues = false;

                var ie = node.Attributes?.GetEnumerator();
                if (ie != null)
                {
                    while (ie.MoveNext())
                    {
                        var attr = (XmlAttribute)ie.Current;

                        if (StringUtils.EqualsIgnoreCase(attr.Name, AttributeInputName))
                        {
                            inputName = StlEntityParser.ReplaceStlEntitiesForAttributeValue(attr.Value, pageInfo, contextInfo);
                        }
                        else if (StringUtils.EqualsIgnoreCase(attr.Name, AttributeIsLoadValues))
                        {
                            isLoadValues = TranslateUtils.ToBool(attr.Value, false);
                        }
                    }
                }

                string inputTemplateString;
                string successTemplateString;
                string failureTemplateString;
                StlParserUtility.GetInnerTemplateStringOfInput(node, out inputTemplateString, out successTemplateString, out failureTemplateString, pageInfo, contextInfo);

                parsedContent = ParseImpl(pageInfo, contextInfo, inputName, isLoadValues, inputTemplateString, successTemplateString, failureTemplateString);
            }
            catch (Exception ex)
            {
                parsedContent = StlParserUtility.GetStlErrorMessage(ElementName, ex);
            }

            return parsedContent;
        }

        private static string ParseImpl(PageInfo pageInfo, ContextInfo contextInfo, string inputName, bool isLoadValues, string inputTemplateString, string successTemplateString, string failureTemplateString)
        {
            var publishmentSystemId = pageInfo.PublishmentSystemId;
            var inputId = DataProvider.InputDao.GetInputIdAsPossible(inputName, publishmentSystemId);

            if (inputId <= 0) return string.Empty;

            var inputInfo = DataProvider.InputDao.GetInputInfo(inputId);
            var inputTemplate = new InputTemplate(pageInfo.PublishmentSystemInfo, inputInfo);
            var parsedContent = inputTemplate.GetTemplate(isLoadValues, inputTemplateString, successTemplateString, failureTemplateString);

            var innerBuilder = new StringBuilder(parsedContent);
            StlParserManager.ParseInnerContent(innerBuilder, pageInfo, contextInfo);
            parsedContent = innerBuilder.ToString();

            if (isLoadValues)
            {
                pageInfo.AddPageScriptsIfNotExists(ElementName, $@"
<script language=""vbscript""> 
Function str2asc(strstr) 
 str2asc = hex(asc(strstr)) 
End Function 
Function asc2str(ascasc) 
 asc2str = chr(ascasc) 
End Function 
</script>
<script type=""text/javascript"" src=""{SiteFilesAssets.Input.GetScriptUrl(pageInfo.ApiUrl)}""></script>");
            }

            return parsedContent;
        }
    }
}
