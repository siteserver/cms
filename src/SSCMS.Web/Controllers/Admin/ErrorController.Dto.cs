using SSCMS.Abstractions;

namespace SSCMS.Controllers.Admin
{
    public partial class ErrorController
    {
        public class GetResult
        {
            public ErrorLog Error { get; set; }
        }
    }
}
