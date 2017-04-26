using System;
using System.Collections.Specialized;
using System.Text;
using System.Xml;
using BaiRong.Core;
using SiteServer.CMS.StlParser.Model;
using SiteServer.CMS.StlParser.Utility;

namespace SiteServer.CMS.StlParser.StlElement
{
	/*
		<stl:marquee 
		scrollDelay="85" 
		direction="vertical/horizontal"
		width="220"
		height="160"></stl:marquee>
		 * */
	public class StlMarquee
	{
		private StlMarquee(){}
		public const string ElementName = "stl:marquee";//无间隔滚动

		public const string Attribute_ScrollDelay = "scrolldelay";	//滚动延迟时间（毫秒）
		public const string Attribute_Direction = "direction";		//滚动方向
		public const string Attribute_Width = "width";				//宽度
		public const string Attribute_Height = "height";			//高度
        public const string Attribute_IsDynamic = "isdynamic";              //是否动态显示

		public const string Direction_Vertical = "vertical";			//垂直
		public const string Direction_Horizontal = "horizontal";		//水平

		public static ListDictionary AttributeList
		{
			get
			{
				var attributes = new ListDictionary();
				attributes.Add(Attribute_ScrollDelay, "滚动延迟时间毫秒");
				attributes.Add(Attribute_Direction, "滚动方向");
				attributes.Add(Attribute_Width, "宽度");
				attributes.Add(Attribute_Height, "高度");
                attributes.Add(Attribute_IsDynamic, "是否动态显示");
				return attributes;
			}
		}


		//对“滚动框”（stl:marquee）元素进行解析
        internal static string Parse(string stlElement, XmlNode node, PageInfo pageInfo, ContextInfo contextInfo)
		{
			var parsedContent = string.Empty;
			try
			{
				var scrollHtml = string.Empty;
				if (node.InnerXml.Trim().Length == 0)
				{
					return string.Empty;
				}
				else
				{
                    var innerBuilder = new StringBuilder(node.InnerXml);
                    StlParserManager.ParseInnerContent(innerBuilder, pageInfo, contextInfo);
                    scrollHtml = innerBuilder.ToString();
				}

				var ie = node.Attributes.GetEnumerator();
                var scrollDelay = 40;   //默认延迟时间;
				var direction = Direction_Vertical;
                var width = "WIDTH:100%;";
				var height = string.Empty;
                var isDynamic = false;

				while (ie.MoveNext())
				{
					var attr = (XmlAttribute)ie.Current;
					var attributeName = attr.Name.ToLower();
					if (attributeName.Equals(Attribute_ScrollDelay))
					{
						try
						{
							scrollDelay = int.Parse(attr.Value);
						}
						catch{}
					}
					else if (attributeName.Equals(Attribute_Direction))
					{
						if (attr.Value.ToLower().Equals(Direction_Horizontal.ToLower()))
						{
							direction = Direction_Horizontal;
						}
					}
					else if (attributeName.Equals(Attribute_Width))
					{
                        attr.Value = attr.Value.Trim();
						if (!string.IsNullOrEmpty(attr.Value))
                        {
                            if (char.IsDigit(attr.Value[attr.Value.Length - 1]))
                            {
                                width = "WIDTH:" + attr.Value + "px;";
                            }
                            else
                            {
                                width = "WIDTH:" + attr.Value + ";";
                            }
                        }
					}
					else if (attributeName.Equals(Attribute_Height))
					{
                        attr.Value = attr.Value.Trim();
                        if (!string.IsNullOrEmpty(attr.Value))
                        {
                            if (char.IsDigit(attr.Value[attr.Value.Length - 1]))
                            {
                                height = "HEIGHT:" + attr.Value + "px;";
                            }
                            else
                            {
                                height = "HEIGHT:" + attr.Value + ";";
                            }
                        }
                    }
                    else if (attributeName.Equals(Attribute_IsDynamic))
                    {
                        isDynamic = TranslateUtils.ToBool(attr.Value);
                    }
				}

                if (isDynamic)
                {
                    parsedContent = StlDynamic.ParseDynamicElement(stlElement, pageInfo, contextInfo);
                }
                else
                {
                    parsedContent = ParseImpl(pageInfo, scrollHtml, scrollDelay, direction, width, height);
                }
			}
            catch (Exception ex)
            {
                parsedContent = StlParserUtility.GetStlErrorMessage(ElementName, ex);
            }

			return parsedContent;
		}

        private static string ParseImpl(PageInfo pageInfo, string scrollHtml, int scrollDelay, string direction, string width, string height)
        {
            var topHtml = string.Empty;
            var bottomHtml = string.Empty;
            var scripts = string.Empty;

            if (string.IsNullOrEmpty(height) && direction == Direction_Vertical)
            {
                height = "HEIGHT:120px;";
            }

            var uniqueID = "Marquee_" + pageInfo.UniqueId;
            if (direction.Equals(Direction_Vertical))
            {
                topHtml = $@"
<DIV id=uniqueID_1 style=""OVERFLOW: hidden; {width} {height}""><DIV id=uniqueID_2 align=left>";

                bottomHtml = @"</DIV><DIV id=uniqueID_3></DIV></DIV>";

                scripts = $@"
<SCRIPT language=javascript>
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
</SCRIPT>";
            }
            else
            {
                topHtml = $@"
<div id=uniqueID_1 style=""OVERFLOW: hidden; {width} {height}""><table cellpadding=0 align=left border=0 cellspace=0><tr><td id=uniqueID_2 nowrap=""nowrap"">";

                bottomHtml = @"</td><td id=uniqueID_3 nowrap=""nowrap""></td></tr></table></div>";

                scripts = $@"
<SCRIPT language=javascript>
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
</SCRIPT>";
            }

            pageInfo.AddPageEndScriptsIfNotExists(ElementName + uniqueID, scripts.Replace("uniqueID", uniqueID));

            return topHtml.Replace("uniqueID", uniqueID) + scrollHtml + bottomHtml.Replace("uniqueID", uniqueID);
        }
	}
}
