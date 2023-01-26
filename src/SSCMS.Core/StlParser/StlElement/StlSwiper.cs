using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;
using SSCMS.Core.StlParser.Attributes;
using SSCMS.Parse;
using SSCMS.Services;
using SSCMS.Utils;

namespace SSCMS.Core.StlParser.StlElement
{
    [StlElement(Title = "轮播滑块", Description = "通过 stl:swiper 标签在模板中实现触摸轮播滑块效果")]
    public static class StlSwiper
    {
        public const string ElementName = "stl:swiper";

        [StlAttribute(Title = "方向")]
        private const string Direction = nameof(Direction);

        [StlAttribute(Title = "宽度")]
        private const string Width = nameof(Width);

        [StlAttribute(Title = "高度")]
        private const string Height = nameof(Height);

        [StlAttribute(Title = "是否显示翻页控件")]
        private const string IsPagination = nameof(IsPagination);

        [StlAttribute(Title = "是否显示左右切换控件")]
        private const string IsNavigation = nameof(IsNavigation);

        [StlAttribute(Title = "是否显示拖动控件")]
        private const string IsScrollbar = nameof(IsScrollbar);

        [StlAttribute(Title = "控件颜色")]
        private const string Color = nameof(Color);

        [StlAttribute(Title = "控件参数")]
        private const string Parameters = nameof(Parameters);

        public const string DirectionVertical = "vertical";         //垂直
        public const string DirectionHorizontal = "horizontal";		//水平

        public static SortedList<string, string> DirectionList => new SortedList<string, string>
        {
            {DirectionVertical, "垂直"},
            {DirectionHorizontal, "水平"}
        };

        public static async Task<object> ParseAsync(IParseManager parseManager)
        {
            var direction = DirectionHorizontal;
            var width = "500";
            var height = "500";
            var isPagination = true;
            var isNavigation = true;
            var isScrollbar = false;
            var color = "#fff";
            var parameters = string.Empty;

            foreach (var name in parseManager.ContextInfo.Attributes.AllKeys)
            {
                var value = parseManager.ContextInfo.Attributes[name];

                if (StringUtils.EqualsIgnoreCase(name, Direction))
                {
                    if (StringUtils.EqualsIgnoreCase(value, DirectionVertical))
                    {
                        direction = DirectionVertical;
                    }
                }
                else if (StringUtils.EqualsIgnoreCase(name, Width))
                {
                    width = value;
                }
                else if (StringUtils.EqualsIgnoreCase(name, Height))
                {
                    height = value;
                }
                else if (StringUtils.EqualsIgnoreCase(name, IsPagination))
                {
                    isPagination = TranslateUtils.ToBool(value, true);
                }
                else if (StringUtils.EqualsIgnoreCase(name, IsNavigation))
                {
                    isNavigation = TranslateUtils.ToBool(value, true);
                }
                else if (StringUtils.EqualsIgnoreCase(name, IsScrollbar))
                {
                    isScrollbar = TranslateUtils.ToBool(value, false);
                }
                else if (StringUtils.EqualsIgnoreCase(name, Color))
                {
                    color = value;
                }
                else if (StringUtils.EqualsIgnoreCase(name, Parameters))
                {
                    parameters = value;
                }
            }

            return await ParseAsync(parseManager, direction, width, height, isPagination, isNavigation, isScrollbar, color, parameters);
        }

        private static async Task<string> ParseAsync(IParseManager parseManager, string direction, string width, string height, bool isPagination, bool isNavigation, bool isScrollbar, string color, string parameters)
        {
            var pageInfo = parseManager.PageInfo;
            var contextInfo = parseManager.ContextInfo;

            await pageInfo.AddPageBodyCodeIfNotExistsAsync(ParsePage.Const.JsAcSwiperJs);

            if (string.IsNullOrEmpty(contextInfo.InnerHtml)) return string.Empty;
            var innerHtml = string.Empty;
            var innerBuilder = new StringBuilder(contextInfo.InnerHtml);
            await parseManager.ParseInnerContentAsync(innerBuilder);
            innerHtml = innerBuilder.ToString();

            var htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(innerHtml);
            var htmlNodes = htmlDoc.DocumentNode.ChildNodes;
            if (htmlNodes == null || htmlNodes.Count == 0) return string.Empty;

            var slides = new StringBuilder();
            foreach (var htmlNode in htmlNodes)
            {
                if (htmlNode.NodeType != HtmlNodeType.Element) continue;
                slides.Append($@"<div class=""swiper-slide"">{htmlNode.OuterHtml}</div>");
            }

            var colorCss = !string.IsNullOrEmpty(color) ? $":root {{--swiper-theme-color: {color};}} .swiper-button-prev, .swiper-button-next{{background: none;}}" : string.Empty;
            var widthCss = !string.IsNullOrEmpty(width) ? $"width: {StringUtils.AddUnitIfNotExists(width)};" : string.Empty;
            var heightCss = !string.IsNullOrEmpty(height) ? $"height: {StringUtils.AddUnitIfNotExists(height)};" : string.Empty;
            var styles = @$"<style>{colorCss} .swiper {{{widthCss}{heightCss}}}</style>";

            var paginationJs = isPagination ? @"
  pagination: {
    el: '.swiper-pagination',
  },
            " : string.Empty;
            var navigationJs = isNavigation ? @"
  navigation: {
    nextEl: '.swiper-button-next',
    prevEl: '.swiper-button-prev',
  },
            " : string.Empty;
            var scrollbarJs = isScrollbar ? @"
  scrollbar: {
    el: '.swiper-scrollbar',
  },
            " : string.Empty;

            if (!string.IsNullOrEmpty(parameters))
            {
                parameters = StringUtils.Trim(parameters);
                parameters = StringUtils.TrimEnd(parameters, ",");
                parameters += ",";
            }

            var scripts = @$"
<script>
var swiper = new Swiper('.swiper', {{
  {parameters}
  direction: '{direction}',
  loop: true,
  spaceBetween: 100,
  {paginationJs}
  {navigationJs}
  {scrollbarJs}
}});
</script>";

            var paginationHtml = isPagination ? @"<div class=""swiper-pagination""></div>" : string.Empty;
            var navigationHtml = isNavigation ? @"
  <div class=""swiper-button-prev""></div>
  <div class=""swiper-button-next""></div>
            " : string.Empty;
            var scrollbarHtml = isScrollbar ? @"<div class=""swiper-scrollbar""></div>" : string.Empty;

            return $@"
{styles}
<div class=""swiper"">
  <div class=""swiper-wrapper"">
    {slides}
  </div>
  {paginationHtml}
  {navigationHtml}
  {scrollbarHtml}
</div>
{scripts}
";
        }
    }
}
