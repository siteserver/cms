using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using Senparc.CO2NET;
using Senparc.CO2NET.RegisterServices;
using SSCMS.Configuration;
using SSCMS.Core.Extensions;
using SSCMS.Core.Plugins.Extensions;
using SSCMS.Repositories;
using SSCMS.Services;
using SSCMS.Utils;
using Constants = SSCMS.Configuration.Constants;

namespace SSCMS.Web
{
    public class Startup
    {
        private const string CorsPolicy = "CorsPolicy";
        private readonly IWebHostEnvironment _env;
        private readonly IConfiguration _config;

        public Startup(IWebHostEnvironment env, IConfiguration config)
        {
            _env = env;
            _config = config;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            var directory = new DirectoryInfo(_env.ContentRootPath);
            services.AddDataProtection().PersistKeysToFileSystem(directory);

            var entryAssembly = Assembly.GetExecutingAssembly();
            var assemblies = new List<Assembly> { entryAssembly }.Concat(entryAssembly.GetReferencedAssemblies().Select(Assembly.Load));

            var settingsManager = services.AddSettingsManager(_config, _env.ContentRootPath, _env.WebRootPath, entryAssembly);
            var pluginManager = services.AddPlugins(_config, settingsManager);

            if (settingsManager.CorsIsOrigins)
            {
                services.AddCors(options =>
                {
                    options.AddPolicy(CorsPolicy,
                        builder => builder
                            .AllowAnyMethod()
                            .AllowAnyHeader()
                            .WithOrigins(settingsManager.CorsOrigins)
                            .AllowCredentials()
                    );
                });
            }
            else
            {
                services.AddCors(options =>
                {
                    options.AddPolicy(CorsPolicy,
                        builder => builder
                            .AllowAnyMethod()
                            .AllowAnyHeader()
                            .SetIsOriginAllowed(x => true)
                            .AllowCredentials()
                    );
                });
            }

            services.AddHttpContextAccessor();

            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(x =>
            {
                x.RequireHttpsMetadata = false;
                x.SaveToken = true;
                x.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(EncryptUtils.GetSecurityKeyBytes(settingsManager.SecurityKey)),
                    ValidateIssuer = false,
                    ValidateAudience = false
                };
                x.Events = new JwtBearerEvents
                {
                    // 拒绝已吊销（登出/改密/锁定之前签发）的令牌（#3894）
                    OnTokenValidated = async (context) =>
                    {
                        var principal = context.Principal;
                        var id = TranslateUtils.ToInt(principal?.FindFirst(Types.Claims.UserId)?.Value);
                        var isAdmin = principal?.IsInRole(Types.Roles.Administrator) ?? false;
                        var dbCacheRepository = context.HttpContext.RequestServices.GetService<SSCMS.Repositories.IDbCacheRepository>();
                        var issuedAt = context.SecurityToken is System.IdentityModel.Tokens.Jwt.JwtSecurityToken jwt
                            ? jwt.IssuedAt
                            : DateTime.MinValue;
                        if (await SSCMS.Core.Utils.TokenRevocationUtils.IsRevokedAsync(dbCacheRepository, isAdmin, id, issuedAt))
                        {
                            context.Fail("Token revoked");
                        }
                    },
                    OnMessageReceived = (context) =>
                    {
                        if (!context.Request.Query.TryGetValue("access_token", out var values))
                        {
                            return Task.CompletedTask;
                        }
                        if (values.Count > 1)
                        {
                            return Task.CompletedTask;
                        }
                        var token = values.Single();
                        if (string.IsNullOrWhiteSpace(token))
                        {
                            return Task.CompletedTask;
                        }
                        context.Token = token;
                        return Task.CompletedTask;
                    }
                };
            });

            // services.Configure<FormOptions>(options =>
            // {
            //     options.MultipartBodyLengthLimit = 524288000;//500MB
            // });
            services.Configure<FormOptions>(x => {
                x.ValueLengthLimit = int.MaxValue;
                x.MultipartBodyLengthLimit = long.MaxValue; // In case of multipart
            });

            services.AddHealthChecks();

            services.AddRazorPages()
                .AddPluginApplicationParts(pluginManager);
            // .SetCompatibilityVersion(CompatibilityVersion.Latest);

            services.AddCache(settingsManager.Redis.ConnectionString);

            services.AddRepositories(assemblies);
            services.AddServices();
            services.AddWxManager(_config);

