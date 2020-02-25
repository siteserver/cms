using System.Threading.Tasks;
using Datory;
using Microsoft.AspNetCore.Mvc;
using SS.CMS.Abstractions;
using SS.CMS.Abstractions.Dto.Result;
using SS.CMS.Packaging;
using SS.CMS.Web.Extensions;

namespace SS.CMS.Web.Controllers.Admin
{
    [Route("sys/admin/packaging/update")]
    public partial class SysPackagingUpdateController : ControllerBase
    {
        private const string Route = "";
        private readonly IAuthManager _authManager;

        public SysPackagingUpdateController(IAuthManager authManager)
        {
            _authManager = authManager;
        }

        [HttpPost, Route(Route)]
        public async Task<ActionResult<BoolResult>> Submit([FromBody]SubmitRequest request)
        {
            var auth = await _authManager.GetAdminAsync();

            if (!auth.IsAdminLoggin)
            {
                return Unauthorized();
            }

            if (StringUtils.EqualsIgnoreCase(request.PackageId, PackageUtils.PackageIdSsCms))
            {
                request.PackageType = PackageType.SsCms.GetValue();
            }

            var idWithVersion = $"{request.PackageId}.{request.Version}";
            if (!PackageUtils.UpdatePackage(idWithVersion, TranslateUtils.ToEnum(request.PackageType, PackageType.Library), out var errorMessage))
            {
                return this.Error(errorMessage);
            }

            return new BoolResult
            {
                Value = true
            };
        }
    }
}
