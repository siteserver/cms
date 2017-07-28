using System;
using System.Collections.Specialized;
using System.Web.UI.WebControls;
using BaiRong.Core;
using SiteServer.CMS.Core;
using SiteServer.CMS.Model;

namespace SiteServer.BackgroundPages.Cms
{
    public class ModalInputAdd : BasePageCms
    {
        protected TextBox InputName;
        protected RadioButtonList IsChecked;
        protected RadioButtonList IsReply;

        protected TextBox MessageSuccess;
        protected TextBox MessageFailure;

        protected RadioButtonList IsAnomynous;
        protected RadioButtonList IsSuccessHide;
        protected RadioButtonList IsSuccessReload;
        protected RadioButtonList IsCtrlEnter;

        private bool _isPreview;

        public static string GetRedirectUrl(int publishmentSystemId, bool refreshLeft)
        {
            return PageUtils.GetCmsUrl(nameof(ModalInputAdd), new NameValueCollection
            {
                {"PublishmentSystemID", publishmentSystemId.ToString()},
                {"RefreshLeft", refreshLeft.ToString()}
            });
        }

        public static string GetOpenWindowStringToAdd(int publishmentSystemId)
        {
            return PageUtils.GetOpenWindowString("添加提交表单", PageUtils.GetCmsUrl(nameof(ModalInputAdd), new NameValueCollection
            {
                {"PublishmentSystemID", publishmentSystemId.ToString()}
            }), 560, 510);
        }

        public static string GetOpenWindowStringToEdit(int publishmentSystemId, int inputId, bool isPreview)
        {
            return PageUtils.GetOpenWindowString("修改提交表单", PageUtils.GetCmsUrl(nameof(ModalInputAdd), new NameValueCollection
            {
                {"PublishmentSystemID", publishmentSystemId.ToString()},
                {"InputID", inputId.ToString()},
                {"IsPreview", isPreview.ToString()}
            }), 560, 510);
        }

        public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            _isPreview = Body.GetQueryBool("IsPreview");

            if (!IsPostBack)
            {
                if (Body.IsQueryExists("InputID"))
                {
                    var inputId = Body.GetQueryInt("InputID");
                    var inputInfo = DataProvider.InputDao.GetInputInfo(inputId);
                    if (inputInfo != null)
                    {
                        InputName.Text = inputInfo.InputName;
                        ControlUtils.SelectListItems(IsChecked, inputInfo.IsChecked.ToString());
                        ControlUtils.SelectListItems(IsReply, inputInfo.IsReply.ToString());

                        MessageSuccess.Text = inputInfo.Additional.MessageSuccess;
                        MessageFailure.Text = inputInfo.Additional.MessageFailure;

                        ControlUtils.SelectListItems(IsAnomynous, inputInfo.Additional.IsAnomynous.ToString());
                        ControlUtils.SelectListItems(IsSuccessHide, inputInfo.Additional.IsSuccessHide.ToString());
                        ControlUtils.SelectListItems(IsSuccessReload, inputInfo.Additional.IsSuccessReload.ToString());
                        ControlUtils.SelectListItems(IsCtrlEnter, inputInfo.Additional.IsCtrlEnter.ToString());
                    }
                }

            }
        }

        public override void Submit_OnClick(object sender, EventArgs e)
        {
            var isChanged = false;
            InputInfo inputInfo;

            if (Body.IsQueryExists("InputID"))
            {
                try
                {
                    var inputId = Body.GetQueryInt("InputID");
                    inputInfo = DataProvider.InputDao.GetInputInfo(inputId);
                    if (inputInfo != null)
                    {
                        if (inputInfo.InputName != InputName.Text)
                        {
                            inputInfo.InputName = InputName.Text;
                        }
                        inputInfo.IsChecked = TranslateUtils.ToBool(IsChecked.SelectedValue);
                        inputInfo.IsReply = TranslateUtils.ToBool(IsReply.SelectedValue);

                        inputInfo.Additional.MessageSuccess = MessageSuccess.Text;
                        inputInfo.Additional.MessageFailure = MessageFailure.Text;

                        inputInfo.Additional.IsAnomynous = TranslateUtils.ToBool(IsAnomynous.SelectedValue);
                        inputInfo.Additional.IsSuccessHide = TranslateUtils.ToBool(IsSuccessHide.SelectedValue);
                        inputInfo.Additional.IsSuccessReload = TranslateUtils.ToBool(IsSuccessReload.SelectedValue);
                        inputInfo.Additional.IsCtrlEnter = TranslateUtils.ToBool(IsCtrlEnter.SelectedValue);
                    }
                    DataProvider.InputDao.Update(inputInfo);

                    Body.AddSiteLog(PublishmentSystemId, "修改提交表单", $"提交表单:{inputInfo.InputName}");

                    isChanged = true;
                }
                catch (Exception ex)
                {
                    FailMessage(ex, "提交表单修改失败！");
                }
            }
            else
            {
                var inputNameArrayList = DataProvider.InputDao.GetInputNameArrayList(PublishmentSystemId);
                if (inputNameArrayList.IndexOf(InputName.Text) != -1)
                {
                    FailMessage("提交表单添加失败，提交表单名称已存在！");
                }
                else
                {
                    try
                    {
                        inputInfo = new InputInfo
                        {
                            InputName = InputName.Text,
                            PublishmentSystemId = PublishmentSystemId,
                            IsChecked = TranslateUtils.ToBool(IsChecked.SelectedValue),
                            IsReply = TranslateUtils.ToBool(IsReply.SelectedValue)
                        };

                        inputInfo.Additional.MessageSuccess = MessageSuccess.Text;
                        inputInfo.Additional.MessageFailure = MessageFailure.Text;

                        inputInfo.Additional.IsAnomynous = TranslateUtils.ToBool(IsAnomynous.SelectedValue);
                        inputInfo.Additional.IsSuccessHide = TranslateUtils.ToBool(IsSuccessHide.SelectedValue);
                        inputInfo.Additional.IsSuccessReload = TranslateUtils.ToBool(IsSuccessReload.SelectedValue);
                        inputInfo.Additional.IsCtrlEnter = TranslateUtils.ToBool(IsCtrlEnter.SelectedValue);

                        DataProvider.InputDao.Insert(inputInfo);

                        Body.AddSiteLog(PublishmentSystemId, "添加提交表单", $"提交表单:{inputInfo.InputName}");

                        isChanged = true;
                    }
                    catch (Exception ex)
                    {
                        FailMessage(ex, "提交表单添加失败！");
                    }
                }
            }

            if (isChanged)
            {
                if (_isPreview)
                {
                    PageUtils.CloseModalPage(Page);
                }
                else
                {
                    PageUtils.CloseModalPageAndRedirect(Page, PageInput.GetRedirectUrl(PublishmentSystemId));
                }
            }
        }
    }
}
