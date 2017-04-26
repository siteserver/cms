using System;
using System.Web.UI.WebControls;
using System.Collections.Specialized;

using BaiRong.Core;
using SiteServer.CMS.BackgroundPages;

using SiteServer.WeiXin.Core;
using SiteServer.WeiXin.Model;


namespace SiteServer.WeiXin.BackgroundPages.Modal
{
    public class StoreCategoryAdd : BackgroundBasePage
    {
        public TextBox CategoryName;
        public PlaceHolder phParentID;
        public DropDownList ParentID;

        private int categoryID = 0;
        private bool[] isLastNodeArray;

        public static string GetOpenWindowStringToAdd(int publishmentSystemID)
        {
            var arguments = new NameValueCollection();
            arguments.Add("publishmentSystemID", publishmentSystemID.ToString());
            return PageUtilityWX.GetOpenWindowString("添加门店属性", "modal_storeCategoryAdd.aspx", arguments, 400, 300);
        }

        public static string GetOpenWindowStringToEdit(int publishmentSystemID, int storeCategoryID)
        {
            var arguments = new NameValueCollection();
            arguments.Add("publishmentSystemID", publishmentSystemID.ToString());
            arguments.Add("storeCategoryID", storeCategoryID.ToString());
            return PageUtilityWX.GetOpenWindowString("编辑门店属性", "modal_storeCategoryAdd.aspx", arguments, 400, 300);
        }

        public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            categoryID = TranslateUtils.ToInt(Request.QueryString["storeCategoryID"]);

            if (!IsPostBack)
            {
                if (categoryID == 0)
                {
                    ParentID.Items.Add(new ListItem("<无上级区域>", "0"));

                    var categoryIDList = DataProviderWX.StoreCategoryDAO.GetAllCategoryIDList(PublishmentSystemID);
                    var count = categoryIDList.Count;
                    if (count > 0)
                    {
                        isLastNodeArray = new bool[count];
                        foreach (var theCategoryID in categoryIDList)
                        {
                            var categoryInfo = DataProviderWX.StoreCategoryDAO.GetCategoryInfo(theCategoryID);
                            var listitem = new ListItem(GetTitle(categoryInfo.ID, categoryInfo.CategoryName, categoryInfo.ParentsCount, categoryInfo.IsLastNode), theCategoryID.ToString());
                            ParentID.Items.Add(listitem);
                        }
                    }
                }
                else
                {
                    phParentID.Visible = false;
                }

                if (categoryID != 0)
                {
                    var categoryInfo = DataProviderWX.StoreCategoryDAO.GetCategoryInfo(categoryID);
                    CategoryName.Text = categoryInfo.CategoryName;
                    ParentID.SelectedValue = categoryInfo.ParentID.ToString();
                }
            }
        }

        public string GetTitle(int categoryID, string categoryName, int parentsCount, bool isLastNode)
        {
            var str = "";
            if (isLastNodeArray.Length > parentsCount)
            {
                if (isLastNode == false)
                {
                    isLastNodeArray[parentsCount] = false;
                }
                else
                {
                    isLastNodeArray[parentsCount] = true;
                }
            }

            for (var i = 0; i < parentsCount; i++)
            {
                if (isLastNodeArray[i])
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
                if (categoryID == 0)
                {
                    var categoryInfo = new StoreCategoryInfo();
                    categoryInfo.PublishmentSystemID = PublishmentSystemID;
                    categoryInfo.CategoryName = CategoryName.Text;
                    categoryInfo.ParentID = TranslateUtils.ToInt(ParentID.SelectedValue);

                    DataProviderWX.StoreCategoryDAO.Insert(PublishmentSystemID, categoryInfo);
                }
                else
                {
                    var categoryInfo = DataProviderWX.StoreCategoryDAO.GetCategoryInfo(categoryID);
                    categoryInfo.CategoryName = CategoryName.Text;
                    categoryInfo.ParentID = TranslateUtils.ToInt(ParentID.SelectedValue);

                    DataProviderWX.StoreCategoryDAO.Update(PublishmentSystemID, categoryInfo); ;
                }

                LogUtils.AddLog(BaiRongDataProvider.AdministratorDao.UserName, "维护门店属性信息");

                SuccessMessage("类别设置成功！");
                isChanged = true;
            }
            catch (Exception ex)
            {
                FailMessage(ex, "类别设置失败！");
            }

            if (isChanged)
            {
                JsUtils.OpenWindow.CloseModalPage(Page);
            }
        }
    }
}
