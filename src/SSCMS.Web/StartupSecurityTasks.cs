using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using SSCMS.Repositories;
using SSCMS.Services;
using SSCMS.Utils;

namespace SSCMS.Web
{
    /// <summary>
    /// 启动后执行的安全任务（GJ20260924）：
    /// 1. 弱 SecurityKey（非 hex32.hex32 格式，退化为仅 8 字节有效的 DES）写错误日志；
    /// 2. 管理员表中可逆口令（Clear/DES/SM4）批量转换为 PBKDF2。
    /// </summary>
    public static class StartupSecurityTasks
    {
        public static async Task RunAsync(IServiceProvider services)
        {
            try
            {
                using var scope = services.CreateScope();
                var provider = scope.ServiceProvider;
                var settingsManager = provider.GetRequiredService<ISettingsManager>();
                var configRepository = provider.GetRequiredService<IConfigRepository>();
                var errorLogRepository = provider.GetRequiredService<IErrorLogRepository>();

                if (string.IsNullOrEmpty(settingsManager.DatabaseConnectionString)) return;
                if (await configRepository.IsNeedInstallAsync()) return;

                if (!EncryptUtils.IsValidSecurityKey(settingsManager.SecurityKey).isValid)
                {
                    const string message = "SecurityKey 为旧格式，加密强度仅相当于 8 字节 DES，可被离线爆破。请超级管理员在“系统设置 → 实用工具 → 加密字符串”中立即轮换。";
                    Console.Error.WriteLine(message);
                    await errorLogRepository.AddErrorLogAsync(new Exception(message));
                }

                var administratorRepository = provider.GetRequiredService<IAdministratorRepository>();
                var count = await administratorRepository.MigrateLegacyPasswordsAsync();
                if (count > 0)
                {
                    Console.WriteLine($"已将 {count} 个管理员的可逆口令转换为 PBKDF2");
                }
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Startup security tasks failed: {ex}");
            }
        }
    }
}
