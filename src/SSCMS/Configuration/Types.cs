namespace SSCMS.Configuration
{
    public static class Types
    {
        public static class Claims
        {
            public const string UserId = System.Security.Claims.ClaimTypes.NameIdentifier;
            public const string UserName = System.Security.Claims.ClaimTypes.Name;
            public const string Role = System.Security.Claims.ClaimTypes.Role;
            public const string IsPersistent = System.Security.Claims.ClaimTypes.IsPersistent;
        }

        public static class Roles
        {
            public const string Administrator = nameof(Administrator);
            public const string User = nameof(User);
        }

        public static class Resources
        {
            public const string App = "app";
            public const string Channel = "channel";
            public const string Content = "content";
        }

        public static class SiteTypes
        {
            public const string Web = "web";
            public const string Wx = "wx";
            public const string Document = "document";
        }

        public static class TableTypes
        {
            public const string Content = "content";
            public const string Custom = "custom";
        }
    }
}
