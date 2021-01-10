using System.Collections.Specialized;
using System.Text;
using System.Threading.Tasks;
using SSCMS.Core.StlParser.Attributes;
using SSCMS.Parse;
using SSCMS.Services;
using SSCMS.Utils;

namespace SSCMS.Core.StlParser.StlElement
{
    [StlElement(Title = "文字缩放", Description = "通过 stl:zoom 标签在模板中实现文字缩放功能")]
    public static class StlZoom
	{
        public const string ElementName = "stl:zoom";

        [StlAttribute(Title = "页面Html 中缩放对象的 Id 属性")]
        private const string ZoomId = nameof(ZoomId);

        [StlAttribute(Title = "缩放字体大小")]
        private const string FontSize = nameof(FontSize);

        public static async Task<object> ParseAsync(IParseManager parseManager)
		{
		    var zoomId = string.Empty;
            var fontSize = 16;
            var attributes = new NameValueCollection();

            foreach (var name in parseManager.ContextInfo.Attributes.AllKeys)
            {
                var value = parseManager.ContextInfo.Attributes[name];

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

            return await ParseAsync(parseManager, attributes, zoomId, fontSize);
		}

        private static async Task<string> ParseAsync(IParseManager parseManager, NameValueCollection attributes, string zoomId, int fontSize)
        {
            var pageInfo = parseManager.PageInfo;
            var contextInfo = parseManager.ContextInfo;

            if (string.IsNullOrEmpty(zoomId))
            {
                zoomId = "content";
            }

            if (!pageInfo.BodyCodes.ContainsKey(ParsePage.Const.JsAeStlZoom))
            {
                pageInfo.BodyCodes.Add(ParsePage.Const.JsAeStlZoom, @"
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
                await parseManager.ParseInnerContentAsync(innerBuilder);
                innerHtml = innerBuilder.ToString();
            }

            attributes["href"] = $"javascript:stlDoZoom('{zoomId}', {fontSize});";

            return $@"<a {TranslateUtils.ToAttributesString(attributes)}>{innerHtml}</a>";
        }
	}
}
