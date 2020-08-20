using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace SSCMS.Web.Controllers.Admin
{
    public partial class SyncDatabaseController
    {
        [HttpPost, Route(Route)]
        public async Task<ActionResult<SubmitResult>> Submit()
        {
            //
            //if (!request.IsAdminLoggin || !request.AdminPermissions.IsSuperAdmin())
            //{
            //    return Unauthorized();
            //}

            await _databaseManager.SyncDatabaseAsync();

            return new SubmitResult
            {
                Version = _settingsManager.Version
            };
        }
    }
}
