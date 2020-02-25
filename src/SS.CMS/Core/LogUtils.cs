using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Datory;
using SS.CMS.StlParser.Model;
using SS.CMS.Abstractions;
using SS.CMS.Framework;

namespace SS.CMS.Core
{
    public static class LogUtils
    {
        public const string CategoryStl = "stl";
        public const string CategoryAdmin = "admin";
        public const string CategoryHome = "home";
        public const string CategoryApi = "api";

        public static readonly Lazy<List<KeyValuePair<string, string>>> AllCategoryList = new Lazy<List<KeyValuePair<string, string>>>(
            () =>
            {
                var list = new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>(CategoryStl, "STL 解析错误"),
                    new KeyValuePair<string, string>(CategoryAdmin, "后台错误"),
                    new KeyValuePair<string, string>(CategoryHome, "用户中心错误"),
                    new KeyValuePair<string, string>(CategoryApi, "API错误")
                };
                return list;
            });

        public static async Task<string> AddStlErrorLogAsync(PageInfo pageInfo, string elementName, string stlContent, Exception ex)
        {
            var summary = string.Empty;
            if (pageInfo != null)
            {
                summary = $@"站点名称：{pageInfo.Site.SiteName}，
模板类型：{pageInfo.Template.TemplateType.GetDisplayName()}，
模板名称：{pageInfo.Template.TemplateName}
<br />";
            }

            summary += $@"STL标签：{WebUtils.HtmlEncode(stlContent)}";
            await DataProvider.ErrorLogRepository.AddErrorLogAsync(new ErrorLog
            {
                Id = 0,
                Category = LogUtils.CategoryStl,
                PluginId = string.Empty,
                Message = ex.Message,
                StackTrace = ex.StackTrace,
                Summary = summary,
                AddDate = DateTime.Now
            });

            return $@"
<!--
{elementName}
error: {ex.Message}
stl: {stlContent}
-->";
        }
    }
}
