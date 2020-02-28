using System.Collections.Generic;
using SS.CMS.Abstractions;
using SS.CMS.StlParser.Model;
using SS.CMS.StlParser.Utility;
using System.Threading.Tasks;
using SS.CMS.Core;

namespace SS.CMS.StlParser.StlEntity
{
    [StlElement(Title = "通用实体", Description = "通过 {stl.} 实体在模板中显示对应数据")]
    public static class StlStlEntities
    {
        public const string EntityName = "stl";

        public static string PoweredBy = "PoweredBy";
        public static string SiteName = "SiteName";
        public static string SiteId = "SiteId";
        public static string SiteDir = "SiteDir";
        public static string SiteUrl = "SiteUrl";
        public static string RootUrl = "RootUrl";
        public static string ApiUrl = "ApiUrl";
        public static string CurrentUrl = "CurrentUrl";
        public static string ChannelUrl = "ChannelUrl";
        public static string HomeUrl = "HomeUrl";
        public static string LoginUrl = "LoginUrl";
        public static string RegisterUrl = "RegisterUrl";
        public static string LogoutUrl = "LogoutUrl";

        public static SortedList<string, string> AttributeList => new SortedList<string, string>
        {
            {PoweredBy, "PoweredBy 链接"},
            {SiteName, "站点名称"},
            {SiteId, "站点ID"},
            {SiteDir, "站点文件夹"},
            {SiteUrl, "站点根目录地址"},
            {RootUrl, "系统根目录地址"},
            {ApiUrl, "Api地址"},
            {CurrentUrl, "当前页地址"},
            {ChannelUrl, "栏目页地址"},
            {HomeUrl, "用户中心地址"},
            {LoginUrl, "用户中心登录页地址"},
            {RegisterUrl, "用户中心注册页地址"},
            {LogoutUrl, "退出登录页地址"}
        };

