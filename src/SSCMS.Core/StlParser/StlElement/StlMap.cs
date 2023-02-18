using System.Threading.Tasks;
using SSCMS.Core.StlParser.Attributes;
using SSCMS.Services;
using SSCMS.Utils;
using System.Collections.Specialized;
using System.Text;

namespace SSCMS.Core.StlParser.StlElement
{
    [StlElement(Title = "显示地图", Description = "通过 stl:map 标签在模板中显示地图")]
    public static class StlMap
    {
        public const string ElementName = "stl:map";

        [StlAttribute(Title = "Id")]
        private const string Id = nameof(Id);

        [StlAttribute(Title = "百度地图密钥")]
        private const string Ak = nameof(Ak);

        [StlAttribute(Title = "经纬度")]
        private const string Point = nameof(Point);

        [StlAttribute(Title = "宽度")]
        private const string Width = nameof(Width);

        [StlAttribute(Title = "高度")]
        private const string Height = nameof(Height);

        [StlAttribute(Title = "样式")]
        private const string Theme = nameof(Theme);

        [StlAttribute(Title = "显示卫星影像")]
        private const string IsHybrid = nameof(IsHybrid);

        [StlAttribute(Title = "缩放")]
        private const string Zoom = nameof(Zoom);

        public static async Task<object> ParseAsync(IParseManager parseManager)
        {
            var contextInfo = parseManager.ContextInfo;
            var id = StringUtils.GetElementId();
            var ak = "3rKHdDkGzculXfZ8iPzr0h6xSxHowlct";
            var point = string.Empty;
            var width = string.Empty;
            var height = string.Empty;
            var theme = "normal";
            var isHybrid = true;
            var zoom = 16;
            var attributes = new NameValueCollection();

            foreach (var name in contextInfo.Attributes.AllKeys)
            {
                var value = contextInfo.Attributes[name];

                if (StringUtils.EqualsIgnoreCase(name, Id))
                {
                    id = value;
                }
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
                else if (StringUtils.EqualsIgnoreCase(name, Theme))
                {
                    theme = value;
                }
                else if (StringUtils.EqualsIgnoreCase(name, IsHybrid))
                {
                    isHybrid = TranslateUtils.ToBool(value, isHybrid);
                }
                else if (StringUtils.EqualsIgnoreCase(name, Zoom))
                {
                    zoom = TranslateUtils.ToInt(value, zoom);
                }
                else
                {
                    attributes[name] = value;
                }
            }

            return await ParseAsync(parseManager, id, ak, point, width, height, theme, isHybrid, zoom, attributes);
        }

        private static async Task<string> ParseAsync(IParseManager parseManager, string id, string ak, string point, string width, string height, string theme, bool isHybrid, int zoom, NameValueCollection attributes)
        {
            var pageInfo = parseManager.PageInfo;
            var contextInfo = parseManager.ContextInfo;

            pageInfo.HeadCodes[$"{nameof(StlMap)}_{ak}"] = $@"
            <script type=""text/javascript"" src =""https://api.map.baidu.com/api?v=2.0&ak={ak}""></script>
            <script type=""text/javascript"" src=""http://api.map.baidu.com/library/SearchInfoWindow/1.5/src/SearchInfoWindow_min.js""></script>
            <link rel=""stylesheet"" href=""http://api.map.baidu.com/library/SearchInfoWindow/1.5/src/SearchInfoWindow_min.css"" />
            ";

            attributes["id"] = id;
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

            var mapTypes = string.Empty;
            if (isHybrid)
            {
              mapTypes = @$"
  map{id}.addControl(new BMap.MapTypeControl({{
    mapTypes: [
      BMAP_NORMAL_MAP,
      BMAP_HYBRID_MAP
    ]
  }}));";
            }

            var script = @$"
<script type=""text/javascript"">
  var html{id} = document.getElementById(""{id}"").innerHTML;
  var map{id} = new BMap.Map(""{id}"");
  var point{id} = new BMap.Point({point});
  map{id}.setMapStyle({{ style: '{theme}' }});
  {mapTypes}
  map{id}.setCenter(point{id});
  map{id}.centerAndZoom(point{id}, {zoom});
  map{id}.addControl(new BMap.ScaleControl());
  map{id}.addControl(new BMap.NavigationControl());
  map{id}.enableScrollWheelZoom(); //启用滚轮放大缩小，默认禁用
  map{id}.enableContinuousZoom(); //启用地图惯性拖拽，默认禁用
  var marker{id} = new BMap.Marker(point{id});
  map{id}.addOverlay(marker{id});
  if (html{id}) {{
    map{id}.openInfoWindow(new BMap.InfoWindow(html{id}), point{id});
    marker{id}.addEventListener(""click"", function () {{
      map{id}.openInfoWindow(new BMap.InfoWindow(html{id}), point{id});
    }});
  }}
</script>
";

            var innerHtml = StringUtils.Trim(contextInfo.InnerHtml);
            if (!string.IsNullOrEmpty(innerHtml))
            {
                var innerBuilder = new StringBuilder(innerHtml);
                await parseManager.ParseInnerContentAsync(innerBuilder);
                innerHtml = innerBuilder.ToString();
            }

            return $@"<div {TranslateUtils.ToAttributesString(attributes)}>{innerHtml}</div>{script}";
        }
    }
}
