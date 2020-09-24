using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using SSCMS.Configuration;
using SSCMS.Models;
using SSCMS.Repositories;

namespace SSCMS.Web.Controllers.Admin
{
    [OpenApiIgnore]
    [Route(Constants.ApiAdminPrefix)]
    public partial class ErrorController : ControllerBase
    {
        public const string Route = "error";

        private readonly IErrorLogRepository _errorLogRepository;

        public ErrorController(IErrorLogRepository errorLogRepository)
        {
            _errorLogRepository = errorLogRepository;
        }

        public class GetResult
        {
            public ErrorLog Error { get; set; }
        }
    }
}