        internal static async Task<string> ParseAsync(string stlEntity, IParseManager parseManager)
        {
            var databaseManager = parseManager.DatabaseManager;
            var pageInfo = parseManager.PageInfo;
            var contextInfo = parseManager.ContextInfo;

            var parsedContent = string.Empty;
            try
            {
                var entityName = StlParserUtility.GetNameFromEntity(stlEntity);
                var attributeName = entityName.Substring(5, entityName.Length - 6);

                if (StringUtils.EqualsIgnoreCase(PoweredBy, attributeName))//支持信息
                {
                    parsedContent = @"Powered by <a href=""https://www.siteserver.cn"" target=""_blank"">SiteServer CMS</a>";
                }
                else if (StringUtils.EqualsIgnoreCase(RootUrl, attributeName))//系统根目录地址
                {
                    parsedContent = PageUtils.ParseConfigRootUrl("~");
                    if (!string.IsNullOrEmpty(parsedContent))
                    {
                        parsedContent = parsedContent.TrimEnd('/');
                    }
                }
                else if (StringUtils.EqualsIgnoreCase(ApiUrl, attributeName))//API地址
                {
                    parsedContent = pageInfo.ApiUrl.TrimEnd('/');
                }
                else if (StringUtils.EqualsIgnoreCase(SiteId, attributeName))//ID
                {
                    parsedContent = pageInfo.SiteId.ToString();
                }
                else if (StringUtils.EqualsIgnoreCase(SiteName, attributeName))//名称
                {
                    parsedContent = pageInfo.Site.SiteName;
                }
                else if (StringUtils.EqualsIgnoreCase(SiteUrl, attributeName))//域名地址
                {
                    parsedContent = (await parseManager.PathManager.GetSiteUrlAsync(pageInfo.Site, pageInfo.IsLocal)).TrimEnd('/');
                }
                else if (StringUtils.EqualsIgnoreCase(SiteDir, attributeName))//文件夹
                {
                    parsedContent = pageInfo.Site.SiteDir;
                }
                else if (StringUtils.EqualsIgnoreCase(CurrentUrl, attributeName))//当前页地址
                {
                    var contentInfo = await parseManager.GetContentAsync();
                    parsedContent = await StlParserUtility.GetStlCurrentUrlAsync(parseManager, pageInfo.Site, contextInfo.ChannelId, contextInfo.ContentId, contentInfo, pageInfo.Template.TemplateType, pageInfo.Template.Id, pageInfo.IsLocal);
                }
                else if (StringUtils.EqualsIgnoreCase(ChannelUrl, attributeName))//栏目页地址
                {
                    parsedContent = await parseManager.PathManager.GetChannelUrlAsync(pageInfo.Site, await databaseManager.ChannelRepository.GetAsync(contextInfo.ChannelId), pageInfo.IsLocal);
                }
                else if (StringUtils.EqualsIgnoreCase(HomeUrl, attributeName))//用户中心地址
                {
                    parsedContent = PageUtils.GetHomeUrl(string.Empty).TrimEnd('/');
                }
                else if (StringUtils.EqualsIgnoreCase(LoginUrl, attributeName))
                {
                    var contentInfo = await parseManager.GetContentAsync();
                    var returnUrl = await StlParserUtility.GetStlCurrentUrlAsync(parseManager, pageInfo.Site, contextInfo.ChannelId, contextInfo.ContentId, contentInfo, pageInfo.Template.TemplateType, pageInfo.Template.Id, pageInfo.IsLocal);
                    parsedContent = PageUtils.GetHomeUrl($"pages/login.html?returnUrl={PageUtils.UrlEncode(returnUrl)}");
                }
                else if (StringUtils.EqualsIgnoreCase(LogoutUrl, attributeName))
                {
                    var contentInfo = await parseManager.GetContentAsync();
                    var returnUrl = await StlParserUtility.GetStlCurrentUrlAsync(parseManager, pageInfo.Site, contextInfo.ChannelId, contextInfo.ContentId, contentInfo, pageInfo.Template.TemplateType, pageInfo.Template.Id, pageInfo.IsLocal);
                    parsedContent = PageUtils.GetHomeUrl($"pages/logout.html?returnUrl={PageUtils.UrlEncode(returnUrl)}");
                }
                else if (StringUtils.EqualsIgnoreCase(RegisterUrl, attributeName))
                {
                    var contentInfo = await parseManager.GetContentAsync();
                    var returnUrl = await StlParserUtility.GetStlCurrentUrlAsync(parseManager, pageInfo.Site, contextInfo.ChannelId, contextInfo.ContentId, contentInfo, pageInfo.Template.TemplateType, pageInfo.Template.Id, pageInfo.IsLocal);
                    parsedContent = PageUtils.GetHomeUrl($"pages/register.html?returnUrl={PageUtils.UrlEncode(returnUrl)}");
                }
                else if (StringUtils.StartsWithIgnoreCase(attributeName, "TableFor"))//
                {
                    if (StringUtils.EqualsIgnoreCase(attributeName, "TableForContent"))
                    {
                        parsedContent = pageInfo.Site.TableName;
                    }
                }
                else if (StringUtils.StartsWithIgnoreCase(attributeName, "Site"))//
                {
                    parsedContent = pageInfo.Site.Get<string>(attributeName.Substring(4));
                }
                else if (pageInfo.Parameters != null && pageInfo.Parameters.ContainsKey(attributeName))
                {
                    pageInfo.Parameters.TryGetValue(attributeName, out parsedContent);
                }
                else
                {
                    if (pageInfo.Site.ContainsKey(attributeName))
                    {
                        parsedContent = pageInfo.Site.Get<string>(attributeName);

                        if (!string.IsNullOrEmpty(parsedContent))
                        {
                            var styleInfo = await databaseManager.TableStyleRepository.GetTableStyleAsync(databaseManager.SiteRepository.TableName, attributeName, databaseManager.TableStyleRepository.GetRelatedIdentities(pageInfo.SiteId));

                            if (styleInfo.Id > 0)
                            {
                                parsedContent = InputTypeUtils.EqualsAny(styleInfo.InputType, InputType.Image,
                                    InputType.File)
                                    ? await parseManager.PathManager.ParseNavigationUrlAsync(pageInfo.Site, parsedContent,
                                        pageInfo.IsLocal)
                                    : await InputParserUtility.GetContentByTableStyleAsync(parseManager.PathManager, parsedContent, string.Empty, pageInfo.Config, pageInfo.Site, styleInfo, string.Empty, null, string.Empty,
                                        true);
                            }
                            else
                            { // 如果字段已经被删除或不再显示了，则此字段的值为空。有时虚拟字段值不会清空
                                parsedContent = string.Empty;
                            }
                        }
                    }
                }
            }
            catch
            {
                // ignored
            }

            return parsedContent;
        }
    }
}
