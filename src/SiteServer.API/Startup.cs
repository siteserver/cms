using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SiteServer.API.Common;
using SiteServer.CMS.Controllers.Admin;
using SiteServer.CMS.Plugin;
using SiteServer.Utils;

namespace SiteServer.API
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCors(options =>
                {
                    options.AddPolicy(
                        "AllowAny",
                        x =>
                        {
                            x.AllowAnyHeader()
                            .AllowAnyMethod()
                            .SetIsOriginAllowed(isOriginAllowed: _ => true)
                            .AllowCredentials();
                        });
                });

            services.AddControllers()
                .AddNewtonsoftJson();

            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddScoped(provider =>
            {
                var httpContext = provider.GetRequiredService<IHttpContextAccessor>().HttpContext;
                return new Request(httpContext);
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseCors("AllowAny");

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            app.UseStaticFiles();

            var applicationPath = "/";
            var applicationPhysicalPath = env.WebRootPath;
            AppSettings.LoadJson(applicationPath, applicationPhysicalPath, PathUtils.Combine(env.ContentRootPath, AppSettings.AppSettingsFileName));

            // PluginManager.Load(() =>
            // {
            //     var httpContext = app.ApplicationServices.GetRequiredService<IHttpContextAccessor>().HttpContext;
            //     return new Request(httpContext);
            // });
        }
    }
}