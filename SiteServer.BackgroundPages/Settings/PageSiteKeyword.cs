using System;
using System.Collections.Specialized;
using System.Text;
using System.Web.UI.WebControls;
using SiteServer.Utils;
using SiteServer.BackgroundPages.Controls;
using SiteServer.BackgroundPages.Core;
using SiteServer.CMS.Core;
using SiteServer.CMS.DataCache;
using SiteServer.CMS.Model;
using SiteServer.CMS.Model.Enumerations;
using SiteServer.Utils.Enumerations;

namespace SiteServer.BackgroundPages.Settings
{
    public class PageSiteKeyword : BasePageCms
    {
        public Repeater RptContents;
        public SqlPager SpContents;

        public Button BtnAdd;
        public Button BtnImport;

        public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            if (AuthRequest.IsQueryExists("Delete") && AuthRequest.IsQueryExists("KeywordID"))
            {
                var keywordId = AuthRequest.GetQueryInt("KeywordID");
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

            SpContents.ControlToPaginate = RptContents;
            SpContents.SelectCommand = DataProvider.KeywordDao.GetSelectCommand();
            RptContents.ItemDataBound += RptContents_ItemDataBound;
            SpContents.SortField = nameof(KeywordInfo.Id);
            SpContents.SortMode = SortMode.DESC; //排序
            SpContents.ItemsPerPage = 20;

            if (IsPostBack) return;

            VerifySystemPermissions(ConfigManager.SettingsPermissions.Site);

            SpContents.DataBind();
            BtnAdd.Attributes.Add("onclick", ModalKeywordAdd.GetOpenWindowStringToAdd());
            BtnImport.Attributes.Add("onclick", ModalKeywordImport.GetOpenWindowString());
        }

        private void RptContents_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType != ListItemType.Item && e.Item.ItemType != ListItemType.AlternatingItem) return;

            var keywordId = SqlUtils.EvalInt(e.Item.DataItem, nameof(KeywordInfo.Id));
            var keyword = SqlUtils.EvalString(e.Item.DataItem, nameof(KeywordInfo.Keyword));
            var alternative = SqlUtils.EvalString(e.Item.DataItem, nameof(KeywordInfo.Alternative));
            var grade = EKeywordGradeUtils.GetEnumType(SqlUtils.EvalString(e.Item.DataItem, nameof(KeywordInfo.Grade)));

            var ltlKeyword = (Literal)e.Item.FindControl("ltlKeyword");
            var ltlAlternative = (Literal)e.Item.FindControl("ltlAlternative");
            var ltlGrade = (Literal)e.Item.FindControl("ltlGrade");
            var ltlEdit = (Literal)e.Item.FindControl("ltlEdit");
            var ltlDelete = (Literal)e.Item.FindControl("ltlDelete");

            ltlKeyword.Text = keyword;
            ltlAlternative.Text = alternative;
            ltlGrade.Text = EKeywordGradeUtils.GetText(grade);
            ltlEdit.Text =
                $@"<a href='javascript:;' onclick=""{ModalKeywordAdd.GetOpenWindowStringToEdit(keywordId)}"">编辑</a>";

            var urlDelete = PageUtils.GetSettingsUrl(nameof(PageSiteKeyword), new NameValueCollection
            {
                {"Delete", "True"},
                {"KeywordID", keywordId.ToString()}
            });
            ltlDelete.Text =
                $@"<a href=""{urlDelete}"" onClick=""javascript:return confirm('此操作将删除关键字“{keyword}”确认吗？')"";>删除</a>";
        }

        protected void ExportWord_Click(object sender, EventArgs e)
        {
            var sbContent = new StringBuilder();
            var list = DataProvider.KeywordDao.GetKeywordInfoList();
            if (list.Count <= 0) return;

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
