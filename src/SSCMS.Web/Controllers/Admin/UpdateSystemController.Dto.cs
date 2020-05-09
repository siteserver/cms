namespace SSCMS.Web.Controllers.Admin
{
    public partial class UpdateSystemController
    {
        public class GetResult
        {
            public bool IsNightly { get; set; }
            public string Version { get; set; }
        }

        public class UpdateRequest
        {
            public string Version { get; set; }
        }
    }
}
