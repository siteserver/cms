using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Web.UI.WebControls;
using SiteServer.Abstractions;
using SiteServer.CMS.Context;
using SiteServer.CMS.Core;
using SiteServer.CMS.DataCache;
using SiteServer.CMS.Plugin;
using SiteServer.CMS.Repositories;


namespace SiteServer.BackgroundPages.Cms
{
    public class ModalSelectColumns : BasePageCms
    {
        protected override bool IsSinglePage => true;

        protected CheckBoxList CblDisplayAttributes;

        private int _channelId;
        private Dictionary<string, Dictionary<string, Func<IContentContext, string>>> _pluginColumns;

        public static string GetOpenWindowString(int siteId, int channelId)
        {
            return LayerUtils.GetOpenScript("设置显示项", PageUtils.GetCmsUrl(siteId, nameof(ModalSelectColumns), new NameValueCollection
            {
                {"channelId", channelId.ToString()}
            }));
        }

        public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            PageUtils.CheckRequestParameter("siteId");

            _channelId = AuthRequest.GetQueryInt("channelId");

            var channelInfo = ChannelManager.GetChannelAsync(SiteId, _channelId).GetAwaiter().GetResult();
            var attributesOfDisplay = TranslateUtils.StringCollectionToStringCollection(channelInfo.ContentAttributesOfDisplay);
            var pluginIds = PluginContentManager.GetContentPluginIds(channelInfo);
            _pluginColumns = PluginContentManager.GetContentColumnsAsync(pluginIds).GetAwaiter().GetResult();

            if (IsPostBack) return;

            var styleList = ContentUtility.GetAllTableStyleList(TableStyleManager.GetContentStyleListAsync(Site, channelInfo).GetAwaiter().GetResult());
            foreach (var style in styleList)
            {
                if (style.Type == InputType.TextEditor) continue;
                
                var listitem = new ListItem($"{style.DisplayName}({style.AttributeName})", style.AttributeName);
                if (style.AttributeName == ContentAttribute.Title)
                {
                    listitem.Selected = true;
                }
                else
                {
                    if (attributesOfDisplay.Contains(style.AttributeName))
                    {
                        listitem.Selected = true;
                    }
                }

                CblDisplayAttributes.Items.Add(listitem);
            }

            if (_pluginColumns != null)
            {
                foreach (var pluginId in _pluginColumns.Keys)
                {
                    var contentColumns = _pluginColumns[pluginId];
                    if (contentColumns == null || contentColumns.Count == 0) continue;

                    foreach (var columnName in contentColumns.Keys)
                    {
                        var attributeName = $"{pluginId}:{columnName}";
                        var listitem = new ListItem($"{columnName}({pluginId})", attributeName);
                        if (attributesOfDisplay.Contains(attributeName))
                        {
                            listitem.Selected = true;
                        }

                        CblDisplayAttributes.Items.Add(listitem);
                    }
                }
            }
        }

        public override void Submit_OnClick(object sender, EventArgs e)
        {
            var channelInfo = ChannelManager.GetChannelAsync(SiteId, _channelId).GetAwaiter().GetResult();
            var attributesOfDisplay = ControlUtils.SelectedItemsValueToStringCollection(CblDisplayAttributes.Items);
            channelInfo.ContentAttributesOfDisplay = attributesOfDisplay;

            DataProvider.ChannelRepository.UpdateAsync(channelInfo).GetAwaiter().GetResult();

            AuthRequest.AddSiteLogAsync(SiteId, "设置内容显示项", $"显示项:{attributesOfDisplay}").GetAwaiter().GetResult();

            LayerUtils.Close(Page);
        }

    }
}
