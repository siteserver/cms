using System.Collections.Generic;
using System.Text;
using BaiRong.Core;
using BaiRong.Core.Model.Enumerations;
using SiteServer.CMS.Core;
using SiteServer.CMS.StlParser.Model;
using SiteServer.CMS.StlParser.Utility;

namespace SiteServer.CMS.StlParser.StlEntity
{
    [Stl(Usage = "通用实体", Description = "通过 {stl.} 实体在模板中显示对应数据")]
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
        public static string LogoutUrl = "LogoutUrl";
        public static string RegisterUrl = "RegisterUrl";

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
	        {LoginUrl, "登录地址"},
	        {LogoutUrl, "登出地址"},
	        {RegisterUrl, "注册地址"}
	    };

        internal static string Parse(string stlEntity, PageInfo pageInfo, ContextInfo contextInfo)
        {
            var parsedContent = string.Empty;
            try
            {
                var entityName = StlParserUtility.GetNameFromEntity(stlEntity);
                var attributeName = entityName.Substring(5, entityName.Length - 6);

                if (StringUtils.EqualsIgnoreCase(PoweredBy, attributeName))//支持信息
                {
                    parsedContent =
                        $@"Powered by <a href=""http://www.siteserver.cn"" target=""_blank"">{EPublishmentSystemTypeUtils
                            .GetAppName(pageInfo.PublishmentSystemInfo.PublishmentSystemType)}</a>";
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
                    parsedContent = pageInfo.PublishmentSystemId.ToString();
                }
                else if (StringUtils.EqualsIgnoreCase(SiteName, attributeName))//名称
                {
                    parsedContent = pageInfo.PublishmentSystemInfo.PublishmentSystemName;
                }
                else if (StringUtils.EqualsIgnoreCase(SiteUrl, attributeName))//域名地址
                {
                    if (!string.IsNullOrEmpty(pageInfo.PublishmentSystemInfo.PublishmentSystemUrl))
                    {
                        parsedContent = pageInfo.PublishmentSystemInfo.PublishmentSystemUrl.TrimEnd('/');
                    }
                }
                else if (StringUtils.EqualsIgnoreCase(SiteDir, attributeName))//文件夹
                {
                    parsedContent = pageInfo.PublishmentSystemInfo.PublishmentSystemDir;
                }
                else if (StringUtils.EqualsIgnoreCase(CurrentUrl, attributeName))//当前页地址
                {
                    parsedContent = StlUtility.GetStlCurrentUrl(pageInfo, contextInfo.ChannelId, contextInfo.ContentId, contextInfo.ContentInfo);
                }
                else if (StringUtils.EqualsIgnoreCase(ChannelUrl, attributeName))//栏目页地址
                {
                    parsedContent = PageUtility.GetChannelUrl(pageInfo.PublishmentSystemInfo, NodeManager.GetNodeInfo(pageInfo.PublishmentSystemId, contextInfo.ChannelId));
                }
                else if (StringUtils.EqualsIgnoreCase(HomeUrl, attributeName))//用户中心地址
                {
                    parsedContent = pageInfo.HomeUrl.TrimEnd('/');
                }
                else if (StringUtils.EqualsIgnoreCase(attributeName, LoginUrl))
                {
                    var returnUrl = StlUtility.GetStlCurrentUrl(pageInfo, contextInfo.ChannelId, contextInfo.ContentId, contextInfo.ContentInfo);
                    parsedContent = HomeUtils.GetLoginUrl(pageInfo.HomeUrl, returnUrl);
                }
                else if (StringUtils.EqualsIgnoreCase(attributeName, LogoutUrl))
                {
                    var returnUrl = StlUtility.GetStlCurrentUrl(pageInfo, contextInfo.ChannelId, contextInfo.ContentId, contextInfo.ContentInfo);
                    parsedContent = HomeUtils.GetLogoutUrl(pageInfo.HomeUrl, returnUrl);
                }
                else if (StringUtils.EqualsIgnoreCase(attributeName, RegisterUrl))
                {
                    var returnUrl = StlUtility.GetStlCurrentUrl(pageInfo, contextInfo.ChannelId, contextInfo.ContentId, contextInfo.ContentInfo);
                    parsedContent = HomeUtils.GetRegisterUrl(pageInfo.HomeUrl, returnUrl);
                }
                else if (StringUtils.StartsWithIgnoreCase(attributeName, "TableFor"))//
                {
                    if (StringUtils.EqualsIgnoreCase(attributeName, "TableForContent"))
                    {
                        parsedContent = pageInfo.PublishmentSystemInfo.AuxiliaryTableForContent;
                    }
                    else if (StringUtils.EqualsIgnoreCase(attributeName, "TableForGovInteract"))
                    {
                        parsedContent = pageInfo.PublishmentSystemInfo.AuxiliaryTableForGovInteract;
                    }
                    else if (StringUtils.EqualsIgnoreCase(attributeName, "TableForGovPublic"))
                    {
                        parsedContent = pageInfo.PublishmentSystemInfo.AuxiliaryTableForGovPublic;
                    }
                    else if (StringUtils.EqualsIgnoreCase(attributeName, "TableForJob"))
                    {
                        parsedContent = pageInfo.PublishmentSystemInfo.AuxiliaryTableForJob;
                    }
                    else if (StringUtils.EqualsIgnoreCase(attributeName, "TableForVote"))
                    {
                        parsedContent = pageInfo.PublishmentSystemInfo.AuxiliaryTableForVote;
                    }
                }
                else if (StringUtils.StartsWithIgnoreCase(attributeName, "Site"))//
                {
                    parsedContent = pageInfo.PublishmentSystemInfo.Additional.GetExtendedAttribute(attributeName.Substring(4));
                }
                else
                {
                    if (pageInfo.PublishmentSystemInfo.Additional.ContainsKey(attributeName))
                    {
                        parsedContent = pageInfo.PublishmentSystemInfo.Additional.GetExtendedAttribute(attributeName);
                        if (parsedContent.StartsWith("@"))
                        {
                            parsedContent = PageUtility.ParseNavigationUrl(pageInfo.PublishmentSystemId, parsedContent);
                        }
                    }
                    else
                    {
                        var stlTagInfo = DataProvider.StlTagDao.GetStlTagInfo(pageInfo.PublishmentSystemId, attributeName) ??
                                         DataProvider.StlTagDao.GetStlTagInfo(0, attributeName);
                        if (!string.IsNullOrEmpty(stlTagInfo?.TagContent))
                        {
                            var innerBuilder = new StringBuilder(stlTagInfo.TagContent);
                            StlParserManager.ParseInnerContent(innerBuilder, pageInfo, contextInfo);
                            parsedContent = innerBuilder.ToString();
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
