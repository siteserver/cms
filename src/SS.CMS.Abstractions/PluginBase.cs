using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Datory;
using SS.CMS.Abstractions.Plugins;

namespace SS.CMS.Abstractions
{
    public abstract class PluginBase : IPlugin
    {
        public string PluginId => AssemblyUtils.GetPluginId(GetType());

        public virtual string Name => AssemblyUtils.GetPluginName(GetType());

        public virtual string Version => AssemblyUtils.GetPluginVersion(GetType());

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

        public List<TableColumn> ContentTableColumns { get; private set; }

        public List<InputStyle> ContentInputStyles { get; private set; }

        public Dictionary<string, List<TableColumn>> DatabaseTables { get; private set; }

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

        public Dictionary<string, Func<IParseContext, string>> StlElementsToParse { get; private set; }

        public Dictionary<string, Func<IJobContext, Task>> Jobs { get; private set; }

        public Dictionary<string, Func<IContentContext, string>> ContentColumns { get; private set; }

        public IPlugin SetSystemDefaultPage(string pageUrl)
        {
            SystemDefaultPageUrl = pageUrl;
            return this;
        }

        public IPlugin SetHomeDefaultPage(string pageUrl)
        {
            HomeDefaultPageUrl = pageUrl;
            return this;
        }

        public IPlugin AddContentModel(string tableName, List<TableColumn> tableColumns, List<InputStyle> inputStyles)
        {
            ContentTableName = tableName;
            ContentTableColumns = tableColumns;
            ContentInputStyles = inputStyles;

            return this;
        }

        public IPlugin AddDatabaseTable(string tableName, List<TableColumn> tableColumns)
        {
            if (DatabaseTables == null)
            {
                DatabaseTables = new Dictionary<string, List<TableColumn>>();
            }

            DatabaseTables[tableName] = tableColumns;

            return this;
        }

        public IPlugin AddContentColumn(string columnName, Func<IContentContext, string> columnFunc)
        {
            if (ContentColumns == null)
            {
                ContentColumns = new Dictionary<string, Func<IContentContext, string>>();
            }

            ContentColumns[columnName] = columnFunc;

            return this;
        }

        public IPlugin AddStlElementParser(string elementName, Func<IParseContext, string> parse)
        {
            if (StlElementsToParse == null)
            {
                StlElementsToParse = new Dictionary<string, Func<IParseContext, string>>();
            }

            StlElementsToParse[elementName] = parse;

            return this;
        }

        public IPlugin AddJob(string command, Func<IJobContext, Task> job)
        {
            if (Jobs == null)
            {
                Jobs = new Dictionary<string, Func<IJobContext, Task>>(StringComparer.CurrentCultureIgnoreCase);
            }

            Jobs[command] = job;

            return this;
        }

        public IPlugin AddApiAuthorization()
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
