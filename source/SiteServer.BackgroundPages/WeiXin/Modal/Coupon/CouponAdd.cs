using System;
using System.Web.UI.WebControls;
using System.Collections.Specialized;

using BaiRong.Core;
using SiteServer.CMS.BackgroundPages;
using SiteServer.WeiXin.Core;
using SiteServer.WeiXin.Model;
using SiteServer.CMS.Core;

namespace SiteServer.WeiXin.BackgroundPages.Modal
{
    public class CouponAdd : BackgroundBasePage
    {
        public TextBox tbTitle;
        public TextBox tbTotalNum;
        public CheckBox cbIsEnabled;

        private int couponID;

        public static string GetOpenWindowStringToAdd(int publishmentSystemID)
        {
            var arguments = new NameValueCollection();
            arguments.Add("publishmentSystemID", publishmentSystemID.ToString());
            return PageUtilityWX.GetOpenWindowString("添加优惠劵", "modal_couponAdd.aspx", arguments, 400, 300);
        }

        public static string GetOpenWindowStringToEdit(int publishmentSystemID, int couponID)
        {
            var arguments = new NameValueCollection();
            arguments.Add("publishmentSystemID", publishmentSystemID.ToString());
            arguments.Add("couponID", couponID.ToString());
            return PageUtilityWX.GetOpenWindowString("编辑优惠劵", "modal_couponAdd.aspx", arguments, 400, 300);
        }

        public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            couponID = TranslateUtils.ToInt(GetQueryString("couponID"));

            if (!IsPostBack)
            {
                if (couponID > 0)
                {
                    var couponInfo = DataProviderWX.CouponDAO.GetCouponInfo(couponID);

                    tbTitle.Text = couponInfo.Title;
                    tbTotalNum.Text = couponInfo.TotalNum.ToString();
                    tbTotalNum.Enabled = false;
                }
            }
        }

        public override void Submit_OnClick(object sender, EventArgs e)
        {
            var isChanged = false;

            try
            {
                if (this.couponID == 0)
                {
                    var totalNum = TranslateUtils.ToInt(tbTotalNum.Text);

                    if (totalNum > 1000)
                    {
                        FailMessage("添加失败，一次最多只能新增1000张优惠劵");
                    }
                    else
                    {
                        var couponInfo = new CouponInfo { ID = 0, PublishmentSystemID = PublishmentSystemID, ActID = 0, Title = tbTitle.Text, TotalNum = totalNum, AddDate = DateTime.Now };

                        var couponID = DataProviderWX.CouponDAO.Insert(couponInfo);

                        if (cbIsEnabled.Checked == false)
                        {
                            DataProviderWX.CouponSNDAO.Insert(PublishmentSystemID, couponID, totalNum);
                        }
                        StringUtility.AddLog(PublishmentSystemID, "添加优惠劵", $"优惠劵:{tbTitle.Text}");

                        isChanged = true;
                    }
                }
                else
                {
                    var couponInfo = DataProviderWX.CouponDAO.GetCouponInfo(couponID);
                    couponInfo.Title = tbTitle.Text;

                    DataProviderWX.CouponDAO.Update(couponInfo);

                    StringUtility.AddLog(PublishmentSystemID, "编辑优惠劵", $"优惠劵:{tbTitle.Text}");

                    isChanged = true;
                }
            }
            catch (Exception ex)
            {
                FailMessage(ex, "失败：" + ex.Message);
            }

            if (isChanged)
            {
                JsUtils.OpenWindow.CloseModalPage(Page);
            }
        }
    }
}
