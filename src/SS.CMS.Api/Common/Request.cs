using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using SS.CMS.Core.Cache;
using SS.CMS.Core.Common;
using SS.CMS.Core.Models;
using SS.CMS.Core.Plugin.Apis;
using SS.CMS.Core.Repositories;
using SS.CMS.Plugin;

namespace SS.CMS.Api.Common
{
    public partial class Request : IRequest
    {
        private readonly HttpContext _context;
        private AdministratorInfo _adminInfo;
        private readonly UserInfo _userInfo;

        public Request(HttpContext context, IAccessTokenRepository accessTokenRepository)
        {
            _context = context;

            try
            {
                var apiToken = ApiToken;
                //if (!string.IsNullOrEmpty(apiToken))
                //{
                //    var tokenInfo = await cacheService.GetAccessTokenInfoAsync(apiToken);
                //    if (tokenInfo != null)
                //    {
                //        if (!string.IsNullOrEmpty(tokenInfo.AdminName))
                //        {
                //            var adminInfo = AdminManager.GetAdminInfoByUserName(tokenInfo.AdminName);
                //            if (adminInfo != null && !adminInfo.Locked)
                //            {
                //                _adminInfo = adminInfo;
                //                IsAdminLoggin = true;
                //            }
                //        }

                //        IsApiAuthenticated = true;
                //    }
                //}

                var userToken = UserToken;
                if (!string.IsNullOrEmpty(userToken))
                {
                    var tokenImpl = accessTokenRepository.ParseAccessToken(userToken);
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
                    var tokenImpl = accessTokenRepository.ParseAccessToken(adminToken);
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

        public IRequestCookieCollection Cookies => _context.Request.Cookies;
        public IHeaderDictionary Headers => _context.Request.Headers;
        public IDictionary<string, object> RouteValues => _context.Request.RouteValues;
        public IFormCollection Form => _context.Request.Form;

        public bool IsHttps => _context.Request.IsHttps;

        public string Host => _context.Request.Host.Value;

        public string Path => _context.Request.Path.Value;

        public string IpAddress => _context.Connection.RemoteIpAddress.ToString();

        public Stream Body => _context.Request.Body;

        public string RawUrl => _context.Request.GetDisplayUrl();
    }
}