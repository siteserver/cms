using System;
using System.Collections.Specialized;
using System.Text;
using System.Web.UI.HtmlControls;
using System.Xml;
using BaiRong.Core;
using BaiRong.Core.Model.Enumerations;
using SiteServer.CMS.StlParser.Model;
using SiteServer.CMS.StlParser.Utility;

namespace SiteServer.CMS.StlParser.StlElement
{
	public class StlPrinter
	{
        private StlPrinter() { }
        public const string ElementName = "stl:printer";    //打印

        public const string Attribute_TitleID = "titleid";		    //页面HTML中打印标题的ID属性
        public const string Attribute_BodyID = "bodyid";		    //页面HTML中打印正文的ID属性
        public const string Attribute_LogoID = "logoid";	        //页面LOGO的ID属性
        public const string Attribute_LocationID = "locationid";	//页面当前位置的ID属性
        public const string Attribute_IsDynamic = "isdynamic";              //是否动态显示

		public static ListDictionary AttributeList
		{
			get
			{
				var attributes = new ListDictionary();
                attributes.Add(Attribute_TitleID, "页面HTML中打印标题的ID属性");
                attributes.Add(Attribute_BodyID, "页面HTML中打印正文的ID属性");
                attributes.Add(Attribute_LogoID, "页面LOGO的ID属性");
                attributes.Add(Attribute_LocationID, "页面当前位置的ID属性");
                attributes.Add(Attribute_IsDynamic, "是否动态显示");
				return attributes;
			}
		}


        //对“打印”（stl:printer）元素进行解析
        public static string Parse(string stlElement, XmlNode node, PageInfo pageInfo, ContextInfo contextInfo)
		{
			var parsedContent = string.Empty;
			try
			{
                var stlAnchor = new HtmlAnchor();
				var ie = node.Attributes.GetEnumerator();

                var titleID = string.Empty;
                var bodyID = string.Empty;
                var logoID = string.Empty;
                var locationID = string.Empty;
                var isDynamic = false;

				while (ie.MoveNext())
				{
					var attr = (XmlAttribute)ie.Current;
					var attributeName = attr.Name.ToLower();
					if (attributeName.Equals(Attribute_TitleID))
					{
                        titleID = attr.Value;
                    }
                    else if (attributeName.Equals(Attribute_BodyID))
                    {
                        bodyID = attr.Value;
                    }
                    else if (attributeName.Equals(Attribute_LogoID))
                    {
                        logoID = attr.Value;
                    }
                    else if (attributeName.Equals(Attribute_LocationID))
                    {
                        locationID = attr.Value;
                    }
                    else if (attributeName.Equals(Attribute_IsDynamic))
                    {
                        isDynamic = TranslateUtils.ToBool(attr.Value);
                    }
                    else
                    {
                        ControlUtils.AddAttributeIfNotExists(stlAnchor, attributeName, attr.Value);
                    }
				}

                if (isDynamic)
                {
                    parsedContent = StlDynamic.ParseDynamicElement(stlElement, pageInfo, contextInfo);
                }
                else
                {
                    parsedContent = ParseImpl(node, pageInfo, contextInfo, stlAnchor, titleID, bodyID, logoID, locationID);
                }
			}
            catch (Exception ex)
            {
                parsedContent = StlParserUtility.GetStlErrorMessage(ElementName, ex);
            }

			return parsedContent;
		}

        private static string ParseImpl(XmlNode node, PageInfo pageInfo, ContextInfo contextInfo, HtmlAnchor stlAnchor, string titleID, string bodyID, string logoID, string locationID)
        {
            var parsedContent = string.Empty;

            var jsUrl = string.Empty;
            if (pageInfo.TemplateInfo.Charset == ECharset.gb2312)
            {
                jsUrl = SiteFilesAssets.GetUrl(pageInfo.ApiUrl, SiteFilesAssets.Print.JsGb2312);
            }
            else
            {
                jsUrl = SiteFilesAssets.GetUrl(pageInfo.ApiUrl, SiteFilesAssets.Print.JsUtf8);
            }

            var iconUrl = SiteFilesAssets.GetUrl(pageInfo.ApiUrl, SiteFilesAssets.Print.IconUrl);
            pageInfo.AddPageScriptsIfNotExists(PageInfo.JsAfStlPrinter, $@"
<script language=""JavaScript"" type=""text/javascript"">
function stlLoadPrintJsCallBack()
{{
    if(typeof forSPrint == ""object"" && forSPrint.Print)
    {{
        forSPrint.data.titleId = ""{titleID}"";
        forSPrint.data.artiBodyId = ""{bodyID}"";
        forSPrint.data.pageLogoId = ""{logoID}"";
        forSPrint.data.pageWayId = ""{locationID}"";
        forSPrint.data.iconUrl = ""{iconUrl}"";
        forSPrint.Print();
    }}
}}

function stlPrintGetBrowser()
{{
    if (navigator.userAgent.indexOf(""MSIE"") != -1)
    {{
        return 1; 
    }}
    else if (navigator.userAgent.indexOf(""Firefox"") != -1)
    {{
        return 2; 
    }}
    else if (navigator.userAgent.indexOf(""Navigator"") != -1)
    {{
        return 3;
    }}
    else if (navigator.userAgent.indexOf(""Opera"") != -1 )
    {{
        return 4;
    }}
    else
    {{
        return 5;
    }}
}}

function stlLoadPrintJs()
{{
    var myBrowser = stlPrintGetBrowser();
    if(myBrowser == 1)
    {{
        var js_url = ""{jsUrl}"";
        var js = document.createElement( ""script"" ); 
        js.setAttribute( ""type"", ""text/javascript"" );
        js.setAttribute( ""src"", js_url);
        js.setAttribute( ""id"", ""printJsUrl"");
        document.body.insertBefore( js, null);
        document.getElementById(""printJsUrl"").onreadystatechange = stlLoadPrintJsCallBack;
    }}
    else
    {{
        var js_url = ""{jsUrl}"";
        var js = document.createElement( ""script"" ); 
        js.setAttribute( ""type"", ""text/javascript"" );
        js.setAttribute( ""src"", js_url);
        js.setAttribute( ""id"", ""printJsUrl"");
        js.setAttribute( ""onload"", ""stlLoadPrintJsCallBack()"");
        document.body.insertBefore( js, null);					
    }}
}}	
</script>
");

            if (node.InnerXml.Trim().Length == 0)
            {
                stlAnchor.InnerHtml = "打印";
            }
            else
            {
                var innerBuilder = new StringBuilder(node.InnerXml);
                StlParserManager.ParseInnerContent(innerBuilder, pageInfo, contextInfo);
                stlAnchor.InnerHtml = innerBuilder.ToString();
            }
            stlAnchor.Attributes["href"] = "javascript:stlLoadPrintJs();";

            parsedContent = ControlUtils.GetControlRenderHtml(stlAnchor);

            return parsedContent;
        }
	}
}
