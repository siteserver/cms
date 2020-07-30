using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Datory;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging;
using SSCMS.Configuration;
using SSCMS.Models;
using SSCMS.Plugins;
using SSCMS.Services;
using SSCMS.Utils;

namespace SSCMS.Core.Plugins.Extensions
{
    public static class PluginApplicationBuilderExtensions
    {
        public static async Task UsePluginsAsync(this IApplicationBuilder app, ISettingsManager settingsManager,
            IPluginManager pluginManager)
        {
            var logger = app.ApplicationServices.GetService<ILoggerFactory>()
                .CreateLogger<IApplicationBuilder>();

            foreach (var plugin in pluginManager.Plugins)
            {
                if (plugin.Disabled) continue;

                logger.LogInformation("Using Plugin '{0}'", plugin.PluginId);

                DirectoryUtils.CreateDirectoryIfNotExists(plugin.WebRootPath);

                var fileProvider = new PhysicalFileProvider(plugin.WebRootPath);
                app.UseStaticFiles(
                    new StaticFileOptions
                    {
                        FileProvider = fileProvider
                    });
            }

            var configures = pluginManager.GetExtensions<IPluginConfigure>();
            if (configures != null)
            {
                foreach (var configure in configures)
                {
                    configure.Configure(app);
                }
            }

            var middlewares = pluginManager.GetExtensions<IPluginMiddleware>();
            if (middlewares != null)
            {
                foreach (var middleware in middlewares)
                {
                    app.Use(next => async context => await middleware.UseAsync(next, context));
                }
            }

            var database = settingsManager.Database;

            var tables = settingsManager.GetTables();
            foreach (var table in tables.Where(table => !string.IsNullOrEmpty(table.Id)))
            {
                List<TableColumn> columns;
                if (table.Type == TableType.Custom)
                {
                    columns = table.Columns;
                }
                else if (table.Type == TableType.Content)
                {
                    columns = database.GetTableColumns(null);
                    columns.AddRange(database.GetTableColumns<Content>());
                    if (table.Columns != null)
                    {
                        foreach (var tableColumn in table.Columns.Where(tableColumn =>
                            !columns.Any(x => StringUtils.EqualsIgnoreCase(x.AttributeName, tableColumn.AttributeName))))
                        {
                            columns.Add(tableColumn);
                        }
                    }
                }
                else
                {
                    columns = database.GetTableColumns(table.Columns);
                }

                if (columns == null || columns.Count == 0) continue;

                try
                {
                    logger.LogInformation("Sync Plugin Table '{0}'", table.Id);
                    if (!await database.IsTableExistsAsync(table.Id))
                    {
                        await database.CreateTableAsync(table.Id, columns);
                    }
                    else
                    {
                        await database.AlterTableAsync(table.Id, columns);
                    }
                }
                catch
                {
                    // ignored
                }
            }
        }
    }
}
