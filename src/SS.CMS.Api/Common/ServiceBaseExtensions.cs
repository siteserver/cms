using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using SS.CMS.Core.Common;
using SS.CMS.Core.Services;
using SS.CMS.Plugin;
using SS.CMS.Utils;

namespace SS.CMS.Api.Common
{
    public static class ServiceBaseExtensions
    {
        public static ActionResult Run<T>(this ServiceBase service, IRequest request, IResponse response, Func<IRequest, IResponse, ResponseResult<T>> func)
        {
            try
            {
                var result = func(request, response);

                if (typeof(T) == typeof(byte[]))
                {
                    return new FileContentResult(result.Body as byte[], result.ContentType);
                }

                return new ObjectResult(result.Body)
                {
                    StatusCode = (int)result.StatusCode,
                    ContentTypes = new MediaTypeCollection
                    {
                        result.ContentType
                    }
                };
            }
            catch (Exception ex)
            {
                LogUtils.AddErrorLog(ex);

                return new ObjectResult(new
                {
                    Value = false,
                    ex.Message
                })
                {
                    StatusCode = (int) HttpStatusCode.InternalServerError,
                    ContentTypes = new MediaTypeCollection
                    {
                        ContentTypeUtils.ContentTypeJson
                    }
                };
            }
        }

        public static async Task<ActionResult> Run<T>(this ServiceBase service, IRequest request, IResponse response, Func<IRequest, IResponse, Task<ResponseResult<T>>> func)
        {
            try
            {
                var result = await func(request, response);

                if (typeof(T) == typeof(byte[]))
                {
                    return new FileContentResult(result.Body as byte[], result.ContentType);
                }

                return new ObjectResult(result.Body)
                {
                    StatusCode = (int)result.StatusCode,
                    ContentTypes = new MediaTypeCollection
                    {
                        result.ContentType
                    }
                };
            }
            catch (Exception ex)
            {
                LogUtils.AddErrorLog(ex);

                return new ObjectResult(new
                {
                    Value = false,
                    ex.Message
                })
                {
                    StatusCode = (int)HttpStatusCode.InternalServerError,
                    ContentTypes = new MediaTypeCollection
                    {
                        ContentTypeUtils.ContentTypeJson
                    }
                };
            }
        }
    }
}
