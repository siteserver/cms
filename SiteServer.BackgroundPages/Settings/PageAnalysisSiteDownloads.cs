using System;
using System.Collections;
using System.Collections.Generic;
using System.Web.UI.WebControls;
using BaiRong.Core;
using BaiRong.Core.Model.Enumerations;
using SiteServer.CMS.Core;

namespace SiteServer.BackgroundPages.Settings
{
    public class PageAnalysisSiteDownloads : BasePageCms
    {
        public DropDownList DdlPublishmentSystemId;
        public Repeater RptContents;
        public Literal LtlTotalNum;

        public string StrArray { get; set; }

        private readonly Hashtable _horizentalHashtable = new Hashtable();
        private readonly Hashtable _verticalHashtable = new Hashtable();
        private readonly List<int> _publishmentSystemIdList = new List<int>();
        private readonly Hashtable _xHashtable = new Hashtable();
        private readonly Hashtable _yHashtableDownload = new Hashtable();
        private const string YTypeDownload = "YType_Download";

        public static string GetRedirectUrl()
        {
            return PageUtils.GetSettingsUrl(nameof(PageAnalysisSiteDownloads), null);
        }

        public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;
            if (IsPostBack) return;

            VerifyAdministratorPermissions(AppManager.Permissions.Settings.Chart);

            DdlPublishmentSystemId.Items.Add(new ListItem("<<全部站点>>", "0"));
            var publishmentSystemIdList = PublishmentSystemManager.GetPublishmentSystemIdListOrderByLevel();
            foreach (var publishmentSystemId in publishmentSystemIdList)
            {
                var publishmentSystemInfo = PublishmentSystemManager.GetPublishmentSystemInfo(publishmentSystemId);
                DdlPublishmentSystemId.Items.Add(new ListItem(publishmentSystemInfo.PublishmentSystemName, publishmentSystemId.ToString()));

                var key = publishmentSystemInfo.PublishmentSystemId;
                //x轴信息
                SetXHashtable(key, publishmentSystemInfo.PublishmentSystemName);
                //y轴信息
                SetYHashtable(key, CountManager.GetCount(publishmentSystemInfo.AuxiliaryTableForContent, publishmentSystemInfo.PublishmentSystemId, ECountType.Download));
            }

            RptContents.DataSource = publishmentSystemIdList;
            RptContents.ItemDataBound += RptContents_ItemDataBound;
            RptContents.DataBind();

            foreach (var key in _publishmentSystemIdList)
            {
                var yValueDownload = GetYHashtable(key);
                if (yValueDownload != "0")
                {
                    StrArray += $@"
xArrayDownload.push('{GetXHashtable(key)}');
                    yArrayDownload.push('{yValueDownload}');";
                }
            }
            LtlTotalNum.Text = GetVerticalTotalNum();
        }

        private void RptContents_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType != ListItemType.Item && e.Item.ItemType != ListItemType.AlternatingItem) return;

            var publishmentSystemId = (int) e.Item.DataItem;

            var publishmentSystemInfo = PublishmentSystemManager.GetPublishmentSystemInfo(publishmentSystemId);
            var ltlPublishmentSystemName = (Literal)e.Item.FindControl("ltlPublishmentSystemName");
            var ltlDownloadNum = (Literal)e.Item.FindControl("ltlDownloadNum");

            ltlPublishmentSystemName.Text = $@"<a href=""{PageAnalysisSiteDownloadsChannels.GetRedirectUrl(publishmentSystemId)}"">{publishmentSystemInfo.PublishmentSystemName}</a>";
            ltlDownloadNum.Text = GetYHashtable(publishmentSystemId);
        }

        public void Analysis_OnClick(object sender, EventArgs e)
        {
            var publishmentSystemId = TranslateUtils.ToInt(DdlPublishmentSystemId.SelectedValue);
            PageUtils.Redirect(publishmentSystemId > 0
                ? PageAnalysisSiteDownloadsChannels.GetRedirectUrl(publishmentSystemId)
                : GetRedirectUrl());
        }

        private void SetXHashtable(int publishmentSystemId, string publishmentSystemName)
        {
            if (!_xHashtable.ContainsKey(publishmentSystemId))
            {
                _xHashtable.Add(publishmentSystemId, publishmentSystemName);
            }
            if (!_publishmentSystemIdList.Contains(publishmentSystemId))
            {
                _publishmentSystemIdList.Add(publishmentSystemId);
            }
            _publishmentSystemIdList.Sort();
            _publishmentSystemIdList.Reverse();
        }

        private string GetXHashtable(int publishmentSystemId)
        {
            return _xHashtable.ContainsKey(publishmentSystemId) ? _xHashtable[publishmentSystemId].ToString() : string.Empty;
        }

        private void SetYHashtable(int publishemtSystemId, int value)
        {

            if (!_yHashtableDownload.ContainsKey(publishemtSystemId))
            {
                _yHashtableDownload.Add(publishemtSystemId, value);
            }
            else
            {
                var num = TranslateUtils.ToInt(_yHashtableDownload[publishemtSystemId].ToString());
                _yHashtableDownload[publishemtSystemId] = num + value;
            }
            SetVertical(YTypeDownload, value);

            SetHorizental(publishemtSystemId, value);
        }

        private string GetYHashtable(int publishemtSystemId)
        {
            if (!_yHashtableDownload.ContainsKey(publishemtSystemId)) return "0";
            var num = TranslateUtils.ToInt(_yHashtableDownload[publishemtSystemId].ToString());
            return num.ToString();
        }

        private void SetHorizental(int publishmentSystemId, int num)
        {
            if (_horizentalHashtable[publishmentSystemId] == null)
            {
                _horizentalHashtable[publishmentSystemId] = num;
            }
            else
            {
                var totalNum = (int)_horizentalHashtable[publishmentSystemId];
                _horizentalHashtable[publishmentSystemId] = totalNum + num;
            }
        }

        private void SetVertical(string type, int num)
        {
            if (_verticalHashtable[type] == null)
            {
                _verticalHashtable[type] = num;
            }
            else
            {
                var totalNum = (int)_verticalHashtable[type];
                _verticalHashtable[type] = totalNum + num;
            }
        }

        private string GetVerticalTotalNum()
        {
            var totalNum = 0;
            foreach (int num in _verticalHashtable.Values)
            {
                totalNum += num;
            }
            return totalNum == 0 ? "0" : $"<strong>{totalNum}</strong>";
        }
    }
}
