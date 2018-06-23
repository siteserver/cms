using System.Text;
using System.Web.UI.HtmlControls;
using SiteServer.Utils;
using SiteServer.CMS.StlParser.Model;
using SiteServer.CMS.StlParser.Utility;

namespace SiteServer.CMS.StlParser.StlElement
{
    [StlClass(Usage = "文字缩放", Description = "通过 stl:zoom 标签在模板中实现文字缩放功能")]
    public class StlZoom
	{
        private StlZoom() { }
        public const string ElementName = "stl:zoom";

        private static readonly Attr ZoomId = new Attr("zoomId", "页面HTML中缩放对象的ID属性");
        private static readonly Attr FontSize = new Attr("fontSize", "缩放字体大小");

        public static string Parse(PageInfo pageInfo, ContextInfo contextInfo)
		{
		    var zoomId = string.Empty;
            var fontSize = 16;
            var stlAnchor = new HtmlAnchor();

            foreach (var name in contextInfo.Attributes.AllKeys)
            {
                var value = contextInfo.Attributes[name];

                if (StringUtils.EqualsIgnoreCase(name, ZoomId.Name))
                {
                    zoomId = value;
                }
                else if (StringUtils.EqualsIgnoreCase(name, FontSize.Name))
                {
                    fontSize = TranslateUtils.ToInt(value, 16);
                }
                else
                {
                    ControlUtils.AddAttributeIfNotExists(stlAnchor, name, value);
                }
            }

            return ParseImpl(pageInfo, contextInfo, stlAnchor, zoomId, fontSize);
		}

        private static string ParseImpl(PageInfo pageInfo, ContextInfo contextInfo, HtmlAnchor stlAnchor, string zoomId, int fontSize)
        {
            if (string.IsNullOrEmpty(zoomId))
            {
                zoomId = "content";
            }

            if (!pageInfo.BodyCodes.ContainsKey(PageInfo.Const.JsAeStlZoom))
            {
                pageInfo.BodyCodes.Add(PageInfo.Const.JsAeStlZoom, @"
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
            }

            if (string.IsNullOrEmpty(contextInfo.InnerHtml))
            {
                stlAnchor.InnerHtml = "缩放";
            }
            else
            {
                var innerBuilder = new StringBuilder(contextInfo.InnerHtml);
                StlParserManager.ParseInnerContent(innerBuilder, pageInfo, contextInfo);
                stlAnchor.InnerHtml = innerBuilder.ToString();
            }
            stlAnchor.Attributes["href"] = $"javascript:stlDoZoom('{zoomId}', {fontSize});";

            return ControlUtils.GetControlRenderHtml(stlAnchor);
        }
	}
}
