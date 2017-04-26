using System;
using System.Collections.Specialized;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using BaiRong.Core;
using BaiRong.Core.Model.Enumerations;
using SiteServer.BackgroundPages.Controls;
using SiteServer.BackgroundPages.Core;
using SiteServer.CMS.Core;
using SiteServer.CMS.Model;
using SiteServer.CMS.Model.Enumerations;

namespace SiteServer.BackgroundPages.Cms
{
	public class PageAdvertisementAdd : BasePageCms
    {
        public Literal ltlPageTitle;
		public PlaceHolder AdvertisementBase;
		public TextBox AdvertisementName;
		public RadioButtonList AdvertisementType;
		public CheckBox IsDateLimited;
		public HtmlTableRow StartDateRow;
		public HtmlTableRow EndDateRow;
        public DateTimeTextBox StartDate;
        public DateTimeTextBox EndDate;
        public ListBox NodeIDCollectionToChannel;
        public ListBox NodeIDCollectionToContent;
        public HtmlTableRow FileTemplateIDCollectionRow;
        public CheckBoxList FileTemplateIDCollection;

        public PlaceHolder AdvertisementFloatImage;
        public TextBox NavigationUrl;
		public TextBox ImageUrl;
		public TextBox Height;
		public TextBox Width;
		public Button SelectImage;
        public Button UploadImage;
		public CheckBox IsCloseable;
		public DropDownList PositionType;
		public TextBox PositionX;
        public TextBox PositionY;
		public RadioButtonList RollingType;

        public PlaceHolder AdvertisementScreenDown;
        public TextBox ScreenDownNavigationUrl;
        public TextBox ScreenDownImageUrl;
        public TextBox ScreenDownDelay;
        public TextBox ScreenDownHeight;
        public TextBox ScreenDownWidth;
        public Button ScreenDownSelectImage;
        public Button ScreenDownUploadImage;

        public PlaceHolder AdvertisementOpenWindow;
        public TextBox OpenWindowFileUrl;
        public TextBox OpenWindowWidth;
        public TextBox OpenWindowHeight;

        public PlaceHolder Done;

        public PlaceHolder OperatingError;
		public Label ErrorLabel;

		public Button Previous;
		public Button Next;

		private bool isEdit = false;
		private string editAdvertisementName = string.Empty;
		private EAdvertisementType editAdvertisementType = EAdvertisementType.FloatImage;
        private bool[] isLastNodeArray;
        private EPositionType ePositionType = EPositionType.LeftTop;

		public string PublishmentSystemUrl => PublishmentSystemInfo.PublishmentSystemUrl;

	    public string RootUrl => WebConfigUtils.ApplicationPath;

        public static string GetRedirectUrl(int publishmentSystemId)
        {
            return PageUtils.GetCmsUrl(nameof(PageAdvertisementAdd), new NameValueCollection
            {
                {"PublishmentSystemID", publishmentSystemId.ToString()}
            });
        }

