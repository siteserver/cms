using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NSwag;
using SS.CMS.Abstractions.Services;
using SS.CMS.Core.Services;

namespace SS.CMS.Api
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

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<KestrelServerOptions>(options =>
            {
                options.AllowSynchronousIO = true;
            });

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
            //services.AddScoped(provider =>
            //{
            //    var httpContext = provider.GetRequiredService<IHttpContextAccessor>().HttpContext;
            //    return new Request(httpContext, null);
            //});
            //services.AddScoped(provider =>
            //{
            //    var httpContext = provider.GetRequiredService<IHttpContextAccessor>().HttpContext;
            //    return new Response(httpContext);
            //});

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

            //services.Configure<AppSettings>(_config.GetSection("SS"));
            //services.AddScoped(sp => sp.GetRequiredService<IOptionsSnapshot<AppSettings>>().Value);
            // services.AddScoped<IDb>(sp =>
            // {
            //     var appSettings = sp.GetRequiredService<IOptionsSnapshot<AppSettings>>().Value;

            //     if (string.IsNullOrEmpty(appSettings.SecretKey))
            //     {
            //         appSettings.SecretKey = StringUtils.GetShortGuid();
            //     }

            //     DatabaseType databaseType;
            //     string connectionString;
            //     if (appSettings.IsProtectData)
            //     {
            //         databaseType = DatabaseType.GetDatabaseType(TranslateUtils.DecryptStringBySecretKey(appSettings.Database.Type, appSettings.SecretKey));
            //         connectionString = TranslateUtils.DecryptStringBySecretKey(appSettings.Database.ConnectionString, appSettings.SecretKey);
            //     }
            //     else
            //     {
            //         databaseType = DatabaseType.GetDatabaseType(appSettings.Database.Type);
            //         connectionString = appSettings.Database.ConnectionString;
            //     }

            //     return new Db(databaseType, connectionString);
            // });

            //AppContext.Load(_env.ContentRootPath, _env.WebRootPath, _config);
            //AppContext.Db = db;

            services.AddMemoryCache();
            services.AddDistributedMemoryCache();

            services.AddSettingsManager(_config, _env.ContentRootPath, _env.WebRootPath);
            services.AddCacheManager();
            services.AddRepositories();
            services.AddPathManager();
            services.AddPluginManager();
            services.AddUrlManager();
            services.AddMenuManager();
            services.AddFileManager();
            services.AddCreateManager();
            services.AddIdentityManager();

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, ISettingsManager settingsManager)
        {
            if (_env.IsDevelopment())
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

            // PluginManager.Load(() =>
            // {
            //     var httpContext = app.ApplicationServices.GetRequiredService<IHttpContextAccessor>().HttpContext;
            //     return new Request(httpContext);
            // });

            app.Map("/" + settingsManager.ApiPrefix.Trim('/'), mainApp =>
            {
                mainApp.Map("/ping", map => map.Run(async
                    ctx => await ctx.Response.WriteAsync("pong")));

                mainApp.UseRouting();

                mainApp.UseAuthorization();

                mainApp.UseEndpoints(endpoints =>
                {
                    endpoints.MapControllers();
                });

                //mainApp.UseAuthentication();
                //mainApp.UseAuthorization();

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