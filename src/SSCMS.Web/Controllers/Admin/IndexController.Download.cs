using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Core.Packaging;
using SSCMS.Dto;

namespace SSCMS.Web.Controllers.Admin
{
    public partial class IndexController
    {
        [Authorize(Roles = AuthTypes.Roles.Administrator)]
        [HttpPost, Route(RouteActionsDownload)]
        public ActionResult<BoolResult> Download([FromBody] DownloadRequest request)
        {
            CloudUtils.DownloadCms(_pathManager, request.Version);

            var isDownload = CloudUtils.IsCmsDownload(_pathManager, request.Version);

            return new BoolResult
            {
                Value = isDownload
            };
        }

        //[Authorize(Roles = AuthTypes.Roles.Administrator)]
        //[HttpPost, Route(RouteActionsDownload)]
        //public async Task<ActionResult<BoolResult>> Download([FromBody] DownloadRequest request)
        //{
        //    PackageUtils.DownloadPackage(_pathManager, request.PackageId, request.Version);

        //    var isDownload = PackageUtils.IsPackageDownload(_pathManager, request.PackageId, request.Version);
        //    if (isDownload)
        //    {
        //        if (StringUtils.EqualsIgnoreCase(request.PackageId, Constants.PackageIdSsCms))
        //        {
        //            await _dbCacheRepository.RemoveAndInsertAsync(PackageUtils.CacheKeySsCmsIsDownload, true.ToString());
        //        }
        //    }

        //    return new BoolResult
        //    {
        //        Value = isDownload
        //    };
        //}
    }
}
