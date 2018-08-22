namespace SiteServer.Cli.Updater.Model36
{
    public static class TableErrorLog
    {
        public const string OldTableName = "ErrorLog";

        public static ConvertInfo Converter => new ConvertInfo
        {
            IsAbandon = true
        };
    }
}
