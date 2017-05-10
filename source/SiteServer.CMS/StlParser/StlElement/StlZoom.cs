using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI.HtmlControls;
using System.Xml;
using BaiRong.Core;
using SiteServer.CMS.StlParser.Model;
using SiteServer.CMS.StlParser.Utility;

namespace SiteServer.CMS.StlParser.StlElement
{
    [Stl(Usage = "文字缩放", Description = "通过 stl:zoom 标签在模板中实现文字缩放功能")]
    public class StlZoom
	{
        private StlZoom() { }
        public const string ElementName = "stl:zoom";

        public const string AttributeZoomId = "zoomId";
        public const string AttributeFontSize = "fontSize";
        public const string AttributeIsDynamic = "isDynamic";

	    public static SortedList<string, string> AttributeList => new SortedList<string, string>
	    {
	        {AttributeZoomId, "页面HTML中缩放对象的ID属性"},
	        {AttributeFontSize, "缩放字体大小"},
	        {AttributeIsDynamic, "是否动态显示"}
	    };

        //对“文字缩放”（stl:zoom）元素进行解析
        public static string Parse(string stlElement, XmlNode node, PageInfo pageInfo, ContextInfo contextInfo)
		{
			string parsedContent;
			try
			{
                var zoomId = string.Empty;
                var fontSize = 16;
                var isDynamic = false;
                var stlAnchor = new HtmlAnchor();

                var ie = node.Attributes?.GetEnumerator();
			    if (ie != null)
			    {
                    while (ie.MoveNext())
                    {
                        var attr = (XmlAttribute)ie.Current;

                        if (StringUtils.EqualsIgnoreCase(attr.Name, AttributeZoomId))
                        {
                            zoomId = attr.Value;
                        }
                        else if (StringUtils.EqualsIgnoreCase(attr.Name, AttributeFontSize))
                        {
                            fontSize = TranslateUtils.ToInt(attr.Value, 16);
                        }
                        else if (StringUtils.EqualsIgnoreCase(attr.Name, AttributeIsDynamic))
                        {
                            isDynamic = TranslateUtils.ToBool(attr.Value);
                        }
                        else
                        {
                            ControlUtils.AddAttributeIfNotExists(stlAnchor, attr.Name, attr.Value);
                        }
                    }
                }		    

                parsedContent = isDynamic ? StlDynamic.ParseDynamicElement(stlElement, pageInfo, contextInfo) : ParseImpl(node, pageInfo, contextInfo, stlAnchor, zoomId, fontSize);
			}
            catch (Exception ex)
            {
                parsedContent = StlParserUtility.GetStlErrorMessage(ElementName, ex);
            }

			return parsedContent;
		}

        private static string ParseImpl(XmlNode node, PageInfo pageInfo, ContextInfo contextInfo, HtmlAnchor stlAnchor, string zoomId, int fontSize)
        {
            if (string.IsNullOrEmpty(zoomId))
            {
                zoomId = "content";
            }

            pageInfo.AddPageScriptsIfNotExists(PageInfo.JsAeStlZoom, @"
<script language=""JavaScript"" type=""text/javascript"">
function stlDoZoom(zoomId, size){
    var artibody = document.getElementById(zoomId);
    if(!artibody){
        return;
    }
    var artibodyChild = artibody.childNodes;
    artibody.style.fontSize = size + 'px';
    for(var i = 0; i < artibodyChild.length; i++){
        if(artibodyChild[i].nodeType == 1){
            artibodyChild[i].style.fontSize = size + 'px';
        }
    }
}
</script>
");

            if (node.InnerXml.Trim().Length == 0)
            {
                stlAnchor.InnerHtml = "缩放";
            }
            else
            {
                var innerBuilder = new StringBuilder(node.InnerXml);
                StlParserManager.ParseInnerContent(innerBuilder, pageInfo, contextInfo);
                stlAnchor.InnerHtml = innerBuilder.ToString();
            }
            stlAnchor.Attributes["href"] = $"javascript:stlDoZoom('{zoomId}', {fontSize});";

            return ControlUtils.GetControlRenderHtml(stlAnchor);
        }
	}
}