            services.AddPseudoServices();
            services.AddPluginServices(pluginManager);
            services.AddTaskServices();

            services.AddLocalization(options => options.ResourcesPath = "Resources");
            services
                .AddControllers()
                .AddViewLocalization(
                    LanguageViewLocationExpanderFormat.Suffix,
                    opts => { opts.ResourcesPath = "Resources"; })
                .AddDataAnnotationsLocalization()
                .AddNewtonsoftJson(options =>
                {
                    options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                    options.SerializerSettings.DateFormatString = "yyyy-MM-dd HH:mm:ss";
                    options.SerializerSettings.ContractResolver
                        = new CamelCasePropertyNamesContractResolver();
                    options.SerializerSettings.Converters.Add(new StringEnumConverter());
                });

            services.AddAntiforgery(options => options.HeaderName = "X-CSRF-TOKEN");

            services.Configure<RequestLocalizationOptions>(options =>
            {
                var supportedCultures = new[]
                {
                    new CultureInfo("en-US"),
                    new CultureInfo("zh-CN")
                };

                options.DefaultRequestCulture = new RequestCulture(culture: "zh-CN", uiCulture: "zh-CN");
                options.SupportedCultures = supportedCultures;
                options.SupportedUICultures = supportedCultures;
            });

            if (!settingsManager.IsSafeMode)
            {
                //http://localhost:5000/api/swagger/v1/swagger.json
                //http://localhost:5000/api/swagger/
                //http://localhost:5000/api/docs/
                services.AddOpenApiDocument(config =>
                {
                    config.PostProcess = document =>
                    {
                        document.Info.Version = "v1";
                        document.Info.Title = "SSCMS REST API";
                        document.Info.Description = "SSCMS REST API 为 SSCMS 提供了一个基于HTTP的API调用，允许开发者通过发送和接收JSON对象来远程与站点进行交互。";
                        document.Info.Contact = new NSwag.OpenApiContact
                        {
                            Name = "SSCMS",
                            Email = string.Empty,
                            Url = Constants.OfficialHost
                        };
                        document.Info.License = new NSwag.OpenApiLicense
                        {
                            Name = "GPL-3.0",
                            Url = "https://github.com/siteserver/cms/blob/staging/LICENSE"
                        };
                    };
                });
            }
        }

