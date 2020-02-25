namespace SS.CMS.Web.Controllers.Admin
{
    public partial class SysPackagingUpdateSsCmsController
    {
        public class SubmitRequest
        {
            public string Version { get; set; }
        }

        public class SubmitResult
        {
            public bool IsCopyFiles { get; set; }
        }
    }
}
