using System;
using System.Collections.Specialized;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using SiteServer.Utils;
using SiteServer.BackgroundPages.Core;
using SiteServer.CMS.Core;
using SiteServer.CMS.Core.Create;
using SiteServer.CMS.Core.Office;
using SiteServer.CMS.DataCache;
using SiteServer.CMS.Model;
using SiteServer.CMS.Model.Attributes;

namespace SiteServer.BackgroundPages.Cms
{
    public class ModalUploadWord : BasePageCms
    {
        public HtmlInputHidden HihFileNames;
        public CheckBox CbIsFirstLineTitle;
        public CheckBox CbIsFirstLineRemove;
        public CheckBox CbIsClearFormat;
        public CheckBox CbIsFirstLineIndent;
        public CheckBox CbIsClearFontSize;
        public CheckBox CbIsClearFontFamily;
        public CheckBox CbIsClearImages;
        public DropDownList DdlContentLevel;

        private ChannelInfo _channelInfo;
        private string _returnUrl;

        public static string GetOpenWindowString(int siteId, int channelId, string returnUrl)
        {
            return LayerUtils.GetOpenScript("批量导入Word文件",
                PageUtilsEx.GetCmsUrl(siteId, nameof(ModalUploadWord), new NameValueCollection
                {
                    {"channelId", channelId.ToString()},
                    {"returnUrl", returnUrl}
                }), 700, 550);
        }

        public string UploadUrl => ModalUploadWordHandler.GetRedirectUrl(SiteId);

        public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            FxUtils.CheckRequestParameter("siteId", "ReturnUrl");
            var channelId = int.Parse(AuthRequest.GetQueryString("channelId"));
            _channelInfo = ChannelManager.GetChannelInfo(SiteId, channelId);
            _returnUrl = AuthRequest.GetQueryString("ReturnUrl");

            if (IsPostBack) return;

            int checkedLevel;
            var isChecked = CheckManager.GetUserCheckLevel(AuthRequest.AdminPermissionsImpl, SiteInfo, SiteId, out checkedLevel);
            FxUtils.LoadContentLevelToEdit(DdlContentLevel, SiteInfo, null, isChecked, checkedLevel);
            ControlUtils.SelectSingleItem(DdlContentLevel, CheckManager.LevelInt.CaoGao.ToString());
        }

        public override void Submit_OnClick(object sender, EventArgs e)
        {
            if (!Page.IsPostBack || !Page.IsValid) return;

            var fileNames = TranslateUtils.StringCollectionToStringList(HihFileNames.Value);
            if (fileNames.Count == 1)
            {
                var fileName = fileNames[0];
                if (!string.IsNullOrEmpty(fileName))
                {
                    var redirectUrl = WebUtils.GetContentAddUploadWordUrl(SiteId, _channelInfo, CbIsFirstLineTitle.Checked, CbIsFirstLineRemove.Checked, CbIsClearFormat.Checked, CbIsFirstLineIndent.Checked, CbIsClearFontSize.Checked, CbIsClearFontFamily.Checked, CbIsClearImages.Checked, TranslateUtils.ToIntWithNagetive(DdlContentLevel.SelectedValue), fileName, _returnUrl);
                    LayerUtils.CloseAndRedirect(Page, redirectUrl);
                }

                return;
            }

            if (fileNames.Count > 1)
            {
                var tableName = ChannelManager.GetTableName(SiteInfo, _channelInfo);
                var styleInfoList = TableStyleManager.GetContentStyleInfoList(SiteInfo, _channelInfo);

                foreach (var fileName in fileNames)
                {
                    if (!string.IsNullOrEmpty(fileName))
                    {
                        var (title, content) = WordManager.GetWord(SiteInfo, CbIsFirstLineTitle.Checked, CbIsFirstLineRemove.Checked, CbIsClearFormat.Checked, CbIsFirstLineIndent.Checked, CbIsClearFontSize.Checked, CbIsClearFontFamily.Checked, CbIsClearImages.Checked, fileName);

                        var contentInfo = new ContentInfo
                        {
                            ChannelId = _channelInfo.Id,
                            SiteId = SiteId,
                            AdminId = AuthRequest.AdminId,
                            AddUserName = AuthRequest.AdminName,
                            AddDate = DateTime.Now,
                            CheckedLevel = TranslateUtils.ToIntWithNagetive(DdlContentLevel.SelectedValue),
                            Title = title,
                            Content = content
                        };

                        contentInfo.Checked = contentInfo.CheckedLevel >= SiteInfo.CheckContentLevel;
                        contentInfo.LastEditUserName = contentInfo.AddUserName;
                        contentInfo.LastEditDate = contentInfo.AddDate;

                        contentInfo.Id = _channelInfo.ContentDao.Insert(SiteInfo, _channelInfo, contentInfo);

                        CreateManager.CreateContent(SiteId, _channelInfo.Id, contentInfo.Id);
                        CreateManager.TriggerContentChangedEvent(SiteId, _channelInfo.Id);
                    }
                }
            }

            LayerUtils.Close(Page);
        }
    }
}
