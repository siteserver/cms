using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json.Serialization;
using SSCMS;
using SSCMS.Plugins;
using SSCMS.Core.Extensions;
using SSCMS.Utils;

namespace SSCMS.Web
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
            var settingsManager = services.AddSettingsManager(_config, _env.ContentRootPath, _env.WebRootPath);

            services.AddRazorPages(config => { config.RootDirectory = $"/{Constants.AdminRootDirectory}"; });

            services.AddHttpContextAccessor();

            services.Configure<FormOptions>(options =>
            {
                options.MultipartBodyLengthLimit = 524288000;//500MB
            });

            services.AddCache(settingsManager.Redis.ConnectionString);

            var executingAssembly = Assembly.GetExecutingAssembly();
            var assemblies = executingAssembly.GetReferencedAssemblies().Select(Assembly.Load).ToList();
            var path = AppDomain.CurrentDomain.RelativeSearchPath ?? AppDomain.CurrentDomain.BaseDirectory;
            var fileAssemblies = Directory.GetFiles(path, $"{nameof(SSCMS)}*.dll").Select(Assembly.LoadFrom).ToArray();
            foreach (var referencedAssembly in fileAssemblies)
            {
                if (!assemblies.Contains(referencedAssembly))
                {
                    assemblies.Add(referencedAssembly);
                }
            }
            if (!assemblies.Contains(executingAssembly))
            {
                assemblies.Add(executingAssembly);
            }
            AssemblyUtils.SetAssemblies(assemblies);

            services.AddRepositories();
            services.AddServices();
            services.AddPlugins();

            services.AddControllers()
                .AddNewtonsoftJson(options =>
                {
                    options.SerializerSettings.DateFormatString = "yyyy-MM-dd HH:mm:ss";
                    options.SerializerSettings.ContractResolver
                        = new CamelCasePropertyNamesContractResolver();
                });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IServiceProvider provider)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            var settingsManager = provider.GetRequiredService<ISettingsManager>();

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

            //app.UseHttpsRedirection();

            app.UseDefaultFiles(new DefaultFilesOptions
            {
                DefaultFileNames = new List<string>
                {
                    "index.html"
                }
            });
            app.UseStaticFiles();

            //app.Map("/" + settingsManager.AdminDirectory + "/assets", admin =>
            //{
            //    admin.UseStaticFiles(new StaticFileOptions
            //    {
            //        FileProvider = new PhysicalFileProvider(
            //            Path.Combine(Directory.GetCurrentDirectory(), "assets"))
            //    });
            //});

            app.Map("/" + settingsManager.AdminDirectory, admin =>
            {
                admin.UseRouting();
                admin.UseEndpoints(endpoints => { endpoints.MapRazorPages(); });
            });

            app.Map(Constants.ApiPrefix, api =>
            {
                api.UseRouting();
                api.UseAuthentication();
                api.UseEndpoints(endpoints => { endpoints.MapControllers(); });

                api.UseOpenApi();
                api.UseSwaggerUi3();
                api.UseReDoc(options =>
                {
                    options.Path = "/docs";
                    options.DocumentPath = "/swagger/v1/swagger.json";
                });
            });

            app.UsePlugins();
        }
    }
}
