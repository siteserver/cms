using System;
using System.Collections.Specialized;
using System.Web.UI.WebControls;
using BaiRong.Core;
using BaiRong.Core.Model.Enumerations;
using SiteServer.CMS.Core;
using SiteServer.CMS.Model;
using SiteServer.CMS.Model.Enumerations;

namespace SiteServer.BackgroundPages.Cms
{
    public class ModalAdMaterialAdd : BasePageCms
    {
        public TextBox AdMaterialName;
        public DropDownList AdMaterialType;
        public RadioButtonList IsEnabled;

        public PlaceHolder phCode;
        public TextBox Code;

        public PlaceHolder phText;
        public TextBox TextWord;
        public TextBox TextLink;
        public TextBox TextColor;
        public TextBox TextFontSize;

        public PlaceHolder phImage;
        public TextBox ImageUrl;
        public Button ImageUrlSelect;
        public Button ImageUrlUpload;
        public TextBox ImageLink;
        public TextBox ImageWidth;
        public TextBox ImageHeight;
        public TextBox ImageAlt;

        public PlaceHolder phFlash;
        public TextBox FlashUrl;
        public Button FlashUrlSelect;
        public Button FlashUrlUpload;
        public TextBox FlashWidth;
        public TextBox FlashHeight;
        public DropDownList Weight;
        public PlaceHolder phWeight;

        private bool _isEdit;
        private int _advId;
        private int _theAdMaterialId;

        public static string GetOpenWindowStringToAdd(int adMaterialId, int publishmentSystemId, int advId)
        {
            return PageUtils.GetOpenWindowString("添加广告物料",
                PageUtils.GetCmsUrl(nameof(ModalAdMaterialAdd), new NameValueCollection
                {
                    {"adMaterialID", adMaterialId.ToString()},
                    {"PublishmentSystemID", publishmentSystemId.ToString()},
                    {"advID", advId.ToString()}
                }), 900, 520);
        }

        public static string GetOpenWindowStringToEdit(int adMaterialId, int publishmentSystemId, int advId)
        {
            return PageUtils.GetOpenWindowString("修改广告物料",
                PageUtils.GetCmsUrl(nameof(ModalAdMaterialAdd), new NameValueCollection
                {
                    {"adMaterialID", adMaterialId.ToString()},
                    {"PublishmentSystemID", publishmentSystemId.ToString()},
                    {"advID", advId.ToString()}
                }), 900, 520);
        }

