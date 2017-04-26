using System;
using System.Collections.Specialized;
using System.Web.UI.WebControls;
using BaiRong.Core;
using SiteServer.CMS.Core;
using SiteServer.CMS.Model;
using SiteServer.CMS.Model.Enumerations;

namespace SiteServer.BackgroundPages.Settings
{
    public class ModalKeywordAdd : BasePageCms
    {
        protected TextBox tbKeyword;
        protected TextBox tbAlternative;
        protected DropDownList ddlGrade;

        private int _keywordId;

        public static string GetOpenWindowStringToAdd()
        {
            return PageUtils.GetOpenWindowString("添加敏感词", PageUtils.GetSettingsUrl(nameof(ModalKeywordAdd), null), 380, 300);
        }

        public static string GetOpenWindowStringToEdit(int keywordId)
        {
            return PageUtils.GetOpenWindowString("修改敏感词",
                PageUtils.GetSettingsUrl(nameof(ModalKeywordAdd), new NameValueCollection
                {
                    {"KeywordID", keywordId.ToString()}
                }), 380, 300);
        }

        public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            _keywordId = Body.GetQueryInt("KeywordID");

            if (!IsPostBack)
            {
                EKeywordGradeUtils.AddListItems(ddlGrade);
                if (_keywordId > 0)
                {
                    var keywordInfo = DataProvider.KeywordDao.GetKeywordInfo(_keywordId);
                    tbKeyword.Text = keywordInfo.Keyword;
                    tbAlternative.Text = keywordInfo.Alternative;
                    ControlUtils.SelectListItems(ddlGrade, EKeywordGradeUtils.GetValue(keywordInfo.Grade));
                }
            }
        }

        public override void Submit_OnClick(object sender, EventArgs e)
        {
            var isChanged = false;

            if (_keywordId > 0)
            {
                try
                {
                    var keywordInfo = DataProvider.KeywordDao.GetKeywordInfo(_keywordId);
                    keywordInfo.Keyword = tbKeyword.Text.Trim();
                    keywordInfo.Alternative = tbAlternative.Text.Trim();
                    keywordInfo.Grade = EKeywordGradeUtils.GetEnumType(ddlGrade.SelectedValue);
                    DataProvider.KeywordDao.Update(keywordInfo);

                    isChanged = true;
                }
                catch (Exception ex)
                {
                    FailMessage(ex, "修改敏感词失败！");
                }
            }
            else
            {
                if (DataProvider.KeywordDao.IsExists(tbKeyword.Text))
                {
                    FailMessage("敏感词添加失败，敏感词名称已存在！");
                }
                else
                {
                    try
                    {
                        var keywordInfo = new KeywordInfo
                        {
                            Keyword = tbKeyword.Text.Trim(),
                            Alternative = tbAlternative.Text.Trim(),
                            Grade = EKeywordGradeUtils.GetEnumType(ddlGrade.SelectedValue)
                        };
                        DataProvider.KeywordDao.Insert(keywordInfo);
                        isChanged = true;
                    }
                    catch (Exception ex)
                    {
                        FailMessage(ex, "添加敏感词失败！");
                    }
                }
            }

            if (isChanged)
            {
                PageUtils.CloseModalPage(Page);
            }
        }
    }
}
