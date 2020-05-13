namespace SiteServer.Utils
{
    public static class Constants
    {
        public const string AuthKeyUserHeader = "X-SS-USER-TOKEN";
        public const string AuthKeyUserCookie = "SS-USER-TOKEN";
        public const string AuthKeyUserQuery = "userToken";
        public const string AuthKeyAdminHeader = "X-SS-ADMIN-TOKEN";
        public const string AuthKeyAdminCookie = "SS-ADMIN-TOKEN";
        public const string AuthKeyAdminQuery = "adminToken";
        public const string AuthKeyApiHeader = "X-SS-API-KEY";
        public const string AuthKeyApiCookie = "SS-API-KEY";
        public const string AuthKeyApiQuery = "apiKey";
        public const int AccessTokenExpireDays = 7;

        public const string ReturnAndNewline = "\r\n";//回车换行
        public const string Html5Empty = @"<html><head><meta charset=""utf-8""></head><body></body></html>";

        public const string Ellipsis = "...";

        public const int PageSize = 25;//后台分页数
        public const string HideElementStyle = "display:none";
        public const string ShowElementStyle = "display:";

        public const string TitleImageAppendix = "t_";
        public const string SmallImageAppendix = "s_";
    }
}
