using System;
using System.Collections.Specialized;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI.WebControls;
using Datory;
using SiteServer.CMS.Core;
using SiteServer.Plugin;
using SiteServer.Utils;
using SiteServer.Utils.Enumerations;

namespace SiteServer.CMS.Fx
{
    public static class FxUtils
    {
        public static HorizontalAlign ToHorizontalAlign(string typeStr)
        {
            return (HorizontalAlign)ToEnum(typeof(HorizontalAlign), typeStr, HorizontalAlign.Left);
        }

        public static VerticalAlign ToVerticalAlign(string typeStr)
        {
            return (VerticalAlign)ToEnum(typeof(VerticalAlign), typeStr, VerticalAlign.Middle);
        }

        public static GridLines ToGridLines(string typeStr)
        {
            return (GridLines)ToEnum(typeof(GridLines), typeStr, GridLines.None);
        }

        public static RepeatDirection ToRepeatDirection(string typeStr)
        {
            return (RepeatDirection)ToEnum(typeof(RepeatDirection), typeStr, RepeatDirection.Vertical);
        }

        public static RepeatLayout ToRepeatLayout(string typeStr)
        {
            return (RepeatLayout)ToEnum(typeof(RepeatLayout), typeStr, RepeatLayout.Table);
        }

        public static object ToEnum(Type enumType, string value, object defaultType)
        {
            object retVal;
            try
            {
                retVal = Enum.Parse(enumType, value, true);
            }
            catch
            {
                retVal = defaultType;
            }
            return retVal;
        }

        public static Unit ToUnit(string unitStr)
        {
            var type = Unit.Empty;
            try
            {
                type = Unit.Parse(unitStr.Trim());
            }
            catch
            {
                // ignored
            }
            return type;
        }

        public static ListItem GetListItem(DatabaseType type, bool selected)
        {
            var item = new ListItem(type.Value, type.Value);
            if (selected)
            {
                item.Selected = true;
            }
            return item;
        }

        public static void AddListItemsToDatabaseType(ListControl listControl)
        {
            if (listControl == null) return;
            listControl.Items.Add(GetListItem(DatabaseType.MySql, false));
            listControl.Items.Add(GetListItem(DatabaseType.SqlServer, false));
            listControl.Items.Add(GetListItem(DatabaseType.PostgreSql, false));
            listControl.Items.Add(GetListItem(DatabaseType.Oracle, false));
        }

        public static string GetHost()
        {
            var host = string.Empty;
            if (HttpContext.Current == null) return string.IsNullOrEmpty(host) ? string.Empty : host.Trim().ToLower();
            host = HttpContext.Current.Request.Headers["HOST"];
            if (string.IsNullOrEmpty(host))
            {
                host = HttpContext.Current.Request.Url.Host;
            }

            return string.IsNullOrEmpty(host) ? string.Empty : host.Trim().ToLower();
        }

        public static string GetScheme()
        {
            var scheme = string.Empty;
            if (HttpContext.Current != null)
            {
                scheme = HttpContext.Current.Request.Headers["SCHEME"];
                if (string.IsNullOrEmpty(scheme))
                {
                    scheme = HttpContext.Current.Request.Url.Scheme;
                }
            }

            return string.IsNullOrEmpty(scheme) ? "http" : scheme.Trim().ToLower();
        }

        public static string ApplicationPath => HttpContext.Current != null && !string.IsNullOrEmpty(HttpContext.Current.Request.ApplicationPath) ? HttpContext.Current.Request.ApplicationPath : "/";

        // 系统根目录访问地址
        public static string GetRootUrl(string relatedUrl)
        {
            return PageUtils.Combine(ApplicationPath, relatedUrl);
        }

        public static string HttpContextRootDomain
        {
            get
            {
                var url = HttpContext.Current.Request.Url;

                if (url.HostNameType != UriHostNameType.Dns) return url.Host;

                var match = Regex.Match(url.Host, "([^.]+\\.[^.]{1,3}(\\.[^.]{1,3})?)$");
                return match.Groups[1].Success ? match.Groups[1].Value : null;
            }
        }

