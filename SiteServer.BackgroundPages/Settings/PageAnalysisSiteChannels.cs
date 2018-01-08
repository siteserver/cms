using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Web.UI.WebControls;
using BaiRong.Core;
using BaiRong.Core.Model.Enumerations;
using SiteServer.BackgroundPages.Controls;
using SiteServer.BackgroundPages.Core;
using SiteServer.CMS.Core;
using SiteServer.CMS.Model.Enumerations;

namespace SiteServer.BackgroundPages.Settings
{
    public class PageAnalysisSiteChannels : BasePageCms
    {
        public DropDownList DdlPublishmentSystemId;
        public DateTimeTextBox TbStartDate;
        public DateTimeTextBox TbEndDate;
        public Repeater RptChannels;

        public string StrArray1 { get; set; }

        private readonly List<int> _publishmentSystemIdList = new List<int>();
        private readonly Hashtable _xHashtable = new Hashtable();
        private readonly Hashtable _yHashtableNew = new Hashtable();
        private readonly Hashtable _yHashtableUpdate = new Hashtable();
        private const string YTypeNew = "YType_New";
        private const string YTypeUpdate = "YType_Update";
        private const int YTypeOther = -1;
        private readonly Hashtable _horizentalHashtable = new Hashtable();
        private readonly Hashtable _verticalHashtable = new Hashtable();
        private NameValueCollection _additional;
        private DateTime _begin;
        private DateTime _end;

        protected override bool IsSinglePage => true;

        public static string GetRedirectUrl(int publishmentSystemId)
        {
            return PageUtils.GetSettingsUrl(nameof(PageAnalysisSiteChannels), new NameValueCollection
            {
                {"publishmentSystemId", publishmentSystemId.ToString()}
            });
        }

        public static string GetRedirectUrl(int publishmentSystemId, string startDate, string endDate)
        {
            return PageUtils.GetSettingsUrl(nameof(PageAnalysisSiteChannels), new NameValueCollection
            {
                {"publishmentSystemId", publishmentSystemId.ToString()},
                {"startDate", startDate},
                {"endDate", endDate}
            });
        }

        public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            if (string.IsNullOrEmpty(Body.GetQueryString("StartDate")))
            {
                _begin = DateTime.Now.AddMonths(-1);
                _end = DateTime.Now;
            }
            else
            {
                _begin = TranslateUtils.ToDateTime(Body.GetQueryString("StartDate"));
                _end = TranslateUtils.ToDateTime(Body.GetQueryString("EndDate"));
            }

            if (IsPostBack) return;

            VerifyAdministratorPermissions(AppManager.Permissions.Settings.Chart);

            DdlPublishmentSystemId.Items.Add(new ListItem("<<全部站点>>", "0"));
            var publishmentSystemIdList = PublishmentSystemManager.GetPublishmentSystemIdListOrderByLevel();
            foreach (var publishmentSystemId in publishmentSystemIdList)
            {
                var publishmentSystemInfo = PublishmentSystemManager.GetPublishmentSystemInfo(publishmentSystemId);
                DdlPublishmentSystemId.Items.Add(new ListItem(publishmentSystemInfo.PublishmentSystemName, publishmentSystemId.ToString()));
            }
            ControlUtils.SelectSingleItem(DdlPublishmentSystemId, PublishmentSystemId.ToString());

            TbStartDate.Text = DateUtils.GetDateAndTimeString(_begin);
            TbEndDate.Text = DateUtils.GetDateAndTimeString(_end);

            _additional = new NameValueCollection
            {
                ["StartDate"] = TbStartDate.Text,
                ["EndDate"] = TbEndDate.Text
            };

            ClientScriptRegisterClientScriptBlock("NodeTreeScript", ChannelLoading.GetScript(PublishmentSystemInfo, ELoadingType.SiteAnalysis, _additional));

            BindGrid();

            foreach (var key in _publishmentSystemIdList)
            {
                var yValueNew = GetYHashtable(key, YTypeNew);
                var yValueUpdate = GetYHashtable(key, YTypeUpdate);

                StrArray1 += $@"
xArrayNew.push('{GetXHashtable(key)}');
yArrayNew.push('{yValueNew}');
yArrayUpdate.push('{yValueUpdate}');";
            }
        }

