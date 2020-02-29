using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Datory;
using SS.CMS.Abstractions;
using Menu = SS.CMS.Abstractions.Menu;

namespace SS.CMS.Core.Plugins
{
    public class PluginServiceImpl: IPluginService
    {
        public string PluginId { get; }

        public IPackageMetadata Metadata { get; }

        public string SystemDefaultPageUrl { get; private set; }
        public string HomeDefaultPageUrl { get; private set; }

        public List<Func<Menu>> SystemMenuFuncs { get; private set; }
        public List<Func<int, Menu>> SiteMenuFuncs { get; private set; }
        public List<Func<Menu>> HomeMenuFuncs { get; private set; }
        public List<Func<Content, Menu>> ContentMenuFuncs { get; private set; }

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

        public PluginServiceImpl(IPackageMetadata metadata)
        {
            PluginId = metadata.Id;
            Metadata = metadata;
        }

        public IPluginService SetSystemDefaultPage(string pageUrl)
        {
            SystemDefaultPageUrl = pageUrl;
            return this;
        }

        public IPluginService SetHomeDefaultPage(string pageUrl)
        {
            HomeDefaultPageUrl = pageUrl;
            return this;
        }

        public IPluginService AddSystemMenu(Func<Menu> menu)
        {
            if (SystemMenuFuncs == null)
            {
                SystemMenuFuncs = new List<Func<Menu>>();
            }
            SystemMenuFuncs.Add(menu);
            return this;
        }

        public IPluginService AddSiteMenu(Func<int, Menu> menuFunc)
        {
            if (SiteMenuFuncs == null)
            {
                SiteMenuFuncs = new List<Func<int, Menu>>();
            }
            SiteMenuFuncs.Add(menuFunc);
            return this;
        }

        public IPluginService AddHomeMenu(Func<Menu> menu)
        {
            if (HomeMenuFuncs == null)
            {
                HomeMenuFuncs = new List<Func<Menu>>();
            }
            HomeMenuFuncs.Add(menu);
            return this;
        }

        public IPluginService AddContentMenu(Func<Content, Menu> menuFunc)
        {
            if (ContentMenuFuncs == null)
            {
                ContentMenuFuncs = new List<Func<Content, Menu>>();
            }

            ContentMenuFuncs.Add(menuFunc);

            return this;
        }

        public IPluginService AddContentModel(string tableName, List<TableColumn> tableColumns, List<InputStyle> inputStyles)
        {
            ContentTableName = tableName;
            ContentTableColumns = tableColumns;
            ContentInputStyles = inputStyles;

            return this;
        }

        public IPluginService AddDatabaseTable(string tableName, List<TableColumn> tableColumns)
        {
            if (DatabaseTables == null)
            {
                DatabaseTables = new Dictionary<string, List<TableColumn>>();
            }

            DatabaseTables[tableName] = tableColumns;

            return this;
        }

        public IPluginService AddContentColumn(string columnName, Func<IContentContext, string> columnFunc)
        {
            if (ContentColumns == null)
            {
                ContentColumns = new Dictionary<string, Func<IContentContext, string>>();
            }

            ContentColumns[columnName] = columnFunc;

            return this;
        }

        public IPluginService AddStlElementParser(string elementName, Func<IParseContext, string> parse)
        {
            if (StlElementsToParse == null)
            {
                StlElementsToParse = new Dictionary<string, Func<IParseContext, string>>();
            }

            StlElementsToParse[elementName] = parse;

            return this;
        }

        public IPluginService AddJob(string command, Func<IJobContext, Task> job)
        {
            if (Jobs == null)
            {
                Jobs = new Dictionary<string, Func<IJobContext, Task>>(StringComparer.CurrentCultureIgnoreCase);
            }

            Jobs[command] = job;

            return this;
        }

        public IPluginService AddApiAuthorization()
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
