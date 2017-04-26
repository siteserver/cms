using System;
using System.Web.UI.WebControls;
using System.Collections.Specialized;

using BaiRong.Core;
using SiteServer.CMS.BackgroundPages;

using SiteServer.WeiXin.Core;
using SiteServer.WeiXin.Model;
using SiteServer.CMS.Core;

namespace SiteServer.WeiXin.BackgroundPages.Modal
{
    public class KeywordAddNews : BackgroundBasePage
	{
        public TextBox tbKeywords;
        public DropDownList ddlMatchType;
        public CheckBox cbIsEnabled;
        public PlaceHolder phSelect;
        public CheckBox cbIsSelect;

        private int keywordID;
        private bool isSingle;

        public static string GetOpenWindowStringToAdd(int publishmentSystemID, bool isSingle)
        {
            var arguments = new NameValueCollection();
            arguments.Add("publishmentSystemID", publishmentSystemID.ToString());
            arguments.Add("isSingle", isSingle.ToString());
            return PageUtilityWX.GetOpenWindowString("添加图文回复关键词", "modal_keywordAddNews.aspx", arguments);
        }

        public static string GetOpenWindowStringToEdit(int publishmentSystemID, int keywordID)
        {
            var arguments = new NameValueCollection();
            arguments.Add("publishmentSystemID", publishmentSystemID.ToString());
            arguments.Add("keywordID", keywordID.ToString());
            return PageUtilityWX.GetOpenWindowString("编辑图文回复关键词", "modal_keywordAddNews.aspx", arguments);
        }

		public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            keywordID = TranslateUtils.ToInt(GetQueryString("keywordID"));
            isSingle = TranslateUtils.ToBool(GetQueryString("isSingle"));

			if (!IsPostBack)
			{
                EMatchTypeUtils.AddListItems(ddlMatchType);
                cbIsEnabled.Checked = true;

                if (keywordID > 0)
                {
                    var keywordInfo = DataProviderWX.KeywordDAO.GetKeywordInfo(keywordID);

                    tbKeywords.Text = keywordInfo.Keywords;
                    ControlUtils.SelectListItems(ddlMatchType, EMatchTypeUtils.GetValue(keywordInfo.MatchType));
                    cbIsEnabled.Checked = !keywordInfo.IsDisabled;
                }
                else
                {
                    phSelect.Visible = true;
                }
			}
		}

        public override void Submit_OnClick(object sender, EventArgs e)
        {
            var isChanged = false;
            var keywordIDNew = 0;

            try
            {
                if (keywordID == 0)
                {
                    var conflictKeywords = string.Empty;
                    if (KeywordManager.IsKeywordInsertConflict(PublishmentSystemID, tbKeywords.Text, out conflictKeywords))
                    {
                        FailMessage($"触发关键词“{conflictKeywords}”已存在，请设置其他关键词");
                    }
                    else
                    {
                        var keywordInfo = new KeywordInfo();

                        keywordInfo.KeywordID = 0;
                        keywordInfo.PublishmentSystemID = PublishmentSystemID;
                        keywordInfo.Keywords = tbKeywords.Text;
                        keywordInfo.IsDisabled = !cbIsEnabled.Checked;
                        keywordInfo.KeywordType = EKeywordType.News;
                        keywordInfo.MatchType = EMatchTypeUtils.GetEnumType(ddlMatchType.SelectedValue);
                        keywordInfo.Reply = string.Empty;
                        keywordInfo.AddDate = DateTime.Now;
                        keywordInfo.Taxis = 0;

                        keywordIDNew = DataProviderWX.KeywordDAO.Insert(keywordInfo);

                        StringUtility.AddLog(PublishmentSystemID, "添加图文回复关键词", $"关键词:{tbKeywords.Text}");

                        isChanged = true;
                    }
                }
                else
                {
                    var conflictKeywords = string.Empty;
                    if (KeywordManager.IsKeywordUpdateConflict(PublishmentSystemID, keywordID, tbKeywords.Text, out conflictKeywords))
                    {
                        FailMessage($"触发关键词“{conflictKeywords}”已存在，请设置其他关键词");
                    }
                    else
                    {
                        var keywordInfo = DataProviderWX.KeywordDAO.GetKeywordInfo(keywordID);
                        keywordInfo.Keywords = tbKeywords.Text;
                        keywordInfo.IsDisabled = !cbIsEnabled.Checked;
                        keywordInfo.MatchType = EMatchTypeUtils.GetEnumType(ddlMatchType.SelectedValue);

                        DataProviderWX.KeywordDAO.Update(keywordInfo);

                        StringUtility.AddLog(PublishmentSystemID, "编辑图文回复关键词", $"关键词:{tbKeywords.Text}");

                        isChanged = true;
                    }
                }
            }
            catch (Exception ex)
            {
                FailMessage(ex, "失败：" + ex.Message);
            }

            if (isChanged)
            {
                if (keywordID == 0)
                {
                    if (cbIsSelect.Checked)
                    {
                        PageUtils.Redirect(ContentSelect.GetRedirectUrlByKeywordAddList(PublishmentSystemID, !isSingle, keywordIDNew));
                    }
                    else
                    {
                        JsUtils.OpenWindow.CloseModalPageAndRedirect(Page, BackgroundKeywordNewsAdd.GetRedirectUrl(PublishmentSystemID, keywordIDNew, 0, isSingle));
                    }
                }
                else
                {
                    JsUtils.OpenWindow.CloseModalPage(Page);
                }
            }
		}
	}
}
