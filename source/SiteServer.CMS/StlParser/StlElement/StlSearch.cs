using System.Collections.Generic;
using System.Text;
using BaiRong.Core;
using BaiRong.Core.Model;
using BaiRong.Core.Model.Attributes;
using SiteServer.CMS.Controllers.Stl;
using SiteServer.CMS.Core;
using SiteServer.CMS.StlParser.Model;
using SiteServer.CMS.StlParser.Utility;

namespace SiteServer.CMS.StlParser.StlElement
{
    [Stl(Usage = "搜索", Description = "通过 stl:search 标签在模板中显示搜索结果")]
    public class StlSearch
    {
        private StlSearch() { }
        public const string ElementName = "stl:search";
        public const string ElementName2 = "stl:searchOutput";

        public const string AttributeIsAllSites = "isAllSites";
        public const string AttributeSiteName = "siteName";
        public const string AttributeSiteDir = "siteDir";
        public const string AttributeSiteIds = "siteIds";
        public const string AttributeChannelIndex = "channelIndex";
        public const string AttributeChannelName = "channelName";
        public const string AttributeChannelIds = "channelIds";
        public const string AttributeType = "type";
        public const string AttributeWord = "word";
        public const string AttributeDateAttribute = "dateAttribute";
        public const string AttributeDateFrom = "dateFrom";
        public const string AttributeDateTo = "dateTo";
        public const string AttributeSince = "since";
        public const string AttributePageNum = "pageNum";
        public const string AttributeIsHighlight = "isHighlight";
        public const string AttributeIsDefaultDisplay = "isDefaultDisplay";

        public static SortedList<string, string> AttributeList => new SortedList<string, string>
        {
            {AttributeIsAllSites, "是否对全部站点进行搜索"},
            {AttributeSiteName, "站点名称"},
            {AttributeSiteDir, "站点文件夹"},
            {AttributeSiteIds, "站点Id列表"},
            {AttributeChannelIndex, "栏目索引"},
            {AttributeChannelName, "栏目名称"},
            {AttributeChannelIds, "栏目Id列表"},
            {AttributeType, "搜索类型"},
            {AttributeWord, "搜索关键词"},
            {AttributeDateAttribute, "搜索时间字段"},
            {AttributeDateFrom, "搜索开始时间"},
            {AttributeDateTo, "搜索结束时间"},
            {AttributeSince, "搜索时间段"},
            {AttributePageNum, "每页显示的内容数目"},
            {AttributeIsHighlight, "是否关键字高亮"},
            {AttributeIsDefaultDisplay, "是否默认显示"}
        };

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
            var isDefaultDisplay = false;

            foreach (var name in contextInfo.Attributes.Keys)
            {
                var value = contextInfo.Attributes[name];

                if (StringUtils.EqualsIgnoreCase(name, AttributeIsAllSites))
                {
                    isAllSites = TranslateUtils.ToBool(value);
                }
                else if (StringUtils.EqualsIgnoreCase(name, AttributeSiteName))
                {
                    siteName = value;
                }
                else if (StringUtils.EqualsIgnoreCase(name, AttributeSiteDir))
                {
                    siteDir = value;
                }
                else if (StringUtils.EqualsIgnoreCase(name, AttributeSiteIds))
                {
                    siteIds = value;
                }
                else if (StringUtils.EqualsIgnoreCase(name, AttributeChannelIndex))
                {
                    channelIndex = value;
                }
                else if (StringUtils.EqualsIgnoreCase(name, AttributeChannelName))
                {
                    channelName = value;
                }
                else if (StringUtils.EqualsIgnoreCase(name, AttributeChannelIds))
                {
                    channelIds = value;
                }
                else if (StringUtils.EqualsIgnoreCase(name, AttributeType))
                {
                    type = value;
                }
                else if (StringUtils.EqualsIgnoreCase(name, AttributeWord))
                {
                    word = value;
                }
                else if (StringUtils.EqualsIgnoreCase(name, AttributeDateAttribute))
                {
                    dateAttribute = value;
                }
                else if (StringUtils.EqualsIgnoreCase(name, AttributeDateFrom))
                {
                    dateFrom = value;
                }
                else if (StringUtils.EqualsIgnoreCase(name, AttributeDateTo))
                {
                    dateTo = value;
                }
                else if (StringUtils.EqualsIgnoreCase(name, AttributeSince))
                {
                    since = value;
                }
                else if (StringUtils.EqualsIgnoreCase(name, AttributePageNum))
                {
                    pageNum = TranslateUtils.ToInt(value, 0);
                }
                else if (StringUtils.EqualsIgnoreCase(name, AttributeIsHighlight))
                {
                    isHighlight = TranslateUtils.ToBool(value);
                }
                else if (StringUtils.EqualsIgnoreCase(name, AttributeIsDefaultDisplay))
                {
                    isDefaultDisplay = TranslateUtils.ToBool(value);
                }
            }

            string loading;
            string yes;
            string no;
            StlInnerUtility.GetLoadingYesNo(pageInfo, contextInfo.InnerXml, out loading, out yes, out no);

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

            pageInfo.AddPageScriptsIfNotExists(PageInfo.Components.Jquery);

            var ajaxDivId = StlParserUtility.GetAjaxDivId(pageInfo.UniqueId);
            var apiUrl = ActionsSearch.GetUrl(pageInfo.PublishmentSystemInfo.Additional.ApiUrl);
            var apiParameters = ActionsSearch.GetParameters(isAllSites, siteName, siteDir, siteIds, channelIndex, channelName, channelIds, type, word, dateAttribute, dateFrom, dateTo, since, pageNum, isHighlight, isDefaultDisplay, pageInfo.PublishmentSystemId, ajaxDivId, yes);

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
        jQuery(""#{ajaxDivId} .stl_no"").show();
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
