using System;
using System.Collections.Specialized;
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
    public class PageAdvAdd : BasePageCms
    {
        public Literal ltlPageTitle;
        public TextBox AdvName;
        public RadioButtonList IsEnabled;
        public CheckBox IsDateLimited;
        public HtmlTableRow StartDateRow;
        public HtmlTableRow EndDateRow;
        public DateTimeTextBox StartDate;
        public DateTimeTextBox EndDate;
        public ListBox NodeIDCollectionToChannel;
        public ListBox NodeIDCollectionToContent;
        public HtmlTableRow FileTemplateIDCollectionRow;
        public CheckBoxList FileTemplateIDCollection;
        public DropDownList LevelType;
        public DropDownList Level;
        public CheckBox IsWeight;
        public DropDownList Weight ;

        public DropDownList RotateType;
        public HtmlTableRow RotateIntervalRow;
        public TextBox RotateInterval;
        public TextBox Summary;

        private int _advId;
        private int _adAreadId;
        private bool _isEdit;
        private bool[] _isLastNodeArray;

        public static string GetRedirectUrl(int publishmentSystemId, int adAreaId)
        {
            return PageUtils.GetCmsUrl(nameof(PageAdvAdd), new NameValueCollection
            {
                {"PublishmentSystemID", publishmentSystemId.ToString()},
                {"AdAreaID", adAreaId.ToString()}
            });
        }

        public void Page_Load(object sender, EventArgs e)
        {
            PageUtils.CheckRequestParameter("PublishmentSystemID");

            _adAreadId = Body.GetQueryInt("AdAreaID");
            if (Body.IsQueryExists("AdvID"))
            {
                _isEdit = true;
                _advId = Body.GetQueryInt("AdvID");
            }
             
            if (!Page.IsPostBack)
            {
                var pageTitle = _isEdit ? "编辑广告" : "添加广告";
                BreadCrumb(AppManager.Cms.LeftMenu.IdFunction, AppManager.Cms.LeftMenu.Function.IdAdvertisement, pageTitle, AppManager.Cms.Permission.WebSite.Advertisement);
                 
                ltlPageTitle.Text = pageTitle;

                StartDate.Text = DateUtils.GetDateAndTimeString(DateTime.Now);
                EndDate.Text = DateUtils.GetDateAndTimeString(DateTime.Now.AddMonths(1));
                 
                var nodeIdList = DataProvider.NodeDao.GetNodeIdListByPublishmentSystemId(PublishmentSystemId);
                var nodeCount = nodeIdList.Count;
                _isLastNodeArray = new bool[nodeCount];
                foreach (int theNodeID in nodeIdList)
                {
                    var nodeInfo = NodeManager.GetNodeInfo(PublishmentSystemId, theNodeID);

                    var title = WebUtils.GetChannelListBoxTitle(PublishmentSystemId, nodeInfo.NodeId, nodeInfo.NodeName, nodeInfo.NodeType, nodeInfo.ParentsCount, nodeInfo.IsLastNode, _isLastNodeArray);
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

                EBooleanUtils.AddListItems(IsEnabled);
                ControlUtils.SelectListItems(IsEnabled, true.ToString());

                EAdvLevelTypeUtils.AddListItems(LevelType);
                ControlUtils.SelectListItems(LevelType, EAdvLevelTypeUtils.GetValue(EAdvLevelType.Hold));

                EAdvLevelUtils.AddListItems(Level);
                ControlUtils.SelectListItems(Level, EAdvLevelUtils.GetValue(EAdvLevel.Level1));

                EAdvWeightUtils.AddListItems(Weight );
                ControlUtils.SelectListItems(Weight , EAdvWeightUtils.GetValue(EAdvWeight .Level1));

                EAdvRotateTypeUtils.AddListItems(RotateType);
                ControlUtils.SelectListItems(RotateType, EAdvRotateTypeUtils.GetValue(EAdvRotateType.HandWeight));

                if (_isEdit)
                {
                    var advInfo = DataProvider.AdvDao.GetAdvInfo(_advId, PublishmentSystemId);
                    AdvName.Text = advInfo.AdvName;
                    IsEnabled.SelectedValue = advInfo.IsEnabled.ToString();
                    IsDateLimited.Checked = advInfo.IsDateLimited;
                    StartDate.Text = DateUtils.GetDateAndTimeString(advInfo.StartDate);
                    EndDate.Text = DateUtils.GetDateAndTimeString(advInfo.EndDate);
                    ControlUtils.SelectListItems(NodeIDCollectionToChannel, TranslateUtils.StringCollectionToStringList(advInfo.NodeIDCollectionToChannel));
                    ControlUtils.SelectListItems(NodeIDCollectionToContent, TranslateUtils.StringCollectionToStringList(advInfo.NodeIDCollectionToContent));
                    ControlUtils.SelectListItems(FileTemplateIDCollection, TranslateUtils.StringCollectionToStringList(advInfo.FileTemplateIDCollection));
                    LevelType.SelectedValue = EAdvLevelTypeUtils.GetValue(advInfo.LevelType);
                    Level.SelectedValue = advInfo.Level.ToString();
                    IsWeight.Checked = advInfo.IsWeight;
                    Weight .SelectedValue = advInfo.Weight .ToString();
                    RotateType.SelectedValue = EAdvRotateTypeUtils.GetValue(advInfo.RotateType);
                    RotateInterval.Text = advInfo.RotateInterval.ToString();
                    Summary.Text = advInfo.Summary;
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

            Level.Visible = Weight .Visible = false;
            IsDateLimited.Enabled = true;

            var levelType = EAdvLevelTypeUtils.GetEnumType(LevelType.SelectedValue);
            if (levelType == EAdvLevelType.Standard)
            {
                Level.Visible = true;
            }
            else
            {
                Level.Visible = false;
            }

            if (IsWeight.Checked == true)
            {
                Weight .Visible = true;
            }
            else
            {
                Weight .Visible = false;
            }

            var rotateType = EAdvRotateTypeUtils.GetEnumType(RotateType.SelectedValue);
            if (rotateType == EAdvRotateType.SlideRotate)
            {
                RotateIntervalRow.Visible = true;
            }
            else
            {
                RotateIntervalRow.Visible = false;
            }
        }

        public override void Submit_OnClick(object sender, EventArgs e)
        {
            if (Page.IsPostBack && Page.IsValid)
            { 
                try
                {
                    if (_isEdit)
                    {
                        var advInfo = DataProvider.AdvDao.GetAdvInfo(_advId, PublishmentSystemId);
                        advInfo.AdvName = AdvName.Text;
                        advInfo.IsEnabled = TranslateUtils.ToBool(IsEnabled.SelectedValue);
                        advInfo.IsDateLimited = IsDateLimited.Checked;
                        advInfo.StartDate = TranslateUtils.ToDateTime(StartDate.Text);
                        advInfo.EndDate = TranslateUtils.ToDateTime(EndDate.Text);
                        advInfo.NodeIDCollectionToChannel = ControlUtils.GetSelectedListControlValueCollection(NodeIDCollectionToChannel);
                        advInfo.NodeIDCollectionToContent = ControlUtils.GetSelectedListControlValueCollection(NodeIDCollectionToContent);
                        advInfo.FileTemplateIDCollection = ControlUtils.GetSelectedListControlValueCollection(FileTemplateIDCollection);
                        advInfo.LevelType = EAdvLevelTypeUtils.GetEnumType(LevelType.SelectedValue);
                        advInfo.Level = TranslateUtils.ToInt(Level.SelectedValue);
                        advInfo.IsWeight = IsWeight.Checked;
                        advInfo.Weight  = TranslateUtils.ToInt(Weight .SelectedValue);
                        advInfo.RotateType = EAdvRotateTypeUtils.GetEnumType(RotateType.SelectedValue);
                        advInfo.RotateInterval = TranslateUtils.ToInt(RotateInterval.Text);
                        advInfo.Summary = Summary.Text;

                        DataProvider.AdvDao.Update(advInfo);

                        Body.AddSiteLog(PublishmentSystemId, "修改固定广告", $"广告名称：{advInfo.AdvName}");
                        SuccessMessage("修改广告成功！");
                    }
                    else
                    {
                        var advInfo = new AdvInfo();
                        advInfo.AdAreaID = _adAreadId;
                        advInfo.PublishmentSystemID = PublishmentSystemId;
                        advInfo.AdvName = AdvName.Text;
                        advInfo.Summary = Summary.Text;
                        advInfo.IsEnabled = TranslateUtils.ToBool(IsEnabled.SelectedValue);
                        advInfo.IsDateLimited = IsDateLimited.Checked;
                        advInfo.StartDate = TranslateUtils.ToDateTime(StartDate.Text);
                        advInfo.EndDate = TranslateUtils.ToDateTime(EndDate.Text);
                        advInfo.NodeIDCollectionToChannel = ControlUtils.GetSelectedListControlValueCollection(NodeIDCollectionToChannel);
                        advInfo.NodeIDCollectionToContent = ControlUtils.GetSelectedListControlValueCollection(NodeIDCollectionToContent);
                        advInfo.FileTemplateIDCollection = ControlUtils.GetSelectedListControlValueCollection(FileTemplateIDCollection);
                        advInfo.LevelType = EAdvLevelTypeUtils.GetEnumType(LevelType.SelectedValue);
                        advInfo.Level = TranslateUtils.ToInt(Level.SelectedValue);
                        advInfo.IsWeight = IsWeight.Checked;
                        advInfo.Weight  = TranslateUtils.ToInt(Weight .SelectedValue);
                        advInfo.RotateType = EAdvRotateTypeUtils.GetEnumType(RotateType.SelectedValue);
                        advInfo.RotateInterval = TranslateUtils.ToInt(RotateInterval.Text);

                        DataProvider.AdvDao.Insert(advInfo);
                        Body.AddSiteLog(PublishmentSystemId, "新增固定广告", $"广告名称：{advInfo.AdvName}");
                        SuccessMessage("新增广告成功！");
                    }

                    AddWaitAndRedirectScript(PageAdv.GetRedirectUrl(PublishmentSystemId, _adAreadId));
                }
                catch (Exception ex)
                {
                    FailMessage(ex, $"操作失败：{ex.Message}");
                }
            }
        }
    }
}
