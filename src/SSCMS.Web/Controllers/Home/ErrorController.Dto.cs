using SSCMS.Models;

namespace SSCMS.Web.Controllers.Home
{
    public partial class ErrorController
    {
        public class GetResult
        {
            public ErrorLog Error { get; set; }
        }
    }
}
