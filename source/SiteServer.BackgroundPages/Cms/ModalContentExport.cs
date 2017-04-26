using System;
using System.Collections.Specialized;
using System.Web.UI.WebControls;
using BaiRong.Core;
using BaiRong.Core.AuxiliaryTable;
using BaiRong.Core.Model;
using BaiRong.Core.Model.Enumerations;
using SiteServer.BackgroundPages.Controls;
using SiteServer.CMS.Core;

namespace SiteServer.BackgroundPages.Cms
{
	public class ModalContentExport : BasePageCms
    {
        public RadioButtonList rblExportType;
        public DropDownList ddlPeriods;
        public DateTimeTextBox tbStartDate;
        public DateTimeTextBox tbEndDate;
        public PlaceHolder phDisplayAttributes;
        public CheckBoxList cblDisplayAttributes;
        public DropDownList ddlIsChecked;

        private int _nodeId;

        public static string GetOpenWindowString(int publishmentSystemId, int nodeId)
        {
            return PageUtils.GetOpenLayerStringWithCheckBoxValue("导出内容",
                PageUtils.GetCmsUrl(nameof(ModalContentExport), new NameValueCollection
                {
                    {"PublishmentSystemID", publishmentSystemId.ToString()},
                    {"NodeID", nodeId.ToString()}
                }), "ContentIDCollection", string.Empty);
        }

        private void LoadDisplayAttributeCheckBoxList()
        {
            var nodeInfo = NodeManager.GetNodeInfo(PublishmentSystemId, _nodeId);
            var relatedIdentities = RelatedIdentities.GetChannelRelatedIdentities(PublishmentSystemId, _nodeId);
            var modelInfo = ContentModelManager.GetContentModelInfo(PublishmentSystemInfo, nodeInfo.ContentModelId);
            var tableStyle = EAuxiliaryTableTypeUtils.GetTableStyle(modelInfo.TableType);
            var styleInfoList = TableStyleManager.GetTableStyleInfoList(tableStyle, modelInfo.TableName, relatedIdentities);
            styleInfoList = ContentUtility.GetAllTableStyleInfoList(PublishmentSystemInfo, tableStyle, styleInfoList);

            foreach (var styleInfo in styleInfoList)
            {
                var listItem = new ListItem(styleInfo.DisplayName, styleInfo.AttributeName);
                listItem.Selected = styleInfo.IsVisible;
                cblDisplayAttributes.Items.Add(listItem);
            }
        }

		public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            _nodeId = Body.GetQueryInt("NodeID", PublishmentSystemId);
			if (!IsPostBack)
			{
                LoadDisplayAttributeCheckBoxList();
                ConfigSettings(true);
			}
		}

        private void ConfigSettings(bool isLoad)
        {
            if (isLoad)
            {
                if (!string.IsNullOrEmpty(PublishmentSystemInfo.Additional.ConfigExportType))
                {
                    rblExportType.SelectedValue = PublishmentSystemInfo.Additional.ConfigExportType;
                }
                if (!string.IsNullOrEmpty(PublishmentSystemInfo.Additional.ConfigExportPeriods))
                {
                    ddlPeriods.SelectedValue = PublishmentSystemInfo.Additional.ConfigExportPeriods;
                }
                if (!string.IsNullOrEmpty(PublishmentSystemInfo.Additional.ConfigExportDisplayAttributes))
                {
                    var displayAttributes = TranslateUtils.StringCollectionToStringList(PublishmentSystemInfo.Additional.ConfigExportDisplayAttributes);
                    ControlUtils.SelectListItems(cblDisplayAttributes, displayAttributes);
                }
                if (!string.IsNullOrEmpty(PublishmentSystemInfo.Additional.ConfigExportIsChecked))
                {
                    ddlIsChecked.SelectedValue = PublishmentSystemInfo.Additional.ConfigExportIsChecked;
                }
            }
            else
            {
                PublishmentSystemInfo.Additional.ConfigExportType = rblExportType.SelectedValue;
                PublishmentSystemInfo.Additional.ConfigExportPeriods = ddlPeriods.SelectedValue;
                PublishmentSystemInfo.Additional.ConfigExportDisplayAttributes = ControlUtils.GetSelectedListControlValueCollection(cblDisplayAttributes);
                PublishmentSystemInfo.Additional.ConfigExportIsChecked = ddlIsChecked.SelectedValue;
                DataProvider.PublishmentSystemDao.Update(PublishmentSystemInfo);
            }
        }

        public void rblExportType_SelectedIndexChanged(object sender, EventArgs e)
        {
            phDisplayAttributes.Visible = rblExportType.SelectedValue != "ContentZip";
        }

        public override void Submit_OnClick(object sender, EventArgs e)
        {
            var displayAttributes = ControlUtils.GetSelectedListControlValueCollection(cblDisplayAttributes);
            if (phDisplayAttributes.Visible && string.IsNullOrEmpty(displayAttributes))
            {
                FailMessage("必须至少选择一项！");
                return;
            }

            ConfigSettings(false);

            var isPeriods = false;
            var startDate = string.Empty;
            var endDate = string.Empty;
            if (ddlPeriods.SelectedValue != "0")
            {
                isPeriods = true;
                if (ddlPeriods.SelectedValue == "-1")
                {
                    startDate = tbStartDate.Text;
                    endDate = tbEndDate.Text;
                }
                else
                {
                    var days = int.Parse(ddlPeriods.SelectedValue);
                    startDate = DateUtils.GetDateString(DateTime.Now.AddDays(-days));
                    endDate = DateUtils.GetDateString(DateTime.Now);
                }
            }
            var checkedState = ETriStateUtils.GetEnumType(ddlPeriods.SelectedValue);
            var redirectUrl = ModalExportMessage.GetRedirectUrlStringToExportContent(PublishmentSystemId, _nodeId, rblExportType.SelectedValue, Body.GetQueryString("ContentIDCollection"), displayAttributes, isPeriods, startDate, endDate, checkedState);
            PageUtils.Redirect(redirectUrl);
		}
	}
}