        public void Page_Load(object sender, EventArgs e)
        {
            _advId = Body.GetQueryInt("AdvID");

            if (Body.IsQueryExists("AdMaterialID"))
            {
                _theAdMaterialId = Body.GetQueryInt("AdMaterialID");
                if (_theAdMaterialId > 0)
                {
                    _isEdit = true;
                }
            }

            if (!Page.IsPostBack)
            {

                EAdvTypeUtils.AddListItems(AdMaterialType);
                ControlUtils.SelectListItems(AdMaterialType, EAdvTypeUtils.GetValue(EAdvType.HtmlCode));

                EBooleanUtils.AddListItems(IsEnabled);
                ControlUtils.SelectListItems(IsEnabled, true.ToString());

                EAdvWeightUtils.AddListItems(Weight);
                ControlUtils.SelectListItems(Weight, EAdvWeightUtils.GetValue(EAdvWeight.Level1));

                var advInfo = DataProvider.AdvDao.GetAdvInfo(_advId, PublishmentSystemId);
                if (advInfo != null)
                {
                    if (advInfo.RotateType == EAdvRotateType.HandWeight)
                    {
                        phWeight.Visible = true;
                    }
                    else
                    {
                        phWeight.Visible = false;
                    }
                }
                ImageUrl.Attributes.Add("onchange", GetShowImageScript("preview_ImageUrl", PublishmentSystemInfo.PublishmentSystemUrl));

                var showPopWinString = ModalSelectImage.GetOpenWindowString(PublishmentSystemInfo, ImageUrl.ClientID);
                ImageUrlSelect.Attributes.Add("onclick", showPopWinString);

                //false -- 不添加水印
                showPopWinString = ModalUploadImageSingle.GetOpenWindowStringToTextBox(PublishmentSystemId, ImageUrl.ClientID, false);
                ImageUrlUpload.Attributes.Add("onclick", showPopWinString);

                FlashUrl.Attributes.Add("onchange", GetShowImageScript("preview_FlashUrl", PublishmentSystemInfo.PublishmentSystemUrl));

                showPopWinString = ModalSelectImage.GetOpenWindowString(PublishmentSystemInfo, FlashUrl.ClientID);
                FlashUrlSelect.Attributes.Add("onclick", showPopWinString);

                //false -- 不添加水印
                showPopWinString = ModalUploadImageSingle.GetOpenWindowStringToTextBox(PublishmentSystemId, FlashUrl.ClientID, false);
                FlashUrlUpload.Attributes.Add("onclick", showPopWinString);

                if (_isEdit)
                {
                    var adMaterialInfo = DataProvider.AdMaterialDao.GetAdMaterialInfo(_theAdMaterialId, PublishmentSystemId);
                    AdMaterialName.Text = adMaterialInfo.AdMaterialName;
                    AdMaterialType.SelectedValue = EAdvTypeUtils.GetValue(adMaterialInfo.AdMaterialType);
                    IsEnabled.SelectedValue = adMaterialInfo.IsEnabled.ToString();

                    Code.Text = adMaterialInfo.Code;
                    TextWord.Text = adMaterialInfo.TextWord;
                    TextLink.Text = adMaterialInfo.TextLink;
                    TextColor.Text = adMaterialInfo.TextColor;
                    TextFontSize.Text = adMaterialInfo.TextFontSize.ToString();
                    Weight.SelectedValue = adMaterialInfo.Weight.ToString();
                    if (adMaterialInfo.AdMaterialType == EAdvType.Image)
                    {
                        ImageUrl.Text = adMaterialInfo.ImageUrl;
                        ImageLink.Text = adMaterialInfo.ImageLink;
                        ImageWidth.Text = adMaterialInfo.ImageWidth.ToString();
                        ImageHeight.Text = adMaterialInfo.ImageHeight.ToString();
                        ImageAlt.Text = adMaterialInfo.ImageAlt;
                    }
                    else if (adMaterialInfo.AdMaterialType == EAdvType.Flash)
                    {
                        FlashUrl.Text = adMaterialInfo.ImageUrl;
                        FlashWidth.Text = adMaterialInfo.ImageWidth.ToString();
                        FlashHeight.Text = adMaterialInfo.ImageHeight.ToString();
                    }
                }

                ReFresh(null, EventArgs.Empty);
            }

            SuccessMessage(string.Empty);
        }

        public string GetPreviewImageSrc(string adType)
        {
            var type = EAdvTypeUtils.GetEnumType(adType);
            var imageUrl = ImageUrl.Text;
            if (type == EAdvType.Flash)
            {
                imageUrl = FlashUrl.Text;
            }
            if (!string.IsNullOrEmpty(imageUrl))
            {
                var extension = PathUtils.GetExtension(imageUrl);
                if (EFileSystemTypeUtils.IsImage(extension))
                {
                    return PageUtility.ParseNavigationUrl(PublishmentSystemInfo, imageUrl);
                }
                else if (EFileSystemTypeUtils.IsFlash(extension))
                {
                    return SiteServerAssets.GetIconUrl("flash.jpg");
                }
                else if (EFileSystemTypeUtils.IsPlayer(extension))
                {
                    return SiteServerAssets.GetIconUrl("player.gif");
                }
            }
            return SiteServerAssets.GetIconUrl("empty.gif");
        }

        public void ReFresh(object sender, EventArgs e)
        {

            phCode.Visible = phText.Visible = phImage.Visible = phFlash.Visible = false;

            var adType = EAdvTypeUtils.GetEnumType(AdMaterialType.SelectedValue);
            if (adType == EAdvType.HtmlCode)
            {
                phCode.Visible = true;
            }
            else if (adType == EAdvType.JsCode)
            {
                phCode.Visible = true;
            }
            else if (adType == EAdvType.Text)
            {
                phText.Visible = true;
            }
            else if (adType == EAdvType.Image)
            {
                phImage.Visible = true;
            }
            else if (adType == EAdvType.Flash)
            {
                phFlash.Visible = true;
            }
        }

