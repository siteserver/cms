using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
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

        #region IWebApi

        public virtual Func<IRequestContext, object> JsonGet => null;
        public virtual Func<IRequestContext, string, object> JsonGetWithName => null;
        public virtual Func<IRequestContext, string, string, object> JsonGetWithNameAndId => null;

        public virtual Func<IRequestContext, object> JsonPost => null;
        public virtual Func<IRequestContext, string, object> JsonPostWithName => null;
        public virtual Func<IRequestContext, string, string, object> JsonPostWithNameAndId => null;

        public virtual Func<IRequestContext, object> JsonPut => null;
        public virtual Func<IRequestContext, string, object> JsonPutWithName => null;
        public virtual Func<IRequestContext, string, string, object> JsonPutWithNameAndId => null;

        public virtual Func<IRequestContext, object> JsonDelete => null;
        public virtual Func<IRequestContext, string, object> JsonDeleteWithName => null;
        public virtual Func<IRequestContext, string, string, object> JsonDeleteWithNameAndId => null;

        public virtual Func<IRequestContext, object> JsonPatch => null;
        public virtual Func<IRequestContext, string, object> JsonPatchWithName => null;
        public virtual Func<IRequestContext, string, string, object> JsonPatchWithNameAndId => null;

        public virtual Func<IRequestContext, HttpResponseMessage> HttpGet => null;
        public virtual Func<IRequestContext, string, HttpResponseMessage> HttpGetWithName => null;
        public virtual Func<IRequestContext, string, string, HttpResponseMessage> HttpGetWithNameAndId => null;

        public virtual Func<IRequestContext, HttpResponseMessage> HttpPost => null;
        public virtual Func<IRequestContext, string, HttpResponseMessage> HttpPostWithName => null;
        public virtual Func<IRequestContext, string, string, HttpResponseMessage> HttpPostWithNameAndId => null;

        public virtual Func<IRequestContext, HttpResponseMessage> HttpPut => null;
        public virtual Func<IRequestContext, string, HttpResponseMessage> HttpPutWithName => null;
        public virtual Func<IRequestContext, string, string, HttpResponseMessage> HttpPutWithNameAndId => null;

        public virtual Func<IRequestContext, HttpResponseMessage> HttpDelete => null;
        public virtual Func<IRequestContext, string, HttpResponseMessage> HttpDeleteWithName => null;
        public virtual Func<IRequestContext, string, string, HttpResponseMessage> HttpDeleteWithNameAndId => null;

        public virtual Func<IRequestContext, HttpResponseMessage> HttpPatch => null;
        public virtual Func<IRequestContext, string, HttpResponseMessage> HttpPatchWithName => null;
        public virtual Func<IRequestContext, string, string, HttpResponseMessage> HttpPatchWithNameAndId => null;

        #endregion

        #region IMenu

        public virtual PluginMenu PluginMenu => null;

        public virtual Func<int, PluginMenu> SiteMenu => null;

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