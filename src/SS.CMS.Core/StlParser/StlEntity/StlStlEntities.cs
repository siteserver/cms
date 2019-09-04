using System.Collections.Generic;
using System.Threading.Tasks;
using SS.CMS.Core.Common;
using SS.CMS.Core.StlParser.Models;
using SS.CMS.Core.StlParser.Utility;
using SS.CMS.Enums;
using SS.CMS.Utils;

namespace SS.CMS.Core.StlParser.StlEntity
{
    [StlElement(Title = "通用实体", Description = "通过 {stl.} 实体在模板中显示对应数据")]
    public class StlStlEntities
    {
        private StlStlEntities()
        {
        }

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

        internal static async Task<string> ParseAsync(string stlEntity, ParseContext parseContext)
        {
            var parsedContent = string.Empty;
            try
            {
                var entityName = StlParserUtility.GetNameFromEntity(stlEntity);
                var attributeName = entityName.Substring(5, entityName.Length - 6);

                if (StringUtils.EqualsIgnoreCase(PoweredBy, attributeName))//支持信息
                {
                    parsedContent = @"Powered by <a href=""http://www.siteserver.cn"" target=""_blank"">SiteServer CMS</a>";
                }
                else if (StringUtils.EqualsIgnoreCase(RootUrl, attributeName))//系统根目录地址
                {
                    parsedContent = parseContext.UrlManager.ParseConfigRootUrl("~");
                    if (!string.IsNullOrEmpty(parsedContent))
                    {
                        parsedContent = parsedContent.TrimEnd('/');
                    }
                }
                else if (StringUtils.EqualsIgnoreCase(ApiUrl, attributeName))//API地址
                {
                    parsedContent = Constants.ApiPrefix.TrimEnd('/');
                }
                else if (StringUtils.EqualsIgnoreCase(SiteId, attributeName))//ID
                {
                    parsedContent = parseContext.SiteInfo.Id.ToString();
                }
                else if (StringUtils.EqualsIgnoreCase(SiteName, attributeName))//名称
                {
                    parsedContent = parseContext.SiteInfo.SiteName;
                }
                else if (StringUtils.EqualsIgnoreCase(SiteUrl, attributeName))//域名地址
                {
                    parsedContent = parseContext.UrlManager.GetSiteUrl(parseContext.SiteInfo, parseContext.PageInfo.IsLocal).TrimEnd('/');
                }
                else if (StringUtils.EqualsIgnoreCase(SiteDir, attributeName))//文件夹
                {
                    parsedContent = parseContext.SiteInfo.SiteDir;
                }
                else if (StringUtils.EqualsIgnoreCase(CurrentUrl, attributeName))//当前页地址
                {
                    var contentInfo = await parseContext.GetContentInfoAsync();
                    parsedContent = await parseContext.GetStlCurrentUrlAsync(parseContext.SiteInfo, parseContext.ChannelId, parseContext.ContentId, contentInfo, parseContext.PageInfo.TemplateInfo.Type, parseContext.PageInfo.TemplateInfo.Id, parseContext.PageInfo.IsLocal);
                }
                else if (StringUtils.EqualsIgnoreCase(ChannelUrl, attributeName))//栏目页地址
                {
                    parsedContent = await parseContext.UrlManager.GetChannelUrlAsync(parseContext.SiteInfo, await parseContext.ChannelRepository.GetChannelAsync(parseContext.ChannelId), parseContext.PageInfo.IsLocal);
                }
                else if (StringUtils.EqualsIgnoreCase(HomeUrl, attributeName))//用户中心地址
                {
                    parsedContent = parseContext.UrlManager.GetHomeUrl(parseContext.SiteInfo, string.Empty).TrimEnd('/');
                }
                else if (StringUtils.EqualsIgnoreCase(LoginUrl, attributeName))
                {
                    var contentInfo = await parseContext.GetContentInfoAsync();
                    var returnUrl = await parseContext.GetStlCurrentUrlAsync(parseContext.SiteInfo, parseContext.ChannelId, parseContext.ContentId, contentInfo, parseContext.PageInfo.TemplateInfo.Type, parseContext.PageInfo.TemplateInfo.Id, parseContext.PageInfo.IsLocal);
                    parsedContent = parseContext.UrlManager.GetHomeUrl(parseContext.SiteInfo, $"pages/login.html?returnUrl={PageUtils.UrlEncode(returnUrl)}");
                }
                else if (StringUtils.EqualsIgnoreCase(LogoutUrl, attributeName))
                {
                    var contentInfo = await parseContext.GetContentInfoAsync();
                    var returnUrl = await parseContext.GetStlCurrentUrlAsync(parseContext.SiteInfo, parseContext.ChannelId, parseContext.ContentId, contentInfo, parseContext.PageInfo.TemplateInfo.Type, parseContext.PageInfo.TemplateInfo.Id, parseContext.PageInfo.IsLocal);
                    parsedContent = parseContext.UrlManager.GetHomeUrl(parseContext.SiteInfo, $"pages/logout.html?returnUrl={PageUtils.UrlEncode(returnUrl)}");
                }
                else if (StringUtils.EqualsIgnoreCase(RegisterUrl, attributeName))
                {
                    var contentInfo = await parseContext.GetContentInfoAsync();
                    var returnUrl = await parseContext.GetStlCurrentUrlAsync(parseContext.SiteInfo, parseContext.ChannelId, parseContext.ContentId, contentInfo, parseContext.PageInfo.TemplateInfo.Type, parseContext.PageInfo.TemplateInfo.Id, parseContext.PageInfo.IsLocal);
                    parsedContent = parseContext.UrlManager.GetHomeUrl(parseContext.SiteInfo, $"pages/register.html?returnUrl={PageUtils.UrlEncode(returnUrl)}");
                }
                else if (StringUtils.StartsWithIgnoreCase(attributeName, "TableFor"))//
                {
                    if (StringUtils.EqualsIgnoreCase(attributeName, "TableForContent"))
                    {
                        parsedContent = parseContext.SiteInfo.TableName;
                    }
                }
                else if (StringUtils.StartsWithIgnoreCase(attributeName, "Site"))//
                {
                    parsedContent = parseContext.SiteInfo.Get<string>(attributeName.Substring(4));
                }
                else if (parseContext.PageInfo.Parameters != null && parseContext.PageInfo.Parameters.ContainsKey(attributeName))
                {
                    parseContext.PageInfo.Parameters.TryGetValue(attributeName, out parsedContent);
                }
                else
                {
                    if (parseContext.SiteInfo.ContainsKey(attributeName))
                    {
                        parsedContent = parseContext.SiteInfo.Get<string>(attributeName);

                        if (!string.IsNullOrEmpty(parsedContent))
                        {
                            var styleInfo = await parseContext.TableStyleRepository.GetTableStyleInfoAsync(parseContext.SiteRepository.TableName, attributeName, parseContext.TableStyleRepository.GetRelatedIdentities(parseContext.SiteInfo.Id));

                            // 如果 styleInfo.TableStyleId <= 0，表示此字段已经被删除了，不需要再显示值了 ekun008
                            if (styleInfo.Id > 0)
                            {
                                parsedContent = InputTypeUtils.EqualsAny(styleInfo.Type, InputType.Image,
                                    InputType.File)
                                    ? parseContext.UrlManager.ParseNavigationUrl(parseContext.SiteInfo, parsedContent,
                                        parseContext.PageInfo.IsLocal)
                                    : InputParserUtility.GetContentByTableStyle(parseContext.FileManager, parseContext.UrlManager, parseContext.SettingsManager, parsedContent, string.Empty,
                                        parseContext.SiteInfo, styleInfo, string.Empty, null, string.Empty,
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
