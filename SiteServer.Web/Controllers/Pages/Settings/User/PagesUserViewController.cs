using System;
using System.Threading.Tasks;
using System.Web.Http;
using SiteServer.CMS.Core;
using SiteServer.CMS.DataCache;

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
            await auth.CheckAdminLoggin(Request);

            CMS.Model.User user = null;
            if (request.UserId > 0)
            {
                user = await DataProvider.UserDao.GetByUserIdAsync(request.UserId);
            }
            else if (!string.IsNullOrEmpty(request.UserName))
            {
                user = await DataProvider.UserDao.GetByUserNameAsync(request.UserName);
            }

            if (user == null)
            {
                auth.NotFound(Request);
            }

            var groupName = await UserGroupManager.GetUserGroupNameAsync(user.GroupId);

            return new GetResult
            {
                User = user,
                GroupName = groupName
            };
        }
    }
}
