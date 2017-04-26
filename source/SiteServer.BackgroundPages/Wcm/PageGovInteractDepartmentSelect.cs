using System;
using System.Collections;
using System.Collections.Specialized;
using System.Text;
using System.Web.UI.WebControls;
using BaiRong.Core;
using BaiRong.Core.Model;
using SiteServer.CMS.Core;
using SiteServer.CMS.Model;
using SiteServer.CMS.Wcm.GovInteract;
using SiteServer.CMS.Wcm.Model;

namespace SiteServer.BackgroundPages.Wcm
{
    public class PageGovInteractDepartmentSelect : BasePageGovInteract
    {
		public Literal ltlDepartmentTree;

        private int _nodeId;

        public static string GetRedirectUrl(int publishmentSystemId, int nodeId)
        {
            return PageUtils.GetWcmUrl(nameof(PageGovInteractDepartmentSelect), new NameValueCollection
            {
                {"PublishmentSystemID", publishmentSystemId.ToString()},
                {"NodeID", nodeId.ToString()}
            });
        }
		
		private string GetDepartmentTreeHtml(GovInteractChannelInfo channelInfo)
		{
			var htmlBuilder = new StringBuilder();
            var departmentIdList = GovInteractManager.GetFirstDepartmentIdList(channelInfo);

            var treeDirectoryUrl = SiteServerAssets.GetIconUrl("tree");

            var theDepartmentIdArrayList = DepartmentManager.GetDepartmentIdList();
            var isLastNodeArray = new bool[theDepartmentIdArrayList.Count];
            foreach (var theDepartmentId in theDepartmentIdArrayList)
			{
                var departmentInfo = DepartmentManager.GetDepartmentInfo(theDepartmentId);
                htmlBuilder.Append(GetTitle(departmentInfo, treeDirectoryUrl, isLastNodeArray, departmentIdList));
				htmlBuilder.Append("<br/>");
			}
			return htmlBuilder.ToString();
		}

        private string GetTitle(DepartmentInfo departmentInfo, string treeDirectoryUrl, bool[] isLastNodeArray, IList departmentIdList)
		{
            var itemBuilder = new StringBuilder();
            if (departmentInfo.IsLastNode == false)
			{
                isLastNodeArray[departmentInfo.ParentsCount] = false;
			}
			else
			{
                isLastNodeArray[departmentInfo.ParentsCount] = true;
			}
            for (var i = 0; i < departmentInfo.ParentsCount; i++)
			{
				if (isLastNodeArray[i])
				{
                    itemBuilder.Append($"<img align=\"absmiddle\" src=\"{treeDirectoryUrl}/tree_empty.gif\"/>");
				}
				else
				{
                    itemBuilder.Append($"<img align=\"absmiddle\" src=\"{treeDirectoryUrl}/tree_line.gif\"/>");
				}
			}
            if (departmentInfo.IsLastNode)
			{
                if (departmentInfo.ChildrenCount > 0)
				{
                    itemBuilder.Append($"<img align=\"absmiddle\" src=\"{treeDirectoryUrl}/tree_plusbottom.gif\"/>");
				}
				else
				{
                    itemBuilder.Append($"<img align=\"absmiddle\" src=\"{treeDirectoryUrl}/tree_minusbottom.gif\"/>");
				}
			}
			else
			{
                if (departmentInfo.ChildrenCount > 0)
				{
                    itemBuilder.Append($"<img align=\"absmiddle\" src=\"{treeDirectoryUrl}/tree_plusmiddle.gif\"/>");
				}
				else
				{
                    itemBuilder.Append($"<img align=\"absmiddle\" src=\"{treeDirectoryUrl}/tree_minusmiddle.gif\"/>");
				}
			}

			var check = "";
            if (departmentIdList.Contains(departmentInfo.DepartmentId))
			{
				check = "checked";
			}

            if (departmentInfo.ParentsCount == 0)
            {
                itemBuilder.Append(
                    $@"<label class=""checkbox inline""><input type=""checkbox"" name=""DepartmentIDCollection"" value=""{departmentInfo
                        .DepartmentId}"" {check} /> {departmentInfo.DepartmentName}</label>");
            }
            else
            {
                itemBuilder.Append(departmentInfo.DepartmentName);
            }
            
            return itemBuilder.ToString();
		}

		public void Page_Load(object sender, EventArgs e)
		{
            _nodeId = TranslateUtils.ToInt(Request.QueryString["NodeID"]);

			if (!IsPostBack)
            {
                BreadCrumb(AppManager.Wcm.LeftMenu.IdGovInteract, AppManager.Wcm.LeftMenu.GovInteract.IdGovInteractConfiguration, "负责部门设置", AppManager.Wcm.Permission.WebSite.GovInteractConfiguration);

                var channelInfo = DataProvider.GovInteractChannelDao.GetChannelInfo(PublishmentSystemId, _nodeId);
                ltlDepartmentTree.Text = GetDepartmentTreeHtml(channelInfo);
			}
		}

        public override void Submit_OnClick(object sender, EventArgs e)
		{
			if (Page.IsPostBack && Page.IsValid)
			{
                var channelInfo = DataProvider.GovInteractChannelDao.GetChannelInfo(PublishmentSystemId, _nodeId);
                channelInfo.DepartmentIDCollection = Request.Form["DepartmentIDCollection"];
                DataProvider.GovInteractChannelDao.Update(channelInfo);
                SuccessMessage("负责部门设置成功");
                AddWaitAndRedirectScript(GetRedirectUrl(PublishmentSystemId, _nodeId));
			}
		}
	}
}
