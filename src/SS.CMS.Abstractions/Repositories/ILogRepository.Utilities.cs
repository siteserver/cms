namespace SS.CMS.Repositories
{
    public partial interface ILogRepository
    {
        void AddAdminLog(string ipAddress, string adminName, string action, string summary = "");
    }
}
