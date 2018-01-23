using System;
using System.Collections.Specialized;
using System.Web.UI.WebControls;
using SiteServer.Utils;
using SiteServer.BackgroundPages.Ajax;
using SiteServer.BackgroundPages.Core;
using SiteServer.CMS.Core;
using SiteServer.CMS.Core.Create;
using SiteServer.CMS.Core.Office;
using SiteServer.CMS.Model;

namespace SiteServer.BackgroundPages.Cms
{
    public class ModalUploadWord : BasePageCms
    {
        public CheckBox CbIsFirstLineTitle;
        public CheckBox CbIsFirstLineRemove;
        public CheckBox CbIsClearFormat;
        public CheckBox CbIsFirstLineIndent;
        public CheckBox CbIsClearFontSize;
        public CheckBox CbIsClearFontFamily;
        public CheckBox CbIsClearImages;
        public DropDownList DdlContentLevel;

        private ChannelInfo _nodeInfo;
        private string _returnUrl;

        public static string GetOpenWindowString(int siteId, int nodeId, string returnUrl)
        {
            return LayerUtils.GetOpenScript("批量导入Word文件",
                PageUtils.GetCmsUrl(siteId, nameof(ModalUploadWord), new NameValueCollection
                {
                    {"nodeID", nodeId.ToString()},
                    {"returnUrl", returnUrl}
                }), 600, 400);
        }

        public string GetUploadWordMultipleUrl()
        {
            return AjaxUploadService.GetUploadWordMultipleUrl(SiteId);
        }

        public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            PageUtils.CheckRequestParameter("siteId", "ReturnUrl");
            var nodeId = int.Parse(Body.GetQueryString("NodeID"));
            _nodeInfo = ChannelManager.GetChannelInfo(SiteId, nodeId);
            _returnUrl = Body.GetQueryString("ReturnUrl");

            if (IsPostBack) return;

            int checkedLevel;
            var isChecked = CheckManager.GetUserCheckLevel(Body.AdminName, SiteInfo, SiteId, out checkedLevel);
            CheckManager.LoadContentLevelToEdit(DdlContentLevel, SiteInfo, _nodeInfo.Id, null, isChecked, checkedLevel);
            ControlUtils.SelectSingleItem(DdlContentLevel, CheckManager.LevelInt.CaoGao.ToString());
        }

        public override void Submit_OnClick(object sender, EventArgs e)
        {
            if (!Page.IsPostBack || !Page.IsValid) return;

            var fileCount = TranslateUtils.ToInt(Request.Form["File_Count"]);
            if (fileCount == 1)
            {
                var fileName = Request.Form["fileName_1"];
                var redirectUrl = WebUtils.GetContentAddUploadWordUrl(SiteId, _nodeInfo, CbIsFirstLineTitle.Checked, CbIsFirstLineRemove.Checked, CbIsClearFormat.Checked, CbIsFirstLineIndent.Checked, CbIsClearFontSize.Checked, CbIsClearFontFamily.Checked, CbIsClearImages.Checked, TranslateUtils.ToIntWithNagetive(DdlContentLevel.SelectedValue), fileName, _returnUrl);
                LayerUtils.CloseAndRedirect(Page, redirectUrl);

                return;
            }
            if (fileCount > 1)
            {
                var tableName = ChannelManager.GetTableName(SiteInfo, _nodeInfo);
                var relatedIdentities = RelatedIdentities.GetChannelRelatedIdentities(SiteId, _nodeInfo.Id);
                var styleInfoList = TableStyleManager.GetTableStyleInfoList(tableName, relatedIdentities);

                for (var index = 1; index <= fileCount; index++)
                {
                    var fileName = Request.Form["fileName_" + index];
                    if (!string.IsNullOrEmpty(fileName))
                    {
                        var formCollection = WordUtils.GetWordNameValueCollection(SiteId, CbIsFirstLineTitle.Checked, CbIsFirstLineRemove.Checked, CbIsClearFormat.Checked, CbIsFirstLineIndent.Checked, CbIsClearFontSize.Checked, CbIsClearFontFamily.Checked, CbIsClearImages.Checked, TranslateUtils.ToInt(DdlContentLevel.SelectedValue), fileName);

                        if (!string.IsNullOrEmpty(formCollection[ContentAttribute.Title]))
                        {
                            var contentInfo = new ContentInfo();

                            BackgroundInputTypeParser.SaveAttributes(contentInfo, SiteInfo, styleInfoList, formCollection, ContentAttribute.AllAttributesLowercase);

                            contentInfo.ChannelId = _nodeInfo.Id;
                            contentInfo.SiteId = SiteId;
                            contentInfo.AddUserName = Body.AdminName;
                            contentInfo.AddDate = DateTime.Now;
                            contentInfo.LastEditUserName = contentInfo.AddUserName;
                            contentInfo.LastEditDate = contentInfo.AddDate;

                            contentInfo.CheckedLevel = TranslateUtils.ToIntWithNagetive(DdlContentLevel.SelectedValue);
                            contentInfo.IsChecked = contentInfo.CheckedLevel >= SiteInfo.Additional.CheckContentLevel;

                            contentInfo.Id = DataProvider.ContentDao.Insert(tableName, SiteInfo, contentInfo);

                            if (contentInfo.IsChecked)
                            {
                                CreateManager.CreateContentAndTrigger(SiteId, _nodeInfo.Id, contentInfo.Id);
                            }
                        }
                    }
                }
            }

            LayerUtils.Close(Page);
        }
    }
}
