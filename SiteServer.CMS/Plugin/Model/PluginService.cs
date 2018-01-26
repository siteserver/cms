using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Web.UI.WebControls;
using SiteServer.Plugin;
using Menu = SiteServer.Plugin.Menu;

namespace SiteServer.CMS.Plugin.Model
{
    public class PluginService: IService
    {
        public string PluginId { get; private set; }

        public IMetadata Metadata { get; private set; }

        public Menu PluginMenu { get; private set; }
        public Func<int, Menu> SiteMenuFunc { get; private set; }
        public string ContentTableName { get; private set; }

        public List<TableColumn> ContentTableColumns { get; private set; }
        public Dictionary<string, List<TableColumn>> DatabaseTables { get; private set; }
        public List<HyperLink> ContentLinks { get; private set; }

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

        //public Dictionary<string, Func<int, int, IAttributes, string>> ContentFormCustomized { get; private set; }

        //public IService AddCustomizedContentForm(string attributeName, Func<int, int, IAttributes, string> customized)
        //{
        //    if (ContentFormCustomized == null)
        //    {
        //        ContentFormCustomized = new Dictionary<string, Func<int, int, IAttributes, string>>();
        //    }

        //    ContentFormCustomized[attributeName] = customized;

        //    return this;
        //}

        public event EventHandler<ContentFormLoadEventArgs> ContentFormLoad;

        public void OnContentFormLoad(ContentFormLoadEventArgs e)
        {
            ContentFormLoad?.Invoke(this, e);
        }

        public event EventHandler<ContentFormSubmitEventArgs> ContentFormSubmit;

        public void OnContentFormSubmit(ContentFormSubmitEventArgs e)
        {
            ContentFormSubmit?.Invoke(this, e);
        }

        public Dictionary<string, Func<IParseContext, string>> StlElementsToParse { get; private set; }
        public Func<IRequestContext, object> JsonGet { get; private set; }
        public Func<IRequestContext, string, object> JsonGetWithName { get; private set; }
        public Func<IRequestContext, string, string, object> JsonGetWithNameAndId { get; private set; }

        public Func<IRequestContext, object> JsonPost { get; private set; }
        public Func<IRequestContext, string, object> JsonPostWithName { get; private set; }
        public Func<IRequestContext, string, string, object> JsonPostWithNameAndId { get; private set; }

        public Func<IRequestContext, object> JsonPut { get; private set; }
        public Func<IRequestContext, string, object> JsonPutWithName { get; private set; }
        public Func<IRequestContext, string, string, object> JsonPutWithNameAndId { get; private set; }

        public Func<IRequestContext, object> JsonDelete { get; private set; }
        public Func<IRequestContext, string, object> JsonDeleteWithName { get; private set; }
        public Func<IRequestContext, string, string, object> JsonDeleteWithNameAndId { get; private set; }

        public Func<IRequestContext, HttpResponseMessage> HttpGet { get; private set; }
        public Func<IRequestContext, string, HttpResponseMessage> HttpGetWithName { get; private set; }
        public Func<IRequestContext, string, string, HttpResponseMessage> HttpGetWithNameAndId { get; private set; }

        public Func<IRequestContext, HttpResponseMessage> HttpPost { get; private set; }
        public Func<IRequestContext, string, HttpResponseMessage> HttpPostWithName { get; private set; }
        public Func<IRequestContext, string, string, HttpResponseMessage> HttpPostWithNameAndId { get; private set; }

        public Func<IRequestContext, HttpResponseMessage> HttpPut { get; private set; }
        public Func<IRequestContext, string, HttpResponseMessage> HttpPutWithName { get; private set; }
        public Func<IRequestContext, string, string, HttpResponseMessage> HttpPutWithNameAndId { get; private set; }

        public Func<IRequestContext, HttpResponseMessage> HttpDelete { get; private set; }
        public Func<IRequestContext, string, HttpResponseMessage> HttpDeleteWithName { get; private set; }
        public Func<IRequestContext, string, string, HttpResponseMessage> HttpDeleteWithNameAndId { get; private set; }

        public PluginService(IMetadata metadata)
        {
            PluginId = metadata.Id;
            Metadata = metadata;
        }

        public IService AddPluginMenu(Menu menu)
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

