using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SS.CMS.Models;
using SS.CMS.Repositories;
using SS.CMS.Services;
using SS.CMS.Utils;

namespace SS.CMS.Api.Controllers.V2
{
    [AllowAnonymous]
    [Route("v2")]
    [ApiController]
    public partial class UserController : ControllerBase
    {
        public const string RouteLogin = "user/login";
        public const string RouteLoginCaptcha = "user/login/captcha";
        public const string RouteInfo = "user/info";
        public const string RouteLogout = "user/logout";

        private readonly IUserManager _userManager;
        private readonly IUserRepository _userRepository;

        public UserController(IUserManager userManager, IUserRepository userRepository)
        {
            _userManager = userManager;
            _userRepository = userRepository;
        }

        [Authorize]
        [HttpGet(RouteInfo)]
        public async Task<ActionResult> GetInfo(string token)
        {
            // var redirect = AdminRedirectCheck(request, checkInstall: true, checkDatabaseVersion: true);
            // if (redirect != null) return Response<object>.Ok(redirect);

            var userInfo = await _userManager.GetUserAsync();

            return Ok(new
            {
                Name = userInfo.UserName,
                Avatar = userInfo.AvatarUrl,
                Introduction = userInfo.Bio,
                Roles = new string[] { "admin" }
            });
        }

        [HttpGet(RouteLoginCaptcha)]
        public ActionResult GetLoginCaptcha()
        {
            var code = CreateValidateCode();

            Response.Cookies.Delete(_cookieName);
            Response.Cookies.Append(_cookieName, code, new CookieOptions
            {
                Expires = DateTimeOffset.UtcNow.AddMinutes(10)
            });

            byte[] buffer;

            using (var image = new Bitmap(130, 53, PixelFormat.Format32bppRgb))
            {
                var r = new Random();
                var colors = _colors[r.Next(0, 5)];

                using (var g = Graphics.FromImage(image))
                {
                    g.FillRectangle(new SolidBrush(Color.FromArgb(240, 243, 248)), 0, 0, 200, 200); //矩形框
                    g.DrawString(code, new Font(FontFamily.GenericSerif, 28, FontStyle.Bold | FontStyle.Italic), new SolidBrush(colors), new PointF(14, 3));//字体/颜色

                    var random = new Random();

                    for (var i = 0; i < 25; i++)
                    {
                        var x1 = random.Next(image.Width);
                        var x2 = random.Next(image.Width);
                        var y1 = random.Next(image.Height);
                        var y2 = random.Next(image.Height);

                        g.DrawLine(new Pen(Color.Silver), x1, y1, x2, y2);
                    }

                    for (var i = 0; i < 100; i++)
                    {
                        var x = random.Next(image.Width);
                        var y = random.Next(image.Height);

                        image.SetPixel(x, y, Color.FromArgb(random.Next()));
                    }

                    g.Save();
                }

                using (var ms = new MemoryStream())
                {
                    image.Save(ms, ImageFormat.Png);
                    buffer = ms.ToArray();
                }
            }

            if (!ContentTypeUtils.TryGetContentType(".png", out var contentType))
            {
                contentType = ContentTypeUtils.ContentTypeAttachment;
            }

            return File(buffer, contentType);
        }

        [HttpPost(RouteLogin)]
        public async Task<ActionResult> Login([FromBody] LoginRequest request)
        {
            UserInfo userInfo;

            if (!_userRepository.Validate(request.UserName, request.Password, true, out var userName, out var errorMessage))
            {
                userInfo = _userRepository.GetUserInfoByUserName(userName);
                if (userInfo != null)
                {
                    _userRepository.UpdateLastActivityDateAndCountOfFailedLogin(userInfo); // 记录最后登录时间、失败次数+1
                }
                return BadRequest(new
                {
                    Code = 400,
                    Message = errorMessage
                });
            }

            userInfo = _userRepository.GetUserInfoByUserName(userName);
            _userRepository.UpdateLastActivityDateAndCountOfLogin(userInfo); // 记录最后登录时间、失败次数清零
            //var accessToken = AdminLogin(userInfo.UserName, context.IsAutoLogin);
            //var expiresAt = DateTime.Now.AddDays(Constants.AccessTokenExpireDays);

            var token = await _userManager.SignInAsync(userInfo);

            return Ok(new
            {
                Token = token,
                //AccessToken = accessToken,
                //ExpiresAt = expiresAt
            });
        }

        [HttpPost(RouteLogout)]
        public async Task<ActionResult> Logout()
        {
            //await _userManager.SignOutAsync();

            return Ok(new
            {
                Value = true
            });
        }
    }
}