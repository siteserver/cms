using System;
using System.Collections.Specialized;
using System.Web.UI.WebControls;
using BaiRong.Core;
using BaiRong.Core.AuxiliaryTable;
using BaiRong.Core.Model.Attributes;
using BaiRong.Core.Model.Enumerations;
using SiteServer.CMS.Core;
using SiteServer.CMS.Model;

namespace SiteServer.BackgroundPages.Plugins
{
    public class ModalInputAdd : BasePageCms
    {
        public TextBox TbInputName;
        public RadioButtonList RblIsChecked;
        public RadioButtonList RblIsReply;
        public RadioButtonList RblIsAnomynous;

        public PlaceHolder PhAdministratorSmsNotify;
        public RadioButtonList RblIsAdministratorSmsNotify;
        public PlaceHolder PhIsAdministratorSmsNotify;
        public TextBox TbAdministratorSmsNotifyTplId;
        public CheckBoxList CblAdministratorSmsNotifyKeys;
        public TextBox TbAdministratorSmsNotifyMobile;

        private bool _isPreview;

        public static string GetRedirectUrl(int publishmentSystemId, bool refreshLeft)
        {
            return PageUtils.GetPluginsUrl(nameof(ModalInputAdd), new NameValueCollection
            {
                {"PublishmentSystemID", publishmentSystemId.ToString()},
                {"RefreshLeft", refreshLeft.ToString()}
            });
        }

        public static string GetOpenWindowStringToAdd(int publishmentSystemId)
        {
            return PageUtils.GetOpenWindowString("添加提交表单", PageUtils.GetPluginsUrl(nameof(ModalInputAdd), new NameValueCollection
            {
                {"PublishmentSystemID", publishmentSystemId.ToString()}
            }), 600, 520);
        }

        public static string GetOpenWindowStringToEdit(int publishmentSystemId, int inputId, bool isPreview)
        {
            return PageUtils.GetOpenWindowString("修改提交表单", PageUtils.GetPluginsUrl(nameof(ModalInputAdd), new NameValueCollection
            {
                {"PublishmentSystemID", publishmentSystemId.ToString()},
                {"InputID", inputId.ToString()},
                {"IsPreview", isPreview.ToString()}
            }), 600, 520);
        }

        public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            _isPreview = Body.GetQueryBool("IsPreview");

            if (IsPostBack) return;

            var inputId = Body.GetQueryInt("InputID");
            var inputInfo = DataProvider.InputDao.GetInputInfo(inputId);
            if (inputInfo != null)
            {
                PhAdministratorSmsNotify.Visible = true;

                TbInputName.Text = inputInfo.InputName;
                ControlUtils.SelectListItems(RblIsChecked, inputInfo.IsChecked.ToString());
                ControlUtils.SelectListItems(RblIsReply, inputInfo.IsReply.ToString());
                ControlUtils.SelectListItems(RblIsAnomynous, inputInfo.Additional.IsAnomynous.ToString());

                ControlUtils.SelectListItems(RblIsAdministratorSmsNotify,
                    inputInfo.Additional.IsAdministratorSmsNotify.ToString());
                TbAdministratorSmsNotifyTplId.Text = inputInfo.Additional.AdministratorSmsNotifyTplId;

                var keys = TranslateUtils.StringCollectionToStringList(inputInfo.Additional.AdministratorSmsNotifyKeys);
                CblAdministratorSmsNotifyKeys.Items.Add(new ListItem(InputContentAttribute.Id, InputContentAttribute.Id));
                CblAdministratorSmsNotifyKeys.Items.Add(new ListItem(InputContentAttribute.AddDate,
                    InputContentAttribute.AddDate));
                var relatedIdentities = RelatedIdentities.GetRelatedIdentities(ETableStyle.InputContent,
                    PublishmentSystemId, inputInfo.InputId);
                var styleInfoList = TableStyleManager.GetTableStyleInfoList(ETableStyle.InputContent,
                    DataProvider.InputContentDao.TableName, relatedIdentities);
                foreach (var styleInfo in styleInfoList)
                {
                    CblAdministratorSmsNotifyKeys.Items.Add(new ListItem(styleInfo.AttributeName,
                        styleInfo.AttributeName));
                }
                ControlUtils.SelectListItems(CblAdministratorSmsNotifyKeys, keys);

                TbAdministratorSmsNotifyMobile.Text = inputInfo.Additional.AdministratorSmsNotifyMobile;
            }
            else
            {
                PhAdministratorSmsNotify.Visible = false;
            }

            RblIsAdministratorSmsNotify_SelectedIndexChanged(null, EventArgs.Empty);
        }

        public void RblIsAdministratorSmsNotify_SelectedIndexChanged(object sender, EventArgs e)
        {
            PhIsAdministratorSmsNotify.Visible = TranslateUtils.ToBool(RblIsAdministratorSmsNotify.SelectedValue);
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
                        if (inputInfo.InputName != TbInputName.Text)
                        {
                            inputInfo.InputName = TbInputName.Text;
                        }
                        inputInfo.IsChecked = TranslateUtils.ToBool(RblIsChecked.SelectedValue);
                        inputInfo.IsReply = TranslateUtils.ToBool(RblIsReply.SelectedValue);
                        inputInfo.Additional.IsAnomynous = TranslateUtils.ToBool(RblIsAnomynous.SelectedValue);

                        inputInfo.Additional.IsAdministratorSmsNotify = TranslateUtils.ToBool(RblIsAdministratorSmsNotify.SelectedValue);
                        inputInfo.Additional.AdministratorSmsNotifyTplId = TbAdministratorSmsNotifyTplId.Text;

                        inputInfo.Additional.AdministratorSmsNotifyKeys =
                            ControlUtils.GetSelectedListControlValueCollection(CblAdministratorSmsNotifyKeys);

                        inputInfo.Additional.AdministratorSmsNotifyMobile = TbAdministratorSmsNotifyMobile.Text;

                        DataProvider.InputDao.Update(inputInfo);

                        Body.AddSiteLog(PublishmentSystemId, "修改提交表单", $"提交表单:{inputInfo.InputName}");
                    }

                    isChanged = true;
                }
                catch (Exception ex)
                {
                    FailMessage(ex, "提交表单修改失败！");
                }
            }
            else
            {
                var inputNameList = DataProvider.InputDao.GetInputNameList(PublishmentSystemId);
                if (inputNameList.IndexOf(TbInputName.Text) != -1)
                {
                    FailMessage("提交表单添加失败，提交表单名称已存在！");
                }
                else
                {
                    try
                    {
                        inputInfo = new InputInfo
                        {
                            InputName = TbInputName.Text,
                            PublishmentSystemId = PublishmentSystemId,
                            IsChecked = TranslateUtils.ToBool(RblIsChecked.SelectedValue),
                            IsReply = TranslateUtils.ToBool(RblIsReply.SelectedValue)
                        };

                        inputInfo.Additional.IsAnomynous = TranslateUtils.ToBool(RblIsAnomynous.SelectedValue);

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