        public static string GetIpAddress()
        {
            var result = string.Empty;

            try
            {
                //取CDN用户真实IP的方法
                //当用户使用代理时，取到的是代理IP
                result = HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
                if (!string.IsNullOrEmpty(result))
                {
                    //可能有代理
                    if (result.IndexOf(".", StringComparison.Ordinal) == -1)
                        result = null;
                    else
                    {
                        if (result.IndexOf(",", StringComparison.Ordinal) != -1)
                        {
                            result = result.Replace("  ", "").Replace("'", "");
                            var temparyip = result.Split(",;".ToCharArray());
                            foreach (var t in temparyip)
                            {
                                if (PageUtils.IsIpAddress(t) && t.Substring(0, 3) != "10." && t.Substring(0, 7) != "192.168" && t.Substring(0, 7) != "172.16.")
                                {
                                    result = t;
                                }
                            }
                            var str = result.Split(',');
                            if (str.Length > 0)
                                result = str[0].Trim();
                        }
                        else if (PageUtils.IsIpAddress(result))
                            return result;
                    }
                }

                if (string.IsNullOrEmpty(result))
                    result = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                if (string.IsNullOrEmpty(result))
                    result = HttpContext.Current.Request.UserHostAddress;
                if (string.IsNullOrEmpty(result))
                    result = "localhost";

                if (result == "::1" || result == "127.0.0.1")
                {
                    result = "localhost";
                }
            }
            catch
            {
                // ignored
            }

            return result;
        }

        public static string SessionId
        {
            get
            {
                var sessionId = CookieUtils.GetCookie("SiteServer.SessionID");
                if (!string.IsNullOrEmpty(sessionId)) return sessionId;
                long i = 1;
                foreach (var b in Guid.NewGuid().ToByteArray())
                {
                    i *= b + 1;
                }
                sessionId = $"{i - DateTime.Now.Ticks:x}";
                CookieUtils.SetCookie("SiteServer.SessionID", sessionId, TimeSpan.FromDays(100));
                return sessionId;
            }
        }

        public static string GetRefererUrl()
        {
            var url = HttpContext.Current.Request.ServerVariables["HTTP_REFERER"];
            return url;
        }

        public static string GetReturnUrl()
        {
            return GetReturnUrl(true);
        }

        public static string GetReturnUrl(bool toReferer)
        {
            var redirectUrl = string.Empty;
            if (!string.IsNullOrEmpty(HttpContext.Current.Request.QueryString["ReturnUrl"]))
            {
                redirectUrl = ParseNavigationUrl(HttpContext.Current.Request.QueryString["ReturnUrl"]);
            }
            else if (toReferer)
            {
                var referer = GetRefererUrl();
                redirectUrl = !string.IsNullOrEmpty(referer) ? referer : GetHost();
            }
            return redirectUrl;
        }

        public static string GetAdminUrl(string relatedUrl)
        {
            return PageUtils.Combine(ApplicationPath, WebConfigUtils.AdminDirectory, relatedUrl);
        }

        public static string GetHomeUrl(string relatedUrl)
        {
            return PageUtils.Combine(ApplicationPath, WebConfigUtils.HomeDirectory, relatedUrl);
        }

        public static string GetSiteFilesUrl(string relatedUrl)
        {
            return PageUtils.Combine(ApplicationPath, DirectoryUtils.SiteFiles.DirectoryName, relatedUrl);
        }

        public static string GetTemporaryFilesUrl(string relatedUrl)
        {
            return PageUtils.Combine(ApplicationPath, DirectoryUtils.SiteFiles.DirectoryName, DirectoryUtils.SiteFiles.TemporaryFiles, relatedUrl);
        }

        public static string GetSiteTemplatesUrl(string relatedUrl)
        {
            return PageUtils.Combine(ApplicationPath, DirectoryUtils.SiteFiles.DirectoryName, DirectoryUtils.SiteTemplates.DirectoryName, relatedUrl);
        }

