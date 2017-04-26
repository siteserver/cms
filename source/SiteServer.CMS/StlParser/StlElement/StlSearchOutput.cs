using System;
using System.Collections.Specialized;
using System.Text;
using System.Xml;
using BaiRong.Core;
using BaiRong.Core.Model.Attributes;
using SiteServer.CMS.Controllers.Stl;
using SiteServer.CMS.StlParser.Model;
using SiteServer.CMS.StlParser.Utility;

namespace SiteServer.CMS.StlParser.StlElement
{
    public class StlSearchOutput
    {
        private StlSearchOutput() { }
        public const string ElementName = "stl:search";
        public const string ElementName2 = "stl:searchoutput";

        public const string AttributePageNum = "pagenum";			                //每页显示的内容数目
        public const string AttributeWidth = "width";				                //宽度
        public const string AttributeIsHighlight = "ishighlight";			        //是否关键字高亮
        public const string AttributeIsRedirectSingle = "isredirectsingle";        //搜索结果仅一条时是否跳转
        public const string AttributeIsDefaultDisplay = "isdefaultdisplay";        //是否默认显示
        public const string AttributeDateAttribute = "dateattribute";              //日期搜索字段

        public static ListDictionary AttributeList
        {
            get
            {
                var attributes = new ListDictionary
                {
                    {AttributePageNum, "每页显示的内容数目"},
                    {AttributeWidth, "宽度"},
                    {AttributeIsHighlight, "是否关键字高亮"},
                    {AttributeIsRedirectSingle, "搜索结果仅一条时是否跳转"},
                    {AttributeIsDefaultDisplay, "是否默认显示"},
                    {AttributeDateAttribute, "日期搜索字段"}
                };
                return attributes;
            }
        }

