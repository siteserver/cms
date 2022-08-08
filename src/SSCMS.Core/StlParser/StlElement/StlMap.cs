using System.Threading.Tasks;
using SSCMS.Core.StlParser.Attributes;
using SSCMS.Services;
using SSCMS.Utils;
using System.Collections.Specialized;

namespace SSCMS.Core.StlParser.StlElement
{
    [StlElement(Title = "显示地图", Description = "通过 stl:map 标签在模板中显示地图")]
    public static class StlMap
    {
        public const string ElementName = "stl:map";

        [StlAttribute(Title = "百度地图密钥")]
        private const string Ak = nameof(Ak);

        [StlAttribute(Title = "经纬度")]
        private const string Point = nameof(Point);

        [StlAttribute(Title = "宽度")]
        private const string Width = nameof(Width);

        [StlAttribute(Title = "高度")]
        private const string Height = nameof(Height);

        public static async Task<object> ParseAsync(IParseManager parseManager)
        {
            var ak = "3rKHdDkGzculXfZ8iPzr0h6xSxHowlct";
            var point = string.Empty;
            var width = string.Empty;
            var height = string.Empty;
            var attributes = new NameValueCollection();

            foreach (var name in parseManager.ContextInfo.Attributes.AllKeys)
            {
                var value = parseManager.ContextInfo.Attributes[name];

                if (StringUtils.EqualsIgnoreCase(name, Ak))
                {
                    ak = await parseManager.ReplaceStlEntitiesForAttributeValueAsync(value);
                }
                else if (StringUtils.EqualsIgnoreCase(name, Point))
                {
                    point = await parseManager.ReplaceStlEntitiesForAttributeValueAsync(value);
                }
                else if (StringUtils.EqualsIgnoreCase(name, Width))
                {
                    width = value;
                }
                else if (StringUtils.EqualsIgnoreCase(name, Height))
                {
                    height = value;
                }
                else
                {
                    attributes[name] = value;
                }
            }

            return Parse(parseManager, ak, point, width, height, attributes);
        }

        private static string Parse(IParseManager parseManager, string ak, string point, string width, string height, NameValueCollection attributes)
        {
            var pageInfo = parseManager.PageInfo;
            var contextInfo = parseManager.ContextInfo;

            pageInfo.HeadCodes[$"{nameof(StlMap)}_{ak}"] = $@"<script type=""text/javascript"" src =""https://api.map.baidu.com/api?v=2.0&s=1&ak={ak}""></script>";

            var elementId = StringUtils.GetElementId();
            attributes["id"] = elementId;
            var style = attributes["style"] ?? string.Empty;
            if (!string.IsNullOrEmpty(width))
            {
                style += $";width:{StringUtils.AddUnitIfNotExists(width)};";
            }
            if (!string.IsNullOrEmpty(height))
            {
                style += $";height:{StringUtils.AddUnitIfNotExists(height)};";
            }
            style = StringUtils.Replace(style, ";;", ";");
            style = StringUtils.Trim(style, ';');

            if (!string.IsNullOrEmpty(style))
            {
                attributes["style"] = style;
            }

            var script = @$"
<script type=""text/javascript"">
  var map{elementId} = new BMap.Map(""{elementId}"");
  var point{elementId} = new BMap.Point({point});
  var marker{elementId} = new BMap.Marker(point{elementId});
  map{elementId}.setCenter(point{elementId});
  map{elementId}.centerAndZoom(point{elementId}, 18);
  map{elementId}.addOverlay(marker{elementId});
  map{elementId}.addControl(new BMap.NavigationControl());
  map{elementId}.enableScrollWheelZoom(); //启用滚轮放大缩小，默认禁用
  map{elementId}.enableContinuousZoom(); //启用地图惯性拖拽，默认禁用
  marker{elementId}.addEventListener(""click"", function () {{
    this.openInfoWindow(infoWindow{elementId});
  }});
</script>
";

            return $@"<div {TranslateUtils.ToAttributesString(attributes)}></div>{script}";
        }
    }
}
