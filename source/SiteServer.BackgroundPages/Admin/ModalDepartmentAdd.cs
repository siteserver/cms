using System;
using System.Collections.Specialized;
using System.Web.UI.WebControls;
using BaiRong.Core;
using BaiRong.Core.Model;
using BaiRong.Core.Text;

namespace SiteServer.BackgroundPages.Admin
{
	public class ModalDepartmentAdd : BasePage
    {
        public TextBox DepartmentName;
        public TextBox Code;
        public PlaceHolder phParentID;
        public DropDownList ParentID;
        public TextBox Summary;

        private int _departmentId;
        private string _returnUrl = string.Empty;
        private bool[] _isLastNodeArray;

        public static string GetOpenWindowStringToAdd(string returnUrl)
        {
            return PageUtils.GetOpenWindowString("添加部门",
                PageUtils.GetAdminUrl(nameof(ModalDepartmentAdd), new NameValueCollection
                {
                    {"ReturnUrl", StringUtils.ValueToUrl(returnUrl)}
                }), 460, 380);
        }

        public static string GetOpenWindowStringToEdit(int departmentId, string returnUrl)
        {
            return PageUtils.GetOpenWindowString("修改部门",
                PageUtils.GetAdminUrl(nameof(ModalDepartmentAdd), new NameValueCollection
                {
                    {"DepartmentID", departmentId.ToString()},
                    {"ReturnUrl", StringUtils.ValueToUrl(returnUrl)}
                }), 460, 380);
        }

		public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            _departmentId = Body.GetQueryInt("DepartmentID");
            _returnUrl = StringUtils.ValueFromUrl(Body.GetQueryString("ReturnUrl"));
            if (string.IsNullOrEmpty(_returnUrl))
            {
                _returnUrl = PageDepartment.GetRedirectUrl(0);
            }

			if (!IsPostBack)
			{
                if (_departmentId == 0)
                {
                    ParentID.Items.Add(new ListItem("<无上级部门>", "0"));

                    var departmentIdList = DepartmentManager.GetDepartmentIdList();
                    var count = departmentIdList.Count;
                    _isLastNodeArray = new bool[count];
                    foreach (var theDepartmentId in departmentIdList)
                    {
                        var departmentInfo = DepartmentManager.GetDepartmentInfo(theDepartmentId);
                        var listitem = new ListItem(GetTitle(departmentInfo.DepartmentId, departmentInfo.DepartmentName, departmentInfo.ParentsCount, departmentInfo.IsLastNode), theDepartmentId.ToString());
                        ParentID.Items.Add(listitem);
                    }
                }
                else
                {
                    phParentID.Visible = false;
                }

                if (_departmentId != 0)
                {
                    var departmentInfo = DepartmentManager.GetDepartmentInfo(_departmentId);

                    DepartmentName.Text = departmentInfo.DepartmentName;
                    Code.Text = departmentInfo.Code;
                    ParentID.SelectedValue = departmentInfo.ParentId.ToString();
                    Summary.Text = departmentInfo.Summary;
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
                if (_departmentId == 0)
                {
                    var departmentInfo = new DepartmentInfo();
                    departmentInfo.DepartmentName = DepartmentName.Text;
                    departmentInfo.Code = Code.Text;
                    departmentInfo.ParentId = TranslateUtils.ToInt(ParentID.SelectedValue);
                    departmentInfo.Summary = Summary.Text;

                    BaiRongDataProvider.DepartmentDao.Insert(departmentInfo);
                }
                else
                {
                    var departmentInfo = DepartmentManager.GetDepartmentInfo(_departmentId);

                    departmentInfo.DepartmentName = DepartmentName.Text;
                    departmentInfo.Code = Code.Text;
                    departmentInfo.ParentId = TranslateUtils.ToInt(ParentID.SelectedValue);
                    departmentInfo.Summary = Summary.Text;

                    BaiRongDataProvider.DepartmentDao.Update(departmentInfo);
                }

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
