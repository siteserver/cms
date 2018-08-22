using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SiteServer.Plugin;
using Menu = SiteServer.Plugin.Menu;

namespace SiteServer.CMS.Plugin.Model
{
    public class ServiceImpl: IService
    {
        public string PluginId { get; }

        public IMetadata Metadata { get; }

        public Menu PluginMenu { get; private set; }
        public Func<int, Menu> SiteMenuFunc { get; private set; }
        public string ContentTableName { get; private set; }
        public bool IsApiAuthorization { get; private set; }

        public List<TableColumn> ContentTableColumns { get; private set; }
        public Dictionary<string, List<TableColumn>> DatabaseTables { get; private set; }
        public List<Menu> ContentMenus { get; private set; }

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
            return ContentFormLoad?.Invoke(this, e);
        }

        public event EventHandler<ContentFormSubmitEventArgs> ContentFormSubmit;

        public void OnContentFormSubmit(ContentFormSubmitEventArgs e)
        {
            ContentFormSubmit?.Invoke(this, e);
        }

        public Dictionary<string, Func<IParseContext, string>> StlElementsToParse { get; private set; }

        public Dictionary<string, Func<IJobContext, Task>> Jobs { get; private set; }

        public Dictionary<string, Func<IContentContext, string>> ContentColumns { get; private set; }

        public ServiceImpl(IMetadata metadata)
        {
            PluginId = metadata.Id;
            Metadata = metadata;
        }

        public IService AddSystemMenu(Menu menu)
        {
            PluginMenu = menu;
            return this;
        }

        public IService AddSiteMenu(Func<int, Menu> siteMenuFunc)
        {
            SiteMenuFunc = siteMenuFunc;
            return this;
        }

        public IService AddContentModel(string tableName, List<TableColumn> tableColumns)
        {
            ContentTableName = tableName;
            ContentTableColumns = tableColumns;

            return this;
        }

        public IService AddDatabaseTable(string tableName, List<TableColumn> tableColumns)
        {
            if (DatabaseTables == null)
            {
                DatabaseTables = new Dictionary<string, List<TableColumn>>();
            }

            DatabaseTables[tableName] = tableColumns;

            return this;
        }

        public IService AddContentMenu(Menu link)
        {
            if (ContentMenus == null)
            {
                ContentMenus = new List<Menu>();
            }

            ContentMenus.Add(link);

            return this;
        }

        public IService AddContentColumn(string columnName, Func<IContentContext, string> columnFunc)
        {
            if (ContentColumns == null)
            {
                ContentColumns = new Dictionary<string, Func<IContentContext, string>>();
            }

            ContentColumns[columnName] = columnFunc;

            return this;
        }

        public IService AddStlElementParser(string elementName, Func<IParseContext, string> parse)
        {
            if (StlElementsToParse == null)
            {
                StlElementsToParse = new Dictionary<string, Func<IParseContext, string>>();
            }

            StlElementsToParse[elementName] = parse;

            return this;
        }

        public IService AddJob(string command, Func<IJobContext, Task> job)
        {
            if (Jobs == null)
            {
                Jobs = new Dictionary<string, Func<IJobContext, Task>>(StringComparer.CurrentCultureIgnoreCase);
            }

            Jobs[command] = job;

            return this;
        }

        public IService AddApiAuthorization()
        {
            IsApiAuthorization = true;

            return this;
        }

        public event RestApiEventHandler RestApiGet;
        public event RestApiEventHandler RestApiPost;
        public event RestApiEventHandler RestApiPut;
        public event RestApiEventHandler RestApiDelete;

        public object OnRestApiGet(RestApiEventArgs e)
        {
            return RestApiGet?.Invoke(this, e);
        }

        public object OnRestApiPost(RestApiEventArgs e)
        {
            return RestApiPost?.Invoke(this, e);
        }

        public object OnRestApiPut(RestApiEventArgs e)
        {
            return RestApiPut?.Invoke(this, e);
        }

        public object OnRestApiDelete(RestApiEventArgs e)
        {
            return RestApiDelete?.Invoke(this, e);
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
