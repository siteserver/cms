using System;
using System.Collections.Specialized;
using System.Web.UI.WebControls;
using BaiRong.Core;
using BaiRong.Core.Model.Attributes;
using SiteServer.BackgroundPages.Ajax;
using SiteServer.BackgroundPages.Core;
using SiteServer.CMS.Core;
using SiteServer.CMS.Core.Create;
using SiteServer.CMS.Core.Office;
using SiteServer.CMS.Core.User;
using SiteServer.CMS.Model;

namespace SiteServer.BackgroundPages.Cms
{
    public class ModalUploadWord : BasePageCms
    {
        public CheckBox cbIsFirstLineTitle;
        public CheckBox cbIsFirstLineRemove;
        public CheckBox cbIsClearFormat;
        public CheckBox cbIsFirstLineIndent;
        public CheckBox cbIsClearFontSize;
        public CheckBox cbIsClearFontFamily;
        public CheckBox cbIsClearImages;
        public RadioButtonList rblContentLevel;

        private NodeInfo _nodeInfo;
        private string _returnUrl;

        public static string GetOpenWindowString(int publishmentSystemId, int nodeId, string returnUrl)
        {
            return PageUtils.GetOpenWindowString("批量导入Word文件",
                PageUtils.GetCmsUrl(nameof(ModalUploadWord), new NameValueCollection
                {
                    {"publishmentSystemID", publishmentSystemId.ToString()},
                    {"nodeID", nodeId.ToString()},
                    {"returnUrl", returnUrl}
                }), 550, 400);
        }

        public string GetUploadWordMultipleUrl()
        {
            return AjaxUploadService.GetUploadWordMultipleUrl(PublishmentSystemId);
        }

        public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            PageUtils.CheckRequestParameter("PublishmentSystemID", "ReturnUrl");
            var nodeId = int.Parse(Body.GetQueryString("NodeID"));
            _nodeInfo = NodeManager.GetNodeInfo(PublishmentSystemId, nodeId);
            _returnUrl = Body.GetQueryString("ReturnUrl");

            if (!IsPostBack)
            {
                int checkedLevel;
                var isChecked = CheckManager.GetUserCheckLevel(Body.AdministratorName, PublishmentSystemInfo, PublishmentSystemId, out checkedLevel);
                LevelManager.LoadContentLevelToEdit(rblContentLevel, PublishmentSystemInfo, _nodeInfo.NodeId, null, isChecked, checkedLevel);
                ControlUtils.SelectListItems(rblContentLevel, LevelManager.LevelInt.CaoGao.ToString());
            }
        }

        public override void Submit_OnClick(object sender, EventArgs e)
        {
            if (Page.IsPostBack && Page.IsValid)
            {
                var fileCount = TranslateUtils.ToInt(Request.Form["File_Count"]);
                if (fileCount == 1)
                {
                    var fileName = Request.Form["fileName_1"];
                    var redirectUrl = WebUtils.GetContentAddUploadWordUrl(PublishmentSystemId, _nodeInfo, cbIsFirstLineTitle.Checked, cbIsFirstLineRemove.Checked, cbIsClearFormat.Checked, cbIsFirstLineIndent.Checked, cbIsClearFontSize.Checked, cbIsClearFontFamily.Checked, cbIsClearImages.Checked, TranslateUtils.ToIntWithNagetive(rblContentLevel.SelectedValue), fileName, _returnUrl);
                    PageUtils.CloseModalPageAndRedirect(Page, redirectUrl);

                    return;
                }
                if (fileCount > 1)
                {
                    var tableStyle = NodeManager.GetTableStyle(PublishmentSystemInfo, _nodeInfo);
                    var tableName = NodeManager.GetTableName(PublishmentSystemInfo, _nodeInfo);
                    var relatedIdentities = RelatedIdentities.GetChannelRelatedIdentities(PublishmentSystemId, _nodeInfo.NodeId);

                    for (var index = 1; index <= fileCount; index++)
                    {
                        var fileName = Request.Form["fileName_" + index];
                        if (!string.IsNullOrEmpty(fileName))
                        {
                            var formCollection = WordUtils.GetWordNameValueCollection(PublishmentSystemId, _nodeInfo.ContentModelId, cbIsFirstLineTitle.Checked, cbIsFirstLineRemove.Checked, cbIsClearFormat.Checked, cbIsFirstLineIndent.Checked, cbIsClearFontSize.Checked, cbIsClearFontFamily.Checked, cbIsClearImages.Checked, TranslateUtils.ToInt(rblContentLevel.SelectedValue), fileName);

                            if (!string.IsNullOrEmpty(formCollection[ContentAttribute.Title]))
                            {
                                var contentInfo = ContentUtility.GetContentInfo(tableStyle);

                                BackgroundInputTypeParser.AddValuesToAttributes(tableStyle, tableName, PublishmentSystemInfo, relatedIdentities, formCollection, contentInfo.Attributes, ContentAttribute.HiddenAttributes);

                                contentInfo.NodeId = _nodeInfo.NodeId;
                                contentInfo.PublishmentSystemId = PublishmentSystemId;
                                contentInfo.AddUserName = Body.AdministratorName;
                                contentInfo.AddDate = DateTime.Now;
                                contentInfo.LastEditUserName = contentInfo.AddUserName;
                                contentInfo.LastEditDate = contentInfo.AddDate;

                                contentInfo.CheckedLevel = TranslateUtils.ToIntWithNagetive(rblContentLevel.SelectedValue);
                                contentInfo.IsChecked = contentInfo.CheckedLevel >= PublishmentSystemInfo.CheckContentLevel;

                                contentInfo.Id = DataProvider.ContentDao.Insert(tableName, PublishmentSystemInfo, contentInfo);

                                if (contentInfo.IsChecked)
                                {
                                    CreateManager.CreateContentAndTrigger(PublishmentSystemId, _nodeInfo.NodeId, contentInfo.Id);
                                }
                            }
                        }
                    }
                }

                PageUtils.CloseModalPage(Page);
            }
        }
    }
}
