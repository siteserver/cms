using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Datory;
using SSCMS.Configuration;
using SSCMS.Core.Repositories;
using SSCMS.Core.StlParser.Attributes;
using SSCMS.Parse;
using SSCMS.Core.StlParser.Models;
using SSCMS.Core.StlParser.Utility;
using SSCMS.Models;
using SSCMS.Services;
using SSCMS.Utils;

namespace SSCMS.Core.StlParser.StlElement
{
    [StlElement(Title = "搜索", Description = "通过 stl:search 标签在模板中显示搜索结果")]
    public static class StlSearch
    {
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

        [StlAttribute(Title = "栏目索引")]
        public const string Index = nameof(Index);

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
                else if (StringUtils.EqualsIgnoreCase(name, ChannelIndex) || StringUtils.EqualsIgnoreCase(name, Index))
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
                loading = parseManager.PathManager.GetContentByFilePath(filePath);
            }
            if (string.IsNullOrEmpty(yes))
            {
                var filePath = parseManager.PathManager.GetSiteFilesPath(Resources.Search.YesTemplatePath);
                yes = parseManager.PathManager.GetContentByFilePath(filePath);
            }
            if (string.IsNullOrEmpty(no))
            {
                var filePath = parseManager.PathManager.GetSiteFilesPath(Resources.Search.NoTemplatePath);
                no = parseManager.PathManager.GetContentByFilePath(filePath);
            }

            await pageInfo.AddPageHeadCodeIfNotExistsAsync(ParsePage.Const.StlClient);
            await pageInfo.AddPageHeadCodeIfNotExistsAsync(ParsePage.Const.Jquery);
            var elementId = StringUtils.GetElementId();

            var apiUrl = GetSearchApiUrl(pageInfo.Site, parseManager.PathManager);
            var apiParameters = GetSearchApiParameters(parseManager.SettingsManager, isAllSites, siteName, siteDir, siteIds, channelIndex, channelName, channelIds, type, word, dateAttribute, dateFrom, dateTo, since, pageNum, isHighlight, pageInfo.SiteId, elementId, yes);

            var builder = new StringBuilder();
            builder.Append($@"
<div id=""{elementId}"">
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
                jQuery(""#{elementId} .stl_loading"").hide();
                jQuery(""#{elementId} .stl_yes"").show();
                jQuery(""#{elementId} .stl_no"").hide();
                jQuery(""#{elementId} .stl_yes"").html(res.value);
            }},
            error: function(e) {{
                jQuery(""#{elementId} .stl_loading"").hide();
                jQuery(""#{elementId} .stl_yes"").hide();
                jQuery(""#{elementId} .stl_no"").show();
            }}
        }});
    }} else {{
        jQuery(""#{elementId} .stl_loading"").hide();
        jQuery(""#{elementId} .stl_yes"").hide();
        jQuery(""#{elementId} .stl_no"").hide();
    }}
}});

function stlRedirect{elementId}(page)
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

        public static string GetSearchApiUrl(Site site, IPathManager pathManager)
        {
            return pathManager.GetApiHostUrl(site, Constants.ApiPrefix, Constants.ApiStlPrefix, Constants.RouteStlActionsSearch);
        }

        public static string GetSearchApiParameters(ISettingsManager settingsManager, bool isAllSites, string siteName, string siteDir, string siteIds, string channelIndex, string channelName, string channelIds, string type, string word, string dateAttribute, string dateFrom, string dateTo, string since, int pageNum, bool isHighlight, int siteId, string ajaxDivId, string template)
        {
            return TranslateUtils.JsonSerialize(new StlSearchRequest
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
            nameof(Entity.Id),
            nameof(Entity.Guid),
            nameof(Entity.CreatedDate),
            nameof(Entity.LastModifiedDate),
            nameof(StlSearchRequest.IsAllSites),
            nameof(StlSearchRequest.SiteId),
            nameof(StlSearchRequest.SiteName),
            nameof(StlSearchRequest.SiteDir),
            nameof(StlSearchRequest.SiteIds),
            nameof(StlSearchRequest.ChannelIndex),
            nameof(StlSearchRequest.ChannelName),
            nameof(StlSearchRequest.ChannelIds),
            nameof(StlSearchRequest.Type),
            nameof(StlSearchRequest.Word),
            nameof(StlSearchRequest.DateAttribute),
            nameof(StlSearchRequest.DateFrom),
            nameof(StlSearchRequest.DateTo),
            nameof(StlSearchRequest.Since),
            nameof(StlSearchRequest.PageNum),
            nameof(StlSearchRequest.IsHighlight),
            nameof(StlSearchRequest.AjaxDivId),
            nameof(StlSearchRequest.Template),
            nameof(StlSearchRequest.Page),
            ContentRepository.AttrExtendValues
        };
    }
}
