using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS;
using SSCMS.Dto.Result;
using SSCMS.Core.Packaging;
using SSCMS.Utils;

namespace SSCMS.Web.Controllers.Admin
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
            

            if (!await _authManager.IsAdminAuthenticatedAsync())
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
