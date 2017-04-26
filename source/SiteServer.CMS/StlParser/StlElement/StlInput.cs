using System;
using System.Collections.Specialized;
using System.Text;
using System.Xml;
using BaiRong.Core;
using SiteServer.CMS.Core;
using SiteServer.CMS.Model;
using SiteServer.CMS.StlParser.Model;
using SiteServer.CMS.StlParser.Parser;
using SiteServer.CMS.StlParser.Utility;
using SiteServer.CMS.StlTemplates;

namespace SiteServer.CMS.StlParser.StlElement
{
    public class StlInput
    {
        private StlInput() { }
        public const string ElementName = "stl:input";

        public const string AttributeInputName = "inputname";		        //提交表单名称
        public const string AttributeIsLoadValues = "isloadvalues";		//是否载入URL参数
        public const string AttributeSiteName = "sitename"; //站点名称

        public static ListDictionary AttributeList
        {
            get
            {
                var attributes = new ListDictionary
                {
                    {AttributeInputName, "提交表单名称"},
                    {AttributeIsLoadValues, "是否载入URL参数"},
                    {AttributeSiteName, "站点名称"}
                };
                return attributes;
            }
        }

        public static string Parse(string stlElement, XmlNode node, PageInfo pageInfo, ContextInfo contextInfo)
        {
            string parsedContent;
            try
            {
                var inputName = string.Empty;
                var isLoadValues = false;
                var siteName = string.Empty;

                var ie = node.Attributes?.GetEnumerator();
                if (ie != null)
                {
                    while (ie.MoveNext())
                    {
                        var attr = (XmlAttribute)ie.Current;
                        var attributeName = attr.Name.ToLower();
                        if (attributeName.Equals(AttributeInputName))
                        {
                            inputName = StlEntityParser.ReplaceStlEntitiesForAttributeValue(attr.Value, pageInfo, contextInfo);
                        }
                        else if (attributeName.Equals(AttributeIsLoadValues))
                        {
                            isLoadValues = TranslateUtils.ToBool(attr.Value, false);
                        }
                        else if (attributeName.Equals(AttributeSiteName))
                        {
                            siteName = attr.Value;
                        }
                    }
                }

                string inputTemplateString;
                string successTemplateString;
                string failureTemplateString;
                StlParserUtility.GetInnerTemplateStringOfInput(node, out inputTemplateString, out successTemplateString, out failureTemplateString, pageInfo, contextInfo);

                parsedContent = ParseImpl(pageInfo, contextInfo, inputName, isLoadValues, inputTemplateString, successTemplateString, failureTemplateString, siteName);
            }
            catch (Exception ex)
            {
                parsedContent = StlParserUtility.GetStlErrorMessage(ElementName, ex);
            }

            return parsedContent;
        }

        private static string ParseImpl(PageInfo pageInfo, ContextInfo contextInfo, string inputName, bool isLoadValues, string inputTemplateString, string successTemplateString, string failureTemplateString, string siteName)
        {
            var parsedContent = string.Empty;
            var prePublishmentSystemInfo = pageInfo.PublishmentSystemInfo;
            var prePageNodeId = pageInfo.PageNodeId;
            var prePageContentId = pageInfo.PageContentId;

            var publishmentSystemId = pageInfo.PublishmentSystemId;
            PublishmentSystemInfo publishmentSystemInfo = null;
            if (!string.IsNullOrEmpty(siteName))
            {
                publishmentSystemInfo = PublishmentSystemManager.GetPublishmentSystemInfoBySiteName(siteName);
            }
            if (publishmentSystemInfo != null)
            {
                pageInfo.ChangeSite(publishmentSystemInfo, publishmentSystemInfo.PublishmentSystemId, 0, contextInfo);
                publishmentSystemId = publishmentSystemInfo.PublishmentSystemId;
            }

            var inputId = DataProvider.InputDao.GetInputIdAsPossible(inputName, publishmentSystemId);

            if (inputId > 0)
            {
                var inputInfo = DataProvider.InputDao.GetInputInfo(inputId);
                var inputTemplate = new InputTemplate(pageInfo.PublishmentSystemInfo, inputInfo);
                parsedContent = inputTemplate.GetTemplate(isLoadValues, inputTemplateString, successTemplateString, failureTemplateString);

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
            }

            pageInfo.ChangeSite(prePublishmentSystemInfo, prePageNodeId, prePageContentId, contextInfo);
            return parsedContent;
        }
    }
}
