using System;
using System.Collections.Specialized;
using System.Web.UI.WebControls;
using BaiRong.Core;
using BaiRong.Core.AuxiliaryTable;
using BaiRong.Core.Model.Enumerations;
using SiteServer.BackgroundPages.Controls;
using SiteServer.CMS.Core;

namespace SiteServer.BackgroundPages.Cms
{
	public class ModalContentExport : BasePageCms
    {
        public DropDownList DdlExportType;
        public DropDownList DdlPeriods;
        public DateTimeTextBox TbStartDate;
        public DateTimeTextBox TbEndDate;
        public PlaceHolder PhDisplayAttributes;
        public CheckBoxList CblDisplayAttributes;
        public DropDownList DdlIsChecked;

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
            var tableStyle = NodeManager.GetTableStyle(PublishmentSystemInfo, nodeInfo);
            var tableName = NodeManager.GetTableName(PublishmentSystemInfo, nodeInfo);
            var styleInfoList = TableStyleManager.GetTableStyleInfoList(tableStyle, tableName, relatedIdentities);
            styleInfoList = ContentUtility.GetAllTableStyleInfoList(PublishmentSystemInfo, tableStyle, styleInfoList);

            foreach (var styleInfo in styleInfoList)
            {
                var listItem = new ListItem(styleInfo.DisplayName, styleInfo.AttributeName)
                {
                    Selected = styleInfo.IsVisible
                };
                CblDisplayAttributes.Items.Add(listItem);
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
                    DdlExportType.SelectedValue = PublishmentSystemInfo.Additional.ConfigExportType;
                }
                if (!string.IsNullOrEmpty(PublishmentSystemInfo.Additional.ConfigExportPeriods))
                {
                    DdlPeriods.SelectedValue = PublishmentSystemInfo.Additional.ConfigExportPeriods;
                }
                if (!string.IsNullOrEmpty(PublishmentSystemInfo.Additional.ConfigExportDisplayAttributes))
                {
                    var displayAttributes = TranslateUtils.StringCollectionToStringList(PublishmentSystemInfo.Additional.ConfigExportDisplayAttributes);
                    ControlUtils.SelectListItems(CblDisplayAttributes, displayAttributes);
                }
                if (!string.IsNullOrEmpty(PublishmentSystemInfo.Additional.ConfigExportIsChecked))
                {
                    DdlIsChecked.SelectedValue = PublishmentSystemInfo.Additional.ConfigExportIsChecked;
                }
            }
            else
            {
                PublishmentSystemInfo.Additional.ConfigExportType = DdlExportType.SelectedValue;
                PublishmentSystemInfo.Additional.ConfigExportPeriods = DdlPeriods.SelectedValue;
                PublishmentSystemInfo.Additional.ConfigExportDisplayAttributes = ControlUtils.GetSelectedListControlValueCollection(CblDisplayAttributes);
                PublishmentSystemInfo.Additional.ConfigExportIsChecked = DdlIsChecked.SelectedValue;
                DataProvider.PublishmentSystemDao.Update(PublishmentSystemInfo);
            }
        }

        public void DdlExportType_SelectedIndexChanged(object sender, EventArgs e)
        {
            PhDisplayAttributes.Visible = DdlExportType.SelectedValue != "ContentZip";
        }

        public override void Submit_OnClick(object sender, EventArgs e)
        {
            var displayAttributes = ControlUtils.GetSelectedListControlValueCollection(CblDisplayAttributes);
            if (PhDisplayAttributes.Visible && string.IsNullOrEmpty(displayAttributes))
            {
                FailMessage("必须至少选择一项！");
                return;
            }

            ConfigSettings(false);

            var isPeriods = false;
            var startDate = string.Empty;
            var endDate = string.Empty;
            if (DdlPeriods.SelectedValue != "0")
            {
                isPeriods = true;
                if (DdlPeriods.SelectedValue == "-1")
                {
                    startDate = TbStartDate.Text;
                    endDate = TbEndDate.Text;
                }
                else
                {
                    var days = int.Parse(DdlPeriods.SelectedValue);
                    startDate = DateUtils.GetDateString(DateTime.Now.AddDays(-days));
                    endDate = DateUtils.GetDateString(DateTime.Now);
                }
            }
            var checkedState = ETriStateUtils.GetEnumType(DdlPeriods.SelectedValue);
            var redirectUrl = ModalExportMessage.GetRedirectUrlStringToExportContent(PublishmentSystemId, _nodeId, DdlExportType.SelectedValue, Body.GetQueryString("ContentIDCollection"), displayAttributes, isPeriods, startDate, endDate, checkedState);
            PageUtils.Redirect(redirectUrl);
		}
	}
}
