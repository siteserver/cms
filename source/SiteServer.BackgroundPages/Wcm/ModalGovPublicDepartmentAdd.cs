using System;
using System.Collections.Specialized;
using System.Web.UI.WebControls;
using BaiRong.Core;
using BaiRong.Core.Model;
using BaiRong.Core.Text;
using SiteServer.CMS.Wcm.GovPublic;

namespace SiteServer.BackgroundPages.Wcm
{
	public class ModalGovPublicDepartmentAdd : BasePageCms
	{
        public TextBox DepartmentName;
        public TextBox Code;
        public DropDownList ParentID;
        public TextBox Summary;

        private string _returnUrl = string.Empty;
        private bool[] _isLastNodeArray;

	    public static string GetOpenWindowStringToAdd(int publishmentSystemId, string returnUrl)
	    {
	        return PageUtils.GetOpenWindowString("添加部门",
	            PageUtils.GetWcmUrl(nameof(ModalGovPublicDepartmentAdd), new NameValueCollection
	            {
	                {"PublishmentSystemID", publishmentSystemId.ToString()},
	                {"ReturnUrl", StringUtils.ValueToUrl(returnUrl)}
	            }), 460, 360);
	    }

	    public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            _returnUrl = StringUtils.ValueFromUrl(Request.QueryString["ReturnUrl"]);
            if (string.IsNullOrEmpty(_returnUrl))
            {
                _returnUrl = PageGovPublicDepartment.GetRedirectUrl(PublishmentSystemId);
            }

			if (!IsPostBack)
			{
                ParentID.Items.Add(new ListItem("<无上级部门>", "0"));

                var departmentIdList = GovPublicManager.GetAllDepartmentIdList(PublishmentSystemInfo);
                var count = departmentIdList.Count;
                _isLastNodeArray = new bool[count];
                foreach (var theDepartmentId in departmentIdList)
                {
                    var departmentInfo = DepartmentManager.GetDepartmentInfo(theDepartmentId);
                    var listitem = new ListItem(GetTitle(departmentInfo.DepartmentId, departmentInfo.DepartmentName, departmentInfo.ParentsCount, departmentInfo.IsLastNode), theDepartmentId.ToString());
                    ParentID.Items.Add(listitem);
                }

				
			}
		}

        public string GetTitle(int departmentID, string departmentName, int parentsCount, bool isLastNode)
        {
            var str = "";
            if (isLastNode == false)
            {
                _isLastNodeArray[parentsCount] = false;
            }
            else
            {
                _isLastNodeArray[parentsCount] = true;
            }
            for (var i = 0; i < parentsCount; i++)
            {
                if (_isLastNodeArray[i])
                {
                    str = string.Concat(str, "　");
                }
                else
                {
                    str = string.Concat(str, "│");
                }
            }
            if (isLastNode)
            {
                str = string.Concat(str, "└");
            }
            else
            {
                str = string.Concat(str, "├");
            }
            str = string.Concat(str, departmentName);
            return str;
        }

        public override void Submit_OnClick(object sender, EventArgs e)
        {
            var isChanged = false;

            try
            {
                var departmentInfo = new DepartmentInfo();
                departmentInfo.DepartmentName = DepartmentName.Text;
                departmentInfo.Code = Code.Text;
                departmentInfo.ParentId = TranslateUtils.ToInt(ParentID.SelectedValue);
                departmentInfo.Summary = Summary.Text;

                BaiRongDataProvider.DepartmentDao.Insert(departmentInfo);

                Body.AddAdminLog("维护部门信息");

                SuccessMessage("部门设置成功！");
                isChanged = true;
            }
            catch (Exception ex)
            {
                FailMessage(ex, "部门设置失败！");
            }

            if (isChanged)
            {
                PageUtils.CloseModalPageAndRedirect(Page, _returnUrl);
            }
        }
	}
}
