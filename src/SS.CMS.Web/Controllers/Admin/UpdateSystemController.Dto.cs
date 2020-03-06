namespace SS.CMS.Web.Controllers.Admin
{
    public partial class UpdateSystemController
    {
        public class GetResult
        {
            public bool Value { get; set; }
            public string PackageId { get; set; }
            public string InstalledVersion { get; set; }
            public bool IsNightly { get; set; }
            public string Version { get; set; }
        }

        public class UpdateRequest
        {
            public string Version { get; set; }
        }
    }
}