        public static string Parse(string stlElement, XmlNode node, PageInfo pageInfo, ContextInfo contextInfo)
        {
            string parsedContent;
            try
            {
                var pageNum = 0;
                var width = string.Empty;
                var isHighlight = false;
                var isRedirectSingle = false;
                var isDefaultDisplay = false;
                var dateAttribute = ContentAttribute.AddDate;

                var ie = node.Attributes?.GetEnumerator();
                if (ie != null)
                {
                    while (ie.MoveNext())
                    {
                        var attr = (XmlAttribute)ie.Current;
                        var attributeName = attr.Name.ToLower();
                        if (attributeName.Equals(AttributePageNum))
                        {
                            pageNum = TranslateUtils.ToInt(attr.Value, 0);
                        }
                        else if (attributeName.Equals(AttributeWidth))
                        {
                            width = attr.Value;
                        }
                        else if (attributeName.Equals(AttributeIsHighlight))
                        {
                            isHighlight = TranslateUtils.ToBool(attr.Value);
                        }
                        else if (attributeName.Equals(AttributeIsRedirectSingle))
                        {
                            isRedirectSingle = TranslateUtils.ToBool(attr.Value);
                        }
                        else if (attributeName.Equals(AttributeIsDefaultDisplay))
                        {
                            isDefaultDisplay = TranslateUtils.ToBool(attr.Value);
                        }
                        else if (attributeName.Equals(AttributeDateAttribute))
                        {
                            dateAttribute = attr.Value;
                        }
                    }
                }

                string loading;
                string yes;
                string no;
                StlParserUtility.GetSearchOutputTemplateString(node, out loading, out yes, out no, pageInfo, contextInfo);

                if (string.IsNullOrEmpty(loading))
                {
                    loading = @"<div style=""margin: 0 auto;
    padding: 40px 0;
    font-size: 14px;
    font-family: 'Microsoft YaHei';
    text-align: center;
    font-weight: 400;"">
        搜索中，请稍后...
</div>";
                }

                if (string.IsNullOrEmpty(yes))
                {
                    yes = @"<stl:pageContents cellpadding=""2"" width=""95%"" layout=""Table"" cellspacing=""2"" pageNum=""22"">
	<stl:a target=""_blank""></stl:a>
</stl:pageContents>
<br />
<stl:pageitems>
<table cellpadding=""0"" width=""90%"" cellspacing=""0"" height=""40"" align=""center"">
  <tr>
    <td align=""left""><stl:pageItem type=""FirstPage"" text=""首页""></stl:pageItem>&nbsp;|&nbsp;<stl:pageItem type=""PreviousPage"" text=""上一页""></stl:pageItem>&nbsp;|&nbsp;<stl:pageItem type=""NextPage"" text=""下一页""></stl:pageItem>&nbsp;|&nbsp;<stl:pageItem type=""LastPage"" text=""末页""></stl:pageItem></td>
    <td align=""right""><stl:pageItem type=""CurrentPageIndex"" text=""当前页：""></stl:pageItem>/<stl:pageItem type=""TotalPageNum""></stl:pageItem>&nbsp;&nbsp;<stl:pageItem type=""PageNavigation""></stl:pageItem></td>
  </tr>
</table>
</stl:pageitems>";
                }

                if (string.IsNullOrEmpty(no))
                {
                    no = @"<div style=""margin: 0 auto;
    padding: 40px 0;
    font-size: 14px;
    font-family: 'Microsoft YaHei';
    text-align: center;
    font-weight: 400;
    display: none;"">
        抱歉，找不到和您的查询相符的网页。
    </div>";
                }

                pageInfo.AddPageScriptsIfNotExists(PageInfo.Components.Jquery);

                var ajaxDivId = StlParserUtility.GetAjaxDivId(pageInfo.UniqueId);
                var apiUrl = ActionsSearch.GetUrl(pageInfo.PublishmentSystemInfo.Additional.ApiUrl);
                var apiParameters = ActionsSearch.GetParameters(pageInfo.PublishmentSystemId, ajaxDivId, pageNum, isHighlight, isRedirectSingle, isDefaultDisplay, dateAttribute, pageInfo.TemplateInfo.Charset, yes);

                var builder = new StringBuilder();
                builder.Append(string.IsNullOrEmpty(width)
                    ? $@"<div id=""{ajaxDivId}"">"
                    : $@"<div id=""{ajaxDivId}"" style=""width:{width}"">");
                builder.Append($@"<div class=""loading"">{loading}</div>");
                builder.Append($@"<div class=""yes"">{string.Empty}</div>");
                builder.Append($@"<div class=""no"">{no}</div>");
                builder.Append("</div>");

                builder.Append($@"
<script type=""text/javascript"" language=""javascript"">
jQuery(document).ready(function(){{
    jQuery(""#{ajaxDivId} .loading"").show();
    jQuery(""#{ajaxDivId} .yes"").hide();
    jQuery(""#{ajaxDivId} .no"").hide();

    var url = '{apiUrl}';
    var parameters = {apiParameters};

    var queryString = document.location.search;
    if (queryString && queryString.length > 1) {{
        queryString = queryString.substring(1);
        var arr = queryString.split('&');
        for(var i=0; i < arr.length; i++) {{
            var item = arr[i];
            var arr2 = item.split('=');
            if (arr2 && arr2.length == 2) {{
                parameters[arr2[0]] = decodeURIComponent(arr2[1]);
            }}
        }}
        if (!parameters['page']) {{
            parameters['page'] = 1;
        }}

        jQuery.support.cors = true;
        jQuery.ajax({{
            url: url,
            type: 'POST',
            contentType: 'application/json',
            data: JSON.stringify(parameters),
            dataType: 'json',
            success: function(res) {{
                jQuery(""#{ajaxDivId} .loading"").hide();
                jQuery(""#{ajaxDivId} .yes"").show();
                jQuery(""#{ajaxDivId} .no"").hide();
                jQuery(""#{ajaxDivId} .yes"").html(res);
            }},
            error: function(e) {{
                jQuery(""#{ajaxDivId} .loading"").hide();
                jQuery(""#{ajaxDivId} .yes"").hide();
                jQuery(""#{ajaxDivId} .no"").show();
            }}
        }});
    }} else {{
        jQuery(""#{ajaxDivId} .loading"").hide();
        jQuery(""#{ajaxDivId} .yes"").hide();
        jQuery(""#{ajaxDivId} .no"").show();
    }}
}});

function stlRedirect{ajaxDivId}(page)
{{
    var queryString = document.location.search;
    if (queryString && queryString.length > 1) {{
        queryString = queryString.substring(1);
        var parameters = '';
        var arr = queryString.split('&');
        for(var i=0; i < arr.length; i++) {{
            var item = arr[i];
            var arr2 = item.split('=');
            if (arr2 && arr2.length == 2) {{
                if (arr2[0] !== 'page') {{
                    parameters += item + '&';
                }}
            }}
        }}
        parameters += 'page=' + page;
        location.href = location.protocol + '//' + location.host + location.pathname + location.hash + '?' + parameters;
    }}
}}

function stlJump{ajaxDivId}(selObj)
{{
    stlRedirect{ajaxDivId}(selObj.options[selObj.selectedIndex].value);
}}
</script>
");

                parsedContent = builder.ToString();
            }
            catch (Exception ex)
            {
                parsedContent = StlParserUtility.GetStlErrorMessage(ElementName, ex);
            }

            return parsedContent;
        }


    }
}
