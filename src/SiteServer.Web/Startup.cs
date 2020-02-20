using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json.Serialization;
using SiteServer.Abstractions;
using SiteServer.Web.Core;
using Microsoft.AspNetCore.Http.Features;
using SiteServer.CMS.Framework;

namespace SiteServer.Web
{
    public class Startup
    {
        private readonly IWebHostEnvironment _env;
        private readonly IConfiguration _config;

        public Startup(IWebHostEnvironment env, IConfiguration config)
        {
            _env = env;
            _config = config;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddRazorPages();

            services.AddHttpContextAccessor();

            //文件上传限制60MB
            services.Configure<FormOptions>(options =>
            {
                options.MultipartBodyLengthLimit = 60000000;
            });

            var settingsManager = services.AddSettingsManager(_config, _env.ContentRootPath, _env.WebRootPath);

            WebConfigUtils.Load(settingsManager);
            DataProvider.Load(settingsManager);

            services.AddCache(settingsManager.Redis.ConnectionString);

            services.AddRepositories();
            services.AddServices();

            services.AddControllers()
                .AddNewtonsoftJson(options =>
                {
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

            app.UseHttpsRedirection();

            app.UseStaticFiles();
            app.UseDefaultFiles();

            app.Map(Constants.AdminPrefix, admin =>
            {
                admin.UseRouting();
                admin.UseEndpoints(endpoints => { endpoints.MapRazorPages(); });
            });

            app.Map(Constants.ApiPrefix, api =>
            {
                api.Map("/ping", map => map.Run(async
                    ctx => await ctx.Response.WriteAsync("pong")));

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
