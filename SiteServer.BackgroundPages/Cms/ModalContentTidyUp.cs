using System;
using System.Collections.Specialized;
using System.Web.UI.WebControls;
using BaiRong.Core;
using BaiRong.Core.Model;
using SiteServer.CMS.Core;

namespace SiteServer.BackgroundPages.Cms
{
    public class ModalContentTidyUp : BasePageCms
    {
        public DropDownList DdlAttributeName;
        public DropDownList DdlIsDesc;
        
        private string _tableName;
        private string _returnUrl;

        public static string GetOpenWindowString(int publishmentSystemId, int nodeId, string returnUrl)
        {
            return LayerUtils.GetOpenScriptWithCheckBoxValue("整理排序", PageUtils.GetCmsUrl(nameof(ModalContentTidyUp), new NameValueCollection
            {
                {"PublishmentSystemID", publishmentSystemId.ToString()},
                {"NodeID", nodeId.ToString()},
                {"ReturnUrl", StringUtils.ValueToUrl(returnUrl)}
            }), "ContentIDCollection", "", 460, 320);
        }

        public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            _returnUrl = StringUtils.ValueFromUrl(Body.GetQueryString("ReturnUrl")).Replace("&DateFrom=&SearchType=&Keyword=&taxisByAddDate=", "");
            if (IsPostBack) return;

            var listItem = new ListItem("内容Id", ContentAttribute.Id) {Selected = true};
            DdlAttributeName.Items.Add(listItem);
            listItem = new ListItem("添加日期", ContentAttribute.AddDate);
            DdlAttributeName.Items.Add(listItem);

            listItem = new ListItem("正序", false.ToString());
            DdlIsDesc.Items.Add(listItem);
            listItem = new ListItem("倒序", true.ToString()) {Selected = true};
            DdlIsDesc.Items.Add(listItem);
        }

        public override void Submit_OnClick(object sender, EventArgs e)
        {
            var nodeId = Body.GetQueryInt("NodeID");
            var nodeInfo = NodeManager.GetNodeInfo(PublishmentSystemId, nodeId);
            _tableName = NodeManager.GetTableName(PublishmentSystemInfo, nodeInfo);

            DataProvider.ContentDao.TidyUp(_tableName, nodeId, DdlAttributeName.SelectedValue, TranslateUtils.ToBool(DdlIsDesc.SelectedValue));

            LayerUtils.CloseAndRedirect(Page, _returnUrl);
        }
    }
}
