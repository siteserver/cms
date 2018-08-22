using System.Text;
using SiteServer.CMS.Api.Sys.Stl;
using SiteServer.Utils;
using SiteServer.CMS.Core;
using SiteServer.CMS.Model.Attributes;
using SiteServer.CMS.StlParser.Model;
using SiteServer.CMS.StlParser.Utility;

namespace SiteServer.CMS.StlParser.StlElement
{
    [StlClass(Usage = "搜索", Description = "通过 stl:search 标签在模板中显示搜索结果")]
    public class StlSearch
    {
        private StlSearch() { }
        public const string ElementName = "stl:search";
        public const string ElementName2 = "stl:searchOutput";

        public static readonly Attr IsAllSites = new Attr("isAllSites", "是否对全部站点进行搜索");
        public static readonly Attr SiteName = new Attr("siteName", "站点名称");
        public static readonly Attr SiteDir = new Attr("siteDir", "站点文件夹");
        public static readonly Attr SiteIds = new Attr("siteIds", "站点Id列表");
        public static readonly Attr ChannelIndex = new Attr("channelIndex", "栏目索引");
        public static readonly Attr ChannelName = new Attr("channelName", "栏目名称");
        public static readonly Attr ChannelIds = new Attr("channelIds", "栏目Id列表");
        public static readonly Attr Type = new Attr("type", "搜索类型");
        public static readonly Attr Word = new Attr("word", "搜索关键词");
        public static readonly Attr DateAttribute = new Attr("dateAttribute", "搜索时间字段");
        public static readonly Attr DateFrom = new Attr("dateFrom", "搜索开始时间");
        public static readonly Attr DateTo = new Attr("dateTo", "搜索结束时间");
        public static readonly Attr Since = new Attr("since", "搜索时间段");
        public static readonly Attr PageNum = new Attr("pageNum", "每页显示的内容数目");
        public static readonly Attr IsHighlight = new Attr("isHighlight", "是否关键字高亮");

        public static string Parse(PageInfo pageInfo, ContextInfo contextInfo)
        {
            var isAllSites = false;
            var siteName = string.Empty;
            var siteDir = string.Empty;
            var siteIds = string.Empty;
            var channelIndex = string.Empty;
            var channelName = string.Empty;
            var channelIds = string.Empty;
            var type = ContentAttribute.Title;
            var word = string.Empty;
            var dateAttribute = ContentAttribute.AddDate;
            var dateFrom = string.Empty;
            var dateTo = string.Empty;
            var since = string.Empty;
            var pageNum = 0;
            var isHighlight = false;

            foreach (var name in contextInfo.Attributes.AllKeys)
            {
                var value = contextInfo.Attributes[name];

                if (StringUtils.EqualsIgnoreCase(name, IsAllSites.Name))
                {
                    isAllSites = TranslateUtils.ToBool(value);
                }
                else if (StringUtils.EqualsIgnoreCase(name, SiteName.Name))
                {
                    siteName = value;
                }
                else if (StringUtils.EqualsIgnoreCase(name, SiteDir.Name))
                {
                    siteDir = value;
                }
                else if (StringUtils.EqualsIgnoreCase(name, SiteIds.Name))
                {
                    siteIds = value;
                }
                else if (StringUtils.EqualsIgnoreCase(name, ChannelIndex.Name))
                {
                    channelIndex = value;
                }
                else if (StringUtils.EqualsIgnoreCase(name, ChannelName.Name))
                {
                    channelName = value;
                }
                else if (StringUtils.EqualsIgnoreCase(name, ChannelIds.Name))
                {
                    channelIds = value;
                }
                else if (StringUtils.EqualsIgnoreCase(name, Type.Name))
                {
                    type = value;
                }
                else if (StringUtils.EqualsIgnoreCase(name, Word.Name))
                {
                    word = value;
                }
                else if (StringUtils.EqualsIgnoreCase(name, DateAttribute.Name))
                {
                    dateAttribute = value;
                }
                else if (StringUtils.EqualsIgnoreCase(name, DateFrom.Name))
                {
                    dateFrom = value;
                }
                else if (StringUtils.EqualsIgnoreCase(name, DateTo.Name))
                {
                    dateTo = value;
                }
                else if (StringUtils.EqualsIgnoreCase(name, Since.Name))
                {
                    since = value;
                }
                else if (StringUtils.EqualsIgnoreCase(name, PageNum.Name))
                {
                    pageNum = TranslateUtils.ToInt(value, 0);
                }
                else if (StringUtils.EqualsIgnoreCase(name, IsHighlight.Name))
                {
                    isHighlight = TranslateUtils.ToBool(value);
                }
            }

            string loading;
            string yes;
            string no;
            StlParserUtility.GetLoadingYesNo(contextInfo.InnerHtml, out loading, out yes, out no);

            if (string.IsNullOrEmpty(loading))
            {
                loading = TemplateManager.GetContentByFilePath(SiteFilesAssets.Search.LoadingTemplatePath);
            }
            if (string.IsNullOrEmpty(yes))
            {
                yes = TemplateManager.GetContentByFilePath(SiteFilesAssets.Search.YesTemplatePath);
            }
            if (string.IsNullOrEmpty(no))
            {
                no = TemplateManager.GetContentByFilePath(SiteFilesAssets.Search.NoTemplatePath);
            }

            pageInfo.AddPageBodyCodeIfNotExists(PageInfo.Const.Jquery);

            var ajaxDivId = StlParserUtility.GetAjaxDivId(pageInfo.UniqueId);
            var apiUrl = ApiRouteActionsSearch.GetUrl(pageInfo.ApiUrl);
            var apiParameters = ApiRouteActionsSearch.GetParameters(isAllSites, siteName, siteDir, siteIds, channelIndex, channelName, channelIds, type, word, dateAttribute, dateFrom, dateTo, since, pageNum, isHighlight, pageInfo.SiteId, ajaxDivId, yes);

            var builder = new StringBuilder();
            builder.Append($@"
<div id=""{ajaxDivId}"">
    <div class=""stl_loading"">{loading}</div>
    <div class=""stl_yes"" style=""display:none""></div>
    <div class=""stl_no"" style=""display:none"">{no}</div>
</div>
");

            builder.Append($@"
<script type=""text/javascript"" language=""javascript"">
jQuery(document).ready(function(){{
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
                var key = (arr2[0] || '').toLowerCase();
                if (key) {{
                    parameters[key] = decodeURIComponent(arr2[1]);
                }}
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
                jQuery(""#{ajaxDivId} .stl_loading"").hide();
                jQuery(""#{ajaxDivId} .stl_yes"").show();
                jQuery(""#{ajaxDivId} .stl_no"").hide();
                jQuery(""#{ajaxDivId} .stl_yes"").html(res);
            }},
            error: function(e) {{
                jQuery(""#{ajaxDivId} .stl_loading"").hide();
                jQuery(""#{ajaxDivId} .stl_yes"").hide();
                jQuery(""#{ajaxDivId} .stl_no"").show();
            }}
        }});
    }} else {{
        jQuery(""#{ajaxDivId} .stl_loading"").hide();
        jQuery(""#{ajaxDivId} .stl_yes"").hide();
        jQuery(""#{ajaxDivId} .stl_no"").hide();
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

            return builder.ToString();
        }
    }
}
