using System;
using System.IO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using SiteServer.CMS.Common;
using SiteServer.CMS.Core;
using SiteServer.CMS.DataCache;
using SiteServer.CMS.Model;
using SiteServer.CMS.Plugin.Apis;
using SiteServer.Plugin;

namespace SiteServer.API.Common
{
    public partial class Request : IRequest
    {
        private HttpContext _context;
        private AdministratorInfo _adminInfo;
        private UserInfo _userInfo;

        public Request(HttpContext context)
        {
            _context = context;

            try
            {
                var apiToken = ApiToken;
                if (!string.IsNullOrEmpty(apiToken))
                {
                    var tokenInfo = AccessTokenManager.GetAccessTokenInfo(apiToken);
                    if (tokenInfo != null)
                    {
                        if (!string.IsNullOrEmpty(tokenInfo.AdminName))
                        {
                            var adminInfo = AdminManager.GetAdminInfoByUserName(tokenInfo.AdminName);
                            if (adminInfo != null && !adminInfo.Locked)
                            {
                                _adminInfo = adminInfo;
                                IsAdminLoggin = true;
                            }
                        }

                        IsApiAuthenticated = true;
                    }
                }

                var userToken = UserToken;
                if (!string.IsNullOrEmpty(userToken))
                {
                    var tokenImpl = UserApi.Instance.ParseAccessToken(userToken);
                    if (tokenImpl.UserId > 0 && !string.IsNullOrEmpty(tokenImpl.UserName))
                    {
                        var userInfo = UserManager.GetUserInfoByUserId(tokenImpl.UserId);
                        if (userInfo != null && !userInfo.Locked && userInfo.Checked && userInfo.UserName == tokenImpl.UserName)
                        {
                            _userInfo = userInfo;
                            IsUserLoggin = true;
                        }
                    }
                }

                var adminToken = AdminToken;
                if (!string.IsNullOrEmpty(adminToken))
                {
                    var tokenImpl = AdminApi.Instance.ParseAccessToken(adminToken);
                    if (tokenImpl.UserId > 0 && !string.IsNullOrEmpty(tokenImpl.UserName))
                    {
                        var adminInfo = AdminManager.GetAdminInfoByUserId(tokenImpl.UserId);
                        if (adminInfo != null && !adminInfo.Locked && adminInfo.UserName == tokenImpl.UserName)
                        {
                            _adminInfo = adminInfo;
                            IsAdminLoggin = true;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogUtils.AddErrorLog(ex);
            }
        }

        public bool IsHttps => _context.Request.IsHttps;

        public string Host => _context.Request.Host.Value;

        public string Path => _context.Request.Path.Value;

        public string IpAddress => _context.Connection.RemoteIpAddress.ToString();

        public Stream Body => _context.Request.Body;

        public string RawUrl => _context.Request.GetDisplayUrl();

        public ActionResult Run(Func<IRequest, IControllerResult> func)
        {
            var result = func(this);
            if (result.StatusCode == 200)
            {
                return new OkObjectResult(result.Result);
            }
            return new BadRequestResult();
        }
    }
}