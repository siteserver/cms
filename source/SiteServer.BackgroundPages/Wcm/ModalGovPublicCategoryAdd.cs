using System;
using System.Collections.Specialized;
using System.Web.UI.WebControls;
using BaiRong.Core;
using BaiRong.Core.Text;
using SiteServer.CMS.Core;
using SiteServer.CMS.Model;
using SiteServer.CMS.Wcm.Model;

namespace SiteServer.BackgroundPages.Wcm
{
	public class ModalGovPublicCategoryAdd : BasePageCms
	{
        public TextBox CategoryName;
        public TextBox CategoryCode;
        public PlaceHolder phParentID;
        public DropDownList ParentID;
        public TextBox Summary;

        private string _classCode = string.Empty;
        private int _categoryId;
        private string _returnUrl = string.Empty;
        private bool[] _isLastNodeArray;

        public static string GetOpenWindowStringToAdd(string classCode, int publishmentSystemId, string returnUrl)
        {
            return PageUtils.GetOpenWindowString("添加节点", PageUtils.GetWcmUrl(nameof(ModalGovPublicCategoryAdd), new NameValueCollection
            {
                {"ClassCode", classCode},
                {"PublishmentSystemID", publishmentSystemId.ToString()},
                {"ReturnUrl", StringUtils.ValueToUrl(returnUrl)}
            }), 500, 340);
        }

        public static string GetOpenWindowStringToEdit(string classCode, int publishmentSystemId, int categoryId, string returnUrl)
        {
            return PageUtils.GetOpenWindowString("修改节点", PageUtils.GetWcmUrl(nameof(ModalGovPublicCategoryAdd), new NameValueCollection
            {
                {"ClassCode", classCode},
                {"PublishmentSystemID", publishmentSystemId.ToString()},
                {"CategoryID", categoryId.ToString()},
                {"ReturnUrl", StringUtils.ValueToUrl(returnUrl)}
            }), 520, 320);
        }

		public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            _classCode = Request.QueryString["ClassCode"];
            _categoryId = TranslateUtils.ToInt(Request.QueryString["CategoryID"]);
            _returnUrl = StringUtils.ValueFromUrl(Request.QueryString["ReturnUrl"]);
            if (string.IsNullOrEmpty(_returnUrl))
            {
                _returnUrl = PageGovPublicCategory.GetRedirectUrl(PublishmentSystemId, _classCode, _categoryId);
            }

			if (!IsPostBack)
			{
                if (_categoryId == 0)
                {
                    ParentID.Items.Add(new ListItem("<无上级节点>", "0"));

                    var categoryIDArrayList = DataProvider.GovPublicCategoryDao.GetCategoryIdArrayList(_classCode, PublishmentSystemId);
                    var count = categoryIDArrayList.Count;
                    _isLastNodeArray = new bool[count];
                    foreach (int theCategoryID in categoryIDArrayList)
                    {
                        var categoryInfo = DataProvider.GovPublicCategoryDao.GetCategoryInfo(theCategoryID);
                        var listitem = new ListItem(GetTitle(categoryInfo.CategoryID, categoryInfo.CategoryName, categoryInfo.ParentsCount, categoryInfo.IsLastNode), theCategoryID.ToString());
                        ParentID.Items.Add(listitem);
                    }
                }
                else
                {
                    phParentID.Visible = false;
                }

                if (_categoryId != 0)
                {
                    var categoryInfo = DataProvider.GovPublicCategoryDao.GetCategoryInfo(_categoryId);

                    CategoryName.Text = categoryInfo.CategoryName;
                    CategoryCode.Text = categoryInfo.CategoryCode;
                    ParentID.SelectedValue = categoryInfo.ParentID.ToString();
                    Summary.Text = categoryInfo.Summary;
                }

				
			}
		}

        public string GetTitle(int categoryID, string departmentName, int parentsCount, bool isLastNode)
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
                if (_categoryId == 0)
                {
                    var categoryInfo = new GovPublicCategoryInfo(0, _classCode, PublishmentSystemId, CategoryName.Text, CategoryCode.Text, TranslateUtils.ToInt(ParentID.SelectedValue), string.Empty, 0, 0, false, 0, DateTime.Now, Summary.Text, 0);

                    DataProvider.GovPublicCategoryDao.Insert(categoryInfo);
                }
                else
                {
                    var categoryInfo = DataProvider.GovPublicCategoryDao.GetCategoryInfo(_categoryId);

                    categoryInfo.CategoryName = CategoryName.Text;
                    categoryInfo.CategoryCode = CategoryCode.Text;
                    categoryInfo.Summary = Summary.Text;

                    DataProvider.GovPublicCategoryDao.Update(categoryInfo);
                }

                Body.AddAdminLog("维护分类信息");

                SuccessMessage("分类设置成功！");
                isChanged = true;
            }
            catch (Exception ex)
            {
                FailMessage(ex, "分类设置失败！");
            }

            if (isChanged)
            {
                PageUtils.CloseModalPageAndRedirect(Page, _returnUrl);
            }
        }
	}
}
