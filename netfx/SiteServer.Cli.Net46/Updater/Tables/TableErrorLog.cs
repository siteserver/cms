namespace SiteServer.Cli.Updater.Tables
{
    public static class TableErrorLog
    {
        public const string OldTableName = "bairong_ErrorLog";

        public static ConvertInfo Converter => new ConvertInfo
        {
            IsAbandon = true
        };
    }
}
