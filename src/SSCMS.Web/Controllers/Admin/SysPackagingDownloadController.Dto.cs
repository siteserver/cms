namespace SSCMS.Controllers.Admin
{
    public partial class SysPackagingDownloadController
    {
        public class SubmitRequest
        {
            public string PackageId { get; set; }
            public string Version { get; set; }
        }
    }
}