        public override void Submit_OnClick(object sender, EventArgs e)
        {
            if (Page.IsPostBack && Page.IsValid)
            {
                if (_isEdit == false)
                {
                    if (DataProvider.AdMaterialDao.IsExists(AdMaterialName.Text, PublishmentSystemId))
                    {
                        FailMessage($"名称为“{AdMaterialName.Text}”的广告物料已存在，请更改广告物料名称！");
                        return;
                    }
                }

                try
                {
                    if (_isEdit)
                    {
                        var adMaterialInfo = DataProvider.AdMaterialDao.GetAdMaterialInfo(_theAdMaterialId, PublishmentSystemId);
                        adMaterialInfo.AdMaterialName = AdMaterialName.Text;
                        adMaterialInfo.AdMaterialType = EAdvTypeUtils.GetEnumType(AdMaterialType.SelectedValue);
                        adMaterialInfo.IsEnabled = TranslateUtils.ToBool(IsEnabled.SelectedValue);

                        adMaterialInfo.Code = Code.Text;
                        adMaterialInfo.TextWord = TextWord.Text;
                        adMaterialInfo.TextLink = TextLink.Text;
                        adMaterialInfo.TextColor = TextColor.Text;
                        adMaterialInfo.TextFontSize = TranslateUtils.ToInt(TextFontSize.Text);
                        adMaterialInfo.Weight = TranslateUtils.ToInt(Weight.SelectedValue);
                        if (adMaterialInfo.AdMaterialType == EAdvType.Image)
                        {
                            adMaterialInfo.ImageUrl = ImageUrl.Text;
                            adMaterialInfo.ImageLink = ImageLink.Text;
                            adMaterialInfo.ImageWidth = TranslateUtils.ToInt(ImageWidth.Text);
                            adMaterialInfo.ImageHeight = TranslateUtils.ToInt(ImageHeight.Text);
                            adMaterialInfo.ImageAlt = ImageAlt.Text;
                        }
                        else if (adMaterialInfo.AdMaterialType == EAdvType.Flash)
                        {
                            adMaterialInfo.ImageUrl = FlashUrl.Text;
                            adMaterialInfo.ImageWidth = TranslateUtils.ToInt(FlashWidth.Text);
                            adMaterialInfo.ImageHeight = TranslateUtils.ToInt(FlashHeight.Text);
                        }

                        DataProvider.AdMaterialDao.Update(adMaterialInfo);
                        Body.AddSiteLog(PublishmentSystemId, "修改广告物料", $"广告物料名称：{adMaterialInfo.AdMaterialName}");

                        SuccessMessage("修改广告物料成功！");
                    }
                    else
                    {
                        var adMaterialInfo = new AdMaterialInfo(0, PublishmentSystemId, _advId, AdMaterialName.Text, EAdvTypeUtils.GetEnumType(AdMaterialType.SelectedValue), Code.Text, TextWord.Text, TextLink.Text, TextColor.Text, TranslateUtils.ToInt(TextFontSize.Text), ImageUrl.Text, ImageLink.Text, TranslateUtils.ToInt(ImageWidth.Text), TranslateUtils.ToInt(ImageHeight.Text), ImageAlt.Text, TranslateUtils.ToInt(Weight.SelectedValue), TranslateUtils.ToBool(IsEnabled.SelectedValue));
                        if (adMaterialInfo.AdMaterialType == EAdvType.Flash)
                        {
                            adMaterialInfo.ImageUrl = FlashUrl.Text;
                            adMaterialInfo.ImageWidth = TranslateUtils.ToInt(FlashWidth.Text);
                            adMaterialInfo.ImageHeight = TranslateUtils.ToInt(FlashHeight.Text);
                        }

                        DataProvider.AdMaterialDao.Insert(adMaterialInfo);

                        Body.AddSiteLog(PublishmentSystemId, "新增广告物料", $"广告物料名称：{adMaterialInfo.AdMaterialName}");

                        SuccessMessage("新增广告物料成功！");
                    }
                    PageUtils.CloseModalPageAndRedirect(Page, PageAdMaterial.GetRedirectUrl(PublishmentSystemId, _advId));

                }
                catch (Exception ex)
                {
                    FailMessage(ex, $"操作失败：{ex.Message}");
                }
            }
        }

    }
}
