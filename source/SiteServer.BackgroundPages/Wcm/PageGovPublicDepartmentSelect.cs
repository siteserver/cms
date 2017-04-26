using System;
using System.Collections;
using System.Collections.Specialized;
using System.Text;
using System.Web.UI.WebControls;
using BaiRong.Core;
using BaiRong.Core.Model;
using SiteServer.CMS.Core;
using SiteServer.CMS.Wcm.GovPublic;

namespace SiteServer.BackgroundPages.Wcm
{
    public class PageGovPublicDepartmentSelect : BasePageGovPublic
    {
		public Literal ltlDepartmentTree;
		
		private string GetDepartmentTreeHtml()
		{
			var htmlBuilder = new StringBuilder();
            var departmentIdList = GovPublicManager.GetFirstDepartmentIdList(PublishmentSystemInfo);

            var treeDirectoryUrl = SiteServerAssets.GetIconUrl("tree");

            var theDepartmentIdList = DepartmentManager.GetDepartmentIdList();
            var isLastNodeArray = new bool[theDepartmentIdList.Count];
            foreach (var theDepartmentId in theDepartmentIdList)
			{
                var departmentInfo = DepartmentManager.GetDepartmentInfo(theDepartmentId);
                htmlBuilder.Append(GetTitle(departmentInfo, treeDirectoryUrl, isLastNodeArray, departmentIdList));
				htmlBuilder.Append("<br/>");
			}
			return htmlBuilder.ToString();
		}

        private string GetTitle(DepartmentInfo departmentInfo, string treeDirectoryUrl, bool[] IsLastNodeArray, IList departmentIDArrayList)
		{
            var itemBuilder = new StringBuilder();
            if (departmentInfo.IsLastNode == false)
			{
                IsLastNodeArray[departmentInfo.ParentsCount] = false;
			}
			else
			{
                IsLastNodeArray[departmentInfo.ParentsCount] = true;
			}
            for (var i = 0; i < departmentInfo.ParentsCount; i++)
			{
				if (IsLastNodeArray[i])
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
            if (departmentIDArrayList.Contains(departmentInfo.DepartmentId))
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
			if (!IsPostBack)
            {
                BreadCrumb(AppManager.Wcm.LeftMenu.IdGovPublic, AppManager.Wcm.LeftMenu.GovPublic.IdGovPublicContentConfiguration, "可选择部门设置", AppManager.Wcm.Permission.WebSite.GovPublicContentConfiguration);

                ltlDepartmentTree.Text = GetDepartmentTreeHtml();
			}
		}

		public override void Submit_OnClick(object sender, EventArgs e)
		{
			if (Page.IsPostBack && Page.IsValid)
			{
                PublishmentSystemInfo.Additional.GovPublicDepartmentIdCollection = Request.Form["DepartmentIDCollection"];
                DataProvider.PublishmentSystemDao.Update(PublishmentSystemInfo);
                SuccessMessage("可选择部门设置成功");
			    AddWaitAndRedirectScript(PageUtils.GetWcmUrl(nameof(PageGovPublicDepartmentSelect), new NameValueCollection
			    {
			        {"PublishmentSystemID", PublishmentSystemId.ToString()}
			    }));
			}
		}
	}
}
