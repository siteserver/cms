using System;
using System.Collections.Specialized;
using System.Web.UI.WebControls;
using BaiRong.Core;
using BaiRong.Core.Model;
using SiteServer.BackgroundPages.Controls;
using SiteServer.BackgroundPages.Core;

namespace SiteServer.BackgroundPages.User
{
    public class ModalUserSelect : BasePage
    {
        public TextBox Keyword;
        public DropDownList CreateDate;
        public DropDownList LastActivityDate;

        public Repeater rptContents;
        public SqlPager spContents;

        private string _textBoxId;

        public static string GetOpenWindowString(string textBoxId)
        {
            return PageUtils.GetOpenWindowString("选择用户", PageUtils.GetUserUrl(nameof(ModalUserSelect), new NameValueCollection
            {
                {"textBoxID", textBoxId}
            }));
        }

        public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            _textBoxId = Request.QueryString["textBoxID"];

            spContents.ControlToPaginate = rptContents;
            spContents.ItemsPerPage = 25;

            if (Request.QueryString["keyword"] == null)
            {
                spContents.SelectCommand = BaiRongDataProvider.UserDao.GetSelectCommand(true);
            }
            else
            {
                spContents.SelectCommand = BaiRongDataProvider.UserDao.GetSelectCommand(Request.QueryString["keyword"], TranslateUtils.ToInt(Request.QueryString["createDate"]), TranslateUtils.ToInt(Request.QueryString["lastActivityDate"]), true, 0, 0, string.Empty);
            }

            rptContents.ItemDataBound += rptContents_ItemDataBound;
            spContents.SortField = BaiRongDataProvider.UserDao.GetSortFieldName();
            spContents.SortMode = SortMode.DESC;

            if (!IsPostBack)
            {

                //this.ddlDepartmentID.Items.Add(new ListItem("<全部>", "0"));
                //int departmentID = TranslateUtils.ToInt(base.Request.QueryString["DepartmentID"]);
                //ArrayList departmentIDArrayList = DepartmentManager.GetDepartmentIDArrayList();
                //int count = departmentIDArrayList.Count;
                //this.isLastNodeArrayOfDepartment = new bool[count];
                //foreach (int theDepartmentID in departmentIDArrayList)
                //{
                //    DepartmentInfo departmentInfo = DepartmentManager.GetDepartmentInfo(theDepartmentID);
                //    ListItem listitem = new ListItem(this.GetDepartment(departmentInfo.DepartmentID, departmentInfo.DepartmentName, departmentInfo.ParentsCount, departmentInfo.IsLastNode), theDepartmentID.ToString());
                //    if (departmentID == theDepartmentID)
                //    {
                //        listitem.Selected = true;
                //    }
                //    this.ddlDepartmentID.Items.Add(listitem);
                //}

                //this.ddlAreaID.Items.Add(new ListItem("<全部>", "0"));
                //int areaID = TranslateUtils.ToInt(base.Request.QueryString["AreaID"]);
                //ArrayList areaIDArrayList = AreaManager.GetAreaIDArrayList();
                //count = areaIDArrayList.Count;
                //this.isLastNodeArrayOfArea = new bool[count];
                //foreach (int theAreaID in areaIDArrayList)
                //{
                //    AreaInfo areaInfo = AreaManager.GetAreaInfo(theAreaID);
                //    ListItem listitem = new ListItem(this.GetArea(areaInfo.AreaID, areaInfo.AreaName, areaInfo.ParentsCount, areaInfo.IsLastNode), theAreaID.ToString());
                //    if (areaID == theAreaID)
                //    {
                //        listitem.Selected = true;
                //    }
                //    this.ddlAreaID.Items.Add(listitem);
                //}

                spContents.DataBind();
            }
        }

        void rptContents_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                var userInfo = new UserInfo(e.Item.DataItem);

                var ltlUserName = (Literal)e.Item.FindControl("ltlUserName");
                var ltlDisplayName = (Literal)e.Item.FindControl("ltlDisplayName");
                var ltlLastActivityDate = (Literal)e.Item.FindControl("ltlLastActivityDate");
                var ltlCreateDate = (Literal)e.Item.FindControl("ltlCreateDate");
                var ltlSelect = (Literal)e.Item.FindControl("ltlSelect");

                ltlUserName.Text = GetUserNameHtml(userInfo.UserName, userInfo.IsLockedOut);
                ltlDisplayName.Text = userInfo.DisplayName;

                ltlLastActivityDate.Text = DateUtils.GetDateAndTimeString(userInfo.LastActivityDate);
                ltlCreateDate.Text = DateUtils.GetDateAndTimeString(userInfo.CreateDate);
                ltlSelect.Text =
                    $@"<input type=""checkbox"" name=""UserNameCollection"" value=""{userInfo.UserName}"" />";
            }
        }

        private string GetUserNameHtml(string userName, bool isLockedOut)
        {
            var showPopWinString = ModalUserView.GetOpenWindowString(userName);
            var state = string.Empty;
            if (isLockedOut)
            {
                state = @"<span style=""color:red;"">[已被锁定]</span>";
            }
            return $@"<a href=""javascript:;"" onclick=""{showPopWinString}"">{userName}</a>{state}";
        }

        public void Search_OnClick(object sender, EventArgs e)
        {
            PageUtils.Redirect(PageUrl);
        }

        public override void Submit_OnClick(object sender, EventArgs e)
        {
            var userNameArrayList = TranslateUtils.StringCollectionToStringList(Request.Form["UserNameCollection"]);

            if (userNameArrayList.Count == 0)
            {
                FailMessage("请勾选所需用户");
            }
            else
            {
                PageUtils.CloseModalPageWithoutRefresh(Page, $@"
var textBox = parent.$('#{_textBoxId}');
if (textBox.val()){{
    textBox.val(textBox.val() + ',{Request.Form["UserNameCollection"]}');
}}else{{
    textBox.val('{Request.Form["UserNameCollection"]}');
}}
");
            }
        }

        private string _pageUrl;
        private string PageUrl
        {
            get
            {
                if (string.IsNullOrEmpty(_pageUrl))
                {
                    _pageUrl = PageUtils.GetUserUrl(nameof(ModalUserSelect), new NameValueCollection
                    {
                        {"keyword", Keyword.Text },
                        {"createDate", CreateDate.SelectedValue },
                        {"lastActivityDate", LastActivityDate.SelectedValue }
                    });
                }
                return _pageUrl;
            }
        }
    }
}