        public IService AddContentLinks(List<HyperLink> links)
        {
            ContentLinks = links;

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

        public IService AddJsonGet(Func<IRequestContext, object> jsonGet)
        {
            JsonGet = jsonGet;

            return this;
        }

        public IService AddJsonGet(Func<IRequestContext, string, object> jsonGetWithName)
        {
            JsonGetWithName = jsonGetWithName;

            return this;
        }

        public IService AddJsonGet(Func<IRequestContext, string, string, object> jsonGetWithNameAndId)
        {
            JsonGetWithNameAndId = jsonGetWithNameAndId;

            return this;
        }

        public IService AddJsonPost(Func<IRequestContext, object> jsonPost)
        {
            JsonPost = jsonPost;

            return this;
        }

        public IService AddJsonPost(Func<IRequestContext, string, object> jsonPostWithName)
        {
            JsonPostWithName = jsonPostWithName;

            return this;
        }

        public IService AddJsonPost(Func<IRequestContext, string, string, object> jsonPostWithNameAndId)
        {
            JsonPostWithNameAndId = jsonPostWithNameAndId;

            return this;
        }

        public IService AddJsonPut(Func<IRequestContext, object> jsonPut)
        {
            JsonPut = jsonPut;

            return this;
        }

        public IService AddJsonPut(Func<IRequestContext, string, object> jsonPutWithName)
        {
            JsonPutWithName = jsonPutWithName;

            return this;
        }

        public IService AddJsonPut(Func<IRequestContext, string, string, object> jsonPutWithNameAndId)
        {
            JsonPutWithNameAndId = jsonPutWithNameAndId;

            return this;
        }

        public IService AddJsonDelete(Func<IRequestContext, object> jsonDelete)
        {
            JsonDelete = jsonDelete;

            return this;
        }

        public IService AddJsonDelete(Func<IRequestContext, string, object> jsonDeleteWithName)
        {
            JsonDeleteWithName = jsonDeleteWithName;

            return this;
        }

        public IService AddJsonDelete(Func<IRequestContext, string, string, object> jsonDeleteWithNameAndId)
        {
            JsonDeleteWithNameAndId = jsonDeleteWithNameAndId;

            return this;
        }

        public IService AddHttpGet(Func<IRequestContext, HttpResponseMessage> httpGet)
        {
            HttpGet = httpGet;

            return this;
        }

        public IService AddHttpGet(Func<IRequestContext, string, HttpResponseMessage> httpGetWithName)
        {
            HttpGetWithName = httpGetWithName;

            return this;
        }

        public IService AddHttpGet(Func<IRequestContext, string, string, HttpResponseMessage> httpGetWithNameAndId)
        {
            HttpGetWithNameAndId = httpGetWithNameAndId;

            return this;
        }

        public IService AddHttpPost(Func<IRequestContext, HttpResponseMessage> httpPost)
        {
            HttpPost = httpPost;

            return this;
        }

        public IService AddHttpPost(Func<IRequestContext, string, HttpResponseMessage> httpPostWithName)
        {
            HttpPostWithName = httpPostWithName;

            return this;
        }

        public IService AddHttpPost(Func<IRequestContext, string, string, HttpResponseMessage> httpPostWithNameAndId)
        {
            HttpPostWithNameAndId = httpPostWithNameAndId;

            return this;
        }

        public IService AddHttpPut(Func<IRequestContext, HttpResponseMessage> httpPut)
        {
            HttpPut = httpPut;

            return this;
        }

        public IService AddHttpPut(Func<IRequestContext, string, HttpResponseMessage> httpPutWithName)
        {
            HttpPutWithName = httpPutWithName;

            return this;
        }

        public IService AddHttpPut(Func<IRequestContext, string, string, HttpResponseMessage> httpPutWithNameAndId)
        {
            HttpPutWithNameAndId = httpPutWithNameAndId;

            return this;
        }

        public IService AddHttpDelete(Func<IRequestContext, HttpResponseMessage> httpDelete)
        {
            HttpDelete = httpDelete;

            return this;
        }

        public IService AddHttpDelete(Func<IRequestContext, string, HttpResponseMessage> httpDeleteWithName)
        {
            HttpDeleteWithName = httpDeleteWithName;

            return this;
        }

        public IService AddHttpDelete(Func<IRequestContext, string, string, HttpResponseMessage> httpDeleteWithNameAndId)
        {
            HttpDeleteWithNameAndId = httpDeleteWithNameAndId;

            return this;
        }

        public event EventHandler<ParseEventArgs> PreParse;
        public event EventHandler<ParseEventArgs> PostParse;

        public void OnPreParse(ParseEventArgs e)
        {
            PreParse?.Invoke(this, e);
        }

        public void OnPostParse(ParseEventArgs e)
        {
            PostParse?.Invoke(this, e);
        }
    }
}
