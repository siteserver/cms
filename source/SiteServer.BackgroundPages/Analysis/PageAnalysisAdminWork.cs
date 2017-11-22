using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Web.UI.WebControls;
using BaiRong.Core;
using BaiRong.Core.Data;
using SiteServer.BackgroundPages.Controls;
using SiteServer.BackgroundPages.Core;
using SiteServer.CMS.Core;
using SiteServer.CMS.Model.Enumerations;

namespace SiteServer.BackgroundPages.Analysis
{
    public class PageAnalysisAdminWork : BasePageCms
    {       
        public DateTimeTextBox TbStartDate;
        public DateTimeTextBox TbEndDate;
        public Literal LtlArray1;
        public Literal LtlArray2;
        public Repeater RptChannels;
        public Repeater RptContents;
        public SqlPager SpContents;

        private readonly List<int> _publishmentSystemIdList = new List<int>();
        private readonly List<string> _userNameList = new List<string>();
        private readonly Hashtable _xHashtable = new Hashtable();
        private readonly Hashtable _yHashtableNew = new Hashtable();
        private readonly Hashtable _yHashtableUpdate = new Hashtable();
        private readonly Hashtable _xHashtableUser = new Hashtable();
        private readonly Hashtable _yHashtableUserNew = new Hashtable();
        private readonly Hashtable _yHashtableUserUpdate = new Hashtable();
        private const string YTypeNew = "YType_New";
        private const string YTypeUpdate = "YType_Update";
        private const int YTypeOther = -1;
        private readonly Hashtable _horizentalHashtable = new Hashtable();
        private readonly Hashtable _verticalHashtable = new Hashtable();        
        private readonly Hashtable _horizentalHashtableUser = new Hashtable();
        private readonly Hashtable _verticalHashtableUser = new Hashtable();
        private NameValueCollection _additional;
        private DateTime _begin;
        private DateTime _end;

        public static string GetRedirectUrl(int publishmentSystemId, string returnUrl)
        {
            return PageUtils.GetAnalysisUrl(nameof(PageAnalysisAdminWork), new NameValueCollection
            {
                {"publishmentSystemID", publishmentSystemId.ToString()},
                {"returnUrl", StringUtils.ValueToUrl(returnUrl)}
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

            SpContents.ControlToPaginate = RptContents;
            RptContents.ItemDataBound += RptContents_ItemDataBound;

            SpContents.ItemsPerPage = StringUtils.Constants.PageSize;
            SpContents.SelectCommand = BaiRongDataProvider.ContentDao.GetSelectCommendOfAdminExcludeRecycle(PublishmentSystemInfo.AuxiliaryTableForContent, PublishmentSystemId, _begin, _end);
            SpContents.SortField = "UserName";
            SpContents.SortMode = SortMode.DESC;

            if (IsPostBack) return;

            if (PublishmentSystemId > 0)
            {
                BreadCrumb(AppManager.Cms.LeftMenu.IdFunction, AppManager.Cms.LeftMenu.Function.IdSiteAnalysis, "管理员工作量统计", AppManager.Cms.Permission.WebSite.SiteAnalysis);
            }
            else
            {
                BreadCrumbAnalysis(AppManager.Analysis.LeftMenu.Chart, "管理员工作量统计", AppManager.Analysis.Permission.AnalysisChart);
            }

            TbStartDate.Text = DateUtils.GetDateAndTimeString(_begin);
            TbEndDate.Text = DateUtils.GetDateAndTimeString(_end);

            _additional = new NameValueCollection
            {
                ["StartDate"] = TbStartDate.Text,
                ["EndDate"] = TbEndDate.Text
            };

            ClientScriptRegisterClientScriptBlock("NodeTreeScript", ChannelLoading.GetScript(PublishmentSystemInfo, ELoadingType.SiteAnalysis, _additional));

            BindGrid();
            SpContents.DataBind();

            foreach (var key in _publishmentSystemIdList)
            {
                var yValueNew = GetYHashtable(key, YTypeNew);
                var yValueUpdate = GetYHashtable(key, YTypeUpdate);

                LtlArray1.Text += $@"
xArrayNew.push('{GetXHashtable(key)}');
yArrayNew.push('{yValueNew}');
yArrayUpdate.push('{yValueUpdate}');";
            }

            foreach (var key in _userNameList)
            {
                var yValueNew = GetYHashtableUser(key, YTypeNew);
                var yValueUpdate = GetYHashtableUser(key, YTypeUpdate);
                LtlArray2.Text += $@"
xArrayNew.push('{GetXHashtableUser(key)}');
yArrayNew.push('{yValueNew}');
yArrayUpdate.push('{yValueUpdate}');";
            }
        }

        private void RptContents_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType != ListItemType.Item && e.Item.ItemType != ListItemType.AlternatingItem) return;

            var userName = SqlUtils.EvalString(e.Item.DataItem, "userName");
            var addCount = SqlUtils.EvalInt(e.Item.DataItem, "addCount");
            var updateCount = SqlUtils.EvalInt(e.Item.DataItem, "updateCount");

            var ltlUserName = (Literal)e.Item.FindControl("ltlUserName");
            var ltlDisplayName = (Literal)e.Item.FindControl("ltlDisplayName");
            var ltlContentAdd = (Literal)e.Item.FindControl("ltlContentAdd");
            var ltlContentUpdate = (Literal)e.Item.FindControl("ltlContentUpdate");

            ltlUserName.Text = userName;
            ltlDisplayName.Text = BaiRongDataProvider.AdministratorDao.GetDisplayName(userName);

            ltlContentAdd.Text = addCount == 0 ? "0" : $"<strong>{addCount}</strong>";
            ltlContentUpdate.Text = updateCount == 0 ? "0" : $"<strong>{updateCount}</strong>";
        }

