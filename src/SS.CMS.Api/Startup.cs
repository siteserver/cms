using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NSwag;
using SS.CMS.Api.Common;
using SS.CMS.Core.Services;
using SS.CMS.Utils;

namespace SS.CMS.Api
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
            services.Configure<KestrelServerOptions>(options =>
            {
                options.AllowSynchronousIO = true;
            });

            services.AddMemoryCache();

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
            services.AddScoped(provider =>
            {
                var httpContext = provider.GetRequiredService<IHttpContextAccessor>().HttpContext;
                return new Response(httpContext);
            });

            AdminRoutes.AddSingleton(services);
            V2Routes.AddSingleton(services);

            services.AddOpenApiDocument(config =>
            {
                config.PostProcess = document =>
                {
                    document.Info.Version = "v2";
                    document.Info.Title = "SS CMS API";
                    document.Info.Description = "A simple ASP.NET Core web API";
                    document.Info.TermsOfService = "None";
                    document.Info.Contact = new OpenApiContact
                    {
                        Name = "SS CMS",
                        Url = "https://www.sscms.com/docs/"
                    };
                };
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

            //app.UseRouting();

            //app.UseAuthorization();

            //app.UseEndpoints(endpoints =>
            //{
            //    endpoints.MapControllers();
            //});

            AppSettings.LoadJson(env.ContentRootPath, env.WebRootPath, PathUtils.Combine(env.ContentRootPath, AppSettings.AppSettingsFileName));

            // PluginManager.Load(() =>
            // {
            //     var httpContext = app.ApplicationServices.GetRequiredService<IHttpContextAccessor>().HttpContext;
            //     return new Request(httpContext);
            // });

            app.Map("/" + AppSettings.ApiPrefix.Trim('/'), mainApp =>
            {
                mainApp.Map("/ping", map => map.Run(async
                    ctx => await ctx.Response.WriteAsync("pong")));

                mainApp.UseRouting();

                mainApp.UseAuthorization();

                mainApp.UseEndpoints(endpoints =>
                {
                    endpoints.MapControllers();
                });

                mainApp.UseOpenApi();
                mainApp.UseSwaggerUi3();
                mainApp.UseReDoc(options =>
                {
                    options.Path = "/docs";
                    options.DocumentPath = "/swagger/v1/swagger.json";
                });
            });

            app.UseStaticFiles();


        }
    }
}