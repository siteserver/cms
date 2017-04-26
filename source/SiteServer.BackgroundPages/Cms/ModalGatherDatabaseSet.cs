using System;
using System.Collections.Specialized;
using System.Web.UI.WebControls;
using BaiRong.Core;
using SiteServer.CMS.Core;

namespace SiteServer.BackgroundPages.Cms
{
	public class ModalGatherDatabaseSet : BasePageCms
    {
		protected DropDownList NodeIDDropDownList;
		protected TextBox GatherNum;

		private string _gatherRuleName;

        public static string GetOpenWindowString(int publishmentSystemId, string gatherRuleName)
        {
            return PageUtils.GetOpenWindowString("信息采集", PageUtils.GetCmsUrl(nameof(ModalGatherDatabaseSet), new NameValueCollection
            {
                {"PublishmentSystemID", publishmentSystemId.ToString()},
                {"GatherRuleName", gatherRuleName}
            }), 460, 280);
        }

		public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            PageUtils.CheckRequestParameter("PublishmentSystemID", "GatherRuleName");
            _gatherRuleName = Body.GetQueryString("GatherRuleName");

			if (!IsPostBack)
			{
				var gatherDatabaseRuleInfo = DataProvider.GatherDatabaseRuleDao.GetGatherDatabaseRuleInfo(_gatherRuleName, PublishmentSystemId);
                InfoMessage("采集名称：" + _gatherRuleName);

				GatherNum.Text = gatherDatabaseRuleInfo.GatherNum.ToString();

                NodeManager.AddListItemsForAddContent(NodeIDDropDownList.Items, PublishmentSystemInfo, true, Body.AdministratorName);
                ControlUtils.SelectListItems(NodeIDDropDownList, gatherDatabaseRuleInfo.NodeId.ToString());
			}
		}

        public override void Submit_OnClick(object sender, EventArgs e)
        {
            var gatherDatabaseRuleInfo = DataProvider.GatherDatabaseRuleDao.GetGatherDatabaseRuleInfo(_gatherRuleName, PublishmentSystemId);

			gatherDatabaseRuleInfo.NodeId = TranslateUtils.ToInt(NodeIDDropDownList.SelectedValue);
			gatherDatabaseRuleInfo.GatherNum = TranslateUtils.ToInt(GatherNum.Text);
            DataProvider.GatherDatabaseRuleDao.Update(gatherDatabaseRuleInfo);

            PageUtils.Redirect(ModalProgressBar.GetRedirectUrlStringWithGatherDatabase(PublishmentSystemId, _gatherRuleName));
		}
	}
}
