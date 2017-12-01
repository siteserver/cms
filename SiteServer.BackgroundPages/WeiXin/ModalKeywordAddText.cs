using System;
using System.Collections.Specialized;
using System.Web.UI.WebControls;
using BaiRong.Core;
using SiteServer.BackgroundPages.Cms;
using SiteServer.CMS.WeiXin.Data;
using SiteServer.CMS.WeiXin.Manager;
using SiteServer.CMS.WeiXin.Model;
using SiteServer.CMS.WeiXin.Model.Enumerations;

namespace SiteServer.BackgroundPages.WeiXin
{
    public class ModalKeywordAddText : BasePageCms
    {
        public TextBox TbKeywords;
        public DropDownList DdlMatchType;
        public CheckBox CbIsEnabled;
        public TextBox TbReply;

        public Button BtnContentSelect;
        public Button BtnChannelSelect;

        private int _keywordId;

        public static string GetOpenWindowStringToAdd(int publishmentSystemId)
        {
            return PageUtils.GetOpenWindowString("添加文本回复关键词",
                PageUtils.GetWeiXinUrl(nameof(ModalKeywordAddText), new NameValueCollection
                {
                    {"publishmentSystemId", publishmentSystemId.ToString()}
                }));
        }

        public static string GetOpenWindowStringToEdit(int publishmentSystemId, int keywordId)
        {
            return PageUtils.GetOpenWindowString("编辑文本回复关键词",
                PageUtils.GetWeiXinUrl(nameof(ModalKeywordAddText), new NameValueCollection
                {
                    {"publishmentSystemId", publishmentSystemId.ToString()},
                    {"keywordId", keywordId.ToString()}
                }));
        }

        public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            _keywordId = Body.GetQueryInt("keywordID");

            if (!IsPostBack)
            {
                EMatchTypeUtils.AddListItems(DdlMatchType);

                CbIsEnabled.Checked = true;

                BtnContentSelect.Attributes.Add("onclick", ModalContentSelect.GetOpenWindowString(PublishmentSystemId, false, "contentSelect"));
                BtnChannelSelect.Attributes.Add("onclick", ModalChannelSelect.GetOpenWindowString(PublishmentSystemId, true));

                if (_keywordId > 0)
                {
                    var keywordInfo = DataProviderWx.KeywordDao.GetKeywordInfo(_keywordId);

                    TbKeywords.Text = keywordInfo.Keywords;
                    ControlUtils.SelectListItems(DdlMatchType, EMatchTypeUtils.GetValue(keywordInfo.MatchType));
                    CbIsEnabled.Checked = !keywordInfo.IsDisabled;
                    TbReply.Text = keywordInfo.Reply;
                }
            }
        }

        public override void Submit_OnClick(object sender, EventArgs e)
        {
            var isChanged = false;

            try
            {
                if (_keywordId == 0)
                {
                    var conflictKeywords = string.Empty;
                    if (KeywordManager.IsKeywordInsertConflict(PublishmentSystemId, PageUtils.FilterSql(TbKeywords.Text), out conflictKeywords))
                    {
                        FailMessage($"触发关键词“{conflictKeywords}”已存在，请设置其他关键词");
                    }
                    else
                    {
                        var keywordInfo = new KeywordInfo();


                        keywordInfo.KeywordId = 0;
                        keywordInfo.PublishmentSystemId = PublishmentSystemId;
                        keywordInfo.Keywords = PageUtils.FilterSql(TbKeywords.Text);
                        keywordInfo.IsDisabled = !CbIsEnabled.Checked;
                        keywordInfo.KeywordType = EKeywordType.Text;
                        keywordInfo.MatchType = EMatchTypeUtils.GetEnumType(DdlMatchType.SelectedValue);
                        keywordInfo.Reply = TbReply.Text;
                        keywordInfo.AddDate = DateTime.Now;
                        keywordInfo.Taxis = 0;

                        DataProviderWx.KeywordDao.Insert(keywordInfo);

                        Body.AddSiteLog(PublishmentSystemId, "添加文本回复关键词", $"关键词:{TbKeywords.Text}");

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
                        keywordInfo.Reply = TbReply.Text;

                        DataProviderWx.KeywordDao.Update(keywordInfo);

                        Body.AddSiteLog(PublishmentSystemId, "编辑文本回复关键词", $"关键词:{TbKeywords.Text}");

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
                PageUtils.CloseModalPage(Page);
            }
        }
    }
}
