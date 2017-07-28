using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI.WebControls;
using BaiRong.Controls;
using BaiRong.Core;
using SiteServer.WeiXin.Core;
using SiteServer.WeiXin.Model;


namespace SiteServer.WeiXin.BackgroundPages
{
    public class BackgroundVoteLog : BackgroundBasePageWX
    {
        public Repeater rptContents;
        public SqlPager spContents;

        public Button btnDelete;
        public Button btnReturn;

        private Dictionary<int, string> idTitleMap;
        private int voteID;
        private string returnUrl;

        public static string GetRedirectUrl(int publishmentSystemID, int voteID, string returnUrl)
        {
            return PageUtils.GetWXUrl(
                $"background_voteLog.aspx?publishmentSystemID={publishmentSystemID}&voteID={voteID}&returnUrl={StringUtils.ValueToUrl(returnUrl)}");
        }

        public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;
            voteID = TranslateUtils.ToInt(Request.QueryString["voteID"]);
            returnUrl = StringUtils.ValueFromUrl(Request.QueryString["returnUrl"]);

            if (!string.IsNullOrEmpty(Request.QueryString["Delete"]))
            {
                var list = TranslateUtils.StringCollectionToIntList(Request.QueryString["IDCollection"]);
                if (list.Count > 0)
                {
                    try
                    {
                        DataProviderWX.VoteLogDAO.Delete(list);//删除投票记录
                        var voteLogInfoList = new List<VoteLogInfo>();
                        voteLogInfoList = DataProviderWX.VoteLogDAO.GetVoteLogInfoListByVoteID(PublishmentSystemID, voteID);//根据投票编号获取投票记录表中所有投票记录集合
                        var voteLogCollection = string.Empty;
                        var allCount = 0;
                        if (voteLogInfoList != null && voteLogInfoList.Count > 0)
                        {
                            allCount = voteLogInfoList.Count;
                            foreach (var vlist in voteLogInfoList)
                            {
                                voteLogCollection = voteLogCollection + vlist.ItemIDCollection + ",";//获取该次投票的所有的投票项目并拼接字符串
                            }
                            var strlength = voteLogCollection.Length;
                            voteLogCollection = voteLogCollection.Substring(0, strlength - 1);

                            var dict = new Dictionary<int, int>();
                            var arr = voteLogCollection.Split(',');
                            for (var i = 0; i < arr.Length; i++)
                            {
                                if (dict.ContainsKey(TranslateUtils.ToInt(arr[i])))
                                {
                                    dict[TranslateUtils.ToInt(arr[i])] += 1;//统计该次投票的每个项目的投票次数，重复的投票次数增1
                                }
                                else
                                {
                                    dict[TranslateUtils.ToInt(arr[i])] = 1;
                                }
                            }
                            var otherItemList = new List<int>();
                            foreach (var item in dict)
                            {
                                otherItemList.Add(TranslateUtils.ToInt(item.Key.ToString()));
                                DataProviderWX.VoteItemDAO.UpdateVoteNumByID(TranslateUtils.ToInt(item.Value.ToString()), TranslateUtils.ToInt(item.Key.ToString()));//修改该次投票的每个项目的投票次数
                            }
                            DataProviderWX.VoteItemDAO.UpdateOtherVoteNumByIDList(otherItemList, 0, voteID);
                            DataProviderWX.VoteDAO.UpdateUserCountByID(allCount, voteID);//修改该次投票的总投票次数

                        }
                        else
                        {
                            DataProviderWX.VoteItemDAO.UpdateAllVoteNumByVoteID(allCount, voteID);//修改该次投票的每个项目的投票次数为0
                            DataProviderWX.VoteDAO.UpdateUserCountByID(allCount, voteID);//修改该次投票的总投票次数为0
                        }


                        SuccessMessage("删除成功！");
                    }
                    catch (Exception ex)
                    {
                        FailMessage(ex, "删除失败！");
                    }
                }
            }

            spContents.ControlToPaginate = rptContents;
            spContents.ItemsPerPage = 30;
            spContents.ConnectionString = BaiRongDataProvider.ConnectionString;
            spContents.SelectCommand = DataProviderWX.VoteLogDAO.GetSelectString(voteID);
            spContents.SortField = VoteLogAttribute.ID;
            spContents.SortMode = SortMode.ASC;
            rptContents.ItemDataBound += new RepeaterItemEventHandler(rptContents_ItemDataBound);

            if (!IsPostBack)
            {

                BreadCrumb(AppManager.WeiXin.LeftMenu.ID_Function, AppManager.WeiXin.LeftMenu.Function.ID_Vote, "投票记录设置", AppManager.WeiXin.Permission.WebSite.Vote);
                var itemInfoList = DataProviderWX.VoteItemDAO.GetVoteItemInfoList(voteID);
                idTitleMap = new Dictionary<int, string>();
                foreach (var itemInfo in itemInfoList)
                {
                    idTitleMap[itemInfo.ID] = itemInfo.Title;
                }

                spContents.DataBind();

                var urlDelete = PageUtils.AddQueryString(GetRedirectUrl(PublishmentSystemID, voteID, returnUrl), "Delete", "True");
                btnDelete.Attributes.Add("onclick", JsUtils.GetRedirectStringWithCheckBoxValueAndAlert(urlDelete, "IDCollection", "IDCollection", "请选择需要删除的投票项", "此操作将删除所选投票项，确认吗？"));
                btnReturn.Attributes.Add("onclick", $"location.href='{returnUrl}';return false");
            }
        }

        void rptContents_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                var logInfo = new VoteLogInfo(e.Item.DataItem);

                var ltlItemIndex = e.Item.FindControl("ltlItemIndex") as Literal;
                var ltlItemIDCollection = e.Item.FindControl("ltlItemIDCollection") as Literal;
                var ltlUserName = e.Item.FindControl("ltlUserName") as Literal;
                var ltlIPAddress = e.Item.FindControl("ltlIPAddress") as Literal;
                var ltlWXOpenID = e.Item.FindControl("ltlWXOpenID") as Literal;
                var ltlAddDate = e.Item.FindControl("ltlAddDate") as Literal;

                ltlItemIndex.Text = (e.Item.ItemIndex + 1).ToString();

                var builder = new StringBuilder();
                foreach (var itemID in TranslateUtils.StringCollectionToIntList(logInfo.ItemIDCollection))
                {
                    if (idTitleMap.ContainsKey(itemID))
                    {
                        builder.Append(idTitleMap[itemID]).Append(",");
                    }
                }
                if (builder.Length > 0) builder.Length -= 1;
                ltlItemIDCollection.Text = builder.ToString();
                ltlUserName.Text = logInfo.UserName;
                ltlIPAddress.Text = logInfo.IPAddress;
                ltlWXOpenID.Text = logInfo.WXOpenID;
                ltlAddDate.Text = DateUtils.GetDateAndTimeString(logInfo.AddDate);
            }
        }

    }
}