        public static string GetSiteTemplateMetadataUrl(string siteTemplateUrl, string relatedUrl)
        {
            return PageUtils.Combine(siteTemplateUrl, DirectoryUtils.SiteTemplates.SiteTemplateMetadata, relatedUrl);
        }

        public static string ParsePluginUrl(string pluginId, string url)
        {
            if (string.IsNullOrEmpty(url)) return string.Empty;

            if (PageUtils.IsProtocolUrl(url)) return url;

            if (StringUtils.StartsWith(url, "~/"))
            {
                return GetRootUrl(url.Substring(1));
            }

            if (StringUtils.StartsWith(url, "@/"))
            {
                return GetAdminUrl(url.Substring(1));
            }

            return GetSiteFilesUrl(PageUtils.Combine(DirectoryUtils.SiteFiles.Plugins, pluginId, url));
        }

        public static string GetSiteServerUrl(string className, NameValueCollection queryString)
        {
            return PageUtils.AddQueryString(GetAdminUrl(className.ToCamelCase() + ".aspx"), queryString);
        }

        public static string GetSettingsUrl(string className, NameValueCollection queryString)
        {
            return PageUtils.AddQueryString(GetAdminUrl(PageUtils.Combine("Settings", className.ToCamelCase() + ".aspx")), queryString);
        }

        public static string GetCmsUrl(int siteId, string className, NameValueCollection queryString)
        {
            queryString = queryString ?? new NameValueCollection();
            queryString.Remove("siteId");
            return PageUtils.AddQueryString(GetAdminUrl($"Cms/{className.ToCamelCase()}.aspx?siteId={siteId}"), queryString);
        }

        public static string GetCmsWebHandlerUrl(int siteId, string className, NameValueCollection queryString)
        {
            queryString = queryString ?? new NameValueCollection();
            queryString.Remove("siteId");
            return PageUtils.AddQueryString(GetAdminUrl($"Cms/{className.ToCamelCase()}.ashx?siteId={siteId}"), queryString);
        }

        public static string GetAjaxUrl(string className, NameValueCollection queryString)
        {
            return PageUtils.AddQueryString(GetAdminUrl(PageUtils.Combine("ajax", className.ToLower() + ".aspx")), queryString);
        }

        public static string GetRootUrlByPhysicalPath(string physicalPath)
        {
            var requestPath = PathUtils.GetPathDifference(WebConfigUtils.PhysicalApplicationPath, physicalPath);
            requestPath = requestPath.Replace(PathUtils.SeparatorChar, PageUtils.SeparatorChar);
            return GetRootUrl(requestPath);
        }

        public static string ParseNavigationUrl(string url)
        {
            if (string.IsNullOrEmpty(url)) return string.Empty;

            url = url.StartsWith("~") ? PageUtils.Combine(ApplicationPath, url.Substring(1)) : url;
            url = url.Replace(PathUtils.SeparatorChar, PageUtils.SeparatorChar);
            return url;
        }

        /// <summary>
        /// 按照给定的host，添加Protocol
        /// Demo: 发送的邮件中，需要内容标题的链接为全连接，那么需要指定他的host
        /// </summary>
        /// <param name="url"></param>
        /// <param name="host"></param>
        /// <returns></returns>
        public static string AddProtocolToUrl(string url, string host)
        {
            if (url == PageUtils.UnClickedUrl)
            {
                return url;
            }
            var retval = string.Empty;

            if (!string.IsNullOrEmpty(url))
            {
                url = url.Trim();
                if (PageUtils.IsProtocolUrl(url))
                {
                    retval = url;
                }
                else
                {
                    if (string.IsNullOrEmpty(host))
                    {
                        retval = url.StartsWith("/") ? GetScheme() + "://" + GetHost() + url : GetScheme() + "://" + url;
                    }
                    else
                    {
                        retval = url.StartsWith("/") ? host.TrimEnd('/') + url : host + url;
                    }
                }
            }
            return retval;
        }

        public static string AddProtocolToUrl(string url)
        {
            return AddProtocolToUrl(url, string.Empty);
        }

