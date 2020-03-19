namespace SSCMS.Web.Controllers.Admin
{
    public partial class SysPackagingUpdateController
    {
        public class SubmitRequest
        {
            public string PackageId { get; set; }
            public string Version { get; set; }
            public string PackageType { get; set; }
        }
    }
}
