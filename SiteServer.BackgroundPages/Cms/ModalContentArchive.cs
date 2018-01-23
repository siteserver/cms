using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using SiteServer.Utils;
using SiteServer.CMS.Core;
using SiteServer.CMS.Core.Create;

namespace SiteServer.BackgroundPages.Cms
{
    public class ModalContentArchive : BasePageCms
    {
        private int _nodeId;
        private string _returnUrl;
        private List<int> _contentIdList;

        public static string GetOpenWindowString(int siteId, int nodeId, string returnUrl)
        {
            return LayerUtils.GetOpenScriptWithCheckBoxValue("内容归档", PageUtils.GetCmsUrl(siteId, nameof(ModalContentArchive), new NameValueCollection
            {
                {"NodeID", nodeId.ToString()},
                {"ReturnUrl", StringUtils.ValueToUrl(returnUrl)}
            }), "ContentIDCollection", "请选择需要归档的内容！", 400, 230);
        }

		public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            PageUtils.CheckRequestParameter("siteId", "NodeID", "ReturnUrl", "ContentIDCollection");

            _nodeId = Body.GetQueryInt("NodeID");
            _returnUrl = StringUtils.ValueFromUrl(Body.GetQueryString("ReturnUrl"));
            _contentIdList = TranslateUtils.StringCollectionToIntList(Body.GetQueryString("ContentIDCollection"));
		}

        public override void Submit_OnClick(object sender, EventArgs e)
        {
            var tableName = ChannelManager.GetTableName(SiteInfo, _nodeId);
            ArchiveManager.CreateArchiveTableIfNotExists(SiteInfo, tableName);
            var tableNameOfArchive = TableMetadataManager.GetTableNameOfArchive(tableName);

            foreach (var contentId in _contentIdList)
            {
                var contentInfo = DataProvider.ContentDao.GetContentInfo(tableName, contentId);
                contentInfo.LastEditDate = DateTime.Now;
                DataProvider.ContentDao.Insert(tableNameOfArchive, SiteInfo, contentInfo);
            }

            DataProvider.ContentDao.DeleteContents(SiteId, tableName, _contentIdList, _nodeId);

            CreateManager.CreateContentTrigger(SiteId, _nodeId);

            Body.AddSiteLog(SiteId, _nodeId, 0, "归档内容", string.Empty);

            LayerUtils.CloseAndRedirect(Page, _returnUrl);
		}

	}
}