        public static string GetUrlWithReturnUrl(string pageUrl, string returnUrl)
        {
            var retval = pageUrl;
            returnUrl = $"ReturnUrl={returnUrl}";
            if (pageUrl.IndexOf("?", StringComparison.Ordinal) != -1)
            {
                if (pageUrl.EndsWith("&"))
                {
                    retval += returnUrl;
                }
                else
                {
                    retval += "&" + returnUrl;
                }
            }
            else
            {
                retval += "?" + returnUrl;
            }
            return ParseNavigationUrl(retval);
        }



        public static string GetUrlByBaseUrl(string rawUrl, string baseUrl)
        {
            var url = string.Empty;
            if (!string.IsNullOrEmpty(rawUrl))
            {
                rawUrl = rawUrl.Trim().TrimEnd('#');
            }
            if (!string.IsNullOrEmpty(baseUrl))
            {
                baseUrl = baseUrl.Trim();
            }
            if (!string.IsNullOrEmpty(rawUrl))
            {
                rawUrl = rawUrl.Trim();
                if (PageUtils.IsProtocolUrl(rawUrl))
                {
                    url = rawUrl;
                }
                else if (rawUrl.StartsWith("/"))
                {
                    var domain = PageUtils.GetUrlWithoutPathInfo(baseUrl);
                    url = domain + rawUrl;
                }
                else if (rawUrl.StartsWith("../"))
                {
                    var count = StringUtils.GetStartCount("../", rawUrl);
                    rawUrl = rawUrl.Remove(0, 3 * count);
                    baseUrl = PageUtils.GetUrlWithoutFileName(baseUrl).TrimEnd('/');
                    baseUrl = PageUtils.RemoveProtocolFromUrl(baseUrl);
                    for (var i = 0; i < count; i++)
                    {
                        var j = baseUrl.LastIndexOf('/');
                        if (j != -1)
                        {
                            baseUrl = StringUtils.Remove(baseUrl, j);
                        }
                        else
                        {
                            break;
                        }
                    }
                    url = PageUtils.Combine(AddProtocolToUrl(baseUrl), rawUrl);
                }
                else
                {
                    if (baseUrl != null && baseUrl.EndsWith("/"))
                    {
                        url = baseUrl + rawUrl;
                    }
                    else
                    {
                        var urlWithoutFileName = PageUtils.GetUrlWithoutFileName(baseUrl);
                        if (!urlWithoutFileName.EndsWith("/"))
                        {
                            urlWithoutFileName += "/";
                        }
                        url = urlWithoutFileName + rawUrl;
                    }
                }
            }
            return url;
        }

        public static string ParseConfigRootUrl(string url)
        {
            return ParseNavigationUrl(url);
        }

        public static string GetMainUrl(int siteId, string pageUrl)
        {
            var queryString = new NameValueCollection();
            if (siteId > 0)
            {
                queryString.Add("siteId", siteId.ToString());
            }
            if (!string.IsNullOrEmpty(pageUrl))
            {
                queryString.Add("pageUrl", PageUtils.UrlEncode(pageUrl));
            }
            return PageUtils.AddQueryString(AdminPagesUtils.MainUrl, queryString);
        }

        public static string GetLoadingUrl(string url)
        {
            return $"{AdminPagesUtils.LoadingUrl}?encryptedUrl={TranslateUtils.EncryptStringBySecretKey(url)}";
        }

        public static string GetLoadingUrl(int siteId, int channelId, int contentId)
        {
            return $"{AdminPagesUtils.LoadingUrl}?siteId={siteId}&channelId={channelId}&contentId={contentId}";
        }

        public static string GetMenusPath(params string[] paths)
        {
            return PageUtils.Combine(SiteServerAssets.GetPath("menus"), PageUtils.Combine(paths));
        }

        public static ListItem GetListItem(EBoolean type, bool selected)
        {
            var item = new ListItem(EBooleanUtils.GetText(type), EBooleanUtils.GetValue(type));
            if (selected)
            {
                item.Selected = true;
            }
            return item;
        }

