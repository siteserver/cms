using System;
using System.Collections.Specialized;
using System.Web.UI.WebControls;
using BaiRong.Core;
using BaiRong.Core.Model.Attributes;
using SiteServer.CMS.Core;

namespace SiteServer.BackgroundPages.Cms
{
    public class ModalContentTidyUp : BasePageCms
    {
        public RadioButtonList rblAttributeName;
        public RadioButtonList rblIsDesc;
        
        private string _tableName;
        private string _returnUrl;

        public static string GetOpenWindowString(int publishmentSystemId, int nodeId, string returnUrl)
        {
            return PageUtils.GetOpenWindowStringWithCheckBoxValue("重新排序", PageUtils.GetCmsUrl(nameof(ModalContentTidyUp), new NameValueCollection
            {
                {"PublishmentSystemID", publishmentSystemId.ToString()},
                {"NodeID", nodeId.ToString()},
                {"ReturnUrl", StringUtils.ValueToUrl(returnUrl)}
            }), "ContentIDCollection", "", 430, 280);
        }

        public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            _returnUrl = StringUtils.ValueFromUrl(Body.GetQueryString("ReturnUrl")).Replace("&DateFrom=&SearchType=&Keyword=&taxisByAddDate=", "");
            if (!IsPostBack)
            {
                var listItem = new ListItem("内容ID", ContentAttribute.Id);
                listItem.Selected = true;
                rblAttributeName.Items.Add(listItem);
                listItem = new ListItem("添加日期", ContentAttribute.AddDate);
                rblAttributeName.Items.Add(listItem);

                listItem = new ListItem("正序", false.ToString());
                rblIsDesc.Items.Add(listItem);
                listItem = new ListItem("倒序", true.ToString());
                listItem.Selected = true;
                rblIsDesc.Items.Add(listItem);
            }
        }

        public override void Submit_OnClick(object sender, EventArgs e)
        {
            var nodeID = Body.GetQueryInt("NodeID");
            var nodeInfo = NodeManager.GetNodeInfo(PublishmentSystemId, nodeID);
            _tableName = NodeManager.GetTableName(PublishmentSystemInfo, nodeInfo);

            DataProvider.ContentDao.TidyUp(_tableName, nodeID, rblAttributeName.SelectedValue, TranslateUtils.ToBool(rblIsDesc.SelectedValue));

            PageUtils.CloseModalPageAndRedirect(Page, _returnUrl);
        }
    }
}
