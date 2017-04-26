using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using BaiRong.Core;
using BaiRong.Core.AuxiliaryTable;
using BaiRong.Core.Model.Enumerations;
using SiteServer.CMS.Core;
using SiteServer.CMS.Core.Create;

namespace SiteServer.BackgroundPages.Cms
{
    public class ModalContentArchive : BasePageCms
    {
        private int _nodeId;
        private ETableStyle _tableStyle;
        private string _returnUrl;
        private List<int> _contentIdArrayList;

        public static string GetOpenWindowString(int publishmentSystemId, int nodeId, string returnUrl)
        {
            return PageUtils.GetOpenWindowStringWithCheckBoxValue("内容归档", PageUtils.GetCmsUrl(nameof(ModalContentArchive), new NameValueCollection
            {
                {"PublishmentSystemID", publishmentSystemId.ToString()},
                {"NodeID", nodeId.ToString()},
                {"ReturnUrl", StringUtils.ValueToUrl(returnUrl)}
            }), "ContentIDCollection", "请选择需要归档的内容！", 400, 200);
        }

		public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            PageUtils.CheckRequestParameter("PublishmentSystemID", "NodeID", "ReturnUrl", "ContentIDCollection");

            _nodeId = Body.GetQueryInt("NodeID");
            _tableStyle = NodeManager.GetTableStyle(PublishmentSystemInfo, _nodeId);
            _returnUrl = StringUtils.ValueFromUrl(Body.GetQueryString("ReturnUrl"));
            _contentIdArrayList = TranslateUtils.StringCollectionToIntList(Body.GetQueryString("ContentIDCollection"));
		}

        public override void Submit_OnClick(object sender, EventArgs e)
        {
            var tableName = NodeManager.GetTableName(PublishmentSystemInfo, _nodeId);
            ArchiveManager.CreateArchiveTableIfNotExists(PublishmentSystemInfo, tableName);
            var tableNameOfArchive = TableManager.GetTableNameOfArchive(tableName);

            foreach (int contentID in _contentIdArrayList)
            {
                var contentInfo = DataProvider.ContentDao.GetContentInfo(_tableStyle, tableName, contentID);
                contentInfo.LastEditDate = DateTime.Now;
                DataProvider.ContentDao.Insert(tableNameOfArchive, PublishmentSystemInfo, contentInfo);
            }

            DataProvider.ContentDao.DeleteContents(PublishmentSystemId, tableName, _contentIdArrayList, _nodeId);

            CreateManager.CreateContentTrigger(PublishmentSystemId, _nodeId);

            Body.AddSiteLog(PublishmentSystemId, _nodeId, 0, "归档内容", string.Empty);

            PageUtils.CloseModalPageAndRedirect(Page, _returnUrl);
		}

	}
}
