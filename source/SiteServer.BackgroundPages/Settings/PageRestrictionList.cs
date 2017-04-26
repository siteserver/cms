using System;
using System.Collections.Specialized;
using System.Web.UI.WebControls;
using BaiRong.Core;

namespace SiteServer.BackgroundPages.Settings
{
    public class PageRestrictionList : BasePage
    {
		public DataGrid MyDataGrid;
        public Button AddButton;
        private string type;

		public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            type = Body.GetQueryString("Type");

			if (Body.IsQueryExists("Delete"))
			{
                var restriction = Body.GetQueryString("Restriction");
			
				try
				{
                    if (type == "Black")
                    {
                        var stringCollection = ConfigManager.Instance.RestrictionBlackList;
                        stringCollection.Remove(restriction);
                        ConfigManager.Instance.RestrictionBlackList = stringCollection;
                    }
                    else
                    {
                        var stringCollection = ConfigManager.Instance.RestrictionWhiteList;
                        stringCollection.Remove(restriction);
                        ConfigManager.Instance.RestrictionWhiteList = stringCollection;
                    }
                    BaiRongDataProvider.ConfigDao.Update(ConfigManager.Instance);
                    
					SuccessDeleteMessage();
				}
				catch(Exception ex)
				{
					FailDeleteMessage(ex);
				}
			}
			if (!IsPostBack)
			{
                var pageTitle = (type == "Black") ? "黑名单" : "白名单";
                BreadCrumbSettings(AppManager.Settings.LeftMenu.Restriction, pageTitle, AppManager.Settings.Permission.SettingsRestriction);

				BindGrid();

                AddButton.Attributes.Add("onclick", ModalRestrictionAdd.GetOpenWindowStringToAdd(0, type));
			}
		}

		public void BindGrid()
		{
			try
			{
                if (type == "Black")
                {
                    MyDataGrid.DataSource = ConfigManager.Instance.RestrictionBlackList;
                }
                else
                {
                    MyDataGrid.DataSource = ConfigManager.Instance.RestrictionWhiteList;
                }
                
                MyDataGrid.ItemDataBound += MyDataGrid_ItemDataBound;
				MyDataGrid.DataBind();
			}
			catch(Exception ex)
			{
                PageUtils.RedirectToErrorPage(ex.Message);
			}
		}

        void MyDataGrid_ItemDataBound(object sender, DataGridItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                var restriction = e.Item.DataItem as string;
                var listItem = e.Item.FindControl("ListItem") as Literal;
                var editUrl = e.Item.FindControl("EditUrl") as Literal;
                var deleteUrl = e.Item.FindControl("DeleteUrl") as Literal;
                listItem.Text = restriction;

                var showPopWinString = ModalRestrictionAdd.GetOpenWindowStringToEdit(0, type, restriction);
                editUrl.Text = $"<a href=\"javascript:;\" onClick=\"{showPopWinString}\">修改</a>";

                var urlDelete = PageUtils.GetSettingsUrl(nameof(PageRestrictionList), new NameValueCollection
                {
                    {"Delete", "True"},
                    {"Type", type},
                    {"Restriction", restriction}
                });

                deleteUrl.Text =
                    $"<a href=\"{urlDelete}\" onClick=\"javascript:return confirm('此操作将删除IP访问规则“{restriction}”，确认吗？');\">删除</a>";
            }
        }

	}
}
