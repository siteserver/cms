using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Datory;
using SSCMS.Configuration;
using SSCMS.Parse;
using SSCMS.Core.StlParser.Model;
using SSCMS.Core.StlParser.Utility;
using SSCMS.Models;
using SSCMS.Services;
using SSCMS.Utils;

namespace SSCMS.Core.StlParser.StlElement
{
    [StlElement(Title = "搜索", Description = "通过 stl:search 标签在模板中显示搜索结果")]
    public class StlSearch
    {
        private StlSearch() { }
        public const string ElementName = "stl:search";
        public const string ElementName2 = "stl:searchOutput";

        [StlAttribute(Title = "是否对全部站点进行搜索")]
        public const string IsAllSites = nameof(IsAllSites);

        [StlAttribute(Title = "站点名称")]
        public const string SiteName = nameof(SiteName);

        [StlAttribute(Title = "站点文件夹")]
        public const string SiteDir = nameof(SiteDir);

        [StlAttribute(Title = "站点Id列表")]
        public const string SiteIds = nameof(SiteIds);

        [StlAttribute(Title = "栏目索引")]
        public const string ChannelIndex = nameof(ChannelIndex);

        [StlAttribute(Title = "栏目名称")]
        public const string ChannelName = nameof(ChannelName);

        [StlAttribute(Title = "栏目Id列表")]
        public const string ChannelIds = nameof(ChannelIds);

        [StlAttribute(Title = "搜索类型")]
        public const string Type = nameof(Type);

        [StlAttribute(Title = "搜索关键词")]
        public const string Word = nameof(Word);

        [StlAttribute(Title = "搜索时间字段")]
        public const string DateAttribute = nameof(DateAttribute);

        [StlAttribute(Title = "搜索开始时间")]
        public const string DateFrom = nameof(DateFrom);

        [StlAttribute(Title = "搜索结束时间")]
        public const string DateTo = nameof(DateTo);

        [StlAttribute(Title = "搜索时间段")]
        public const string Since = nameof(Since);

        [StlAttribute(Title = "每页显示的内容数目")]
        public const string PageNum = nameof(PageNum);

        [StlAttribute(Title = "是否关键字高亮")]
        public const string IsHighlight = nameof(IsHighlight);

