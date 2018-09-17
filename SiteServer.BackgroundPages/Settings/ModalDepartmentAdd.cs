using System;
using System.Collections.Specialized;
using System.Web.UI.WebControls;
using SiteServer.CMS.Core;
using SiteServer.CMS.DataCache;
using SiteServer.CMS.Model;
using SiteServer.Utils;

namespace SiteServer.BackgroundPages.Settings
{
	public class ModalDepartmentAdd : BasePage
    {
        public TextBox TbDepartmentName;
        public TextBox TbCode;
        public PlaceHolder PhParentId;
        public DropDownList DdlParentId;
        public TextBox TbSummary;

        private int _departmentId;
        private string _returnUrl = string.Empty;
        private bool[] _isLastNodeArray;

        public static string GetOpenWindowStringToAdd(string returnUrl)
        {
            return LayerUtils.GetOpenScript("添加部门",
                PageUtils.GetSettingsUrl(nameof(ModalDepartmentAdd), new NameValueCollection
                {
                    {"ReturnUrl", StringUtils.ValueToUrl(returnUrl)}
                }), 460, 400);
        }

        public static string GetOpenWindowStringToEdit(int departmentId, string returnUrl)
        {
            return LayerUtils.GetOpenScript("修改部门",
                PageUtils.GetSettingsUrl(nameof(ModalDepartmentAdd), new NameValueCollection
                {
                    {"DepartmentID", departmentId.ToString()},
                    {"ReturnUrl", StringUtils.ValueToUrl(returnUrl)}
                }), 460, 400);
        }

		public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            _departmentId = AuthRequest.GetQueryInt("DepartmentID");
            _returnUrl = StringUtils.ValueFromUrl(AuthRequest.GetQueryString("ReturnUrl"));
            if (string.IsNullOrEmpty(_returnUrl))
            {
                _returnUrl = PageAdminDepartment.GetRedirectUrl(0);
            }

            if (IsPostBack) return;

            if (_departmentId == 0)
            {
                DdlParentId.Items.Add(new ListItem("<无上级部门>", "0"));

                var departmentIdList = DepartmentManager.GetDepartmentIdList();
                var count = departmentIdList.Count;
                _isLastNodeArray = new bool[count];
                foreach (var theDepartmentId in departmentIdList)
                {
                    var departmentInfo = DepartmentManager.GetDepartmentInfo(theDepartmentId);
                    var listitem = new ListItem(GetTitle(departmentInfo.Id, departmentInfo.DepartmentName, departmentInfo.ParentsCount, departmentInfo.IsLastNode), theDepartmentId.ToString());
                    DdlParentId.Items.Add(listitem);
                }
            }
            else
            {
                PhParentId.Visible = false;
            }

            if (_departmentId != 0)
            {
                var departmentInfo = DepartmentManager.GetDepartmentInfo(_departmentId);

                TbDepartmentName.Text = departmentInfo.DepartmentName;
                TbCode.Text = departmentInfo.Code;
                DdlParentId.SelectedValue = departmentInfo.ParentId.ToString();
                TbSummary.Text = departmentInfo.Summary;
            }
        }

        public string GetTitle(int departmentId, string departmentName, int parentsCount, bool isLastNode)
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
                str = string.Concat(str, _isLastNodeArray[i] ? "　" : "│");
            }
            str = string.Concat(str, isLastNode ? "└" : "├");
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
                    var departmentInfo = new DepartmentInfo
                    {
                        DepartmentName = TbDepartmentName.Text,
                        Code = TbCode.Text,
                        ParentId = TranslateUtils.ToInt(DdlParentId.SelectedValue),
                        Summary = TbSummary.Text
                    };

                    DataProvider.DepartmentDao.Insert(departmentInfo);
                }
                else
                {
                    var departmentInfo = DepartmentManager.GetDepartmentInfo(_departmentId);

                    departmentInfo.DepartmentName = TbDepartmentName.Text;
                    departmentInfo.Code = TbCode.Text;
                    departmentInfo.ParentId = TranslateUtils.ToInt(DdlParentId.SelectedValue);
                    departmentInfo.Summary = TbSummary.Text;

                    DataProvider.DepartmentDao.Update(departmentInfo);
                }

                AuthRequest.AddAdminLog("维护部门信息");

                SuccessMessage("部门设置成功！");
                isChanged = true;
            }
            catch (Exception ex)
            {
                FailMessage(ex, "部门设置失败！");
            }

            if (isChanged)
            {
                LayerUtils.CloseAndRedirect(Page, _returnUrl);
            }
        }
	}
}
