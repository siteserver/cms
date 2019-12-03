using System;
using System.Collections.Specialized;
using System.Web.UI.WebControls;
using SiteServer.Abstractions;
using SiteServer.BackgroundPages.Controls;
using SiteServer.CMS.Context;
using SiteServer.CMS.Context.Enumerations;
using SiteServer.CMS.Core;
using SiteServer.CMS.DataCache;
using SiteServer.CMS.Repositories;

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

        private int _channelId;

        public static string GetOpenWindowString(int siteId, int channelId)
        {
            return LayerUtils.GetOpenScriptWithCheckBoxValue("导出内容",
                PageUtils.GetCmsUrl(siteId, nameof(ModalContentExport), new NameValueCollection
                {
                    {"channelId", channelId.ToString()}
                }), "contentIdCollection", string.Empty);
        }

        private void LoadDisplayAttributeCheckBoxList()
        {
            var nodeInfo = ChannelManager.GetChannelAsync(SiteId, _channelId).GetAwaiter().GetResult();
            var styleList = ContentUtility.GetAllTableStyleList(TableStyleManager.GetContentStyleListAsync(Site, nodeInfo).GetAwaiter().GetResult());

            foreach (var style in styleList)
            {
                var listItem = new ListItem(style.DisplayName, style.AttributeName)
                {
                    Selected = true
                };
                CblDisplayAttributes.Items.Add(listItem);
            }
        }

		public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            _channelId = AuthRequest.GetQueryInt("channelId", SiteId);
            if (IsPostBack) return;

            LoadDisplayAttributeCheckBoxList();
            ConfigSettings(true);
        }

        private void ConfigSettings(bool isLoad)
        {
            if (isLoad)
            {
                if (!string.IsNullOrEmpty(Site.ConfigExportType))
                {
                    DdlExportType.SelectedValue = Site.ConfigExportType;
                }
                if (!string.IsNullOrEmpty(Site.ConfigExportPeriods))
                {
                    DdlPeriods.SelectedValue = Site.ConfigExportPeriods;
                }
                if (!string.IsNullOrEmpty(Site.ConfigExportDisplayAttributes))
                {
                    var displayAttributes = StringUtils.GetStringList(Site.ConfigExportDisplayAttributes);
                    ControlUtils.SelectMultiItems(CblDisplayAttributes, displayAttributes);
                }
                if (!string.IsNullOrEmpty(Site.ConfigExportIsChecked))
                {
                    DdlIsChecked.SelectedValue = Site.ConfigExportIsChecked;
                }
            }
            else
            {
                Site.ConfigExportType = DdlExportType.SelectedValue;
                Site.ConfigExportPeriods = DdlPeriods.SelectedValue;
                Site.ConfigExportDisplayAttributes = ControlUtils.GetSelectedListControlValueCollection(CblDisplayAttributes);
                Site.ConfigExportIsChecked = DdlIsChecked.SelectedValue;
                DataProvider.SiteRepository.UpdateAsync(Site).GetAwaiter().GetResult();
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
            var redirectUrl = ModalExportMessage.GetRedirectUrlStringToExportContent(SiteId, _channelId, DdlExportType.SelectedValue, AuthRequest.GetQueryString("contentIdCollection"), displayAttributes, isPeriods, startDate, endDate, checkedState);
            PageUtils.Redirect(redirectUrl);
		}
	}
}
