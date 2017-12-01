using System;
using System.Web.UI.WebControls;
using BaiRong.Core;
using SiteServer.CMS.Core;
using SiteServer.CMS.Model;
using SiteServer.CMS.Model.Enumerations;

namespace SiteServer.BackgroundPages.Settings
{
    public class ModalKeywordImport : BasePageCms
    {
        public DropDownList ddlGrade;
        public TextBox tbKeywords;

        public static string GetOpenWindowString()
        {
            return PageUtils.GetOpenWindowString("导入敏感词",
                PageUtils.GetSettingsUrl(nameof(ModalKeywordImport), null), 500, 500);
        }

        public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;
            if (!IsPostBack)
            {
                EKeywordGradeUtils.AddListItems(ddlGrade);
            }
        }

        public override void Submit_OnClick(object sender, EventArgs e)
        {
            var isChanged = false;

            try
            {
                var grade = EKeywordGradeUtils.GetEnumType(ddlGrade.SelectedValue);

                var keywordArray = tbKeywords.Text.Split(',');
                foreach (var item in keywordArray)
                {
                    if (!string.IsNullOrEmpty(item))
                    {
                        var value = item.Trim();
                        var keyword = string.Empty;
                        var alternative = string.Empty;

                        if (value.IndexOf('|') != -1)
                        {
                            keyword = value.Split('|')[0];
                            alternative = value.Split('|')[1];
                        }
                        else
                        {
                            keyword = value;
                        }

                        if (!string.IsNullOrEmpty(keyword) && !DataProvider.KeywordDao.IsExists(keyword))
                        {
                            var keywordInfo = new KeywordInfo(0, keyword, alternative, grade);
                            DataProvider.KeywordDao.Insert(keywordInfo);
                        }
                    }
                }

                isChanged = true;
            }
            catch (Exception ex)
            {
                FailMessage(ex, "导入敏感词失败");
            }

            if (isChanged)
            {
                PageUtils.CloseModalPage(Page);
            }
        }
    }
}
