namespace SSCMS.Web.Controllers.Admin.Plugins
{
    public partial class AddController
    {
        public class GetResult
        {
            public bool IsNightly { get; set; }
            public string PluginVersion { get; set; }
            public string PackageIds { get; set; }
        }
    }
}
