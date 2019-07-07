using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using NSwag;
using SS.CMS.Core.Services;
using SS.CMS.Services;
using SS.CMS.Utils;

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

            var settingsManager = services.AddSettingsManager(_config, _env.ContentRootPath, _env.WebRootPath);

            services.AddDistributedCache(settingsManager.CacheType, settingsManager.CacheConnectionString);
            services.AddRepositories();
            services.AddPathManager();
            services.AddPluginManager();
            services.AddUrlManager();
            services.AddFileManager();
            services.AddCreateManager();
            services.AddTableManager();

            services.AddScoped<IUserManager, UserManager>();

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = "yourdomain.com",
                        ValidAudience = "yourdomain.com",
                        IssuerSigningKey = new SymmetricSecurityKey(
                            Encoding.UTF8.GetBytes(settingsManager.SecurityKey))
                    };
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

            // In production, the VueJs files will be served from this directory
            services.AddSpaStaticFiles(configuration =>
            {
                configuration.RootPath = "admin";
            });

            services.AddControllers()
                .AddNewtonsoftJson();
            // services.AddApiVersioning(o =>
            // {
            //     o.ReportApiVersions = true;
            //     o.AssumeDefaultVersionWhenUnspecified = true;
            //     o.DefaultApiVersion = new ApiVersion(1, 0);
            // });

            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

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
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseCors("AllowAny");

            app.UseHttpsRedirection();

            app.UseStaticFiles();
            app.UseSpaStaticFiles();

            app.Map(Constants.ApiPrefix, api =>
            {
                api.Map("/ping", map => map.Run(async
                    ctx => await ctx.Response.WriteAsync("pong")));

                api.UseRouting();

                api.UseAuthentication();
                api.UseAuthorization();

                api.UseEndpoints(endpoints =>
                {
                    endpoints.MapControllers();
                });

                api.UseOpenApi();
                api.UseSwaggerUi3();
                api.UseReDoc(options =>
                {
                    options.Path = "/docs";
                    options.DocumentPath = "/swagger/v1/swagger.json";
                });
            });

            // var adminMatch = string.IsNullOrEmpty(settingsManager.AdminPrefix) ? string.Empty : $"/{settingsManager.AdminPrefix}";
            // app.Map(adminMatch, admin =>
            // {
            //     admin.UseRouting();

            //     admin.UseSpa(spa =>
            //     {
            //         spa.Options.SourcePath = "ClientApp";
            //         if (_env.IsDevelopment())
            //         {
            //             spa.UseProxyToSpaDevelopmentServer("http://localhost:3000");
            //         }
            //     });
            // });
        }
    }
}