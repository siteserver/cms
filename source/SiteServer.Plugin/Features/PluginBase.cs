using System;
using System.Collections.Generic;
using System.IO;
using System.Web;

namespace SiteServer.Plugin.Features
{
    public class PluginBase
    {
        #region IContentModel

        public virtual List<PluginContentLink> ContentLinks => null;

        public virtual void AfterContentAdded(int siteId, int channelId, int contentId)
        {

        }

        public virtual void AfterContentTranslated(int siteId, int channelId, int contentId, int targetSiteId, int targetChannelId,
            int targetContentId)
        {

        }

        public virtual void AfterContentDeleted(int siteId, int channelId, int contentId)
        {

        }

        public virtual bool IsCustomContentTable => false;
        public virtual string CustomContentTableName => string.Empty;
        public virtual List<PluginTableColumn> CustomContentTableColumns => null;

        #endregion

        #region IFileSystemWatcher

        public virtual void OnChanged(object sender, FileSystemEventArgs e)
        {

        }

        #endregion

        #region IHttp

        public virtual void HttpGet(HttpRequest request, HttpResponse response)
        {

        }

        public virtual void HttpGet(HttpRequest request, HttpResponse response, string name)
        {

        }

        public virtual void HttpGet(HttpRequest request, HttpResponse response, string name, int id)
        {

        }

        public virtual void HttpPost(HttpRequest request, HttpResponse response)
        {

        }

        public virtual void HttpPost(HttpRequest request, HttpResponse response, string name)
        {

        }

        public virtual void HttpPost(HttpRequest request, HttpResponse response, string name, int id)
        {

        }

        public virtual void HttpPut(HttpRequest request, HttpResponse response)
        {

        }

        public virtual void HttpPut(HttpRequest request, HttpResponse response, string name)
        {

        }

        public virtual void HttpPut(HttpRequest request, HttpResponse response, string name, int id)
        {

        }

        public virtual void HttpDelete(HttpRequest request, HttpResponse response)
        {

        }

        public virtual void HttpDelete(HttpRequest request, HttpResponse response, string name)
        {

        }

        public virtual void HttpDelete(HttpRequest request, HttpResponse response, string name, int id)
        {

        }

        public virtual void HttpPatch(HttpRequest request, HttpResponse response)
        {

        }

        public virtual void HttpPatch(HttpRequest request, HttpResponse response, string name)
        {

        }

        public virtual void HttpPatch(HttpRequest request, HttpResponse response, string name, int id)
        {

        }

        #endregion

        #region IMenu

        public virtual PluginMenu GetTopMenu()
        {
            return null;
        }

        public virtual PluginMenu GetSiteMenu(int siteId)
        {
            return null;
        }

        #endregion

        #region IPageAdmin

        public virtual void OnPreLoad(EventArgs e)
        {

        }

        public virtual void OnLoadComplete(EventArgs e)
        {

        }

        #endregion

        #region IParser

        public virtual List<string> ElementNames => null;

        public virtual string Parse(PluginParserContext context)
        {
            return string.Empty;
        }

        #endregion

        #region IPlugin

        /// <summary>
        /// 激活插件，执行初始化
        /// </summary>
        /// <param name="context"></param>
        public virtual void Active(PluginContext context)
        {

        }

        /// <summary>
        /// 停止插件，执行与释放或重置非托管资源相关的应用程序定义的任务
        /// </summary>
        /// <param name="context"></param>
        public virtual void Deactive(PluginContext context)
        {

        }

        /// <summary>
        /// 卸载插件
        /// </summary>
        /// <param name="context"></param>
        public virtual void Uninstall(PluginContext context)
        {
            
        }

        #endregion       

        #region IRestful

        public virtual object RestfulGet(IRequestContext context)
        {
            return null;
        }

        public virtual object RestfulGet(IRequestContext context, string name)
        {
            return null;
        }

        public virtual object RestfulGet(IRequestContext context, string name, int id)
        {
            return null;
        }

        public virtual object RestfulPost(IRequestContext context)
        {
            return null;
        }

        public virtual object RestfulPost(IRequestContext context, string name)
        {
            return null;
        }

        public virtual object RestfulPost(IRequestContext context, string name, int id)
        {
            return null;
        }

        public virtual object RestfulPut(IRequestContext context)
        {
            return null;
        }

        public virtual object RestfulPut(IRequestContext context, string name)
        {
            return null;
        }

        public virtual object RestfulPut(IRequestContext context, string name, int id)
        {
            return null;
        }

        public virtual object RestfulDelete(IRequestContext context)
        {
            return null;
        }

        public virtual object RestfulDelete(IRequestContext context, string name)
        {
            return null;
        }

        public virtual object RestfulDelete(IRequestContext context, string name, int id)
        {
            return null;
        }

        public virtual object RestfulPatch(IRequestContext context)
        {
            return null;
        }

        public virtual object RestfulPatch(IRequestContext context, string name)
        {
            return null;
        }

        public virtual object RestfulPatch(IRequestContext context, string name, int id)
        {
            return null;
        }

        #endregion

        #region ITable

        public virtual Dictionary<string, List<PluginTableColumn>> Tables => null;

        #endregion
    }
}