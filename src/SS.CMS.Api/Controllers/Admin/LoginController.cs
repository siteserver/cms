using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SS.CMS.Abstractions.Models;
using SS.CMS.Abstractions.Repositories;
using SS.CMS.Abstractions.Services;
using SS.CMS.Utils;

namespace SS.CMS.Api.Controllers.Admin
{
    [Route("admin")]
    [ApiController]
    public partial class LoginController : ControllerBase
    {
        public const string Route = "login";
        public const string RouteCaptcha = "login/captcha";

        private readonly IIdentityManager _identityManager;
        private readonly IAdministratorRepository _administratorRepository;

        public LoginController(IIdentityManager identityManager, IAdministratorRepository administratorRepository)
        {
            _identityManager = identityManager;
            _administratorRepository = administratorRepository;
        }

        [HttpGet(Route)]
        public ActionResult Get()
        {
            // var redirect = AdminRedirectCheck(request, checkInstall: true, checkDatabaseVersion: true);
            // if (redirect != null) return Response<object>.Ok(redirect);

            var adminInfo = _identityManager.IsAdminLoggin ? _identityManager.AdminInfo : null;

            return Ok(new
            {
                Value = adminInfo
            });
        }

        [HttpGet(RouteCaptcha)]
        public ActionResult GetCaptcha()
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

        [HttpPost(Route)]
        public ActionResult Login([FromBody] LoginContext context)
        {
            Request.Cookies.TryGetValue(_cookieName, out var code);

            if (string.IsNullOrEmpty(code))
            {
                return BadRequest("验证码已超时，请点击刷新验证码！");
            }

            Response.Cookies.Delete(_cookieName);

            if (!StringUtils.EqualsIgnoreCase(code, context.Captcha))
            {
                return BadRequest("验证码不正确，请重新输入！");
            }

            AdministratorInfo adminInfo;

            if (!_administratorRepository.Validate(context.Account, context.Password, true, out var userName, out var errorMessage))
            {
                adminInfo = _administratorRepository.GetAdminInfoByUserName(userName);
                if (adminInfo != null)
                {
                    _administratorRepository.UpdateLastActivityDateAndCountOfFailedLogin(adminInfo); // 记录最后登录时间、失败次数+1
                }
                return BadRequest(errorMessage);
            }

            adminInfo = _administratorRepository.GetAdminInfoByUserName(userName);
            _administratorRepository.UpdateLastActivityDateAndCountOfLogin(adminInfo); // 记录最后登录时间、失败次数清零
            var accessToken = AdminLogin(adminInfo.UserName, context.IsAutoLogin);
            var expiresAt = DateTime.Now.AddDays(Constants.AccessTokenExpireDays);

            return Ok(new
            {
                Value = adminInfo,
                AccessToken = accessToken,
                ExpiresAt = expiresAt
            });
        }
    }
}