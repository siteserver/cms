using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SS.CMS.Abstractions;
using SS.CMS.Abstractions.Dto.Result;
using SS.CMS.Packaging;

namespace SS.CMS.Web.Controllers.Admin
{
    [Route("sys/admin/packaging/download")]
    public partial class SysPackagingDownloadController : ControllerBase
    {
        private const string Route = "";

        private readonly IAuthManager _authManager;
        private readonly IPathManager _pathManager;
        private readonly IDbCacheRepository _dbCacheRepository;

        public SysPackagingDownloadController(IAuthManager authManager, IPathManager pathManager, IDbCacheRepository dbCacheRepository)
        {
            _authManager = authManager;
            _pathManager = pathManager;
            _dbCacheRepository = dbCacheRepository;
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
                PackageUtils.DownloadPackage(_pathManager, request.PackageId, request.Version);
            }
            catch
            {
                PackageUtils.DownloadPackage(_pathManager, request.PackageId, request.Version);
            }

            if (StringUtils.EqualsIgnoreCase(request.PackageId, PackageUtils.PackageIdSsCms))
            {
                await _dbCacheRepository.RemoveAndInsertAsync(PackageUtils.CacheKeySsCmsIsDownload, true.ToString());
            }

            return new BoolResult
            {
                Value = true
            };
        }
    }
}