        public string GetPreviewImageSrc(string adType)
        {
            var type = EAdvertisementTypeUtils.GetEnumType(adType);
            var imageUrl = ImageUrl.Text;
            if (type == EAdvertisementType.ScreenDown)
            {
                imageUrl = ScreenDownImageUrl.Text;
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

		public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            PageUtils.CheckRequestParameter("PublishmentSystemID");
			if (Body.IsQueryExists("AdvertisementName"))
			{
				isEdit = true;
				editAdvertisementName = Body.GetQueryString("AdvertisementName");
                if (DataProvider.AdvertisementDao.IsExists(editAdvertisementName, PublishmentSystemId))
				{
                    editAdvertisementType = DataProvider.AdvertisementDao.GetAdvertisementType(editAdvertisementName, PublishmentSystemId);
				}
				else
				{
					ErrorLabel.Text = $"不存在名称为“{editAdvertisementName}”的广告！";
					SetActivePanel(WizardPanel.OperatingError, OperatingError);
					return;
				}
			}

			if (!Page.IsPostBack)
            {
                var pageTitle = isEdit ? "编辑漂浮广告" : "添加漂浮广告";
                BreadCrumb(AppManager.Cms.LeftMenu.IdFunction, AppManager.Cms.LeftMenu.Function.IdAdvertisement, pageTitle, AppManager.Cms.Permission.WebSite.Advertisement);

                ltlPageTitle.Text = pageTitle;

                StartDate.Text = DateUtils.GetDateAndTimeString(DateTime.Now);
                EndDate.Text = DateUtils.GetDateAndTimeString(DateTime.Now.AddMonths(1));

				EAdvertisementTypeUtils.AddListItems(AdvertisementType);
				ControlUtils.SelectListItems(AdvertisementType, EAdvertisementTypeUtils.GetValue(EAdvertisementType.FloatImage));

                var nodeIdList = DataProvider.NodeDao.GetNodeIdListByPublishmentSystemId(PublishmentSystemId);
                var nodeCount = nodeIdList.Count;
                isLastNodeArray = new bool[nodeCount];
                foreach (int theNodeID in nodeIdList)
                {
                    var nodeInfo = NodeManager.GetNodeInfo(PublishmentSystemId, theNodeID);

                    var title = WebUtils.GetChannelListBoxTitle(PublishmentSystemId, nodeInfo.NodeId, nodeInfo.NodeName, nodeInfo.NodeType, nodeInfo.ParentsCount, nodeInfo.IsLastNode, isLastNodeArray);
                    var listitem = new ListItem(title, nodeInfo.NodeId.ToString());
                    NodeIDCollectionToChannel.Items.Add(listitem);
                    title = title + $"({nodeInfo.ContentNum})";
                    var listitem2 = new ListItem(title, nodeInfo.NodeId.ToString());
                    NodeIDCollectionToContent.Items.Add(listitem2);
                }

                var fileTemplateInfoArrayList = DataProvider.TemplateDao.GetTemplateInfoArrayListByType(PublishmentSystemId, ETemplateType.FileTemplate);
                if (fileTemplateInfoArrayList.Count > 0)
                {
                    foreach (TemplateInfo fileTemplateInfo in fileTemplateInfoArrayList)
                    {
                        var listitem = new ListItem(fileTemplateInfo.CreatedFileFullName, fileTemplateInfo.TemplateId.ToString());
                        FileTemplateIDCollection.Items.Add(listitem);
                    }
                }
                else
                {
                    FileTemplateIDCollectionRow.Visible = false;
                }

                EPositionTypeUtils.AddListItems(PositionType, ERollingType.Static);

				ERollingTypeUtils.AddListItems(RollingType);
				ControlUtils.SelectListItems(RollingType, ERollingTypeUtils.GetValue(ERollingType.FollowingScreen));

                var showPopWinString = ModalSelectImage.GetOpenWindowString(PublishmentSystemInfo, ImageUrl.ClientID);
				SelectImage.Attributes.Add("onclick", showPopWinString);

                showPopWinString = ModalUploadImageSingle.GetOpenWindowStringToTextBox(PublishmentSystemId, ImageUrl.ClientID);
                UploadImage.Attributes.Add("onclick", showPopWinString);

                showPopWinString = ModalSelectImage.GetOpenWindowString(PublishmentSystemInfo, ScreenDownImageUrl.ClientID);
                ScreenDownSelectImage.Attributes.Add("onclick", showPopWinString);

                showPopWinString = ModalUploadImageSingle.GetOpenWindowStringToTextBox(PublishmentSystemId, ScreenDownImageUrl.ClientID);
                ScreenDownUploadImage.Attributes.Add("onclick", showPopWinString);

				SetActivePanel(WizardPanel.AdvertisementBase, AdvertisementBase);

                if (isEdit)
                {
                    var adInfo = DataProvider.AdvertisementDao.GetAdvertisementInfo(editAdvertisementName, PublishmentSystemId);
                    AdvertisementName.Text = adInfo.AdvertisementName;
                    AdvertisementName.Enabled = false;
                    AdvertisementType.SelectedValue = EAdvertisementTypeUtils.GetValue(editAdvertisementType);

                    IsDateLimited.Checked = adInfo.IsDateLimited;
                    StartDate.Text = DateUtils.GetDateAndTimeString(adInfo.StartDate);
                    EndDate.Text = DateUtils.GetDateAndTimeString(adInfo.EndDate);
                    ControlUtils.SelectListItems(NodeIDCollectionToChannel, TranslateUtils.StringCollectionToStringList(adInfo.NodeIDCollectionToChannel));
                    ControlUtils.SelectListItems(NodeIDCollectionToContent, TranslateUtils.StringCollectionToStringList(adInfo.NodeIDCollectionToContent));
                    ControlUtils.SelectListItems(FileTemplateIDCollection, TranslateUtils.StringCollectionToStringList(adInfo.FileTemplateIDCollection));

                    if (adInfo.AdvertisementType == EAdvertisementType.FloatImage)
                    {
                        var adFloatImageInfo = new AdvertisementFloatImageInfo(adInfo.Settings);
                        IsCloseable.Checked = adFloatImageInfo.IsCloseable;
                        ePositionType = adFloatImageInfo.PositionType;
                        PositionX.Text = adFloatImageInfo.PositionX.ToString();
                        PositionY.Text = adFloatImageInfo.PositionY.ToString();
                        RollingType.SelectedValue = ERollingTypeUtils.GetValue(adFloatImageInfo.RollingType);

                        NavigationUrl.Text = adFloatImageInfo.NavigationUrl;
                        ImageUrl.Text = adFloatImageInfo.ImageUrl;
                        Height.Text = adFloatImageInfo.Height.ToString();
                        Width.Text = adFloatImageInfo.Width.ToString();
                    }
                    else if (adInfo.AdvertisementType == EAdvertisementType.ScreenDown)
                    {
                        var adScreenDownInfo = new AdvertisementScreenDownInfo(adInfo.Settings);
                        ScreenDownNavigationUrl.Text = adScreenDownInfo.NavigationUrl;
                        ScreenDownImageUrl.Text = adScreenDownInfo.ImageUrl;
                        ScreenDownDelay.Text = adScreenDownInfo.Delay.ToString();
                        ScreenDownWidth.Text = adScreenDownInfo.Width.ToString();
                        ScreenDownHeight.Text = adScreenDownInfo.Height.ToString();
                    }
                    else if (adInfo.AdvertisementType == EAdvertisementType.OpenWindow)
                    {
                        var adOpenWindowInfo = new AdvertisementOpenWindowInfo(adInfo.Settings);
                        OpenWindowFileUrl.Text = adOpenWindowInfo.FileUrl;
                        OpenWindowWidth.Text = adOpenWindowInfo.Width.ToString();
                        OpenWindowHeight.Text = adOpenWindowInfo.Height.ToString();
                    }
                }
                ReFresh(null, EventArgs.Empty);
			}

			SuccessMessage(string.Empty);
		}

		public void ReFresh(object sender, EventArgs e)
		{
			if (IsDateLimited.Checked)
			{
				StartDateRow.Visible = true;
				EndDateRow.Visible = true;
			}
			else
			{
				StartDateRow.Visible = false;
				EndDateRow.Visible = false;
			}

            PositionType.Items.Clear();
            var rollingType = ERollingTypeUtils.GetEnumType(RollingType.SelectedValue);
            EPositionTypeUtils.AddListItems(PositionType, rollingType);
            ControlUtils.SelectListItems(PositionType, EPositionTypeUtils.GetValue(ePositionType));
		}

		public WizardPanel CurrentWizardPanel
		{
			get
			{
				if (ViewState["WizardPanel"] != null)
					return (WizardPanel)ViewState["WizardPanel"];

				return WizardPanel.AdvertisementBase;
			}
			set
			{
				ViewState["WizardPanel"] = value;
			}
		}


		public enum WizardPanel
		{
			AdvertisementBase,
			AdvertisementFloatImage,
            AdvertisementScreenDown,
            AdvertisementOpenWindow,
			OperatingError,
			Done
		}

		void SetActivePanel(WizardPanel panel, Control controlToShow)
		{
            var currentPanel = FindControl(CurrentWizardPanel.ToString()) as PlaceHolder;
			if (currentPanel != null)
				currentPanel.Visible = false;

			switch (panel)
			{
				case WizardPanel.AdvertisementBase:
                    Previous.CssClass = "btn disabled";
					break;
				case WizardPanel.Done:
                    AddWaitAndRedirectScript(PageAdvertisement.GetRedirectUrl(PublishmentSystemId));
                    Next.CssClass = "btn btn-primary disabled";
                    Previous.CssClass = "btn disabled";
					break;
				case WizardPanel.OperatingError:
                    Next.CssClass = "btn btn-primary disabled";
                    Previous.CssClass = "btn disabled";
					break;
				default:
					Next.CssClass = "btn btn-primary";
                    Previous.CssClass = "btn";
					break;
			}

			controlToShow.Visible = true;
			CurrentWizardPanel = panel;
		}


		public bool Validate_AdvertisementBase(out string errorMessage)
		{
			if (isEdit)
			{
				errorMessage = string.Empty;
				return true;
			}
			else
			{
                if (DataProvider.AdvertisementDao.IsExists(AdvertisementName.Text, PublishmentSystemId))
				{
					errorMessage = $"名称为“{AdvertisementName.Text}”的广告已存在，请更改广告名称！";
					return false;
				}
				else
				{
					errorMessage = string.Empty;
					return true;
				}
			}
		}

		public bool Validate_InsertFloatImageAdvertisement(out string errorMessage)
		{
            var adInfo = new AdvertisementInfo(AdvertisementName.Text, PublishmentSystemId, EAdvertisementTypeUtils.GetEnumType(AdvertisementType.SelectedValue), IsDateLimited.Checked, TranslateUtils.ToDateTime(StartDate.Text), TranslateUtils.ToDateTime(EndDate.Text), DateTime.Now, ControlUtils.GetSelectedListControlValueCollection(NodeIDCollectionToChannel), ControlUtils.GetSelectedListControlValueCollection(NodeIDCollectionToContent), ControlUtils.GetSelectedListControlValueCollection(FileTemplateIDCollection), string.Empty);
            var adFloatImageInfo = new AdvertisementFloatImageInfo(IsCloseable.Checked, EPositionTypeUtils.GetEnumType(PositionType.SelectedValue), TranslateUtils.ToInt(PositionX.Text), TranslateUtils.ToInt(PositionY.Text), ERollingTypeUtils.GetEnumType(RollingType.SelectedValue), NavigationUrl.Text, ImageUrl.Text, TranslateUtils.ToInt(Height.Text), TranslateUtils.ToInt(Width.Text));
            adInfo.Settings = adFloatImageInfo.ToString();
			try
			{
				if (isEdit)
				{
                    DataProvider.AdvertisementDao.Update(adInfo);

                    Body.AddSiteLog(PublishmentSystemId, "修改漂浮广告", $"广告名称：{adInfo.AdvertisementName}");
				}
				else
				{
                    DataProvider.AdvertisementDao.Insert(adInfo);

                    Body.AddSiteLog(PublishmentSystemId, "新增漂浮广告", $"广告名称：{adInfo.AdvertisementName}");
				}
				errorMessage = string.Empty;
				return true;
			}
			catch
			{
				errorMessage = "操作失败！";
				return false;
			}
		}

        public bool Validate_InsertOpenWindowAdvertisement(out string errorMessage)
        {
            var adInfo = new AdvertisementInfo(AdvertisementName.Text, PublishmentSystemId, EAdvertisementTypeUtils.GetEnumType(AdvertisementType.SelectedValue), IsDateLimited.Checked, TranslateUtils.ToDateTime(StartDate.Text), TranslateUtils.ToDateTime(EndDate.Text), DateTime.Now, ControlUtils.GetSelectedListControlValueCollection(NodeIDCollectionToChannel), ControlUtils.GetSelectedListControlValueCollection(NodeIDCollectionToContent), ControlUtils.GetSelectedListControlValueCollection(FileTemplateIDCollection), string.Empty);
            var adOpenWindowInfo = new AdvertisementOpenWindowInfo(OpenWindowFileUrl.Text, TranslateUtils.ToInt(OpenWindowHeight.Text), TranslateUtils.ToInt(OpenWindowWidth.Text));
            adInfo.Settings = adOpenWindowInfo.ToString();
            try
            {
                if (isEdit)
                {
                    DataProvider.AdvertisementDao.Update(adInfo);
                }
                else
                {
                    DataProvider.AdvertisementDao.Insert(adInfo);
                }
                errorMessage = string.Empty;
                return true;
            }
            catch
            {
                errorMessage = "操作失败！";
                return false;
            }
        }

        public bool Validate_InsertScreenDownAdvertisement(out string errorMessage)
        {
            var adInfo = new AdvertisementInfo(AdvertisementName.Text, PublishmentSystemId, EAdvertisementTypeUtils.GetEnumType(AdvertisementType.SelectedValue), IsDateLimited.Checked, TranslateUtils.ToDateTime(StartDate.Text), TranslateUtils.ToDateTime(EndDate.Text), DateTime.Now, ControlUtils.GetSelectedListControlValueCollection(NodeIDCollectionToChannel), ControlUtils.GetSelectedListControlValueCollection(NodeIDCollectionToContent), ControlUtils.GetSelectedListControlValueCollection(FileTemplateIDCollection), string.Empty);
            var adScreenDownInfo = new AdvertisementScreenDownInfo(ScreenDownNavigationUrl.Text, ScreenDownImageUrl.Text, TranslateUtils.ToInt(ScreenDownDelay.Text), TranslateUtils.ToInt(ScreenDownHeight.Text), TranslateUtils.ToInt(ScreenDownWidth.Text));
            adInfo.Settings = adScreenDownInfo.ToString();
            try
            {
                if (isEdit)
                {
                    DataProvider.AdvertisementDao.Update(adInfo);
                }
                else
                {
                    DataProvider.AdvertisementDao.Insert(adInfo);
                }
                errorMessage = string.Empty;
                return true;
            }
            catch
            {
                errorMessage = "操作失败！";
                return false;
            }
        }

		public void NextPanel(object sender, EventArgs e)
		{
			var errorMessage = "";
			switch (CurrentWizardPanel)
			{
				case WizardPanel.AdvertisementBase:

					if (Validate_AdvertisementBase(out errorMessage))
					{
                        var adType = EAdvertisementTypeUtils.GetEnumType(AdvertisementType.SelectedValue);
                        if (adType == EAdvertisementType.FloatImage)
                        {
                            SetActivePanel(WizardPanel.AdvertisementFloatImage, AdvertisementFloatImage);
                        }
                        else if (adType == EAdvertisementType.ScreenDown)
                        {
                            SetActivePanel(WizardPanel.AdvertisementScreenDown, AdvertisementScreenDown);
                        }
                        else if (adType == EAdvertisementType.OpenWindow)
                        {
                            SetActivePanel(WizardPanel.AdvertisementOpenWindow, AdvertisementOpenWindow);
                        }						
					}
					else
					{
                        FailMessage(errorMessage);
						SetActivePanel(WizardPanel.AdvertisementBase, AdvertisementBase);
					}

					break;

                case WizardPanel.AdvertisementFloatImage:

                    if (Validate_InsertFloatImageAdvertisement(out errorMessage))
					{
						SetActivePanel(WizardPanel.Done, Done);
					}
					else
					{
						ErrorLabel.Text = errorMessage;
						SetActivePanel(WizardPanel.OperatingError, OperatingError);
					}

					break;

                case WizardPanel.AdvertisementScreenDown:

                    if (Validate_InsertScreenDownAdvertisement(out errorMessage))
                    {
                        SetActivePanel(WizardPanel.Done, Done);
                    }
                    else
                    {
                        ErrorLabel.Text = errorMessage;
                        SetActivePanel(WizardPanel.OperatingError, OperatingError);
                    }

                    break;

                case WizardPanel.AdvertisementOpenWindow:

                    if (Validate_InsertOpenWindowAdvertisement(out errorMessage))
                    {
                        SetActivePanel(WizardPanel.Done, Done);
                    }
                    else
                    {
                        ErrorLabel.Text = errorMessage;
                        SetActivePanel(WizardPanel.OperatingError, OperatingError);
                    }

                    break;

				case WizardPanel.Done:
					break;
			}
		}

		public void PreviousPanel(object sender, EventArgs e)
		{
			switch (CurrentWizardPanel)
			{
				case WizardPanel.AdvertisementBase:
					break;

				case WizardPanel.AdvertisementFloatImage:
                    SetActivePanel(WizardPanel.AdvertisementBase, AdvertisementBase);
					break;

                case WizardPanel.AdvertisementScreenDown:
                    SetActivePanel(WizardPanel.AdvertisementBase, AdvertisementBase);
                    break;

                case WizardPanel.AdvertisementOpenWindow:
                    SetActivePanel(WizardPanel.AdvertisementBase, AdvertisementBase);
                    break;
			}
		}
	}
}
