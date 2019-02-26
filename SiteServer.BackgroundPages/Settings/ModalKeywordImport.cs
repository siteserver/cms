using System;
using System.Web.UI.WebControls;
using SiteServer.Utils;
using SiteServer.CMS.Core.Enumerations;
using SiteServer.CMS.Database.Core;
using SiteServer.CMS.Database.Models;

namespace SiteServer.BackgroundPages.Settings
{
    public class ModalKeywordImport : BasePageCms
    {
        public DropDownList DdlGrade;
        public TextBox TbKeywords;

        public static string GetOpenWindowString()
        {
            return LayerUtils.GetOpenScript("导入敏感词",
                PageUtils.GetSettingsUrl(nameof(ModalKeywordImport), null), 500, 530);
        }

        public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;
            if (!IsPostBack)
            {
                EKeywordGradeUtils.AddListItems(DdlGrade);
            }
        }

        public override void Submit_OnClick(object sender, EventArgs e)
        {
            var isChanged = false;

            try
            {
                var grade = EKeywordGradeUtils.GetEnumType(DdlGrade.SelectedValue);

                var keywordArray = TbKeywords.Text.Split(',');
                foreach (var item in keywordArray)
                {
                    if (!string.IsNullOrEmpty(item))
                    {
                        var value = item.Trim();
                        string keyword;
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

                        if (!string.IsNullOrEmpty(keyword) && !DataProvider.Keyword.IsExists(keyword))
                        {
                            var keywordInfo = new KeywordInfo
                            {
                                Keyword = keyword,
                                Alternative = alternative,
                                KeywordGrade = grade
                            };
                            DataProvider.Keyword.Insert(keywordInfo);
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
                LayerUtils.Close(Page);
            }
        }
    }
}
