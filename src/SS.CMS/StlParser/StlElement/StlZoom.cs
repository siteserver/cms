using System.Collections.Specialized;
using System.Text;
using System.Threading.Tasks;
using SS.CMS.Abstractions;
using SS.CMS.StlParser.Model;
using SS.CMS.StlParser.Utility;

namespace SS.CMS.StlParser.StlElement
{
    [StlElement(Title = "文字缩放", Description = "通过 stl:zoom 标签在模板中实现文字缩放功能")]
    public class StlZoom
	{
        private StlZoom() { }
        public const string ElementName = "stl:zoom";

        [StlAttribute(Title = "页面Html 中缩放对象的 Id 属性")]
        private const string ZoomId = nameof(ZoomId);

        [StlAttribute(Title = "缩放字体大小")]
        private const string FontSize = nameof(FontSize);

        public static async Task<object> ParseAsync(PageInfo pageInfo, ContextInfo contextInfo)
		{
		    var zoomId = string.Empty;
            var fontSize = 16;
            var attributes = new NameValueCollection();

            foreach (var name in contextInfo.Attributes.AllKeys)
            {
                var value = contextInfo.Attributes[name];

                if (StringUtils.EqualsIgnoreCase(name, ZoomId))
                {
                    zoomId = value;
                }
                else if (StringUtils.EqualsIgnoreCase(name, FontSize))
                {
                    fontSize = TranslateUtils.ToInt(value, 16);
                }
                else
                {
                    TranslateUtils.AddAttributeIfNotExists(attributes, name, value);
                }
            }

            return await ParseImplAsync(pageInfo, contextInfo, attributes, zoomId, fontSize);
		}

        private static async Task<string> ParseImplAsync(PageInfo pageInfo, ContextInfo contextInfo, NameValueCollection attributes, string zoomId, int fontSize)
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

            string innerHtml;
            if (string.IsNullOrEmpty(contextInfo.InnerHtml))
            {
                innerHtml = "缩放";
            }
            else
            {
                var innerBuilder = new StringBuilder(contextInfo.InnerHtml);
                await StlParserManager.ParseInnerContentAsync(innerBuilder, pageInfo, contextInfo);
                innerHtml = innerBuilder.ToString();
            }

            attributes["href"] = $"javascript:stlDoZoom('{zoomId}', {fontSize});";

            return $@"<a {TranslateUtils.ToAttributesString(attributes)}>{innerHtml}</a>";
        }
	}
}
