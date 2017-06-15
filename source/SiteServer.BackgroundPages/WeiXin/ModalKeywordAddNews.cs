using System;
using System.Collections.Specialized;
using System.Web.UI.WebControls;
using BaiRong.Core;
using SiteServer.CMS.WeiXin.Data;
using SiteServer.CMS.WeiXin.Manager;
using SiteServer.CMS.WeiXin.Model;
using SiteServer.CMS.WeiXin.Model.Enumerations;

namespace SiteServer.BackgroundPages.WeiXin
{
    public class ModalKeywordAddNews : BasePageCms
    {
        public TextBox TbKeywords;
        public DropDownList DdlMatchType;
        public CheckBox CbIsEnabled;
        public PlaceHolder PhSelect;
        public CheckBox CbIsSelect;

        private int _keywordId;
        private bool _isSingle;

        public static string GetOpenWindowStringToAdd(int publishmentSystemId, bool isSingle)
        {
            return PageUtils.GetOpenWindowString("添加图文回复关键词",
                PageUtils.GetWeiXinUrl(nameof(ModalKeywordAddNews), new NameValueCollection
                {
                    {"publishmentSystemId", publishmentSystemId.ToString()},
                    {"isSingle", isSingle.ToString()}
                }));
        }

        public static string GetOpenWindowStringToEdit(int publishmentSystemId, int keywordId)
        {
            return PageUtils.GetOpenWindowString("编辑图文回复关键词",
                PageUtils.GetWeiXinUrl(nameof(ModalKeywordAddNews), new NameValueCollection
                {
                    {"publishmentSystemId", publishmentSystemId.ToString()},
                    {"keywordId", keywordId.ToString()}
                }));
        }

		public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            _keywordId = Body.GetQueryInt("keywordID");
            _isSingle = TranslateUtils.ToBool(Body.GetQueryString("isSingle"));

			if (!IsPostBack)
			{
                EMatchTypeUtils.AddListItems(DdlMatchType);
                CbIsEnabled.Checked = true;

                if (_keywordId > 0)
                {
                    var keywordInfo = DataProviderWx.KeywordDao.GetKeywordInfo(_keywordId);

                    TbKeywords.Text = keywordInfo.Keywords;
                    ControlUtils.SelectListItems(DdlMatchType, EMatchTypeUtils.GetValue(keywordInfo.MatchType));
                    CbIsEnabled.Checked = !keywordInfo.IsDisabled;
                }
                else
                {
                    PhSelect.Visible = true;
                }
			}
		}

        public override void Submit_OnClick(object sender, EventArgs e)
        {
            var isChanged = false;
            var keywordIdNew = 0;

            try
            {
                if (_keywordId == 0)
                {
                    var conflictKeywords = string.Empty;
                    if (KeywordManager.IsKeywordInsertConflict(PublishmentSystemId, TbKeywords.Text, out conflictKeywords))
                    {
                        FailMessage($"触发关键词“{conflictKeywords}”已存在，请设置其他关键词");
                    }
                    else
                    {
                        var keywordInfo = new KeywordInfo();

                        keywordInfo.KeywordId = 0;
                        keywordInfo.PublishmentSystemId = PublishmentSystemId;
                        keywordInfo.Keywords = TbKeywords.Text;
                        keywordInfo.IsDisabled = !CbIsEnabled.Checked;
                        keywordInfo.KeywordType = EKeywordType.News;
                        keywordInfo.MatchType = EMatchTypeUtils.GetEnumType(DdlMatchType.SelectedValue);
                        keywordInfo.Reply = string.Empty;
                        keywordInfo.AddDate = DateTime.Now;
                        keywordInfo.Taxis = 0;

                        keywordIdNew = DataProviderWx.KeywordDao.Insert(keywordInfo);

                        Body.AddSiteLog(PublishmentSystemId, "添加图文回复关键词", $"关键词:{TbKeywords.Text}");

                        isChanged = true;
                    }
                }
                else
                {
                    var conflictKeywords = string.Empty;
                    if (KeywordManager.IsKeywordUpdateConflict(PublishmentSystemId, _keywordId, TbKeywords.Text, out conflictKeywords))
                    {
                        FailMessage($"触发关键词“{conflictKeywords}”已存在，请设置其他关键词");
                    }
                    else
                    {
                        var keywordInfo = DataProviderWx.KeywordDao.GetKeywordInfo(_keywordId);
                        keywordInfo.Keywords = TbKeywords.Text;
                        keywordInfo.IsDisabled = !CbIsEnabled.Checked;
                        keywordInfo.MatchType = EMatchTypeUtils.GetEnumType(DdlMatchType.SelectedValue);

                        DataProviderWx.KeywordDao.Update(keywordInfo);

                        Body.AddSiteLog(PublishmentSystemId, "编辑图文回复关键词", $"关键词:{TbKeywords.Text}");

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
                if (_keywordId == 0)
                {
                    if (CbIsSelect.Checked)
                    {
                        PageUtils.Redirect(ModalContentSelect.GetRedirectUrlByKeywordAddList(PublishmentSystemId, !_isSingle, keywordIdNew));
                    }
                    else
                    {
                        PageUtils.CloseModalPageAndRedirect(Page, PageKeywordNewsAdd.GetRedirectUrl(PublishmentSystemId, keywordIdNew, 0, _isSingle));
                    }
                }
                else
                {
                    PageUtils.CloseModalPage(Page);
                }
            }
		}
	}
}
