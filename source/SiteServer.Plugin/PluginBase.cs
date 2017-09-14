using System;
using System.Collections.Generic;
using System.IO;
using System.Web;
using SiteServer.Plugin.Models;

namespace SiteServer.Plugin
{
    public class PluginBase: IPlugin
    {
        #region IPlugin

        public virtual Action<PluginContext> OnPluginActive => null;

        public virtual Action<PluginContext> OnPluginDeactive => null;

        public virtual Action<PluginContext> OnPluginUninstall => null;

        #endregion       

        #region IChannel

        public virtual List<PluginContentLink> ContentLinks => null;

        public virtual Action<int, int, int> OnContentAdded => null;

        public virtual Action<int, int, int, int, int, int> OnContentTranslated => null;

        public virtual Action<int, int, int> OnContentDeleted => null;

        #endregion

        #region IContentTable

        public virtual string ContentTableName => null;

        public virtual List<PluginTableColumn> ContentTableColumns => null;

        #endregion

        #region IFileSystem

        public virtual Action<object, FileSystemEventArgs> OnFileSystemChanged => null;

        #endregion

        #region IHttpApi

        public virtual Action<HttpRequest, HttpResponse> HttpGet => null;

        public virtual Action<HttpRequest, HttpResponse, string> HttpGetWithName => null;

        public virtual Action<HttpRequest, HttpResponse, string, int> HttpGetWithNameAndId => null;

        public virtual Action<HttpRequest, HttpResponse> HttpPost => null;

        public virtual Action<HttpRequest, HttpResponse, string> HttpPostWithName => null;

        public virtual Action<HttpRequest, HttpResponse, string, int> HttpPostWithNameAndId => null;

        public virtual Action<HttpRequest, HttpResponse> HttpPut => null;

        public virtual Action<HttpRequest, HttpResponse, string> HttpPutWithName => null;

        public virtual Action<HttpRequest, HttpResponse, string, int> HttpPutWithNameAndId => null;

        public virtual Action<HttpRequest, HttpResponse> HttpDelete => null;

        public virtual Action<HttpRequest, HttpResponse, string> HttpDeleteWithName => null;

        public virtual Action<HttpRequest, HttpResponse, string, int> HttpDeleteWithNameAndId => null;

        public virtual Action<HttpRequest, HttpResponse> HttpPatch => null;

        public virtual Action<HttpRequest, HttpResponse, string> HttpPatchWithName => null;

        public virtual Action<HttpRequest, HttpResponse, string, int> HttpPatchWithNameAndId => null;

        #endregion

        #region IJsonApi

        public virtual Func<IRequestContext, object> JsonGet => null;

        public virtual Func<IRequestContext, string, object> JsonGetWithName => null;

        public virtual Func<IRequestContext, string, int, object> JsonGetWithNameAndId => null;

        public virtual Func<IRequestContext, object> JsonPost => null;

        public virtual Func<IRequestContext, string, object> JsonPostWithName => null;

        public virtual Func<IRequestContext, string, int, object> JsonPostWithNameAndId => null;

        public virtual Func<IRequestContext, object> JsonPut => null;

        public virtual Func<IRequestContext, string, object> JsonPutWithName => null;

        public virtual Func<IRequestContext, string, int, object> JsonPutWithNameAndId => null;

        public virtual Func<IRequestContext, object> JsonDelete => null;

        public virtual Func<IRequestContext, string, object> JsonDeleteWithName => null;

        public virtual Func<IRequestContext, string, int, object> JsonDeleteWithNameAndId => null;

        public virtual Func<IRequestContext, object> JsonPatch => null;

        public virtual Func<IRequestContext, string, object> JsonPatchWithName => null;

        public virtual Func<IRequestContext, string, int, object> JsonPatchWithNameAndId => null;

        #endregion

        #region IMenu

        public virtual Func<PluginMenu> GlobalMenu => null;

        public virtual Func<int, PluginMenu> Menu => null;

        #endregion

        #region IPageAdmin

        public virtual Action<EventArgs> OnPageAdminPreLoad => null;

        public virtual Action<EventArgs> OnPageAdminLoadComplete => null;

        #endregion

        #region IParse

        public virtual Dictionary<string, Func<PluginParseContext, string>> ElementsToParse => null;

        #endregion

        #region IRender

        public virtual Func<PluginRenderContext, string> Render => null;

        #endregion       

        #region ITable

        public virtual Dictionary<string, List<PluginTableColumn>> Tables => null;

        #endregion
    }
}