        public void BindGrid()
        {
            var nodeIdList = DataProvider.NodeDao.GetNodeIdListByParentId(PublishmentSystemId, 0);
            foreach (var nodeId in nodeIdList)
            {
                var nodeInfo = NodeManager.GetNodeInfo(PublishmentSystemId, nodeId);
                var tableName = NodeManager.GetTableName(PublishmentSystemInfo, nodeId);

                SetXHashtable(nodeId, nodeInfo.NodeName);

                SetYHashtable(nodeId, DataProvider.ContentDao.GetCountOfContentAdd(tableName, PublishmentSystemId, nodeInfo.NodeId, TranslateUtils.ToDateTime(_additional["StartDate"]), TranslateUtils.ToDateTime(_additional["EndDate"]), string.Empty), YTypeNew);
                SetYHashtable(nodeId, DataProvider.ContentDao.GetCountOfContentUpdate(tableName, PublishmentSystemId, nodeInfo.NodeId, TranslateUtils.ToDateTime(_additional["StartDate"]), TranslateUtils.ToDateTime(_additional["EndDate"]), string.Empty), YTypeUpdate);
            }
            var ds = BaiRongDataProvider.ContentDao.GetDataSetOfAdminExcludeRecycle(PublishmentSystemInfo.AuxiliaryTableForContent, PublishmentSystemId, _begin, _end);
            if (ds == null || ds.Tables.Count <= 0) return;

            var dt = ds.Tables[0];
            if (dt.Rows.Count <= 0) return;

            foreach (DataRow dr in dt.Rows)
            {
                SetXHashtableUser(dr["userName"].ToString(), dr["userName"].ToString());
                SetYHashtableUser(dr["userName"].ToString(), TranslateUtils.ToInt(dr["addCount"].ToString()), YTypeNew);
                SetYHashtableUser(dr["userName"].ToString(), TranslateUtils.ToInt(dr["updateCount"].ToString()), YTypeUpdate);
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

            var element = (NoTagText)e.Item.FindControl("ElHtml");

            element.Text = ChannelLoading.GetChannelRowHtml(PublishmentSystemInfo, nodeInfo, enabled, ELoadingType.SiteAnalysis, _additional, Body.AdministratorName);
        }

        public void Analysis_OnClick(object sender, EventArgs e)
        {
            var pageUrl = PageUtils.GetAnalysisUrl(nameof(PageAnalysisAdminWork), new NameValueCollection
            {
                {"PublishmentSystemID", PublishmentSystemId.ToString()},
                {"StartDate", TbStartDate.Text},
                {"EndDate", TbEndDate.Text}
            });
            PageUtils.Redirect(pageUrl);
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

        private void SetXHashtableUser(string userName, string publishmentSystemName)
        {
            if (!_xHashtableUser.ContainsKey(userName))
            {
                _xHashtableUser.Add(userName, publishmentSystemName);
            }
            if (!_userNameList.Contains(userName))
            {
                _userNameList.Add(userName);
            }
            _userNameList.Sort();
            _userNameList.Reverse();
        }

        private string GetXHashtableUser(string userName)
        {
            return _xHashtableUser.ContainsKey(userName) ? _xHashtableUser[userName].ToString() : string.Empty;
        }

        private void SetYHashtableUser(string userName, int value, string yType)
        {
            switch (yType)
            {
                case YTypeNew:
                    if (!_yHashtableUserNew.ContainsKey(userName))
                    {
                        _yHashtableUserNew.Add(userName, value);
                    }
                    else
                    {
                        var num = TranslateUtils.ToInt(_yHashtableUserNew[userName].ToString());
                        _yHashtableUserNew[userName] = num + value;
                    }
                    SetVerticalUser(YTypeNew, value);
                    break;
                case YTypeUpdate:
                    if (!_yHashtableUserUpdate.ContainsKey(userName))
                    {
                        _yHashtableUserUpdate.Add(userName, value);
                    }
                    else
                    {
                        var num = TranslateUtils.ToInt(_yHashtableUserUpdate[userName].ToString());
                        _yHashtableUserUpdate[userName] = num + value;
                    }
                    SetVerticalUser(YTypeUpdate, value);
                    break;
            }
            SetHorizentalUser(userName, value);
        }

        private string GetYHashtableUser(string userName, string yType)
        {
            switch (yType)
            {
                case YTypeNew:
                    if (_yHashtableUserNew.ContainsKey(userName))
                    {
                        var num = TranslateUtils.ToInt(_yHashtableUserNew[userName].ToString());
                        return num.ToString();
                    }
                    return "0";
                case YTypeUpdate:
                    if (_yHashtableUserUpdate.ContainsKey(userName))
                    {
                        var num = TranslateUtils.ToInt(_yHashtableUserUpdate[userName].ToString());
                        return num.ToString();
                    }
                    return "0";

                default:
                    return "0";
            }
        }

        private void SetHorizentalUser(string userName, int num)
        {
            if (_horizentalHashtableUser[userName] == null)
            {
                _horizentalHashtableUser[userName] = num;
            }
            else
            {
                var totalNum = (int)_horizentalHashtableUser[userName];
                _horizentalHashtableUser[userName] = totalNum + num;
            }
        }

        private void SetVerticalUser(string type, int num)
        {
            if (_verticalHashtableUser[type] == null)
            {
                _verticalHashtableUser[type] = num;
            }
            else
            {
                var totalNum = (int)_verticalHashtableUser[type];
                _verticalHashtableUser[type] = totalNum + num;
            }
        }
    }
}
