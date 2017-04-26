using System;
using System.Collections;
using System.Collections.Specialized;
using System.Web.UI.WebControls;
using BaiRong.Core;
using SiteServer.CMS.Core;

namespace SiteServer.BackgroundPages.Cms
{
	public class ModalTrackerIpView : BasePageCms
    {
        public DataList MyDataList;
        protected Button ExportAndDelete;
        protected Button Export;

        private string _startDateString;
        private string _endDateString;
        private int _nodeId;
        private int _contentId;
        private int _totalNum;
        private int _totalAccessNum;

        public static string GetOpenWindowString(string startDateString, string endDateString, int publishmentSystemId, int nodeId, int contentId, int totalNum)
        {
            return PageUtils.GetOpenWindowString("内容点击详细记录", PageUtils.GetCmsUrl(nameof(ModalTrackerIpView), new NameValueCollection
            {
                {"StartDateString", startDateString},
                {"EndDateString", endDateString},
                {"PublishmentSystemID", publishmentSystemId.ToString()},
                {"NodeID", nodeId.ToString()},
                {"ContentID", contentId.ToString()},
                {"TotalNum", totalNum.ToString()}
            }), 580, 520, true);
        }

		public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            _startDateString = Body.GetQueryString("StartDateString");
            _endDateString = Body.GetQueryString("EndDateString");
            _nodeId = Body.GetQueryInt("NodeID");
            _contentId = Body.GetQueryInt("ContentID");
            _totalNum = Body.GetQueryInt("TotalNum");

            if (_nodeId == PublishmentSystemId)
            {
                ExportAndDelete.Visible = true;
            }

			if (!IsPostBack)
			{
                var begin = DateUtils.SqlMinValue;
                if (!string.IsNullOrEmpty(_startDateString))
                {
                    begin = TranslateUtils.ToDateTime(_startDateString);
                }
                var end = TranslateUtils.ToDateTime(_endDateString);

                if (!string.IsNullOrEmpty(_startDateString))
                {
                    InfoMessage(
                        $"开始时间：{_startDateString}&nbsp;&nbsp;结束时间：{_endDateString}&nbsp;&nbsp;总访问量：{_totalNum}");
                }
                else
                {
                    InfoMessage($"结束时间：{_endDateString}&nbsp;&nbsp;总访问量：{_totalNum}");
                }

                var ipAddresses = DataProvider.TrackingDao.GetContentIpAddressArrayList(PublishmentSystemId, _nodeId, _contentId, begin, end);
                var ipAddressWithNumSortedList = new SortedList();
                foreach (string ipAddress in ipAddresses)
                {
                    if (ipAddressWithNumSortedList[ipAddress] != null)
                    {
                        ipAddressWithNumSortedList[ipAddress] = (int)ipAddressWithNumSortedList[ipAddress] + 1;
                    }
                    else
                    {
                        ipAddressWithNumSortedList[ipAddress] = 1;
                    }
                }

                foreach (string ipAddress in ipAddressWithNumSortedList.Keys)
                {
                    var accessNum = 0;
                    if (ipAddressWithNumSortedList[ipAddress] != null)
                    {
                        accessNum = Convert.ToInt32(ipAddressWithNumSortedList[ipAddress]);
                    }
                    _totalAccessNum += accessNum;
                }

                MyDataList.DataSource = ipAddressWithNumSortedList;
                MyDataList.ItemDataBound += MyDataList_ItemDataBound;
                MyDataList.DataBind();
			}
		}

        public void Export_Click(object sender, EventArgs e)
        {
            var redirectUrl = ModalExportMessage.GetRedirectUrlStringToExportTracker(_startDateString, _endDateString, PublishmentSystemId, _nodeId, _contentId, _totalNum, false);
            PageUtils.Redirect(redirectUrl);
        }

        public void ExportAndDelete_Click(object sender, EventArgs e)
        {
            var redirectUrl = ModalExportMessage.GetRedirectUrlStringToExportTracker(_startDateString, _endDateString, PublishmentSystemId, _nodeId, _contentId, _totalNum, true);
            PageUtils.Redirect(redirectUrl);
        }

        void MyDataList_ItemDataBound(object sender, DataListItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                var entry = (DictionaryEntry)e.Item.DataItem;

                var ltlItemTitle = (Literal)e.Item.FindControl("ltlItemTitle");
                var ltlAccessNumBar = (Literal)e.Item.FindControl("ltlAccessNumBar");
                var ltlItemCount = (Literal)e.Item.FindControl("ltlItemCount");

                var accessNum = Convert.ToInt32(entry.Value);

                ltlItemTitle.Text = entry.Key.ToString();
                ltlAccessNumBar.Text = $@"<div class=""progress progress-success progress-striped"">
            <div class=""bar"" style=""width: {GetAccessNumBarWidth(accessNum)}%""></div>
          </div>";
                ltlItemCount.Text = accessNum.ToString();
            }
        }

        private double GetAccessNumBarWidth(int accessNum)
        {
            double width = 0;
            if (_totalAccessNum > 0)
            {
                width = Convert.ToDouble(accessNum) / Convert.ToDouble(_totalAccessNum);
                width = Math.Round(width, 2) * 200;
            }
            return width;
        }
	}
}
