using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Datory;
using SSCMS.Configuration;
using SSCMS.Models;
using SSCMS.Utils;

namespace SSCMS
{
    public abstract class OldPluginBase : IOldPlugin
    {
        private static string GetPluginId(Type type)
        {
            var assemblyName = type.Assembly.GetName();
            return assemblyName.Name;
        }

        private static string GetPluginName(Type type)
        {
            var name = GetPluginId(type);
            return StringUtils.Contains(name, ".") ? name.Substring(name.LastIndexOf('.') + 1) : name;
        }

        private static string GetPluginVersion(Type type)
        {
            var assemblyName = type.Assembly.GetName();
            if (assemblyName.Version == null)
            {
                return "1.0.0";
            }

            return StringUtils.TrimEnd(assemblyName.Version.ToString(), ".0");
        }

        public string PluginId => GetPluginId(GetType());

        public virtual string Name => GetPluginName(GetType());

        public virtual string Version => GetPluginVersion(GetType());

        public virtual string IconUrl => null;

        public virtual string ProjectUrl => null;

        public virtual string LicenseUrl => null;

        public virtual string Copyright => null;

        public virtual string Description => null;

        public virtual string ReleaseNotes => null;

        public virtual string Tags => null;

        public virtual string Authors => null;

        public virtual string Owners => null;

        public virtual string Language => null;

        public string SystemDefaultPageUrl { get; private set; }
        public string HomeDefaultPageUrl { get; private set; }

        public virtual List<Menu> GetSystemMenus() => null;

        public virtual async Task<List<Menu>> GetSystemMenusAsync()
        {
            return await Task.FromResult<List<Menu>>(null);
        }

        public virtual List<Menu> GetSiteMenus(int siteId) => null;

        public virtual async Task<List<Menu>> GetSiteMenusAsync(int siteId)
        {
            return await Task.FromResult<List<Menu>>(null);
        }

        public virtual List<Menu> GetHomeMenus() => null;

        public virtual async Task<List<Menu>> GetHomeMenusAsync()
        {
            return await Task.FromResult<List<Menu>>(null);
        }

        public virtual List<Menu> GetContentMenus(Content content) => null;

        public virtual async Task<List<Menu>> GetContentMenusAsync(Content content)
        {
            return await Task.FromResult<List<Menu>>(null);
        }

        public string ContentTableName { get; private set; }
        public bool IsApiAuthorization { get; private set; }

        public List<Datory.TableColumn> ContentTableColumns { get; private set; }

        public List<InputStyle> ContentInputStyles { get; private set; }

        public Dictionary<string, List<Datory.TableColumn>> DatabaseTables { get; private set; }

        public event EventHandler<ContentEventArgs> ContentAddCompleted;

        public void OnContentAddCompleted(ContentEventArgs e)
        {
            ContentAddCompleted?.Invoke(this, e);
        }

        public event EventHandler<ContentEventArgs> ContentDeleteCompleted;

        public void OnContentDeleteCompleted(ContentEventArgs e)
        {
            ContentDeleteCompleted?.Invoke(this, e);
        }

        public event EventHandler<ContentTranslateEventArgs> ContentTranslateCompleted;

        public void OnContentTranslateCompleted(ContentTranslateEventArgs e)
        {
            ContentTranslateCompleted?.Invoke(this, e);
        }

        public event ContentFormLoadEventHandler ContentFormLoad;

        public string OnContentFormLoad(ContentFormLoadEventArgs e)
        {
            if (ContentFormLoad == null) return string.Empty;
            var html = ContentFormLoad.Invoke(this, e);
            return html;
        }

        public event EventHandler<ContentFormSubmitEventArgs> ContentFormSubmit;

        public void OnContentFormSubmit(ContentFormSubmitEventArgs e)
        {
            ContentFormSubmit?.Invoke(this, e);
        }

        public Dictionary<string, Func<IStlParseContext, string>> StlElementsToParse { get; private set; }

        public Dictionary<string, Func<IJobContext, Task>> Jobs { get; private set; }

        public Dictionary<string, Func<IContentContext, string>> ContentColumns { get; private set; }

        public IOldPlugin SetSystemDefaultPage(string pageUrl)
        {
            SystemDefaultPageUrl = pageUrl;
            return this;
        }

        public IOldPlugin SetHomeDefaultPage(string pageUrl)
        {
            HomeDefaultPageUrl = pageUrl;
            return this;
        }

        public IOldPlugin AddContentModel(string tableName, List<Datory.TableColumn> tableColumns, List<InputStyle> inputStyles)
        {
            ContentTableName = tableName;
            ContentTableColumns = tableColumns;
            ContentInputStyles = inputStyles;

            return this;
        }

        public IOldPlugin AddDatabaseTable(string tableName, List<Datory.TableColumn> tableColumns)
        {
            if (DatabaseTables == null)
            {
                DatabaseTables = new Dictionary<string, List<Datory.TableColumn>>();
            }

            DatabaseTables[tableName] = tableColumns;

            return this;
        }

        public IOldPlugin AddContentColumn(string columnName, Func<IContentContext, string> columnFunc)
        {
            if (ContentColumns == null)
            {
                ContentColumns = new Dictionary<string, Func<IContentContext, string>>();
            }

            ContentColumns[columnName] = columnFunc;

            return this;
        }

        public IOldPlugin AddStlElementParser(string elementName, Func<IStlParseContext, string> parse)
        {
            if (StlElementsToParse == null)
            {
                StlElementsToParse = new Dictionary<string, Func<IStlParseContext, string>>();
            }

            StlElementsToParse[elementName] = parse;

            return this;
        }

        public IOldPlugin AddJob(string command, Func<IJobContext, Task> job)
        {
            if (Jobs == null)
            {
                Jobs = new Dictionary<string, Func<IJobContext, Task>>(StringComparer.CurrentCultureIgnoreCase);
            }

            Jobs[command] = job;

            return this;
        }

        public IOldPlugin AddApiAuthorization()
        {
            IsApiAuthorization = true;

            return this;
        }

        public event EventHandler<ParseEventArgs> BeforeStlParse;
        public event EventHandler<ParseEventArgs> AfterStlParse;

        public void OnBeforeStlParse(ParseEventArgs e)
        {
            BeforeStlParse?.Invoke(this, e);
        }

        public void OnAfterStlParse(ParseEventArgs e)
        {
            AfterStlParse?.Invoke(this, e);
        }
    }
}
