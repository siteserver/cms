using System;
using System.Collections.Specialized;
using System.Web.UI.WebControls;
using BaiRong.Core;
using SiteServer.CMS.WeiXin.Data;
using SiteServer.CMS.WeiXin.Manager;
using SiteServer.CMS.WeiXin.Model.Enumerations;

namespace SiteServer.BackgroundPages.WeiXin
{
    public class ModalKeywordEdit : BasePageCms
    {
        public TextBox TbKeyword;
        public DropDownList DdlMatchType;
        public CheckBox CbIsEnabled;

        private int _keywordId;
        private string _keyword;

        public static string GetOpenWindowString(int publishmentSystemId, int keywordId, string keyword)
        {
            return PageUtils.GetOpenWindowString("编辑关键词",
                PageUtils.GetWeiXinUrl(nameof(ModalKeywordEdit), new NameValueCollection
                {
                    {"publishmentSystemId", publishmentSystemId.ToString()},
                    {"keywordId", keywordId.ToString()},
                    {"keyword", keyword}
                }));
        }

		public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            _keywordId = Body.GetQueryInt("keywordID");
            _keyword = Body.GetQueryString("keyword");

			if (!IsPostBack)
			{
                EMatchTypeUtils.AddListItems(DdlMatchType);

                CbIsEnabled.Checked = true;

                if (_keywordId > 0)
                {
                    var keywordInfo = DataProviderWx.KeywordDao.GetKeywordInfo(_keywordId);

                    TbKeyword.Text = _keyword;
                    ControlUtils.SelectListItems(DdlMatchType, EMatchTypeUtils.GetValue(keywordInfo.MatchType));
                    CbIsEnabled.Checked = !keywordInfo.IsDisabled;
                }
			}
		}

        public override void Submit_OnClick(object sender, EventArgs e)
        {
            var isChanged = false;

            try
            {
                var conflictKeywords = string.Empty;
                if (KeywordManager.IsKeywordUpdateConflict(PublishmentSystemId, _keywordId, TbKeyword.Text, out conflictKeywords))
                {
                    FailMessage($"关键词“{conflictKeywords}”已存在，请设置其他关键词");
                }
                else
                {
                    var keywordInfo = DataProviderWx.KeywordDao.GetKeywordInfo(_keywordId);
                    var keywordList = TranslateUtils.StringCollectionToStringList(keywordInfo.Keywords, ' ');
                    var i = keywordList.IndexOf(_keyword);
                    if (i != -1)
                    {
                        keywordList[i] = TbKeyword.Text;
                    }
                    keywordInfo.Keywords = TranslateUtils.ObjectCollectionToString(keywordList, " ");
                    keywordInfo.IsDisabled = !CbIsEnabled.Checked;
                    keywordInfo.MatchType = EMatchTypeUtils.GetEnumType(DdlMatchType.SelectedValue);

                    DataProviderWx.KeywordDao.Update(keywordInfo);

                    Body.AddSiteLog(PublishmentSystemId, "编辑关键词", $"关键词:{_keyword}");

                    isChanged = true;
                }
            }
            catch (Exception ex)
            {
                FailMessage(ex, "失败：" + ex.Message);
            }

            if (isChanged)
            {
                PageUtils.CloseModalPage(Page);
            }
		}
	}
}
