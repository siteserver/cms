using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json.Serialization;
using SSCMS.Core.Extensions;
using SSCMS.Services;
using SSCMS.Utils;

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
            var entryAssembly = Assembly.GetExecutingAssembly();
            var assemblies = new List<Assembly> { entryAssembly }.Concat(entryAssembly.GetReferencedAssemblies().Select(Assembly.Load));

            var settingsManager = services.AddSettingsManager(_config, _env.ContentRootPath, _env.WebRootPath, entryAssembly, assemblies);
            var pluginManager = services.AddPlugins(settingsManager);

            services.AddCors(options =>
            {
                options.AddPolicy(CorsPolicy,
                    builder => builder
                        .AllowAnyMethod()
                        .AllowAnyHeader()
                        .SetIsOriginAllowed(_ => true)
                        .AllowCredentials()
                );
            });

            services.AddHttpContextAccessor();

            var key = Encoding.ASCII.GetBytes(settingsManager.SecurityKey);
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
                        IssuerSigningKey = new SymmetricSecurityKey(key),
                        ValidateIssuer = false,
                        ValidateAudience = false
                    };
                });

            services.Configure<FormOptions>(options =>
            {
                options.MultipartBodyLengthLimit = 524288000;//500MB
            });

            services.AddHealthChecks();
            services.AddRazorPages()
                .AddPluginApplicationParts(pluginManager)
                .SetCompatibilityVersion(CompatibilityVersion.Latest);

            services.AddCache(settingsManager.Redis.ConnectionString);

            services.AddRepositories(assemblies);
            services.AddServices();

            services.AddLocalization(options => options.ResourcesPath = "Resources");
            services
                .AddControllers()
                .AddViewLocalization(LanguageViewLocationExpanderFormat.Suffix)
                .AddDataAnnotationsLocalization()
                .AddNewtonsoftJson(options =>
                {
                    options.SerializerSettings.DateFormatString = "yyyy-MM-dd HH:mm:ss";
                    options.SerializerSettings.ContractResolver
                        = new CamelCasePropertyNamesContractResolver();
                });

            services.Configure<RequestLocalizationOptions>(options =>
            {
                var supportedCultures = new List<CultureInfo>
                {
                    new CultureInfo("en-US"),
                    new CultureInfo("zh-CN")
                };

                options.DefaultRequestCulture = new RequestCulture("zh-CN");
                options.SupportedCultures = supportedCultures;
                options.SupportedUICultures = supportedCultures;
            });

            //http://localhost:5000/api/swagger/v1/swagger.json
            //http://localhost:5000/api/swagger/
            //http://localhost:5000/api/docs/
            services.AddOpenApiDocument(config =>
            {
                config.PostProcess = document =>
                {
                    document.Info.Version = "v1";
                    document.Info.Title = "SS CMS REST API";
                    document.Info.Description = "SS CMS REST API 为 SS CMS 提供了一个基于HTTP的API调用，允许开发者通过发送和接收JSON对象来远程与站点进行交互。";
                    document.Info.Contact = new NSwag.OpenApiContact
                    {
                        Name = "SS CMS",
                        Email = string.Empty,
                        Url = "https://www.siteserver.cn"
                    };
                    document.Info.License = new NSwag.OpenApiLicense
                    {
                        Name = "GPL-3.0",
                        Url = "https://github.com/siteserver/cms/blob/staging/LICENSE"
                    };
                };
            });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IPluginManager pluginManager)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseExceptionHandler(a => a.Run(async context =>
            {
                var exceptionHandlerPathFeature = context.Features.Get<IExceptionHandlerPathFeature>();
                var exception = exceptionHandlerPathFeature.Error;

                var result = TranslateUtils.JsonSerialize(new
                {
                    exception.Message,
                    exception.StackTrace,
                    AddDate = DateTime.Now
                });
                context.Response.ContentType = "application/json";
                await context.Response.WriteAsync(result);
            }));

            app.UseCors(CorsPolicy);

            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
            });

            //app.UseHttpsRedirection();

            app.UseDefaultFiles(new DefaultFilesOptions
            {
                DefaultFileNames = new List<string>
                {
                    "index.html"
                }
            });
            app.UseStaticFiles();
            app.Map("/ss-admin/assets", assets =>
            {
                var dir = "wwwroot/SiteFiles/assets";
                if (env.IsDevelopment())
                {
                    dir = "assets";
                }

                assets.UseStaticFiles(new StaticFileOptions
                {
                    FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), dir))
                });
            });

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

            app.UsePlugins(pluginManager);

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGet("/xx", async context =>
                {
                    await context.Response.WriteAsync("Hello World!");
                });

                endpoints.MapHealthChecks("/healthz");

                endpoints.MapControllers();
                endpoints.MapRazorPages();
            });
            //api.UseEndpoints(endpoints => { endpoints.MapControllerRoute("default", "{controller}/{action}/{id?}"); });

            app.UseRequestLocalization();

            app.UseOpenApi();
            app.UseSwaggerUi3();
            app.UseReDoc(options =>
            {
                options.Path = "/docs";
                options.DocumentPath = "/swagger/v1/swagger.json";
            });
        }

        //public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ISettingsManager settingsManager)
        //{
        //    if (env.IsDevelopment())
        //    {
        //        app.UseDeveloperExceptionPage();
        //    }

        //    app.UseExceptionHandler(a => a.Run(async context =>
        //    {
        //        var exceptionHandlerPathFeature = context.Features.Get<IExceptionHandlerPathFeature>();
        //        var exception = exceptionHandlerPathFeature.Error;

        //        var result = TranslateUtils.JsonSerialize(new
        //        {
        //            exception.Message,
        //            exception.StackTrace,
        //            AddDate = DateTime.Now
        //        });
        //        context.Response.ContentType = "application/json";
        //        await context.Response.WriteAsync(result);
        //    }));

        //    app.UseCors(CorsPolicy);

        //    app.UseForwardedHeaders(new ForwardedHeadersOptions
        //    {
        //        ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
        //    });

        //    //app.UseHttpsRedirection();

        //    app.UseDefaultFiles(new DefaultFilesOptions
        //    {
        //        DefaultFileNames = new List<string>
        //        {
        //            "index.html"
        //        }
        //    });
        //    app.UseStaticFiles();

        //    app.Map("/admin/assets", assets =>
        //    {
        //        assets.UseStaticFiles(new StaticFileOptions
        //        {
        //            FileProvider = new PhysicalFileProvider(
        //                Path.Combine(Directory.GetCurrentDirectory(), "assets"))
        //        });
        //    });

        //    var supportedCultures = new[]
        //    {
        //        new CultureInfo("en-US"),
        //        new CultureInfo("zh-CN")
        //    };

        //    app.Map("/" + settingsManager.AdminDirectory, admin =>
        //    {
        //        admin.UseRequestLocalization(new RequestLocalizationOptions
        //        {
        //            DefaultRequestCulture = new RequestCulture("zh-CN"),
        //            // Formatting numbers, dates, etc.
        //            SupportedCultures = supportedCultures,
        //            // UI strings that we have localized.
        //            SupportedUICultures = supportedCultures
        //        });

        //        admin.UseRouting();
        //        admin.UseEndpoints(endpoints => { endpoints.MapRazorPages(); });

        //        admin.UseRequestLocalization();
        //    });

        //    app.Map(Constants.ApiPrefix, api =>
        //    {
        //        api.UseRequestLocalization(new RequestLocalizationOptions
        //        {
        //            DefaultRequestCulture = new RequestCulture("zh-CN"),
        //            // Formatting numbers, dates, etc.
        //            SupportedCultures = supportedCultures,
        //            // UI strings that we have localized.
        //            SupportedUICultures = supportedCultures
        //        });

        //        api.UseRouting();

        //        api.UseAuthentication();
        //        api.UseAuthorization();

        //        api.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        //        //api.UseEndpoints(endpoints => { endpoints.MapControllerRoute("default", "{controller}/{action}/{id?}"); });

        //        api.UseRequestLocalization();

        //        api.UseOpenApi();
        //        api.UseSwaggerUi3();
        //        api.UseReDoc(options =>
        //        {
        //            options.Path = "/docs";
        //            options.DocumentPath = "/swagger/v1/swagger.json";
        //        });
        //    });

        //    app.UsePlugins();
        //}
    }
}
