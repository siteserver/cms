using SS.CMS.Core.Cache;
using SS.CMS.Core.Common;
using SS.CMS.Plugin;

namespace SS.CMS.Core.Services.Admin
{
    public class SyncService : ServiceBase
    {
        public const string Route = "sync";

        public ResponseResult<object> Get(IRequest request, IResponse response)
        {
            if (!request.IsAdminLoggin) return Unauthorized();

            if (SystemManager.IsNeedInstall())
            {
                return BadRequest("系统未安装，向导被禁用");
            }

            return Ok(new
            {
                Value = true,
                ConfigManager.Instance.DatabaseVersion,
                SystemManager.ProductVersion
            });
        }

        public ResponseResult<object> Update(IRequest request, IResponse response)
        {
            if (!request.IsAdminLoggin) return Unauthorized();

            SystemManager.SyncDatabase();

            return Ok(new
            {
                SystemManager.ProductVersion
            });
        }
    }
}