using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using BaiRong.Core;
using BaiRong.Core.Table;
using SiteServer.CMS.Core;
using SiteServer.CMS.Core.Create;

namespace SiteServer.BackgroundPages.Cms
{
    public class ModalContentArchive : BasePageCms
    {
        private int _nodeId;
        private string _returnUrl;
        private List<int> _contentIdList;

        public static string GetOpenWindowString(int publishmentSystemId, int nodeId, string returnUrl)
        {
            return LayerUtils.GetOpenScriptWithCheckBoxValue("内容归档", PageUtils.GetCmsUrl(nameof(ModalContentArchive), new NameValueCollection
            {
                {"PublishmentSystemID", publishmentSystemId.ToString()},
                {"NodeID", nodeId.ToString()},
                {"ReturnUrl", StringUtils.ValueToUrl(returnUrl)}
            }), "ContentIDCollection", "请选择需要归档的内容！", 400, 230);
        }

		public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            PageUtils.CheckRequestParameter("PublishmentSystemID", "NodeID", "ReturnUrl", "ContentIDCollection");

            _nodeId = Body.GetQueryInt("NodeID");
            _returnUrl = StringUtils.ValueFromUrl(Body.GetQueryString("ReturnUrl"));
            _contentIdList = TranslateUtils.StringCollectionToIntList(Body.GetQueryString("ContentIDCollection"));
		}

        public override void Submit_OnClick(object sender, EventArgs e)
        {
            var tableName = NodeManager.GetTableName(PublishmentSystemInfo, _nodeId);
            ArchiveManager.CreateArchiveTableIfNotExists(PublishmentSystemInfo, tableName);
            var tableNameOfArchive = TableMetadataManager.GetTableNameOfArchive(tableName);

            foreach (var contentId in _contentIdList)
            {
                var contentInfo = DataProvider.ContentDao.GetContentInfo(tableName, contentId);
                contentInfo.LastEditDate = DateTime.Now;
                DataProvider.ContentDao.Insert(tableNameOfArchive, PublishmentSystemInfo, contentInfo);
            }

            DataProvider.ContentDao.DeleteContents(PublishmentSystemId, tableName, _contentIdList, _nodeId);

            CreateManager.CreateContentTrigger(PublishmentSystemId, _nodeId);

            Body.AddSiteLog(PublishmentSystemId, _nodeId, 0, "归档内容", string.Empty);

            LayerUtils.CloseAndRedirect(Page, _returnUrl);
		}

	}
}