        public static async Task<object> ParseAsync(IParseManager parseManager)
        {
            var pageInfo = parseManager.PageInfo;
            var contextInfo = parseManager.ContextInfo;

            var isAllSites = false;
            var siteName = string.Empty;
            var siteDir = string.Empty;
            var siteIds = string.Empty;
            var channelIndex = string.Empty;
            var channelName = string.Empty;
            var channelIds = string.Empty;
            var type = nameof(Content.Title);
            var word = string.Empty;
            var dateAttribute = nameof(Content.AddDate);
            var dateFrom = string.Empty;
            var dateTo = string.Empty;
            var since = string.Empty;
            var pageNum = 0;
            var isHighlight = false;

            foreach (var name in contextInfo.Attributes.AllKeys)
            {
                var value = contextInfo.Attributes[name];

                if (StringUtils.EqualsIgnoreCase(name, IsAllSites))
                {
                    isAllSites = TranslateUtils.ToBool(value);
                }
                else if (StringUtils.EqualsIgnoreCase(name, SiteName))
                {
                    siteName = await parseManager.ReplaceStlEntitiesForAttributeValueAsync(value);
                }
                else if (StringUtils.EqualsIgnoreCase(name, SiteDir))
                {
                    siteDir = await parseManager.ReplaceStlEntitiesForAttributeValueAsync(value);
                }
                else if (StringUtils.EqualsIgnoreCase(name, SiteIds))
                {
                    siteIds = await parseManager.ReplaceStlEntitiesForAttributeValueAsync(value);
                }
                else if (StringUtils.EqualsIgnoreCase(name, ChannelIndex))
                {
                    channelIndex = await parseManager.ReplaceStlEntitiesForAttributeValueAsync(value);
                }
                else if (StringUtils.EqualsIgnoreCase(name, ChannelName))
                {
                    channelName = await parseManager.ReplaceStlEntitiesForAttributeValueAsync(value);
                }
                else if (StringUtils.EqualsIgnoreCase(name, ChannelIds))
                {
                    channelIds = await parseManager.ReplaceStlEntitiesForAttributeValueAsync(value);
                }
                else if (StringUtils.EqualsIgnoreCase(name, Type))
                {
                    type = await parseManager.ReplaceStlEntitiesForAttributeValueAsync(value);
                }
                else if (StringUtils.EqualsIgnoreCase(name, Word))
                {
                    word = await parseManager.ReplaceStlEntitiesForAttributeValueAsync(value);
                }
                else if (StringUtils.EqualsIgnoreCase(name, DateAttribute))
                {
                    dateAttribute = await parseManager.ReplaceStlEntitiesForAttributeValueAsync(value);
                }
                else if (StringUtils.EqualsIgnoreCase(name, DateFrom))
                {
                    dateFrom = await parseManager.ReplaceStlEntitiesForAttributeValueAsync(value);
                }
                else if (StringUtils.EqualsIgnoreCase(name, DateTo))
                {
                    dateTo = await parseManager.ReplaceStlEntitiesForAttributeValueAsync(value);
                }
                else if (StringUtils.EqualsIgnoreCase(name, Since))
                {
                    since = await parseManager.ReplaceStlEntitiesForAttributeValueAsync(value);
                }
                else if (StringUtils.EqualsIgnoreCase(name, PageNum))
                {
                    pageNum = TranslateUtils.ToInt(value);
                }
                else if (StringUtils.EqualsIgnoreCase(name, IsHighlight))
                {
                    isHighlight = TranslateUtils.ToBool(value);
                }
            }

            StlParserUtility.GetLoadingYesNo(contextInfo.InnerHtml, out var loading, out var yes, out var no);

            if (string.IsNullOrEmpty(loading))
            {
                var filePath = parseManager.PathManager.GetSiteFilesPath(Resources.Search.LoadingTemplatePath);
                loading = await parseManager.PathManager.GetContentByFilePathAsync(filePath);
            }
            if (string.IsNullOrEmpty(yes))
            {
                var filePath = parseManager.PathManager.GetSiteFilesPath(Resources.Search.YesTemplatePath);
                yes = await parseManager.PathManager.GetContentByFilePathAsync(filePath);
            }
            if (string.IsNullOrEmpty(no))
            {
                var filePath = parseManager.PathManager.GetSiteFilesPath(Resources.Search.NoTemplatePath);
                no = await parseManager.PathManager.GetContentByFilePathAsync(filePath);
            }

            await pageInfo.AddPageBodyCodeIfNotExistsAsync(ParsePage.Const.StlClient);
            await pageInfo.AddPageBodyCodeIfNotExistsAsync(ParsePage.Const.Jquery);
            var ajaxDivId = StlParserUtility.GetAjaxDivId(pageInfo.UniqueId);

            var apiUrl = GetSearchApiUrl(parseManager.SettingsManager);
            var apiParameters = GetSearchApiParameters(parseManager.SettingsManager, isAllSites, siteName, siteDir, siteIds, channelIndex, channelName, channelIds, type, word, dateAttribute, dateFrom, dateTo, since, pageNum, isHighlight, pageInfo.SiteId, ajaxDivId, yes);

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
                    var value = decodeURIComponent(arr2[1]) || '';
                    value = value.replace(/\+/g, ' ');
                    parameters[key] = value;
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
                jQuery(""#{ajaxDivId} .stl_yes"").html(res.value);
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
</script>
");

            return builder.ToString();
        }

        public class SearchRequest : Entity
        {
            public bool IsAllSites { get; set; }
            public int SiteId { get; set; }
            public string SiteName { get; set; }
            public string SiteDir { get; set; }
            public string SiteIds { get; set; }
            public string ChannelIndex { get; set; }
            public string ChannelName { get; set; }
            public string ChannelIds { get; set; }
            public string Type { get; set; }
            public string Word { get; set; }
            public string DateAttribute { get; set; }
            public string DateFrom { get; set; }
            public string DateTo { get; set; }
            public string Since { get; set; }
            public int PageNum { get; set; }
            public bool IsHighlight { get; set; }
            public string AjaxDivId { get; set; }
            public string Template { get; set; }
            public int Page { get; set; }
        }

        public static string GetSearchApiUrl(ISettingsManager settingsManager)
        {
            return PageUtils.Combine(settingsManager.ApiHost, Constants.ApiPrefix, Constants.ApiStlPrefix, Constants.RouteStlActionsSearch);
        }

        public static string GetSearchApiParameters(ISettingsManager settingsManager, bool isAllSites, string siteName, string siteDir, string siteIds, string channelIndex, string channelName, string channelIds, string type, string word, string dateAttribute, string dateFrom, string dateTo, string since, int pageNum, bool isHighlight, int siteId, string ajaxDivId, string template)
        {
            return TranslateUtils.JsonSerialize(new SearchRequest
            {
                IsAllSites = isAllSites,
                SiteName = siteName,
                SiteDir = siteDir,
                SiteIds = siteIds,
                ChannelIndex = channelIndex,
                ChannelName = channelName,
                ChannelIds = channelIds,
                Type = type,
                Word = word,
                DateAttribute = dateAttribute,
                DateFrom = dateFrom,
                DateTo = dateTo,
                Since = since,
                PageNum = pageNum,
                IsHighlight = isHighlight,
                SiteId = siteId,
                AjaxDivId = ajaxDivId,
                Template = settingsManager.Encrypt(template)
            });
        }

        public static List<string> GetSearchExcludeAttributeNames => new List<string>
        {
            nameof(SearchRequest.IsAllSites),
            nameof(SearchRequest.SiteName),
            nameof(SearchRequest.SiteDir),
            nameof(SearchRequest.SiteIds),
            nameof(SearchRequest.ChannelIndex),
            nameof(SearchRequest.ChannelName),
            nameof(SearchRequest.ChannelIds),
            nameof(SearchRequest.Type),
            nameof(SearchRequest.Word),
            nameof(SearchRequest.DateAttribute),
            nameof(SearchRequest.DateFrom),
            nameof(SearchRequest.DateTo),
            nameof(SearchRequest.Since),
            nameof(SearchRequest.PageNum),
            nameof(SearchRequest.IsHighlight),
            nameof(SearchRequest.SiteId),
            nameof(SearchRequest.AjaxDivId),
            nameof(SearchRequest.Template),
            "ExtendValues"
        };
    }
}
