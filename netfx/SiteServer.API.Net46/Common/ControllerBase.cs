using SiteServer.BackgroundPages.Common;
using SiteServer.CMS.Core;
using SiteServer.Plugin;
using System;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;

namespace SiteServer.API.Common
{
    public class ControllerBase : ApiController
    {
        //protected new Request Request => GetRequest();

        private Request _request;
        private Response _response;

        protected Request GetRequest()
        {
            if (_request != null) return _request;
            _request = BackgroundPages.Common.Request.Current;
            return _request;
        }

        protected Response GetResponse()
        {
            if (_response != null) return _response;
            _response = Response.Current;
            return _response;
        }

        public IHttpActionResult Run<T>(Func<IRequest, IResponse, ResponseResult<T>> func)
        {
            try
            {
                var request = GetRequest();
                var response = GetResponse();

                var result = func(request, response);
                if (result.StatusCode == HttpStatusCode.OK)
                {
                    return Ok(result.Body);
                }

                return BadRequest(result.Body.ToString());
            }
            catch (Exception ex)
            {
                LogUtils.AddErrorLog(ex);
                return InternalServerError(ex);
            }
        }

        public async Task<IHttpActionResult> Run<T>(Func<IRequest, IResponse, Task<ResponseResult<T>>> func)
        {
            try
            {
                var request = GetRequest();
                var response = GetResponse();

                var result = await func(request, response);
                if (result.StatusCode == HttpStatusCode.OK)
                {
                    return Ok(result.Body);
                }

                return BadRequest(result.Body.ToString());
            }
            catch (Exception ex)
            {
                LogUtils.AddErrorLog(ex);
                return InternalServerError(ex);
            }
        }

        //public void Run(Func<IRequest, IResponse<byte[]>> func)
        //{
        //    try
        //    {
        //        var response = func(GetRequest());
        //        if (response.StatusCode == Response<byte[]>.StatusOK)
        //        {
        //            if (response.ContentType == Response<byte[]>.ContentTypePng)
        //            {
        //                var httpResponse = HttpContext.Current.Response;

        //                httpResponse.BufferOutput = true;  //特别注意
        //                httpResponse.Cache.SetExpires(DateTime.Now.AddMilliseconds(-1));//特别注意
        //                httpResponse.Cache.SetCacheability(HttpCacheability.NoCache);//特别注意
        //                httpResponse.AppendHeader("Pragma", "No-Cache"); //特别注意
        //                httpResponse.ContentType = response.ContentType;

        //                httpResponse.ClearContent();
        //                httpResponse.BinaryWrite(response.Body);
        //                httpResponse.End();
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        LogUtils.AddErrorLog(ex);
        //    }
        //}
    }
}