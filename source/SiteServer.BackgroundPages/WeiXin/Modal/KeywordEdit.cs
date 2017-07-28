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
    public class KeywordEdit : BackgroundBasePage
	{
        public TextBox tbKeyword;
        public DropDownList ddlMatchType;
        public CheckBox cbIsEnabled;

        private int keywordID;
        private string keyword;

        public static string GetOpenWindowString(int publishmentSystemID, int keywordID, string keyword)
        {
            var arguments = new NameValueCollection();
            arguments.Add("publishmentSystemID", publishmentSystemID.ToString());
            arguments.Add("keywordID", keywordID.ToString());
            arguments.Add("keyword", keyword.ToString());
            return PageUtilityWX.GetOpenWindowString("编辑关键词", "modal_keywordEdit.aspx", arguments);
        }

		public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            keywordID = TranslateUtils.ToInt(GetQueryString("keywordID"));
            keyword = GetQueryString("keyword");

			if (!IsPostBack)
			{
                EMatchTypeUtils.AddListItems(ddlMatchType);

                cbIsEnabled.Checked = true;

                if (keywordID > 0)
                {
                    var keywordInfo = DataProviderWX.KeywordDAO.GetKeywordInfo(keywordID);

                    tbKeyword.Text = keyword;
                    ControlUtils.SelectListItems(ddlMatchType, EMatchTypeUtils.GetValue(keywordInfo.MatchType));
                    cbIsEnabled.Checked = !keywordInfo.IsDisabled;
                }
			}
		}

        public override void Submit_OnClick(object sender, EventArgs e)
        {
            var isChanged = false;

            try
            {
                var conflictKeywords = string.Empty;
                if (KeywordManager.IsKeywordUpdateConflict(PublishmentSystemID, keywordID, tbKeyword.Text, out conflictKeywords))
                {
                    FailMessage($"关键词“{conflictKeywords}”已存在，请设置其他关键词");
                }
                else
                {
                    var keywordInfo = DataProviderWX.KeywordDAO.GetKeywordInfo(keywordID);
                    var keywordList = TranslateUtils.StringCollectionToStringList(keywordInfo.Keywords, ' ');
                    var i = keywordList.IndexOf(keyword);
                    if (i != -1)
                    {
                        keywordList[i] = tbKeyword.Text;
                    }
                    keywordInfo.Keywords = TranslateUtils.ObjectCollectionToString(keywordList, " ");
                    keywordInfo.IsDisabled = !cbIsEnabled.Checked;
                    keywordInfo.MatchType = EMatchTypeUtils.GetEnumType(ddlMatchType.SelectedValue);

                    DataProviderWX.KeywordDAO.Update(keywordInfo);

                    StringUtility.AddLog(PublishmentSystemID, "编辑关键词", $"关键词:{keyword}");

                    isChanged = true;
                }
            }
            catch (Exception ex)
            {
                FailMessage(ex, "失败：" + ex.Message);
            }

            if (isChanged)
            {
                JsUtils.OpenWindow.CloseModalPage(Page);
            }
		}
	}
}
