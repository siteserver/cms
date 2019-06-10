namespace SS.CMS.Utils
{
    public static class Constants
    {
        public const string CookieAdminToken = "SS-ADMIN-TOKEN";
        public const string CookieUserToken = "SS-USER-TOKEN";
        public const string CookieApiKey = "SS-API-KEY";

        public const string HeaderUserToken = "X-SS-USER-TOKEN";
        public const string HeaderAdminToken = "X-SS-ADMIN-TOKEN";
        public const string HeaderApiKey = "X-SS-API-KEY";

        public const string QueryUserToken = "userToken";
        public const string QueryAdminToken = "adminToken";
        public const string QueryApiKey = "apiKey";

        public const int AccessTokenExpireDays = 7;

        public const string ReturnAndNewline = "\r\n";//回车换行
        public const string Html5Empty = @"<html><head><meta charset=""utf-8""></head><body></body></html>";

        public const string OracleEmptyValue = "_EMPTY_";

        public const string Ellipsis = "...";

        public const int PageSize = 25;//后台分页数
        public const string HideElementStyle = "display:none";
        public const string ShowElementStyle = "display:";

        public const string TitleImageAppendix = "t_";
        public const string SmallImageAppendix = "s_";

        public static string GetContentTableName(int siteId)
        {
            return $"siteserver_Content_{siteId}";
        }
    }
}
