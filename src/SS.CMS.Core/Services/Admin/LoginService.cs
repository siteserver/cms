using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using Microsoft.AspNetCore.Http;
using SS.CMS.Core.Cache;
using SS.CMS.Core.Common;
using SS.CMS.Core.Models;
using SS.CMS.Plugin;
using SS.CMS.Utils;

namespace SS.CMS.Core.Services.Admin
{
    public class LoginService : ServiceBase
    {
        public const string Route = "login";
        public const string RouteCaptcha = "login/captcha";

        private static readonly Color[] Colors = { Color.FromArgb(37, 72, 91), Color.FromArgb(68, 24, 25), Color.FromArgb(17, 46, 2), Color.FromArgb(70, 16, 100), Color.FromArgb(24, 88, 74) };

        public ResponseResult<object> Get(IRequest request, IResponse response)
        {
            // var redirect = AdminRedirectCheck(request, checkInstall: true, checkDatabaseVersion: true);
            // if (redirect != null) return Response<object>.Ok(redirect);

            var adminInfo = request.IsAdminLoggin ? request.AdminInfo : null;

            return Ok(new
            {
                Value = adminInfo
            });
        }

        private const string CookieName = "SS-" + nameof(LoginService);

        private static string CreateValidateCode()
        {
            var validateCode = "";

            char[] s = { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z', 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z' };
            var r = new Random();
            for (var i = 0; i < 4; i++)
            {
                validateCode += s[r.Next(0, s.Length)].ToString();
            }

            return validateCode;
        }

        public ResponseResult<byte[]> GetCaptcha(IRequest request, IResponse response)
        {
            var code = CreateValidateCode();
            if (CacheUtils.Exists($"SiteServer.CMS.Services.Admin.LoginService.{code}"))
            {
                code = CreateValidateCode();
            }
            
            response.Cookies.Delete(CookieName);
            response.Cookies.Append(CookieName, code, new CookieOptions
            {
                Expires = DateTimeOffset.UtcNow.AddMinutes(10)
            });

            //request.SetCookie("SS-" + nameof(LoginService), code, TimeSpan.FromMinutes(10));

            byte[] buffer;

            using (var image = new Bitmap(130, 53, PixelFormat.Format32bppRgb))
            {
                var r = new Random();
                var colors = Colors[r.Next(0, 5)];

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

            return File(".png", buffer);
        }

        public ResponseResult<object> Login(IRequest request, IResponse response)
        {
            var account = request.GetPostString("account");
            var password = request.GetPostString("password");
            var captcha = request.GetPostString("captcha");
            var isAutoLogin = request.GetPostBool("isAutoLogin");

            request.Cookies.TryGetValue(CookieName, out var code);

            if (string.IsNullOrEmpty(code) || CacheUtils.Exists($"SiteServer.CMS.Services.Admin.LoginService.{code}"))
            {
                return BadRequest("验证码已超时，请点击刷新验证码！");
            }

            response.Cookies.Delete(CookieName);

            CacheUtils.InsertMinutes($"SiteServer.CMS.Services.Admin.LoginService.{code}", true, 10);

            if (!StringUtils.EqualsIgnoreCase(code, captcha))
            {
                return BadRequest("验证码不正确，请重新输入！");
            }

            AdministratorInfo adminInfo;

            if (!DataProvider.AdministratorDao.Validate(account, password, true, out var userName, out var errorMessage))
            {
                adminInfo = AdminManager.GetAdminInfoByUserName(userName);
                if (adminInfo != null)
                {
                    DataProvider.AdministratorDao.UpdateLastActivityDateAndCountOfFailedLogin(adminInfo); // 记录最后登录时间、失败次数+1
                }
                return BadRequest(errorMessage);
            }

            adminInfo = AdminManager.GetAdminInfoByUserName(userName);
            DataProvider.AdministratorDao.UpdateLastActivityDateAndCountOfLogin(adminInfo); // 记录最后登录时间、失败次数清零
            var accessToken = response.AdminLogin(adminInfo.UserName, isAutoLogin);
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