        private ForwardedHeadersOptions GetForwardedHeadersOptions()
        {
            var options = new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto,
                ForwardLimit = 1
            };
            // 默认 KnownProxies 已包含 IPv4/IPv6 loopback
            var trustedProxies = _config.GetSection("AdminRestriction:TrustedProxies").Get<string[]>();
            if (trustedProxies != null)
            {
                foreach (var proxy in trustedProxies)
                {
                    if (string.IsNullOrWhiteSpace(proxy)) continue;
                    var value = proxy.Trim();
                    if (value.Contains('/'))
                    {
                        var parts = value.Split('/');
                        if (System.Net.IPAddress.TryParse(parts[0], out var prefix) && int.TryParse(parts[1], out var length))
                        {
                            options.KnownNetworks.Add(new Microsoft.AspNetCore.HttpOverrides.IPNetwork(prefix, length));
                        }
                    }
                    else if (System.Net.IPAddress.TryParse(value, out var address))
                    {
                        options.KnownProxies.Add(address);
                    }
                }
            }
            if (trustedProxies == null || !trustedProxies.Any(p => !string.IsNullOrWhiteSpace(p)))
            {
                // 未配置 TrustedProxies 时信任任意来源的代理，nginx / 负载均衡不在同一台机器上也无需额外配置。
                // 清空 KnownProxies/KnownNetworks 即表示不限制代理来源；ForwardLimit = 1 只取最近一跳代理追加的 IP。
                // 如需收紧（例如站点直接暴露公网、未经代理），可在 sscms.json 中配置 AdminRestriction:TrustedProxies。
                options.KnownProxies.Clear();
                options.KnownNetworks.Clear();
            }
            return options;
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ISettingsManager settingsManager, IPluginManager pluginManager, IErrorLogRepository errorLogRepository, IOptions<SenparcSetting> senparcSetting)
        {
            // 放在管道最前：采用 X-Forwarded-For 作为客户端 IP。默认信任任意代理来源，
            // 如在 sscms.json 配置了 AdminRestriction:TrustedProxies，则只信任所配置的代理
            app.UseForwardedHeaders(GetForwardedHeadersOptions());

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseExceptionHandler(a => a.Run(async context =>
            {
                var exceptionHandlerPathFeature = context.Features.Get<IExceptionHandlerPathFeature>();
                var exception = exceptionHandlerPathFeature.Error;

                var errorId = 0;
                try
                {
                    errorId = errorLogRepository.AddErrorLogAsync(exception).GetAwaiter().GetResult();
                }
                catch
                {
                    // ignored
                }

                string result;
                if (env.IsDevelopment())
                {
                    result = TranslateUtils.JsonSerialize(new
                    {
                        exception.Message,
                        exception.StackTrace,
                        CreatedDate = DateTime.Now
                    });
                }
                else
                {
                    // 生产环境不向客户端返回异常详情（避免泄露库名、表名、字段名、路径等），详情见系统错误日志
                    result = TranslateUtils.JsonSerialize(new
                    {
                        Message = "系统异常，请稍后重试或联系管理员",
                        ErrorId = errorId,
                        CreatedDate = DateTime.Now
                    });
                }
                context.Response.ContentType = "application/json";
                await context.Response.WriteAsync(result);
            }));

            // 移除可能泄露中间件/框架版本的响应头
            app.Use(async (context, next) =>
            {
                context.Response.OnStarting(() =>
                {
                    var headers = context.Response.Headers;
                    headers.Remove("Server");
                    headers.Remove("X-Powered-By");
                    headers.Remove("X-AspNet-Version");
                    headers.Remove("X-AspNetMvc-Version");
                    if (!headers.ContainsKey("X-Content-Type-Options"))
                    {
                        headers["X-Content-Type-Options"] = "nosniff";
                    }
                    return Task.CompletedTask;
                });
                await next();
            });

            app.UseCors(CorsPolicy);

            // 后台 IP 白名单在管道层统一拦截 /api/admin、/ss-admin（GJ20260924）
            app.UseMiddleware<AdminRestrictionMiddleware>();

            //app.UseHttpsRedirection();

            var options = new DefaultFilesOptions();
            options.DefaultFileNames.Clear();
            options.DefaultFileNames.Add("index.html");
            app.UseDefaultFiles(options);

            //if (settingsManager.Containerized)
            //{
            //    app.Map($"/{DirectoryUtils.SiteFiles.DirectoryName}/assets", assets =>
            //    {
            //        assets.UseStaticFiles(new StaticFileOptions
            //        {
            //            FileProvider = new PhysicalFileProvider(PathUtils.Combine(settingsManager.ContentRootPath, "assets"))
            //        });
            //    });
            //}

            app.Use(async (context, next) =>
            {
                await next();
                if (context.Response.StatusCode == 404)
                {
                    context.Request.Path = "/404.html";
                    await next();
                }
            });
            app.UseStaticFiles();

            var supportedCultures = new[]
            {
                new CultureInfo("en-US"),
                new CultureInfo("zh-CN")
            };

            app.UseRequestLocalization(new RequestLocalizationOptions
            {
                DefaultRequestCulture = new RequestCulture("zh-CN"),
                // Formatting numbers, dates, etc.
                SupportedCultures = supportedCultures,
                // UI strings that we have localized.
                SupportedUICultures = supportedCultures
            });

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UsePluginsAsync(settingsManager, pluginManager).GetAwaiter().GetResult();

            // 启动安全检查：迁移可逆管理员口令、提示弱 SecurityKey（GJ20260924）
            _ = Task.Run(() => SSCMS.Web.StartupSecurityTasks.RunAsync(app.ApplicationServices));

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapRazorPages();
            });
            //api.UseEndpoints(endpoints => { endpoints.MapControllerRoute("default", "{controller}/{action}/{id?}"); });

            app.UseRequestLocalization();

            RegisterService.Start(senparcSetting.Value)
                //自动扫描自定义扩展缓存（二选一）
                .UseSenparcGlobal(true)
                //ָ指定自定义扩展缓存（二选一）
                //.UseSenparcGlobal(false, () => GetExCacheStrategies(senparcSetting.Value))   
                ;

            // if (!settingsManager.IsSafeMode)
            // {
            //     app.UseOpenApi();
            //     app.UseSwaggerUi3();
            //     app.UseReDoc(settings =>
            //     {
            //         settings.Path = "/api/docs";
            //         settings.DocumentPath = "/swagger/v1/swagger.json";
            //     }); 
            // }
        }
    }
}
