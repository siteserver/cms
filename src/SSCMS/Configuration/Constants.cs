using System;

namespace SSCMS.Configuration
{
    public static class Constants
    {
        public const string EnvironmentPrefix = "SSCMS_";
        public const string ConfigFileName = "sscms.json";
        public const string PackageFileName = "package.json";
        public const string ReadmeFileName = "README.md";
        public const string ChangeLogFileName = "CHANGELOG.md";
        public const string PluginConfigFileName = "config.json";
        public const string AdminDirectory = "ss-admin";
        public const string HomeDirectory = "home";
        public const string WwwrootDirectory = "wwwroot";
        public const string PluginsDirectory = "plugins";
        public const string DefaultLanguage = "en";
        public const string EncryptStingIndicator = "0secret0";

        public const string LocalDbHostVirtualPath = "~/database.sqlite";
        public const string LocalDbContainerVirtualPath = "~/wwwroot/sitefiles/database.sqlite";

        public const int AccessTokenExpireDays = 7;

        public const string PagePlaceHolder = "[SITESERVER_PAGE]";//内容翻页占位符

        public const string ApiPrefix = "/api";
        public const string ApiAdminPrefix = "/api/admin";
        public const string ApiHomePrefix = "/api/home";
        public const string ApiWxPrefix = "/api/wx";
        public const string ApiV1Prefix = "/api/v1";
        public const string ApiStlPrefix = "/stl";
        public const string RoutePreview = "preview/{siteId}";
        public const string RoutePreviewChannel = "preview/{siteId}/{channelId}";
        public const string RoutePreviewContent = "preview/{siteId}/{channelId}/{contentId}";
        public const string RoutePreviewFile = "preview/{siteId}/file/{fileTemplateId}";
        public const string RoutePreviewSpecial = "preview/{siteId}/special/{specialId}";

        public const string RouteStlActionsDownload = "actions/download";
        public const string RouteStlActionsDynamic = "actions/dynamic";
        public const string RouteStlRouteActionsIf = "actions/if";
        public const string RouteStlActionsPageContents = "actions/pagecontents";
        public const string RouteStlActionsSearch = "actions/search";
        public const string RouteStlActionsTrigger = "actions/trigger";

        public const string ScopeChannels = "Channels";
        public const string ScopeContents = "Contents";
        public const string ScopeAdministrators = "Administrators";
        public const string ScopeUsers = "Users";
        public const string ScopeStl = "STL";

        public static DateTime SqlMinValue { get; } = new DateTime(1754, 1, 1, 0, 0, 0, 0);

        public static string GetSessionIdCacheKey(int userId)
        {
            return $"SESSION-ID-{userId}";
        }

        public const char Newline = '\n';//换行
        public const string ReturnAndNewline = "\r\n";//回车换行
        public const string Html5Empty = @"<html>
  <head>
    <meta charset=""utf-8"" />
    <meta http-equiv=""X-UA-Compatible"" content=""IE=edge,chrome=1"">
    <meta name=""renderer"" content=""webkit"" />
    <meta name=""viewport"" content=""width=device-width, initial-scale=1, shrink-to-fit=no"">
    <meta http-equiv=""cache-control"" content=""max-age=0"" />
    <meta http-equiv=""cache-control"" content=""no-cache"" />
    <meta http-equiv=""expires"" content=""0"" />
    <meta http-equiv=""expires"" content=""Tue, 01 Jan 1980 1:00:00 GMT"" />
    <meta http-equiv=""pragma"" content=""no-cache"" />
  </head>
  <body></body>
</html>
";

        public const string Ellipsis = "...";

        public const int PageSize = 25;//后台分页数
        public const string SmallImageAppendix = "s_";


        public const string ActionsLoginSuccess = "登录成功";
        public const string ActionsLoginFailure = "登录失败";

        public const string ErrorUpload = "请选择有效的文件上传!";
        public const string ErrorImageExtensionAllowed = "此图片格式已被禁止上传，请转换格式后上传!";
        public const string ErrorImageSizeAllowed = "此图片大小已超过限制，请压缩后上传!";
        public const string ErrorVideoExtensionAllowed = "此视频格式已被禁止上传，请转换格式后上传!";
        public const string ErrorVideoSizeAllowed = "此视频大小已超过限制，请压缩后上传!";
        public const string ErrorAudioExtensionAllowed = "此音频格式已被禁止上传，请转换格式后上传!";
        public const string ErrorAudioSizeAllowed = "此音频大小已超过限制，请压缩后上传!";
        public const string ErrorFileExtensionAllowed = "此文件格式已被禁止上传，请转换格式后上传!";
        public const string ErrorFileSizeAllowed = "此文件大小已超过限制，请压缩后上传!";
    }
}
