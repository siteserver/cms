using System.Collections.Generic;
using System.Text;
using SiteServer.Utils;
using SiteServer.CMS.StlParser.Model;
using SiteServer.CMS.StlParser.Utility;

namespace SiteServer.CMS.StlParser.StlElement
{
    [StlElement(Title = "无间隔滚动", Description = "通过 stl:marquee 标签在模板中创建一个能够无间隔滚动的内容块")]
    public class StlMarquee
	{
		private StlMarquee(){}
		public const string ElementName = "stl:marquee";

		[StlAttribute(Title = "滚动延迟时间（毫秒）")]
        private const string ScrollDelay = nameof(ScrollDelay);

		[StlAttribute(Title = "滚动方向")]
        private const string Direction = nameof(Direction);

		[StlAttribute(Title = "宽度")]
        private const string Width = nameof(Width);

		[StlAttribute(Title = "高度")]
        private const string Height = nameof(Height);

        public const string DirectionVertical = "vertical";         //垂直
        public const string DirectionHorizontal = "horizontal";		//水平

        public static SortedList<string, string> DirectionList => new SortedList<string, string>
        {
            {DirectionVertical, "垂直"},
            {DirectionHorizontal, "水平"}
        };

        internal static string Parse(PageInfo pageInfo, ContextInfo contextInfo)
		{
            if (string.IsNullOrEmpty(contextInfo.InnerHtml)) return string.Empty;

            var innerBuilder = new StringBuilder(contextInfo.InnerHtml);
            StlParserManager.ParseInnerContent(innerBuilder, pageInfo, contextInfo);
            var scrollHtml = innerBuilder.ToString();

            var scrollDelay = 40;
            var direction = DirectionVertical;
            var width = "width:100%;";
            var height = string.Empty;

            foreach (var name in contextInfo.Attributes.AllKeys)
            {
                var value = contextInfo.Attributes[name];

                if (StringUtils.EqualsIgnoreCase(name, ScrollDelay))
                {
                    scrollDelay = TranslateUtils.ToInt(value, 40);
                }
                else if (StringUtils.EqualsIgnoreCase(name, Direction))
                {
                    if (value.ToLower().Equals(DirectionHorizontal.ToLower()))
                    {
                        direction = DirectionHorizontal;
                    }
                }
                else if (StringUtils.EqualsIgnoreCase(name, Width))
                {
                    value = value.Trim();
                    if (!string.IsNullOrEmpty(value))
                    {
                        if (char.IsDigit(value[value.Length - 1]))
                        {
                            width = "width:" + value + "px;";
                        }
                        else
                        {
                            width = "width:" + value + ";";
                        }
                    }
                }
                else if (StringUtils.EqualsIgnoreCase(name, Height))
                {
                    value = value.Trim();
                    if (!string.IsNullOrEmpty(value))
                    {
                        if (char.IsDigit(value[value.Length - 1]))
                        {
                            height = "height:" + value + "px;";
                        }
                        else
                        {
                            height = "height:" + value + ";";
                        }
                    }
                }
            }

            return ParseImpl(pageInfo, scrollHtml, scrollDelay, direction, width, height);
		}

        private static string ParseImpl(PageInfo pageInfo, string scrollHtml, int scrollDelay, string direction, string width, string height)
        {
            string topHtml;
            string bottomHtml;
            string scripts;

            if (string.IsNullOrEmpty(height) && direction == DirectionVertical)
            {
                height = "height:120px;";
            }

            var uniqueId = "Marquee_" + pageInfo.UniqueId;
            if (direction.Equals(DirectionVertical))
            {
                topHtml = $@"
<div id=""uniqueID_1"" style=""overflow: hidden; {width} {height}""><div id=""uniqueID_2"" align=""left"">";

                bottomHtml = @"</div><div id=""uniqueID_3""></div></div>";

                scripts = $@"
<script language=javascript>
var uniqueID_isMar = true;
var uniqueID_1=document.getElementById(""uniqueID_1"");
var uniqueID_2=document.getElementById(""uniqueID_2"");
var uniqueID_3=document.getElementById(""uniqueID_3"");
if (uniqueID_1.style.pixelHeight){{
    uniqueID_isMar = uniqueID_2.offsetHeight > uniqueID_1.style.pixelHeight;
}}else{{
    var uniqueID_height = parseInt(uniqueID_1.style.height.replace('%', '').replace('px', ''));
    uniqueID_isMar = uniqueID_2.offsetHeight > uniqueID_height;
}}
if (uniqueID_isMar){{
    uniqueID_3.innerHTML=uniqueID_2.innerHTML;
    function uniqueID_function(){{
	     <!--if(uniqueID_3.offsetTop-uniqueID_1.scrollTop<=0)-->
        if(uniqueID_2.offsetHeight*2==uniqueID_1.scrollTop+uniqueID_1.offsetHeight)
		    uniqueID_1.scrollTop-=uniqueID_2.offsetHeight;
	    else{{
		    uniqueID_1.scrollTop++
	    }}
    }}
    var uniqueID_myMar=setInterval(uniqueID_function,{scrollDelay});
    uniqueID_1.onmouseover=function() {{clearInterval(uniqueID_myMar)}}
    uniqueID_1.onmouseout=function() {{uniqueID_myMar=setInterval(uniqueID_function,{scrollDelay})}}
}}
</script>";
            }
            else
            {
                topHtml = $@"
<div id=uniqueID_1 style=""OVERFLOW: hidden; {width} {height}""><table cellpadding=0 align=left border=0 cellspace=0><tr><td id=uniqueID_2 nowrap=""nowrap"">";

                bottomHtml = @"</td><td id=uniqueID_3 nowrap=""nowrap""></td></tr></table></div>";

                scripts = $@"
<script language=javascript>
var uniqueID_isMar = true;
var uniqueID_1=document.getElementById(""uniqueID_1"");
var uniqueID_2=document.getElementById(""uniqueID_2"");
var uniqueID_3=document.getElementById(""uniqueID_3"");
if (uniqueID_1.style.pixelWidth){{
    uniqueID_isMar = uniqueID_2.offsetWidth > uniqueID_1.style.pixelWidth;
}}else{{
    var uniqueID_width = parseInt(uniqueID_1.style.width.replace('%', '').replace('px', ''));
    uniqueID_isMar = uniqueID_2.offsetWidth > uniqueID_width;
}}
if (uniqueID_isMar){{
    function uniqueID_function(){{
	    uniqueID_3.innerHTML=uniqueID_2.innerHTML;
	    <!--if(uniqueID_3.offsetLeft-uniqueID_1.scrollLeft<=0)-->
        if(uniqueID_2.offsetWidth*2+1==uniqueID_1.scrollLeft+uniqueID_1.offsetWidth )
		    uniqueID_1.scrollLeft-=uniqueID_2.offsetWidth;
	    else{{
		    uniqueID_1.scrollLeft++
	    }}
    }}
    var uniqueID_myMar=setInterval(uniqueID_function,{scrollDelay});
    uniqueID_1.onmouseover=function() {{clearInterval(uniqueID_myMar)}}
    uniqueID_1.onmouseout=function() {{uniqueID_myMar=setInterval(uniqueID_function,{scrollDelay})}}
}}
</script>";
            }

            if (!pageInfo.FootCodes.ContainsKey(ElementName + uniqueId))
            {
                pageInfo.FootCodes.Add(ElementName + uniqueId, scripts.Replace("uniqueID", uniqueId));
            }

            return topHtml.Replace("uniqueID", uniqueId) + scrollHtml + bottomHtml.Replace("uniqueID", uniqueId);
        }
	}
}
