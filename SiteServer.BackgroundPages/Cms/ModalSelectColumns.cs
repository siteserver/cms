using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Web.UI.WebControls;
using SiteServer.Utils;
using SiteServer.CMS.Core;
using SiteServer.CMS.Model;

namespace SiteServer.BackgroundPages.Cms
{
    public class ModalSelectColumns : BasePageCms
    {
        public CheckBoxList CblDisplayAttributes;

        private int _relatedIdentity;
        private List<int> _relatedIdentities;
        private bool _isList;

        public static string GetOpenWindowString(int siteId, int relatedIdentity, bool isList)
        {
            return LayerUtils.GetOpenScript("选择需要显示的项", PageUtils.GetCmsUrl(siteId, nameof(ModalSelectColumns), new NameValueCollection
            {
                {"RelatedIdentity", relatedIdentity.ToString()},
                {"IsList", isList.ToString()}
            }), 520, 550);
        }

        public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            PageUtils.CheckRequestParameter("siteId");

            _relatedIdentity = Body.GetQueryInt("RelatedIdentity");
            _isList = Body.GetQueryBool("IsList");

            var nodeInfo = ChannelManager.GetChannelInfo(SiteId, _relatedIdentity);
            var tableName = ChannelManager.GetTableName(SiteInfo, nodeInfo);
            _relatedIdentities = RelatedIdentities.GetChannelRelatedIdentities(SiteId, _relatedIdentity);
            var attributesOfDisplay = TranslateUtils.StringCollectionToStringCollection(nodeInfo.Additional.ContentAttributesOfDisplay);

            if (IsPostBack) return;

            var styleInfoList = TableStyleManager.GetTableStyleInfoList(tableName, _relatedIdentities);
            var columnTableStyleInfoList = ContentUtility.GetColumnTableStyleInfoList(SiteInfo, styleInfoList);
            foreach (var styleInfo in columnTableStyleInfoList)
            {
                if (styleInfo.AttributeName == ContentAttribute.Title) continue;
                var listitem = new ListItem(styleInfo.DisplayName, styleInfo.AttributeName);

                if (_isList)
                {
                    if (attributesOfDisplay.Contains(styleInfo.AttributeName))
                    {
                        listitem.Selected = true;
                    }
                }
                else
                {
                    listitem.Selected = true;
                }

                CblDisplayAttributes.Items.Add(listitem);
            }
        }

        public override void Submit_OnClick(object sender, EventArgs e)
        {
            var nodeInfo = ChannelManager.GetChannelInfo(SiteId, _relatedIdentity);
            var attributesOfDisplay = ControlUtils.SelectedItemsValueToStringCollection(CblDisplayAttributes.Items);
            nodeInfo.Additional.ContentAttributesOfDisplay = attributesOfDisplay;

            DataProvider.ChannelDao.Update(nodeInfo);

            Body.AddSiteLog(SiteId, "设置内容显示项", $"显示项:{attributesOfDisplay}");

            if (!_isList)
            {
                LayerUtils.CloseWithoutRefresh(Page);
            }
            else
            {
                LayerUtils.Close(Page);
            }
        }

    }
}
