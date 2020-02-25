using System;
using System.IO;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json.Serialization;
using SS.CMS.Abstractions;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.FileProviders;
using SS.CMS.Core;
using SS.CMS.Extensions;

namespace SS.CMS.Web
{
    public class Startup
    {
        private readonly IWebHostEnvironment _env;
        private readonly IConfiguration _config;
        private ISettingsManager _settingsManager;

        public Startup(IWebHostEnvironment env, IConfiguration config)
        {
            _env = env;
            _config = config;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            _settingsManager = services.AddSettingsManager(_config, _env.ContentRootPath, _env.WebRootPath);

            GlobalSettings.Load(_settingsManager);

            services.AddRazorPages(config => { config.RootDirectory = $"/{Constants.AdminRootDirectory}"; });

            services.AddHttpContextAccessor();

            services.Configure<FormOptions>(options =>
            {
                options.MultipartBodyLengthLimit = 60000000;//60MB
            });

            services.AddCache(_settingsManager.Redis.ConnectionString);

            services.AddRepositories();
            services.AddServices();

            services.AddControllers()
                .AddNewtonsoftJson(options =>
                {
                    options.SerializerSettings.DateFormatString = "yyyy-MM-dd HH:mm:ss";
                    options.SerializerSettings.ContractResolver
                        = new CamelCasePropertyNamesContractResolver();
                });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
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

            app.UseHttpsRedirection();

            app.UseStaticFiles();
            app.UseDefaultFiles();

            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(
                    Path.Combine(Directory.GetCurrentDirectory(), $"{Constants.AdminRootDirectory}/assets")),
                RequestPath = "/" + _settingsManager.AdminDirectory + "/assets"
            });

            app.Map("/" + _settingsManager.AdminDirectory, admin =>
            {
                admin.UseRouting();
                admin.UseEndpoints(endpoints => { endpoints.MapRazorPages(); });
            });

            app.Map(Constants.ApiPrefix, api =>
            {
                api.UseRouting();

                api.UseAuthentication();
                api.UseAuthorization();

                api.UseEndpoints(endpoints => { endpoints.MapControllers(); });

                api.UseOpenApi();
                api.UseSwaggerUi3();
                api.UseReDoc(options =>
                {
                    options.Path = "/docs";
                    options.DocumentPath = "/swagger/v1/swagger.json";
                });
            });
        }
    }
}
