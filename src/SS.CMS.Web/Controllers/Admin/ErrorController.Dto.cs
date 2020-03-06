using SS.CMS.Abstractions;

namespace SS.CMS.Web.Controllers.Admin
{
    public partial class ErrorController
    {
        public class GetResult
        {
            public ErrorLog Error { get; set; }
        }
    }
}
