using System;
using System.Collections.Specialized;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using SiteServer.Abstractions;
using SiteServer.BackgroundPages.Core;
using SiteServer.CMS.Context;
using SiteServer.CMS.Core;
using SiteServer.CMS.Core.Create;
using SiteServer.CMS.Core.Office;
using SiteServer.CMS.DataCache;
using SiteServer.CMS.Repositories;
using Content = SiteServer.Abstractions.Content;
using WebUtils = SiteServer.BackgroundPages.Core.WebUtils;

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

        private Channel _channel;
        private string _returnUrl;

        public static string GetOpenWindowString(int siteId, int channelId, string returnUrl)
        {
            return LayerUtils.GetOpenScript("批量导入Word文件",
                PageUtils.GetCmsUrl(siteId, nameof(ModalUploadWord), new NameValueCollection
                {
                    {"channelId", channelId.ToString()},
                    {"returnUrl", returnUrl}
                }), 700, 550);
        }

        public string UploadUrl => ModalUploadWordHandler.GetRedirectUrl(SiteId);

        public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            PageUtils.CheckRequestParameter("siteId", "ReturnUrl");
            var channelId = int.Parse(AuthRequest.GetQueryString("channelId"));
            _channel = ChannelManager.GetChannelAsync(SiteId, channelId).GetAwaiter().GetResult();
            _returnUrl = AuthRequest.GetQueryString("ReturnUrl");

            if (IsPostBack) return;

            var (isChecked, checkedLevel) = CheckManager.GetUserCheckLevelAsync(AuthRequest.AdminPermissionsImpl, Site, SiteId).GetAwaiter().GetResult();
            CheckManager.LoadContentLevelToEdit(DdlContentLevel, Site, null, isChecked, checkedLevel);
            ControlUtils.SelectSingleItem(DdlContentLevel, CheckManager.LevelInt.CaoGao.ToString());
        }

        public override void Submit_OnClick(object sender, EventArgs e)
        {
            if (!Page.IsPostBack || !Page.IsValid) return;

            var fileNames = StringUtils.GetStringList(HihFileNames.Value);
            if (fileNames.Count == 1)
            {
                var fileName = fileNames[0];
                if (!string.IsNullOrEmpty(fileName))
                {
                    var redirectUrl = WebUtils.GetContentAddUploadWordUrl(SiteId, _channel, CbIsFirstLineTitle.Checked, CbIsFirstLineRemove.Checked, CbIsClearFormat.Checked, CbIsFirstLineIndent.Checked, CbIsClearFontSize.Checked, CbIsClearFontFamily.Checked, CbIsClearImages.Checked, TranslateUtils.ToIntWithNagetive(DdlContentLevel.SelectedValue), fileName, _returnUrl);
                    LayerUtils.CloseAndRedirect(Page, redirectUrl);
                }

                return;
            }

            if (fileNames.Count > 1)
            {
                var styleList = TableStyleManager.GetContentStyleListAsync(Site, _channel).GetAwaiter().GetResult();

                foreach (var fileName in fileNames)
                {
                    if (!string.IsNullOrEmpty(fileName))
                    {
                        var formCollection = WordUtils.GetWordNameValueCollectionAsync(SiteId, CbIsFirstLineTitle.Checked, CbIsFirstLineRemove.Checked, CbIsClearFormat.Checked, CbIsFirstLineIndent.Checked, CbIsClearFontSize.Checked, CbIsClearFontFamily.Checked, CbIsClearImages.Checked, fileName).GetAwaiter().GetResult();

                        if (!string.IsNullOrEmpty(formCollection[ContentAttribute.Title]))
                        {
                            var dict = BackgroundInputTypeParser.SaveAttributesAsync(Site, styleList, formCollection, ContentAttribute.AllAttributes.Value).GetAwaiter().GetResult();

                            var contentInfo = new Content(dict)
                            {
                                ChannelId = _channel.Id,
                                SiteId = SiteId,
                                AddUserName = AuthRequest.AdminName,
                                AddDate = DateTime.Now
                            };

                            contentInfo.LastEditUserName = contentInfo.AddUserName;
                            contentInfo.LastEditDate = contentInfo.AddDate;

                            contentInfo.CheckedLevel = TranslateUtils.ToIntWithNagetive(DdlContentLevel.SelectedValue);
                            contentInfo.Checked = contentInfo.CheckedLevel >= Site.CheckContentLevel;

                            contentInfo.Title = formCollection[ContentAttribute.Title];

                            contentInfo.Id = DataProvider.ContentRepository.InsertAsync(Site, _channel, contentInfo).GetAwaiter().GetResult();

                            CreateManager.CreateContentAsync(SiteId, _channel.Id, contentInfo.Id).GetAwaiter().GetResult();
                            CreateManager.TriggerContentChangedEventAsync(SiteId, _channel.Id).GetAwaiter().GetResult();
                        }
                    }
                }
            }

            LayerUtils.Close(Page);
        }
    }
}
