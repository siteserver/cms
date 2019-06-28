using System.Collections.Specialized;
using System.Text;
using System.Threading.Tasks;
using SS.CMS.Core.StlParser.Models;
using SS.CMS.Core.StlParser.Utility;
using SS.CMS.Utils;

namespace SS.CMS.Core.StlParser.StlElement
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

        public static async Task<object> ParseAsync(ParseContext parseContext)
        {
            var zoomId = string.Empty;
            var fontSize = 16;
            var attributes = new NameValueCollection();

            foreach (var name in parseContext.Attributes.AllKeys)
            {
                var value = parseContext.Attributes[name];

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

            return await ParseImplAsync(parseContext, attributes, zoomId, fontSize);
        }

        private static async Task<string> ParseImplAsync(ParseContext parseContext, NameValueCollection attributes, string zoomId, int fontSize)
        {
            if (string.IsNullOrEmpty(zoomId))
            {
                zoomId = "content";
            }

            if (!parseContext.BodyCodes.ContainsKey(PageInfo.Const.JsAeStlZoom))
            {
                parseContext.BodyCodes.Add(PageInfo.Const.JsAeStlZoom, @"
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

            var innerHtml = "缩放";
            if (!string.IsNullOrEmpty(parseContext.InnerHtml))
            {
                var innerBuilder = new StringBuilder(parseContext.InnerHtml);
                await parseContext.ParseInnerContentAsync(innerBuilder);
                innerHtml = innerBuilder.ToString();
            }
            attributes["href"] = $"javascript:stlDoZoom('{zoomId}', {fontSize});";

            return $@"<a {TranslateUtils.ToAttributesString(attributes)}>{innerHtml}</a>";
        }
    }
}
