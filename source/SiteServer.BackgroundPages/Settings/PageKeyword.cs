using System;
using System.Collections.Specialized;
using System.Text;
using System.Web.UI.WebControls;
using BaiRong.Core;
using BaiRong.Core.Data;
using BaiRong.Core.Model.Enumerations;
using SiteServer.BackgroundPages.Controls;
using SiteServer.BackgroundPages.Core;
using SiteServer.CMS.Core;
using SiteServer.CMS.Model.Enumerations;

namespace SiteServer.BackgroundPages.Settings
{
    public class PageKeyword : BasePageCms
    {
        public Repeater rptContents;
        public SqlPager spContents;

        public Button btnAdd;
        public Button btnImport;

        public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            if (Body.IsQueryExists("Delete") && Body.IsQueryExists("KeywordID"))
            {
                var keywordId = Body.GetQueryInt("KeywordID");
                try
                {
                    DataProvider.KeywordDao.Delete(keywordId);
                    SuccessDeleteMessage();
                }
                catch (Exception ex)
                {
                    FailDeleteMessage(ex);
                }
            }

            spContents.ControlToPaginate = rptContents;
            spContents.SelectCommand = DataProvider.KeywordDao.GetSelectCommand();
            rptContents.ItemDataBound += rptContents_ItemDataBound;
            spContents.SortField = "KeywordID";
            spContents.SortMode = SortMode.DESC; //排序
            spContents.ItemsPerPage = 20;

            if (!IsPostBack)
            {
                BreadCrumbSettings(AppManager.Settings.LeftMenu.Config, "敏感词管理", AppManager.Settings.Permission.SettingsConfig);

                spContents.DataBind();
                btnAdd.Attributes.Add("onclick", ModalKeywordAdd.GetOpenWindowStringToAdd());
                btnImport.Attributes.Add("onclick", ModalKeywordImport.GetOpenWindowString());
            }
        }

        void rptContents_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                var keywordID = SqlUtils.EvalInt(e.Item.DataItem, "KeywordID");
                var keyword = SqlUtils.EvalString(e.Item.DataItem, "Keyword");
                var alternative = SqlUtils.EvalString(e.Item.DataItem, "Alternative");
                var grade = EKeywordGradeUtils.GetEnumType(SqlUtils.EvalString(e.Item.DataItem, "Grade"));

                var ltlKeyword = e.Item.FindControl("ltlKeyword") as Literal;
                var ltlAlternative = e.Item.FindControl("ltlAlternative") as Literal;
                var ltlGrade = e.Item.FindControl("ltlGrade") as Literal;
                var ltlEdit = e.Item.FindControl("ltlEdit") as Literal;
                var ltlDelete = e.Item.FindControl("ltlDelete") as Literal;

                ltlKeyword.Text = keyword;
                ltlAlternative.Text = alternative;
                ltlGrade.Text = EKeywordGradeUtils.GetText(grade);
                ltlEdit.Text =
                    $@"<a href='javascript:;' onclick=""{ModalKeywordAdd.GetOpenWindowStringToEdit(keywordID)}"">编辑</a>";

                var urlDelete = PageUtils.GetSettingsUrl(nameof(PageKeyword), new NameValueCollection
                {
                    {"Delete", "True"},
                    {"KeywordID", keywordID.ToString()}
                });
                ltlDelete.Text =
                    $@"<a href=""{urlDelete}"" onClick=""javascript:return confirm('此操作将删除关键字“{keyword}”确认吗？')"";>删除</a>";
            }
        }

        protected void ExportWord_Click(object sender, EventArgs e)
        {
            var sbContent = new StringBuilder();
            var list = DataProvider.KeywordDao.GetKeywordInfoList();
            if (list.Count > 0)
            {
                foreach (var keywordInfo in list)
                {
                    sbContent.Append(keywordInfo.Keyword);
                    if (!string.IsNullOrEmpty(keywordInfo.Alternative))
                    {
                        sbContent.Append("|");
                        sbContent.Append(keywordInfo.Alternative);
                    }
                    sbContent.Append(",");
                }
                if (sbContent.Length > 0) sbContent.Length -= 1;

                var filePath = PathUtils.GetTemporaryFilesPath("敏感词.txt");
                FileUtils.DeleteFileIfExists(filePath);
                FileUtils.WriteText(filePath, ECharset.utf_8, sbContent.ToString());
                PageUtils.Download(Page.Response, filePath);
            }
        }
    }
}
