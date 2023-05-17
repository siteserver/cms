using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Datory;
using Datory.Utils;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging;
using SSCMS.Configuration;
using SSCMS.Models;
using SSCMS.Plugins;
using SSCMS.Repositories;
using SSCMS.Services;
using SSCMS.Utils;

namespace SSCMS.Core.Plugins.Extensions
{
    public static class PluginApplicationBuilderExtensions
    {
        public static async Task UsePluginsAsync(this IApplicationBuilder app, ISettingsManager settingsManager,
            IPluginManager pluginManager, IErrorLogRepository errorLogRepository)
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

            var database = settingsManager.Database;

            var tables = settingsManager.GetTables();
            foreach (var table in tables.Where(table => !string.IsNullOrEmpty(table.Id)))
            {
                List<TableColumn> columns;
                if (StringUtils.EqualsIgnoreCase(table.Type, Types.TableTypes.Custom))
                {
                    columns = table.Columns;
                }
                else if (StringUtils.EqualsIgnoreCase(table.Type, Types.TableTypes.Content))
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

                await SyncPluginTableAsync(database, table.Id, columns, logger);
            }

            var assemblies = pluginManager.EnabledPlugins.Select(x => x.Assembly);
            var entities = PluginUtils.GetImplementations<Entity>(assemblies);
            foreach (var entity in entities)
            {
                var tableName = ReflectionUtils.GetTableName(entity);
                if (tables.Exists(t => StringUtils.EqualsIgnoreCase(t.Id, tableName))) continue;

                var tableColumns = ReflectionUtils.GetTableColumns(entity);
                var columns = database.GetTableColumns(tableColumns);

                await SyncPluginTableAsync(database, tableName, columns, logger);
            }
        }

        private static async Task SyncPluginTableAsync(IDatabase database, string tableName, List<TableColumn> columns, ILogger<IApplicationBuilder> logger)
        {
            if (columns == null || columns.Count == 0) return;

            try
            {
                logger.LogInformation("Sync Plugin Table '{0}'", tableName);
                if (!await database.IsTableExistsAsync(tableName))
                {
                    await database.CreateTableAsync(tableName, columns);
                }
                else
                {
                    await database.AlterTableAsync(tableName, columns);
                }
            }
            catch
            {
                // ignored
            }
        }
    }
}
