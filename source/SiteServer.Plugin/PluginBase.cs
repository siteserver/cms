using System;
using System.Collections.Generic;
using System.IO;

namespace SiteServer.Plugin
{
    public class PluginBase
    {
        #region IPlugin virtual methods

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

        #region IContentModel virtual methods

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

        #region IFileSystemWatcher virtual methods

        public virtual void OnChanged(object sender, FileSystemEventArgs e)
        {
            
        }

        #endregion

        #region IMenu virtual methods

        public virtual PluginMenu GetTopMenu()
        {
            return null;
        }

        public virtual PluginMenu GetSiteMenu(int siteId)
        {
            return null;
        }

        #endregion

        #region IPageAdmin virtual methods

        public virtual void OnPreLoad(EventArgs e)
        {
            
        }

        public virtual void OnLoadComplete(EventArgs e)
        {
            
        }

        #endregion

        #region IRestful virtual methods

        public virtual object Get(IRequestContext context)
        {
            throw new NotImplementedException();
        }

        public virtual object Get(IRequestContext context, string name)
        {
            throw new NotImplementedException();
        }

        public virtual object Get(IRequestContext context, string name, int id)
        {
            throw new NotImplementedException();
        }

        public virtual object Post(IRequestContext context)
        {
            throw new NotImplementedException();
        }

        public virtual object Post(IRequestContext context, string name)
        {
            throw new NotImplementedException();
        }

        public virtual object Post(IRequestContext context, string name, int id)
        {
            throw new NotImplementedException();
        }

        public virtual object Put(IRequestContext context)
        {
            throw new NotImplementedException();
        }

        public virtual object Put(IRequestContext context, string name)
        {
            throw new NotImplementedException();
        }

        public virtual object Put(IRequestContext context, string name, int id)
        {
            throw new NotImplementedException();
        }

        public virtual object Delete(IRequestContext context)
        {
            throw new NotImplementedException();
        }

        public virtual object Delete(IRequestContext context, string name)
        {
            throw new NotImplementedException();
        }

        public virtual object Delete(IRequestContext context, string name, int id)
        {
            throw new NotImplementedException();
        }

        public virtual object Patch(IRequestContext context)
        {
            throw new NotImplementedException();
        }

        public virtual object Patch(IRequestContext context, string name)
        {
            throw new NotImplementedException();
        }

        public virtual object Patch(IRequestContext context, string name, int id)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region ITable

        public virtual Dictionary<string, List<PluginTableColumn>> Tables => null;

        #endregion
    }
}