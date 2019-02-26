using System;
using System.Collections.Specialized;
using System.Web.UI.WebControls;
using SiteServer.Utils;
using SiteServer.CMS.Core.Enumerations;
using SiteServer.CMS.Database.Core;
using SiteServer.CMS.Database.Models;

namespace SiteServer.BackgroundPages.Settings
{
    public class ModalKeywordAdd : BasePageCms
    {
        protected TextBox TbKeyword;
        protected TextBox TbAlternative;
        protected DropDownList DdlGrade;

        private int _keywordId;

        public static string GetOpenWindowStringToAdd()
        {
            return LayerUtils.GetOpenScript("添加敏感词", PageUtils.GetSettingsUrl(nameof(ModalKeywordAdd), null), 460, 300);
        }

        public static string GetOpenWindowStringToEdit(int keywordId)
        {
            return LayerUtils.GetOpenScript("修改敏感词",
                PageUtils.GetSettingsUrl(nameof(ModalKeywordAdd), new NameValueCollection
                {
                    {"KeywordID", keywordId.ToString()}
                }), 460, 300);
        }

        public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            _keywordId = AuthRequest.GetQueryInt("KeywordID");

            if (IsPostBack) return;

            EKeywordGradeUtils.AddListItems(DdlGrade);
            if (_keywordId <= 0) return;

            var keywordInfo = DataProvider.Keyword.Get(_keywordId);
            TbKeyword.Text = keywordInfo.Keyword;
            TbAlternative.Text = keywordInfo.Alternative;
            ControlUtils.SelectSingleItem(DdlGrade, EKeywordGradeUtils.GetValue(keywordInfo.KeywordGrade));
        }

        public override void Submit_OnClick(object sender, EventArgs e)
        {
            var isChanged = false;

            if (_keywordId > 0)
            {
                try
                {
                    var keywordInfo = DataProvider.Keyword.Get(_keywordId);
                    keywordInfo.Keyword = TbKeyword.Text.Trim();
                    keywordInfo.Alternative = TbAlternative.Text.Trim();
                    keywordInfo.KeywordGrade = EKeywordGradeUtils.GetEnumType(DdlGrade.SelectedValue);
                    DataProvider.Keyword.Update(keywordInfo);

                    isChanged = true;
                }
                catch (Exception ex)
                {
                    FailMessage(ex, "修改敏感词失败！");
                }
            }
            else
            {
                if (DataProvider.Keyword.IsExists(TbKeyword.Text))
                {
                    FailMessage("敏感词添加失败，敏感词名称已存在！");
                }
                else
                {
                    try
                    {
                        var keywordInfo = new KeywordInfo
                        {
                            Keyword = TbKeyword.Text.Trim(),
                            Alternative = TbAlternative.Text.Trim(),
                            KeywordGrade = EKeywordGradeUtils.GetEnumType(DdlGrade.SelectedValue)
                        };
                        DataProvider.Keyword.Insert(keywordInfo);
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
                LayerUtils.Close(Page);
            }
        }
    }
}
