using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SS.CMS.Abstractions;
using SS.CMS.Abstractions.Dto.Result;
using SS.CMS.Framework;
using SS.CMS.Packaging;

namespace SS.CMS.Web.Controllers.Admin
{
    [Route("sys/admin/packaging/download")]
    public partial class SysPackagingDownloadController : ControllerBase
    {
        private const string Route = "";
        private readonly IAuthManager _authManager;

        public SysPackagingDownloadController(IAuthManager authManager)
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

            try
            {
                PackageUtils.DownloadPackage(request.PackageId, request.Version);
            }
            catch
            {
                PackageUtils.DownloadPackage(request.PackageId, request.Version);
            }

            if (StringUtils.EqualsIgnoreCase(request.PackageId, PackageUtils.PackageIdSsCms))
            {
                await DataProvider.DbCacheRepository.RemoveAndInsertAsync(PackageUtils.CacheKeySsCmsIsDownload, true.ToString());
            }

            return new BoolResult
            {
                Value = true
            };
        }
    }
}
