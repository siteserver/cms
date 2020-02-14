using System.Threading.Tasks;
using System.Web.Http;
using SiteServer.CMS.Core;
using SiteServer.CMS.Extensions;
using SiteServer.CMS.Repositories;

namespace SiteServer.API.Controllers.Pages.Settings.User
{
    
    [RoutePrefix("pages/settings/userView")]
    public partial class PagesUserViewController : ApiController
    {
        private const string Route = "";

        [HttpGet, Route(Route)]
        public async Task<GetResult> Get([FromUri] GetRequest request)
        {
            var auth = await AuthenticatedRequest.GetAuthAsync();
            if (!auth.IsAdminLoggin)
            {
                return Request.Unauthorized<GetResult>();
            }

            Abstractions.User user = null;
            if (request.UserId > 0)
            {
                user = await DataProvider.UserRepository.GetByUserIdAsync(request.UserId);
            }
            else if (!string.IsNullOrEmpty(request.UserName))
            {
                user = await DataProvider.UserRepository.GetByUserNameAsync(request.UserName);
            }

            if (user == null)
            {
                return Request.NotFound<GetResult>();
            }

            var groupName = await DataProvider.UserGroupRepository.GetUserGroupNameAsync(user.GroupId);

            return new GetResult
            {
                User = user,
                GroupName = groupName
            };
        }
    }
}