        public static void AddListItems(ListControl listControl)
        {
            if (listControl != null)
            {
                listControl.Items.Add(GetListItem(EBoolean.True, false));
                listControl.Items.Add(GetListItem(EBoolean.False, false));
            }
        }

        public static void AddListItems(ListControl listControl, string trueText, string falseText)
        {
            if (listControl != null)
            {
                var item = new ListItem(trueText, EBooleanUtils.GetValue(EBoolean.True));
                listControl.Items.Add(item);
                item = new ListItem(falseText, EBooleanUtils.GetValue(EBoolean.False));
                listControl.Items.Add(item);
            }
        }

        public static ListItem GetListItem(ECharset type, bool selected)
        {
            var item = new ListItem(ECharsetUtils.GetText(type), ECharsetUtils.GetValue(type));
            if (selected)
            {
                item.Selected = true;
            }
            return item;
        }


        public static void AddListItemsToECharset(ListControl listControl)
        {
            if (listControl != null)
            {
                listControl.Items.Add(GetListItem(ECharset.utf_8, false));
                listControl.Items.Add(GetListItem(ECharset.gb2312, false));
                listControl.Items.Add(GetListItem(ECharset.big5, false));
                listControl.Items.Add(GetListItem(ECharset.iso_8859_1, false));
                listControl.Items.Add(GetListItem(ECharset.euc_kr, false));
                listControl.Items.Add(GetListItem(ECharset.euc_jp, false));
                listControl.Items.Add(GetListItem(ECharset.iso_8859_6, false));
                listControl.Items.Add(GetListItem(ECharset.windows_874, false));
                listControl.Items.Add(GetListItem(ECharset.iso_8859_9, false));
                listControl.Items.Add(GetListItem(ECharset.iso_8859_5, false));
                listControl.Items.Add(GetListItem(ECharset.iso_8859_8, false));
                listControl.Items.Add(GetListItem(ECharset.iso_8859_7, false));
                listControl.Items.Add(GetListItem(ECharset.windows_1258, false));
                listControl.Items.Add(GetListItem(ECharset.iso_8859_2, false));
            }
        }

        public static ListItem GetListItem(EDataFormat type, bool selected)
        {
            var item = new ListItem(EDataFormatUtils.GetText(type), EDataFormatUtils.GetValue(type));
            if (selected)
            {
                item.Selected = true;
            }
            return item;
        }

        public static void AddListItemsToEDataFormat(ListControl listControl)
        {
            if (listControl != null)
            {
                listControl.Items.Add(GetListItem(EDataFormat.String, false));
                listControl.Items.Add(GetListItem(EDataFormat.Json, false));
                listControl.Items.Add(GetListItem(EDataFormat.Xml, false));
            }
        }

        public static ListItem GetListItem(EDateFormatType type, bool selected)
        {
            var item = new ListItem(EDateFormatTypeUtils.GetText(type), EDateFormatTypeUtils.GetValue(type));
            if (selected)
            {
                item.Selected = true;
            }
            return item;
        }

        public static void AddListItemsToEDateFormatType(ListControl listControl)
        {
            if (listControl != null)
            {
                listControl.Items.Add(GetListItem(EDateFormatType.Month, false));
                listControl.Items.Add(GetListItem(EDateFormatType.Day, false));
                listControl.Items.Add(GetListItem(EDateFormatType.Year, false));
                listControl.Items.Add(GetListItem(EDateFormatType.Chinese, false));
            }
        }

        public static ListItem GetListItem(EFileSystemType type, bool selected)
        {
            var item = new ListItem(EFileSystemTypeUtils.GetValue(type), EFileSystemTypeUtils.GetValue(type));
            if (selected)
            {
                item.Selected = true;
            }
            return item;
        }

        public static void AddListItemsToEFileSystemType(ListControl listControl)
        {
            if (listControl != null)
            {
                listControl.Items.Add(GetListItem(EFileSystemType.Html, false));
                listControl.Items.Add(GetListItem(EFileSystemType.Htm, false));
                listControl.Items.Add(GetListItem(EFileSystemType.SHtml, false));
                listControl.Items.Add(GetListItem(EFileSystemType.Xml, false));
                listControl.Items.Add(GetListItem(EFileSystemType.Json, false));
            }
        }

