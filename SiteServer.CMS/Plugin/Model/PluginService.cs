using System;
using System.Collections.Generic;
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

        //public Func<IRequest, object> JsonGet { get; private set; }
        //public Func<IRequest, string, object> JsonGetWithName { get; private set; }
        //public Func<IRequest, string, string, object> JsonGetWithNameAndId { get; private set; }

        //public Func<IRequest, object> JsonPost { get; private set; }
        //public Func<IRequest, string, object> JsonPostWithName { get; private set; }
        //public Func<IRequest, string, string, object> JsonPostWithNameAndId { get; private set; }

        //public Func<IRequest, object> JsonPut { get; private set; }
        //public Func<IRequest, string, object> JsonPutWithName { get; private set; }
        //public Func<IRequest, string, string, object> JsonPutWithNameAndId { get; private set; }

        //public Func<IRequest, object> JsonDelete { get; private set; }
        //public Func<IRequest, string, object> JsonDeleteWithName { get; private set; }
        //public Func<IRequest, string, string, object> JsonDeleteWithNameAndId { get; private set; }

        //public Func<IRequest, HttpResponseMessage> HttpGet { get; private set; }
        //public Func<IRequest, string, HttpResponseMessage> HttpGetWithName { get; private set; }
        //public Func<IRequest, string, string, HttpResponseMessage> HttpGetWithNameAndId { get; private set; }

        //public Func<IRequest, HttpResponseMessage> HttpPost { get; private set; }
        //public Func<IRequest, string, HttpResponseMessage> HttpPostWithName { get; private set; }
        //public Func<IRequest, string, string, HttpResponseMessage> HttpPostWithNameAndId { get; private set; }

        //public Func<IRequest, HttpResponseMessage> HttpPut { get; private set; }
        //public Func<IRequest, string, HttpResponseMessage> HttpPutWithName { get; private set; }
        //public Func<IRequest, string, string, HttpResponseMessage> HttpPutWithNameAndId { get; private set; }

        //public Func<IRequest, HttpResponseMessage> HttpDelete { get; private set; }
        //public Func<IRequest, string, HttpResponseMessage> HttpDeleteWithName { get; private set; }
        //public Func<IRequest, string, string, HttpResponseMessage> HttpDeleteWithNameAndId { get; private set; }

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

        public IService AddContentLink(HyperLink link)
        {
            if (ContentLinks == null)
            {
                ContentLinks = new List<HyperLink>();
            }

            ContentLinks.Add(link);

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

        public event ApiEventHandler ApiGet;
        public event ApiEventHandler ApiPost;
        public event ApiEventHandler ApiPut;
        public event ApiEventHandler ApiDelete;

        public object OnApiGet(ApiEventArgs e)
        {
            return ApiGet?.Invoke(this, e);
        }

        public object OnApiPost(ApiEventArgs e)
        {
            return ApiPost?.Invoke(this, e);
        }

        public object OnApiPut(ApiEventArgs e)
        {
            return ApiPut?.Invoke(this, e);
        }

        public object OnApiDelete(ApiEventArgs e)
        {
            return ApiDelete?.Invoke(this, e);
        }

        //public IService AddJsonGet(Func<IRequest, object> jsonGet)
        //{
        //    JsonGet = jsonGet;

        //    return this;
        //}

        //public IService AddJsonGet(Func<IRequest, string, object> jsonGetWithName)
        //{
        //    JsonGetWithName = jsonGetWithName;

        //    return this;
        //}

        //public IService AddJsonGet(Func<IRequest, string, string, object> jsonGetWithNameAndId)
        //{
        //    JsonGetWithNameAndId = jsonGetWithNameAndId;

        //    return this;
        //}

        //public IService AddJsonPost(Func<IRequest, object> jsonPost)
        //{
        //    JsonPost = jsonPost;

        //    return this;
        //}

        //public IService AddJsonPost(Func<IRequest, string, object> jsonPostWithName)
        //{
        //    JsonPostWithName = jsonPostWithName;

        //    return this;
        //}

        //public IService AddJsonPost(Func<IRequest, string, string, object> jsonPostWithNameAndId)
        //{
        //    JsonPostWithNameAndId = jsonPostWithNameAndId;

        //    return this;
        //}

        //public IService AddJsonPut(Func<IRequest, object> jsonPut)
        //{
        //    JsonPut = jsonPut;

        //    return this;
        //}

        //public IService AddJsonPut(Func<IRequest, string, object> jsonPutWithName)
        //{
        //    JsonPutWithName = jsonPutWithName;

        //    return this;
        //}

        //public IService AddJsonPut(Func<IRequest, string, string, object> jsonPutWithNameAndId)
        //{
        //    JsonPutWithNameAndId = jsonPutWithNameAndId;

        //    return this;
        //}

        //public IService AddJsonDelete(Func<IRequest, object> jsonDelete)
        //{
        //    JsonDelete = jsonDelete;

        //    return this;
        //}

        //public IService AddJsonDelete(Func<IRequest, string, object> jsonDeleteWithName)
        //{
        //    JsonDeleteWithName = jsonDeleteWithName;

        //    return this;
        //}

        //public IService AddJsonDelete(Func<IRequest, string, string, object> jsonDeleteWithNameAndId)
        //{
        //    JsonDeleteWithNameAndId = jsonDeleteWithNameAndId;

        //    return this;
        //}

        //public IService AddHttpGet(Func<IRequest, HttpResponseMessage> httpGet)
        //{
        //    HttpGet = httpGet;

        //    return this;
        //}

        //public IService AddHttpGet(Func<IRequest, string, HttpResponseMessage> httpGetWithName)
        //{
        //    HttpGetWithName = httpGetWithName;

        //    return this;
        //}

        //public IService AddHttpGet(Func<IRequest, string, string, HttpResponseMessage> httpGetWithNameAndId)
        //{
        //    HttpGetWithNameAndId = httpGetWithNameAndId;

        //    return this;
        //}

        //public IService AddHttpPost(Func<IRequest, HttpResponseMessage> httpPost)
        //{
        //    HttpPost = httpPost;

        //    return this;
        //}

        //public IService AddHttpPost(Func<IRequest, string, HttpResponseMessage> httpPostWithName)
        //{
        //    HttpPostWithName = httpPostWithName;

        //    return this;
        //}

        //public IService AddHttpPost(Func<IRequest, string, string, HttpResponseMessage> httpPostWithNameAndId)
        //{
        //    HttpPostWithNameAndId = httpPostWithNameAndId;

        //    return this;
        //}

        //public IService AddHttpPut(Func<IRequest, HttpResponseMessage> httpPut)
        //{
        //    HttpPut = httpPut;

        //    return this;
        //}

        //public IService AddHttpPut(Func<IRequest, string, HttpResponseMessage> httpPutWithName)
        //{
        //    HttpPutWithName = httpPutWithName;

        //    return this;
        //}

        //public IService AddHttpPut(Func<IRequest, string, string, HttpResponseMessage> httpPutWithNameAndId)
        //{
        //    HttpPutWithNameAndId = httpPutWithNameAndId;

        //    return this;
        //}

        //public IService AddHttpDelete(Func<IRequest, HttpResponseMessage> httpDelete)
        //{
        //    HttpDelete = httpDelete;

        //    return this;
        //}

        //public IService AddHttpDelete(Func<IRequest, string, HttpResponseMessage> httpDeleteWithName)
        //{
        //    HttpDeleteWithName = httpDeleteWithName;

        //    return this;
        //}

        //public IService AddHttpDelete(Func<IRequest, string, string, HttpResponseMessage> httpDeleteWithNameAndId)
        //{
        //    HttpDeleteWithNameAndId = httpDeleteWithNameAndId;

        //    return this;
        //}

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
