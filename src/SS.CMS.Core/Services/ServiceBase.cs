using System.Net;
using SS.CMS.Core.Cache;
using SS.CMS.Core.Common;
using SS.CMS.Plugin;
using SS.CMS.Utils;

namespace SS.CMS.Core.Services
{
    public abstract partial class ServiceBase
    {
        protected object AdminRedirectCheck(IRequest request, bool checkInstall = false, bool checkDatabaseVersion = false,
            bool checkLogin = false)
        {
            var redirect = false;
            var redirectUrl = string.Empty;

            if (checkInstall && string.IsNullOrWhiteSpace(AppSettings.ConnectionString))
            {
                redirect = true;
                redirectUrl = AdminUrl.InstallUrl;
            }
            else if (checkDatabaseVersion && ConfigManager.Instance.Initialized &&
                     ConfigManager.Instance.DatabaseVersion != SystemManager.ProductVersion)
            {
                redirect = true;
                redirectUrl = AdminUrl.SyncUrl;
            }
            else if (checkLogin && !request.IsAdminLoggin)
            {
                redirect = true;
                redirectUrl = AdminUrl.LoginUrl;
            }

            if (redirect)
            {
                return new
                {
                    Value = false,
                    RedirectUrl = redirectUrl
                };
            }

            return null;
        }

        protected ResponseResult<object> Ok(object value)
        {
            return new ResponseResult<object>
            {
                StatusCode = HttpStatusCode.OK,
                ContentType = ContentTypeUtils.ContentTypeJson,
                Body = value
            };
        }

        protected ResponseResult<byte[]> File(string extName, byte[] bytes)
        {
            if (!ContentTypeUtils.TryGetContentType(extName, out var contentType))
            {
                contentType = ContentTypeUtils.ContentTypeAttachment;
            }
            return new ResponseResult<byte[]>
            {
                StatusCode = HttpStatusCode.OK,
                ContentType = contentType,
                Body = bytes
            };
        }

        protected ResponseResult<object> BadRequest(string message)
        {
            return new ResponseResult<object>
            {
                StatusCode = HttpStatusCode.BadRequest,
                ContentType = "application/json",
                Body = new
                {
                    Value = false,
                    Message = message
                }
            };
        }

        protected ResponseResult<object> NotFound()
        {
            return new ResponseResult<object>
            {
                StatusCode = HttpStatusCode.NotFound,
                ContentType = "application/json",
                Body = new
                {
                    Value = false,
                    Message = "NotFound"
                }
            };
        }

        protected ResponseResult<object> Unauthorized()
        {
            return new ResponseResult<object>
            {
                StatusCode = HttpStatusCode.Unauthorized,
                ContentType = "application/json",
                Body = new
                {
                    Value = false,
                    Message = "Unauthorized"
                }
            };
        }
    }
}