        public static ListItem GetListItem(EPredefinedRole type, bool selected)
        {
            var item = new ListItem(EPredefinedRoleUtils.GetText(type), EPredefinedRoleUtils.GetValue(type));
            if (selected)
            {
                item.Selected = true;
            }
            return item;
        }

        public static ListItem GetListItem(EScopeType type, bool selected)
        {
            var item = new ListItem(EScopeTypeUtils.GetValue(type) + " (" + EScopeTypeUtils.GetText(type) + ")", EScopeTypeUtils.GetValue(type));
            if (selected)
            {
                item.Selected = true;
            }
            return item;
        }

        public static void AddListItemsToEScopeType(ListControl listControl)
        {
            if (listControl != null)
            {
                listControl.Items.Add(GetListItem(EScopeType.Self, false));
                listControl.Items.Add(GetListItem(EScopeType.Children, false));
                listControl.Items.Add(GetListItem(EScopeType.SelfAndChildren, false));
                listControl.Items.Add(GetListItem(EScopeType.Descendant, false));
                listControl.Items.Add(GetListItem(EScopeType.All, false));
            }
        }

        public static ListItem GetListItem(EStatictisXType type, bool selected)
        {
            var item = new ListItem(EStatictisXTypeUtils.GetText(type), EStatictisXTypeUtils.GetValue(type));
            if (selected)
            {
                item.Selected = true;
            }
            return item;
        }

        public static void AddListItemsToEStatictisXType(ListControl listControl)
        {
            if (listControl != null)
            {
                listControl.Items.Add(GetListItem(EStatictisXType.Day, false));
                listControl.Items.Add(GetListItem(EStatictisXType.Month, false));
                listControl.Items.Add(GetListItem(EStatictisXType.Year, false));
            }
        }

        public static void AddListItemsToETriState(ListControl listControl, string allText, string trueText, string falseText)
        {
            if (listControl != null)
            {
                var item = new ListItem(allText, ETriStateUtils.GetValue(ETriState.All));
                listControl.Items.Add(item);
                item = new ListItem(trueText, ETriStateUtils.GetValue(ETriState.True));
                listControl.Items.Add(item);
                item = new ListItem(falseText, ETriStateUtils.GetValue(ETriState.False));
                listControl.Items.Add(item);
            }
        }

        public static ListItem GetListItem(EUserPasswordRestriction type, bool selected)
        {
            var item = new ListItem(EUserPasswordRestrictionUtils.GetText(type), EUserPasswordRestrictionUtils.GetValue(type));
            if (selected)
            {
                item.Selected = true;
            }
            return item;
        }

        public static void AddListItemsToEUserPasswordRestriction(ListControl listControl)
        {
            if (listControl != null)
            {
                listControl.Items.Add(GetListItem(EUserPasswordRestriction.None, false));
                listControl.Items.Add(GetListItem(EUserPasswordRestriction.LetterAndDigit, false));
                listControl.Items.Add(GetListItem(EUserPasswordRestriction.LetterAndDigitAndSymbol, false));
            }
        }

        public static string GetCurrentPagePath()
        {
            if (HttpContext.Current != null)
            {
                return HttpContext.Current.Request.PhysicalPath;
            }
            return string.Empty;
        }

        public static string MapPath(string virtualPath)
        {
            virtualPath = PathUtils.RemovePathInvalidChar(virtualPath);
            string retVal;
            if (!string.IsNullOrEmpty(virtualPath))
            {
                if (virtualPath.StartsWith("~"))
                {
                    virtualPath = virtualPath.Substring(1);
                }
                virtualPath = PageUtils.Combine("~", virtualPath);
            }
            else
            {
                virtualPath = "~/";
            }
            if (HttpContext.Current != null)
            {
                retVal = HttpContext.Current.Server.MapPath(virtualPath);
            }
            else
            {
                var rootPath = WebConfigUtils.PhysicalApplicationPath;

                virtualPath = !string.IsNullOrEmpty(virtualPath) ? virtualPath.Substring(2) : string.Empty;
                retVal = PathUtils.Combine(rootPath, virtualPath);
            }

            if (retVal == null) retVal = string.Empty;
            return retVal.Replace("/", "\\");
        }

