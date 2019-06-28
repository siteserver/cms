using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SS.CMS.Models;
using SS.CMS.Repositories;
using SS.CMS.Services;
using SS.CMS.Utils;

namespace SS.CMS.Api.Controllers.Admin
{
    [AllowAnonymous]
    [Route("admin")]
    [ApiController]
    public partial class LoginController : ControllerBase
    {
        public const string Route = "login";
        public const string RouteCaptcha = "login/captcha";

        private readonly IUserManager _userManager;
        private readonly IUserRepository _userRepository;

        public LoginController(IUserManager userManager, IUserRepository userRepository)
        {
            _userManager = userManager;
            _userRepository = userRepository;
        }

        [Authorize(Roles = AuthTypes.Roles.Administrator)]
        [HttpGet(Route)]
        public async Task<ActionResult> Get()
        {
            // var redirect = AdminRedirectCheck(request, checkInstall: true, checkDatabaseVersion: true);
            // if (redirect != null) return Response<object>.Ok(redirect);

            var accountInfo = await _userManager.GetUserAsync();

            return Ok(new
            {
                Value = accountInfo
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
        public async Task<ActionResult> Login([FromBody] LoginContext context)
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

            UserInfo userInfo;

            if (!_userRepository.Validate(context.Account, context.Password, true, out var userName, out var errorMessage))
            {
                userInfo = _userRepository.GetUserInfoByUserName(userName);
                if (userInfo != null)
                {
                    _userRepository.UpdateLastActivityDateAndCountOfFailedLogin(userInfo); // 记录最后登录时间、失败次数+1
                }
                return BadRequest(errorMessage);
            }

            userInfo = _userRepository.GetUserInfoByUserName(userName);
            _userRepository.UpdateLastActivityDateAndCountOfLogin(userInfo); // 记录最后登录时间、失败次数清零
            //var accessToken = AdminLogin(userInfo.UserName, context.IsAutoLogin);
            //var expiresAt = DateTime.Now.AddDays(Constants.AccessTokenExpireDays);

            await _userManager.SignInAsync(userInfo);

            return Ok(new
            {
                Value = userInfo,
                //AccessToken = accessToken,
                //ExpiresAt = expiresAt
            });
        }
    }
}