        public void BindGrid()
        {
            var nodeIdList = DataProvider.NodeDao.GetNodeIdListByParentId(PublishmentSystemId, 0);
            foreach (var nodeId in nodeIdList)
            {
                var nodeInfo = NodeManager.GetNodeInfo(PublishmentSystemId, nodeId);
                var tableName = NodeManager.GetTableName(PublishmentSystemInfo, nodeId);

                SetXHashtable(nodeId, nodeInfo.NodeName);

                SetYHashtable(nodeId, DataProvider.ContentDao.GetCountOfContentAdd(tableName, PublishmentSystemId, nodeInfo.NodeId, EScopeType.All, TranslateUtils.ToDateTime(_additional["StartDate"]), TranslateUtils.ToDateTime(_additional["EndDate"]), string.Empty), YTypeNew);
                SetYHashtable(nodeId, DataProvider.ContentDao.GetCountOfContentUpdate(tableName, PublishmentSystemId, nodeInfo.NodeId, EScopeType.All, TranslateUtils.ToDateTime(_additional["StartDate"]), TranslateUtils.ToDateTime(_additional["EndDate"]), string.Empty), YTypeUpdate);
            }

            RptChannels.DataSource = DataProvider.NodeDao.GetNodeIdListByParentId(PublishmentSystemId, 0);
            RptChannels.ItemDataBound += RptChannels_ItemDataBound;
            RptChannels.DataBind();
        }

        private void RptChannels_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType != ListItemType.Item && e.Item.ItemType != ListItemType.AlternatingItem) return;

            var nodeId = (int)e.Item.DataItem;
            var enabled = IsOwningNodeId(nodeId);
            if (!enabled)
            {
                if (!IsHasChildOwningNodeId(nodeId)) e.Item.Visible = false;
            }
            var nodeInfo = NodeManager.GetNodeInfo(PublishmentSystemId, nodeId);

            var ltlRow = (Literal)e.Item.FindControl("ltlRow");

            ltlRow.Text = ChannelLoading.GetChannelRowHtml(PublishmentSystemInfo, nodeInfo, enabled, ELoadingType.SiteAnalysis, _additional, Body.AdminName);
        }

        public void Analysis_OnClick(object sender, EventArgs e)
        {
            var publishmentSystemId = TranslateUtils.ToInt(DdlPublishmentSystemId.SelectedValue);
            PageUtils.Redirect(publishmentSystemId > 0
                ? GetRedirectUrl(publishmentSystemId, TbStartDate.Text, TbEndDate.Text)
                : PageAnalysisSite.GetRedirectUrl(TbStartDate.Text, TbEndDate.Text));
        }

        private void SetXHashtable(int publishmentSystemId, string publishmentSystemName)
        {
            if (publishmentSystemId == YTypeOther)
            {
                if (!_xHashtable.ContainsKey(YTypeOther))
                {
                    _xHashtable.Add(YTypeOther, "其他");
                }
            }
            else if (!_xHashtable.ContainsKey(publishmentSystemId))
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

        private void SetYHashtable(int publishemtSystemId, int value, string yType)
        {
            switch (yType)
            {
                case YTypeNew:
                    if (!_yHashtableNew.ContainsKey(publishemtSystemId))
                    {
                        _yHashtableNew.Add(publishemtSystemId, value);
                    }
                    else
                    {
                        var num = TranslateUtils.ToInt(_yHashtableNew[publishemtSystemId].ToString());
                        _yHashtableNew[publishemtSystemId] = num + value;
                    }
                    SetVertical(YTypeNew, value);
                    break;
                case YTypeUpdate:
                    if (!_yHashtableUpdate.ContainsKey(publishemtSystemId))
                    {
                        _yHashtableUpdate.Add(publishemtSystemId, value);
                    }
                    else
                    {
                        var num = TranslateUtils.ToInt(_yHashtableUpdate[publishemtSystemId].ToString());
                        _yHashtableUpdate[publishemtSystemId] = num + value;
                    }
                    SetVertical(YTypeUpdate, value);
                    break;
            }
            SetHorizental(publishemtSystemId, value);
        }

        private string GetYHashtable(int publishemtSystemId, string yType)
        {
            switch (yType)
            {
                case YTypeNew:
                    if (_yHashtableNew.ContainsKey(publishemtSystemId))
                    {
                        var num = TranslateUtils.ToInt(_yHashtableNew[publishemtSystemId].ToString());
                        return num.ToString();
                    }
                    return "0";
                case YTypeUpdate:
                    if (_yHashtableUpdate.ContainsKey(publishemtSystemId))
                    {
                        var num = TranslateUtils.ToInt(_yHashtableUpdate[publishemtSystemId].ToString());
                        return num.ToString();
                    }
                    return "0";

                default:
                    return "0";
            }
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
    }
}