        public static string GetSiteFilesPath(params string[] paths)
        {
            return MapPath(PathUtils.Combine("~/" + DirectoryUtils.SiteFiles.DirectoryName, PathUtils.Combine(paths)));
        }

        public static string PluginsPath => GetSiteFilesPath(DirectoryUtils.SiteFiles.Plugins);

        public static string GetPluginPath(string pluginId, params string[] paths)
        {
            return GetSiteFilesPath(DirectoryUtils.SiteFiles.Plugins, pluginId, PathUtils.Combine(paths));
        }

        public static string GetPluginNuspecPath(string pluginId)
        {
            return GetPluginPath(pluginId, pluginId + ".nuspec");
        }

        public static string GetPluginDllDirectoryPath(string pluginId)
        {
            var fileName = pluginId + ".dll";

            if (FileUtils.IsFileExists(GetPluginPath(pluginId, "Bin", fileName)))
            {
                return GetPluginPath(pluginId, "Bin");
            }
            if (FileUtils.IsFileExists(GetPluginPath(pluginId, "Bin", "Debug", fileName)))
            {
                return GetPluginPath(pluginId, "Bin", "Debug");
            }
            if (FileUtils.IsFileExists(GetPluginPath(pluginId, "Bin", "Release", fileName)))
            {
                return GetPluginPath(pluginId, "Bin", "Release");
            }

            return string.Empty;
        }

        public static string GetPackagesPath(params string[] paths)
        {
            return GetSiteFilesPath(DirectoryUtils.SiteFiles.Packages, PathUtils.Combine(paths));
        }

        public static void GetValidateAttributesForListItem(ListControl control, bool isValidate, string displayName, bool isRequire, int minNum, int maxNum, string validateType, string regExp, string errorMessage)
        {
            if (!isValidate) return;

            control.Attributes.Add("isValidate", true.ToString().ToLower());
            control.Attributes.Add("displayName", displayName);
            control.Attributes.Add("isRequire", isRequire.ToString().ToLower());
            control.Attributes.Add("minNum", minNum.ToString());
            control.Attributes.Add("maxNum", maxNum.ToString());
            control.Attributes.Add("validateType", validateType);
            control.Attributes.Add("regExp", regExp);
            control.Attributes.Add("errorMessage", errorMessage);
            control.Attributes.Add("isListItem", true.ToString().ToLower());
        }

        public static ListItem GetListItem(ValidateType type, bool selected)
        {
            var item = new ListItem(ValidateTypeUtils.GetText(type), type.Value);
            if (selected)
            {
                item.Selected = true;
            }
            return item;
        }

        public static void AddListItemsToValidateType(ListControl listControl)
        {
            if (listControl != null)
            {
                listControl.Items.Add(GetListItem(ValidateType.None, false));
                listControl.Items.Add(GetListItem(ValidateType.Chinese, false));
                listControl.Items.Add(GetListItem(ValidateType.English, false));
                listControl.Items.Add(GetListItem(ValidateType.Email, false));
                listControl.Items.Add(GetListItem(ValidateType.Url, false));
                listControl.Items.Add(GetListItem(ValidateType.Phone, false));
                listControl.Items.Add(GetListItem(ValidateType.Mobile, false));
                listControl.Items.Add(GetListItem(ValidateType.Integer, false));
                listControl.Items.Add(GetListItem(ValidateType.Currency, false));
                listControl.Items.Add(GetListItem(ValidateType.Zip, false));
                listControl.Items.Add(GetListItem(ValidateType.IdCard, false));
                listControl.Items.Add(GetListItem(ValidateType.RegExp, false));
            }
        }
    }
}
