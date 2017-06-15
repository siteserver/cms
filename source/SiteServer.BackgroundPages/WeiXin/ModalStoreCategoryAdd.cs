using System;
using System.Collections.Specialized;
using System.Web.UI.WebControls;
using BaiRong.Core;
using SiteServer.CMS.WeiXin.Data;
using SiteServer.CMS.WeiXin.Model;

namespace SiteServer.BackgroundPages.WeiXin
{
    public class ModalStoreCategoryAdd : BasePageCms
    {
        public TextBox CategoryName;
        public PlaceHolder PhParentId;
        public DropDownList ParentId;

        private int _categoryId = 0;
        private bool[] _isLastNodeArray;

        public static string GetOpenWindowStringToAdd(int publishmentSystemId)
        {
            return PageUtils.GetOpenWindowString("添加门店属性",
                PageUtils.GetWeiXinUrl(nameof(ModalStoreCategoryAdd), new NameValueCollection
                {
                    {"publishmentSystemId", publishmentSystemId.ToString()}
                }), 400, 300);
        }

        public static string GetOpenWindowStringToEdit(int publishmentSystemId, int storeCategoryId)
        {
            return PageUtils.GetOpenWindowString("编辑门店属性",
                PageUtils.GetWeiXinUrl(nameof(ModalStoreCategoryAdd), new NameValueCollection
                {
                    {"publishmentSystemId", publishmentSystemId.ToString()},
                    {"storeCategoryId", storeCategoryId.ToString()}
                }), 400, 300);
        }

        public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            _categoryId = TranslateUtils.ToInt(Request.QueryString["storeCategoryID"]);

            if (!IsPostBack)
            {
                if (_categoryId == 0)
                {
                    ParentId.Items.Add(new ListItem("<无上级区域>", "0"));

                    var categoryIdList = DataProviderWx.StoreCategoryDao.GetAllCategoryIdList(PublishmentSystemId);
                    var count = categoryIdList.Count;
                    if (count > 0)
                    {
                        _isLastNodeArray = new bool[count];
                        foreach (var theCategoryId in categoryIdList)
                        {
                            var categoryInfo = DataProviderWx.StoreCategoryDao.GetCategoryInfo(theCategoryId);
                            var listitem = new ListItem(GetTitle(categoryInfo.Id, categoryInfo.CategoryName, categoryInfo.ParentsCount, categoryInfo.IsLastNode), theCategoryId.ToString());
                            ParentId.Items.Add(listitem);
                        }
                    }
                }
                else
                {
                    PhParentId.Visible = false;
                }

                if (_categoryId != 0)
                {
                    var categoryInfo = DataProviderWx.StoreCategoryDao.GetCategoryInfo(_categoryId);
                    CategoryName.Text = categoryInfo.CategoryName;
                    ParentId.SelectedValue = categoryInfo.ParentId.ToString();
                }
            }
        }

        public string GetTitle(int categoryId, string categoryName, int parentsCount, bool isLastNode)
        {
            var str = "";
            if (_isLastNodeArray.Length > parentsCount)
            {
                if (isLastNode == false)
                {
                    _isLastNodeArray[parentsCount] = false;
                }
                else
                {
                    _isLastNodeArray[parentsCount] = true;
                }
            }

            for (var i = 0; i < parentsCount; i++)
            {
                if (_isLastNodeArray[i])
                {
                    str = String.Concat(str, "　");
                }
                else
                {
                    str = String.Concat(str, "│");
                }
            }
            if (isLastNode)
            {
                str = String.Concat(str, "└");
            }
            else
            {
                str = String.Concat(str, "├");
            }
            str = String.Concat(str, categoryName);
            return str;
        }

        public override void Submit_OnClick(object sender, EventArgs e)
        {
            var isChanged = false;

            try
            {
                if (_categoryId == 0)
                {
                    var categoryInfo = new StoreCategoryInfo();
                    categoryInfo.PublishmentSystemId = PublishmentSystemId;
                    categoryInfo.CategoryName = CategoryName.Text;
                    categoryInfo.ParentId = TranslateUtils.ToInt(ParentId.SelectedValue);

                    DataProviderWx.StoreCategoryDao.Insert(PublishmentSystemId, categoryInfo);
                }
                else
                {
                    var categoryInfo = DataProviderWx.StoreCategoryDao.GetCategoryInfo(_categoryId);
                    categoryInfo.CategoryName = CategoryName.Text;
                    categoryInfo.ParentId = TranslateUtils.ToInt(ParentId.SelectedValue);

                    DataProviderWx.StoreCategoryDao.Update(PublishmentSystemId, categoryInfo); ;
                }

                LogUtils.AddAdminLog(Body.AdministratorName, "维护门店属性信息");

                SuccessMessage("类别设置成功！");
                isChanged = true;
            }
            catch (Exception ex)
            {
                FailMessage(ex, "类别设置失败！");
            }

            if (isChanged)
            {
                PageUtils.CloseModalPage(Page);
            }
        }
    }
}
