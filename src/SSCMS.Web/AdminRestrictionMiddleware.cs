using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using SSCMS.Configuration;
using SSCMS.Services;
using SSCMS.Utils;

namespace SSCMS.Web
{
    /// <summary>
    /// 后台访问限制统一入口（GJ20260924）：原先只有 /api/admin/index 调用了 IsVisitAllowed，
    /// 其余后台接口（含 login）都能从白名单外访问。这里在管道层对整个后台前缀生效。
    /// </summary>
    public class AdminRestrictionMiddleware
    {
        private static readonly string[] ProtectedPrefixes =
        {
            Constants.ApiAdminPrefix + "/",
            "/" + Constants.AdminDirectory + "/",
            Constants.ApiV1Prefix + "/administrators/actions/login",
            Constants.ApiV1Prefix + "/administrators/actions/resetPassword",
        };

        private readonly RequestDelegate _next;

        public AdminRestrictionMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public static bool IsProtectedPath(PathString path)
        {
            var value = path.Value;
            if (string.IsNullOrEmpty(value)) return false;
            if (string.Equals(value, Constants.ApiAdminPrefix, StringComparison.OrdinalIgnoreCase) ||
                string.Equals(value, "/" + Constants.AdminDirectory, StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }
            foreach (var prefix in ProtectedPrefixes)
            {
                if (value.StartsWith(prefix, StringComparison.OrdinalIgnoreCase)) return true;
            }
            return false;
        }

        public async Task InvokeAsync(HttpContext context, ISettingsManager settingsManager)
        {
            var hasRestriction = !string.IsNullOrEmpty(settingsManager.AdminRestrictionHost) ||
                                 settingsManager.AdminRestrictionAllowList is { Length: > 0 } ||
                                 settingsManager.AdminRestrictionBlockList is { Length: > 0 };

            // 未安装（无数据库连接）时放行，保证安装向导可用
            if (hasRestriction && !string.IsNullOrEmpty(settingsManager.DatabaseConnectionString) &&
                IsProtectedPath(context.Request.Path) &&
                !PageUtils.IsVisitAllowed(settingsManager, context.Request))
            {
                context.Response.StatusCode = StatusCodes.Status403Forbidden;
                context.Response.ContentType = "application/json; charset=utf-8";
                await context.Response.WriteAsync(TranslateUtils.JsonSerialize(new
                {
                    Message = "访问已被禁止，请与网站管理员联系开通访问权限"
                }));
                return;
            }

            await _next(context);
        }
    }
}
