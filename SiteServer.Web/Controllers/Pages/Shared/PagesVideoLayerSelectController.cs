using System.Threading.Tasks;
using System.Web.Http;
using SiteServer.Abstractions;
using SiteServer.Abstractions.Dto.Result;
using SiteServer.API.Context;
using SiteServer.CMS.Core;
using SiteServer.CMS.Framework;
using SiteServer.CMS.Repositories;

namespace SiteServer.API.Controllers.Pages.Shared
{
    [RoutePrefix("pages/shared/videoLayerSelect")]
    public partial class PagesVideoLayerSelectController : ApiController
    {
        private const string Route = "";
        private const string RouteSelect = "actions/select";

        [HttpGet, Route(Route)]
        public async Task<QueryResult> List([FromUri]QueryRequest request)
        {
            var auth = await AuthenticatedRequest.GetAuthAsync();
            if (!auth.IsAdminLoggin) return Request.Unauthorized<QueryResult>();

            var groups = await DataProvider.LibraryGroupRepository.GetAllAsync(LibraryType.Video);
            groups.Insert(0, new LibraryGroup
            {
                Id = 0,
                GroupName = "全部视频"
            });
            var count = await DataProvider.LibraryVideoRepository.GetCountAsync(request.GroupId, request.Keyword);
            var items = await DataProvider.LibraryVideoRepository.GetAllAsync(request.GroupId, request.Keyword, request.Page, request.PerPage);

            return new QueryResult
            {
                Groups = groups,
                Count = count,
                Items = items
            };
        }

        [HttpPost, Route(RouteSelect)]
        public async Task<StringResult> Select([FromBody]SelectRequest request)
        {
            var auth = await AuthenticatedRequest.GetAuthAsync();
            if (!auth.IsAdminLoggin) return Request.Unauthorized<StringResult>();

            var site = await DataProvider.SiteRepository.GetAsync(request.SiteId);
            var library = await DataProvider.LibraryVideoRepository.GetAsync(request.LibraryId);

            var libraryFilePath = PathUtils.Combine(WebConfigUtils.PhysicalApplicationPath, library.Url);
            if (!FileUtils.IsFileExists(libraryFilePath))
            {
                return Request.BadRequest<StringResult>("视频不存在，请重新选择");
            }

            var localDirectoryPath = await PathUtility.GetUploadDirectoryPathAsync(site, UploadType.Video);
            var filePath = PathUtils.Combine(localDirectoryPath, PathUtility.GetUploadFileName(site, libraryFilePath));

            DirectoryUtils.CreateDirectoryIfNotExists(filePath);
            FileUtils.CopyFile(libraryFilePath, filePath);

            var fileUrl = await PageUtility.GetSiteUrlByPhysicalPathAsync(site, filePath, true);

            return new StringResult
            {
                Value = fileUrl
            };
        }
    }
}
